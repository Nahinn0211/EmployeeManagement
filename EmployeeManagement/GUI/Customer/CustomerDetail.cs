using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;

namespace EmployeeManagement.GUI.Customer
{
    public partial class CustomerDetailForm : Form
    {
        #region Fields
        private CustomerBLL customerBLL;
        private Models.Customer customer;
        private bool isReadOnly;
        private bool isEditMode;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox customerIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage contactTab;

        // Basic info controls
        private TextBox customerCodeTextBox;
        private TextBox companyNameTextBox;
        private ComboBox statusComboBox;
        private TextBox notesTextBox;

        // Contact info controls
        private TextBox contactNameTextBox;
        private TextBox contactTitleTextBox;
        private TextBox phoneTextBox;
        private TextBox emailTextBox;
        private TextBox addressTextBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        private readonly string[] statuses = { "Đang hợp tác", "Tạm dừng", "Ngừng hợp tác" };

        // Validation
        private ErrorProvider errorProvider;
        #endregion

        #region Constructors
        public CustomerDetailForm()
        {
            InitializeComponent();
            customerBLL = new CustomerBLL();
            customer = new Models.Customer();
            isEditMode = false;
            isReadOnly = false;
            SetupForm();
            SetDefaultValues();
        }

        public CustomerDetailForm(Models.Customer existingCustomer, bool readOnly = false)
        {
            InitializeComponent();
            customerBLL = new CustomerBLL();
            customer = existingCustomer ?? throw new ArgumentNullException(nameof(existingCustomer));
            isEditMode = true;
            isReadOnly = readOnly;
            SetupForm();
            LoadCustomerData();
        }

        public Models.Customer UpdatedCustomer => customer;
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = isReadOnly ? "Xem chi tiết khách hàng" :
                       isEditMode ? "Chỉnh sửa khách hàng" : "Thêm khách hàng mới";
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

            // Customer icon
            customerIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateCustomerIcon();

            // Title label
            string titleText = isReadOnly ? "👁️ CHI TIẾT KHÁCH HÀNG" :
                              isEditMode ? "✏️ CHỈNH SỬA KHÁCH HÀNG" : "➕ THÊM KHÁCH HÀNG MỚI";

            titleLabel = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            string subtitleText = isReadOnly ? "Xem thông tin chi tiết khách hàng" :
                                 isEditMode ? "Cập nhật thông tin khách hàng" : "Nhập thông tin để tạo khách hàng mới";

            var subtitleLabel = new Label
            {
                Text = subtitleText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(customerIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateCustomerIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string iconText = isReadOnly ? "👁️" : isEditMode ? "✏️" : "🏢";
                    var size = g.MeasureString(iconText, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(iconText, font, brush, x, y);
                }
            }
            customerIcon.Image = bmp;
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
            SetupContactTab();

            tabControl.TabPages.Add(basicInfoTab);
            tabControl.TabPages.Add(contactTab);

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

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Customer Code (Required)
            var customerCodePanel = CreateInputPanel("Mã khách hàng *:", out customerCodeTextBox);
            customerCodeTextBox.Leave += CustomerCodeTextBox_Leave;
            customerCodeTextBox.ReadOnly = isEditMode; // Không cho edit mã khi đang sửa
            basicLayout.Controls.Add(customerCodePanel, 0, 0);

            // Status
            var statusPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var statusLabel = new Label
            {
                Text = "Trạng thái:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 0, 0),
                Enabled = !isReadOnly
            };
            statusComboBox.Items.AddRange(statuses);

            statusPanel.Controls.Add(statusComboBox);
            statusPanel.Controls.Add(statusLabel);
            basicLayout.Controls.Add(statusPanel, 1, 0);

            // Company Name (Required)
            var companyNamePanel = CreateInputPanel("Tên công ty *:", out companyNameTextBox, false, true);
            companyNameTextBox.Leave += CompanyNameTextBox_Leave;
            basicLayout.Controls.Add(companyNamePanel, 0, 1);
            basicLayout.SetColumnSpan(companyNamePanel, 2);

            // Notes
            var notesPanel = CreateInputPanel("Ghi chú:", out notesTextBox, true);
            basicLayout.Controls.Add(notesPanel, 0, 3);
            basicLayout.SetColumnSpan(notesPanel, 2);

            basicInfoTab.Controls.Add(basicLayout);
        }

