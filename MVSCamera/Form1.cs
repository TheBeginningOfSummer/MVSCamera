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

        private void BTN�����豸_Click(object sender, EventArgs e)
        {
            mvs.GetDeviceList();
            CB�豸�б�.Items.Clear();
            foreach (var item in mvs.DeviceNames)
            {
                CB�豸�б�.Items.Add(item);
            }
        }

        private void TSM���豸_Click(object sender, EventArgs e)
        {
            if (mvs.DeviceList.nDeviceNum == 0 || CB�豸�б�.SelectedIndex == -1)
            {
                ShowError("No device, please select");
                return;
            }
            mvs.OpenCamera(mvs.DeviceList.pDeviceInfo[CB�豸�б�.SelectedIndex]);
        }

        private void TSM�ر��豸_Click(object sender, EventArgs e)
        {
            mvs.CloseCamera();
        }

        private void TSM��ʼ�ɼ�_Click(object sender, EventArgs e)
        {
            mvs.StartGrab();
        }

        private void TSMֹͣ�ɼ�_Click(object sender, EventArgs e)
        {
            mvs.StopGrab();
        }

        private void TSM��ʾͼƬ_Click(object sender, EventArgs e)
        {
            mvs.Display(PBͼƬ.Handle);
        }
    }
}