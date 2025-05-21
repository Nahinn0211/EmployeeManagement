using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using MaterialSkin;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Finance
{
    public partial class FinanceDetailForm : MaterialForm
    {
        #region Fields
        private readonly FinanceBLL financeBLL;
        private Models.Finance currentFinance;
        private int financeId;
        private bool isEditMode;
        private bool isViewOnlyMode;
        private int currentUserId;
        private Models.Project preSelectedProject; // Thêm field này

        // Controls
        private MaterialTextBox txtTransactionCode;
        private MaterialComboBox cmbTransactionType;
        private MaterialComboBox cmbCategory;
        private MaterialTextBox txtAmount;
        private DateTimePicker dtpTransactionDate;
        private MaterialComboBox cmbProject;
        private MaterialComboBox cmbCustomer;
        private MaterialComboBox cmbEmployee;
        private MaterialComboBox cmbPaymentMethod;
        private MaterialTextBox txtReferenceNo;
        private MaterialTextBox txtDescription;
        private MaterialComboBox cmbStatus;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;
        private MaterialButton btnGenerateCode;
        private Label lblValidationMessage;
        #endregion

        #region Constructors
        // Constructor 1: Thêm mới giao dịch
        public FinanceDetailForm(int userId)
        {
            InitializeComponent();
            financeBLL = new FinanceBLL();
            currentUserId = userId;
            isEditMode = false;
            isViewOnlyMode = false;
            SetupMaterialDesign();
            SetupControls();
            LoadComboBoxData();
            SetDefaultValues();
        }

        // Constructor 2: Thêm mới với dự án được chọn trước
        public FinanceDetailForm(int userId, Models.Project preSelectedProject)
        {
            InitializeComponent();
            financeBLL = new FinanceBLL();
            currentUserId = userId;
            isEditMode = false;
            isViewOnlyMode = false;
            this.preSelectedProject = preSelectedProject;
            SetupMaterialDesign();
            SetupControls();
            LoadComboBoxData();
            SetDefaultValues();
            SetPreSelectedProject();
        }

        // Constructor 3: Chỉnh sửa hoặc xem giao dịch
        public FinanceDetailForm(int financeId, int userId, bool viewOnly)
        {
            InitializeComponent();
            this.financeId = financeId;
            financeBLL = new FinanceBLL();
            currentUserId = userId;
            isEditMode = true;
            isViewOnlyMode = viewOnly;
            SetupMaterialDesign();
            SetupControls();
            LoadComboBoxData();
            LoadFinanceData();
            if (isViewOnlyMode)
            {
                SetViewOnlyMode();
            }
        }

        // Constructor 4: Cho ProjectFinanceForm - chỉ xem giao dịch
        public FinanceDetailForm(Models.Finance finance, bool viewOnly)
        {
            InitializeComponent();
            currentFinance = finance;
            this.financeId = finance.FinanceID;
            financeBLL = new FinanceBLL();
            currentUserId = 1; // Default user ID hoặc get từ session
            isEditMode = true;
            isViewOnlyMode = viewOnly;
            SetupMaterialDesign();
            SetupControls();
            LoadComboBoxData();
            LoadFinanceFromObject();
            if (isViewOnlyMode)
            {
                SetViewOnlyMode();
            }
        }

        // Constructor 5: Cho ProjectFinanceForm - chỉnh sửa giao dịch
        public FinanceDetailForm(Models.Finance finance)
        {
            InitializeComponent();
            currentFinance = finance;
            this.financeId = finance.FinanceID;
            financeBLL = new FinanceBLL();
            currentUserId = 1; // Default user ID hoặc get từ session
            isEditMode = true;
            isViewOnlyMode = false;
            SetupMaterialDesign();
            SetupControls();
            LoadComboBoxData();
            LoadFinanceFromObject();
        }
        #endregion

        #region Pre-Selected Project Methods
        private void SetPreSelectedProject()
        {
            if (preSelectedProject != null && cmbProject.Items.Count > 0)
            {
                for (int i = 0; i < cmbProject.Items.Count; i++)
                {
                    if (cmbProject.Items[i] is ComboBoxItem item &&
                        item.Value?.Equals(preSelectedProject.ProjectID) == true)
                    {
                        cmbProject.SelectedIndex = i;
                        cmbProject.Enabled = false; // Lock project selection
                        break;
                    }
                }
            }
        }

        // Public method để set project từ bên ngoài
        public void SetPreSelectedProject(Models.Project project)
        {
            preSelectedProject = project;
            if (cmbProject != null && cmbProject.Items.Count > 0)
            {
                SetPreSelectedProject();
            }
        }
        #endregion

        #region View-Only Mode Methods
        private void SetViewOnlyMode()
        {
            // Disable all input controls
            txtTransactionCode.Enabled = false;
            cmbTransactionType.Enabled = false;
            cmbCategory.Enabled = false;
            txtAmount.Enabled = false;
            dtpTransactionDate.Enabled = false;
            cmbProject.Enabled = false;
            cmbCustomer.Enabled = false;
            cmbEmployee.Enabled = false;
            cmbPaymentMethod.Enabled = false;
            txtReferenceNo.Enabled = false;
            txtDescription.Enabled = false;
            cmbStatus.Enabled = false;

            // Hide action buttons
            btnSave.Visible = false;
            btnGenerateCode.Visible = false;

            // Change cancel button to close
            btnCancel.Text = "ĐÓNG";
            btnCancel.Location = new Point(btnSave.Location.X, btnCancel.Location.Y);

            // Update title
            this.Text = "Xem chi tiết Giao dịch Tài chính";

            // Change form colors for read-only indication
            foreach (Control control in this.Controls)
            {
                SetControlReadOnly(control);
            }
        }

        private void SetControlReadOnly(Control control)
        {
            if (control is MaterialTextBox textBox)
            {
                textBox.BackColor = Color.FromArgb(248, 249, 250);
                textBox.ForeColor = Color.FromArgb(64, 64, 64);
            }
            else if (control is MaterialComboBox comboBox)
            {
                comboBox.BackColor = Color.FromArgb(248, 249, 250);
                comboBox.ForeColor = Color.FromArgb(64, 64, 64);
            }
            else if (control is DateTimePicker datePicker)
            {
                datePicker.BackColor = Color.FromArgb(248, 249, 250);
                datePicker.ForeColor = Color.FromArgb(64, 64, 64);
            }

            // Recursively set for child controls
            foreach (Control childControl in control.Controls)
            {
                SetControlReadOnly(childControl);
            }
        }
        #endregion

        #region Setup Methods
        private void SetupMaterialDesign()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void SetupControls()
        {
            string title = isViewOnlyMode ? "Xem chi tiết Giao dịch Tài chính" :
                                      isEditMode ? "Cập nhật Giao dịch Tài chính" :
                                      "Thêm Giao dịch Tài chính";

            // Main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };
            this.Controls.Add(mainPanel);

            // Title
            Label lblTitle = new Label
            {
                Text = title.ToUpper(),
                Font = new Font("Roboto", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 71, 79),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            mainPanel.Controls.Add(lblTitle);

            // Validation message
            lblValidationMessage = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(0, 35),
                Visible = false
            };
            mainPanel.Controls.Add(lblValidationMessage);

            int yPos = 70;
            int labelWidth = 150;
            int controlWidth = 300;
            int spacing = 45;

            // Transaction Code
            CreateLabel(mainPanel, "Mã giao dịch:", 0, yPos, labelWidth);
            txtTransactionCode = CreateTextBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            btnGenerateCode = new MaterialButton
            {
                Text = "Tạo mã",
                Location = new Point(labelWidth + controlWidth + 20, yPos - 5),
                Size = new Size(80, 36),
                Type = MaterialButton.MaterialButtonType.Contained
            };
            btnGenerateCode.Click += BtnGenerateCode_Click;
            mainPanel.Controls.Add(btnGenerateCode);
            yPos += spacing;

            // Transaction Type
            CreateLabel(mainPanel, "Loại giao dịch:", 0, yPos, labelWidth);
            cmbTransactionType = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            cmbTransactionType.SelectedIndexChanged += CmbTransactionType_SelectedIndexChanged;
            yPos += spacing;

            // Category
            CreateLabel(mainPanel, "Danh mục:", 0, yPos, labelWidth);
            cmbCategory = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            yPos += spacing;

            // Amount
            CreateLabel(mainPanel, "Số tiền:", 0, yPos, labelWidth);
            txtAmount = CreateTextBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            txtAmount.KeyPress += TxtAmount_KeyPress;
            yPos += spacing;

            // Transaction Date
            CreateLabel(mainPanel, "Ngày giao dịch:", 0, yPos, labelWidth);
            dtpTransactionDate = new DateTimePicker
            {
                Location = new Point(labelWidth + 10, yPos),
                Size = new Size(controlWidth, 30),
                Format = DateTimePickerFormat.Short
            };
            mainPanel.Controls.Add(dtpTransactionDate);
            yPos += spacing;

            // Project
            CreateLabel(mainPanel, "Dự án:", 0, yPos, labelWidth);
            cmbProject = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            cmbProject.SelectedIndexChanged += CmbProject_SelectedIndexChanged;
            yPos += spacing;

            // Customer
            CreateLabel(mainPanel, "Khách hàng:", 0, yPos, labelWidth);
            cmbCustomer = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            yPos += spacing;

            // Employee
            CreateLabel(mainPanel, "Nhân viên:", 0, yPos, labelWidth);
            cmbEmployee = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            cmbEmployee.SelectedIndexChanged += CmbEmployee_SelectedIndexChanged;
            yPos += spacing;

            // Payment Method
            CreateLabel(mainPanel, "Phương thức:", 0, yPos, labelWidth);
            cmbPaymentMethod = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            yPos += spacing;

            // Reference No
            CreateLabel(mainPanel, "Số tham chiếu:", 0, yPos, labelWidth);
            txtReferenceNo = CreateTextBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            yPos += spacing;

            // Status
            CreateLabel(mainPanel, "Trạng thái:", 0, yPos, labelWidth);
            cmbStatus = CreateComboBox(mainPanel, labelWidth + 10, yPos, controlWidth);
            yPos += spacing;

            // Description
            CreateLabel(mainPanel, "Mô tả:", 0, yPos, labelWidth);
            txtDescription = new MaterialTextBox
            {
                Location = new Point(labelWidth + 10, yPos),
                Size = new Size(controlWidth, 100),
                Multiline = true,
                MaxLength = 500
            };
            mainPanel.Controls.Add(txtDescription);
            yPos += 120;

            // Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(this.Width, 60),
                BackColor = Color.White
            };
            mainPanel.Controls.Add(buttonPanel);

            btnSave = new MaterialButton
            {
                Text = "Lưu",
                Location = new Point(labelWidth + 10, 10),
                Size = new Size(100, 36),
                Type = MaterialButton.MaterialButtonType.Contained
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new MaterialButton
            {
                Text = "Hủy",
                Location = new Point(labelWidth + 120, 10),
                Size = new Size(100, 36),
                Type = MaterialButton.MaterialButtonType.Text
            };
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);

            // Adjust form height
            this.Height = yPos + 120;
        }

        private Label CreateLabel(Control parent, string text, int x, int y, int width)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y + 5),
                Size = new Size(width, 23),
                Font = new Font("Roboto", 10),
                ForeColor = Color.FromArgb(55, 71, 79)
            };
            parent.Controls.Add(label);
            return label;
        }

        private MaterialTextBox CreateTextBox(Control parent, int x, int y, int width)
        {
            MaterialTextBox textBox = new MaterialTextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 30)
            };
            parent.Controls.Add(textBox);
            return textBox;
        }

        private MaterialComboBox CreateComboBox(Control parent, int x, int y, int width)
        {
            MaterialComboBox comboBox = new MaterialComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            parent.Controls.Add(comboBox);
            return comboBox;
        }
        #endregion

        #region Data Loading
        private void LoadComboBoxData()
        {
            try
            {
                // Transaction Types
                cmbTransactionType.Items.Clear();
                cmbTransactionType.Items.Add("");
                foreach (var type in TransactionTypes.Types)
                {
                    cmbTransactionType.Items.Add(type);
                }

                // Payment Methods
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("");
                foreach (var method in PaymentMethods.Methods)
                {
                    cmbPaymentMethod.Items.Add(method);
                }

                // Status
                cmbStatus.Items.Clear();
                cmbStatus.Items.Add("");
                foreach (var status in FinanceStatus.Statuses)
                {
                    cmbStatus.Items.Add(status);
                }

                // Projects
                cmbProject.Items.Clear();
                cmbProject.Items.Add(new ComboBoxItem { Text = "", Value = null });
                var projects = financeBLL.GetProjectsForDropdown();
                foreach (var project in projects)
                {
                    cmbProject.Items.Add(new ComboBoxItem { Text = project.ProjectName, Value = project.ProjectID });
                }

                // Customers
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add(new ComboBoxItem { Text = "", Value = null });
                var customers = financeBLL.GetCustomersForDropdown();
                foreach (var customer in customers)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem { Text = customer.CompanyName, Value = customer.CustomerID });
                }

                // Employees
                cmbEmployee.Items.Clear();
                cmbEmployee.Items.Add(new ComboBoxItem { Text = "", Value = null });
                var employees = financeBLL.GetEmployeesForDropdown();
                foreach (var employee in employees)
                {
                    cmbEmployee.Items.Add(new ComboBoxItem { Text = employee.FullName, Value = employee.EmployeeID });
                }

                // Set pre-selected project if exists
                if (preSelectedProject != null)
                {
                    SetPreSelectedProject();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoryByTransactionType(string transactionType)
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("");

            if (!string.IsNullOrEmpty(transactionType))
            {
                var categories = FinanceCategories.GetCategoriesByType(transactionType);
                foreach (var category in categories)
                {
                    cmbCategory.Items.Add(category);
                }
            }
        }

        private void SetDefaultValues()
        {
            dtpTransactionDate.Value = DateTime.Now;
            cmbStatus.SelectedIndex = cmbStatus.Items.Cast<object>()
                .ToList().FindIndex(item => item.ToString() == "Đã ghi nhận");
        }

        private void LoadFinanceData()
        {
            try
            {
                currentFinance = financeBLL.GetFinanceById(financeId);
                if (currentFinance == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin giao dịch", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                LoadFinanceFromObject();
                btnGenerateCode.Enabled = false; // Disable generate code in edit mode
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadFinanceFromObject()
        {
            if (currentFinance == null) return;

            // Fill data
            txtTransactionCode.Text = currentFinance.TransactionCode;
            cmbTransactionType.SelectedItem = currentFinance.TransactionType;
            LoadCategoryByTransactionType(currentFinance.TransactionType);
            cmbCategory.SelectedItem = currentFinance.Category;
            txtAmount.Text = currentFinance.Amount.ToString("N0");
            dtpTransactionDate.Value = currentFinance.TransactionDate;
            txtReferenceNo.Text = currentFinance.ReferenceNo;
            txtDescription.Text = currentFinance.Description;
            cmbPaymentMethod.SelectedItem = currentFinance.PaymentMethod;
            cmbStatus.SelectedItem = currentFinance.Status;

            // Set selected items for combo boxes with values
            SetComboBoxSelectedValue(cmbProject, currentFinance.ProjectID);
            SetComboBoxSelectedValue(cmbCustomer, currentFinance.CustomerID);
            SetComboBoxSelectedValue(cmbEmployee, currentFinance.EmployeeID);
        }

        private void SetComboBoxSelectedValue(MaterialComboBox comboBox, int? value)
        {
            if (value.HasValue)
            {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i] is ComboBoxItem item && item.Value?.Equals(value.Value) == true)
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        private void BtnGenerateCode_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cmbTransactionType.Text))
                {
                    MessageBox.Show("Vui lòng chọn loại giao dịch trước", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string code = financeBLL.GenerateTransactionCode(cmbTransactionType.Text);
                txtTransactionCode.Text = code;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCategoryByTransactionType(cmbTransactionType.Text);
        }

        private void CmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProject.SelectedItem is ComboBoxItem item && item.Value != null)
            {
                // Clear other relationship selections
                cmbCustomer.SelectedIndex = 0;
                cmbEmployee.SelectedIndex = 0;
            }
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem is ComboBoxItem item && item.Value != null)
            {
                // Clear other relationship selections
                cmbProject.SelectedIndex = 0;
                cmbEmployee.SelectedIndex = 0;
            }
        }

        private void CmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployee.SelectedItem is ComboBoxItem item && item.Value != null)
            {
                // Clear other relationship selections
                cmbProject.SelectedIndex = 0;
                cmbCustomer.SelectedIndex = 0;
            }
        }

        private void TxtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow numbers, decimal point, and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if ((e.KeyChar == '.' || e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf('.') > -1 || (sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var finance = CreateFinanceFromForm();

                    if (isEditMode)
                    {
                        finance.FinanceID = financeId;
                        bool success = financeBLL.UpdateFinance(finance);
                        if (success)
                        {
                            MessageBox.Show("Cập nhật giao dịch thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật giao dịch thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        int newId = financeBLL.AddFinance(finance);
                        if (newId > 0)
                        {
                            MessageBox.Show("Thêm giao dịch thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Thêm giao dịch thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Validation
        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtTransactionCode.Text))
                errors.Add("Mã giao dịch không được để trống");

            if (string.IsNullOrWhiteSpace(cmbTransactionType.Text))
                errors.Add("Vui lòng chọn loại giao dịch");

            if (string.IsNullOrWhiteSpace(cmbCategory.Text))
                errors.Add("Vui lòng chọn danh mục");

            if (string.IsNullOrWhiteSpace(txtAmount.Text) || !decimal.TryParse(txtAmount.Text.Replace(",", ""), out decimal amount) || amount <= 0)
                errors.Add("Số tiền phải là số dương");

            if (string.IsNullOrWhiteSpace(cmbStatus.Text))
                errors.Add("Vui lòng chọn trạng thái");

            // Check relationship constraint
            int relationshipCount = 0;
            if (GetComboBoxValue(cmbProject).HasValue) relationshipCount++;
            if (GetComboBoxValue(cmbCustomer).HasValue) relationshipCount++;
            if (GetComboBoxValue(cmbEmployee).HasValue) relationshipCount++;

            if (relationshipCount > 1)
                errors.Add("Giao dịch chỉ có thể liên kết với một đối tượng (Dự án, Khách hàng, hoặc Nhân viên)");

            if (errors.Any())
            {
                lblValidationMessage.Text = string.Join("\n", errors);
                lblValidationMessage.Visible = true;
                return false;
            }

            lblValidationMessage.Visible = false;
            return true;
        }

        private Models.Finance CreateFinanceFromForm()
        {
            var finance = new Models.Finance
            {
                TransactionCode = txtTransactionCode.Text.Trim(),
                TransactionType = cmbTransactionType.Text,
                Category = cmbCategory.Text,
                Amount = decimal.Parse(txtAmount.Text.Replace(",", "")),
                TransactionDate = dtpTransactionDate.Value.Date,
                ProjectID = GetComboBoxValue(cmbProject),
                CustomerID = GetComboBoxValue(cmbCustomer),
                EmployeeID = GetComboBoxValue(cmbEmployee),
                PaymentMethod = cmbPaymentMethod.Text,
                ReferenceNo = txtReferenceNo.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Status = cmbStatus.Text,
                RecordedByID = currentUserId
            };

            return finance;
        }

        private int? GetComboBoxValue(MaterialComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item && item.Value != null)
            {
                return (int)item.Value;
            }
            return null;
        }
        #endregion

        #region Helper Classes
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        #endregion
    }
}