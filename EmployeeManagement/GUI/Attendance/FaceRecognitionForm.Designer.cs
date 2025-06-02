namespace EmployeeManagement.GUI.Attendance
{
    partial class FaceRecognitionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.panelCamera = new System.Windows.Forms.Panel();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.lblCameraStatus = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnStartRecognition = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBarRecognition = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelResult = new System.Windows.Forms.Panel();
            this.pictureBoxEmployee = new System.Windows.Forms.PictureBox();
            this.lblEmployeeInfo = new System.Windows.Forms.Label();
            this.lblConfidence = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblResultTitle = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.timerRecognition = new System.Windows.Forms.Timer(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelMain.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.panelControls.SuspendLayout();
            this.panelResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmployee)).BeginInit();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panelMain.Controls.Add(this.panelCamera);
            this.panelMain.Controls.Add(this.panelResult);
            this.panelMain.Controls.Add(this.panelControls);
            this.panelMain.Controls.Add(this.panelFooter);
            this.panelMain.Controls.Add(this.panelHeader);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Size = new System.Drawing.Size(1000, 700);
            this.panelMain.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.panelHeader.Controls.Add(this.btnSettings);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(20, 20);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(960, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(960, 80);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🎯 CHẤM CÔNG BẰNG KHUÔN MẶT";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(860, 20);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(80, 40);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "⚙️ Cài đặt";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // panelCamera
            // 
            this.panelCamera.BackColor = System.Drawing.Color.White;
            this.panelCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCamera.Controls.Add(this.pictureBoxCamera);
            this.panelCamera.Controls.Add(this.lblCameraStatus);
            this.panelCamera.Location = new System.Drawing.Point(20, 120);
            this.panelCamera.Name = "panelCamera";
            this.panelCamera.Size = new System.Drawing.Size(640, 480);
            this.panelCamera.TabIndex = 1;
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCamera.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(638, 448);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCamera.TabIndex = 0;
            this.pictureBoxCamera.TabStop = false;
            // 
            // lblCameraStatus
            // 
            this.lblCameraStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(150)))));
            this.lblCameraStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCameraStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCameraStatus.ForeColor = System.Drawing.Color.White;
            this.lblCameraStatus.Location = new System.Drawing.Point(0, 448);
            this.lblCameraStatus.Name = "lblCameraStatus";
            this.lblCameraStatus.Size = new System.Drawing.Size(638, 30);
            this.lblCameraStatus.TabIndex = 1;
            this.lblCameraStatus.Text = "📷 Camera đang sẵn sàng";
            this.lblCameraStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.White;
            this.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControls.Controls.Add(this.progressBarRecognition);
            this.panelControls.Controls.Add(this.lblStatus);
            this.panelControls.Controls.Add(this.btnCancel);
            this.panelControls.Controls.Add(this.btnStartRecognition);
            this.panelControls.Location = new System.Drawing.Point(680, 120);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(20);
            this.panelControls.Size = new System.Drawing.Size(300, 200);
            this.panelControls.TabIndex = 2;
            // 
            // btnStartRecognition
            // 
            this.btnStartRecognition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnStartRecognition.FlatAppearance.BorderSize = 0;
            this.btnStartRecognition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartRecognition.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartRecognition.ForeColor = System.Drawing.Color.White;
            this.btnStartRecognition.Location = new System.Drawing.Point(20, 20);
            this.btnStartRecognition.Name = "btnStartRecognition";
            this.btnStartRecognition.Size = new System.Drawing.Size(260, 60);
            this.btnStartRecognition.TabIndex = 0;
            this.btnStartRecognition.Text = "🚀 BẮT ĐẦU NHẬN DIỆN";
            this.btnStartRecognition.UseVisualStyleBackColor = false;
            this.btnStartRecognition.Click += new System.EventHandler(this.BtnStartRecognition_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(20, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(260, 60);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "❌ HỦY NHẬN DIỆN";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // progressBarRecognition
            // 
            this.progressBarRecognition.Location = new System.Drawing.Point(20, 100);
            this.progressBarRecognition.Name = "progressBarRecognition";
            this.progressBarRecognition.Size = new System.Drawing.Size(260, 20);
            this.progressBarRecognition.TabIndex = 2;
            this.progressBarRecognition.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 140);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(260, 40);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Sẵn sàng chấm công";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelResult
            // 
            this.panelResult.BackColor = System.Drawing.Color.White;
            this.panelResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResult.Controls.Add(this.lblResultTitle);
            this.panelResult.Controls.Add(this.pictureBoxEmployee);
            this.panelResult.Controls.Add(this.lblEmployeeInfo);
            this.panelResult.Controls.Add(this.lblConfidence);
            this.panelResult.Controls.Add(this.lblTime);
            this.panelResult.Location = new System.Drawing.Point(680, 340);
            this.panelResult.Name = "panelResult";
            this.panelResult.Padding = new System.Windows.Forms.Padding(20);
            this.panelResult.Size = new System.Drawing.Size(300, 260);
            this.panelResult.TabIndex = 3;
            this.panelResult.Visible = false;
            // 
            // lblResultTitle
            // 
            this.lblResultTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblResultTitle.Location = new System.Drawing.Point(20, 20);
            this.lblResultTitle.Name = "lblResultTitle";
            this.lblResultTitle.Size = new System.Drawing.Size(258, 30);
            this.lblResultTitle.TabIndex = 0;
            this.lblResultTitle.Text = "✅ CHẤM CÔNG THÀNH CÔNG";
            this.lblResultTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxEmployee
            // 
            this.pictureBoxEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxEmployee.Location = new System.Drawing.Point(20, 60);
            this.pictureBoxEmployee.Name = "pictureBoxEmployee";
            this.pictureBoxEmployee.Size = new System.Drawing.Size(100, 120);
            this.pictureBoxEmployee.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxEmployee.TabIndex = 1;
            this.pictureBoxEmployee.TabStop = false;
            // 
            // lblEmployeeInfo
            // 
            this.lblEmployeeInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployeeInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblEmployeeInfo.Location = new System.Drawing.Point(130, 60);
            this.lblEmployeeInfo.Name = "lblEmployeeInfo";
            this.lblEmployeeInfo.Size = new System.Drawing.Size(148, 30);
            this.lblEmployeeInfo.TabIndex = 2;
            this.lblEmployeeInfo.Text = "NV001 - Nguyễn Văn A";
            this.lblEmployeeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblConfidence
            // 
            this.lblConfidence.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfidence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblConfidence.Location = new System.Drawing.Point(130, 100);
            this.lblConfidence.Name = "lblConfidence";
            this.lblConfidence.Size = new System.Drawing.Size(148, 25);
            this.lblConfidence.TabIndex = 3;
            this.lblConfidence.Text = "Độ tin cậy: 95.5%";
            this.lblConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblTime.Location = new System.Drawing.Point(130, 130);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(148, 50);
            this.lblTime.TabIndex = 4;
            this.lblTime.Text = "Thời gian: 08:30:45";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelFooter.Controls.Add(this.lblInstructions);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(20, 620);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(10);
            this.panelFooter.Size = new System.Drawing.Size(960, 60);
            this.panelFooter.TabIndex = 4;
            // 
            // lblInstructions
            // 
            this.lblInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstructions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblInstructions.Location = new System.Drawing.Point(10, 10);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(940, 40);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "💡 Hướng dẫn: Nhấn \"Bắt đầu nhận diện\" và nhìn thẳng vào camera. Hệ thống sẽ tự" +
    " động chấm công khi nhận diện thành công.";
            this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FaceRecognitionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chấm công bằng Khuôn mặt - Employee Management System";
            this.panelMain.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelCamera.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmployee)).EndInit();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel panelCamera;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Label lblCameraStatus;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Button btnStartRecognition;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBarRecognition;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelResult;
        private System.Windows.Forms.Label lblResultTitle;
        private System.Windows.Forms.PictureBox pictureBoxEmployee;
        private System.Windows.Forms.Label lblEmployeeInfo;
        private System.Windows.Forms.Label lblConfidence;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Timer timerRecognition;
        private System.Windows.Forms.Timer timerUpdate;
    }
}