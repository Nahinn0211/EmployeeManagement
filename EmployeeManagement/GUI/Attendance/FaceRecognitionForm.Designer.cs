namespace EmployeeManagement.GUI.Attendance
{
    partial class FaceRecognitionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

     
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelResult = new System.Windows.Forms.Panel();
            this.pictureBoxEmployee = new System.Windows.Forms.PictureBox();
            this.lblEmployeeInfo = new System.Windows.Forms.Label();
            this.lblConfidence = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.panelCamera = new System.Windows.Forms.Panel();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.lblCameraStatus = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.progressBarRecognition = new System.Windows.Forms.ProgressBar();
            this.btnStartRecognition = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnRegisterFace = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.timerRecognition = new System.Windows.Forms.Timer(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelMain.SuspendLayout();
            this.panelResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmployee)).BeginInit();
            this.panelCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelResult);
            this.panelMain.Controls.Add(this.panelCamera);
            this.panelMain.Controls.Add(this.panelControls);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1000, 660);
            this.panelMain.TabIndex = 0;
            // 
            // panelResult
            // 
            this.panelResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResult.Controls.Add(this.pictureBoxEmployee);
            this.panelResult.Controls.Add(this.lblEmployeeInfo);
            this.panelResult.Controls.Add(this.lblConfidence);
            this.panelResult.Controls.Add(this.lblTime);
            this.panelResult.Location = new System.Drawing.Point(680, 20);
            this.panelResult.Name = "panelResult";
            this.panelResult.Size = new System.Drawing.Size(300, 400);
            this.panelResult.TabIndex = 2;
            this.panelResult.Visible = false;
            // 
            // pictureBoxEmployee
            // 
            this.pictureBoxEmployee.BackColor = System.Drawing.Color.White;
            this.pictureBoxEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxEmployee.Location = new System.Drawing.Point(100, 20);
            this.pictureBoxEmployee.Name = "pictureBoxEmployee";
            this.pictureBoxEmployee.Size = new System.Drawing.Size(100, 120);
            this.pictureBoxEmployee.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxEmployee.TabIndex = 3;
            this.pictureBoxEmployee.TabStop = false;
            // 
            // lblEmployeeInfo
            // 
            this.lblEmployeeInfo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblEmployeeInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblEmployeeInfo.Location = new System.Drawing.Point(10, 160);
            this.lblEmployeeInfo.Name = "lblEmployeeInfo";
            this.lblEmployeeInfo.Size = new System.Drawing.Size(280, 60);
            this.lblEmployeeInfo.TabIndex = 0;
            this.lblEmployeeInfo.Text = "NV001 - Nguyễn Văn A";
            this.lblEmployeeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConfidence
            // 
            this.lblConfidence.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblConfidence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblConfidence.Location = new System.Drawing.Point(10, 230);
            this.lblConfidence.Name = "lblConfidence";
            this.lblConfidence.Size = new System.Drawing.Size(280, 30);
            this.lblConfidence.TabIndex = 1;
            this.lblConfidence.Text = "Độ tin cậy: 95.8%";
            this.lblConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblTime.Location = new System.Drawing.Point(10, 270);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(280, 60);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "Thời gian: 15/12/2024 08:30:45\r\nLoại: Chấm công vào";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCamera
            // 
            this.panelCamera.BackColor = System.Drawing.Color.Black;
            this.panelCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCamera.Controls.Add(this.pictureBoxCamera);
            this.panelCamera.Controls.Add(this.lblCameraStatus);
            this.panelCamera.Location = new System.Drawing.Point(20, 20);
            this.panelCamera.Name = "panelCamera";
            this.panelCamera.Size = new System.Drawing.Size(640, 480);
            this.panelCamera.TabIndex = 0;
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCamera.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(638, 478);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabIndex = 1;
            this.pictureBoxCamera.TabStop = false;
            // 
            // lblCameraStatus
            // 
            this.lblCameraStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCameraStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCameraStatus.ForeColor = System.Drawing.Color.White;
            this.lblCameraStatus.Location = new System.Drawing.Point(10, 10);
            this.lblCameraStatus.Name = "lblCameraStatus";
            this.lblCameraStatus.Size = new System.Drawing.Size(300, 30);
            this.lblCameraStatus.TabIndex = 0;
            this.lblCameraStatus.Text = "📷 Đang khởi tạo camera...";
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.progressBarRecognition);
            this.panelControls.Controls.Add(this.btnStartRecognition);
            this.panelControls.Controls.Add(this.btnCancel);
            this.panelControls.Controls.Add(this.btnSettings);
            this.panelControls.Controls.Add(this.btnRegisterFace);
            this.panelControls.Controls.Add(this.lblStatus);
            this.panelControls.Location = new System.Drawing.Point(20, 520);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(640, 120);
            this.panelControls.TabIndex = 1;
            // 
            // progressBarRecognition
            // 
            this.progressBarRecognition.Location = new System.Drawing.Point(150, 95);
            this.progressBarRecognition.Name = "progressBarRecognition";
            this.progressBarRecognition.Size = new System.Drawing.Size(340, 20);
            this.progressBarRecognition.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarRecognition.TabIndex = 5;
            this.progressBarRecognition.Visible = false;
            // 
            // btnStartRecognition
            // 
            this.btnStartRecognition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnStartRecognition.FlatAppearance.BorderSize = 0;
            this.btnStartRecognition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartRecognition.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnStartRecognition.ForeColor = System.Drawing.Color.White;
            this.btnStartRecognition.Location = new System.Drawing.Point(200, 40);
            this.btnStartRecognition.Name = "btnStartRecognition";
            this.btnStartRecognition.Size = new System.Drawing.Size(240, 50);
            this.btnStartRecognition.TabIndex = 1;
            this.btnStartRecognition.Text = "🚀 BẮT ĐẦU NHẬN DIỆN";
            this.btnStartRecognition.UseVisualStyleBackColor = false;
            this.btnStartRecognition.Click += new System.EventHandler(this.BtnStartRecognition_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(200, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(240, 50);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "⏹️ HỦY NHẬN DIỆN";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(10, 40);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(120, 50);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "⚙️ Cài đặt";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // btnRegisterFace
            // 
            this.btnRegisterFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnRegisterFace.FlatAppearance.BorderSize = 0;
            this.btnRegisterFace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegisterFace.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRegisterFace.ForeColor = System.Drawing.Color.White;
            this.btnRegisterFace.Location = new System.Drawing.Point(510, 40);
            this.btnRegisterFace.Name = "btnRegisterFace";
            this.btnRegisterFace.Size = new System.Drawing.Size(120, 50);
            this.btnRegisterFace.TabIndex = 4;
            this.btnRegisterFace.Text = "👤 Đăng ký\r\nKhuôn mặt";
            this.btnRegisterFace.UseVisualStyleBackColor = false;
            this.btnRegisterFace.Click += new System.EventHandler(this.BtnRegisterFace_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblStatus.Location = new System.Drawing.Point(10, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(620, 25);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "⏳ Đang khởi tạo hệ thống...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerRecognition
            // 
            this.timerRecognition.Interval = 1000;
            this.timerRecognition.Tick += new System.EventHandler(this.TimerRecognition_Tick);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 100;
            this.timerUpdate.Tick += new System.EventHandler(this.TimerUpdate_Tick);
            // 
            // FaceRecognitionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 660);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FaceRecognitionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chấm công bằng khuôn mặt";
            this.Load += new System.EventHandler(this.FaceRecognitionForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FaceRecognitionForm_FormClosing);
            this.panelMain.ResumeLayout(false);
            this.panelResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmployee)).EndInit();
            this.panelCamera.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelCamera;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Label lblCameraStatus;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Button btnStartRecognition;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnRegisterFace;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBarRecognition;
        private System.Windows.Forms.Panel panelResult;
        private System.Windows.Forms.Label lblEmployeeInfo;
        private System.Windows.Forms.Label lblConfidence;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.PictureBox pictureBoxEmployee;
        private System.Windows.Forms.Timer timerRecognition;
        private System.Windows.Forms.Timer timerUpdate;
    }
}