        private void SetupContactTab()
        {
            contactTab = new TabPage
            {
                Text = "Thông tin liên hệ",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var contactLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            contactLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            contactLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            contactLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            contactLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            contactLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            contactLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Contact Name
            var contactNamePanel = CreateInputPanel("Tên người liên hệ:", out contactNameTextBox);
            contactLayout.Controls.Add(contactNamePanel, 0, 0);

            // Contact Title
            var contactTitlePanel = CreateInputPanel("Chức vụ:", out contactTitleTextBox);
            contactLayout.Controls.Add(contactTitlePanel, 1, 0);

            // Phone
            var phonePanel = CreateInputPanel("Số điện thoại:", out phoneTextBox);
            phoneTextBox.Leave += PhoneTextBox_Leave;
            phoneTextBox.KeyPress += NumbersOnly_KeyPress;
            contactLayout.Controls.Add(phonePanel, 0, 1);

            // Email
            var emailPanel = CreateInputPanel("Email:", out emailTextBox);
            emailTextBox.Leave += EmailTextBox_Leave;
            contactLayout.Controls.Add(emailPanel, 1, 1);

            // Address (spanning 2 columns)
            var addressPanel = CreateInputPanel("Địa chỉ:", out addressTextBox, true);
            contactLayout.Controls.Add(addressPanel, 0, 3);
            contactLayout.SetColumnSpan(addressPanel, 2);

            contactTab.Controls.Add(contactLayout);
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
                string saveText = isEditMode ? "💾 Cập nhật" : "💾 Tạo khách hàng";
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
        private Panel CreateInputPanel(string labelText, out TextBox textBox, bool isMultiline = false, bool spanColumns = false)
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

        private void CustomerCodeTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly && !isEditMode)
                ValidateCustomerCode();
        }

        private void CompanyNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidateCompanyName();
        }

