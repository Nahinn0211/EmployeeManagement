using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Admin
{
    public partial class RoleDetailForm : Form
    {
        #region Fields
        private PermissionBLL permissionBLL;
        private Role role;
        private bool isReadOnly;
        private bool isEditMode;
        private bool isDuplicate;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox roleIcon;

        // Content controls
        private Panel basicInfoPanel;
        private Panel templatePanel;
        private Panel previewPanel;

        // Basic info controls
        private TextBox roleNameTextBox;
        private TextBox descriptionTextBox;
        private CheckBox useTemplateCheckBox;
        private ComboBox templateComboBox;

        // Preview controls
        private Label previewLabel;
        private ListBox permissionsListBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        // Validation
        private ErrorProvider errorProvider;
        #endregion

        #region Constructors
        public RoleDetailForm()
        {
            InitializeComponent();
            permissionBLL = new PermissionBLL();
            role = new Role();
            isEditMode = false;
            isReadOnly = false;
            isDuplicate = false;
            SetupForm();
            SetDefaultValues();
        }

        public RoleDetailForm(Role existingRole, bool readOnly = false, bool duplicate = false)
        {
            InitializeComponent();
            permissionBLL = new PermissionBLL();
            role = existingRole ?? throw new ArgumentNullException(nameof(existingRole));
            isEditMode = !duplicate;
            isReadOnly = readOnly;
            isDuplicate = duplicate;
            SetupForm();
            LoadRoleData();
        }

        public Role UpdatedRole => role;
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            string titleText = isReadOnly ? "Xem chi tiết quyền" :
                              isDuplicate ? "Sao chép quyền" :
                              isEditMode ? "Chỉnh sửa quyền" : "Thêm quyền mới";

            this.Text = titleText;
            this.Size = new Size(700, 600);
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

            // Role icon
            roleIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateRoleIcon();

            // Title label
            string titleText = isReadOnly ? "👁️ CHI TIẾT QUYỀN" :
                              isDuplicate ? "📋 SAO CHÉP QUYỀN" :
                              isEditMode ? "✏️ CHỈNH SỬA QUYỀN" : "➕ THÊM QUYỀN MỚI";

            titleLabel = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(500, 40),
                AutoEllipsis = true
            };

            string subtitleText = isReadOnly ? "Xem thông tin chi tiết quyền" :
                                 isDuplicate ? "Tạo quyền mới từ quyền hiện tại" :
                                 isEditMode ? "Cập nhật thông tin quyền" : "Nhập thông tin để tạo quyền mới";

            var subtitleLabel = new Label
            {
                Text = subtitleText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(500, 25)
            };

            headerPanel.Controls.Add(roleIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateRoleIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string iconText = isReadOnly ? "👁️" :
                                     isDuplicate ? "📋" :
                                     isEditMode ? "✏️" : "🔑";
                    var size = g.MeasureString(iconText, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(iconText, font, brush, x, y);
                }
            }
            roleIcon.Image = bmp;
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
                RowCount = 1,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            SetupBasicInfoPanel();
            SetupPreviewPanel();

            contentLayout.Controls.Add(basicInfoPanel, 0, 0);
            contentLayout.Controls.Add(previewPanel, 1, 0);

            contentPanel.Controls.Add(contentLayout);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupBasicInfoPanel()
        {
            basicInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(5)
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Role name
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Description
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Use template checkbox
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Template selection
            basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Spacer

            // Role Name (Required)
            var roleNamePanel = CreateInputPanel("Tên quyền *:", out roleNameTextBox);
            roleNameTextBox.Leave += RoleNameTextBox_Leave;
            basicLayout.Controls.Add(roleNamePanel, 0, 0);

            // Description
            var descriptionPanel = CreateInputPanel("Mô tả:", out descriptionTextBox, true);
            descriptionTextBox.Leave += DescriptionTextBox_Leave;
            basicLayout.Controls.Add(descriptionPanel, 0, 1);

            // Use template checkbox (only for new roles)
            if (!isEditMode && !isReadOnly)
            {
                useTemplateCheckBox = new CheckBox
                {
                    Text = "Sử dụng mẫu có sẵn",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(5)
                };
                useTemplateCheckBox.CheckedChanged += UseTemplateCheckBox_CheckedChanged;
                basicLayout.Controls.Add(useTemplateCheckBox, 0, 2);

                // Template selection
                templatePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    Padding = new Padding(10),
                    Enabled = false
                };

                var templateLabel = new Label
                {
                    Text = "Chọn mẫu:",
                    Dock = DockStyle.Top,
                    Height = 25,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(64, 64, 64)
                };

                templateComboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Margin = new Padding(0, 5, 0, 0)
                };
                templateComboBox.SelectedIndexChanged += TemplateComboBox_SelectedIndexChanged;

                LoadTemplates();

                templatePanel.Controls.Add(templateComboBox);
                templatePanel.Controls.Add(templateLabel);
                basicLayout.Controls.Add(templatePanel, 0, 3);
            }

            basicInfoPanel.Controls.Add(basicLayout);
        }

        private void SetupPreviewPanel()
        {
            previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                Margin = new Padding(10, 0, 0, 0)
            };

            var previewLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            previewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            previewLabel = new Label
            {
                Text = "📋 Xem trước quyền:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            permissionsListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SelectionMode = SelectionMode.None
            };

            previewLayout.Controls.Add(previewLabel, 0, 0);
            previewLayout.Controls.Add(permissionsListBox, 0, 1);

            previewPanel.Controls.Add(previewLayout);
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
                string saveText = isDuplicate ? "📋 Sao chép" :
                                 isEditMode ? "💾 Cập nhật" : "💾 Tạo quyền";
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

            string tipText = isReadOnly ? "💡 Chế độ xem - Không thể chỉnh sửa" :
                            "💡 Tip: Các trường có dấu (*) là bắt buộc";
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
        private void LoadTemplates()
        {
            if (templateComboBox == null) return;

            templateComboBox.Items.Clear();
            templateComboBox.Items.Add("-- Chọn mẫu --");

            var defaultRoles = permissionBLL.GetDefaultRoles();
            foreach (var roleName in defaultRoles)
            {
                templateComboBox.Items.Add(roleName);
            }

            templateComboBox.SelectedIndex = 0;
        }

        private void SetDefaultValues()
        {
            if (!isEditMode && !isDuplicate)
            {
                roleNameTextBox.Text = "";
                descriptionTextBox.Text = "";
                UpdatePreview();
            }
        }

        private void LoadRoleData()
        {
            if (role == null) return;

            try
            {
                if (isDuplicate)
                {
                    roleNameTextBox.Text = $"{role.RoleName} - Copy";
                    descriptionTextBox.Text = $"Sao chép từ: {role.Description}";
                }
                else
                {
                    roleNameTextBox.Text = role.RoleName;
                    descriptionTextBox.Text = role.Description ?? "";
                }

                UpdatePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePreview()
        {
            permissionsListBox.Items.Clear();

            if (string.IsNullOrWhiteSpace(roleNameTextBox.Text))
            {
                permissionsListBox.Items.Add("Nhập tên quyền để xem trước...");
                return;
            }

            // Get template if selected
            RoleTemplate template = null;
            if (useTemplateCheckBox?.Checked == true && templateComboBox?.SelectedIndex > 0)
            {
                template = permissionBLL.GetRoleTemplate(templateComboBox.SelectedItem.ToString());
            }

            // Show role info
            permissionsListBox.Items.Add($"🔑 Tên quyền: {roleNameTextBox.Text}");

            if (!string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                permissionsListBox.Items.Add($"📝 Mô tả: {descriptionTextBox.Text}");

            permissionsListBox.Items.Add("");

            // Show permissions if template is selected
            if (template != null)
            {
                permissionsListBox.Items.Add("🛡️ Quyền được cấp:");
                foreach (var permission in template.Permissions)
                {
                    permissionsListBox.Items.Add($"  • {permission}");
                }
            }
            else
            {
                permissionsListBox.Items.Add("ℹ️ Chọn mẫu để xem quyền được cấp");
            }
        }
        #endregion

        #region Event Handlers
        private void UseTemplateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            templatePanel.Enabled = useTemplateCheckBox.Checked;
            UpdatePreview();
        }

        private void TemplateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (templateComboBox.SelectedIndex > 0)
            {
                var template = permissionBLL.GetRoleTemplate(templateComboBox.SelectedItem.ToString());
                if (template != null && string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                {
                    descriptionTextBox.Text = template.Description;
                }
            }
            UpdatePreview();
        }

        private void RoleNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
            {
                ValidateRoleName();
                UpdatePreview();
            }
        }

        private void DescriptionTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
            {
                ValidateDescription();
                UpdatePreview();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveRole();

                    if (isEditMode)
                    {
                        if (permissionBLL.UpdateRole(role))
                        {
                            MessageBox.Show("Cập nhật quyền thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        int newRoleId = permissionBLL.AddRole(role);
                        role.RoleID = newRoleId;
                        string message = isDuplicate ? "Sao chép quyền thành công!" : "Thêm quyền mới thành công!";
                        MessageBox.Show(message, "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu quyền: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả thông tin?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (isEditMode || isDuplicate)
                    LoadRoleData();
                else
                    ResetForm();
            }
        }
        #endregion

        #region Validation
        private bool ValidateRoleName()
        {
            errorProvider.SetError(roleNameTextBox, "");

            if (string.IsNullOrWhiteSpace(roleNameTextBox.Text))
            {
                errorProvider.SetError(roleNameTextBox, "Tên quyền không được để trống");
                return false;
            }

            if (roleNameTextBox.Text.Length < 2)
            {
                errorProvider.SetError(roleNameTextBox, "Tên quyền phải có ít nhất 2 ký tự");
                return false;
            }

            if (roleNameTextBox.Text.Length > 50)
            {
                errorProvider.SetError(roleNameTextBox, "Tên quyền không được vượt quá 50 ký tự");
                return false;
            }

            // Check for duplicates
            try
            {
                int excludeId = (isEditMode && !isDuplicate) ? role.RoleID : 0;
                if (permissionBLL.IsRoleNameExists(roleNameTextBox.Text.Trim(), excludeId))
                {
                    errorProvider.SetError(roleNameTextBox, "Tên quyền này đã tồn tại");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra tên quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateDescription()
        {
            errorProvider.SetError(descriptionTextBox, "");

            if (!string.IsNullOrEmpty(descriptionTextBox.Text) && descriptionTextBox.Text.Length > 255)
            {
                errorProvider.SetError(descriptionTextBox, "Mô tả không được vượt quá 255 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            isValid &= ValidateRoleName();
            isValid &= ValidateDescription();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }
        #endregion

        #region Data Operations
        private void ResetForm()
        {
            roleNameTextBox.Clear();
            descriptionTextBox.Clear();

            if (useTemplateCheckBox != null)
            {
                useTemplateCheckBox.Checked = false;
                templateComboBox.SelectedIndex = 0;
            }

            errorProvider.Clear();
            SetDefaultValues();
            UpdatePreview();
        }

        private void SaveRole()
        {
            try
            {
                role.RoleName = roleNameTextBox.Text.Trim();
                role.Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ?
                    null : descriptionTextBox.Text.Trim();

                if (!isEditMode || isDuplicate)
                {
                    role.CreatedAt = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin quyền: {ex.Message}", ex);
            }
        }
        #endregion
    }
}