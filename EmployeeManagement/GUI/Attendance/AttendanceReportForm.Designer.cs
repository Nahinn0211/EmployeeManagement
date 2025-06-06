namespace EmployeeManagement.GUI.Attendance
{
    partial class AttendanceReportForm
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.cmbDepartment = new System.Windows.Forms.ComboBox();
            this.lblDepartment = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.tabControlReports = new System.Windows.Forms.TabControl();
            this.tabPageDaily = new System.Windows.Forms.TabPage();
            this.dataGridViewDaily = new System.Windows.Forms.DataGridView();
            this.panelDailyStats = new System.Windows.Forms.Panel();
            this.lblDailyStats = new System.Windows.Forms.Label();
            this.tabPageSummary = new System.Windows.Forms.TabPage();
            this.dataGridViewSummary = new System.Windows.Forms.DataGridView();
            this.panelSummaryStats = new System.Windows.Forms.Panel();
            this.lblSummaryStats = new System.Windows.Forms.Label();
            this.tabPageFaceRecognition = new System.Windows.Forms.TabPage();
            this.dataGridViewFaceRecognition = new System.Windows.Forms.DataGridView();
            this.panelFaceStats = new System.Windows.Forms.Panel();
            this.lblFaceStats = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.lblStatusFooter = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panelMain.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelFilter.SuspendLayout();
            this.groupBoxFilter.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.tabControlReports.SuspendLayout();
            this.tabPageDaily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDaily)).BeginInit();
            this.panelDailyStats.SuspendLayout();
            this.tabPageSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).BeginInit();
            this.panelSummaryStats.SuspendLayout();
            this.tabPageFaceRecognition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFaceRecognition)).BeginInit();
            this.panelFaceStats.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelContent);
            this.panelMain.Controls.Add(this.panelFilter);
            this.panelMain.Controls.Add(this.panelFooter);
            this.panelMain.Controls.Add(this.panelHeader);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(10);
            this.panelMain.Size = new System.Drawing.Size(1200, 800);
            this.panelMain.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(10, 10);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1180, 60);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1180, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BÁO CÁO CHẤM CÔNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelFilter
            // 
            this.panelFilter.Controls.Add(this.groupBoxFilter);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(10, 70);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Padding = new System.Windows.Forms.Padding(10);
            this.panelFilter.Size = new System.Drawing.Size(1180, 120);
            this.panelFilter.TabIndex = 1;
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Controls.Add(this.btnExport);
            this.groupBoxFilter.Controls.Add(this.btnClear);
            this.groupBoxFilter.Controls.Add(this.btnSearch);
            this.groupBoxFilter.Controls.Add(this.cmbStatus);
            this.groupBoxFilter.Controls.Add(this.lblStatus);
            this.groupBoxFilter.Controls.Add(this.cmbEmployee);
            this.groupBoxFilter.Controls.Add(this.lblEmployee);
            this.groupBoxFilter.Controls.Add(this.cmbDepartment);
            this.groupBoxFilter.Controls.Add(this.lblDepartment);
            this.groupBoxFilter.Controls.Add(this.dtpToDate);
            this.groupBoxFilter.Controls.Add(this.lblToDate);
            this.groupBoxFilter.Controls.Add(this.dtpFromDate);
            this.groupBoxFilter.Controls.Add(this.lblFromDate);
            this.groupBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxFilter.Location = new System.Drawing.Point(10, 10);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxFilter.Size = new System.Drawing.Size(1160, 100);
            this.groupBoxFilter.TabIndex = 0;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "Bộ lọc báo cáo";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(120, 30);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(120, 30);
            this.dtpFromDate.TabIndex = 1;
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromDate.Location = new System.Drawing.Point(15, 33);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(79, 23);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "Từ ngày:";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(360, 30);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(120, 30);
            this.dtpToDate.TabIndex = 3;
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToDate.Location = new System.Drawing.Point(260, 33);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(88, 23);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "Đến ngày:";
            // 
            // cmbDepartment
            // 
            this.cmbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartment.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDepartment.FormattingEnabled = true;
            this.cmbDepartment.Location = new System.Drawing.Point(600, 30);
            this.cmbDepartment.Name = "cmbDepartment";
            this.cmbDepartment.Size = new System.Drawing.Size(200, 31);
            this.cmbDepartment.TabIndex = 5;
            this.cmbDepartment.SelectedIndexChanged += new System.EventHandler(this.CmbDepartment_SelectedIndexChanged);
            // 
            // lblDepartment
            // 
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDepartment.Location = new System.Drawing.Point(520, 33);
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(95, 23);
            this.lblDepartment.TabIndex = 4;
            this.lblDepartment.Text = "Phòng ban:";
            // 
            // cmbEmployee
            // 
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEmployee.FormattingEnabled = true;
            this.cmbEmployee.Location = new System.Drawing.Point(120, 65);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(240, 31);
            this.cmbEmployee.TabIndex = 7;
            // 
            // lblEmployee
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployee.Location = new System.Drawing.Point(15, 68);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(89, 23);
            this.lblEmployee.TabIndex = 6;
            this.lblEmployee.Text = "Nhân viên:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(480, 65);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 31);
            this.cmbStatus.TabIndex = 9;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(400, 68);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(87, 23);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(680, 60);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 35);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(790, 60);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 35);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Xóa bộ lọc";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(900, 60);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 35);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Xuất Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.tabControlReports);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(10, 190);
            this.panelContent.Name = "panelContent";
            this.panelMain.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panelContent.Size = new System.Drawing.Size(1180, 570);
            this.panelContent.TabIndex = 2;
            // 
            // tabControlReports
            // 
            this.tabControlReports.Controls.Add(this.tabPageDaily);
            this.tabControlReports.Controls.Add(this.tabPageSummary);
            this.tabControlReports.Controls.Add(this.tabPageFaceRecognition);
            this.tabControlReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlReports.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlReports.Location = new System.Drawing.Point(10, 0);
            this.tabControlReports.Name = "tabControlReports";
            this.tabControlReports.SelectedIndex = 0;
            this.tabControlReports.Size = new System.Drawing.Size(1160, 570);
            this.tabControlReports.TabIndex = 0;
            // 
            // tabPageDaily
            // 
            this.tabPageDaily.Controls.Add(this.dataGridViewDaily);
            this.tabPageDaily.Controls.Add(this.panelDailyStats);
            this.tabPageDaily.Location = new System.Drawing.Point(4, 32);
            this.tabPageDaily.Name = "tabPageDaily";
            this.tabPageDaily.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDaily.Size = new System.Drawing.Size(1152, 534);
            this.tabPageDaily.TabIndex = 0;
            this.tabPageDaily.Text = "Chấm công hàng ngày";
            this.tabPageDaily.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDaily
            // 
            this.dataGridViewDaily.AllowUserToAddRows = false;
            this.dataGridViewDaily.AllowUserToDeleteRows = false;
            this.dataGridViewDaily.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewDaily.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewDaily.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewDaily.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.dataGridViewDaily.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewDaily.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewDaily.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDaily.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewDaily.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDaily.EnableHeadersVisualStyles = false;
            this.dataGridViewDaily.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDaily.Name = "dataGridViewDaily";
            this.dataGridViewDaily.ReadOnly = true;
            this.dataGridViewDaily.RowHeadersWidth = 51;
            this.dataGridViewDaily.RowTemplate.Height = 24;
            this.dataGridViewDaily.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDaily.Size = new System.Drawing.Size(1146, 468);
            this.dataGridViewDaily.TabIndex = 0;
            // 
            // panelDailyStats
            // 
            this.panelDailyStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelDailyStats.Controls.Add(this.lblDailyStats);
            this.panelDailyStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDailyStats.Location = new System.Drawing.Point(3, 471);
            this.panelDailyStats.Name = "panelDailyStats";
            this.panelDailyStats.Padding = new System.Windows.Forms.Padding(10);
            this.panelDailyStats.Size = new System.Drawing.Size(1146, 60);
            this.panelDailyStats.TabIndex = 1;
            // 
            // lblDailyStats
            // 
            this.lblDailyStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDailyStats.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDailyStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblDailyStats.Location = new System.Drawing.Point(10, 10);
            this.lblDailyStats.Name = "lblDailyStats";
            this.lblDailyStats.Size = new System.Drawing.Size(1126, 40);
            this.lblDailyStats.TabIndex = 0;
            this.lblDailyStats.Text = "Thống kê: 0 bản ghi";
            this.lblDailyStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageSummary
            // 
            this.tabPageSummary.Controls.Add(this.dataGridViewSummary);
            this.tabPageSummary.Controls.Add(this.panelSummaryStats);
            this.tabPageSummary.Location = new System.Drawing.Point(4, 32);
            this.tabPageSummary.Name = "tabPageSummary";
            this.tabPageSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSummary.Size = new System.Drawing.Size(1152, 534);
            this.tabPageSummary.TabIndex = 1;
            this.tabPageSummary.Text = "Tổng hợp theo nhân viên";
            this.tabPageSummary.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSummary
            // 
            this.dataGridViewSummary.AllowUserToAddRows = false;
            this.dataGridViewSummary.AllowUserToDeleteRows = false;
            this.dataGridViewSummary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewSummary.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewSummary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewSummary.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.dataGridViewSummary.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewSummary.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSummary.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSummary.EnableHeadersVisualStyles = false;
            this.dataGridViewSummary.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewSummary.Name = "dataGridViewSummary";
            this.dataGridViewSummary.ReadOnly = true;
            this.dataGridViewSummary.RowHeadersWidth = 51;
            this.dataGridViewSummary.RowTemplate.Height = 24;
            this.dataGridViewSummary.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSummary.Size = new System.Drawing.Size(1146, 468);
            this.dataGridViewSummary.TabIndex = 0;
            // 
            // panelSummaryStats
            // 
            this.panelSummaryStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelSummaryStats.Controls.Add(this.lblSummaryStats);
            this.panelSummaryStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSummaryStats.Location = new System.Drawing.Point(3, 471);
            this.panelSummaryStats.Name = "panelSummaryStats";
            this.panelSummaryStats.Padding = new System.Windows.Forms.Padding(10);
            this.panelSummaryStats.Size = new System.Drawing.Size(1146, 60);
            this.panelSummaryStats.TabIndex = 1;
            // 
            // lblSummaryStats
            // 
            this.lblSummaryStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSummaryStats.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummaryStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblSummaryStats.Location = new System.Drawing.Point(10, 10);
            this.lblSummaryStats.Name = "lblSummaryStats";
            this.lblSummaryStats.Size = new System.Drawing.Size(1126, 40);
            this.lblSummaryStats.TabIndex = 0;
            this.lblSummaryStats.Text = "Thống kê: 0 nhân viên";
            this.lblSummaryStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageFaceRecognition
            // 
            this.tabPageFaceRecognition.Controls.Add(this.dataGridViewFaceRecognition);
            this.tabPageFaceRecognition.Controls.Add(this.panelFaceStats);
            this.tabPageFaceRecognition.Location = new System.Drawing.Point(4, 32);
            this.tabPageFaceRecognition.Name = "tabPageFaceRecognition";
            this.tabPageFaceRecognition.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFaceRecognition.Size = new System.Drawing.Size(1152, 534);
            this.tabPageFaceRecognition.TabIndex = 2;
            this.tabPageFaceRecognition.Text = "Chấm công bằng khuôn mặt";
            this.tabPageFaceRecognition.UseVisualStyleBackColor = true;
            // 
            // dataGridViewFaceRecognition
            // 
            this.dataGridViewFaceRecognition.AllowUserToAddRows = false;
            this.dataGridViewFaceRecognition.AllowUserToDeleteRows = false;
            this.dataGridViewFaceRecognition.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewFaceRecognition.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewFaceRecognition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewFaceRecognition.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.dataGridViewFaceRecognition.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewFaceRecognition.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewFaceRecognition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFaceRecognition.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewFaceRecognition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewFaceRecognition.EnableHeadersVisualStyles = false;
            this.dataGridViewFaceRecognition.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewFaceRecognition.Name = "dataGridViewFaceRecognition";
            this.dataGridViewFaceRecognition.ReadOnly = true;
            this.dataGridViewFaceRecognition.RowHeadersWidth = 51;
            this.dataGridViewFaceRecognition.RowTemplate.Height = 24;
            this.dataGridViewFaceRecognition.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewFaceRecognition.Size = new System.Drawing.Size(1146, 468);
            this.dataGridViewFaceRecognition.TabIndex = 0;
            // 
            // panelFaceStats
            // 
            this.panelFaceStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelFaceStats.Controls.Add(this.lblFaceStats);
            this.panelFaceStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFaceStats.Location = new System.Drawing.Point(3, 471);
            this.panelFaceStats.Name = "panelFaceStats";
            this.panelFaceStats.Padding = new System.Windows.Forms.Padding(10);
            this.panelFaceStats.Size = new System.Drawing.Size(1146, 60);
            this.panelFaceStats.TabIndex = 1;
            // 
            // lblFaceStats
            // 
            this.lblFaceStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFaceStats.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFaceStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.lblFaceStats.Location = new System.Drawing.Point(10, 10);
            this.lblFaceStats.Name = "lblFaceStats";
            this.lblFaceStats.Size = new System.Drawing.Size(1126, 40);
            this.lblFaceStats.TabIndex = 0;
            this.lblFaceStats.Text = "Thống kê: 0 lần chấm công bằng khuôn mặt";
            this.lblFaceStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panelFooter.Controls.Add(this.progressBar);
            this.panelFooter.Controls.Add(this.lblStatusFooter);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(10, 760);
            this.panelFooter.Name = "panelFooter";
             this.panelMain.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);

            this.panelFooter.Size = new System.Drawing.Size(1180, 40);
            this.panelFooter.TabIndex = 3;
            // 
            // lblStatusFooter
            // 
            this.lblStatusFooter.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatusFooter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblStatusFooter.Location = new System.Drawing.Point(10, 5);
            this.lblStatusFooter.Name = "lblStatusFooter";
            this.lblStatusFooter.Size = new System.Drawing.Size(300, 30);
            this.lblStatusFooter.TabIndex = 0;
            this.lblStatusFooter.Text = "Sẵn sàng";
            this.lblStatusFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.progressBar.Location = new System.Drawing.Point(970, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 30);
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            // 
            // AttendanceReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.panelMain);
            this.Name = "AttendanceReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo chấm công";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelMain.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.groupBoxFilter.ResumeLayout(false);
            this.groupBoxFilter.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.tabControlReports.ResumeLayout(false);
            this.tabPageDaily.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDaily)).EndInit();
            this.panelDailyStats.ResumeLayout(false);
            this.tabPageSummary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).EndInit();
            this.panelSummaryStats.ResumeLayout(false);
            this.tabPageFaceRecognition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFaceRecognition)).EndInit();
            this.panelFaceStats.ResumeLayout(false);
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.GroupBox groupBoxFilter;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.ComboBox cmbDepartment;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.ComboBox cmbEmployee;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.TabControl tabControlReports;
        private System.Windows.Forms.TabPage tabPageDaily;
        private System.Windows.Forms.DataGridView dataGridViewDaily;
        private System.Windows.Forms.Panel panelDailyStats;
        private System.Windows.Forms.Label lblDailyStats;
        private System.Windows.Forms.TabPage tabPageSummary;
        private System.Windows.Forms.DataGridView dataGridViewSummary;
        private System.Windows.Forms.Panel panelSummaryStats;
        private System.Windows.Forms.Label lblSummaryStats;
        private System.Windows.Forms.TabPage tabPageFaceRecognition;
        private System.Windows.Forms.DataGridView dataGridViewFaceRecognition;
        private System.Windows.Forms.Panel panelFaceStats;
        private System.Windows.Forms.Label lblFaceStats;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblStatusFooter;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}