using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Customer
{
    public partial class CustomerDetailForm : Form
    {
        #region Fields
        private CustomerBLL customerBLL;
        private Models.Entity.Customer currentCustomer;
        private bool isEditMode;
        private bool isViewOnly;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private Label subtitleLabel;

        // Content controls
        private GroupBox companyInfoGroup;
        private GroupBox contactInfoGroup;
        private GroupBox otherInfoGroup;

        // Company info controls
        private Label lblCustomerCode;
        private TextBox txtCustomerCode;
        private Label lblCompanyName;
        private TextBox txtCompanyName;
        private Label lblStatus;
        private ComboBox cmbStatus;

        // Contact info controls
        private Label lblContactName;
        private TextBox txtContactName;
        private Label lblContactTitle;
        private TextBox txtContactTitle;
        private Label lblPhone;
        private TextBox txtPhone;
        private Label lblEmail;
        private TextBox txtEmail;

        // Other info controls
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblNotes;
        private TextBox txtNotes;

        // Footer controls
        private Button btnSave;
        private Button btnCancel;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnGenerateCode;
        #endregion

        #region Constructors
        public CustomerDetailForm()
        {
            InitializeComponent();
            customerBLL = new CustomerBLL();
            isEditMode = false;
            isViewOnly = false;
            InitializeLayout();
            SetupNewCustomer();
        }

        public CustomerDetailForm(Models.Entity.Customer customer, bool viewOnly = false)
        {
            InitializeComponent();
            customerBLL = new CustomerBLL();
            currentCustomer = customer;
            isEditMode = customer != null;
            isViewOnly = viewOnly;
            InitializeLayout();
            if (customer != null)
                LoadCustomerData();
        }
        #endregion

        #region Form Setup
        private void InitializeLayout()
        {
            this.Text = isEditMode ? (isViewOnly ? "Chi tiết khách hàng" : "Chỉnh sửa khách hàng") : "Thêm khách hàng mới";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            SetupMainLayout();
            SetupHeader();
            SetupContent();
            SetupFooter();
            ApplyFormMode();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Content
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));   // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            titleLabel = new Label
            {
                Text = isEditMode ? (isViewOnly ? "CHI TIẾT KHÁCH HÀNG" : "CHỈNH SỬA KHÁCH HÀNG") : "THÊM KHÁCH HÀNG MỚI",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Size = new Size(760, 35),
                Location = new Point(0, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            subtitleLabel = new Label
            {
                Text = isEditMode ? $"Mã: {currentCustomer?.CustomerCode}" : "Vui lòng điền đầy đủ thông tin bên dưới",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(128, 128, 128),
                AutoSize = false,
                Size = new Size(760, 25),
                Location = new Point(0, 45),
                TextAlign = ContentAlignment.MiddleLeft
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
                AutoScroll = true
            };

            SetupCompanyInfoGroup();
            SetupContactInfoGroup();
            SetupOtherInfoGroup();

            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupCompanyInfoGroup()
        {
            companyInfoGroup = new GroupBox
            {
                Text = "Thông tin công ty",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(740, 150),
                Location = new Point(10, 10)
            };

            // Customer Code
            lblCustomerCode = CreateLabel("Mã khách hàng (*)", new Point(20, 30));
            txtCustomerCode = CreateTextBox(new Point(150, 27));
            btnGenerateCode = new Button
            {
                Text = "Tự động",
                Size = new Size(80, 25),
                Location = new Point(320, 27),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnGenerateCode.Click += BtnGenerateCode_Click;

            // Company Name
            lblCompanyName = CreateLabel("Tên công ty (*)", new Point(20, 65));
            txtCompanyName = CreateTextBox(new Point(150, 62), 400);

            // Status
            lblStatus = CreateLabel("Trạng thái", new Point(20, 100));
            cmbStatus = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(150, 97),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbStatus.Items.AddRange(new[] { "Đang hợp tác", "Tạm dừng", "Ngừng hợp tác" });
            cmbStatus.SelectedIndex = 0;

            companyInfoGroup.Controls.AddRange(new Control[] {
                lblCustomerCode, txtCustomerCode, btnGenerateCode,
                lblCompanyName, txtCompanyName,
                lblStatus, cmbStatus
            });

            contentPanel.Controls.Add(companyInfoGroup);
        }

        private void SetupContactInfoGroup()
        {
            contactInfoGroup = new GroupBox
            {
                Text = "Thông tin liên hệ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(740, 150),
                Location = new Point(10, 170)
            };

            // Contact Name
            lblContactName = CreateLabel("Người liên hệ", new Point(20, 30));
            txtContactName = CreateTextBox(new Point(150, 27), 200);

            // Contact Title
            lblContactTitle = CreateLabel("Chức vụ", new Point(370, 30));
            txtContactTitle = CreateTextBox(new Point(450, 27), 200);

            // Phone
            lblPhone = CreateLabel("Điện thoại", new Point(20, 65));
            txtPhone = CreateTextBox(new Point(150, 62), 200);

            // Email
            lblEmail = CreateLabel("Email", new Point(370, 65));
            txtEmail = CreateTextBox(new Point(450, 62), 250);

            contactInfoGroup.Controls.AddRange(new Control[] {
                lblContactName, txtContactName,
                lblContactTitle, txtContactTitle,
                lblPhone, txtPhone,
                lblEmail, txtEmail
            });

            contentPanel.Controls.Add(contactInfoGroup);
        }

        private void SetupOtherInfoGroup()
        {
            otherInfoGroup = new GroupBox
            {
                Text = "Thông tin khác",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(740, 150),
                Location = new Point(10, 330)
            };

            // Address
            lblAddress = CreateLabel("Địa chỉ", new Point(20, 30));
            txtAddress = CreateTextBox(new Point(80, 27), 600);

            // Notes
            lblNotes = CreateLabel("Ghi chú", new Point(20, 65));
            txtNotes = new TextBox
            {
                Size = new Size(600, 60),
                Location = new Point(80, 62),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9)
            };

            otherInfoGroup.Controls.AddRange(new Control[] {
                lblAddress, txtAddress,
                lblNotes, txtNotes
            });

            contentPanel.Controls.Add(otherInfoGroup);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 15, 20, 15)
            };

            btnCancel = CreateButton("Hủy", Color.FromArgb(108, 117, 125));
            btnCancel.Click += BtnCancel_Click;

            btnSave = CreateButton("Lưu", Color.FromArgb(40, 167, 69));
            btnSave.Click += BtnSave_Click;

            btnEdit = CreateButton("Chỉnh sửa", Color.FromArgb(255, 193, 7));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateButton("Xóa", Color.FromArgb(220, 53, 69));
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);

            footerPanel.Controls.Add(buttonPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
        }

        private TextBox CreateTextBox(Point location, int width = 150)
        {
            return new TextBox
            {
                Location = location,
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 9)
            };
        }

        private Button CreateButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(100, 35),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0)
            };
        }
        #endregion

        #region Data Management
        private void SetupNewCustomer()
        {
            currentCustomer = new Models.Entity.Customer
            {
                Status = "Đang hợp tác",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            ClearForm();
        }

        private void LoadCustomerData()
        {
            if (currentCustomer == null) return;

            txtCustomerCode.Text = currentCustomer.CustomerCode;
            txtCompanyName.Text = currentCustomer.CompanyName;
            txtContactName.Text = currentCustomer.ContactName;
            txtContactTitle.Text = currentCustomer.ContactTitle;
            txtPhone.Text = currentCustomer.Phone;
            txtEmail.Text = currentCustomer.Email;
            txtAddress.Text = currentCustomer.Address;
            txtNotes.Text = currentCustomer.Notes;

            // Set status
            for (int i = 0; i < cmbStatus.Items.Count; i++)
            {
                if (cmbStatus.Items[i].ToString() == currentCustomer.Status)
                {
                    cmbStatus.SelectedIndex = i;
                    break;
                }
            }

            // Update subtitle
            subtitleLabel.Text = $"Mã: {currentCustomer.CustomerCode} | Tạo: {currentCustomer.CreatedAt:dd/MM/yyyy}";
        }

        private void ClearForm()
        {
            txtCustomerCode.Text = "";
            txtCompanyName.Text = "";
            txtContactName.Text = "";
            txtContactTitle.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtNotes.Text = "";
            cmbStatus.SelectedIndex = 0;
        }

        private Models.Entity.Customer GetCustomerFromForm()
        {
            var customer = currentCustomer ?? new Models.Entity.Customer();

            customer.CustomerCode = txtCustomerCode.Text.Trim();
            customer.CompanyName = txtCompanyName.Text.Trim();
            customer.ContactName = txtContactName.Text.Trim();
            customer.ContactTitle = txtContactTitle.Text.Trim();
            customer.Phone = txtPhone.Text.Trim();
            customer.Email = txtEmail.Text.Trim();
            customer.Address = txtAddress.Text.Trim();
            customer.Notes = txtNotes.Text.Trim();
            customer.Status = cmbStatus.SelectedItem?.ToString() ?? "Đang hợp tác";

            return customer;
        }

        private void ApplyFormMode()
        {
            bool isReadOnly = isViewOnly;

            // Enable/disable controls
            txtCustomerCode.ReadOnly = isEditMode || isReadOnly;
            txtCompanyName.ReadOnly = isReadOnly;
            txtContactName.ReadOnly = isReadOnly;
            txtContactTitle.ReadOnly = isReadOnly;
            txtPhone.ReadOnly = isReadOnly;
            txtEmail.ReadOnly = isReadOnly;
            txtAddress.ReadOnly = isReadOnly;
            txtNotes.ReadOnly = isReadOnly;
            cmbStatus.Enabled = !isReadOnly;
            btnGenerateCode.Visible = !isEditMode && !isReadOnly;

            // Show/hide buttons
            btnSave.Visible = !isReadOnly;
            btnEdit.Visible = isViewOnly;
            btnDelete.Visible = isViewOnly;

            // Change background color for readonly fields
            if (isReadOnly)
            {
                Color readOnlyColor = Color.FromArgb(248, 249, 250);
                txtCustomerCode.BackColor = readOnlyColor;
                txtCompanyName.BackColor = readOnlyColor;
                txtContactName.BackColor = readOnlyColor;
                txtContactTitle.BackColor = readOnlyColor;
                txtPhone.BackColor = readOnlyColor;
                txtEmail.BackColor = readOnlyColor;
                txtAddress.BackColor = readOnlyColor;
                txtNotes.BackColor = readOnlyColor;
            }
        }
        #endregion

        #region Validation
        private bool ValidateForm()
        {
            var errors = new List<string>();

            // Required fields
            if (string.IsNullOrWhiteSpace(txtCustomerCode.Text))
                errors.Add("Mã khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
                errors.Add("Tên công ty không được để trống");

            // Email validation
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                        errors.Add("Email không hợp lệ");
                }
                catch
                {
                    errors.Add("Email không hợp lệ");
                }
            }

            // Phone validation
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phone = txtPhone.Text.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10,11}$"))
                    errors.Add("Số điện thoại không hợp lệ (10-11 chữ số)");
            }

            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors), "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        #endregion

        #region Event Handlers
        private void BtnGenerateCode_Click(object sender, EventArgs e)
        {
            try
            {
                txtCustomerCode.Text = customerBLL.GenerateCustomerCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var customer = GetCustomerFromForm();

                if (isEditMode)
                {
                    // Update existing customer
                    if (customerBLL.UpdateCustomer(customer))
                    {
                        MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    // Add new customer
                    int newId = customerBLL.AddCustomer(customer);
                    if (newId > 0)
                    {
                        MessageBox.Show("Thêm khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            isViewOnly = false;
            ApplyFormMode();
            this.Text = "Chỉnh sửa khách hàng";
            titleLabel.Text = "CHỈNH SỬA KHÁCH HÀNG";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (currentCustomer == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa khách hàng '{currentCustomer.CompanyName}'?\n\nHành động này không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var canDelete = customerBLL.CanDeleteCustomer(currentCustomer.CustomerID);
                    if (!canDelete.CanDelete)
                    {
                        MessageBox.Show(canDelete.Reason, "Không thể xóa",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (customerBLL.DeleteCustomer(currentCustomer.CustomerID))
                    {
                        MessageBox.Show("Xóa khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}