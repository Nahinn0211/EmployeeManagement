using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.Models;

namespace EmployeeManagement.GUI.Projects
{
    public partial class ProjectListForm : Form
    {
        #region Fields
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;
        private Label titleLabel;
        private TextBox searchTextBox;
        private ComboBox statusComboBox;
        private ComboBox managerComboBox;
        private Button searchButton;
        private Button clearButton;
        private DataGridView projectDataGridView;
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Label statisticsLabel;

        private List<Models.Project> projects;
        private List<Models.Project> filteredProjects;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên dự án, mã dự án...";
        #endregion

        #region Constructor
        public ProjectListForm()
        {
            InitializeComponent();
            InitializeLayout();
            InitializeData();
            LoadProjects();
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Dự án";
            this.BackColor = Color.White;
            this.Size = new Size(1400, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupSearchPanel();
            SetupDataGrid();
            SetupFooter();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };

            // Define row heights
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            };

            titleLabel = new Label
            {
                Text = "📋 QUẢN LÝ DỰ ÁN",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Search controls container
            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Manager filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Clear button

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 35,
                Margin = new Padding(0, 5, 10, 5)
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Planning", "In Progress", "Completed", "On Hold" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Manager ComboBox
            managerComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            managerComboBox.Items.Add("Tất cả quản lý");
            managerComboBox.SelectedIndex = 0;
            managerComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(managerComboBox, 2, 0);
            searchContainer.Controls.Add(searchButton, 3, 0);
            searchContainer.Controls.Add(clearButton, 4, 0);

            searchPanel.Controls.Add(searchContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
        }

        private void SetupDataGrid()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            projectDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                AllowUserToResizeColumns = true,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 40 },
                ScrollBars = ScrollBars.Both,
                AutoGenerateColumns = false
            };

            SetupDataGridStyles();
            SetupDataGridColumns();
            SetupDataGridEvents();

