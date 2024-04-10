namespace MVSCamera
{
    public partial class Form1 : Form
    {
        readonly MVS mvs = new();

        public Form1()
        {
            InitializeComponent();

            mvs.ErrorMsg += ShowError;
        }

        private void ShowError(string msg)
        {
            TBMessage.Invoke(new Action(() => TBMessage.Text = msg));
        }

        private void BTN查找设备_Click(object sender, EventArgs e)
        {
            mvs.GetDeviceList();
            CB设备列表.Items.Clear();
            foreach (var item in mvs.DeviceNames)
            {
                CB设备列表.Items.Add(item);
            }
        }

        private void TSM打开设备_Click(object sender, EventArgs e)
        {
            if (mvs.DeviceList.nDeviceNum == 0 || CB设备列表.SelectedIndex == -1)
            {
                ShowError("No device, please select");
                return;
            }
            mvs.OpenCamera(mvs.DeviceList.pDeviceInfo[CB设备列表.SelectedIndex]);
        }

        private void TSM关闭设备_Click(object sender, EventArgs e)
        {
            mvs.CloseCamera();
        }

        private void TSM开始采集_Click(object sender, EventArgs e)
        {
            mvs.StartGrab();
        }

        private void TSM停止采集_Click(object sender, EventArgs e)
        {
            mvs.StopGrab();
        }

        private void TSM显示图片_Click(object sender, EventArgs e)
        {
            mvs.Display(PB图片.Handle);
        }
    }
}