        private void PhoneTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidatePhone();
        }

        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidateEmail();
        }

        private void NumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, backspace, space, dash, parentheses, and plus
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ' ' &&
                e.KeyChar != '-' && e.KeyChar != '(' && e.KeyChar != ')' && e.KeyChar != '+')
            {
                e.Handled = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveCustomer();

                    if (isEditMode)
                    {
                        if (customerBLL.UpdateCustomer(customer))
                        {
                            MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        int newCustomerId = customerBLL.AddCustomer(customer);
                        customer.CustomerID = newCustomerId;
                        MessageBox.Show("Thêm khách hàng mới thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}", "Lỗi",
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
                    LoadCustomerData();
                else
                    ResetForm();
            }
        }
        #endregion

        #region Validation
        private bool ValidateCustomerCode()
        {
            errorProvider.SetError(customerCodeTextBox, "");

            if (string.IsNullOrWhiteSpace(customerCodeTextBox.Text))
            {
                errorProvider.SetError(customerCodeTextBox, "Mã khách hàng không được để trống");
                return false;
            }

            if (customerCodeTextBox.Text.Length < 3)
            {
                errorProvider.SetError(customerCodeTextBox, "Mã khách hàng phải có ít nhất 3 ký tự");
                return false;
            }

            // Kiểm tra trùng lặp
            try
            {
                int excludeId = isEditMode ? customer.CustomerID : 0;
                if (customerBLL.IsCustomerCodeExists(customerCodeTextBox.Text.Trim(), excludeId))
                {
                    errorProvider.SetError(customerCodeTextBox, "Mã khách hàng này đã tồn tại");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra mã khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateCompanyName()
        {
            errorProvider.SetError(companyNameTextBox, "");

            if (string.IsNullOrWhiteSpace(companyNameTextBox.Text))
            {
                errorProvider.SetError(companyNameTextBox, "Tên công ty không được để trống");
                return false;
            }

            if (companyNameTextBox.Text.Length > 200)
            {
                errorProvider.SetError(companyNameTextBox, "Tên công ty không được vượt quá 200 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidatePhone()
        {
            errorProvider.SetError(phoneTextBox, "");

            if (!string.IsNullOrWhiteSpace(phoneTextBox.Text))
            {
                string cleanPhone = phoneTextBox.Text.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");

                if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
                {
                    errorProvider.SetError(phoneTextBox, "Số điện thoại phải có 10 hoặc 11 chữ số");
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(cleanPhone, @"^\d+$"))
                {
                    errorProvider.SetError(phoneTextBox, "Số điện thoại chỉ được chứa số");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateEmail()
        {
            errorProvider.SetError(emailTextBox, "");

            if (!string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                string email = emailTextBox.Text.Trim();

                if (!email.Contains("@") || !email.Contains("."))
                {
                    errorProvider.SetError(emailTextBox, "Email không hợp lệ");
                    return false;
                }

                if (email.Length > 100)
                {
                    errorProvider.SetError(emailTextBox, "Email không được vượt quá 100 ký tự");
                    return false;
                }

                // Kiểm tra trùng lặp
                try
                {
                    int excludeId = isEditMode ? customer.CustomerID : 0;
                    if (customerBLL.IsEmailExists(email, excludeId))
                    {
                        errorProvider.SetError(emailTextBox, "Email này đã được sử dụng");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi kiểm tra email: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (!isEditMode) // Chỉ validate customer code khi thêm mới
                isValid &= ValidateCustomerCode();

            isValid &= ValidateCompanyName();
            isValid &= ValidatePhone();
            isValid &= ValidateEmail();

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
            if (!isEditMode)
            {
                customerCodeTextBox.Text = customerBLL.GenerateCustomerCode();
                statusComboBox.SelectedIndex = 0; // Đang hợp tác
            }
        }

        private void LoadCustomerData()
        {
            if (customer == null) return;

            try
            {
                customerCodeTextBox.Text = customer.CustomerCode;
                companyNameTextBox.Text = customer.CompanyName;
                contactNameTextBox.Text = customer.ContactName;
                contactTitleTextBox.Text = customer.ContactTitle;
                phoneTextBox.Text = customer.Phone;
                emailTextBox.Text = customer.Email;
                addressTextBox.Text = customer.Address;
                notesTextBox.Text = customer.Notes;

                // Set status
                int statusIndex = Array.IndexOf(statuses, customer.Status);
                statusComboBox.SelectedIndex = statusIndex >= 0 ? statusIndex : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            // Clear all text boxes
            customerCodeTextBox.Clear();
            companyNameTextBox.Clear();
            contactNameTextBox.Clear();
            contactTitleTextBox.Clear();
            phoneTextBox.Clear();
            emailTextBox.Clear();
            addressTextBox.Clear();
            notesTextBox.Clear();

            // Reset combo box
            statusComboBox.SelectedIndex = 0;

            // Clear error provider
            errorProvider.Clear();

            // Set default values again
            SetDefaultValues();

            // Return to first tab
            tabControl.SelectedIndex = 0;
        }

        private void SaveCustomer()
        {
            try
            {
                customer.CustomerCode = customerCodeTextBox.Text.Trim();
                customer.CompanyName = companyNameTextBox.Text.Trim();
                customer.ContactName = contactNameTextBox.Text.Trim();
                customer.ContactTitle = contactTitleTextBox.Text.Trim();
                customer.Phone = phoneTextBox.Text.Trim();
                customer.Email = emailTextBox.Text.Trim();
                customer.Address = addressTextBox.Text.Trim();
                customer.Notes = notesTextBox.Text.Trim();
                customer.Status = statusComboBox.Text;

                if (!isEditMode)
                {
                    customer.CreatedAt = DateTime.Now;
                }
                customer.UpdatedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin khách hàng: {ex.Message}", ex);
            }
        }
        #endregion
    }
}