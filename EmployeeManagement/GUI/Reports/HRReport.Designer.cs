namespace EmployeeManagement.GUI.Reports
{
    partial class HRReportForm
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.dashboardPanel = new System.Windows.Forms.Panel();
            this.chartsPanel = new System.Windows.Forms.Panel();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.tabEmployees = new System.Windows.Forms.TabPage();
            this.employeePanel = new System.Windows.Forms.Panel();
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.dtpHireTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpHireFrom = new System.Windows.Forms.DateTimePicker();
            this.lblHireDate = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.cmbPosition = new System.Windows.Forms.ComboBox();
            this.lblPosition = new System.Windows.Forms.Label();
            this.cmbDepartment = new System.Windows.Forms.ComboBox();
            this.lblDepartment = new System.Windows.Forms.Label();
            this.tabDepartments = new System.Windows.Forms.TabPage();
            this.departmentPanel = new System.Windows.Forms.Panel();
            this.dgvDepartments = new System.Windows.Forms.DataGridView();
            this.departmentStatsPanel = new System.Windows.Forms.Panel();
            this.tabBirthdays = new System.Windows.Forms.TabPage();
            this.birthdayPanel = new System.Windows.Forms.Panel();
            this.dgvBirthdays = new System.Windows.Forms.DataGridView();
            this.birthdayControlPanel = new System.Windows.Forms.Panel();
            this.btnLoadBirthdays = new System.Windows.Forms.Button();
            this.cmbBirthdayYear = new System.Windows.Forms.ComboBox();
            this.lblBirthdayYear = new System.Windows.Forms.Label();
            this.cmbBirthdayMonth = new System.Windows.Forms.ComboBox();
            this.lblBirthdayMonth = new System.Windows.Forms.Label();
            this.tabAttendance = new System.Windows.Forms.TabPage();
            this.attendancePanel = new System.Windows.Forms.Panel();
            this.dgvAttendance = new System.Windows.Forms.DataGridView();
            this.attendanceControlPanel = new System.Windows.Forms.Panel();
            this.btnLoadAttendance = new System.Windows.Forms.Button();
            this.cmbAttendanceYear = new System.Windows.Forms.ComboBox();
            this.lblAttendanceYear = new System.Windows.Forms.Label();
            this.cmbAttendanceMonth = new System.Windows.Forms.ComboBox();
            this.lblAttendanceMonth = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.dashboardPanel.SuspendLayout();
            this.tabEmployees.SuspendLayout();
            this.employeePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.filterPanel.SuspendLayout();
            this.tabDepartments.SuspendLayout();
            this.departmentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).BeginInit();
            this.tabBirthdays.SuspendLayout();
            this.birthdayPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBirthdays)).BeginInit();
            this.birthdayControlPanel.SuspendLayout();
            this.tabAttendance.SuspendLayout();
            this.attendancePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttendance)).BeginInit();
            this.attendanceControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.headerPanel.Controls.Add(this.btnPrint);
            this.headerPanel.Controls.Add(this.btnExport);
            this.headerPanel.Controls.Add(this.btnRefresh);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.headerPanel.Size = new System.Drawing.Size(1400, 80);
            this.headerPanel.TabIndex = 0;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.White;
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnPrint.Location = new System.Drawing.Point(1280, 20);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 40);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "In báo cáo";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnExport.Location = new System.Drawing.Point(1160, 20);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 40);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Xuất Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnRefresh.Location = new System.Drawing.Point(1040, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 40);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(20, 25);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(223, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "BÁO CÁO NHÂN SỰ";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabEmployees);
            this.tabControl.Controls.Add(this.tabDepartments);
            this.tabControl.Controls.Add(this.tabBirthdays);
            this.tabControl.Controls.Add(this.tabAttendance);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.Location = new System.Drawing.Point(0, 80);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(20, 8);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1400, 820);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.dashboardPanel);
            this.tabDashboard.Location = new System.Drawing.Point(4, 32);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabDashboard.Size = new System.Drawing.Size(1392, 784);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "🏠 Dashboard";
            this.tabDashboard.UseVisualStyleBackColor = true;
            // 
            // dashboardPanel
            // 
            this.dashboardPanel.Controls.Add(this.chartsPanel);
            this.dashboardPanel.Controls.Add(this.statsPanel);
            this.dashboardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardPanel.Location = new System.Drawing.Point(3, 3);
            this.dashboardPanel.Name = "dashboardPanel";
            this.dashboardPanel.Padding = new System.Windows.Forms.Padding(20);
            this.dashboardPanel.Size = new System.Drawing.Size(1386, 778);
            this.dashboardPanel.TabIndex = 0;
            // 
            // chartsPanel
            // 
            this.chartsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.chartsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartsPanel.Location = new System.Drawing.Point(20, 220);
            this.chartsPanel.Name = "chartsPanel";
            this.chartsPanel.Size = new System.Drawing.Size(1346, 538);
            this.chartsPanel.TabIndex = 1;
            // 
            // statsPanel
            // 
            this.statsPanel.BackColor = System.Drawing.Color.White;
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statsPanel.Location = new System.Drawing.Point(20, 20);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1346, 200);
            this.statsPanel.TabIndex = 0;
            // 
            // tabEmployees
            // 
            this.tabEmployees.Controls.Add(this.employeePanel);
            this.tabEmployees.Location = new System.Drawing.Point(4, 32);
            this.tabEmployees.Name = "tabEmployees";
            this.tabEmployees.Padding = new System.Windows.Forms.Padding(3);
            this.tabEmployees.Size = new System.Drawing.Size(1392, 784);
            this.tabEmployees.TabIndex = 1;
            this.tabEmployees.Text = "👥 Báo cáo Nhân viên";
            this.tabEmployees.UseVisualStyleBackColor = true;
            // 
            // employeePanel
            // 
            this.employeePanel.Controls.Add(this.dgvEmployees);
            this.employeePanel.Controls.Add(this.filterPanel);
            this.employeePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeePanel.Location = new System.Drawing.Point(3, 3);
            this.employeePanel.Name = "employeePanel";
            this.employeePanel.Padding = new System.Windows.Forms.Padding(20);
            this.employeePanel.Size = new System.Drawing.Size(1386, 778);
            this.employeePanel.TabIndex = 0;
            // 
            // dgvEmployees
            // 
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.BackgroundColor = System.Drawing.Color.White;
            this.dgvEmployees.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvEmployees.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvEmployees.Location = new System.Drawing.Point(20, 140);
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(1346, 618);
            this.dgvEmployees.TabIndex = 1;
            // 
            // filterPanel
            // 
            this.filterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.filterPanel.Controls.Add(this.btnClearFilter);
            this.filterPanel.Controls.Add(this.btnFilter);
            this.filterPanel.Controls.Add(this.txtSearch);
            this.filterPanel.Controls.Add(this.lblSearch);
            this.filterPanel.Controls.Add(this.dtpHireTo);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.dtpHireFrom);
            this.filterPanel.Controls.Add(this.lblHireDate);
            this.filterPanel.Controls.Add(this.cmbStatus);
            this.filterPanel.Controls.Add(this.lblStatus);
            this.filterPanel.Controls.Add(this.cmbGender);
            this.filterPanel.Controls.Add(this.lblGender);
            this.filterPanel.Controls.Add(this.cmbPosition);
            this.filterPanel.Controls.Add(this.lblPosition);
            this.filterPanel.Controls.Add(this.cmbDepartment);
            this.filterPanel.Controls.Add(this.lblDepartment);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(20, 20);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(15);
            this.filterPanel.Size = new System.Drawing.Size(1346, 120);
            this.filterPanel.TabIndex = 0;
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.BackColor = System.Drawing.Color.Gray;
            this.btnClearFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearFilter.ForeColor = System.Drawing.Color.White;
            this.btnClearFilter.Location = new System.Drawing.Point(820, 50);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(80, 30);
            this.btnClearFilter.TabIndex = 15;
            this.btnClearFilter.Text = "Xóa lọc";
            this.btnClearFilter.UseVisualStyleBackColor = false;
            this.btnClearFilter.Click += new System.EventHandler(this.BtnClearFilter_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(730, 50);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(80, 30);
            this.btnFilter.TabIndex = 14;
            this.btnFilter.Text = "Lọc";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.BtnFilter_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(510, 52);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Tên hoặc mã nhân viên...";
            this.txtSearch.Size = new System.Drawing.Size(200, 25);
            this.txtSearch.TabIndex = 13;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(440, 55);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(71, 19);
            this.lblSearch.TabIndex = 12;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // dtpHireTo
            // 
            this.dtpHireTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHireTo.Location = new System.Drawing.Point(290, 52);
            this.dtpHireTo.Name = "dtpHireTo";
            this.dtpHireTo.Size = new System.Drawing.Size(130, 25);
            this.dtpHireTo.TabIndex = 11;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(260, 55);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(32, 19);
            this.lblTo.TabIndex = 10;
            this.lblTo.Text = "đến";
            // 
            // dtpHireFrom
            // 
            this.dtpHireFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHireFrom.Location = new System.Drawing.Point(120, 52);
            this.dtpHireFrom.Name = "dtpHireFrom";
            this.dtpHireFrom.Size = new System.Drawing.Size(130, 25);
            this.dtpHireFrom.TabIndex = 9;
            // 
            // lblHireDate
            // 
            this.lblHireDate.AutoSize = true;
            this.lblHireDate.Location = new System.Drawing.Point(15, 55);
            this.lblHireDate.Name = "lblHireDate";
            this.lblHireDate.Size = new System.Drawing.Size(99, 19);
            this.lblHireDate.TabIndex = 8;
            this.lblHireDate.Text = "Ngày vào làm:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Location = new System.Drawing.Point(780, 12);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 25);
            this.cmbStatus.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(700, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(74, 19);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.Location = new System.Drawing.Point(580, 12);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(100, 25);
            this.cmbGender.TabIndex = 5;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(510, 15);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(64, 19);
            this.lblGender.TabIndex = 4;
            this.lblGender.Text = "Giới tính:";
            // 
            // cmbPosition
            // 
            this.cmbPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPosition.Location = new System.Drawing.Point(340, 12);
            this.cmbPosition.Name = "cmbPosition";
            this.cmbPosition.Size = new System.Drawing.Size(150, 25);
            this.cmbPosition.TabIndex = 3;
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(270, 15);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(59, 19);
            this.lblPosition.TabIndex = 2;
            this.lblPosition.Text = "Chức vụ:";
            // 
            // cmbDepartment
            // 
            this.cmbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartment.Location = new System.Drawing.Point(100, 12);
            this.cmbDepartment.Name = "cmbDepartment";
            this.cmbDepartment.Size = new System.Drawing.Size(150, 25);
            this.cmbDepartment.TabIndex = 1;
            // 
            // lblDepartment
            // 
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Location = new System.Drawing.Point(15, 15);
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(77, 19);
            this.lblDepartment.TabIndex = 0;
            this.lblDepartment.Text = "Phòng ban:";
            // 
            // tabDepartments
            // 
            this.tabDepartments.Controls.Add(this.departmentPanel);
            this.tabDepartments.Location = new System.Drawing.Point(4, 32);
            this.tabDepartments.Name = "tabDepartments";
            this.tabDepartments.Size = new System.Drawing.Size(1392, 784);
            this.tabDepartments.TabIndex = 2;
            this.tabDepartments.Text = "🏢 Báo cáo Phòng ban";
            this.tabDepartments.UseVisualStyleBackColor = true;
            // 
            // departmentPanel
            // 
            this.departmentPanel.Controls.Add(this.dgvDepartments);
            this.departmentPanel.Controls.Add(this.departmentStatsPanel);
            this.departmentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.departmentPanel.Location = new System.Drawing.Point(0, 0);
            this.departmentPanel.Name = "departmentPanel";
            this.departmentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.departmentPanel.Size = new System.Drawing.Size(1392, 784);
            this.departmentPanel.TabIndex = 0;
            // 
            // dgvDepartments
            // 
            this.dgvDepartments.AllowUserToAddRows = false;
            this.dgvDepartments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDepartments.BackgroundColor = System.Drawing.Color.White;
            this.dgvDepartments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDepartments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDepartments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDepartments.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvDepartments.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvDepartments.Location = new System.Drawing.Point(20, 120);
            this.dgvDepartments.Name = "dgvDepartments";
            this.dgvDepartments.ReadOnly = true;
            this.dgvDepartments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDepartments.Size = new System.Drawing.Size(1352, 644);
            this.dgvDepartments.TabIndex = 1;
            // 
            // departmentStatsPanel
            // 
            this.departmentStatsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.departmentStatsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.departmentStatsPanel.Name = "departmentStatsPanel";
            this.departmentStatsPanel.Size = new System.Drawing.Size(1352, 100);
            this.departmentStatsPanel.TabIndex = 0;
            // 
            // tabBirthdays
            // 
            this.tabBirthdays.Controls.Add(this.birthdayPanel);
            this.tabBirthdays.Location = new System.Drawing.Point(4, 32);
            this.tabBirthdays.Name = "tabBirthdays";
            this.tabBirthdays.Size = new System.Drawing.Size(1392, 784);
            this.tabBirthdays.TabIndex = 3;
            this.tabBirthdays.Text = "🎂 Sinh nhật";
            this.tabBirthdays.UseVisualStyleBackColor = true;
            // 
            // birthdayPanel
            // 
            this.birthdayPanel.Controls.Add(this.dgvBirthdays);
            this.birthdayPanel.Controls.Add(this.birthdayControlPanel);
            this.birthdayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.birthdayPanel.Location = new System.Drawing.Point(0, 0);
            this.birthdayPanel.Name = "birthdayPanel";
            this.birthdayPanel.Padding = new System.Windows.Forms.Padding(20);
            this.birthdayPanel.Size = new System.Drawing.Size(1392, 784);
            this.birthdayPanel.TabIndex = 0;
            // 
            // dgvBirthdays
            // 
            this.dgvBirthdays.AllowUserToAddRows = false;
            this.dgvBirthdays.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBirthdays.BackgroundColor = System.Drawing.Color.White;
            this.dgvBirthdays.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBirthdays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBirthdays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBirthdays.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvBirthdays.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvBirthdays.Location = new System.Drawing.Point(20, 80);
            this.dgvBirthdays.Name = "dgvBirthdays";
            this.dgvBirthdays.ReadOnly = true;
            this.dgvBirthdays.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBirthdays.Size = new System.Drawing.Size(1352, 684);
            this.dgvBirthdays.TabIndex = 1;
            // 
            // birthdayControlPanel
            // 
            this.birthdayControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.birthdayControlPanel.Controls.Add(this.btnLoadBirthdays);
            this.birthdayControlPanel.Controls.Add(this.cmbBirthdayYear);
            this.birthdayControlPanel.Controls.Add(this.lblBirthdayYear);
            this.birthdayControlPanel.Controls.Add(this.cmbBirthdayMonth);
            this.birthdayControlPanel.Controls.Add(this.lblBirthdayMonth);
            this.birthdayControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.birthdayControlPanel.Location = new System.Drawing.Point(20, 20);
            this.birthdayControlPanel.Name = "birthdayControlPanel";
            this.birthdayControlPanel.Size = new System.Drawing.Size(1352, 60);
            this.birthdayControlPanel.TabIndex = 0;
            // 
            // btnLoadBirthdays
            // 
            this.btnLoadBirthdays.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnLoadBirthdays.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadBirthdays.ForeColor = System.Drawing.Color.White;
            this.btnLoadBirthdays.Location = new System.Drawing.Point(350, 15);
            this.btnLoadBirthdays.Name = "btnLoadBirthdays";
            this.btnLoadBirthdays.Size = new System.Drawing.Size(120, 30);
            this.btnLoadBirthdays.TabIndex = 4;
            this.btnLoadBirthdays.Text = "Tải danh sách";
            this.btnLoadBirthdays.UseVisualStyleBackColor = false;
            this.btnLoadBirthdays.Click += new System.EventHandler(this.BtnLoadBirthdays_Click);
            // 
            // cmbBirthdayYear
            // 
            this.cmbBirthdayYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBirthdayYear.Location = new System.Drawing.Point(230, 17);
            this.cmbBirthdayYear.Name = "cmbBirthdayYear";
            this.cmbBirthdayYear.Size = new System.Drawing.Size(100, 25);
            this.cmbBirthdayYear.TabIndex = 3;
            // 
            // lblBirthdayYear
            // 
            this.lblBirthdayYear.AutoSize = true;
            this.lblBirthdayYear.Location = new System.Drawing.Point(190, 20);
            this.lblBirthdayYear.Name = "lblBirthdayYear";
            this.lblBirthdayYear.Size = new System.Drawing.Size(38, 19);
            this.lblBirthdayYear.TabIndex = 2;
            this.lblBirthdayYear.Text = "Năm:";
            // 
            // cmbBirthdayMonth
            // 
            this.cmbBirthdayMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBirthdayMonth.Location = new System.Drawing.Point(70, 17);
            this.cmbBirthdayMonth.Name = "cmbBirthdayMonth";
            this.cmbBirthdayMonth.Size = new System.Drawing.Size(100, 25);
            this.cmbBirthdayMonth.TabIndex = 1;
            // 
            // lblBirthdayMonth
            // 
            this.lblBirthdayMonth.AutoSize = true;
            this.lblBirthdayMonth.Location = new System.Drawing.Point(20, 20);
            this.lblBirthdayMonth.Name = "lblBirthdayMonth";
            this.lblBirthdayMonth.Size = new System.Drawing.Size(48, 19);
            this.lblBirthdayMonth.TabIndex = 0;
            this.lblBirthdayMonth.Text = "Tháng:";
            // 
            // tabAttendance
            // 
            this.tabAttendance.Controls.Add(this.attendancePanel);
            this.tabAttendance.Location = new System.Drawing.Point(4, 32);
            this.tabAttendance.Name = "tabAttendance";
            this.tabAttendance.Size = new System.Drawing.Size(1392, 784);
            this.tabAttendance.TabIndex = 4;
            this.tabAttendance.Text = "⏰ Chấm công";
            this.tabAttendance.UseVisualStyleBackColor = true;
            // 
            // attendancePanel
            // 
            this.attendancePanel.Controls.Add(this.dgvAttendance);
            this.attendancePanel.Controls.Add(this.attendanceControlPanel);
            this.attendancePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attendancePanel.Location = new System.Drawing.Point(0, 0);
            this.attendancePanel.Name = "attendancePanel";
            this.attendancePanel.Padding = new System.Windows.Forms.Padding(20);
            this.attendancePanel.Size = new System.Drawing.Size(1392, 784);
            this.attendancePanel.TabIndex = 0;
            // 
            // dgvAttendance
            // 
            this.dgvAttendance.AllowUserToAddRows = false;
            this.dgvAttendance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAttendance.BackgroundColor = System.Drawing.Color.White;
            this.dgvAttendance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAttendance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttendance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttendance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvAttendance.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvAttendance.Location = new System.Drawing.Point(20, 80);
            this.dgvAttendance.Name = "dgvAttendance";
            this.dgvAttendance.ReadOnly = true;
            this.dgvAttendance.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAttendance.Size = new System.Drawing.Size(1352, 684);
            this.dgvAttendance.TabIndex = 1;
            // 
            // attendanceControlPanel
            // 
            this.attendanceControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.attendanceControlPanel.Controls.Add(this.btnLoadAttendance);
            this.attendanceControlPanel.Controls.Add(this.cmbAttendanceYear);
            this.attendanceControlPanel.Controls.Add(this.lblAttendanceYear);
            this.attendanceControlPanel.Controls.Add(this.cmbAttendanceMonth);
            this.attendanceControlPanel.Controls.Add(this.lblAttendanceMonth);
            this.attendanceControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.attendanceControlPanel.Location = new System.Drawing.Point(20, 20);
            this.attendanceControlPanel.Name = "attendanceControlPanel";
            this.attendanceControlPanel.Size = new System.Drawing.Size(1352, 60);
            this.attendanceControlPanel.TabIndex = 0;
            // 
            // btnLoadAttendance
            // 
            this.btnLoadAttendance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.btnLoadAttendance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadAttendance.ForeColor = System.Drawing.Color.White;
            this.btnLoadAttendance.Location = new System.Drawing.Point(350, 15);
            this.btnLoadAttendance.Name = "btnLoadAttendance";
            this.btnLoadAttendance.Size = new System.Drawing.Size(120, 30);
            this.btnLoadAttendance.TabIndex = 4;
            this.btnLoadAttendance.Text = "Tải báo cáo";
            this.btnLoadAttendance.UseVisualStyleBackColor = false;
            this.btnLoadAttendance.Click += new System.EventHandler(this.BtnLoadAttendance_Click);
            // 
            // cmbAttendanceYear
            // 
            this.cmbAttendanceYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttendanceYear.Location = new System.Drawing.Point(230, 17);
            this.cmbAttendanceYear.Name = "cmbAttendanceYear";
            this.cmbAttendanceYear.Size = new System.Drawing.Size(100, 25);
            this.cmbAttendanceYear.TabIndex = 3;
            // 
            // lblAttendanceYear
            // 
            this.lblAttendanceYear.AutoSize = true;
            this.lblAttendanceYear.Location = new System.Drawing.Point(190, 20);
            this.lblAttendanceYear.Name = "lblAttendanceYear";
            this.lblAttendanceYear.Size = new System.Drawing.Size(38, 19);
            this.lblAttendanceYear.TabIndex = 2;
            this.lblAttendanceYear.Text = "Năm:";
            // 
            // cmbAttendanceMonth
            // 
            this.cmbAttendanceMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttendanceMonth.Location = new System.Drawing.Point(70, 17);
            this.cmbAttendanceMonth.Name = "cmbAttendanceMonth";
            this.cmbAttendanceMonth.Size = new System.Drawing.Size(100, 25);
            this.cmbAttendanceMonth.TabIndex = 1;
            // 
            // lblAttendanceMonth
            // 
            this.lblAttendanceMonth.AutoSize = true;
            this.lblAttendanceMonth.Location = new System.Drawing.Point(20, 20);
            this.lblAttendanceMonth.Name = "lblAttendanceMonth";
            this.lblAttendanceMonth.Size = new System.Drawing.Size(48, 19);
            this.lblAttendanceMonth.TabIndex = 0;
            this.lblAttendanceMonth.Text = "Tháng:";
            // 
            // HRReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "HRReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo Nhân sự";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.HRReportForm_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.dashboardPanel.ResumeLayout(false);
            this.tabEmployees.ResumeLayout(false);
            this.employeePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.tabDepartments.ResumeLayout(false);
            this.departmentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).EndInit();
            this.tabBirthdays.ResumeLayout(false);
            this.birthdayPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBirthdays)).EndInit();
            this.birthdayControlPanel.ResumeLayout(false);
            this.birthdayControlPanel.PerformLayout();
            this.tabAttendance.ResumeLayout(false);
            this.attendancePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttendance)).EndInit();
            this.attendanceControlPanel.ResumeLayout(false);
            this.attendanceControlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDashboard;
        private System.Windows.Forms.Panel dashboardPanel;
        private System.Windows.Forms.Panel chartsPanel;
        private System.Windows.Forms.Panel statsPanel;
        private System.Windows.Forms.TabPage tabEmployees;
        private System.Windows.Forms.Panel employeePanel;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.ComboBox cmbDepartment;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.ComboBox cmbPosition;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblHireDate;
        private System.Windows.Forms.DateTimePicker dtpHireFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpHireTo;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.TabPage tabDepartments;
        private System.Windows.Forms.Panel departmentPanel;
        private System.Windows.Forms.DataGridView dgvDepartments;
        private System.Windows.Forms.Panel departmentStatsPanel;
        private System.Windows.Forms.TabPage tabBirthdays;
        private System.Windows.Forms.Panel birthdayPanel;
        private System.Windows.Forms.DataGridView dgvBirthdays;
        private System.Windows.Forms.Panel birthdayControlPanel;
        private System.Windows.Forms.Label lblBirthdayMonth;
        private System.Windows.Forms.ComboBox cmbBirthdayMonth;
        private System.Windows.Forms.Label lblBirthdayYear;
        private System.Windows.Forms.ComboBox cmbBirthdayYear;
        private System.Windows.Forms.Button btnLoadBirthdays;
        private System.Windows.Forms.TabPage tabAttendance;
        private System.Windows.Forms.Panel attendancePanel;
        private System.Windows.Forms.DataGridView dgvAttendance;
        private System.Windows.Forms.Panel attendanceControlPanel;
        private System.Windows.Forms.Label lblAttendanceMonth;
        private System.Windows.Forms.ComboBox cmbAttendanceMonth;
        private System.Windows.Forms.Label lblAttendanceYear;
        private System.Windows.Forms.ComboBox cmbAttendanceYear;
        private System.Windows.Forms.Button btnLoadAttendance;
    }
}