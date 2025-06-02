namespace EmployeeManagement.GUI.Attendance
{
    partial class FaceRegistrationForm
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.groupBoxEmployeeInfo = new System.Windows.Forms.GroupBox();
            this.cmbEmployees = new System.Windows.Forms.ComboBox();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.lblDepartment = new System.Windows.Forms.Label();
            this.txtEmployeeName = new System.Windows.Forms.TextBox();
            this.lblEmployeeName = new System.Windows.Forms.Label();
            this.txtEmployeeCode = new System.Windows.Forms.TextBox();
            this.lblEmployeeCode = new System.Windows.Forms.Label();
            this.groupBoxImageCapture = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.panelImageButtons = new System.Windows.Forms.Panel();
            this.btnClearImage = new System.Windows.Forms.Button();
            this.btnCaptureFromCamera = new System.Windows.Forms.Button();
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.lblImageStatus = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.progressBarRegistration = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBoxRegisteredFaces = new System.Windows.Forms.GroupBox();
            this.listViewRegistered = new System.Windows.Forms.ListView();
            this.columnEmployeeId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEmployeeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnRegistrationDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelListButtons = new System.Windows.Forms.Panel();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.groupBoxEmployeeInfo.SuspendLayout();
            this.groupBoxImageCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelImageButtons.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.groupBoxRegisteredFaces.SuspendLayout();
            this.panelListButtons.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.groupBoxEmployeeInfo);
            this.panelMain.Controls.Add(this.groupBoxImageCapture);
            this.panelMain.Controls.Add(this.panelStatus);
            this.panelMain.Controls.Add(this.groupBoxRegisteredFaces);
            this.panelMain.Controls.Add(this.panelButtons);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(15);
            this.panelMain.Size = new System.Drawing.Size(1000, 700);
            this.panelMain.TabIndex = 0;
            // 
            // groupBoxEmployeeInfo
            // 
            this.groupBoxEmployeeInfo.Controls.Add(this.cmbEmployees);
            this.groupBoxEmployeeInfo.Controls.Add(this.lblEmployee);
            this.groupBoxEmployeeInfo.Controls.Add(this.txtDepartment);
            this.groupBoxEmployeeInfo.Controls.Add(this.lblDepartment);
            this.groupBoxEmployeeInfo.Controls.Add(this.txtEmployeeName);
            this.groupBoxEmployeeInfo.Controls.Add(this.lblEmployeeName);
            this.groupBoxEmployeeInfo.Controls.Add(this.txtEmployeeCode);
            this.groupBoxEmployeeInfo.Controls.Add(this.lblEmployeeCode);
            this.groupBoxEmployeeInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxEmployeeInfo.Location = new System.Drawing.Point(18, 18);
            this.groupBoxEmployeeInfo.Name = "groupBoxEmployeeInfo";
            this.groupBoxEmployeeInfo.Size = new System.Drawing.Size(470, 180);
            this.groupBoxEmployeeInfo.TabIndex = 0;
            this.groupBoxEmployeeInfo.TabStop = false;
            this.groupBoxEmployeeInfo.Text = "Thông tin nhân viên";
            // 
            // cmbEmployees
            // 
            this.cmbEmployees.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployees.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbEmployees.FormattingEnabled = true;
            this.cmbEmployees.Location = new System.Drawing.Point(120, 35);
            this.cmbEmployees.Name = "cmbEmployees";
            this.cmbEmployees.Size = new System.Drawing.Size(330, 28);
            this.cmbEmployees.TabIndex = 1;
            this.cmbEmployees.SelectedIndexChanged += new System.EventHandler(this.CmbEmployees_SelectedIndexChanged);
            // 
            // lblEmployee
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEmployee.Location = new System.Drawing.Point(20, 38);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(77, 20);
            this.lblEmployee.TabIndex = 0;
            this.lblEmployee.Text = "Nhân viên:";
            // 
            // txtDepartment
            // 
            this.txtDepartment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDepartment.Location = new System.Drawing.Point(120, 135);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.ReadOnly = true;
            this.txtDepartment.Size = new System.Drawing.Size(330, 27);
            this.txtDepartment.TabIndex = 7;
            // 
            // lblDepartment
            // 
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDepartment.Location = new System.Drawing.Point(20, 138);
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(77, 20);
            this.lblDepartment.TabIndex = 6;
            this.lblDepartment.Text = "Phòng ban:";
            // 
            // txtEmployeeName
            // 
            this.txtEmployeeName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEmployeeName.Location = new System.Drawing.Point(120, 102);
            this.txtEmployeeName.Name = "txtEmployeeName";
            this.txtEmployeeName.ReadOnly = true;
            this.txtEmployeeName.Size = new System.Drawing.Size(330, 27);
            this.txtEmployeeName.TabIndex = 5;
            // 
            // lblEmployeeName
            // 
            this.lblEmployeeName.AutoSize = true;
            this.lblEmployeeName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEmployeeName.Location = new System.Drawing.Point(20, 105);
            this.lblEmployeeName.Name = "lblEmployeeName";
            this.lblEmployeeName.Size = new System.Drawing.Size(62, 20);
            this.lblEmployeeName.TabIndex = 4;
            this.lblEmployeeName.Text = "Họ tên:";
            // 
            // txtEmployeeCode
            // 
            this.txtEmployeeCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEmployeeCode.Location = new System.Drawing.Point(120, 69);
            this.txtEmployeeCode.Name = "txtEmployeeCode";
            this.txtEmployeeCode.ReadOnly = true;
            this.txtEmployeeCode.Size = new System.Drawing.Size(330, 27);
            this.txtEmployeeCode.TabIndex = 3;
            // 
            // lblEmployeeCode
            // 
            this.lblEmployeeCode.AutoSize = true;
            this.lblEmployeeCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEmployeeCode.Location = new System.Drawing.Point(20, 72);
            this.lblEmployeeCode.Name = "lblEmployeeCode";
            this.lblEmployeeCode.Size = new System.Drawing.Size(94, 20);
            this.lblEmployeeCode.TabIndex = 2;
            this.lblEmployeeCode.Text = "Mã nhân viên:";
            // 
            // groupBoxImageCapture
            // 
            this.groupBoxImageCapture.Controls.Add(this.pictureBoxPreview);
            this.groupBoxImageCapture.Controls.Add(this.panelImageButtons);
            this.groupBoxImageCapture.Controls.Add(this.lblImageStatus);
            this.groupBoxImageCapture.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxImageCapture.Location = new System.Drawing.Point(512, 18);
            this.groupBoxImageCapture.Name = "groupBoxImageCapture";
            this.groupBoxImageCapture.Size = new System.Drawing.Size(470, 280);
            this.groupBoxImageCapture.TabIndex = 1;
            this.groupBoxImageCapture.TabStop = false;
            this.groupBoxImageCapture.Text = "Ảnh khuôn mặt";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Location = new System.Drawing.Point(20, 35);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(200, 200);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // panelImageButtons
            // 
            this.panelImageButtons.Controls.Add(this.btnClearImage);
            this.panelImageButtons.Controls.Add(this.btnCaptureFromCamera);
            this.panelImageButtons.Controls.Add(this.btnSelectImage);
            this.panelImageButtons.Location = new System.Drawing.Point(240, 35);
            this.panelImageButtons.Name = "panelImageButtons";
            this.panelImageButtons.Size = new System.Drawing.Size(210, 200);
            this.panelImageButtons.TabIndex = 1;
            // 
            // btnClearImage
            // 
            this.btnClearImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnClearImage.FlatAppearance.BorderSize = 0;
            this.btnClearImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearImage.ForeColor = System.Drawing.Color.White;
            this.btnClearImage.Location = new System.Drawing.Point(15, 140);
            this.btnClearImage.Name = "btnClearImage";
            this.btnClearImage.Size = new System.Drawing.Size(180, 40);
            this.btnClearImage.TabIndex = 2;
            this.btnClearImage.Text = "Xóa ảnh";
            this.btnClearImage.UseVisualStyleBackColor = false;
            this.btnClearImage.Click += new System.EventHandler(this.BtnClearImage_Click);
            // 
            // btnCaptureFromCamera
            // 
            this.btnCaptureFromCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnCaptureFromCamera.FlatAppearance.BorderSize = 0;
            this.btnCaptureFromCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCaptureFromCamera.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCaptureFromCamera.ForeColor = System.Drawing.Color.White;
            this.btnCaptureFromCamera.Location = new System.Drawing.Point(15, 80);
            this.btnCaptureFromCamera.Name = "btnCaptureFromCamera";
            this.btnCaptureFromCamera.Size = new System.Drawing.Size(180, 40);
            this.btnCaptureFromCamera.TabIndex = 1;
            this.btnCaptureFromCamera.Text = "Chụp từ camera";
            this.btnCaptureFromCamera.UseVisualStyleBackColor = false;
            this.btnCaptureFromCamera.Click += new System.EventHandler(this.BtnCaptureFromCamera_Click);
            // 
            // btnSelectImage
            // 
            this.btnSelectImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnSelectImage.FlatAppearance.BorderSize = 0;
            this.btnSelectImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSelectImage.ForeColor = System.Drawing.Color.White;
            this.btnSelectImage.Location = new System.Drawing.Point(15, 20);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(180, 40);
            this.btnSelectImage.TabIndex = 0;
            this.btnSelectImage.Text = "Chọn ảnh từ máy";
            this.btnSelectImage.UseVisualStyleBackColor = false;
            this.btnSelectImage.Click += new System.EventHandler(this.BtnSelectImage_Click);
            // 
            // lblImageStatus
            // 
            this.lblImageStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblImageStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblImageStatus.Location = new System.Drawing.Point(20, 245);
            this.lblImageStatus.Name = "lblImageStatus";
            this.lblImageStatus.Size = new System.Drawing.Size(430, 25);
            this.lblImageStatus.TabIndex = 2;
            this.lblImageStatus.Text = "Chưa chọn ảnh";
            this.lblImageStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.progressBarRegistration);
            this.panelStatus.Controls.Add(this.lblStatus);
            this.panelStatus.Location = new System.Drawing.Point(18, 215);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(470, 83);
            this.panelStatus.TabIndex = 2;
            // 
            // progressBarRegistration
            // 
            this.progressBarRegistration.Location = new System.Drawing.Point(20, 45);
            this.progressBarRegistration.MarqueeAnimationSpeed = 30;
            this.progressBarRegistration.Name = "progressBarRegistration";
            this.progressBarRegistration.Size = new System.Drawing.Size(430, 25);
            this.progressBarRegistration.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarRegistration.TabIndex = 1;
            this.progressBarRegistration.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(430, 25);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Vui lòng chọn nhân viên và ảnh";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxRegisteredFaces
            // 
            this.groupBoxRegisteredFaces.Controls.Add(this.listViewRegistered);
            this.groupBoxRegisteredFaces.Controls.Add(this.panelListButtons);
            this.groupBoxRegisteredFaces.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxRegisteredFaces.Location = new System.Drawing.Point(18, 315);
            this.groupBoxRegisteredFaces.Name = "groupBoxRegisteredFaces";
            this.groupBoxRegisteredFaces.Size = new System.Drawing.Size(964, 300);
            this.groupBoxRegisteredFaces.TabIndex = 3;
            this.groupBoxRegisteredFaces.TabStop = false;
            this.groupBoxRegisteredFaces.Text = "Danh sách khuôn mặt đã đăng ký";
            // 
            // listViewRegistered
            // 
            this.listViewRegistered.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEmployeeId,
            this.columnEmployeeName,
            this.columnRegistrationDate});
            this.listViewRegistered.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listViewRegistered.FullRowSelect = true;
            this.listViewRegistered.GridLines = true;
            this.listViewRegistered.HideSelection = false;
            this.listViewRegistered.Location = new System.Drawing.Point(20, 35);
            this.listViewRegistered.MultiSelect = false;
            this.listViewRegistered.Name = "listViewRegistered";
            this.listViewRegistered.Size = new System.Drawing.Size(730, 240);
            this.listViewRegistered.TabIndex = 0;
            this.listViewRegistered.UseCompatibleStateImageBehavior = false;
            this.listViewRegistered.View = System.Windows.Forms.View.Details;
            // 
            // columnEmployeeId
            // 
            this.columnEmployeeId.Text = "Mã nhân viên";
            this.columnEmployeeId.Width = 150;
            // 
            // columnEmployeeName
            // 
            this.columnEmployeeName.Text = "Tên nhân viên";
            this.columnEmployeeName.Width = 300;
            // 
            // columnRegistrationDate
            // 
            this.columnRegistrationDate.Text = "Ngày đăng ký";
            this.columnRegistrationDate.Width = 150;
            // 
            // panelListButtons
            // 
            this.panelListButtons.Controls.Add(this.btnRefreshList);
            this.panelListButtons.Controls.Add(this.btnDeleteSelected);
            this.panelListButtons.Location = new System.Drawing.Point(770, 35);
            this.panelListButtons.Name = "panelListButtons";
            this.panelListButtons.Size = new System.Drawing.Size(180, 240);
            this.panelListButtons.TabIndex = 1;
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnRefreshList.FlatAppearance.BorderSize = 0;
            this.btnRefreshList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefreshList.ForeColor = System.Drawing.Color.White;
            this.btnRefreshList.Location = new System.Drawing.Point(15, 80);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(150, 40);
            this.btnRefreshList.TabIndex = 1;
            this.btnRefreshList.Text = "Làm mới";
            this.btnRefreshList.UseVisualStyleBackColor = false;
            this.btnRefreshList.Click += new System.EventHandler(this.BtnRefreshList_Click);
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnDeleteSelected.FlatAppearance.BorderSize = 0;
            this.btnDeleteSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSelected.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteSelected.ForeColor = System.Drawing.Color.White;
            this.btnDeleteSelected.Location = new System.Drawing.Point(15, 20);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(150, 40);
            this.btnDeleteSelected.TabIndex = 0;
            this.btnDeleteSelected.Text = "Xóa đã chọn";
            this.btnDeleteSelected.UseVisualStyleBackColor = false;
            this.btnDeleteSelected.Click += new System.EventHandler(this.BtnDeleteSelected_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnRegister);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(15, 635);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(970, 50);
            this.panelButtons.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(720, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnRegister.Enabled = false;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(860, 10);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(120, 35);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "Đăng ký";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // FaceRegistrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FaceRegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đăng ký khuôn mặt nhân viên";
            this.panelMain.ResumeLayout(false);
            this.groupBoxEmployeeInfo.ResumeLayout(false);
            this.groupBoxEmployeeInfo.PerformLayout();
            this.groupBoxImageCapture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelImageButtons.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.groupBoxRegisteredFaces.ResumeLayout(false);
            this.panelListButtons.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.GroupBox groupBoxEmployeeInfo;
        private System.Windows.Forms.ComboBox cmbEmployees;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.TextBox txtDepartment;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.TextBox txtEmployeeName;
        private System.Windows.Forms.Label lblEmployeeName;
        private System.Windows.Forms.TextBox txtEmployeeCode;
        private System.Windows.Forms.Label lblEmployeeCode;
        private System.Windows.Forms.GroupBox groupBoxImageCapture;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Panel panelImageButtons;
        private System.Windows.Forms.Button btnClearImage;
        private System.Windows.Forms.Button btnCaptureFromCamera;
        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.Label lblImageStatus;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.ProgressBar progressBarRegistration;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBoxRegisteredFaces;
        private System.Windows.Forms.ListView listViewRegistered;
        private System.Windows.Forms.ColumnHeader columnEmployeeId;
        private System.Windows.Forms.ColumnHeader columnEmployeeName;
        private System.Windows.Forms.ColumnHeader columnRegistrationDate;
        private System.Windows.Forms.Panel panelListButtons;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRegister;
    }
}