            gridPanel.Controls.Add(projectDataGridView);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var footerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM DỰ ÁN", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "📊 Đang tải..."
            };

            statsPanel.Controls.Add(statisticsLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }
        #endregion

        #region Control Helpers
        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 5),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(140, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 15, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void SetupSearchTextBoxEvents()
        {
            searchTextBox.GotFocus += (s, e) => {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) => {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };
        }

        private void SetupDataGridStyles()
        {
            projectDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            projectDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(10, 10, 10, 10),
                WrapMode = DataGridViewTriState.False
            };

            projectDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            projectDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "ProjectName", HeaderText = "Tên dự án", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Customer", HeaderText = "Khách hàng", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Manager", HeaderText = "Quản lý", Width = 130, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "Budget", HeaderText = "Ngân sách", Width = 130, Alignment = DataGridViewContentAlignment.MiddleRight },
                new { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "EndDate", HeaderText = "Ngày kết thúc", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "Progress", HeaderText = "Tiến độ", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 80,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment }
                };

                if (col.Name == "Budget")
                    column.DefaultCellStyle.Format = "C0";
                else if (col.Name == "StartDate" || col.Name == "EndDate")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                projectDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            projectDataGridView.SelectionChanged += (s, e) => {
                bool hasSelection = projectDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            projectDataGridView.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    ViewProject();
            };

            projectDataGridView.CellFormatting += ProjectDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddProject();
            editButton.Click += (s, e) => EditProject();
            viewButton.Click += (s, e) => ViewProject();
            deleteButton.Click += (s, e) => DeleteProject();
        }
        #endregion

        #region Data Management
        private void InitializeData()
        {
            projects = new List<Models.Project>
            {
                new Models.Project
                {
                    ProjectID = 1,
                    ProjectCode = "PRJ001",
                    ProjectName = "Hệ thống quản lý nhân sự ERP",
                    Description = "Phát triển hệ thống quản lý nhân sự toàn diện cho doanh nghiệp",
                    StartDate = new DateTime(2024, 1, 15),
                    EndDate = new DateTime(2024, 12, 15),
                    Budget = 500000000,
                    Status = "In Progress",
                    CustomerID = 1,
                    ManagerID = 1,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Models.Project
                {
                    ProjectID = 2,
                    ProjectCode = "PRJ002",
                    ProjectName = "Ứng dụng Mobile Banking",
                    Description = "Phát triển ứng dụng ngân hàng di động cho khách hàng cá nhân",
                    StartDate = new DateTime(2024, 3, 1),
                    EndDate = new DateTime(2024, 8, 1),
                    Budget = 800000000,
                    Status = "Planning",
                    CustomerID = 2,
                    ManagerID = 2,
                    CreatedAt = DateTime.Now.AddDays(-45)
                },
                new Models.Project
                {
                    ProjectID = 3,
                    ProjectCode = "PRJ003",
                    ProjectName = "Website thương mại điện tử",
                    Description = "Xây dựng website bán hàng trực tuyến với tính năng đầy đủ",
                    StartDate = new DateTime(2023, 10, 1),
                    EndDate = new DateTime(2024, 2, 1),
                    Budget = 300000000,
                    Status = "Completed",
                    CustomerID = 3,
                    ManagerID = 1,
                    CreatedAt = DateTime.Now.AddDays(-120)
                },
                new Models.Project
                {
                    ProjectID = 4,
                    ProjectCode = "PRJ004",
                    ProjectName = "Hệ thống CRM cho doanh nghiệp",
                    Description = "Phát triển hệ thống quản lý quan hệ khách hàng",
                    StartDate = new DateTime(2024, 2, 1),
                    EndDate = new DateTime(2024, 10, 1),
                    Budget = 450000000,
                    Status = "On Hold",
                    CustomerID = 4,
                    ManagerID = 3,
                    CreatedAt = DateTime.Now.AddDays(-60)
                },
                new Models.Project
                {
                    ProjectID = 5,
                    ProjectCode = "PRJ005",
                    ProjectName = "Ứng dụng học trực tuyến",
                    Description = "Platform học tập trực tuyến với video và bài tập tương tác",
                    StartDate = new DateTime(2024, 4, 1),
                    EndDate = new DateTime(2024, 11, 1),
                    Budget = 600000000,
                    Status = "In Progress",
                    CustomerID = 5,
                    ManagerID = 2,
                    CreatedAt = DateTime.Now.AddDays(-15)
                }
            };

            filteredProjects = new List<Models.Project>(projects.Cast<Models.Project>());
        }

        private void LoadProjects()
        {
            try
            {
                var dataSource = filteredProjects.Select(p => new ProjectDisplayModel
                {
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    Customer = $"Khách hàng {p.CustomerID}",
                    Manager = $"Quản lý {p.ManagerID}",
                    Status = GetStatusDisplayText(p.Status),
                    Budget = p.Budget,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Progress = CalculateProgress(p)
                }).ToList();

                projectDataGridView.DataSource = dataSource;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = searchTextBox.Text == searchPlaceholder ? "" : searchTextBox.Text.ToLower();
                string statusFilter = statusComboBox.SelectedIndex == 0 ? "" : statusComboBox.Text;
                string managerFilter = managerComboBox.SelectedIndex == 0 ? "" : managerComboBox.Text;

                filteredProjects = projects.Where(p =>
                    (string.IsNullOrEmpty(searchText) ||
                     p.ProjectName.ToLower().Contains(searchText) ||
                     p.ProjectCode.ToLower().Contains(searchText) ||
                     p.Description.ToLower().Contains(searchText)) &&
                    (string.IsNullOrEmpty(statusFilter) || p.Status == statusFilter) &&
                    (string.IsNullOrEmpty(managerFilter))
                ).ToList();

                LoadProjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchTextBox.Text = searchPlaceholder;
            searchTextBox.ForeColor = Color.Gray;
            statusComboBox.SelectedIndex = 0;
            managerComboBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private void UpdateStatistics()
        {
            var total = filteredProjects.Count;
            var inProgress = filteredProjects.Count(p => p.Status == "In Progress");
            var completed = filteredProjects.Count(p => p.Status == "Completed");
            var planning = filteredProjects.Count(p => p.Status == "Planning");
            var onHold = filteredProjects.Count(p => p.Status == "On Hold");

            statisticsLabel.Text = $"📊 Tổng: {total} | 📋 Lên kế hoạch: {planning} | 🔄 Đang thực hiện: {inProgress} | ✅ Hoàn thành: {completed} | ⏸️ Tạm dừng: {onHold}";
        }
        #endregion

        #region Helper Methods
        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Planning" => "📋 Lên kế hoạch",
                "In Progress" => "🔄 Đang thực hiện",
                "Completed" => "✅ Hoàn thành",
                "On Hold" => "⏸️ Tạm dừng",
                _ => status
            };
        }

        private string CalculateProgress(Models.Project project)
        {
            var totalDays = (project.EndDate - project.StartDate).TotalDays;
            if (totalDays <= 0) return "0%";

            var elapsedDays = (DateTime.Now - project.StartDate).TotalDays;
            var progress = Math.Max(0, Math.Min(100, (elapsedDays / totalDays) * 100));

            if (project.Status == "Completed") progress = 100;
            else if (project.Status == "Planning") progress = 0;

            return $"{progress:F0}%";
        }

        private Models.Project GetSelectedProject()
        {
            if (projectDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = projectDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is ProjectDisplayModel displayModel)
                {
                    return projects.FirstOrDefault(p => p.ProjectCode == displayModel.ProjectCode);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void ProjectDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = projectDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    "📋 Lên kế hoạch" => Color.FromArgb(255, 152, 0),
                    "🔄 Đang thực hiện" => Color.FromArgb(33, 150, 243),
                    "✅ Hoàn thành" => Color.FromArgb(76, 175, 80),
                    "⏸️ Tạm dừng" => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "Progress" && e.Value != null)
            {
                var progressStr = e.Value.ToString().Replace("%", "");
                if (double.TryParse(progressStr, out double progressValue))
                {
                    e.CellStyle.ForeColor = progressValue switch
                    {
                        >= 80 => Color.FromArgb(76, 175, 80),
                        >= 50 => Color.FromArgb(255, 152, 0),
                        _ => Color.FromArgb(244, 67, 54)
                    };
                }
            }
        }

        private void AddProject()
        {
            try
            {
                using (var form = new ProjectCreate())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Thêm dự án mới vào danh sách
                        var newProject = form.CreatedProject;
                        newProject.ProjectID = projects.Count + 1; // Assign new ID
                        projects.Add(newProject);

                        ApplyFilters(); // Refresh grid
                        MessageBox.Show("Thêm dự án thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditProject()
        {
            var project = GetSelectedProject();
            if (project == null) return;

            try
            {
                using (var form = new ProjectDetail(project))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh data after editing
                        ApplyFilters();
                        MessageBox.Show("Cập nhật dự án thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewProject()
        {
            var project = GetSelectedProject();
            if (project == null) return;

            try
            {
                using (var form = new ProjectDetail(project, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fix for CS1503: Ensure the correct type is being used in the `projects.Remove(project)` line.
        // The error indicates a mismatch between `EmployeeManagement.Models.Project` and `EmployeeManagement.GUI.Project`.
        // Update the `projects.Remove` call to use the correct type.

        private void DeleteProject()
        {
            var project = GetSelectedProject();
            if (project == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa dự án '{project.ProjectName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    // Ensure the correct type is used for removal
                    var projectToRemove = projects.FirstOrDefault(p => p.ProjectID == project.ProjectID);
                    if (projectToRemove != null)
                    {
                        projects.Remove(projectToRemove);
                        ApplyFilters();
                        MessageBox.Show("Xóa dự án thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }

    #region Display Models
    public class ProjectDisplayModel
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Progress { get; set; }
    }
    #endregion
}