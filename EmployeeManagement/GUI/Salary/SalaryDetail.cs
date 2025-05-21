using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Salary
{
    public partial class SalaryDetailForm : Form
    {
        #region Fields
        private SalaryBLL salaryBLL;
        private Models.Salary salary;
        private bool isReadOnly;
        private bool isEditMode;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox salaryIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage paymentTab;

        // Basic info controls
        private ComboBox employeeComboBox;
        private ComboBox monthComboBox;
        private ComboBox yearComboBox;
        private TextBox baseSalaryTextBox;
        private TextBox allowanceTextBox;
        private TextBox bonusTextBox;
        private TextBox deductionTextBox;
        private TextBox netSalaryTextBox;
        private TextBox notesTextBox;

        // Payment info controls
        private ComboBox paymentStatusComboBox;
        private DateTimePicker paymentDatePicker;
        private CheckBox hasPaymentDateCheckBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;
        private Button calculateButton;

        // Validation
        private ErrorProvider errorProvider;
        #endregion

        #region Constructors
        public SalaryDetailForm()
        {
            InitializeComponent();
            salaryBLL = new SalaryBLL();
            salary = new Models.Salary();
            isEditMode = false;
            isReadOnly = false;
            SetupForm();
            LoadComboBoxData();
            SetDefaultValues();
        }

        public SalaryDetailForm(Models.Salary existingSalary, bool readOnly = false)
        {
            InitializeComponent();
            salaryBLL = new SalaryBLL();
            salary = existingSalary ?? throw new ArgumentNullException(nameof(existingSalary));
            isEditMode = true;
            isReadOnly = readOnly;
            SetupForm();
            LoadComboBoxData();
            LoadSalaryData();
        }

        public Models.Salary UpdatedSalary => salary;
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = isReadOnly ? "Xem chi tiết bảng lương" :
                       isEditMode ? "Chỉnh sửa bảng lương" : "Thêm bảng lương mới";
            this.Size = new Size(900, 700);
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

            // Salary icon
            salaryIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(76, 175, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateSalaryIcon();

            // Title label
            string titleText = isReadOnly ? "👁️ CHI TIẾT BẢNG LƯƠNG" :
                              isEditMode ? "✏️ CHỈNH SỬA BẢNG LƯƠNG" : "➕ THÊM BẢNG LƯƠNG MỚI";

            titleLabel = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            string subtitleText = isReadOnly ? "Xem thông tin chi tiết bảng lương" :
                                 isEditMode ? "Cập nhật thông tin bảng lương" : "Nhập thông tin để tạo bảng lương mới";

            var subtitleLabel = new Label
            {
                Text = subtitleText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(salaryIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateSalaryIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(76, 175, 80));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string iconText = isReadOnly ? "👁️" : isEditMode ? "✏️" : "💰";
                    var size = g.MeasureString(iconText, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(iconText, font, brush, x, y);
                }
            }
            salaryIcon.Image = bmp;
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

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
            SetupPaymentTab();

            tabControl.TabPages.Add(basicInfoTab);
            tabControl.TabPages.Add(paymentTab);

            contentPanel.Controls.Add(tabControl);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupBasicInfoTab()
        {
            basicInfoTab = new TabPage
            {
                Text = "Thông tin lương",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Employee
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Month/Year
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Base salary/Allowance
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Bonus/Deduction
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Net salary/Calculate
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Notes

            // Employee (Required)
            var employeePanel = CreateComboPanel("Nhân viên *:", out employeeComboBox);
            employeeComboBox.SelectedIndexChanged += EmployeeComboBox_SelectedIndexChanged;
            employeeComboBox.Enabled = !isEditMode && !isReadOnly;
            basicLayout.Controls.Add(employeePanel, 0, 0);
            basicLayout.SetColumnSpan(employeePanel, 2);

            // Month
            var monthPanel = CreateComboPanel("Tháng *:", out monthComboBox);
            for (int i = 1; i <= 12; i++)
                monthComboBox.Items.Add($"Tháng {i}");
            monthComboBox.Enabled = !isReadOnly;
            basicLayout.Controls.Add(monthPanel, 0, 1);

            // Year
            var yearPanel = CreateComboPanel("Năm *:", out yearComboBox);
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 2; year--)
                yearComboBox.Items.Add(year.ToString());
            yearComboBox.Enabled = !isReadOnly;
            basicLayout.Controls.Add(yearPanel, 1, 1);

            // Base Salary
            var baseSalaryPanel = CreateInputPanel("Lương cơ bản *:", out baseSalaryTextBox);
            baseSalaryTextBox.KeyPress += NumbersOnly_KeyPress;
            baseSalaryTextBox.Leave += SalaryTextBox_Leave;
            basicLayout.Controls.Add(baseSalaryPanel, 0, 2);

            // Allowance
            var allowancePanel = CreateInputPanel("Phụ cấp:", out allowanceTextBox);
            allowanceTextBox.KeyPress += NumbersOnly_KeyPress;
            allowanceTextBox.Leave += SalaryTextBox_Leave;
            basicLayout.Controls.Add(allowancePanel, 1, 2);

            // Bonus
            var bonusPanel = CreateInputPanel("Thưởng:", out bonusTextBox);
            bonusTextBox.KeyPress += NumbersOnly_KeyPress;
            bonusTextBox.Leave += SalaryTextBox_Leave;
            basicLayout.Controls.Add(bonusPanel, 0, 3);

            // Deduction
            var deductionPanel = CreateInputPanel("Khấu trừ:", out deductionTextBox);
            deductionTextBox.KeyPress += NumbersOnly_KeyPress;
            deductionTextBox.Leave += SalaryTextBox_Leave;
            basicLayout.Controls.Add(deductionPanel, 1, 3);

            // Net Salary
            var netSalaryPanel = CreateInputPanel("Lương thực nhận:", out netSalaryTextBox);
            netSalaryTextBox.ReadOnly = true;
            netSalaryTextBox.BackColor = Color.FromArgb(248, 249, 250);
            basicLayout.Controls.Add(netSalaryPanel, 0, 4);

            // Calculate Button
            var calculatePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                Padding = new Padding(10, 25, 10, 10)
            };

            calculateButton = new Button
            {
                Text = "🧮 Tính lương thực nhận",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
                Enabled = !isReadOnly
            };
            calculateButton.Click += CalculateButton_Click;

            calculatePanel.Controls.Add(calculateButton);
            basicLayout.Controls.Add(calculatePanel, 1, 4);

            // Notes
            var notesPanel = CreateInputPanel("Ghi chú:", out notesTextBox, true);
            basicLayout.Controls.Add(notesPanel, 0, 5);
            basicLayout.SetColumnSpan(notesPanel, 2);

            basicInfoTab.Controls.Add(basicLayout);
        }

        private void SetupPaymentTab()
        {
            paymentTab = new TabPage
            {
                Text = "Thông tin thanh toán",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var paymentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Payment Status
            var statusPanel = CreateComboPanel("Trạng thái thanh toán:", out paymentStatusComboBox);
            paymentStatusComboBox.Items.AddRange(SalaryConstants.PaymentStatuses);
            paymentStatusComboBox.SelectedIndexChanged += PaymentStatusComboBox_SelectedIndexChanged;
            paymentStatusComboBox.Enabled = !isReadOnly;
            paymentLayout.Controls.Add(statusPanel, 0, 0);
            paymentLayout.SetColumnSpan(statusPanel, 2);

            // Payment Date Checkbox
            hasPaymentDateCheckBox = new CheckBox
            {
                Text = "Có ngày thanh toán",
                Location = new Point(10, 10),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Enabled = !isReadOnly
            };
            hasPaymentDateCheckBox.CheckedChanged += HasPaymentDateCheckBox_CheckedChanged;

            // Payment Date
            var paymentDatePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var paymentDateLabel = new Label
            {
                Text = "Ngày thanh toán:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            paymentDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            paymentDatePanel.Controls.Add(paymentDatePicker);
            paymentDatePanel.Controls.Add(paymentDateLabel);
            paymentDatePanel.Controls.Add(hasPaymentDateCheckBox);

            paymentLayout.Controls.Add(paymentDatePanel, 0, 1);
            paymentLayout.SetColumnSpan(paymentDatePanel, 2);

            paymentTab.Controls.Add(paymentLayout);
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

            if (!isReadOnly)
            {
                string saveText = isEditMode ? "💾 Cập nhật" : "💾 Tạo bảng lương";
                saveButton = CreateFooterButton(saveText, Color.FromArgb(76, 175, 80));
                saveButton.Click += SaveButton_Click;

                resetButton = CreateFooterButton("🔄 Đặt lại", Color.FromArgb(255, 152, 0));
                resetButton.Click += ResetButton_Click;

                buttonPanel.Controls.Add(saveButton);
                buttonPanel.Controls.Add(resetButton);
            }

            cancelButton = CreateFooterButton(isReadOnly ? "❌ Đóng" : "❌ Hủy", Color.FromArgb(158, 158, 158));
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(cancelButton);

            // Progress indicator
            var progressPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.Transparent
            };

            string tipText = isReadOnly ? "💡 Chế độ xem - Không thể chỉnh sửa" : "💡 Tip: Các trường có dấu (*) là bắt buộc";
            var progressLabel = new Label
            {
                Text = tipText,
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
        private Panel CreateInputPanel(string labelText, out TextBox textBox, bool isMultiline = false)
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
                ForeColor = labelText.Contains("*") ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = isMultiline,
                ScrollBars = isMultiline ? ScrollBars.Vertical : ScrollBars.None,
                Margin = new Padding(0, 5, 0, 0),
                ReadOnly = isReadOnly
            };

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);

            return panel;
        }

        private Panel CreateComboPanel(string labelText, out ComboBox comboBox)
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
                ForeColor = labelText.Contains("*") ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64)
            };

            comboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 0, 0),
                Enabled = !isReadOnly
            };

            panel.Controls.Add(comboBox);
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

        #region Data Operations
        private void LoadComboBoxData()
        {
            try
            {
                // Load employees
                employeeComboBox.Items.Clear();
                var employees = salaryBLL.GetEmployeesForDropdown();
                foreach (var employee in employees)
                {
                    employeeComboBox.Items.Add(employee);
                }
                employeeComboBox.DisplayMember = "DisplayText";
                employeeComboBox.ValueMember = "EmployeeID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultValues()
        {
            if (!isEditMode)
            {
                monthComboBox.SelectedIndex = DateTime.Now.Month - 1;
                yearComboBox.SelectedIndex = 0;
                paymentStatusComboBox.SelectedIndex = 0; // Chưa thanh toán
                baseSalaryTextBox.Text = "0";
                allowanceTextBox.Text = "0";
                bonusTextBox.Text = "0";
                deductionTextBox.Text = "0";
                netSalaryTextBox.Text = "0";
            }
        }

        private void LoadSalaryData()
        {
            if (salary == null) return;

            try
            {
                // Set employee
                for (int i = 0; i < employeeComboBox.Items.Count; i++)
                {
                    if (employeeComboBox.Items[i] is EmployeeDropdownItem item &&
                        item.EmployeeID == salary.EmployeeID)
                    {
                        employeeComboBox.SelectedIndex = i;
                        break;
                    }
                }

                monthComboBox.SelectedIndex = salary.Month - 1;
                yearComboBox.Text = salary.Year.ToString();
                baseSalaryTextBox.Text = salary.BaseSalary.ToString("0");
                allowanceTextBox.Text = salary.Allowance.ToString("0");
                bonusTextBox.Text = salary.Bonus.ToString("0");
                deductionTextBox.Text = salary.Deduction.ToString("0");
                netSalaryTextBox.Text = salary.NetSalary.ToString("#,##0");
                notesTextBox.Text = salary.Notes;

                // Payment info
                paymentStatusComboBox.Text = salary.PaymentStatus;
                if (salary.PaymentDate.HasValue)
                {
                    hasPaymentDateCheckBox.Checked = true;
                    paymentDatePicker.Value = salary.PaymentDate.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu bảng lương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = (TabControl)sender;
            var tabPage = tabControl.TabPages[e.Index];
            var tabRect = tabControl.GetTabRect(e.Index);

            var brush = e.State == DrawItemState.Selected
                ? new SolidBrush(Color.FromArgb(76, 175, 80))
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

        private void EmployeeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (employeeComboBox.SelectedItem is EmployeeDropdownItem employee)
            {
                baseSalaryTextBox.Text = employee.BaseSalary.ToString("0");
                CalculateNetSalary();
            }
        }

        private void PaymentStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto set payment date if status is paid
            if (paymentStatusComboBox.Text == "Đã thanh toán" && !hasPaymentDateCheckBox.Checked)
            {
                hasPaymentDateCheckBox.Checked = true;
                paymentDatePicker.Value = DateTime.Now;
            }
        }

        private void HasPaymentDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            paymentDatePicker.Enabled = hasPaymentDateCheckBox.Checked;
            if (hasPaymentDateCheckBox.Checked && paymentDatePicker.Value < new DateTime(2000, 1, 1))
            {
                paymentDatePicker.Value = DateTime.Now;
            }
        }

        private void NumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SalaryTextBox_Leave(object sender, EventArgs e)
        {
            CalculateNetSalary();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            CalculateNetSalary();
        }

        private void CalculateNetSalary()
        {
            try
            {
                decimal baseSalary = decimal.TryParse(baseSalaryTextBox.Text, out decimal bs) ? bs : 0;
                decimal allowance = decimal.TryParse(allowanceTextBox.Text, out decimal al) ? al : 0;
                decimal bonus = decimal.TryParse(bonusTextBox.Text, out decimal bo) ? bo : 0;
                decimal deduction = decimal.TryParse(deductionTextBox.Text, out decimal de) ? de : 0;

                decimal netSalary = baseSalary + allowance + bonus - deduction;
                netSalaryTextBox.Text = netSalary.ToString("#,##0");
            }
            catch (Exception)
            {
                netSalaryTextBox.Text = "0";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveSalary();

                    if (isEditMode)
                    {
                        if (salaryBLL.UpdateSalary(salary))
                        {
                            MessageBox.Show("Cập nhật bảng lương thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        int newSalaryId = salaryBLL.AddSalary(salary);
                        salary.SalaryID = newSalaryId;
                        MessageBox.Show("Thêm bảng lương mới thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu bảng lương: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả thông tin?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (isEditMode)
                    LoadSalaryData();
                else
                    ResetForm();
            }
        }
        #endregion

        #region Validation
        private bool ValidateForm()
        {
            errorProvider.Clear();
            bool isValid = true;

            // Validate employee
            if (employeeComboBox.SelectedIndex == -1)
            {
                errorProvider.SetError(employeeComboBox, "Vui lòng chọn nhân viên");
                isValid = false;
            }

            // Validate month
            if (monthComboBox.SelectedIndex == -1)
            {
                errorProvider.SetError(monthComboBox, "Vui lòng chọn tháng");
                isValid = false;
            }

            // Validate year
            if (yearComboBox.SelectedIndex == -1)
            {
                errorProvider.SetError(yearComboBox, "Vui lòng chọn năm");
                isValid = false;
            }

            // Validate base salary
            if (!decimal.TryParse(baseSalaryTextBox.Text, out decimal baseSalary) || baseSalary < 0)
            {
                errorProvider.SetError(baseSalaryTextBox, "Lương cơ bản phải là số không âm");
                isValid = false;
            }

            // Validate allowance
            if (!decimal.TryParse(allowanceTextBox.Text, out decimal allowance) || allowance < 0)
            {
                errorProvider.SetError(allowanceTextBox, "Phụ cấp phải là số không âm");
                isValid = false;
            }

            // Validate bonus
            if (!decimal.TryParse(bonusTextBox.Text, out decimal bonus) || bonus < 0)
            {
                errorProvider.SetError(bonusTextBox, "Thưởng phải là số không âm");
                isValid = false;
            }

            // Validate deduction
            if (!decimal.TryParse(deductionTextBox.Text, out decimal deduction) || deduction < 0)
            {
                errorProvider.SetError(deductionTextBox, "Khấu trừ phải là số không âm");
                isValid = false;
            }

            // Business validation - check if salary already exists
            if (!isEditMode && employeeComboBox.SelectedItem is EmployeeDropdownItem employee)
            {
                try
                {
                    int month = monthComboBox.SelectedIndex + 1;
                    int year = int.Parse(yearComboBox.Text);

                    var existingSalaries = salaryBLL.GetSalariesByEmployee(employee.EmployeeID);
                    if (existingSalaries.Any(s => s.Month == month && s.Year == year))
                    {
                        errorProvider.SetError(monthComboBox, $"Đã tồn tại bảng lương cho nhân viên này trong tháng {month}/{year}");
                        isValid = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi kiểm tra trùng lặp: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }
            }

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Switch to basic info tab
            }

            return isValid;
        }

        private void SaveSalary()
        {
            try
            {
                if (employeeComboBox.SelectedItem is EmployeeDropdownItem employee)
                {
                    salary.EmployeeID = employee.EmployeeID;
                }

                salary.Month = monthComboBox.SelectedIndex + 1;
                salary.Year = int.Parse(yearComboBox.Text);
                salary.BaseSalary = decimal.Parse(baseSalaryTextBox.Text);
                salary.Allowance = decimal.Parse(allowanceTextBox.Text);
                salary.Bonus = decimal.Parse(bonusTextBox.Text);
                salary.Deduction = decimal.Parse(deductionTextBox.Text);
                salary.Notes = notesTextBox.Text.Trim();
                salary.PaymentStatus = paymentStatusComboBox.Text;

                if (hasPaymentDateCheckBox.Checked)
                    salary.PaymentDate = paymentDatePicker.Value;
                else
                    salary.PaymentDate = null;

                // Calculate net salary
                salary.CalculateNetSalary();

                if (!isEditMode)
                {
                    salary.CreatedAt = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin bảng lương: {ex.Message}", ex);
            }
        }

        private void ResetForm()
        {
            // Clear all controls
            employeeComboBox.SelectedIndex = -1;
            monthComboBox.SelectedIndex = -1;
            yearComboBox.SelectedIndex = -1;
            baseSalaryTextBox.Clear();
            allowanceTextBox.Clear();
            bonusTextBox.Clear();
            deductionTextBox.Clear();
            netSalaryTextBox.Clear();
            notesTextBox.Clear();
            paymentStatusComboBox.SelectedIndex = -1;
            hasPaymentDateCheckBox.Checked = false;

            // Clear error provider
            errorProvider.Clear();

            // Set default values again
            SetDefaultValues();

            // Return to first tab
            tabControl.SelectedIndex = 0;
        }
        #endregion
    }
}