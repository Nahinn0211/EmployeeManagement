using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MaterialSkin.Controls;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.GUI.Reports
{
    public partial class ProjectReportForm : Form
    {
        private readonly ProjectReportBLL projectReportBLL;
        private List<ProjectReportModel> allProjects;
        private ProjectReportFilter currentFilter;

        // Controls
        private MaterialTabControl tabControl;
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
        private MaterialButton btnFilter;
        private MaterialButton btnClearFilter;
        private DataGridView dgvProjects;

        // Charts tab controls
        private Chart chartProjectStatus;
        private Chart chartCompletion;
        private Chart chartBudget;

        // Export tab controls
        private MaterialButton btnExportExcel;
        private MaterialButton btnExportPDF;
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
            this.WindowState = FormWindowState.Maximized;
        }

        private void CreateControls()
        {
            // Main tab control
            tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                SelectedIndex = 0
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
            tabOverview = new TabPage("Tổng quan");
            tabOverview.BackColor = Color.White;

            // Stats panel
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                Padding = new Padding(20)
            };

            CreateStatsCards();
            tabOverview.Controls.Add(statsPanel);

            // Recent projects grid
            var lblRecent = new Label
            {
                Text = "Dự án gần đây",
                Font = new Font("Roboto", 16, FontStyle.Bold),
                Location = new Point(20, 220),
                AutoSize = true
            };

            var dgvRecent = new DataGridView
            {
                Location = new Point(20, 250),
                Size = new Size(1300, 300),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
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
                new { Title = "Tổng dự án", Value = "0", Color = Color.FromArgb(33, 150, 243), Control = (Label)null },
                new { Title = "Đang thực hiện", Value = "0", Color = Color.FromArgb(76, 175, 80), Control = (Label)null },
                new { Title = "Hoàn thành", Value = "0", Color = Color.FromArgb(156, 39, 176), Control = (Label)null },
                new { Title = "Quá hạn", Value = "0", Color = Color.FromArgb(244, 67, 54), Control = (Label)null }
            };

            for (int i = 0; i < cards.Length; i++)
            {
                var card = CreateStatsCard(cards[i].Title, cards[i].Value, cards[i].Color);
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

        private Panel CreateStatsCard(string title, string value, Color color)
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
                Font = new Font("Roboto", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Roboto", 32, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(20, 50),
                AutoSize = true
            };

            var iconLabel = new Label
            {
                Text = "📊",
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                Location = new Point(220, 50),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(iconLabel);

            // Add shadow effect
            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), rect);
            };

            return card;
        }

        private void CreateDetailsTab()
        {
            tabDetails = new TabPage("Chi tiết");
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
                Margin = new Padding(0, 120, 0, 0)
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
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(20)
            };

            // Row 1 controls
            var lblDateRange = new Label
            {
                Text = "Khoảng thời gian:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            dtpStartDate = new DateTimePicker
            {
                Location = new Point(150, 17),
                Size = new Size(150, 23),
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
                Size = new Size(150, 23),
                Format = DateTimePickerFormat.Short,
                Checked = false,
                ShowCheckBox = true
            };

            var lblStatus = new Label
            {
                Text = "Trạng thái:",
                Location = new Point(520, 20),
                AutoSize = true
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(600, 17),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Tất cả", "Khởi tạo", "Đang thực hiện", "Tạm dừng", "Hoàn thành", "Hủy bỏ" });
            cmbStatus.SelectedIndex = 0;

            // Row 2 controls
            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtSearchKeyword = new TextBox
            {
                Location = new Point(100, 57),
                Size = new Size(300, 23),
                PlaceholderText = "Nhập tên hoặc mã dự án..."
            };

            btnFilter = new MaterialButton
            {
                Text = "LỌC",
                Location = new Point(420, 55),
                Size = new Size(80, 30),
                Type = MaterialButton.MaterialButtonType.Contained
            };
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter = new MaterialButton
            {
                Text = "XÓA BỘ LỌC",
                Location = new Point(510, 55),
                Size = new Size(120, 30),
                Type = MaterialButton.MaterialButtonType.Outlined
            };
            btnClearFilter.Click += BtnClearFilter_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblDateRange, dtpStartDate, lblTo, dtpEndDate, lblStatus, cmbStatus,
                lblSearch, txtSearchKeyword, btnFilter, btnClearFilter
            });
        }

        private void CreateChartsTab()
        {
            tabCharts = new TabPage("Biểu đồ");
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
            chartCompletion = CreateChart("Tiến độ Dự án", SeriesChartType.Column);
            chartsPanel.Controls.Add(chartCompletion, 1, 0);

            // Budget Chart
            chartBudget = CreateChart("Ngân sách Dự án", SeriesChartType.Bar);
            chartsPanel.Controls.Add(chartBudget, 0, 1);

            tabCharts.Controls.Add(chartsPanel);
            tabControl.TabPages.Add(tabCharts);
        }

        private void CreateExportTab()
        {
            tabExport = new TabPage("Xuất báo cáo");
            tabExport.BackColor = Color.White;

            var exportPanel = new Panel
            {
                Size = new Size(400, 300),
                Location = new Point(50, 50)
            };

            var lblTitle = new Label
            {
                Text = "Xuất báo cáo dự án",
                Font = new Font("Roboto", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblDescription = new Label
            {
                Text = "Chọn định dạng để xuất báo cáo:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            btnExportExcel = new MaterialButton
            {
                Text = "📊  XUẤT EXCEL",
                Location = new Point(20, 100),
                Size = new Size(200, 50),
                Type = MaterialButton.MaterialButtonType.Contained
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            btnExportPDF = new MaterialButton
            {
                Text = "📄  XUẤT PDF",
                Location = new Point(20, 170),
                Size = new Size(200, 50),
                Type = MaterialButton.MaterialButtonType.Contained
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
                IsValueShownAsLabel = true
            };
            chart.Series.Add(series);

            var chartTitle = new Title(title)
            {
                Font = new Font("Roboto", 12, FontStyle.Bold),
                Docking = Docking.Top
            };
            chart.Titles.Add(chartTitle);

            return chart;
        }

        private void SetupProjectsGrid()
        {
            dgvProjects.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "ProjectName", HeaderText = "Tên dự án", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "ManagerName", HeaderText = "Quản lý", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CompletionPercentage", HeaderText = "Tiến độ (%)", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "EndDate", HeaderText = "Ngày kết thúc", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Budget", HeaderText = "Ngân sách", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ActualCost", HeaderText = "Chi phí thực tế", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "TotalTasks", HeaderText = "Tổng CV", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "CompletedTasks", HeaderText = "CV hoàn thành", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "TotalEmployees", HeaderText = "Số NV", Width = 80 }
            });

            // Format columns
            dgvProjects.Columns["CompletionPercentage"].DefaultCellStyle.Format = "N1";
            dgvProjects.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvProjects.Columns["EndDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvProjects.Columns["Budget"].DefaultCellStyle.Format = "N0";
            dgvProjects.Columns["ActualCost"].DefaultCellStyle.Format = "N0";

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
                            break;
                        case "Đang thực hiện":
                            e.CellStyle.BackColor = Color.FromArgb(200, 220, 255);
                            break;
                        case "Tạm dừng":
                            e.CellStyle.BackColor = Color.FromArgb(255, 240, 200);
                            break;
                        case "Hủy bỏ":
                            e.CellStyle.BackColor = Color.FromArgb(255, 200, 200);
                            break;
                    }
                }
            };
        }

        private void SetupRecentProjectsGrid(DataGridView dgv)
        {
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ProjectName", HeaderText = "Tên dự án", Width = 300 },
                new DataGridViewTextBoxColumn { Name = "ManagerName", HeaderText = "Quản lý", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CompletionPercentage", HeaderText = "Tiến độ (%)", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120 }
            });

            dgv.Columns["CompletionPercentage"].DefaultCellStyle.Format = "N1";
            dgv.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
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

                Logger.LogInfo("Đã tải dữ liệu báo cáo dự án");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError($"Lỗi tải dữ liệu ProjectReportForm: {ex.Message}");
            }
        }

        private void UpdateStatsCards(EmployeeManagement.Models.ProjectStatistics stats)
        {
            if (lblTotalProjects != null) lblTotalProjects.Text = stats.TotalProjects.ToString();
            if (lblActiveProjects != null) lblActiveProjects.Text = stats.ActiveProjects.ToString();
            if (lblCompletedProjects != null) lblCompletedProjects.Text = stats.CompletedProjects.ToString();
            if (lblOverdueProjects != null) lblOverdueProjects.Text = stats.OverdueProjects.ToString();
        }

        private void UpdateProjectsGrid()
        {
            var filteredProjects = ApplyFilter(allProjects);
            dgvProjects.DataSource = filteredProjects;

            // Update recent projects in overview tab
            var recentProjects = allProjects.OrderByDescending(p => p.StartDate).Take(10).ToList();
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
                UpdateStatusChart();
                UpdateCompletionChart();
                UpdateBudgetChart();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi cập nhật biểu đồ: {ex.Message}");
            }
        }

        private void UpdateStatusChart()
        {
            chartProjectStatus.Series["Data"].Points.Clear();

            var statusGroups = allProjects.GroupBy(p => p.Status)
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

            var topProjects = allProjects.OrderByDescending(p => p.Budget).Take(10);

            foreach (var project in topProjects)
            {
                var point = chartBudget.Series["Data"].Points.AddXY(project.ProjectName, project.Budget);
                chartBudget.Series["Data"].Points[point].ToolTip = $"{project.ProjectName}\nNgân sách: {project.Budget:N0} VND";
            }
        }

        private List<ProjectReportModel> ApplyFilter(List<ProjectReportModel> projects)
        {
            var filtered = projects.AsQueryable();

            if (dtpStartDate.Checked && currentFilter.StartDate.HasValue)
                filtered = filtered.Where(p => p.StartDate >= currentFilter.StartDate);

            if (dtpEndDate.Checked && currentFilter.EndDate.HasValue)
                filtered = filtered.Where(p => p.EndDate <= currentFilter.EndDate);

            if (!string.IsNullOrEmpty(currentFilter.Status) && currentFilter.Status != "Tất cả")
                filtered = filtered.Where(p => p.Status == currentFilter.Status);

            if (!string.IsNullOrEmpty(currentFilter.SearchKeyword))
                filtered = filtered.Where(p => p.ProjectName.Contains(currentFilter.SearchKeyword) ||
                                             p.ProjectCode.Contains(currentFilter.SearchKeyword));

            return filtered.ToList();
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
                Logger.LogError($"Lỗi xuất Excel ProjectReport: {ex.Message}");
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