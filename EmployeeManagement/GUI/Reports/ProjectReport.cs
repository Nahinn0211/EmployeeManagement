using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Reports
{
    public partial class ProjectReportForm : Form
    {
        private readonly ProjectReportBLL projectReportBLL;
        private List<ProjectReportDTO> allProjects;
        private ProjectReportFilter currentFilter;

        // Controls
        private TabControl tabControl;
        private TabPage tabOverview;
        private TabPage tabDetails;
        private TabPage tabCharts;
        private TabPage tabExport;

        // Overview tab controls
        private Panel statsPanel;
        private Label lblTotalProjects;
        private Label lblActiveProjects;
        private Label lblCompletedProjects;
        private Label lblOverdueProjects;

        // Details tab controls
        private Panel filterPanel;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private ComboBox cmbStatus;
        private TextBox txtSearchKeyword;
        private Button btnFilter;
        private Button btnClearFilter;
        private DataGridView dgvProjects;

        // Charts tab controls
        private Chart chartProjectStatus;
        private Chart chartCompletion;
        private Chart chartBudget;

        // Export tab controls
        private Button btnExportExcel;
        private Button btnExportPDF;
        private ProgressBar progressExport;

        public ProjectReportForm()
        {
            projectReportBLL = new ProjectReportBLL();
            currentFilter = new ProjectReportFilter();

            InitializeComponent();
            SetupForm();
            CreateControls();
            LoadData();
        }

        private void SetupForm()
        {
            this.Text = "Báo cáo Dự án";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void CreateControls()
        {
            // Main tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                SelectedIndex = 0,
                Font = new Font("Segoe UI", 10)
            };

            // Create tabs
            CreateOverviewTab();
            CreateDetailsTab();
            CreateChartsTab();
            CreateExportTab();

            this.Controls.Add(tabControl);
        }

        private void CreateOverviewTab()
        {
            tabOverview = new TabPage("📊 Tổng quan");
            tabOverview.BackColor = Color.White;

            // Stats panel
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            CreateStatsCards();
            tabOverview.Controls.Add(statsPanel);

            // Recent projects grid
            var lblRecent = new Label
            {
                Text = "📋 Dự án gần đây",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 220),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var dgvRecent = new DataGridView
            {
                Location = new Point(20, 250),
                Size = new Size(1300, 350),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false
            };

            SetupRecentProjectsGrid(dgvRecent);

            tabOverview.Controls.Add(lblRecent);
            tabOverview.Controls.Add(dgvRecent);
            tabControl.TabPages.Add(tabOverview);
        }

        private void CreateStatsCards()
        {
            var cards = new[]
            {
                new { Title = "Tổng dự án", Icon = "📊", Color = Color.FromArgb(33, 150, 243) },
                new { Title = "Đang thực hiện", Icon = "⚡", Color = Color.FromArgb(76, 175, 80) },
                new { Title = "Hoàn thành", Icon = "✅", Color = Color.FromArgb(156, 39, 176) },
                new { Title = "Quá hạn", Icon = "⚠️", Color = Color.FromArgb(244, 67, 54) }
            };

            for (int i = 0; i < cards.Length; i++)
            {
                var card = CreateStatsCard(cards[i].Title, "0", cards[i].Icon, cards[i].Color);
                card.Location = new Point(20 + i * 320, 20);
                statsPanel.Controls.Add(card);

                // Store reference to value labels
                var valueLabel = card.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Size >= 24);
                switch (i)
                {
                    case 0: lblTotalProjects = valueLabel; break;
                    case 1: lblActiveProjects = valueLabel; break;
                    case 2: lblCompletedProjects = valueLabel; break;
                    case 3: lblOverdueProjects = valueLabel; break;
                }
            }
        }

        private Panel CreateStatsCard(string title, string value, string icon, Color color)
        {
            var card = new Panel
            {
                Size = new Size(300, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(20, 50),
                AutoSize = true
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                Location = new Point(220, 50),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(iconLabel);

            // Add hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(250, 250, 250);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            return card;
        }

        private void CreateDetailsTab()
        {
            tabDetails = new TabPage("📝 Chi tiết");
            tabDetails.BackColor = Color.White;

            // Filter panel
            CreateFilterPanel();

            // Projects grid
            dgvProjects = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            SetupProjectsGrid();

            var container = new Panel { Dock = DockStyle.Fill };
            container.Controls.Add(dgvProjects);
            container.Controls.Add(filterPanel);

            tabDetails.Controls.Add(container);
            tabControl.TabPages.Add(tabDetails);
        }

        private void CreateFilterPanel()
        {
            filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            // Row 1 controls
            var lblDateRange = new Label
            {
                Text = "Khoảng thời gian:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            dtpStartDate = new DateTimePicker
            {
                Location = new Point(150, 17),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Checked = false,
                ShowCheckBox = true
            };

            var lblTo = new Label
            {
                Text = "đến",
                Location = new Point(310, 20),
                AutoSize = true
            };

            dtpEndDate = new DateTimePicker
            {
                Location = new Point(340, 17),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Checked = false,
                ShowCheckBox = true
            };

            var lblStatus = new Label
            {
                Text = "Trạng thái:",
                Location = new Point(520, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(600, 17),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbStatus.Items.AddRange(new[] { "Tất cả", "Khởi tạo", "Đang thực hiện", "Tạm dừng", "Hoàn thành", "Hủy bỏ" });
            cmbStatus.SelectedIndex = 0;

            // Row 2 controls
            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            txtSearchKeyword = new TextBox
            {
                Location = new Point(100, 57),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9)
            };

            btnFilter = new Button
            {
                Text = "🔍 LỌC",
                Location = new Point(420, 55),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter = new Button
            {
                Text = "🗑️ XÓA BỘ LỌC",
                Location = new Point(530, 55),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnClearFilter.Click += BtnClearFilter_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblDateRange, dtpStartDate, lblTo, dtpEndDate, lblStatus, cmbStatus,
                lblSearch, txtSearchKeyword, btnFilter, btnClearFilter
            });
        }

        private void CreateChartsTab()
        {
            tabCharts = new TabPage("📈 Biểu đồ");
            tabCharts.BackColor = Color.White;

            var chartsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(20)
            };

            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Project Status Chart
            chartProjectStatus = CreateChart("Trạng thái Dự án", SeriesChartType.Pie);
            chartsPanel.Controls.Add(chartProjectStatus, 0, 0);

            // Completion Chart
            chartCompletion = CreateChart("Phân bố Tiến độ", SeriesChartType.Column);
            chartsPanel.Controls.Add(chartCompletion, 1, 0);

            // Budget Chart
            chartBudget = CreateChart("Top Dự án theo Ngân sách", SeriesChartType.Bar);
            chartsPanel.Controls.Add(chartBudget, 0, 1);

            tabCharts.Controls.Add(chartsPanel);
            tabControl.TabPages.Add(tabCharts);
        }

        private void CreateExportTab()
        {
            tabExport = new TabPage("📤 Xuất báo cáo");
            tabExport.BackColor = Color.White;

            var exportPanel = new Panel
            {
                Size = new Size(400, 300),
                Location = new Point(50, 50)
            };

            var lblTitle = new Label
            {
                Text = "📊 Xuất báo cáo dự án",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var lblDescription = new Label
            {
                Text = "Chọn định dạng để xuất báo cáo:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            btnExportExcel = new Button
            {
                Text = "📊  XUẤT EXCEL",
                Location = new Point(20, 100),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            btnExportPDF = new Button
            {
                Text = "📄  XUẤT PDF",
                Location = new Point(20, 170),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnExportPDF.Click += BtnExportPDF_Click;

            progressExport = new ProgressBar
            {
                Location = new Point(20, 240),
                Size = new Size(360, 23),
                Visible = false
            };

            exportPanel.Controls.AddRange(new Control[] {
                lblTitle, lblDescription, btnExportExcel, btnExportPDF, progressExport
            });

            tabExport.Controls.Add(exportPanel);
            tabControl.TabPages.Add(tabExport);
        }

        private Chart CreateChart(string title, SeriesChartType chartType)
        {
            var chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var chartArea = new ChartArea("MainArea")
            {
                BackColor = Color.White,
                BorderColor = Color.Gray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Solid
            };
            chart.ChartAreas.Add(chartArea);

            var series = new Series("Data")
            {
                ChartType = chartType,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9)
            };
            chart.Series.Add(series);

            var chartTitle = new Title(title)
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Docking = Docking.Top
            };
            chart.Titles.Add(chartTitle);

            return chart;
        }

        private void SetupProjectsGrid()
        {
            dgvProjects.Columns.Clear();

            dgvProjects.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 100, DataPropertyName = "ProjectCode" },
                new DataGridViewTextBoxColumn { Name = "ProjectName", HeaderText = "Tên dự án", Width = 200, DataPropertyName = "ProjectName" },
                new DataGridViewTextBoxColumn { Name = "ManagerName", HeaderText = "Quản lý", Width = 150, DataPropertyName = "ManagerName" },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", Width = 120, DataPropertyName = "Status" },
                new DataGridViewTextBoxColumn { Name = "CompletionPercentage", HeaderText = "Tiến độ (%)", Width = 100, DataPropertyName = "CompletionPercentage" },
                new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120, DataPropertyName = "StartDate" },
                new DataGridViewTextBoxColumn { Name = "EndDate", HeaderText = "Ngày kết thúc", Width = 120, DataPropertyName = "EndDate" },
                new DataGridViewTextBoxColumn { Name = "Budget", HeaderText = "Ngân sách", Width = 120, DataPropertyName = "Budget" },
                new DataGridViewTextBoxColumn { Name = "ActualCost", HeaderText = "Chi phí thực tế", Width = 120, DataPropertyName = "ActualCost" },
                new DataGridViewTextBoxColumn { Name = "TotalTasks", HeaderText = "Tổng CV", Width = 80, DataPropertyName = "TotalTasks" },
                new DataGridViewTextBoxColumn { Name = "CompletedTasks", HeaderText = "CV hoàn thành", Width = 100, DataPropertyName = "CompletedTasks" },
                new DataGridViewTextBoxColumn { Name = "TotalEmployees", HeaderText = "Số NV", Width = 80, DataPropertyName = "TotalEmployees" }
            });

            // Format columns
            dgvProjects.Columns["CompletionPercentage"].DefaultCellStyle.Format = "N1";
            dgvProjects.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvProjects.Columns["EndDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvProjects.Columns["Budget"].DefaultCellStyle.Format = "N0";
            dgvProjects.Columns["ActualCost"].DefaultCellStyle.Format = "N0";

            // Style header
            dgvProjects.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Color coding for status
            dgvProjects.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgvProjects.Columns["Status"].Index && e.Value != null)
                {
                    var status = e.Value.ToString();
                    switch (status)
                    {
                        case "Hoàn thành":
                            e.CellStyle.BackColor = Color.FromArgb(200, 255, 200);
                            e.CellStyle.ForeColor = Color.FromArgb(0, 100, 0);
                            break;
                        case "Đang thực hiện":
                            e.CellStyle.BackColor = Color.FromArgb(200, 220, 255);
                            e.CellStyle.ForeColor = Color.FromArgb(0, 0, 139);
                            break;
                        case "Tạm dừng":
                            e.CellStyle.BackColor = Color.FromArgb(255, 240, 200);
                            e.CellStyle.ForeColor = Color.FromArgb(139, 69, 0);
                            break;
                        case "Hủy bỏ":
                            e.CellStyle.BackColor = Color.FromArgb(255, 200, 200);
                            e.CellStyle.ForeColor = Color.FromArgb(139, 0, 0);
                            break;
                    }
                    e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            };
        }

        private void SetupRecentProjectsGrid(DataGridView dgv)
        {
            dgv.Columns.Clear();

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 120, DataPropertyName = "ProjectCode" },
                new DataGridViewTextBoxColumn { Name = "ProjectName", HeaderText = "Tên dự án", Width = 300, DataPropertyName = "ProjectName" },
                new DataGridViewTextBoxColumn { Name = "ManagerName", HeaderText = "Quản lý", Width = 150, DataPropertyName = "ManagerName" },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", Width = 120, DataPropertyName = "Status" },
                new DataGridViewTextBoxColumn { Name = "CompletionPercentage", HeaderText = "Tiến độ (%)", Width = 100, DataPropertyName = "CompletionPercentage" },
                new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120, DataPropertyName = "StartDate" }
            });

            dgv.Columns["CompletionPercentage"].DefaultCellStyle.Format = "N1";
            dgv.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Style header
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
        }

        private void LoadData()
        {
            try
            {
                // Load statistics
                var stats = projectReportBLL.GetProjectStatistics();
                UpdateStatsCards(stats);

                // Load all projects
                allProjects = projectReportBLL.GetProjectReports();
                UpdateProjectsGrid();

                // Load charts
                UpdateCharts();

                // Log success
                Console.WriteLine($"Đã tải {allProjects?.Count ?? 0} dự án");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Lỗi tải dữ liệu ProjectReportForm: {ex.Message}");

                // Initialize empty data to prevent crashes
                allProjects = new List<ProjectReportDTO>();
                UpdateStatsCards(new ProjectStatistics());
            }
        }

        private void UpdateStatsCards(ProjectStatistics stats)
        {
            if (lblTotalProjects != null) lblTotalProjects.Text = stats.TotalProjects.ToString();
            if (lblActiveProjects != null) lblActiveProjects.Text = stats.ActiveProjects.ToString();
            if (lblCompletedProjects != null) lblCompletedProjects.Text = stats.CompletedProjects.ToString();
            if (lblOverdueProjects != null) lblOverdueProjects.Text = stats.OverdueProjects.ToString();
        }

        private void UpdateProjectsGrid()
        {
            var filteredProjects = ApplyFilter(allProjects ?? new List<ProjectReportDTO>());
            dgvProjects.DataSource = filteredProjects;

            // Update recent projects in overview tab
            var recentProjects = (allProjects ?? new List<ProjectReportDTO>())
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .ToList();

            var recentGrid = tabOverview.Controls.OfType<DataGridView>().FirstOrDefault();
            if (recentGrid != null)
            {
                recentGrid.DataSource = recentProjects;
            }
        }

        private void UpdateCharts()
        {
            try
            {
                if (allProjects == null || !allProjects.Any()) return;

                UpdateStatusChart();
                UpdateCompletionChart();
                UpdateBudgetChart();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật biểu đồ: {ex.Message}");
            }
        }

        private void UpdateStatusChart()
        {
            chartProjectStatus.Series["Data"].Points.Clear();

            var statusGroups = allProjects
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            foreach (var group in statusGroups)
            {
                chartProjectStatus.Series["Data"].Points.AddXY(group.Status, group.Count);
            }
        }

        private void UpdateCompletionChart()
        {
            chartCompletion.Series["Data"].Points.Clear();

            var completionRanges = new[]
            {
                new { Range = "0-25%", Min = 0, Max = 25 },
                new { Range = "26-50%", Min = 26, Max = 50 },
                new { Range = "51-75%", Min = 51, Max = 75 },
                new { Range = "76-100%", Min = 76, Max = 100 }
            };

            foreach (var range in completionRanges)
            {
                var count = allProjects.Count(p => p.CompletionPercentage >= range.Min && p.CompletionPercentage <= range.Max);
                chartCompletion.Series["Data"].Points.AddXY(range.Range, count);
            }
        }

        private void UpdateBudgetChart()
        {
            chartBudget.Series["Data"].Points.Clear();

            var topProjects = allProjects
                .OrderByDescending(p => p.Budget)
                .Take(10);

            foreach (var project in topProjects)
            {
                var point = chartBudget.Series["Data"].Points.AddXY(project.ProjectName, project.Budget);
                chartBudget.Series["Data"].Points[point].ToolTip = $"{project.ProjectName}\nNgân sách: {project.Budget:N0} VND";
            }
        }

        // FIXED: Sửa lỗi null propagating operator trong LINQ expression
        private List<ProjectReportDTO> ApplyFilter(List<ProjectReportDTO> projects)
        {
            if (projects == null) return new List<ProjectReportDTO>();

            var filtered = new List<ProjectReportDTO>();

            foreach (var project in projects)
            {
                bool matchesFilter = true;

                // Check date filters
                if (dtpStartDate.Checked && currentFilter.StartDate.HasValue)
                {
                    if (project.StartDate == null || project.StartDate < currentFilter.StartDate)
                        matchesFilter = false;
                }

                if (dtpEndDate.Checked && currentFilter.EndDate.HasValue)
                {
                    if (project.EndDate == null || project.EndDate > currentFilter.EndDate)
                        matchesFilter = false;
                }

                // Check status filter
                if (!string.IsNullOrEmpty(currentFilter.Status) && currentFilter.Status != "Tất cả")
                {
                    if (project.Status != currentFilter.Status)
                        matchesFilter = false;
                }

                // Check search keyword
                if (!string.IsNullOrEmpty(currentFilter.SearchKeyword))
                {
                    bool containsKeyword = false;
                    if (!string.IsNullOrEmpty(project.ProjectName) &&
                        project.ProjectName.Contains(currentFilter.SearchKeyword))
                        containsKeyword = true;

                    if (!string.IsNullOrEmpty(project.ProjectCode) &&
                        project.ProjectCode.Contains(currentFilter.SearchKeyword))
                        containsKeyword = true;

                    if (!containsKeyword)
                        matchesFilter = false;
                }

                if (matchesFilter)
                {
                    filtered.Add(project);
                }
            }

            return filtered;
        }

        #region Event Handlers

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                // Update filter from controls
                currentFilter.StartDate = dtpStartDate.Checked ? dtpStartDate.Value : null;
                currentFilter.EndDate = dtpEndDate.Checked ? dtpEndDate.Value : null;
                currentFilter.Status = cmbStatus.SelectedItem?.ToString() == "Tất cả" ? "" : cmbStatus.SelectedItem?.ToString();
                currentFilter.SearchKeyword = txtSearchKeyword.Text.Trim();

                // Apply filter
                UpdateProjectsGrid();

                MessageBox.Show("Đã áp dụng bộ lọc", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            // Clear filter controls
            dtpStartDate.Checked = false;
            dtpEndDate.Checked = false;
            cmbStatus.SelectedIndex = 0;
            txtSearchKeyword.Clear();

            // Reset filter
            currentFilter = new ProjectReportFilter();

            // Refresh data
            UpdateProjectsGrid();

            MessageBox.Show("Đã xóa bộ lọc", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Xuất báo cáo Excel",
                    FileName = $"BaoCaoDuAn_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    btnExportExcel.Enabled = false;
                    progressExport.Visible = true;
                    progressExport.Style = ProgressBarStyle.Marquee;

                    var filteredProjects = ApplyFilter(allProjects);

                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        projectReportBLL.ExportToExcel(filteredProjects, saveDialog.FileName);
                    });

                    progressExport.Visible = false;
                    btnExportExcel.Enabled = true;

                    MessageBox.Show($"Đã xuất báo cáo thành công!\nFile: {saveDialog.FileName}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở file sau khi xuất
                    if (MessageBox.Show("Bạn có muốn mở file vừa xuất?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                progressExport.Visible = false;
                btnExportExcel.Enabled = true;

                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Lỗi xuất Excel ProjectReport: {ex.Message}");
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất PDF đang được phát triển", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}