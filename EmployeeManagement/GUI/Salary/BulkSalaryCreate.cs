using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Salary
{
    public partial class BulkSalaryCreateForm : Form
    {
        #region Fields
        private SalaryBLL salaryBLL;
        private List<EmployeeDropdownItem> employees;
        private List<EmployeeDropdownItem> selectedEmployees;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private Label instructionLabel;

        // Content controls
        private GroupBox periodGroupBox;
        private ComboBox monthComboBox;
        private ComboBox yearComboBox;

        private GroupBox employeeGroupBox;
        private CheckedListBox employeeCheckedListBox;
        private Button selectAllButton;
        private Button selectNoneButton;
        private TextBox searchEmployeeTextBox;

        private GroupBox salaryGroupBox;
        private RadioButton useBaseSalaryRadio;
        private RadioButton useCustomSalaryRadio;
        private TextBox customAllowanceTextBox;
        private TextBox customBonusTextBox;
        private TextBox customDeductionTextBox;

        private GroupBox previewGroupBox;
        private DataGridView previewDataGridView;
        private Label summaryLabel;

        // Footer controls
        private Button previewButton;
        private Button createButton;
        private Button cancelButton;
        #endregion

        #region Constructor
        public BulkSalaryCreateForm()
        {
            InitializeComponent();
            salaryBLL = new SalaryBLL();
            selectedEmployees = new List<EmployeeDropdownItem>();
            SetupForm();
            LoadEmployees();
            SetDefaultValues();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Tạo Bảng Lương Hàng Loạt";
            this.Size = new Size(1200, 800);
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
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Header
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
                Text = "📋 TẠO BẢNG LƯƠNG HÀNG LOẠT",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 15),
                Size = new Size(600, 30),
                AutoEllipsis = true
            };

            instructionLabel = new Label
            {
                Text = "Tạo bảng lương cho nhiều nhân viên cùng lúc trong một tháng. Hệ thống sẽ tự động lấy lương cơ bản từ chức vụ của từng nhân viên.",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(20, 50),
                Size = new Size(800, 40)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(instructionLabel);

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

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            SetupPeriodAndEmployeePanel(out var leftPanel);
            SetupSalaryAndPreviewPanel(out var rightPanel);

            contentLayout.Controls.Add(leftPanel, 0, 0);
            contentLayout.Controls.Add(rightPanel, 1, 0);
            contentLayout.SetColumnSpan(SetupPreviewPanel(), 2);
            contentLayout.Controls.Add(SetupPreviewPanel(), 0, 1);

            contentPanel.Controls.Add(contentLayout);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupPeriodAndEmployeePanel(out Panel leftPanel)
        {
            leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 10, 0)
            };

            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Period Selection
            periodGroupBox = new GroupBox
            {
                Text = "📅 Chọn thời gian",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Padding = new Padding(15)
            };

            var periodLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };

            var monthPanel = CreateComboPanel("Tháng:", out monthComboBox);
            for (int i = 1; i <= 12; i++)
                monthComboBox.Items.Add($"Tháng {i}");

            var yearPanel = CreateComboPanel("Năm:", out yearComboBox);
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 2; year--)
                yearComboBox.Items.Add(year.ToString());

            periodLayout.Controls.Add(monthPanel, 0, 0);
            periodLayout.Controls.Add(yearPanel, 1, 0);
            periodGroupBox.Controls.Add(periodLayout);

            // Employee Selection
            employeeGroupBox = new GroupBox
            {
                Text = "👥 Chọn nhân viên",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Padding = new Padding(15)
            };

            var employeeLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };

            employeeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Search
            employeeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // Buttons
            employeeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // List
            employeeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Count

            // Search box
            searchEmployeeTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "🔍 Tìm kiếm nhân viên..."
            };
            searchEmployeeTextBox.TextChanged += SearchEmployeeTextBox_TextChanged;

            // Select buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };

            selectAllButton = new Button
            {
                Text = "✅ Chọn tất cả",
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                FlatAppearance = { BorderSize = 0 }
            };
            selectAllButton.Click += SelectAllButton_Click;

            selectNoneButton = new Button
            {
                Text = "❌ Bỏ chọn",
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            selectNoneButton.Click += SelectNoneButton_Click;

            buttonPanel.Controls.Add(selectAllButton);
            buttonPanel.Controls.Add(selectNoneButton);

            // Employee list
            employeeCheckedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                CheckOnClick = true
            };
            employeeCheckedListBox.ItemCheck += EmployeeCheckedListBox_ItemCheck;

            // Count label
            var countLabel = new Label
            {
                Text = "Đã chọn: 0 nhân viên",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Tag = "countLabel"
            };

            employeeLayout.Controls.Add(searchEmployeeTextBox, 0, 0);
            employeeLayout.Controls.Add(buttonPanel, 0, 1);
            employeeLayout.Controls.Add(employeeCheckedListBox, 0, 2);
            employeeLayout.Controls.Add(countLabel, 0, 3);

            employeeGroupBox.Controls.Add(employeeLayout);

            leftLayout.Controls.Add(periodGroupBox, 0, 0);
            leftLayout.Controls.Add(employeeGroupBox, 0, 1);

            leftPanel.Controls.Add(leftLayout);
        }

        private void SetupSalaryAndPreviewPanel(out Panel rightPanel)
        {
            rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };

            salaryGroupBox = new GroupBox
            {
                Text = "💰 Cấu hình lương",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Padding = new Padding(15)
            };

            var salaryLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(10)
            };

            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            salaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Base salary option
            useBaseSalaryRadio = new RadioButton
            {
                Text = "Sử dụng lương cơ bản từ chức vụ",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            useBaseSalaryRadio.CheckedChanged += SalaryOption_CheckedChanged;

            // Custom salary option
            useCustomSalaryRadio = new RadioButton
            {
                Text = "Tùy chỉnh các khoản phụ",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            useCustomSalaryRadio.CheckedChanged += SalaryOption_CheckedChanged;

            // Custom amounts
            var allowancePanel = CreateInputPanel("Phụ cấp chung:", out customAllowanceTextBox);
            customAllowanceTextBox.Text = "0";
            customAllowanceTextBox.KeyPress += NumbersOnly_KeyPress;

            var bonusPanel = CreateInputPanel("Thưởng chung:", out customBonusTextBox);
            customBonusTextBox.Text = "0";
            customBonusTextBox.KeyPress += NumbersOnly_KeyPress;

            var deductionPanel = CreateInputPanel("Khấu trừ chung:", out customDeductionTextBox);
            customDeductionTextBox.Text = "0";
            customDeductionTextBox.KeyPress += NumbersOnly_KeyPress;

            // Info panel
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var infoLabel = new Label
            {
                Text = "💡 Lương cơ bản sẽ được lấy từ chức vụ của từng nhân viên. " +
                       "Các khoản phụ cấp, thưởng, khấu trừ sẽ áp dụng cho tất cả nhân viên được chọn.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(33, 150, 243),
                TextAlign = ContentAlignment.MiddleLeft
            };

            infoPanel.Controls.Add(infoLabel);

            salaryLayout.Controls.Add(useBaseSalaryRadio, 0, 0);
            salaryLayout.Controls.Add(useCustomSalaryRadio, 0, 1);
            salaryLayout.Controls.Add(allowancePanel, 0, 2);
            salaryLayout.Controls.Add(bonusPanel, 0, 3);
            salaryLayout.Controls.Add(deductionPanel, 0, 4);
            salaryLayout.Controls.Add(infoPanel, 0, 5);

            salaryGroupBox.Controls.Add(salaryLayout);
            rightPanel.Controls.Add(salaryGroupBox);

            // Enable/disable custom fields
            UpdateCustomFieldsEnabled();
        }

        private Panel SetupPreviewPanel()
        {
            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            previewGroupBox = new GroupBox
            {
                Text = "👁️ Xem trước kết quả",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Padding = new Padding(15)
            };

            var previewLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };

            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            previewDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 30 }
            };

            SetupPreviewGridColumns();
            SetupPreviewGridStyles();

            summaryLabel = new Label
            {
                Text = "Chưa có dữ liệu xem trước",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            previewLayout.Controls.Add(previewDataGridView, 0, 0);
            previewLayout.Controls.Add(summaryLabel, 0, 1);

            previewGroupBox.Controls.Add(previewLayout);
            previewPanel.Controls.Add(previewGroupBox);

            return previewPanel;
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

            previewButton = new Button
            {
                Text = "👁️ Xem trước",
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            previewButton.Click += PreviewButton_Click;

            createButton = new Button
            {
                Text = "💾 Tạo bảng lương",
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 },
                Enabled = false
            };
            createButton.Click += CreateButton_Click;

            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Size = new Size(100, 45),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(previewButton);
            buttonPanel.Controls.Add(createButton);
            buttonPanel.Controls.Add(cancelButton);

            footerPanel.Controls.Add(buttonPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }
        #endregion

        #region Helper Methods
        private Panel CreateComboPanel(string labelText, out ComboBox comboBox)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            comboBox = new ComboBox
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30
            };

            panel.Controls.Add(comboBox);
            panel.Controls.Add(label);
            return panel;
        }

        private Panel CreateInputPanel(string labelText, out TextBox textBox)
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
                Height = 20,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                Height = 25
            };

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);
            return panel;
        }

        private void SetupPreviewGridColumns()
        {
            previewDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "EmployeeCode", HeaderText = "Mã NV", Width = 80 },
                new { Name = "EmployeeName", HeaderText = "Tên nhân viên", Width = 150 },
                new { Name = "DepartmentName", HeaderText = "Phòng ban", Width = 120 },
                new { Name = "BaseSalary", HeaderText = "Lương CB", Width = 100 },
                new { Name = "Allowance", HeaderText = "Phụ cấp", Width = 80 },
                new { Name = "Bonus", HeaderText = "Thưởng", Width = 80 },
                new { Name = "Deduction", HeaderText = "Khấu trừ", Width = 80 },
                new { Name = "NetSalary", HeaderText = "Thực nhận", Width = 100 }
            };

            foreach (var col in columns)
            {
                previewDataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width
                });
            }
        }

        private void SetupPreviewGridStyles()
        {
            previewDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                Font = new Font("Segoe UI", 9)
            };

            previewDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
        }

        private void LoadEmployees()
        {
            try
            {
                employees = salaryBLL.GetEmployeesForDropdown();
                RefreshEmployeeList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshEmployeeList()
        {
            employeeCheckedListBox.Items.Clear();
            string searchText = searchEmployeeTextBox.Text.ToLower();

            var filteredEmployees = employees.Where(e =>
                string.IsNullOrEmpty(searchText) ||
                e.EmployeeCode.ToLower().Contains(searchText) ||
                e.FullName.ToLower().Contains(searchText) ||
                e.DepartmentName.ToLower().Contains(searchText)
            ).ToList();

            foreach (var employee in filteredEmployees)
            {
                employeeCheckedListBox.Items.Add(employee);
            }

            employeeCheckedListBox.DisplayMember = "DisplayText";
        }

        private void SetDefaultValues()
        {
            monthComboBox.SelectedIndex = DateTime.Now.Month - 1;
            yearComboBox.SelectedIndex = 0;
        }

        private void UpdateCustomFieldsEnabled()
        {
            bool enabled = useCustomSalaryRadio.Checked;
            customAllowanceTextBox.Enabled = enabled;
            customBonusTextBox.Enabled = enabled;
            customDeductionTextBox.Enabled = enabled;
        }

        private void UpdateSelectedCount()
        {
            var countLabel = employeeGroupBox.Controls.Find("countLabel", true).FirstOrDefault() as Label;
            if (countLabel != null)
            {
                int selectedCount = employeeCheckedListBox.CheckedItems.Count;
                countLabel.Text = $"Đã chọn: {selectedCount} nhân viên";
            }
        }
        #endregion

        #region Event Handlers
        private void SearchEmployeeTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshEmployeeList();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < employeeCheckedListBox.Items.Count; i++)
            {
                employeeCheckedListBox.SetItemChecked(i, true);
            }
        }

        private void SelectNoneButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < employeeCheckedListBox.Items.Count; i++)
            {
                employeeCheckedListBox.SetItemChecked(i, false);
            }
        }

        private void EmployeeCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Update count after the check state changes
            BeginInvoke(new Action(UpdateSelectedCount));
        }

        private void SalaryOption_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCustomFieldsEnabled();
        }

        private void NumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                GeneratePreview();
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput() && ConfirmCreation())
            {
                CreateSalaries();
            }
        }

        private bool ValidateInput()
        {
            if (monthComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn tháng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (yearComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn năm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (employeeCheckedListBox.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một nhân viên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GeneratePreview()
        {
            try
            {
                int month = monthComboBox.SelectedIndex + 1;
                int year = int.Parse(yearComboBox.Text);
                decimal allowance = decimal.Parse(customAllowanceTextBox.Text);
                decimal bonus = decimal.Parse(customBonusTextBox.Text);
                decimal deduction = decimal.Parse(customDeductionTextBox.Text);

                selectedEmployees.Clear();
                var previewData = new List<object>();
                decimal totalSalary = 0;

                foreach (EmployeeDropdownItem employee in employeeCheckedListBox.CheckedItems)
                {
                    selectedEmployees.Add(employee);

                    decimal baseSalary = employee.BaseSalary;
                    decimal netSalary = baseSalary + allowance + bonus - deduction;
                    totalSalary += netSalary;

                    previewData.Add(new
                    {
                        EmployeeCode = employee.EmployeeCode,
                        EmployeeName = employee.FullName,
                        DepartmentName = employee.DepartmentName,
                        BaseSalary = $"{baseSalary:#,##0}",
                        Allowance = $"{allowance:#,##0}",
                        Bonus = $"{bonus:#,##0}",
                        Deduction = $"{deduction:#,##0}",
                        NetSalary = $"{netSalary:#,##0}"
                    });
                }

                previewDataGridView.DataSource = previewData;
                summaryLabel.Text = $"Tổng cộng: {selectedEmployees.Count} nhân viên | " +
                                   $"Tổng tiền lương: {totalSalary:#,##0} VNĐ | " +
                                   $"Thời gian: Tháng {month}/{year}";

                createButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo xem trước: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ConfirmCreation()
        {
            int month = monthComboBox.SelectedIndex + 1;
            int year = int.Parse(yearComboBox.Text);

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn tạo bảng lương cho {selectedEmployees.Count} nhân viên trong tháng {month}/{year}?\n\n" +
                "Lưu ý: Nếu nhân viên đã có bảng lương trong tháng này, hệ thống sẽ bỏ qua.\n\n" +
                "Tiếp tục?",
                "Xác nhận tạo bảng lương",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            return result == DialogResult.Yes;
        }

        private void CreateSalaries()
        {
            try
            {
                int month = monthComboBox.SelectedIndex + 1;
                int year = int.Parse(yearComboBox.Text);
                decimal allowance = decimal.Parse(customAllowanceTextBox.Text);
                decimal bonus = decimal.Parse(customBonusTextBox.Text);
                decimal deduction = decimal.Parse(customDeductionTextBox.Text);

                int successCount = 0;
                int skipCount = 0;
                var errors = new List<string>();

                foreach (var employee in selectedEmployees)
                {
                    try
                    {
                        // Check if salary already exists
                        var existingSalaries = salaryBLL.GetSalariesByEmployee(employee.EmployeeID);
                        if (existingSalaries.Any(s => s.Month == month && s.Year == year))
                        {
                            skipCount++;
                            continue;
                        }

                        var salary = new Models.Salary
                        {
                            EmployeeID = employee.EmployeeID,
                            Month = month,
                            Year = year,
                            BaseSalary = employee.BaseSalary,
                            Allowance = allowance,
                            Bonus = bonus,
                            Deduction = deduction,
                            PaymentStatus = "Chưa thanh toán",
                            Notes = $"Tạo tự động hàng loạt cho tháng {month}/{year}"
                        };

                        salary.CalculateNetSalary();

                        if (salaryBLL.AddSalary(salary) > 0)
                        {
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Không thể tạo bảng lương cho {employee.EmployeeCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi khi tạo bảng lương cho {employee.EmployeeCode}: {ex.Message}");
                    }
                }

                // Show results
                string message = $"Hoàn thành!\n\n" +
                               $"✅ Tạo thành công: {successCount} bảng lương\n" +
                               $"⏭️ Đã bỏ qua (đã tồn tại): {skipCount} bảng lương";

                if (errors.Count > 0)
                {
                    message += $"\n❌ Lỗi: {errors.Count} bảng lương\n\n";
                    message += string.Join("\n", errors.Take(3));
                    if (errors.Count > 3)
                        message += $"\n... và {errors.Count - 3} lỗi khác";
                }

                MessageBox.Show(message, "Kết quả tạo bảng lương",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (successCount > 0)
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo bảng lương hàng loạt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}