using MvCamCtrl.NET;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MVSCamera
{
    public class MVS
    {
        //设备列表
        public MyCamera.MV_CC_DEVICE_INFO_LIST DeviceList;
        //设备名称
        public List<string> DeviceNames = new();
        //当前相机
        public MyCamera? CurrentCamera;
        //错误信息
        public Action<string>? ErrorMsg;

        #region 图片数据存放
        uint bufferSize = 0;
        byte[] bufferForDriver = new byte[4096];
        uint imageSize = 0;
        byte[] imageBytes = new byte[4096];
        #endregion

        public MVS() {
            
        }

        public void GetDeviceList()
        {
            int nRet;
            // ch:创建设备列表 en:Create Device List
            GC.Collect();
            DeviceNames.Clear();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref DeviceList);
            if (nRet != 0)
            {
                ErrorMsg?.Invoke("Enumerate devices fail!");
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < DeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO? deviceTemp = (MyCamera.MV_CC_DEVICE_INFO?)Marshal.PtrToStructure(DeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (deviceTemp == null) return;
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)deviceTemp;
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO? gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO?)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo?.chUserDefinedName != "")
                    {
                        DeviceNames.Add("GigE: " + gigeInfo?.chUserDefinedName + " (" + gigeInfo?.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        DeviceNames.Add("GigE: " + gigeInfo?.chManufacturerName + " " + gigeInfo?.chModelName + " (" + gigeInfo?.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO? usbInfo = (MyCamera.MV_USB3_DEVICE_INFO?)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo?.chUserDefinedName != "")
                    {
                        DeviceNames.Add("USB: " + usbInfo?.chUserDefinedName + " (" + usbInfo?.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        DeviceNames.Add("USB: " + usbInfo?.chManufacturerName + " " + usbInfo?.chModelName + " (" + usbInfo?.chSerialNumber + ")");
                        //cbDeviceList.Items.Add("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }
        }

        public void OpenCamera(IntPtr cameraInfo)
        {
            if (DeviceList.nDeviceNum == 0)
            {
                ErrorMsg?.Invoke("No device, please select");
                return;
            }
            int nRet = -1;
            MyCamera.MV_CC_DEVICE_INFO? deviceTemp = (MyCamera.MV_CC_DEVICE_INFO?)Marshal.
                PtrToStructure(cameraInfo, typeof(MyCamera.MV_CC_DEVICE_INFO));
            if (deviceTemp == null) return;
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)deviceTemp;
            CurrentCamera = new MyCamera();
            nRet = CurrentCamera.MV_CC_CreateDevice_NET(ref device);
            if (nRet != MyCamera.MV_OK) return;
            nRet = CurrentCamera.MV_CC_OpenDevice_NET();
            if (nRet != MyCamera.MV_OK)
            {
                CurrentCamera.MV_CC_DestroyDevice_NET();
                ErrorMsg?.Invoke("Device open fail!");
                return;
            }

            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = CurrentCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = CurrentCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            CurrentCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
            CurrentCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);    // ch:连续模式 | en:Continuous
        }

        public void CloseCamera()
        {
            // ch:关闭设备 | en:Close Device
            if (CurrentCamera == null) return;
            int nRet = CurrentCamera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }
            nRet = CurrentCamera.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }
        }

        public void StartGrab()
        {
            if (CurrentCamera == null) return;
            // ch:开始采集 | en:Start Grabbing
            int nRet = CurrentCamera.MV_CC_StartGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ErrorMsg?.Invoke("Grab Fail!");
                return;
            }

            // ch:标志位置位true | en:Set position bit true
            //m_bGrabbing = true;

            // ch:显示 | en:Display
            //nRet = m_pMyCamera.MV_CC_Display_NET(pictureBox1.Handle);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    ShowErrorMsg("Display Fail！", nRet);
            //}
        }

        public void Display(IntPtr window)
        {
            // ch:显示 | en:Display
            if (CurrentCamera == null) return;
            int nRet = CurrentCamera.MV_CC_Display_NET(window);
            if (MyCamera.MV_OK != nRet)
            {
                ErrorMsg?.Invoke("Display Fail！");
            }
        }

        public void StopGrab()
        {
            if (CurrentCamera == null) return;
            // ch:停止采集 | en:Stop Grabbing
            int nRet = CurrentCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ErrorMsg?.Invoke("Stop Grabbing Fail!");
            }
        }

        private static bool IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }

        /************************************************************************
         *  @fn     IsColorData()
         *  @brief  判断是否是彩色数据
         *  @param  enGvspPixelType         [IN]           像素格式
         *  @return 成功，返回0；错误，返回-1 
         ************************************************************************/
        private static bool IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }

        public Bitmap? GetBmp()
        {
            #region 获取图片大小申请字节数组大小
            if (CurrentCamera == null) return default;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = CurrentCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (nRet != MyCamera.MV_OK)
            {
                ErrorMsg?.Invoke("Get PayloadSize failed");
                return default;
            }
            uint nPayloadSize = stParam.nCurValue;
            //IntPtr pBufForDriver = Marshal.AllocHGlobal((int)nPayloadSize);
            if (nPayloadSize > bufferSize)
            {
                bufferSize = nPayloadSize;
                bufferForDriver = new byte[bufferSize];

                // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                imageSize = bufferSize * 3 + 2048;
                imageBytes = new byte[imageSize];
            }
            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(bufferForDriver, 0);
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            // ch:超时获取一帧，超时时间为1秒 | en:Get one frame timeout, timeout is 1 sec
            nRet = CurrentCamera.MV_CC_GetOneFrameTimeout_NET(pData, bufferSize, ref stFrameInfo, 1000);
            if (nRet != MyCamera.MV_OK)
            {
                ErrorMsg?.Invoke("No Data!");
                return default;
            }
            #endregion
            #region 判断图片种类
            MyCamera.MvGvspPixelType enDstPixelType;
            if (IsMonoData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            }
            else if (IsColorData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            }
            else
            {
                ErrorMsg?.Invoke("No such pixel type!");
                return default;
            }
            #endregion
            #region 像素格式转换
            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(imageBytes, 0);
            //MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            stConverPixelParam.nWidth = stFrameInfo.nWidth;
            stConverPixelParam.nHeight = stFrameInfo.nHeight;
            stConverPixelParam.pSrcData = pData;
            stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
            stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
            stConverPixelParam.enDstPixelType = enDstPixelType;
            stConverPixelParam.pDstBuffer = pImage;
            stConverPixelParam.nDstBufferSize = imageSize;
            nRet = CurrentCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (nRet != MyCamera.MV_OK) return default;
            #endregion
            #region 建立图片数据
            if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            {
                //************************Mono8 转 Bitmap*******************************
                Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pImage);
                ColorPalette cp = bmp.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }
                // set palette back
                bmp.Palette = cp;
                return bmp;
                //bmp.Save("image.bmp", ImageFormat.Bmp);
            }
            else
            {
                //*********************RGB8 转 Bitmap**************************
                for (int i = 0; i < stFrameInfo.nHeight; i++)
                {
                    for (int j = 0; j < stFrameInfo.nWidth; j++)
                    {
                        byte chRed = imageBytes[i * stFrameInfo.nWidth * 3 + j * 3];
                        imageBytes[i * stFrameInfo.nWidth * 3 + j * 3] = imageBytes[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                        imageBytes[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                    }
                }
                Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pImage);
                return bmp;
                //bmp.Save("image.bmp", ImageFormat.Bmp);
            }
            #endregion
        }

        public void GetParam()
        {
            if (CurrentCamera == null) return;
            MyCamera.MVCC_FLOATVALUE stParam = new();
            int nRet = CurrentCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            nRet = CurrentCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            nRet = CurrentCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
            string s = stParam.fCurValue.ToString("F1");
        }

        public void SetParam(float exposureTime, float gain, float frameRate)
        {
            int nRet;
            if (CurrentCamera == null) return;
            //曝光
            CurrentCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
            nRet = CurrentCamera.MV_CC_SetFloatValue_NET("ExposureTime", exposureTime);
            //增益
            CurrentCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
            nRet = CurrentCamera.MV_CC_SetFloatValue_NET("Gain", gain);
            //帧率
            nRet = CurrentCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", frameRate);
        }

        public void TriggerMode(bool isTrigger = false, uint triggerSource = 7)
        {
            if (CurrentCamera == null) return;
            if (isTrigger)
            {
                CurrentCamera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
                CurrentCamera.MV_CC_SetEnumValue_NET("TriggerSource", triggerSource);
            }
            else
            {
                CurrentCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);
            }
        }

        public void Trigger()
        {
            if (CurrentCamera == null) return;
            // ch:触发命令 | en:Trigger command
            int nRet = CurrentCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                ErrorMsg?.Invoke("Trigger Fail!");
            }
        }

    }
}
