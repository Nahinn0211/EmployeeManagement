using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Admin
{
    public partial class UserDetailForm : Form
    {
        #region Fields
        private UserBLL userBLL;
        private User user;
        private bool isReadOnly;
        private bool isEditMode;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox userIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage accountTab;
        private TabPage rolesTab;

        // Basic info controls
        private TextBox usernameTextBox;
        private TextBox fullNameTextBox;
        private TextBox emailTextBox;
        private ComboBox employeeComboBox;
        private CheckBox isActiveCheckBox;

        // Account info controls
        private TextBox passwordTextBox;
        private TextBox confirmPasswordTextBox;
        private Button generatePasswordButton;
        private CheckBox showPasswordCheckBox;

        // Roles controls
        private CheckedListBox rolesCheckedListBox;
        private Label selectedRolesLabel;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        // Validation
        private ErrorProvider errorProvider;

        // Data
        private List<Models.Entity.Employee> availableEmployees;
        private List<Role> allRoles;
        #endregion

        #region Constructors
        public UserDetailForm()
        {
            InitializeComponent();
            userBLL = new UserBLL();
            user = new User();
            isEditMode = false;
            isReadOnly = false;
            SetupForm();
            LoadDropdownData();
            SetDefaultValues();
        }

        public UserDetailForm(User existingUser, bool readOnly = false)
        {
            InitializeComponent();
            userBLL = new UserBLL();
            user = existingUser ?? throw new ArgumentNullException(nameof(existingUser));
            isEditMode = true;
            isReadOnly = readOnly;
            SetupForm();
            LoadDropdownData();
            LoadUserData();
        }

        public User UpdatedUser => user;
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = isReadOnly ? "Xem chi tiết người dùng" :
                       isEditMode ? "Chỉnh sửa người dùng" : "Thêm người dùng mới";
            this.Size = new Size(900, 750);
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

            // User icon
            userIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateUserIcon();

            // Title label
            string titleText = isReadOnly ? "👁️ CHI TIẾT NGƯỜI DÙNG" :
                              isEditMode ? "✏️ CHỈNH SỬA NGƯỜI DÙNG" : "➕ THÊM NGƯỜI DÙNG MỚI";

            titleLabel = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            string subtitleText = isReadOnly ? "Xem thông tin chi tiết người dùng" :
                                 isEditMode ? "Cập nhật thông tin người dùng" : "Nhập thông tin để tạo người dùng mới";

            var subtitleLabel = new Label
            {
                Text = subtitleText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(userIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateUserIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string iconText = isReadOnly ? "👁️" : isEditMode ? "✏️" : "👥";
                    var size = g.MeasureString(iconText, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(iconText, font, brush, x, y);
                }
            }
            userIcon.Image = bmp;
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
            SetupAccountTab();
            SetupRolesTab();

            tabControl.TabPages.Add(basicInfoTab);
            if (!isReadOnly) // Only show account tab when creating/editing
                tabControl.TabPages.Add(accountTab);
            tabControl.TabPages.Add(rolesTab);

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
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            // Username (Required)
            var usernamePanel = CreateInputPanel("Tên đăng nhập *:", out usernameTextBox);
            usernameTextBox.Leave += UsernameTextBox_Leave;
            usernameTextBox.ReadOnly = isEditMode; // Don't allow editing username
            basicLayout.Controls.Add(usernamePanel, 0, 0);

            // Full Name
            var fullNamePanel = CreateInputPanel("Họ tên:", out fullNameTextBox);
            fullNameTextBox.Leave += FullNameTextBox_Leave;
            basicLayout.Controls.Add(fullNamePanel, 1, 0);

            // Email
            var emailPanel = CreateInputPanel("Email:", out emailTextBox);
            emailTextBox.Leave += EmailTextBox_Leave;
            basicLayout.Controls.Add(emailPanel, 0, 1);

            // Employee
            var employeePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var employeeLabel = new Label
            {
                Text = "Nhân viên liên kết:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            employeeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 0, 0),
                Enabled = !isReadOnly
            };

            employeePanel.Controls.Add(employeeComboBox);
            employeePanel.Controls.Add(employeeLabel);
            basicLayout.Controls.Add(employeePanel, 1, 1);

            // Active status
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
                Text = "Trạng thái tài khoản:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            isActiveCheckBox = new CheckBox
            {
                Text = "Tài khoản hoạt động",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Checked = true,
                Enabled = !isReadOnly,
                Margin = new Padding(0, 5, 0, 0)
            };

            statusPanel.Controls.Add(isActiveCheckBox);
            statusPanel.Controls.Add(statusLabel);
            basicLayout.Controls.Add(statusPanel, 0, 2);
            basicLayout.SetColumnSpan(statusPanel, 2);

            basicInfoTab.Controls.Add(basicLayout);
        }

        private void SetupAccountTab()
        {
            accountTab = new TabPage
            {
                Text = "Thông tin tài khoản",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var accountLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            accountLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            accountLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            accountLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            accountLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            accountLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            accountLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Password
            var passwordPanel = CreateInputPanel(isEditMode ? "Mật khẩu mới (để trống nếu không đổi):" : "Mật khẩu *:", out passwordTextBox);
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.Leave += PasswordTextBox_Leave;
            accountLayout.Controls.Add(passwordPanel, 0, 0);

            // Confirm Password
            var confirmPasswordPanel = CreateInputPanel("Xác nhận mật khẩu:", out confirmPasswordTextBox);
            confirmPasswordTextBox.UseSystemPasswordChar = true;
            confirmPasswordTextBox.Leave += ConfirmPasswordTextBox_Leave;
            accountLayout.Controls.Add(confirmPasswordPanel, 1, 0);

            // Generate Password Button
            generatePasswordButton = new Button
            {
                Text = "🔑 Tạo mật khẩu ngẫu nhiên",
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5, 15, 5, 5),
                FlatAppearance = { BorderSize = 0 }
            };
            generatePasswordButton.Click += GeneratePasswordButton_Click;

            // Show Password Checkbox
            showPasswordCheckBox = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(5, 20, 5, 5),
                AutoSize = true
            };
            showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            buttonPanel.Controls.Add(generatePasswordButton);
            buttonPanel.Controls.Add(showPasswordCheckBox);

            accountLayout.Controls.Add(buttonPanel, 0, 2);
            accountLayout.SetColumnSpan(buttonPanel, 2);

            accountTab.Controls.Add(accountLayout);
        }

        private void SetupRolesTab()
        {
            rolesTab = new TabPage
            {
                Text = "Phân quyền",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var rolesLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            rolesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            rolesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rolesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // Title
            var rolesTitle = new Label
            {
                Text = "Chọn quyền cho người dùng:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Roles CheckedListBox
            rolesCheckedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                CheckOnClick = true,
                Enabled = !isReadOnly,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 10, 0, 10)
            };
            rolesCheckedListBox.ItemCheck += RolesCheckedListBox_ItemCheck;

            // Selected roles summary
            selectedRolesLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Chưa chọn quyền nào"
            };

            rolesLayout.Controls.Add(rolesTitle, 0, 0);
            rolesLayout.Controls.Add(rolesCheckedListBox, 0, 1);
            rolesLayout.Controls.Add(selectedRolesLabel, 0, 2);

            rolesTab.Controls.Add(rolesLayout);
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
                string saveText = isEditMode ? "💾 Cập nhật" : "💾 Tạo người dùng";
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
                Width = 350,
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
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = labelText.Contains("*") ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
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

        #region Data Loading
        private void LoadDropdownData()
        {
            try
            {
                // Load available employees
                availableEmployees = userBLL.GetAvailableEmployees();
                employeeComboBox.Items.Clear();
                employeeComboBox.Items.Add("-- Không liên kết nhân viên --");

                foreach (var employee in availableEmployees)
                {
                    employeeComboBox.Items.Add(new EmployeeDropdownModel
                    {
                        EmployeeID = employee.EmployeeID,
                        EmployeeCode = employee.EmployeeCode,
                        FullName = employee.FullName,
                        DepartmentName = employee.Department?.DepartmentName ?? "",
                        PositionName = employee.Position?.PositionName ?? ""
                    });
                }

                // Load all roles
                allRoles = userBLL.GetAllRoles();
                rolesCheckedListBox.Items.Clear();

                foreach (var role in allRoles)
                {
                    rolesCheckedListBox.Items.Add(new RoleDropdownModel
                    {
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        Description = role.Description ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dropdown: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultValues()
        {
            if (!isEditMode)
            {
                usernameTextBox.Text = "";
                employeeComboBox.SelectedIndex = 0;
                isActiveCheckBox.Checked = true;
            }
        }

        private void LoadUserData()
        {
            if (user == null) return;

            try
            {
                usernameTextBox.Text = user.Username;
                fullNameTextBox.Text = user.FullName ?? "";
                emailTextBox.Text = user.Email ?? "";
                isActiveCheckBox.Checked = user.IsActive;

                // Set employee
                if (user.EmployeeID.HasValue)
                {
                    // Add current employee to dropdown if not in available list
                    var currentEmployee = user.Employee;
                    if (currentEmployee != null)
                    {
                        var existingItem = employeeComboBox.Items.Cast<object>()
                            .FirstOrDefault(item => item is EmployeeDropdownModel model &&
                                          model.EmployeeID == currentEmployee.EmployeeID);

                        if (existingItem == null)
                        {
                            var currentEmployeeModel = new EmployeeDropdownModel
                            {
                                EmployeeID = currentEmployee.EmployeeID,
                                EmployeeCode = currentEmployee.EmployeeCode,
                                FullName = currentEmployee.FullName,
                                DepartmentName = currentEmployee.Department?.DepartmentName ?? "",
                                PositionName = currentEmployee.Position?.PositionName ?? ""
                            };
                            employeeComboBox.Items.Add(currentEmployeeModel);
                        }

                        // Select the employee
                        for (int i = 0; i < employeeComboBox.Items.Count; i++)
                        {
                            if (employeeComboBox.Items[i] is EmployeeDropdownModel model &&
                                model.EmployeeID == currentEmployee.EmployeeID)
                            {
                                employeeComboBox.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    employeeComboBox.SelectedIndex = 0;
                }

                // Load user roles
                if (isEditMode)
                {
                    var userRoles = userBLL.GetUserRoles(user.UserID);
                    for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
                    {
                        if (rolesCheckedListBox.Items[i] is RoleDropdownModel roleModel)
                        {
                            bool isAssigned = userRoles.Any(r => r.RoleID == roleModel.RoleID);
                            rolesCheckedListBox.SetItemChecked(i, isAssigned);
                        }
                    }
                }

                UpdateSelectedRolesDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu người dùng: {ex.Message}", "Lỗi",
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

        private void UsernameTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly && !isEditMode)
                ValidateUsername();
        }

        private void FullNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
            {
                if (!isEditMode && string.IsNullOrWhiteSpace(usernameTextBox.Text) && !string.IsNullOrWhiteSpace(fullNameTextBox.Text))
                {
                    usernameTextBox.Text = userBLL.GenerateUsername(fullNameTextBox.Text);
                }
            }
        }

        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidateEmail();
        }

        private void PasswordTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidatePassword();
        }

        private void ConfirmPasswordTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidateConfirmPassword();
        }

        private void GeneratePasswordButton_Click(object sender, EventArgs e)
        {
            string generatedPassword = userBLL.GenerateRandomPassword(8);
            passwordTextBox.Text = generatedPassword;
            confirmPasswordTextBox.Text = generatedPassword;

            MessageBox.Show($"Mật khẩu đã được tạo: {generatedPassword}\nVui lòng lưu lại mật khẩu này!",
                "Mật khẩu được tạo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            passwordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
            confirmPasswordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        }

        private void RolesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Use BeginInvoke to ensure the checked state is updated before we read it
            this.BeginInvoke(new Action(() => UpdateSelectedRolesDisplay()));
        }

        private void UpdateSelectedRolesDisplay()
        {
            var selectedRoles = new List<string>();

            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                if (rolesCheckedListBox.GetItemChecked(i) && rolesCheckedListBox.Items[i] is RoleDropdownModel roleModel)
                {
                    selectedRoles.Add(roleModel.RoleName);
                }
            }

            if (selectedRoles.Count == 0)
            {
                selectedRolesLabel.Text = "Chưa chọn quyền nào";
                selectedRolesLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
            else
            {
                selectedRolesLabel.Text = $"Đã chọn {selectedRoles.Count} quyền: {string.Join(", ", selectedRoles)}";
                selectedRolesLabel.ForeColor = Color.FromArgb(76, 175, 80);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveUser();

                    if (isEditMode)
                    {
                        if (userBLL.UpdateUser(user))
                        {
                            // Update roles
                            var selectedRoleIds = GetSelectedRoleIds();
                            userBLL.UpdateUserRoles(user.UserID, selectedRoleIds);

                            MessageBox.Show("Cập nhật người dùng thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        int newUserId = userBLL.AddUser(user);
                        user.UserID = newUserId;

                        // Assign roles
                        var selectedRoleIds = GetSelectedRoleIds();
                        userBLL.UpdateUserRoles(newUserId, selectedRoleIds);

                        MessageBox.Show("Thêm người dùng mới thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu người dùng: {ex.Message}", "Lỗi",
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
                    LoadUserData();
                else
                    ResetForm();
            }
        }
        #endregion

        #region Validation
        private bool ValidateUsername()
        {
            errorProvider.SetError(usernameTextBox, "");

            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                errorProvider.SetError(usernameTextBox, "Tên đăng nhập không được để trống");
                return false;
            }

            if (usernameTextBox.Text.Length < 3)
            {
                errorProvider.SetError(usernameTextBox, "Tên đăng nhập phải có ít nhất 3 ký tự");
                return false;
            }

            if (usernameTextBox.Text.Length > 50)
            {
                errorProvider.SetError(usernameTextBox, "Tên đăng nhập không được vượt quá 50 ký tự");
                return false;
            }

            // Check for duplicates
            try
            {
                int excludeId = isEditMode ? user.UserID : 0;
                if (userBLL.IsUsernameExists(usernameTextBox.Text.Trim(), excludeId))
                {
                    errorProvider.SetError(usernameTextBox, "Tên đăng nhập này đã tồn tại");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra tên đăng nhập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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

                // Check for duplicates
                try
                {
                    int excludeId = isEditMode ? user.UserID : 0;
                    if (userBLL.IsEmailExists(email, excludeId))
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

        private bool ValidatePassword()
        {
            errorProvider.SetError(passwordTextBox, "");

            // For edit mode, password is optional
            if (isEditMode && string.IsNullOrWhiteSpace(passwordTextBox.Text))
                return true;

            if (!isEditMode && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                errorProvider.SetError(passwordTextBox, "Mật khẩu không được để trống");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                if (passwordTextBox.Text.Length < 6)
                {
                    errorProvider.SetError(passwordTextBox, "Mật khẩu phải có ít nhất 6 ký tự");
                    return false;
                }

                if (passwordTextBox.Text.Length > 50)
                {
                    errorProvider.SetError(passwordTextBox, "Mật khẩu không được vượt quá 50 ký tự");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateConfirmPassword()
        {
            errorProvider.SetError(confirmPasswordTextBox, "");

            // For edit mode, if password is empty, confirm password should also be empty
            if (isEditMode && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                if (!string.IsNullOrWhiteSpace(confirmPasswordTextBox.Text))
                {
                    errorProvider.SetError(confirmPasswordTextBox, "Vui lòng để trống hoặc nhập cùng với mật khẩu mới");
                    return false;
                }
                return true;
            }

            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                errorProvider.SetError(confirmPasswordTextBox, "Xác nhận mật khẩu không khớp");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (!isEditMode) // Only validate username when creating new user
                isValid &= ValidateUsername();

            isValid &= ValidateEmail();
            isValid &= ValidatePassword();
            isValid &= ValidateConfirmPassword();

            // Validate employee selection
            if (employeeComboBox.SelectedIndex > 0 && employeeComboBox.SelectedItem is EmployeeDropdownModel selectedEmployee)
            {
                try
                {
                    int excludeId = isEditMode ? user.UserID : 0;
                    if (userBLL.IsEmployeeAlreadyUser(selectedEmployee.EmployeeID, excludeId))
                    {
                        MessageBox.Show("Nhân viên này đã có tài khoản người dùng!", "Lỗi validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        isValid = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi kiểm tra nhân viên: {ex.Message}", "Lỗi",
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
        #endregion

        #region Data Operations
        private void ResetForm()
        {
            // Clear all text boxes
            usernameTextBox.Clear();
            fullNameTextBox.Clear();
            emailTextBox.Clear();
            passwordTextBox.Clear();
            confirmPasswordTextBox.Clear();

            // Reset combo boxes and checkboxes
            employeeComboBox.SelectedIndex = 0;
            isActiveCheckBox.Checked = true;
            showPasswordCheckBox.Checked = false;

            // Uncheck all roles
            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                rolesCheckedListBox.SetItemChecked(i, false);
            }

            // Clear error provider
            errorProvider.Clear();

            // Set default values again
            SetDefaultValues();

            // Return to first tab
            tabControl.SelectedIndex = 0;

            UpdateSelectedRolesDisplay();
        }

        private void SaveUser()
        {
            try
            {
                user.Username = usernameTextBox.Text.Trim();
                user.FullName = string.IsNullOrWhiteSpace(fullNameTextBox.Text) ? null : fullNameTextBox.Text.Trim();
                user.Email = string.IsNullOrWhiteSpace(emailTextBox.Text) ? null : emailTextBox.Text.Trim();
                user.IsActive = isActiveCheckBox.Checked;

                // Set employee
                if (employeeComboBox.SelectedIndex > 0 && employeeComboBox.SelectedItem is EmployeeDropdownModel selectedEmployee)
                {
                    user.EmployeeID = selectedEmployee.EmployeeID;
                }
                else
                {
                    user.EmployeeID = null;
                }

                // Set password only if provided
                if (!string.IsNullOrWhiteSpace(passwordTextBox.Text))
                {
                    user.Password = passwordTextBox.Text;
                }

                if (!isEditMode)
                {
                    user.CreatedAt = DateTime.Now;
                }
                user.UpdatedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin người dùng: {ex.Message}", ex);
            }
        }

        private List<int> GetSelectedRoleIds()
        {
            var selectedRoleIds = new List<int>();

            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                if (rolesCheckedListBox.GetItemChecked(i) && rolesCheckedListBox.Items[i] is RoleDropdownModel roleModel)
                {
                    selectedRoleIds.Add(roleModel.RoleID);
                }
            }

            return selectedRoleIds;
        }
        #endregion

        #region Form Events
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None && !isReadOnly)
            {
                var result = MessageBox.Show(
                    "Bạn có muốn thoát mà không lưu thay đổi?",
                    "Xác nhận thoát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                ShowHelp();
            }
            else if (e.Control && e.KeyCode == Keys.S && !isReadOnly)
            {
                SaveButton_Click(null, null);
            }

            base.OnKeyDown(e);
        }

        private void ShowHelp()
        {
            string helpText = @"HƯỚNG DẪN SỬ DỤNG:

📋 THÔNG TIN CƠ BẢN:
• Tên đăng nhập: Bắt buộc, tối thiểu 3 ký tự, không được trùng
• Họ tên: Tùy chọn, sẽ tự động tạo username nếu để trống
• Email: Tùy chọn, phải đúng định dạng và không được trùng
• Nhân viên liên kết: Tùy chọn, mỗi nhân viên chỉ có 1 tài khoản

🔐 THÔNG TIN TÀI KHOẢN:
• Mật khẩu: Bắt buộc khi tạo mới, tối thiểu 6 ký tự
• Xác nhận mật khẩu: Phải giống với mật khẩu
• Có thể tạo mật khẩu ngẫu nhiên tự động

👑 PHÂN QUYỀN:
• Chọn các quyền phù hợp với vai trò người dùng
• Có thể không chọn quyền nào (tài khoản sẽ bị hạn chế)

⌨️ PHÍM TẮT:
• Ctrl+S: Lưu thông tin
• Esc: Thoát form
• F1: Hiển thị trợ giúp này";

            MessageBox.Show(helpText, "Trợ giúp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}