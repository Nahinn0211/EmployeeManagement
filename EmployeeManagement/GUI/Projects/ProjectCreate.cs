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
    public partial class ProjectCreate : Form
    {
        #region Fields
        private Models.Project project;
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox projectIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage detailsTab;

        // Basic info tab controls
        private TextBox projectCodeTextBox;
        private TextBox projectNameTextBox;
        private ComboBox customerComboBox;
        private ComboBox managerComboBox;
        private ComboBox statusComboBox;
        private TextBox budgetTextBox;
        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
        private ComboBox priorityComboBox;

        // Details tab controls
        private TextBox descriptionTextBox;
        private TextBox objectivesTextBox;
        private ListBox requirementsListBox;
        private ListBox technologiesListBox;
        private Button addRequirementButton;
        private Button removeRequirementButton;
        private Button addTechnologyButton;
        private Button removeTechnologyButton;
        private TextBox newRequirementTextBox;
        private TextBox newTechnologyTextBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        private readonly string[] projectStatuses = { "Planning", "In Progress", "Completed", "On Hold" };
        private readonly string[] priorities = { "Low", "Medium", "High", "Critical" };
        private readonly string[] customers = { "Khách hàng 1", "Khách hàng 2", "Khách hàng 3", "Khách hàng 4", "Khách hàng 5" };
        private readonly string[] managers = { "Quản lý 1", "Quản lý 2", "Quản lý 3", "Quản lý 4", "Quản lý 5" };

        // Validation
        private ErrorProvider errorProvider;
        #endregion

        #region Constructor
        public ProjectCreate()
        {
            this.project = new Models.Project();
            InitializeComponent();
            SetupForm();
            SetDefaultValues();
        }

        public Models.Project CreatedProject => project;
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Tạo dự án mới";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

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
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
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

            // Project icon
            projectIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(76, 175, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateProjectIcon();

            // Title label
            titleLabel = new Label
            {
                Text = "🚀 TẠO DỰ ÁN MỚI",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            var subtitleLabel = new Label
            {
                Text = "Nhập thông tin để tạo dự án mới trong hệ thống",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(projectIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateProjectIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(76, 175, 80));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    var text = "➕";
                    var size = g.MeasureString(text, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
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
                Padding = new Padding(25)
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 600,
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
                basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));

            // Project Code (Required)
            basicLayout.Controls.Add(CreateLabel("Mã dự án *:", true), 0, 0);
            projectCodeTextBox = CreateTextBox();
            projectCodeTextBox.Leave += ProjectCodeTextBox_Leave;
            basicLayout.Controls.Add(projectCodeTextBox, 1, 0);

            // Status
            basicLayout.Controls.Add(CreateLabel("Trạng thái:", true), 2, 0);
            statusComboBox = CreateComboBox(projectStatuses);
            statusComboBox.SelectedIndex = 0; // Default: Planning
            basicLayout.Controls.Add(statusComboBox, 3, 0);

            // Project Name (Required)
            basicLayout.Controls.Add(CreateLabel("Tên dự án *:", true), 0, 1);
            projectNameTextBox = CreateTextBox();
            projectNameTextBox.Leave += ProjectNameTextBox_Leave;
            basicLayout.Controls.Add(projectNameTextBox, 1, 1);
            basicLayout.SetColumnSpan(projectNameTextBox, 3);

            // Customer
            basicLayout.Controls.Add(CreateLabel("Khách hàng:", true), 0, 2);
            customerComboBox = CreateComboBox(customers);
            customerComboBox.SelectedIndex = 0;
            basicLayout.Controls.Add(customerComboBox, 1, 2);

            // Manager
            basicLayout.Controls.Add(CreateLabel("Quản lý dự án:", true), 2, 2);
            managerComboBox = CreateComboBox(managers);
            managerComboBox.SelectedIndex = 0;
            basicLayout.Controls.Add(managerComboBox, 3, 2);

            // Budget
            basicLayout.Controls.Add(CreateLabel("Ngân sách (VND):", true), 0, 3);
            budgetTextBox = CreateTextBox();
            budgetTextBox.Leave += BudgetTextBox_Leave;
            budgetTextBox.KeyPress += BudgetTextBox_KeyPress;
            basicLayout.Controls.Add(budgetTextBox, 1, 3);

            // Priority
            basicLayout.Controls.Add(CreateLabel("Độ ưu tiên:", true), 2, 3);
            priorityComboBox = CreateComboBox(priorities);
            priorityComboBox.SelectedIndex = 1; // Default: Medium
            basicLayout.Controls.Add(priorityComboBox, 3, 3);

            // Start Date (Required)
            basicLayout.Controls.Add(CreateLabel("Ngày bắt đầu *:", true), 0, 4);
            startDatePicker = CreateDateTimePicker();
            startDatePicker.Value = DateTime.Now.Date;
            startDatePicker.ValueChanged += StartDatePicker_ValueChanged;
            basicLayout.Controls.Add(startDatePicker, 1, 4);

            // End Date (Required)
            basicLayout.Controls.Add(CreateLabel("Ngày kết thúc *:", true), 2, 4);
            endDatePicker = CreateDateTimePicker();
            endDatePicker.Value = DateTime.Now.AddMonths(6);
            endDatePicker.ValueChanged += EndDatePicker_ValueChanged;
            basicLayout.Controls.Add(endDatePicker, 3, 4);

            scrollPanel.Controls.Add(basicLayout);
            basicInfoTab.Controls.Add(scrollPanel);
        }

        private void SetupDetailsTab()
        {
            detailsTab = new TabPage
            {
                Text = "Chi tiết và yêu cầu",
                BackColor = Color.White,
                Padding = new Padding(25)
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
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            // Description
            var descPanel = CreateSectionPanel("Mô tả dự án:", out descriptionTextBox, true);
            detailsLayout.Controls.Add(descPanel, 0, 0);

            // Objectives
            var objPanel = CreateSectionPanel("Mục tiêu:", out objectivesTextBox, true);
            detailsLayout.Controls.Add(objPanel, 1, 0);

            // Requirements
            var reqPanel = CreateListSectionPanel("Yêu cầu chức năng:", out requirementsListBox,
                out addRequirementButton, out removeRequirementButton, out newRequirementTextBox);
            SetupRequirementsEvents();
            detailsLayout.Controls.Add(reqPanel, 0, 1);
            detailsLayout.SetRowSpan(reqPanel, 2);

            // Technologies
            var techPanel = CreateListSectionPanel("Công nghệ sử dụng:", out technologiesListBox,
                out addTechnologyButton, out removeTechnologyButton, out newTechnologyTextBox);
            SetupTechnologiesEvents();
            detailsLayout.Controls.Add(techPanel, 1, 1);
            detailsLayout.SetRowSpan(techPanel, 2);

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
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(resetButton);
            buttonPanel.Controls.Add(cancelButton);

            // Progress indicator
            var progressPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.Transparent
            };

            var progressLabel = new Label
            {
                Text = "💡 Tip: Các trường có dấu (*) là bắt buộc",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            progressPanel.Controls.Add(progressLabel);

            footerPanel.Controls.Add(buttonPanel);
            footerPanel.Controls.Add(progressPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }
        #endregion

        #region Control Creators
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

        private ComboBox CreateComboBox(string[] items)
        {
            var combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
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
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
        }

        private Panel CreateSectionPanel(string labelText, out TextBox textBox, bool isMultiline = false)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = isMultiline,
                ScrollBars = isMultiline ? ScrollBars.Vertical : ScrollBars.None,
                Margin = new Padding(0, 5, 0, 0)
            };

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);

            return panel;
        }

        private Panel CreateListSectionPanel(string labelText, out ListBox listBox,
            out Button addButton, out Button removeButton, out TextBox inputTextBox)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            inputTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10),
                Height = 25,
                Margin = new Padding(0, 5, 0, 5)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 35,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent
            };

            addButton = new Button
            {
                Text = "➕ Thêm",
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 0, 5, 0)
            };
            addButton.FlatAppearance.BorderSize = 0;

            removeButton = new Button
            {
                Text = "➖ Xóa",
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            removeButton.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(removeButton);

            listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One
            };

            panel.Controls.Add(listBox);
            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(inputTextBox);
            panel.Controls.Add(label);

            return panel;
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

        #region Event Handlers
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

        private void ProjectCodeTextBox_Leave(object sender, EventArgs e)
        {
            ValidateProjectCode();
        }

        private void ProjectNameTextBox_Leave(object sender, EventArgs e)
        {
            ValidateProjectName();
        }

        private void BudgetTextBox_Leave(object sender, EventArgs e)
        {
            ValidateBudget();
        }

        private void BudgetTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow digits, backspace, and decimal
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void StartDatePicker_ValueChanged(object sender, EventArgs e)
        {
            ValidateDates();
        }

        private void EndDatePicker_ValueChanged(object sender, EventArgs e)
        {
            ValidateDates();
        }

        private void SetupRequirementsEvents()
        {
            addRequirementButton.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(newRequirementTextBox.Text))
                {
                    requirementsListBox.Items.Add(newRequirementTextBox.Text.Trim());
                    newRequirementTextBox.Clear();
                }
            };

            removeRequirementButton.Click += (s, e) => {
                if (requirementsListBox.SelectedIndex >= 0)
                {
                    requirementsListBox.Items.RemoveAt(requirementsListBox.SelectedIndex);
                }
            };

            newRequirementTextBox.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    addRequirementButton.PerformClick();
                    e.Handled = true;
                }
            };
        }

        private void SetupTechnologiesEvents()
        {
            addTechnologyButton.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(newTechnologyTextBox.Text))
                {
                    technologiesListBox.Items.Add(newTechnologyTextBox.Text.Trim());
                    newTechnologyTextBox.Clear();
                }
            };

            removeTechnologyButton.Click += (s, e) => {
                if (technologiesListBox.SelectedIndex >= 0)
                {
                    technologiesListBox.Items.RemoveAt(technologiesListBox.SelectedIndex);
                }
            };

            newTechnologyTextBox.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    addTechnologyButton.PerformClick();
                    e.Handled = true;
                }
            };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveProject();
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
        #endregion

        #region Validation
        private bool ValidateProjectCode()
        {
            errorProvider.SetError(projectCodeTextBox, "");

            if (string.IsNullOrWhiteSpace(projectCodeTextBox.Text))
            {
                errorProvider.SetError(projectCodeTextBox, "Mã dự án không được để trống");
                return false;
            }

            if (projectCodeTextBox.Text.Length < 3)
            {
                errorProvider.SetError(projectCodeTextBox, "Mã dự án phải có ít nhất 3 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateProjectName()
        {
            errorProvider.SetError(projectNameTextBox, "");

            if (string.IsNullOrWhiteSpace(projectNameTextBox.Text))
            {
                errorProvider.SetError(projectNameTextBox, "Tên dự án không được để trống");
                return false;
            }

            if (projectNameTextBox.Text.Length < 5)
            {
                errorProvider.SetError(projectNameTextBox, "Tên dự án phải có ít nhất 5 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateBudget()
        {
            errorProvider.SetError(budgetTextBox, "");

            if (!string.IsNullOrWhiteSpace(budgetTextBox.Text))
            {
                if (!decimal.TryParse(budgetTextBox.Text, out decimal budget) || budget < 0)
                {
                    errorProvider.SetError(budgetTextBox, "Ngân sách phải là số không âm");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateDates()
        {
            errorProvider.SetError(endDatePicker, "");

            if (endDatePicker.Value <= startDatePicker.Value)
            {
                errorProvider.SetError(endDatePicker, "Ngày kết thúc phải sau ngày bắt đầu");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            isValid &= ValidateProjectCode();
            isValid &= ValidateProjectName();
            isValid &= ValidateBudget();
            isValid &= ValidateDates();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Switch to basic info tab
            }

            return isValid;
        }
        #endregion

        #region Data Operations
        private void SetDefaultValues()
        {
            projectCodeTextBox.Text = "PRJ" + DateTime.Now.ToString("yyyyMMddHHmm");
            statusComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndex = 0;
            managerComboBox.SelectedIndex = 0;
            priorityComboBox.SelectedIndex = 1;
            startDatePicker.Value = DateTime.Now.Date;
            endDatePicker.Value = DateTime.Now.AddMonths(6);

            // Add default requirements
            requirementsListBox.Items.AddRange(new string[]
            {
                "Phân tích yêu cầu nghiệp vụ",
                "Thiết kế kiến trúc hệ thống",
                "Phát triển ứng dụng",
                "Kiểm thử và đảm bảo chất lượng"
            });

            // Add default technologies
            technologiesListBox.Items.AddRange(new string[]
            {
                "C# .NET",
                "SQL Server",
                "HTML/CSS/JavaScript"
            });
        }

        private void ResetForm()
        {
            // Clear all text boxes
            projectCodeTextBox.Clear();
            projectNameTextBox.Clear();
            budgetTextBox.Clear();
            descriptionTextBox.Clear();
            objectivesTextBox.Clear();
            newRequirementTextBox.Clear();
            newTechnologyTextBox.Clear();

            // Reset combo boxes
            statusComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndex = 0;
            managerComboBox.SelectedIndex = 0;
            priorityComboBox.SelectedIndex = 1;

            // Reset dates
            startDatePicker.Value = DateTime.Now.Date;
            endDatePicker.Value = DateTime.Now.AddMonths(6);

            // Clear lists
            requirementsListBox.Items.Clear();
            technologiesListBox.Items.Clear();

            // Clear error provider
            errorProvider.Clear();

            // Set default values again
            SetDefaultValues();

            // Return to first tab
            tabControl.SelectedIndex = 0;
        }

        private void SaveProject()
        {
            try
            {
                project.ProjectCode = projectCodeTextBox.Text.Trim();
                project.ProjectName = projectNameTextBox.Text.Trim();
                project.Description = descriptionTextBox.Text.Trim();
                project.Status = statusComboBox.Text;
                project.StartDate = startDatePicker.Value;
                project.EndDate = endDatePicker.Value;
                project.CustomerID = customerComboBox.SelectedIndex + 1;
                project.ManagerID = managerComboBox.SelectedIndex + 1;
                project.CreatedAt = DateTime.Now;

                // Parse budget
                if (decimal.TryParse(budgetTextBox.Text, out decimal budget))
                    project.Budget = budget;

                this.DialogResult = DialogResult.OK;
                MessageBox.Show("Tạo dự án thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}