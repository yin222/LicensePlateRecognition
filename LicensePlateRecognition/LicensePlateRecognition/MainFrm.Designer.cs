namespace LicensePlateRecognition
{
    partial class MainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.LicensePlateImg = new System.Windows.Forms.PictureBox();
            this.speedLbl = new System.Windows.Forms.Label();
            this.openImageBtn = new CCWin.SkinControl.SkinButton();
            this.beginBtn = new CCWin.SkinControl.SkinButton();
            this.txtResult = new CCWin.SkinControl.SkinTextBox();
            this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
            this.btnBegin2 = new CCWin.SkinControl.SkinButton();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            ((System.ComponentModel.ISupportInitialize)(this.LicensePlateImg)).BeginInit();
            this.skinGroupBox1.SuspendLayout();
            this.skinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LicensePlateImg
            // 
            this.LicensePlateImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LicensePlateImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LicensePlateImg.Dock = System.Windows.Forms.DockStyle.Top;
            this.LicensePlateImg.Location = new System.Drawing.Point(4, 32);
            this.LicensePlateImg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LicensePlateImg.Name = "LicensePlateImg";
            this.LicensePlateImg.Size = new System.Drawing.Size(1143, 297);
            this.LicensePlateImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LicensePlateImg.TabIndex = 0;
            this.LicensePlateImg.TabStop = false;
            // 
            // speedLbl
            // 
            this.speedLbl.Location = new System.Drawing.Point(136, 445);
            this.speedLbl.Name = "speedLbl";
            this.speedLbl.Size = new System.Drawing.Size(224, 28);
            this.speedLbl.TabIndex = 4;
            this.speedLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openImageBtn
            // 
            this.openImageBtn.BackColor = System.Drawing.Color.Transparent;
            this.openImageBtn.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.openImageBtn.DownBack = null;
            this.openImageBtn.Location = new System.Drawing.Point(159, 11);
            this.openImageBtn.Margin = new System.Windows.Forms.Padding(4);
            this.openImageBtn.MouseBack = null;
            this.openImageBtn.Name = "openImageBtn";
            this.openImageBtn.NormlBack = null;
            this.openImageBtn.Size = new System.Drawing.Size(151, 65);
            this.openImageBtn.TabIndex = 8;
            this.openImageBtn.Text = "选择文件";
            this.openImageBtn.UseVisualStyleBackColor = false;
            this.openImageBtn.Click += new System.EventHandler(this.openImageBtn_Click);
            // 
            // beginBtn
            // 
            this.beginBtn.BackColor = System.Drawing.Color.Transparent;
            this.beginBtn.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.beginBtn.DownBack = null;
            this.beginBtn.Location = new System.Drawing.Point(476, 11);
            this.beginBtn.Margin = new System.Windows.Forms.Padding(4);
            this.beginBtn.MouseBack = null;
            this.beginBtn.Name = "beginBtn";
            this.beginBtn.NormlBack = null;
            this.beginBtn.Size = new System.Drawing.Size(151, 65);
            this.beginBtn.TabIndex = 9;
            this.beginBtn.Text = "识别一";
            this.beginBtn.UseVisualStyleBackColor = false;
            this.beginBtn.Click += new System.EventHandler(this.beginBtn_Click);
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.Color.Transparent;
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.DownBack = null;
            this.txtResult.Enabled = false;
            this.txtResult.Icon = null;
            this.txtResult.IconIsButton = false;
            this.txtResult.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.txtResult.IsPasswordChat = '\0';
            this.txtResult.IsSystemPasswordChar = false;
            this.txtResult.Lines = new string[0];
            this.txtResult.Location = new System.Drawing.Point(4, 22);
            this.txtResult.Margin = new System.Windows.Forms.Padding(0);
            this.txtResult.MaxLength = 32767;
            this.txtResult.MinimumSize = new System.Drawing.Size(37, 35);
            this.txtResult.MouseBack = null;
            this.txtResult.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.NormlBack = null;
            this.txtResult.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.txtResult.ReadOnly = false;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtResult.Size = new System.Drawing.Size(1135, 180);
            // 
            // 
            // 
            this.txtResult.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtResult.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.txtResult.SkinTxt.Location = new System.Drawing.Point(7, 6);
            this.txtResult.SkinTxt.Margin = new System.Windows.Forms.Padding(4);
            this.txtResult.SkinTxt.Multiline = true;
            this.txtResult.SkinTxt.Name = "BaseText";
            this.txtResult.SkinTxt.Size = new System.Drawing.Size(1121, 168);
            this.txtResult.SkinTxt.TabIndex = 0;
            this.txtResult.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.txtResult.SkinTxt.WaterText = "";
            this.txtResult.TabIndex = 10;
            this.txtResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtResult.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.txtResult.WaterText = "";
            this.txtResult.WordWrap = true;
            // 
            // skinGroupBox1
            // 
            this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Controls.Add(this.txtResult);
            this.skinGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.skinGroupBox1.ForeColor = System.Drawing.Color.Blue;
            this.skinGroupBox1.Location = new System.Drawing.Point(4, 329);
            this.skinGroupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.skinGroupBox1.Name = "skinGroupBox1";
            this.skinGroupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.skinGroupBox1.RectBackColor = System.Drawing.Color.White;
            this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox1.Size = new System.Drawing.Size(1143, 206);
            this.skinGroupBox1.TabIndex = 11;
            this.skinGroupBox1.TabStop = false;
            this.skinGroupBox1.Text = "识别结果:";
            this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.White;
            this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // btnBegin2
            // 
            this.btnBegin2.BackColor = System.Drawing.Color.Transparent;
            this.btnBegin2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnBegin2.DownBack = null;
            this.btnBegin2.Location = new System.Drawing.Point(832, 11);
            this.btnBegin2.Margin = new System.Windows.Forms.Padding(4);
            this.btnBegin2.MouseBack = null;
            this.btnBegin2.Name = "btnBegin2";
            this.btnBegin2.NormlBack = null;
            this.btnBegin2.Size = new System.Drawing.Size(151, 65);
            this.btnBegin2.TabIndex = 12;
            this.btnBegin2.Text = "识别二";
            this.btnBegin2.UseVisualStyleBackColor = false;
            this.btnBegin2.Click += new System.EventHandler(this.btnBegin2_Click);
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel1.Controls.Add(this.openImageBtn);
            this.skinPanel1.Controls.Add(this.btnBegin2);
            this.skinPanel1.Controls.Add(this.beginBtn);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(4, 552);
            this.skinPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Size = new System.Drawing.Size(1143, 104);
            this.skinPanel1.TabIndex = 13;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1151, 660);
            this.Controls.Add(this.skinPanel1);
            this.Controls.Add(this.skinGroupBox1);
            this.Controls.Add(this.speedLbl);
            this.Controls.Add(this.LicensePlateImg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainFrm";
            this.Text = "车牌识别";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainFrm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainFrm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainFrm_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.LicensePlateImg)).EndInit();
            this.skinGroupBox1.ResumeLayout(false);
            this.skinPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox LicensePlateImg;
        private System.Windows.Forms.Label speedLbl;
        private CCWin.SkinControl.SkinButton openImageBtn;
        private CCWin.SkinControl.SkinButton beginBtn;
        private CCWin.SkinControl.SkinTextBox txtResult;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox1;
        private CCWin.SkinControl.SkinButton btnBegin2;
        private CCWin.SkinControl.SkinPanel skinPanel1;
    }
}

