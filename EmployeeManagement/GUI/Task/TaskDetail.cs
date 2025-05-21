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
    public partial class TaskDetail : Form
    {
        #region Fields
        private Models.WorkTask task;
        private TaskBLL taskBLL;
        private ErrorProvider errorProvider;
        private List<Models.Project> projects;
        private List<Models.Employee> employees;
        private bool isViewOnly;

        // UI Controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        private Label titleLabel;
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage detailsTab;

        // Basic Info Controls
        private TextBox taskCodeTextBox;
        private TextBox taskNameTextBox;
        private ComboBox projectComboBox;
        private ComboBox assignedToComboBox;
        private DateTimePicker startDatePicker;
        private DateTimePicker dueDatePicker;
        private DateTimePicker completedDatePicker;
        private CheckBox completedDateCheckBox;
        private ComboBox statusComboBox;
        private ComboBox priorityComboBox;
        private TrackBar completionTrackBar;
        private Label completionLabel;

        // Details Controls
        private TextBox descriptionTextBox;
        private TextBox notesTextBox;
        private Label createdAtLabel;
        private Label updatedAtLabel;

        // Footer Controls
        private Button saveButton;
        private Button closeButton;

        private readonly string[] statuses = { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trì hoãn", "Hủy bỏ" };
        private readonly string[] priorities = { "Cao", "Trung bình", "Thấp" };
        #endregion

        #region Constructor
        public TaskDetail(Models.WorkTask task, bool isViewOnly = false)
        {
            InitializeComponent();

            this.task = task;
            this.isViewOnly = isViewOnly;

            taskBLL = new TaskBLL();

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

            LoadDataFromDatabase();
            SetupForm();
            LoadTaskData();
        }
        #endregion

        #region Data Methods
        private void LoadDataFromDatabase()
        {
            try
            {
                projects = taskBLL.GetAllProjects();
                employees = taskBLL.GetAvailableEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTaskData()
        {
            // Cơ bản
            taskCodeTextBox.Text = task.TaskCode;
            taskNameTextBox.Text = task.TaskName;

            // Project
            int projectIndex = projects.FindIndex(p => p.ProjectID == task.ProjectID);
            if (projectIndex >= 0)
                projectComboBox.SelectedIndex = projectIndex;

            // Assigned To
            if (task.AssignedToID.HasValue)
            {
                int employeeIndex = employees.FindIndex(e => e.EmployeeID == task.AssignedToID.Value);
                if (employeeIndex >= 0)
                    assignedToComboBox.SelectedIndex = employeeIndex + 1; // +1 vì index 0 là "Chưa giao"
            }
            else
            {
                assignedToComboBox.SelectedIndex = 0; // "Chưa giao"
            }

            // Dates
            if (task.StartDate.HasValue)
                startDatePicker.Value = task.StartDate.Value;

            if (task.DueDate.HasValue)
                dueDatePicker.Value = task.DueDate.Value;

            if (task.CompletedDate.HasValue)
            {
                completedDateCheckBox.Checked = true;
                completedDatePicker.Value = task.CompletedDate.Value;
                completedDatePicker.Enabled = true;
            }
            else
            {
                completedDateCheckBox.Checked = false;
                completedDatePicker.Enabled = false;
            }

            // Status
            int statusIndex = Array.IndexOf(statuses, task.Status);
            if (statusIndex >= 0)
                statusComboBox.SelectedIndex = statusIndex;

            // Priority
            int priorityIndex = Array.IndexOf(priorities, task.Priority);
            if (priorityIndex >= 0)
                priorityComboBox.SelectedIndex = priorityIndex;

            // Completion
            completionTrackBar.Value = (int)task.CompletionPercentage;
            UpdateCompletionLabel();

            // Chi tiết
            descriptionTextBox.Text = task.Description;
            notesTextBox.Text = task.Notes;

            // Timestamps
            createdAtLabel.Text = $"Tạo lúc: {task.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}";
            updatedAtLabel.Text = $"Cập nhật lúc: {task.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss")}";

            // Nếu là view only thì disable tất cả controls
            if (isViewOnly)
            {
                DisableAllControls();
                saveButton.Visible = false;
                closeButton.Text = "✓ Đóng";
                this.Text = "Xem chi tiết công việc";
                titleLabel.Text = "👁️ XEM CHI TIẾT CÔNG VIỆC";
            }
        }

        private void DisableAllControls()
        {
            // Input controls
            taskCodeTextBox.ReadOnly = true;
            taskNameTextBox.ReadOnly = true;
            projectComboBox.Enabled = false;
            assignedToComboBox.Enabled = false;
            startDatePicker.Enabled = false;
            dueDatePicker.Enabled = false;
            completedDatePicker.Enabled = false;
            completedDateCheckBox.Enabled = false;
            statusComboBox.Enabled = false;
            priorityComboBox.Enabled = false;
            completionTrackBar.Enabled = false;
            descriptionTextBox.ReadOnly = true;
            notesTextBox.ReadOnly = true;
        }

        private bool ValidateForm()
        {
            if (isViewOnly) return true;

            bool isValid = true;

            // Xác thực Task Name
            if (string.IsNullOrWhiteSpace(taskNameTextBox.Text))
            {
                errorProvider.SetError(taskNameTextBox, "Tên công việc không được để trống");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(taskNameTextBox, "");
            }

            // Xác thực Project
            if (projectComboBox.SelectedIndex < 0)
            {
                errorProvider.SetError(projectComboBox, "Vui lòng chọn dự án");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(projectComboBox, "");
            }

            // Xác thực Due Date phải sau Start Date
            if (dueDatePicker.Value < startDatePicker.Value)
            {
                errorProvider.SetError(dueDatePicker, "Hạn hoàn thành phải sau ngày bắt đầu");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(dueDatePicker, "");
            }

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Chuyển đến tab cơ bản
            }

            return isValid;
        }

        private void UpdateTask()
        {
            try
            {
                // Cơ bản
                task.TaskName = taskNameTextBox.Text.Trim();
                task.ProjectID = projects[projectComboBox.SelectedIndex].ProjectID;

                if (assignedToComboBox.SelectedIndex > 0) // Skip "Chưa giao" option
                    task.AssignedToID = employees[assignedToComboBox.SelectedIndex - 1].EmployeeID;
                else
                    task.AssignedToID = null;

                task.StartDate = startDatePicker.Value;
                task.DueDate = dueDatePicker.Value;
                task.Status = statusComboBox.Text;
                task.Priority = priorityComboBox.Text;
                task.CompletionPercentage = completionTrackBar.Value;

                // Chi tiết
                task.Description = descriptionTextBox.Text.Trim();
                task.Notes = notesTextBox.Text.Trim();

                // Completed Date
                if (completedDateCheckBox.Checked)
                    task.CompletedDate = completedDatePicker.Value;
                else
                    task.CompletedDate = null;

                // Automatic rules
                if (task.Status == "Hoàn thành" && !task.CompletedDate.HasValue)
                {
                    task.CompletedDate = DateTime.Now;
                    task.CompletionPercentage = 100;
                }
                else if (task.Status != "Hoàn thành" && task.CompletionPercentage == 100)
                {
                    task.Status = "Hoàn thành";
                    if (!task.CompletedDate.HasValue)
                        task.CompletedDate = DateTime.Now;
                }

                // Cập nhật task vào database
                taskBLL.UpdateTask(task);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin công việc: {ex.Message}", ex);
            }
        }
        #endregion

        #region Event Handlers
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    UpdateTask();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật công việc: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (isViewOnly)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (MessageBox.Show("Bạn có chắc chắn muốn đóng mà không lưu thay đổi?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void CompletionTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompletionLabel();

            // Tự động chuyển trạng thái nếu tiến độ là 100%
            if (completionTrackBar.Value == 100 && statusComboBox.Text != "Hoàn thành")
            {
                statusComboBox.SelectedIndex = Array.IndexOf(statuses, "Hoàn thành");

                // Tự động chọn ngày hoàn thành
                if (!completedDateCheckBox.Checked)
                {
                    completedDateCheckBox.Checked = true;
                    completedDatePicker.Value = DateTime.Now;
                }
            }
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Tự động cập nhật tiến độ và ngày hoàn thành nếu trạng thái là Hoàn thành
            if (statusComboBox.Text == "Hoàn thành" && completionTrackBar.Value < 100)
            {
                completionTrackBar.Value = 100;

                // Tự động chọn ngày hoàn thành
                if (!completedDateCheckBox.Checked)
                {
                    completedDateCheckBox.Checked = true;
                    completedDatePicker.Value = DateTime.Now;
                }
            }
            // Nếu trạng thái là Chưa bắt đầu, đặt tiến độ về 0
            else if (statusComboBox.Text == "Chưa bắt đầu" && completionTrackBar.Value > 0)
            {
                completionTrackBar.Value = 0;
                completedDateCheckBox.Checked = false;
            }
        }

        private void CompletedDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            completedDatePicker.Enabled = completedDateCheckBox.Checked;

            // Nếu đã hoàn thành 100% và check Completed Date, tự động chuyển trạng thái thành Hoàn thành
            if (completedDateCheckBox.Checked && completionTrackBar.Value == 100)
            {
                statusComboBox.SelectedIndex = Array.IndexOf(statuses, "Hoàn thành");
            }
            // Nếu bỏ check Completed Date, đảm bảo trạng thái không phải là Hoàn thành
            else if (!completedDateCheckBox.Checked && statusComboBox.Text == "Hoàn thành")
            {
                statusComboBox.SelectedIndex = Array.IndexOf(statuses, "Đang thực hiện");
            }
        }

        private void UpdateCompletionLabel()
        {
            completionLabel.Text = $"{completionTrackBar.Value}% hoàn thành";

            // Thay đổi màu sắc tùy theo tiến độ
            if (completionTrackBar.Value == 100)
                completionLabel.ForeColor = Color.FromArgb(76, 175, 80);
            else if (completionTrackBar.Value >= 50)
                completionLabel.ForeColor = Color.FromArgb(33, 150, 243);
            else if (completionTrackBar.Value > 0)
                completionLabel.ForeColor = Color.FromArgb(255, 152, 0);
            else
                completionLabel.ForeColor = Color.FromArgb(158, 158, 158);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = (TabControl)sender;
            var tabPage = tabControl.TabPages[e.Index];
            var tabRect = tabControl.GetTabRect(e.Index);

            var brush = e.State == DrawItemState.Selected
                ? new SolidBrush(Color.FromArgb(33, 150, 243))
                : new SolidBrush(Color.FromArgb(240, 240, 240));

            e.Graphics.FillRectangle(brush, tabRect);

            var textColor = e.State == DrawItemState.Selected ? Color.White : Color.FromArgb(64, 64, 64);
            var textBrush = new SolidBrush(textColor);
            var font = new Font("Segoe UI", 10, FontStyle.Bold);

            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            e.Graphics.DrawString(tabPage.Text, font, textBrush, tabRect, stringFormat);

            brush.Dispose();
            textBrush.Dispose();
            font.Dispose();
        }
        #endregion

        #region UI Setup Methods
        private void SetupForm()
        {
            this.Text = isViewOnly ? "Xem chi tiết công việc" : "Chỉnh sửa công việc";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            SetupLayout();
            SetupHeader();
            SetupContent();
            SetupFooter();
        }

        private void SetupLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(25),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Define row heights
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            titleLabel = new Label
            {
                Text = isViewOnly ? "👁️ XEM CHI TIẾT CÔNG VIỆC" : "✏️ CHỈNH SỬA CÔNG VIỆC",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            var subtitleLabel = new Label
            {
                Text = isViewOnly ? "Xem thông tin chi tiết của công việc" : "Chỉnh sửa thông tin công việc trong hệ thống",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(20, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                ItemSize = new Size(150, 40),
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };

            tabControl.DrawItem += TabControl_DrawItem;

            SetupBasicInfoTab();
            SetupDetailsTab();

            tabControl.TabPages.Add(basicInfoTab);
            tabControl.TabPages.Add(detailsTab);

            contentPanel.Controls.Add(tabControl);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupBasicInfoTab()
        {
            basicInfoTab = new TabPage
            {
                Text = "Thông tin cơ bản",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 8,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            // Column widths
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Row heights
            for (int i = 0; i < 8; i++)
                basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));

            // Task Code (Read-only)
            basicLayout.Controls.Add(CreateLabel("Mã công việc:", false), 0, 0);
            taskCodeTextBox = CreateTextBox();
            taskCodeTextBox.ReadOnly = true;
            basicLayout.Controls.Add(taskCodeTextBox, 1, 0);

            // Task Name (Required)
            basicLayout.Controls.Add(CreateLabel("Tên công việc *:", true), 2, 0);
            taskNameTextBox = CreateTextBox();
            basicLayout.Controls.Add(taskNameTextBox, 3, 0);

            // Project (Required)
            basicLayout.Controls.Add(CreateLabel("Dự án *:", true), 0, 1);
            projectComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5),
                DisplayMember = "ProjectName",
                ValueMember = "ProjectID"
            };

            foreach (var project in projects)
            {
                projectComboBox.Items.Add(project.ProjectName);
            }

            basicLayout.Controls.Add(projectComboBox, 1, 1);

            // Assigned To
            basicLayout.Controls.Add(CreateLabel("Người thực hiện:", false), 2, 1);
            assignedToComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5),
                DisplayMember = "FullName",
                ValueMember = "EmployeeID"
            };

            // Thêm tùy chọn "Chưa giao"
            assignedToComboBox.Items.Add("Chưa giao");

            foreach (var employee in employees)
            {
                assignedToComboBox.Items.Add(employee.FullName);
            }

            basicLayout.Controls.Add(assignedToComboBox, 3, 1);

            // Start Date
            basicLayout.Controls.Add(CreateLabel("Ngày bắt đầu:", false), 0, 2);
            startDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
            basicLayout.Controls.Add(startDatePicker, 1, 2);

            // Due Date
            basicLayout.Controls.Add(CreateLabel("Hạn hoàn thành:", false), 2, 2);
            dueDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
            basicLayout.Controls.Add(dueDatePicker, 3, 2);

            // Completed Date (Optional with checkbox)
            var completedDatePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            completedDateCheckBox = new CheckBox
            {
                Text = "Ngày hoàn thành:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(0, 5),
                Size = new Size(150, 25),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            completedDateCheckBox.CheckedChanged += CompletedDateCheckBox_CheckedChanged;

            completedDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Location = new Point(0, 35),
                Size = new Size(180, 30),
                Enabled = false
            };

            completedDatePanel.Controls.Add(completedDatePicker);
            completedDatePanel.Controls.Add(completedDateCheckBox);
            basicLayout.Controls.Add(completedDatePanel, 0, 3);
            basicLayout.SetColumnSpan(completedDatePanel, 2);

            // Status
            basicLayout.Controls.Add(CreateLabel("Trạng thái:", false), 2, 3);
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            statusComboBox.Items.AddRange(statuses);
            statusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(statusComboBox, 3, 3);

            // Priority
            basicLayout.Controls.Add(CreateLabel("Ưu tiên:", false), 0, 4);
            priorityComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            priorityComboBox.Items.AddRange(priorities);
            basicLayout.Controls.Add(priorityComboBox, 1, 4);

            // Completion Percentage
            var completionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                Padding = new Padding(0, 10, 0, 0)
            };

            var completionLabel1 = new Label
            {
                Text = "Tiến độ hoàn thành:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Top,
                Height = 25
            };

            completionTrackBar = new TrackBar
            {
                Dock = DockStyle.Fill,
                TickStyle = TickStyle.Both,
                Minimum = 0,
                Maximum = 100,
                LargeChange = 10,
                SmallChange = 5,
                Value = 0
            };
            completionTrackBar.ValueChanged += CompletionTrackBar_ValueChanged;

            completionLabel = new Label
            {
                Text = "0% hoàn thành",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(158, 158, 158),
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter
            };

            completionPanel.Controls.Add(completionLabel);
            completionPanel.Controls.Add(completionTrackBar);
            completionPanel.Controls.Add(completionLabel1);

            basicLayout.Controls.Add(completionPanel, 2, 4);
            basicLayout.SetColumnSpan(completionPanel, 2);

            // Timestamp info
            createdAtLabel = new Label
            {
                Text = "Tạo lúc: ...",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(128, 128, 128),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5)
            };
            basicLayout.Controls.Add(createdAtLabel, 0, 7);
            basicLayout.SetColumnSpan(createdAtLabel, 2);

            updatedAtLabel = new Label
            {
                Text = "Cập nhật lúc: ...",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(128, 128, 128),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5)
            };
            basicLayout.Controls.Add(updatedAtLabel, 2, 7);
            basicLayout.SetColumnSpan(updatedAtLabel, 2);

            basicInfoTab.Controls.Add(basicLayout);
        }

        private void SetupDetailsTab()
        {
            detailsTab = new TabPage
            {
                Text = "Thông tin chi tiết",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            // Description
            var descriptionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var descriptionLabel = new Label
            {
                Text = "Mô tả công việc:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            descriptionTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 5, 0, 0)
            };

            descriptionPanel.Controls.Add(descriptionTextBox);
            descriptionPanel.Controls.Add(descriptionLabel);
            detailsLayout.Controls.Add(descriptionPanel, 0, 0);

            // Notes
            var notesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var notesLabel = new Label
            {
                Text = "Ghi chú:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            notesTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 5, 0, 0)
            };

            notesPanel.Controls.Add(notesTextBox);
            notesPanel.Controls.Add(notesLabel);
            detailsLayout.Controls.Add(notesPanel, 0, 1);

            detailsTab.Controls.Add(detailsLayout);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(25, 15, 25, 15)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            saveButton = CreateFooterButton("💾 Lưu thay đổi", Color.FromArgb(76, 175, 80));
            saveButton.Click += SaveButton_Click;
            if (!isViewOnly) buttonPanel.Controls.Add(saveButton);

            closeButton = CreateFooterButton(isViewOnly ? "✓ Đóng" : "❌ Hủy", isViewOnly ? Color.FromArgb(76, 175, 80) : Color.FromArgb(158, 158, 158));
            closeButton.Click += CloseButton_Click;
            buttonPanel.Controls.Add(closeButton);

            footerPanel.Controls.Add(buttonPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }

        private Label CreateLabel(string text, bool isRequired = false)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = isRequired ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5)
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Height = 30
            };
        }

        private Button CreateFooterButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(140, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }
        #endregion
    }
}