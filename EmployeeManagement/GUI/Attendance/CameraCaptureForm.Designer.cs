namespace EmployeeManagement.GUI.Attendance
{
    partial class CameraCaptureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelMain = new Panel();
            panelPreview = new Panel();
            btnCancel = new Button();
            btnOK = new Button();
            pictureBoxPreview = new PictureBox();
            lblPreview = new Label();
            pictureBoxCamera = new PictureBox();
            panelControls = new Panel();
            lblStatus = new Label();
            btnCapture = new Button();
            btnStopCamera = new Button();
            btnStartCamera = new Button();
            cmbCameras = new ComboBox();
            lblCamera = new Label();
            panelMain.SuspendLayout();
            panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
            panelControls.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.White;
            panelMain.Controls.Add(panelPreview);
            panelMain.Controls.Add(pictureBoxCamera);
            panelMain.Controls.Add(panelControls);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Margin = new Padding(3, 4, 3, 4);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(10, 12, 10, 12);
            panelMain.Size = new Size(800, 812);
            panelMain.TabIndex = 0;
            // 
            // panelPreview
            // 
            panelPreview.BackColor = Color.FromArgb(250, 250, 250);
            panelPreview.BorderStyle = BorderStyle.FixedSingle;
            panelPreview.Controls.Add(btnCancel);
            panelPreview.Controls.Add(btnOK);
            panelPreview.Controls.Add(pictureBoxPreview);
            panelPreview.Controls.Add(lblPreview);
            panelPreview.Location = new Point(510, 125);
            panelPreview.Margin = new Padding(3, 4, 3, 4);
            panelPreview.Name = "panelPreview";
            panelPreview.Size = new Size(260, 450);
            panelPreview.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(140, 301);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 31);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.BackColor = Color.FromArgb(76, 175, 80);
            btnOK.Enabled = false;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnOK.ForeColor = Color.White;
            btnOK.Location = new Point(10, 301);
            btnOK.Margin = new Padding(3, 4, 3, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(110, 31);
            btnOK.TabIndex = 2;
            btnOK.Text = "Sử dụng ảnh này";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += BtnOK_Click;
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BackColor = Color.White;
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(10, 44);
            pictureBoxPreview.Margin = new Padding(3, 4, 3, 4);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(240, 224);
            pictureBoxPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxPreview.TabIndex = 1;
            pictureBoxPreview.TabStop = false;
            // 
            // lblPreview
            // 
            lblPreview.AutoSize = true;
            lblPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPreview.Location = new Point(10, 12);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(97, 20);
            lblPreview.TabIndex = 0;
            lblPreview.Text = "Ảnh đã chụp";
            // 
            // pictureBoxCamera
            // 
            pictureBoxCamera.BackColor = Color.Black;
            pictureBoxCamera.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxCamera.Location = new Point(10, 125);
            pictureBoxCamera.Margin = new Padding(3, 4, 3, 4);
            pictureBoxCamera.Name = "pictureBoxCamera";
            pictureBoxCamera.Size = new Size(480, 450);
            pictureBoxCamera.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxCamera.TabIndex = 1;
            pictureBoxCamera.TabStop = false;
            // 
            // panelControls
            // 
            panelControls.BackColor = Color.FromArgb(245, 245, 245);
            panelControls.Controls.Add(lblStatus);
            panelControls.Controls.Add(btnCapture);
            panelControls.Controls.Add(btnStopCamera);
            panelControls.Controls.Add(btnStartCamera);
            panelControls.Controls.Add(cmbCameras);
            panelControls.Controls.Add(lblCamera);
            panelControls.Location = new Point(10, 12);
            panelControls.Margin = new Padding(3, 4, 3, 4);
            panelControls.Name = "panelControls";
            panelControls.Size = new Size(480, 100);
            panelControls.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.FromArgb(117, 117, 117);
            lblStatus.Location = new Point(120, 56);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(350, 31);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Chọn camera và nhấn 'Bật Camera'";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCapture
            // 
            btnCapture.BackColor = Color.FromArgb(33, 150, 243);
            btnCapture.Enabled = false;
            btnCapture.FlatAppearance.BorderSize = 0;
            btnCapture.FlatStyle = FlatStyle.Flat;
            btnCapture.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCapture.ForeColor = Color.White;
            btnCapture.Location = new Point(10, 56);
            btnCapture.Margin = new Padding(3, 4, 3, 4);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(100, 31);
            btnCapture.TabIndex = 4;
            btnCapture.Text = "Chụp ảnh";
            btnCapture.UseVisualStyleBackColor = false;
            btnCapture.Click += BtnCapture_Click;
            // 
            // btnStopCamera
            // 
            btnStopCamera.BackColor = Color.FromArgb(244, 67, 54);
            btnStopCamera.Enabled = false;
            btnStopCamera.FlatAppearance.BorderSize = 0;
            btnStopCamera.FlatStyle = FlatStyle.Flat;
            btnStopCamera.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStopCamera.ForeColor = Color.White;
            btnStopCamera.Location = new Point(380, 12);
            btnStopCamera.Margin = new Padding(3, 4, 3, 4);
            btnStopCamera.Name = "btnStopCamera";
            btnStopCamera.Size = new Size(85, 38);
            btnStopCamera.TabIndex = 3;
            btnStopCamera.Text = "Tắt Camera";
            btnStopCamera.UseVisualStyleBackColor = false;
            btnStopCamera.Click += BtnStopCamera_Click;
            // 
            // btnStartCamera
            // 
            btnStartCamera.BackColor = Color.FromArgb(76, 175, 80);
            btnStartCamera.FlatAppearance.BorderSize = 0;
            btnStartCamera.FlatStyle = FlatStyle.Flat;
            btnStartCamera.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStartCamera.ForeColor = Color.White;
            btnStartCamera.Location = new Point(285, 12);
            btnStartCamera.Margin = new Padding(3, 4, 3, 4);
            btnStartCamera.Name = "btnStartCamera";
            btnStartCamera.Size = new Size(85, 38);
            btnStartCamera.TabIndex = 2;
            btnStartCamera.Text = "Bật Camera";
            btnStartCamera.UseVisualStyleBackColor = false;
            btnStartCamera.Click += BtnStartCamera_Click;
            // 
            // cmbCameras
            // 
            cmbCameras.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCameras.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbCameras.FormattingEnabled = true;
            cmbCameras.Location = new Point(75, 15);
            cmbCameras.Margin = new Padding(3, 4, 3, 4);
            cmbCameras.Name = "cmbCameras";
            cmbCameras.Size = new Size(200, 28);
            cmbCameras.TabIndex = 1;
            // 
            // lblCamera
            // 
            lblCamera.AutoSize = true;
            lblCamera.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCamera.Location = new Point(10, 19);
            lblCamera.Name = "lblCamera";
            lblCamera.Size = new Size(63, 20);
            lblCamera.TabIndex = 0;
            lblCamera.Text = "Camera:";
            // 
            // CameraCaptureForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 812);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CameraCaptureForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chụp ảnh từ Camera";
            panelMain.ResumeLayout(false);
            panelPreview.ResumeLayout(false);
            panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            panelControls.ResumeLayout(false);
            panelControls.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.ComboBox cmbCameras;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.Button btnStartCamera;
        private System.Windows.Forms.Button btnStopCamera;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}