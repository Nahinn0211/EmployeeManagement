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

namespace EmployeeManagement.GUI.Task
{
    public partial class TaskListForm : Form
    {
        #region Fields
        private TaskBLL taskBLL;
        private List<Models.WorkTask> tasks;
        private List<Models.WorkTask> filteredTasks;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên công việc, mã công việc...";
        #endregion

        #region Constructor
        public TaskListForm()
        {
            InitializeComponent();
            InitializeLayout();
            taskBLL = new TaskBLL();
            LoadTasks();
        }
        #endregion

        #region Data Methods
        private void LoadTasks()
        {
            try
            {
                tasks = taskBLL.GetAllTasks();
                filteredTasks = new List<Models.WorkTask>(tasks);
                LoadTasksToGrid();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTasksToGrid()
        {
            try
            {
                var dataSource = filteredTasks.Select(t => new TaskDisplayModel
                {
                    TaskID = t.TaskID,
                    TaskCode = t.TaskCode,
                    TaskName = t.TaskName,
                    ProjectName = t.Project?.ProjectName ?? "Không xác định",
                    AssignedToName = t.AssignedTo?.FullName ?? "Chưa giao",
                    StartDate = t.StartDate,
                    DueDate = t.DueDate,
                    Status = GetStatusDisplayText(t.Status),
                    Priority = GetPriorityDisplayText(t.Priority),
                    CompletionPercentage = t.CompletionPercentage
                }).ToList();

                taskDataGridView.DataSource = dataSource;
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
                string projectFilter = projectComboBox.SelectedIndex == 0 ? "" : projectComboBox.Text;
                string priorityFilter = priorityComboBox.SelectedIndex == 0 ? "" : priorityComboBox.Text;

                filteredTasks = tasks.Where(t =>
                    (string.IsNullOrEmpty(searchText) ||
                     t.TaskName.ToLower().Contains(searchText) ||
                     t.TaskCode.ToLower().Contains(searchText) ||
                     (t.AssignedTo?.FullName?.ToLower().Contains(searchText) ?? false)) &&
                    (string.IsNullOrEmpty(statusFilter) || t.Status == statusFilter) &&
                    (string.IsNullOrEmpty(projectFilter) || t.Project?.ProjectName == projectFilter) &&
                    (string.IsNullOrEmpty(priorityFilter) || t.Priority == priorityFilter)
                ).ToList();

                LoadTasksToGrid();
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
            projectComboBox.SelectedIndex = 0;
            priorityComboBox.SelectedIndex = 0;
            filteredTasks = new List<Models.WorkTask>(tasks);
            LoadTasksToGrid();
        }

        private void UpdateStatistics()
        {
            var total = filteredTasks.Count;
            var notStarted = filteredTasks.Count(t => t.Status == "Chưa bắt đầu");
            var inProgress = filteredTasks.Count(t => t.Status == "Đang thực hiện");
            var completed = filteredTasks.Count(t => t.Status == "Hoàn thành");

            statisticsLabel.Text = $"📊 Tổng: {total} | 🆕 Chưa bắt đầu: {notStarted} | ⏳ Đang thực hiện: {inProgress} | ✅ Hoàn thành: {completed}";
        }
        #endregion

        #region Helper Methods
        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Chưa bắt đầu" => "🆕 Chưa bắt đầu",
                "Đang thực hiện" => "⏳ Đang thực hiện",
                "Hoàn thành" => "✅ Hoàn thành",
                "Trì hoãn" => "⏸️ Trì hoãn",
                "Hủy bỏ" => "❌ Hủy bỏ",
                _ => status
            };
        }

        private string GetPriorityDisplayText(string priority)
        {
            return priority switch
            {
                "Cao" => "🔴 Cao",
                "Trung bình" => "🟠 Trung bình",
                "Thấp" => "🟢 Thấp",
                _ => priority
            };
        }

        private Models.WorkTask GetSelectedTask()
        {
            if (taskDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = taskDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is TaskDisplayModel displayModel)
                {
                    return tasks.FirstOrDefault(t => t.TaskID == displayModel.TaskID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void TaskDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = taskDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    string s when s.Contains("Hoàn thành") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Đang thực hiện") => Color.FromArgb(33, 150, 243),
                    string s when s.Contains("Chưa bắt đầu") => Color.FromArgb(158, 158, 158),
                    string s when s.Contains("Trì hoãn") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Hủy bỏ") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "Priority" && e.Value != null)
            {
                var priority = e.Value.ToString();
                e.CellStyle.ForeColor = priority switch
                {
                    string p when p.Contains("Cao") => Color.FromArgb(244, 67, 54),
                    string p when p.Contains("Trung bình") => Color.FromArgb(255, 152, 0),
                    string p when p.Contains("Thấp") => Color.FromArgb(76, 175, 80),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "CompletionPercentage" && e.Value != null)
            {
                decimal percentage = Convert.ToDecimal(e.Value);
                if (percentage == 100)
                    e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                else if (percentage >= 50)
                    e.CellStyle.ForeColor = Color.FromArgb(33, 150, 243);
                else if (percentage > 0)
                    e.CellStyle.ForeColor = Color.FromArgb(255, 152, 0);
                else
                    e.CellStyle.ForeColor = Color.FromArgb(158, 158, 158);

                e.Value = $"{percentage}%";
            }
        }

        private void AddTask()
        {
            try
            {
                using (var form = new TaskCreate())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadTasks();
                        MessageBox.Show("Thêm công việc thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditTask()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            try
            {
                using (var form = new TaskDetail(task))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadTasks();
                        MessageBox.Show("Cập nhật công việc thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewTask()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            try
            {
                using (var form = new TaskDetail(task, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteTask()
        {
            var task = GetSelectedTask();
            if (task == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa công việc '{task.TaskName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    taskBLL.DeleteTask(task.TaskID);
                    LoadTasks();
                    MessageBox.Show("Xóa công việc thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Công việc";
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
                Text = "📋 QUẢN LÝ CÔNG VIỆC",
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
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Project filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Priority filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Clear button

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
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trì hoãn", "Hủy bỏ" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Project ComboBox
            projectComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            LoadProjectsToComboBox();
            projectComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Priority ComboBox
            priorityComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            priorityComboBox.Items.AddRange(new[] { "Tất cả độ ưu tiên", "Cao", "Trung bình", "Thấp" });
            priorityComboBox.SelectedIndex = 0;
            priorityComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(projectComboBox, 2, 0);
            searchContainer.Controls.Add(priorityComboBox, 3, 0);
            searchContainer.Controls.Add(searchButton, 4, 0);
            searchContainer.Controls.Add(clearButton, 5, 0);

            searchPanel.Controls.Add(searchContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
        }

        private void LoadProjectsToComboBox()
        {
            try
            {
                projectComboBox.Items.Clear();
                projectComboBox.Items.Add("Tất cả dự án");

                var projects = taskBLL.GetAllProjects();
                foreach (var project in projects)
                {
                    projectComboBox.Items.Add(project.ProjectName);
                }

                projectComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            taskDataGridView = new DataGridView
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

            gridPanel.Controls.Add(taskDataGridView);
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

            addButton = CreateActionButton("➕ THÊM CÔNG VIỆC", Color.FromArgb(76, 175, 80));
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
            taskDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            taskDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
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

            taskDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            taskDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "TaskID", HeaderText = "ID", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "TaskCode", HeaderText = "Mã công việc", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "TaskName", HeaderText = "Tên công việc", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "ProjectName", HeaderText = "Dự án", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "AssignedToName", HeaderText = "Người thực hiện", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "StartDate", HeaderText = "Ngày bắt đầu", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "DueDate", HeaderText = "Hạn hoàn thành", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Priority", HeaderText = "Ưu tiên", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "CompletionPercentage", HeaderText = "Tiến độ", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
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
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                if (col.Name == "StartDate" || col.Name == "DueDate")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                taskDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            taskDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = taskDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            taskDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewTask();
            };

            taskDataGridView.CellFormatting += TaskDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddTask();
            editButton.Click += (s, e) => EditTask();
            viewButton.Click += (s, e) => ViewTask();
            deleteButton.Click += (s, e) => DeleteTask();
        }
        #endregion
    }
}