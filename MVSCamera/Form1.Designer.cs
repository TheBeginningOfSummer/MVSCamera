namespace MVSCamera
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            CB设备列表 = new ComboBox();
            PB图片 = new PictureBox();
            BTN查找设备 = new Button();
            MS菜单栏 = new MenuStrip();
            TSM设置 = new ToolStripMenuItem();
            TSM设备 = new ToolStripMenuItem();
            TSM打开设备 = new ToolStripMenuItem();
            TSM关闭设备 = new ToolStripMenuItem();
            TSM显示图片 = new ToolStripMenuItem();
            TSM采集 = new ToolStripMenuItem();
            TSM开始采集 = new ToolStripMenuItem();
            TSM停止采集 = new ToolStripMenuItem();
            TBMessage = new TextBox();
            ((System.ComponentModel.ISupportInitialize)PB图片).BeginInit();
            MS菜单栏.SuspendLayout();
            SuspendLayout();
            // 
            // CB设备列表
            // 
            CB设备列表.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CB设备列表.FormattingEnabled = true;
            CB设备列表.Location = new Point(12, 28);
            CB设备列表.Name = "CB设备列表";
            CB设备列表.Size = new Size(610, 25);
            CB设备列表.TabIndex = 0;
            // 
            // PB图片
            // 
            PB图片.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PB图片.Location = new Point(12, 59);
            PB图片.Name = "PB图片";
            PB图片.Size = new Size(610, 348);
            PB图片.TabIndex = 1;
            PB图片.TabStop = false;
            // 
            // BTN查找设备
            // 
            BTN查找设备.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN查找设备.Location = new Point(628, 28);
            BTN查找设备.Name = "BTN查找设备";
            BTN查找设备.Size = new Size(144, 25);
            BTN查找设备.TabIndex = 2;
            BTN查找设备.Text = "查找设备";
            BTN查找设备.UseVisualStyleBackColor = true;
            BTN查找设备.Click += BTN查找设备_Click;
            // 
            // MS菜单栏
            // 
            MS菜单栏.Items.AddRange(new ToolStripItem[] { TSM设置, TSM设备, TSM采集 });
            MS菜单栏.Location = new Point(0, 0);
            MS菜单栏.Name = "MS菜单栏";
            MS菜单栏.Size = new Size(784, 25);
            MS菜单栏.TabIndex = 4;
            MS菜单栏.Text = "menuStrip1";
            // 
            // TSM设置
            // 
            TSM设置.Alignment = ToolStripItemAlignment.Right;
            TSM设置.Name = "TSM设置";
            TSM设置.Size = new Size(44, 21);
            TSM设置.Text = "设置";
            // 
            // TSM设备
            // 
            TSM设备.Alignment = ToolStripItemAlignment.Right;
            TSM设备.DropDownItems.AddRange(new ToolStripItem[] { TSM打开设备, TSM关闭设备, TSM显示图片 });
            TSM设备.Name = "TSM设备";
            TSM设备.Size = new Size(44, 21);
            TSM设备.Text = "设备";
            // 
            // TSM打开设备
            // 
            TSM打开设备.Name = "TSM打开设备";
            TSM打开设备.Size = new Size(180, 22);
            TSM打开设备.Text = "打开设备";
            TSM打开设备.Click += TSM打开设备_Click;
            // 
            // TSM关闭设备
            // 
            TSM关闭设备.Name = "TSM关闭设备";
            TSM关闭设备.Size = new Size(180, 22);
            TSM关闭设备.Text = "关闭设备";
            TSM关闭设备.Click += TSM关闭设备_Click;
            // 
            // TSM显示图片
            // 
            TSM显示图片.Name = "TSM显示图片";
            TSM显示图片.Size = new Size(180, 22);
            TSM显示图片.Text = "显示图片";
            TSM显示图片.Click += TSM显示图片_Click;
            // 
            // TSM采集
            // 
            TSM采集.Alignment = ToolStripItemAlignment.Right;
            TSM采集.DropDownItems.AddRange(new ToolStripItem[] { TSM开始采集, TSM停止采集 });
            TSM采集.Name = "TSM采集";
            TSM采集.Size = new Size(44, 21);
            TSM采集.Text = "采集";
            // 
            // TSM开始采集
            // 
            TSM开始采集.Name = "TSM开始采集";
            TSM开始采集.Size = new Size(180, 22);
            TSM开始采集.Text = "开始采集";
            TSM开始采集.Click += TSM开始采集_Click;
            // 
            // TSM停止采集
            // 
            TSM停止采集.Name = "TSM停止采集";
            TSM停止采集.Size = new Size(180, 22);
            TSM停止采集.Text = "停止采集";
            TSM停止采集.Click += TSM停止采集_Click;
            // 
            // TBMessage
            // 
            TBMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TBMessage.Location = new Point(12, 413);
            TBMessage.Name = "TBMessage";
            TBMessage.ReadOnly = true;
            TBMessage.ScrollBars = ScrollBars.Vertical;
            TBMessage.Size = new Size(610, 23);
            TBMessage.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(TBMessage);
            Controls.Add(BTN查找设备);
            Controls.Add(PB图片);
            Controls.Add(CB设备列表);
            Controls.Add(MS菜单栏);
            MainMenuStrip = MS菜单栏;
            Name = "Form1";
            Text = "海康相机";
            ((System.ComponentModel.ISupportInitialize)PB图片).EndInit();
            MS菜单栏.ResumeLayout(false);
            MS菜单栏.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox CB设备列表;
        private PictureBox PB图片;
        private Button BTN查找设备;
        private MenuStrip MS菜单栏;
        private ToolStripMenuItem TSM设置;
        private ToolStripMenuItem TSM设备;
        private ToolStripMenuItem TSM打开设备;
        private ToolStripMenuItem TSM关闭设备;
        private ToolStripMenuItem TSM采集;
        private ToolStripMenuItem TSM开始采集;
        private ToolStripMenuItem TSM停止采集;
        private ToolStripMenuItem TSM显示图片;
        private TextBox TBMessage;
    }
}