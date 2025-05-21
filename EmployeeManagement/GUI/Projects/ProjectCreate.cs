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
    public partial class ProjectCreate : Form
    {
        #region Fields
        private Models.Project project;
        private ProjectBLL projectBLL;
        private ErrorProvider errorProvider;
        private List<Models.Employee> managers;

        // UI Controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        private Label titleLabel;
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage detailsTab;

        // Basic info tab controls
        private TextBox projectCodeTextBox;
        private TextBox projectNameTextBox;
        private ComboBox managerComboBox;
        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
        private NumericUpDown budgetNumeric;
        private ComboBox statusComboBox;
        private TrackBar completionTrackBar;
        private Label completionLabel;

        // Details tab controls
        private TextBox descriptionTextBox;
        private TextBox notesTextBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        private readonly string[] statuses = { "Khởi tạo", "Đang thực hiện", "Hoàn thành", "Tạm dừng", "Hủy bỏ" };
        #endregion

        #region Constructor
        public ProjectCreate()
        {
            InitializeComponent();

            projectBLL = new ProjectBLL();
            project = new Models.Project();

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

            LoadDataFromDatabase();
            SetupForm();
            SetDefaultValues();
        }

        public Models.Project CreatedProject => project;
        #endregion

        #region Data Methods
        private void LoadDataFromDatabase()
        {
            try
            {
                managers = projectBLL.GetAvailableManagers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultValues()
        {
            project.ProjectCode = projectBLL.GenerateProjectCode();
            projectCodeTextBox.Text = project.ProjectCode;

            statusComboBox.SelectedIndex = 0; // Khởi tạo

            if (managerComboBox.Items.Count > 0)
                managerComboBox.SelectedIndex = 0;

            startDatePicker.Value = DateTime.Now.Date;
            endDatePicker.Value = DateTime.Now.Date.AddMonths(3); // Default 3 months project
            budgetNumeric.Value = 100000000; // Default 100 million VND
            completionTrackBar.Value = 0;
            UpdateCompletionLabel();
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Validate Project Code
            if (string.IsNullOrWhiteSpace(projectCodeTextBox.Text))
            {
                errorProvider.SetError(projectCodeTextBox, "Mã dự án không được để trống");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(projectCodeTextBox, "");
            }

            // Validate Project Name
            if (string.IsNullOrWhiteSpace(projectNameTextBox.Text))
            {
                errorProvider.SetError(projectNameTextBox, "Tên dự án không được để trống");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(projectNameTextBox, "");
            }

            // Validate Manager
            if (managerComboBox.SelectedIndex < 0)
            {
                errorProvider.SetError(managerComboBox, "Vui lòng chọn quản lý dự án");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(managerComboBox, "");
            }

            // Validate Dates
            if (endDatePicker.Value < startDatePicker.Value)
            {
                errorProvider.SetError(endDatePicker, "Ngày kết thúc phải sau ngày bắt đầu");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(endDatePicker, "");
            }

            // Validate Budget
            if (budgetNumeric.Value <= 0)
            {
                errorProvider.SetError(budgetNumeric, "Ngân sách phải lớn hơn 0");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(budgetNumeric, "");
            }

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Switch to basic info tab
            }

            return isValid;
        }

        private void SaveProject()
        {
            try
            {
                // Basic info
                project.ProjectCode = projectCodeTextBox.Text.Trim();
                project.ProjectName = projectNameTextBox.Text.Trim();
                project.ManagerID = managers[managerComboBox.SelectedIndex].EmployeeID;
                project.StartDate = startDatePicker.Value;
                project.EndDate = endDatePicker.Value;
                project.Budget = budgetNumeric.Value;
                project.Status = statusComboBox.Text;
                project.CompletionPercentage = completionTrackBar.Value;

                // Details
                project.Description = descriptionTextBox.Text.Trim();
                project.Notes = notesTextBox.Text.Trim();

                // Add project to database
                int projectId = projectBLL.AddProject(project);
                project.ProjectID = projectId;

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin dự án: {ex.Message}", ex);
            }
        }

        private void ResetForm()
        {
            projectNameTextBox.Clear();
            descriptionTextBox.Clear();
            notesTextBox.Clear();

            SetDefaultValues();
            errorProvider.Clear();
            tabControl.SelectedIndex = 0;
        }
        #endregion

        #region Event Handlers
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveProject();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu dự án: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả thông tin?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ResetForm();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CompletionTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompletionLabel();

            // Auto-update status if completion is 100%
            if (completionTrackBar.Value == 100 && statusComboBox.Text != "Hoàn thành")
            {
                statusComboBox.SelectedIndex = Array.IndexOf(statuses, "Hoàn thành");
            }
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-update completion if status is completed
            if (statusComboBox.Text == "Hoàn thành" && completionTrackBar.Value < 100)
            {
                completionTrackBar.Value = 100;
            }
            // If status is initial, set completion to 0
            else if (statusComboBox.Text == "Khởi tạo" && completionTrackBar.Value > 0)
            {
                completionTrackBar.Value = 0;
            }
        }

        private void UpdateCompletionLabel()
        {
            completionLabel.Text = $"{completionTrackBar.Value}% hoàn thành";

            // Change color based on completion
            if (completionTrackBar.Value == 100)
                completionLabel.ForeColor = Color.FromArgb(76, 175, 80);
            else if (completionTrackBar.Value >= 75)
                completionLabel.ForeColor = Color.FromArgb(139, 195, 74);
            else if (completionTrackBar.Value >= 50)
                completionLabel.ForeColor = Color.FromArgb(255, 152, 0);
            else if (completionTrackBar.Value >= 25)
                completionLabel.ForeColor = Color.FromArgb(255, 193, 7);
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
            this.Text = "Tạo dự án mới";
            this.Size = new Size(900, 700);
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
                Text = "📋 TẠO DỰ ÁN MỚI",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            var subtitleLabel = new Label
            {
                Text = "Nhập thông tin để tạo dự án mới trong hệ thống",
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
                RowCount = 7,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            // Column widths
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Row heights
            for (int i = 0; i < 7; i++)
                basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));

            // Project Code (Generated)
            basicLayout.Controls.Add(CreateLabel("Mã dự án *:", true), 0, 0);
            projectCodeTextBox = CreateTextBox();
            projectCodeTextBox.ReadOnly = true;
            basicLayout.Controls.Add(projectCodeTextBox, 1, 0);

            // Project Name (Required)
            basicLayout.Controls.Add(CreateLabel("Tên dự án *:", true), 2, 0);
            projectNameTextBox = CreateTextBox();
            basicLayout.Controls.Add(projectNameTextBox, 3, 0);

            // Manager (Required)
            basicLayout.Controls.Add(CreateLabel("Quản lý dự án *:", true), 0, 1);
            managerComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5),
                DisplayMember = "FullName",
                ValueMember = "EmployeeID"
            };

            foreach (var manager in managers)
            {
                managerComboBox.Items.Add(manager.FullName);
            }

            if (managerComboBox.Items.Count > 0)
                managerComboBox.SelectedIndex = 0;

            basicLayout.Controls.Add(managerComboBox, 1, 1);

            // Status
            basicLayout.Controls.Add(CreateLabel("Trạng thái:", false), 2, 1);
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            statusComboBox.Items.AddRange(statuses);
            statusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(statusComboBox, 3, 1);

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

            // End Date
            basicLayout.Controls.Add(CreateLabel("Ngày kết thúc:", false), 2, 2);
            endDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
            basicLayout.Controls.Add(endDatePicker, 3, 2);

            // Budget
            basicLayout.Controls.Add(CreateLabel("Ngân sách (VNĐ):", false), 0, 3);
            budgetNumeric = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Margin = new Padding(5),
                Minimum = 1000,
                Maximum = 999999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };
            basicLayout.Controls.Add(budgetNumeric, 1, 3);

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

            basicLayout.Controls.Add(completionPanel, 2, 3);
            basicLayout.SetColumnSpan(completionPanel, 2);

            // Required fields info
            var requiredFieldsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var requiredFieldsLabel = new Label
            {
                Text = "💡 Các trường có dấu (*) là bắt buộc phải nhập",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10)
            };

            requiredFieldsPanel.Controls.Add(requiredFieldsLabel);
            basicLayout.Controls.Add(requiredFieldsPanel, 0, 6);
            basicLayout.SetColumnSpan(requiredFieldsPanel, 4);

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
                Text = "Mô tả dự án:",
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

            saveButton = CreateFooterButton("💾 Tạo dự án", Color.FromArgb(76, 175, 80));
            saveButton.Click += SaveButton_Click;

            resetButton = CreateFooterButton("🔄 Đặt lại", Color.FromArgb(255, 152, 0));
            resetButton.Click += ResetButton_Click;

            cancelButton = CreateFooterButton("❌ Hủy", Color.FromArgb(158, 158, 158));
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(resetButton);
            buttonPanel.Controls.Add(cancelButton);

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