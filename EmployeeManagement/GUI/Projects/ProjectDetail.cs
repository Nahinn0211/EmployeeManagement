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
    public partial class ProjectDetail : Form
    {
        #region Fields
        private Models.Project project;
        private bool isReadOnly;
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private Label statusLabel;
        private PictureBox projectIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage generalTab;
        private TabPage detailsTab;
        private TabPage timelineTab;

        // General tab controls
        private TextBox projectCodeTextBox;
        private TextBox projectNameTextBox;
        private TextBox customerTextBox;
        private TextBox managerTextBox;
        private ComboBox statusComboBox;
        private TextBox budgetTextBox;
        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
        private ProgressBar progressBar;
        private Label progressLabel;

        // Details tab controls
        private TextBox descriptionTextBox;
        private ListBox requirementsListBox;
        private ListBox technologiesListBox;

        // Timeline tab controls
        private ListView timelineListView;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button editButton;

        private readonly string[] projectStatuses = { "Planning", "In Progress", "Completed", "On Hold" };
        #endregion

        #region Constructor
        public ProjectDetail(Models.Project project = null, bool readOnly = false)
        {
            this.project = project ?? new Models.Project();
            this.isReadOnly = readOnly;
            InitializeComponent();
            SetupForm();
            LoadProjectData();
        }

        public ProjectDetail() : this(null, false)
        {
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = project.ProjectID == 0 ? "Thêm dự án mới" :
                       (isReadOnly ? "Chi tiết dự án" : "Chỉnh sửa dự án");
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

            // Apply read-only state if needed
            if (isReadOnly)
                SetReadOnlyMode();
        }

        private void SetupLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Define row heights
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            // Project icon
            projectIcon = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create project icon
            CreateProjectIcon();

            // Title label
            titleLabel = new Label
            {
                Text = project.ProjectName ?? "Dự án mới",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(120, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            // Status label
            statusLabel = new Label
            {
                Text = GetStatusDisplayText(project.Status ?? "Planning"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(120, 65),
                Size = new Size(400, 25),
                ForeColor = GetStatusColor(project.Status ?? "Planning")
            };

            headerPanel.Controls.Add(projectIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statusLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateProjectIcon()
        {
            var bmp = new Bitmap(80, 80);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 20, FontStyle.Bold))
                {
                    var text = "📋";
                    var size = g.MeasureString(text, font);
                    var x = (80 - size.Width) / 2;
                    var y = (80 - size.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            projectIcon.Image = bmp;
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
                Font = new Font("Segoe UI", 10),
                ItemSize = new Size(120, 35),
                SizeMode = TabSizeMode.Fixed
            };

            SetupGeneralTab();
            SetupDetailsTab();
            SetupTimelineTab();

            tabControl.TabPages.Add(generalTab);
            tabControl.TabPages.Add(detailsTab);
            tabControl.TabPages.Add(timelineTab);

            contentPanel.Controls.Add(tabControl);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupGeneralTab()
        {
            generalTab = new TabPage
            {
                Text = "Thông tin chung",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var generalLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 480,
                ColumnCount = 4,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            // Column widths
            generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Row heights
            for (int i = 0; i < 6; i++)
                generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            // Project Code
            generalLayout.Controls.Add(CreateLabel("Mã dự án:"), 0, 0);
            projectCodeTextBox = CreateTextBox();
            generalLayout.Controls.Add(projectCodeTextBox, 1, 0);

            // Status
            generalLayout.Controls.Add(CreateLabel("Trạng thái:"), 2, 0);
            statusComboBox = CreateComboBox(projectStatuses);
            statusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;
            generalLayout.Controls.Add(statusComboBox, 3, 0);

            // Project Name
            generalLayout.Controls.Add(CreateLabel("Tên dự án:"), 0, 1);
            projectNameTextBox = CreateTextBox();
            projectNameTextBox.TextChanged += ProjectNameTextBox_TextChanged;
            generalLayout.Controls.Add(projectNameTextBox, 1, 1);
            generalLayout.SetColumnSpan(projectNameTextBox, 3);

            // Customer
            generalLayout.Controls.Add(CreateLabel("Khách hàng:"), 0, 2);
            customerTextBox = CreateTextBox();
            generalLayout.Controls.Add(customerTextBox, 1, 2);

            // Manager
            generalLayout.Controls.Add(CreateLabel("Quản lý:"), 2, 2);
            managerTextBox = CreateTextBox();
            generalLayout.Controls.Add(managerTextBox, 3, 2);

            // Budget
            generalLayout.Controls.Add(CreateLabel("Ngân sách:"), 0, 3);
            budgetTextBox = CreateTextBox();
            generalLayout.Controls.Add(budgetTextBox, 1, 3);

            // Progress
            generalLayout.Controls.Add(CreateLabel("Tiến độ:"), 2, 3);
            var progressPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 25,
                Style = ProgressBarStyle.Continuous
            };
            progressLabel = new Label
            {
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 25,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            progressPanel.Controls.Add(progressBar);
            progressPanel.Controls.Add(progressLabel);
            generalLayout.Controls.Add(progressPanel, 3, 3);

            // Start Date
            generalLayout.Controls.Add(CreateLabel("Ngày bắt đầu:"), 0, 4);
            startDatePicker = CreateDateTimePicker();
            startDatePicker.ValueChanged += DatePicker_ValueChanged;
            generalLayout.Controls.Add(startDatePicker, 1, 4);

            // End Date
            generalLayout.Controls.Add(CreateLabel("Ngày kết thúc:"), 2, 4);
            endDatePicker = CreateDateTimePicker();
            endDatePicker.ValueChanged += DatePicker_ValueChanged;
            generalLayout.Controls.Add(endDatePicker, 3, 4);

            scrollPanel.Controls.Add(generalLayout);
            generalTab.Controls.Add(scrollPanel);
        }

        private void SetupDetailsTab()
        {
            detailsTab = new TabPage
            {
                Text = "Chi tiết",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

            // Description
            var descPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            descPanel.Controls.Add(CreateLabel("Mô tả dự án:", true));
            descriptionTextBox = CreateMultilineTextBox();
            descPanel.Controls.Add(descriptionTextBox);
            detailsLayout.Controls.Add(descPanel, 0, 0);
            detailsLayout.SetColumnSpan(descPanel, 2);

            // Requirements
            var reqPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            reqPanel.Controls.Add(CreateLabel("Yêu cầu:", true));
            requirementsListBox = CreateListBox();
            reqPanel.Controls.Add(requirementsListBox);
            detailsLayout.Controls.Add(reqPanel, 0, 1);
            detailsLayout.SetRowSpan(reqPanel, 2);

            // Technologies
            var techPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            techPanel.Controls.Add(CreateLabel("Công nghệ sử dụng:", true));
            technologiesListBox = CreateListBox();
            techPanel.Controls.Add(technologiesListBox);
            detailsLayout.Controls.Add(techPanel, 1, 1);
            detailsLayout.SetRowSpan(techPanel, 2);

            detailsTab.Controls.Add(detailsLayout);
        }

        private void SetupTimelineTab()
        {
            timelineTab = new TabPage
            {
                Text = "Lịch sử",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var timelinePanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            timelinePanel.Controls.Add(CreateLabel("Lịch sử dự án:", true));

            timelineListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 30, 0, 0)
            };

            timelineListView.Columns.Add("Ngày", 120);
            timelineListView.Columns.Add("Sự kiện", 200);
            timelineListView.Columns.Add("Mô tả", 300);
            timelineListView.Columns.Add("Người thực hiện", 150);

            timelinePanel.Controls.Add(timelineListView);
            timelineTab.Controls.Add(timelinePanel);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            if (isReadOnly)
            {
                editButton = CreateFooterButton("✏️ Chỉnh sửa", Color.FromArgb(255, 152, 0));
                editButton.Click += EditButton_Click;
                buttonPanel.Controls.Add(editButton);
            }
            else
            {
                saveButton = CreateFooterButton("💾 Lưu", Color.FromArgb(76, 175, 80));
                saveButton.Click += SaveButton_Click;
                buttonPanel.Controls.Add(saveButton);
            }

            cancelButton = CreateFooterButton("❌ Hủy", Color.FromArgb(158, 158, 158));
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            buttonPanel.Controls.Add(cancelButton);

            footerPanel.Controls.Add(buttonPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }
        #endregion

        #region Control Creators
        private Label CreateLabel(string text, bool isDockTop = false)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = isDockTop ? DockStyle.Top : DockStyle.Fill,
                Height = isDockTop ? 25 : 0,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5)
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Height = 25
            };
        }

        private TextBox CreateMultilineTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 30, 0, 0)
            };
        }

        private ComboBox CreateComboBox(string[] items)
        {
            var combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            combo.Items.AddRange(items);
            return combo;
        }

        private DateTimePicker CreateDateTimePicker()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
        }

        private ListBox CreateListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 30, 0, 0)
            };
        }

        private Button CreateFooterButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(120, 40),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }
        #endregion

        #region Data Loading
        private void LoadProjectData()
        {
            if (project == null) return;

            // General tab
            projectCodeTextBox.Text = project.ProjectCode ?? "";
            projectNameTextBox.Text = project.ProjectName ?? "";
            customerTextBox.Text = $"Khách hàng {project.CustomerID}";
            managerTextBox.Text = $"Quản lý {project.ManagerID}";
            budgetTextBox.Text = project.Budget.ToString("C0");

            if (statusComboBox.Items.Contains(project.Status))
                statusComboBox.SelectedItem = project.Status;
            else
                statusComboBox.SelectedIndex = 0;

            startDatePicker.Value = project.StartDate == DateTime.MinValue ? DateTime.Now : project.StartDate;
            endDatePicker.Value = project.EndDate == DateTime.MinValue ? DateTime.Now.AddMonths(6) : project.EndDate;

            UpdateProgress();

            // Update title
            titleLabel.Text = project.ProjectName ?? "Dự án mới";
            statusLabel.Text = GetStatusDisplayText(project.Status ?? "Planning");
            statusLabel.ForeColor = GetStatusColor(project.Status ?? "Planning");

            // Details tab
            descriptionTextBox.Text = project.Description ?? "";

            // Sample requirements
            requirementsListBox.Items.AddRange(new[]
            {
                "Phân tích yêu cầu nghiệp vụ",
                "Thiết kế giao diện người dùng",
                "Phát triển backend API",
                "Tích hợp cơ sở dữ liệu",
                "Testing và QA",
                "Deployment và maintenance"
            });

            // Sample technologies
            technologiesListBox.Items.AddRange(new[]
            {
                "C# .NET Framework",
                "SQL Server",
                "Windows Forms",
                "Entity Framework",
                "Git Version Control"
            });

            // Timeline tab
            LoadTimeline();
        }

        private void LoadTimeline()
        {
            timelineListView.Items.Clear();

            var timelineEvents = new[]
            {
                new { Date = project.CreatedAt, Event = "Tạo dự án", Description = "Dự án được khởi tạo", User = "Admin" },
                new { Date = project.StartDate, Event = "Bắt đầu", Description = "Bắt đầu thực hiện dự án", User = $"Quản lý {project.ManagerID}" },
                new { Date = DateTime.Now, Event = "Cập nhật", Description = "Cập nhật thông tin dự án", User = "User" }
            };

            foreach (var evt in timelineEvents)
            {
                var item = new ListViewItem(evt.Date.ToString("dd/MM/yyyy"));
                item.SubItems.Add(evt.Event);
                item.SubItems.Add(evt.Description);
                item.SubItems.Add(evt.User);
                timelineListView.Items.Add(item);
            }
        }

        private void UpdateProgress()
        {
            var totalDays = (endDatePicker.Value - startDatePicker.Value).TotalDays;
            if (totalDays <= 0)
            {
                progressBar.Value = 0;
                progressLabel.Text = "0%";
                return;
            }

            var elapsedDays = (DateTime.Now - startDatePicker.Value).TotalDays;
            var progress = Math.Max(0, Math.Min(100, (elapsedDays / totalDays) * 100));

            if (statusComboBox.SelectedItem?.ToString() == "Completed") progress = 100;
            else if (statusComboBox.SelectedItem?.ToString() == "Planning") progress = 0;

            progressBar.Value = (int)progress;
            progressLabel.Text = $"{progress:F0}%";
            progressLabel.ForeColor = GetProgressColor((int)progress);
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

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Planning" => Color.FromArgb(255, 152, 0),
                "In Progress" => Color.FromArgb(33, 150, 243),
                "Completed" => Color.FromArgb(76, 175, 80),
                "On Hold" => Color.FromArgb(244, 67, 54),
                _ => Color.FromArgb(64, 64, 64)
            };
        }

        private Color GetProgressColor(int progress)
        {
            return progress switch
            {
                >= 80 => Color.FromArgb(76, 175, 80),
                >= 50 => Color.FromArgb(255, 152, 0),
                _ => Color.FromArgb(244, 67, 54)
            };
        }

        private void SetReadOnlyMode()
        {
            projectCodeTextBox.ReadOnly = true;
            projectNameTextBox.ReadOnly = true;
            customerTextBox.ReadOnly = true;
            managerTextBox.ReadOnly = true;
            budgetTextBox.ReadOnly = true;
            statusComboBox.Enabled = false;
            startDatePicker.Enabled = false;
            endDatePicker.Enabled = false;
            descriptionTextBox.ReadOnly = true;

            // Change background color for read-only controls
            var readOnlyColor = Color.FromArgb(248, 249, 250);
            projectCodeTextBox.BackColor = readOnlyColor;
            projectNameTextBox.BackColor = readOnlyColor;
            customerTextBox.BackColor = readOnlyColor;
            managerTextBox.BackColor = readOnlyColor;
            budgetTextBox.BackColor = readOnlyColor;
            descriptionTextBox.BackColor = readOnlyColor;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(projectCodeTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập mã dự án!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                projectCodeTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(projectNameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên dự án!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                projectNameTextBox.Focus();
                return false;
            }

            if (endDatePicker.Value <= startDatePicker.Value)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                endDatePicker.Focus();
                return false;
            }

            return true;
        }

        private void SaveProject()
        {
            if (!ValidateInput()) return;

            try
            {
                project.ProjectCode = projectCodeTextBox.Text.Trim();
                project.ProjectName = projectNameTextBox.Text.Trim();
                project.Description = descriptionTextBox.Text.Trim();
                project.Status = statusComboBox.SelectedItem?.ToString() ?? "Planning";
                project.StartDate = startDatePicker.Value;
                project.EndDate = endDatePicker.Value;

                // Parse budget
                if (decimal.TryParse(budgetTextBox.Text.Replace("₫", "").Replace(",", ""), out decimal budget))
                    project.Budget = budget;

                // Update header
                titleLabel.Text = project.ProjectName;
                statusLabel.Text = GetStatusDisplayText(project.Status);
                statusLabel.ForeColor = GetStatusColor(project.Status);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dự án: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void ProjectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            titleLabel.Text = string.IsNullOrWhiteSpace(projectNameTextBox.Text) ? "Dự án mới" : projectNameTextBox.Text;
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var status = statusComboBox.SelectedItem?.ToString() ?? "Planning";
            statusLabel.Text = GetStatusDisplayText(status);
            statusLabel.ForeColor = GetStatusColor(status);
            UpdateProgress();
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateProgress();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            // Switch to edit mode
            isReadOnly = false;
            SetupFooter(); // Recreate footer with save/cancel buttons

            // Enable controls
            projectCodeTextBox.ReadOnly = false;
            projectNameTextBox.ReadOnly = false;
            customerTextBox.ReadOnly = false;
            managerTextBox.ReadOnly = false;
            budgetTextBox.ReadOnly = false;
            statusComboBox.Enabled = true;
            startDatePicker.Enabled = true;
            endDatePicker.Enabled = true;
            descriptionTextBox.ReadOnly = true;

            // Restore background colors
            projectCodeTextBox.BackColor = Color.White;
            projectNameTextBox.BackColor = Color.White;
            customerTextBox.BackColor = Color.White;
            managerTextBox.BackColor = Color.White;
            budgetTextBox.BackColor = Color.White;
            descriptionTextBox.BackColor = Color.White;
        }
        #endregion
    }
}