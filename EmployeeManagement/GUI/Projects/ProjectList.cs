using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;

namespace EmployeeManagement.GUI.Projects
{
    public partial class ProjectListForm : Form
    {
        #region Fields
        private ProjectBLL projectBLL;
        private List<Models.Project> projects;
        private List<Models.Project> filteredProjects;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên dự án, mã dự án...";

        // UI Controls
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
        #endregion

        #region Constructor
        public ProjectListForm()
        {
            InitializeComponent();
            InitializeLayout();
            projectBLL = new ProjectBLL();
            LoadProjects();
        }
        #endregion

        #region Data Methods
        private void LoadProjects()
        {
            try
            {
                projects = projectBLL.GetAllProjects();
                filteredProjects = new List<Models.Project>(projects);
                LoadProjectsToGrid();
                LoadManagersToComboBox();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProjectsToGrid()
        {
            try
            {
                var dataSource = filteredProjects.Select(p => new
                {
                    p.ProjectID,
                    p.ProjectCode,
                    p.ProjectName,
                    StartDate = p.StartDate?.ToString("dd/MM/yyyy") ?? "Chưa xác định",
                    EndDate = p.EndDate?.ToString("dd/MM/yyyy") ?? "Chưa xác định",
                    Status = GetStatusDisplayText(p.Status),
                    Budget = FormatBudget(p.Budget),
                    Progress = p.CompletionPercentage + "%",
                    ManagerName = p.Manager?.FullName ?? "Chưa phân công",
                    EmployeeCount = p.Employees?.Count ?? 0,
                    TaskCount = p.Tasks?.Count ?? 0,
                    Health = projectBLL.GetProjectHealthStatus(p)
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

        private void LoadManagersToComboBox()
        {
            try
            {
                managerComboBox.Items.Clear();
                managerComboBox.Items.Add("Tất cả quản lý");

                var managers = projectBLL.GetAvailableManagers();
                foreach (var manager in managers)
                {
                    managerComboBox.Items.Add(manager.FullName);
                }

                managerComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách quản lý: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = searchTextBox.Text == searchPlaceholder ? "" : searchTextBox.Text.ToLower();
                string statusFilter = statusComboBox.SelectedIndex == 0 ? "" : GetOriginalStatus(statusComboBox.Text);
                string managerFilter = managerComboBox.SelectedIndex == 0 ? "" : managerComboBox.Text;

                filteredProjects = projects.Where(p =>
                    (string.IsNullOrEmpty(searchText) ||
                     p.ProjectName.ToLower().Contains(searchText) ||
                     p.ProjectCode.ToLower().Contains(searchText)) &&
                    (string.IsNullOrEmpty(statusFilter) || p.Status == statusFilter) &&
                    (string.IsNullOrEmpty(managerFilter) || p.Manager?.FullName == managerFilter)
                ).ToList();

                LoadProjectsToGrid();
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
            filteredProjects = new List<Models.Project>(projects);
            LoadProjectsToGrid();
        }

        private void UpdateStatistics()
        {
            var total = filteredProjects.Count;
            var active = filteredProjects.Count(p => p.Status == "Đang thực hiện");
            var completed = filteredProjects.Count(p => p.Status == "Hoàn thành");
            var onHold = filteredProjects.Count(p => p.Status == "Tạm dừng");
            var initializing = filteredProjects.Count(p => p.Status == "Khởi tạo");
            var cancelled = filteredProjects.Count(p => p.Status == "Hủy bỏ");

            statisticsLabel.Text = $"📊 Tổng: {total} | 🆕 Khởi tạo: {initializing} | 🚀 Đang thực hiện: {active} | ✅ Hoàn thành: {completed} | ⏸️ Tạm dừng: {onHold} | ❌ Hủy bỏ: {cancelled}";
        }
        #endregion

        #region Helper Methods
        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Khởi tạo" => "🆕 Khởi tạo",
                "Đang thực hiện" => "🚀 Đang thực hiện",
                "Hoàn thành" => "✅ Hoàn thành",
                "Tạm dừng" => "⏸️ Tạm dừng",
                "Hủy bỏ" => "❌ Hủy bỏ",
                _ => status
            };
        }

        private string GetOriginalStatus(string displayStatus)
        {
            return displayStatus switch
            {
                "🆕 Khởi tạo" => "Khởi tạo",
                "🚀 Đang thực hiện" => "Đang thực hiện",
                "✅ Hoàn thành" => "Hoàn thành",
                "⏸️ Tạm dừng" => "Tạm dừng",
                "❌ Hủy bỏ" => "Hủy bỏ",
                _ => displayStatus
            };
        }

        private string FormatBudget(decimal budget)
        {
            if (budget >= 1000000000) // >= 1 tỷ
                return $"{budget / 1000000000:F1} tỷ";
            else if (budget >= 1000000) // >= 1 triệu
                return $"{budget / 1000000:F1} tr";
            else if (budget >= 1000) // >= 1 nghìn
                return $"{budget / 1000:F0}k";
            else
                return $"{budget:N0}";
        }

        private Models.Project GetSelectedProject()
        {
            if (projectDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = projectDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem != null)
                {
                    dynamic item = selectedRow.DataBoundItem;
                    return projects.FirstOrDefault(p => p.ProjectID == item.ProjectID);
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
                    string s when s.Contains("Hoàn thành") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Đang thực hiện") => Color.FromArgb(33, 150, 243),
                    string s when s.Contains("Khởi tạo") => Color.FromArgb(158, 158, 158),
                    string s when s.Contains("Tạm dừng") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Hủy bỏ") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "Progress" && e.Value != null)
            {
                var progressText = e.Value.ToString();
                if (decimal.TryParse(progressText.Replace("%", ""), out decimal progress))
                {
                    e.CellStyle.ForeColor = progress switch
                    {
                        >= 100 => Color.FromArgb(76, 175, 80),
                        >= 75 => Color.FromArgb(139, 195, 74),
                        >= 50 => Color.FromArgb(255, 152, 0),
                        >= 25 => Color.FromArgb(255, 193, 7),
                        _ => Color.FromArgb(244, 67, 54)
                    };
                }
            }
            else if (columnName == "Health" && e.Value != null)
            {
                var health = e.Value.ToString();
                e.CellStyle.ForeColor = health switch
                {
                    string h when h.Contains("Hoàn thành") || h.Contains("Đúng tiến độ") => Color.FromArgb(76, 175, 80),
                    string h when h.Contains("Chậm tiến độ") => Color.FromArgb(255, 152, 0),
                    string h when h.Contains("Nguy cơ trễ hạn") || h.Contains("Đã quá hạn") => Color.FromArgb(244, 67, 54),
                    string h when h.Contains("Tạm dừng") => Color.FromArgb(255, 152, 0),
                    string h when h.Contains("Đã hủy") => Color.FromArgb(158, 158, 158),
                    _ => Color.FromArgb(158, 158, 158)
                };
            }
        }

        private void AddProject()
        {
            try
            {
                // Sử dụng ProjectManagementForm với mode Create
                var form = new ProjectManagementForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProjects();
                    MessageBox.Show("Thêm dự án thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (project == null)
            {
                MessageBox.Show("Vui lòng chọn dự án cần chỉnh sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Sử dụng ProjectManagementForm với mode Update
                var form = new ProjectManagementForm(project.ProjectID, FormMode.Update);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProjects();
                    MessageBox.Show("Cập nhật dự án thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (project == null)
            {
                MessageBox.Show("Vui lòng chọn dự án cần xem!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Sử dụng ProjectManagementForm với mode Detail
                var form = new ProjectManagementForm(project.ProjectID, FormMode.Detail);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteProject()
        {
            var project = GetSelectedProject();
            if (project == null)
            {
                MessageBox.Show("Vui lòng chọn dự án cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra xem có thể xóa dự án không
                if (!projectBLL.CanDeleteProject(project.ProjectID))
                {
                    MessageBox.Show(
                        $"Không thể xóa dự án '{project.ProjectName}' vì:\n" +
                        $"- Dự án có {project.Tasks?.Count ?? 0} công việc đang hoạt động\n" +
                        $"- Dự án có {project.Employees?.Count ?? 0} nhân viên tham gia\n" +
                        $"- Dự án có tài liệu hoặc giao dịch tài chính liên quan\n\n" +
                        "Vui lòng xử lý các liên kết trước khi xóa dự án.",
                        "Không thể xóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa dự án '{project.ProjectName}'?\n\n" +
                    $"Thông tin dự án:\n" +
                    $"- Mã dự án: {project.ProjectCode}\n" +
                    $"- Ngân sách: {FormatBudget(project.Budget)} VNĐ\n" +
                    $"- Trạng thái: {project.Status}\n" +
                    $"- Tiến độ: {project.CompletionPercentage}%\n\n" +
                    "⚠️ Hành động này không thể hoàn tác!",
                    "Xác nhận xóa dự án",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    projectBLL.DeleteProject(project.ProjectID);
                    LoadProjects();
                    MessageBox.Show(
                        $"Đã xóa dự án '{project.ProjectName}' thành công!",
                        "Xóa thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Manager filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Clear button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Export button

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
            statusComboBox.Items.AddRange(new[] {
                "Tất cả trạng thái", "🆕 Khởi tạo", "🚀 Đang thực hiện",
                "✅ Hoàn thành", "⏸️ Tạm dừng", "❌ Hủy bỏ"
            });
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
            managerComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Export Button
            var exportButton = CreateStyledButton("📤 XUẤT EXCEL", Color.FromArgb(76, 175, 80));
            exportButton.Click += (s, e) => ExportToExcel();

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(managerComboBox, 2, 0);
            searchContainer.Controls.Add(searchButton, 3, 0);
            searchContainer.Controls.Add(clearButton, 4, 0);
            searchContainer.Controls.Add(exportButton, 5, 0);

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
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) =>
            {
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
                SelectionForeColor = Color.Black,
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
                new { Name = "ProjectID", HeaderText = "ID", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "ProjectCode", HeaderText = "Mã dự án", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "ProjectName", HeaderText = "Tên dự án", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "EndDate", HeaderText = "Ngày kết thúc", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Progress", HeaderText = "Tiến độ", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Budget", HeaderText = "Ngân sách", Width = 100, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "ManagerName", HeaderText = "Quản lý", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "EmployeeCount", HeaderText = "NV", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "TaskCount", HeaderText = "CV", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Health", HeaderText = "Tình trạng", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
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
                    MinimumWidth = 60,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                projectDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            projectDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = projectDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            projectDataGridView.CellDoubleClick += (s, e) =>
            {
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

        private void ExportToExcel()
        {
            try
            {
                MessageBox.Show("Chức năng xuất Excel đang được phát triển!\n\nTính năng sẽ bao gồm:\n" +
                    "• Xuất toàn bộ danh sách dự án\n" +
                    "• Xuất dữ liệu đã được lọc\n" +
                    "• Bao gồm thống kê tổng quan\n" +
                    "• Định dạng chuyên nghiệp",
                    "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method để refresh data từ bên ngoài
        public void RefreshData()
        {
            LoadProjects();
        }

        // Method để focus vào một dự án cụ thể
        public void SelectProject(int projectId)
        {
            try
            {
                LoadProjects(); // Refresh data first

                for (int i = 0; i < projectDataGridView.Rows.Count; i++)
                {
                    if (projectDataGridView.Rows[i].DataBoundItem != null)
                    {
                        dynamic item = projectDataGridView.Rows[i].DataBoundItem;
                        if (item.ProjectID == projectId)
                        {
                            projectDataGridView.ClearSelection();
                            projectDataGridView.Rows[i].Selected = true;
                            projectDataGridView.FirstDisplayedScrollingRowIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method để lọc theo trạng thái từ bên ngoài
        public void FilterByStatus(string status)
        {
            try
            {
                var displayStatus = GetStatusDisplayText(status);
                for (int i = 0; i < statusComboBox.Items.Count; i++)
                {
                    if (statusComboBox.Items[i].ToString() == displayStatus)
                    {
                        statusComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc theo trạng thái: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method để search từ bên ngoài
        public void SearchProjects(string searchText)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchTextBox.ForeColor = Color.Black;
                    searchTextBox.Text = searchText;
                }
                else
                {
                    searchTextBox.ForeColor = Color.Gray;
                    searchTextBox.Text = searchPlaceholder;
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}