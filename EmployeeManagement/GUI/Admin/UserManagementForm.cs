using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Admin
{
    public partial class UserManagementForm : Form
    {
        #region Fields
        private UserBLL userBLL;
        private List<Models.Entity.User> users;
        private List<Models.Entity.User> filteredUsers;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên đăng nhập, email, họ tên...";

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;

        // Search controls
        private TextBox searchTextBox;
        private ComboBox statusComboBox;
        private ComboBox employeeFilterComboBox;
        private Button searchButton;
        private Button clearButton;

        // Grid controls
        private DataGridView userDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button activateButton;
        private Button deactivateButton;
        private Button resetPasswordButton;
        private Button manageRolesButton;
        private Label statisticsLabel;
        #endregion

        #region Constructor
        public UserManagementForm()
        {
            InitializeComponent();
            userBLL = new UserBLL();
            InitializeLayout();
            LoadUsersFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadUsersFromDatabase()
        {
            try
            {
                users = userBLL.GetAllUsers();
                filteredUsers = new List<Models.Entity.User>(users);
                LoadUsersToGrid();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Data Management
        private void LoadUsersToGrid()
        {
            try
            {
                var dataSource = filteredUsers.Select(u => new UserDisplayModel
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    Email = u.Email ?? "",
                    FullName = u.FullName ?? "",
                    EmployeeName = u.Employee?.FullName ?? "Chưa liên kết",
                    EmployeeCode = u.Employee?.EmployeeCode ?? "",
                    DepartmentName = u.Employee?.Department?.DepartmentName ?? "",
                    PositionName = u.Employee?.Position?.PositionName ?? "",
                    IsActive = u.IsActive,
                    IsActiveDisplay = UserConstants.GetStatusDisplay(u.IsActive),
                    LastLogin = u.LastLogin,
                    LastLoginDisplay = UserConstants.GetLastLoginDisplay(u.LastLogin),
                    CreatedAt = u.CreatedAt,
                    CreatedAtDisplay = u.CreatedAt.ToString("dd/MM/yyyy"),
                    HasEmployee = u.EmployeeID.HasValue
                }).ToList();

                userDataGridView.DataSource = dataSource;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = searchTextBox.Text == searchPlaceholder ? "" : searchTextBox.Text.ToLower();
                bool? statusFilter = null;
                bool? employeeFilter = null;

                if (statusComboBox.SelectedIndex == 1) statusFilter = true;
                else if (statusComboBox.SelectedIndex == 2) statusFilter = false;

                if (employeeFilterComboBox.SelectedIndex == 1) employeeFilter = true;
                else if (employeeFilterComboBox.SelectedIndex == 2) employeeFilter = false;

                filteredUsers = users.Where(u =>
                    (string.IsNullOrEmpty(searchText) ||
                     u.Username.ToLower().Contains(searchText) ||
                     (u.Email ?? "").ToLower().Contains(searchText) ||
                     (u.FullName ?? "").ToLower().Contains(searchText) ||
                     (u.Employee?.FullName ?? "").ToLower().Contains(searchText)) &&
                    (!statusFilter.HasValue || u.IsActive == statusFilter.Value) &&
                    (!employeeFilter.HasValue || u.EmployeeID.HasValue == employeeFilter.Value)
                ).ToList();

                LoadUsersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchTextBox.Text = searchPlaceholder;
            searchTextBox.ForeColor = Color.Gray;
            statusComboBox.SelectedIndex = 0;
            employeeFilterComboBox.SelectedIndex = 0;
            filteredUsers = new List<Models.Entity.User>(users);
            LoadUsersToGrid();
        }

        private void UpdateStatistics()
        {
            var stats = userBLL.GetUserStatistics();
            var filtered = filteredUsers.Count;

            statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng: {stats.TotalUsers} | ✅ Hoạt động: {stats.ActiveUsers} | ❌ Vô hiệu: {stats.InactiveUsers} | 👤 Có nhân viên: {stats.UsersWithEmployee}";
        }
        #endregion

        #region Helper Methods
        private Models.Entity.User GetSelectedUser()
        {
            if (userDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = userDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is UserDisplayModel displayModel)
                {
                    return users.FirstOrDefault(u => u.UserID == displayModel.UserID);
                }
            }
            return null;
        }

        private List<Models.Entity.User> GetSelectedUsers()
        {
            var selectedUsers = new List<Models.Entity.User>();
            foreach (DataGridViewRow row in userDataGridView.SelectedRows)
            {
                if (row.DataBoundItem is UserDisplayModel displayModel)
                {
                    var user = users.FirstOrDefault(u => u.UserID == displayModel.UserID);
                    if (user != null)
                        selectedUsers.Add(user);
                }
            }
            return selectedUsers;
        }
        #endregion

        #region Event Handlers
        private void UserDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = userDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "IsActiveDisplay" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status.Contains("Hoạt động") ?
                    Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "EmployeeName" && e.Value != null)
            {
                if (e.Value.ToString() == "Chưa liên kết")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 152, 0);
                    e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Italic);
                }
            }
        }

        private void AddUser()
        {
            try
            {
                using (var form = new UserDetailForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsersFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Thêm người dùng thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditUser()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            try
            {
                using (var form = new UserDetailForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsersFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật người dùng thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewUser()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            try
            {
                using (var form = new UserDetailForm(user, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            try
            {
                var canDelete = userBLL.CanDeleteUser(user.UserID);
                if (!canDelete.CanDelete)
                {
                    MessageBox.Show(canDelete.Reason, "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa người dùng '{user.Username}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (userBLL.DeleteUser(user.UserID))
                    {
                        LoadUsersFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Xóa người dùng thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActivateUser()
        {
            var selectedUsers = GetSelectedUsers();
            if (selectedUsers.Count == 0) return;

            try
            {
                string message = selectedUsers.Count == 1
                    ? $"Bạn có chắc chắn muốn kích hoạt tài khoản '{selectedUsers[0].Username}'?"
                    : $"Bạn có chắc chắn muốn kích hoạt {selectedUsers.Count} tài khoản đã chọn?";

                var result = MessageBox.Show(message, "Xác nhận kích hoạt",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int successCount = 0;
                    foreach (var user in selectedUsers)
                    {
                        if (userBLL.ActivateUser(user.UserID))
                            successCount++;
                    }

                    LoadUsersFromDatabase();
                    MaterialSnackBar snackBar = new MaterialSnackBar($"Kích hoạt thành công {successCount} tài khoản!", "OK", true);
                    snackBar.Show(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kích hoạt tài khoản: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeactivateUser()
        {
            var selectedUsers = GetSelectedUsers();
            if (selectedUsers.Count == 0) return;

            try
            {
                string message = selectedUsers.Count == 1
                    ? $"Bạn có chắc chắn muốn vô hiệu hóa tài khoản '{selectedUsers[0].Username}'?"
                    : $"Bạn có chắc chắn muốn vô hiệu hóa {selectedUsers.Count} tài khoản đã chọn?";

                var result = MessageBox.Show(message, "Xác nhận vô hiệu hóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int successCount = 0;
                    foreach (var user in selectedUsers)
                    {
                        if (userBLL.DeactivateUser(user.UserID))
                            successCount++;
                    }

                    LoadUsersFromDatabase();
                    MaterialSnackBar snackBar = new MaterialSnackBar($"Vô hiệu hóa thành công {successCount} tài khoản!", "OK", true);
                    snackBar.Show(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi vô hiệu hóa tài khoản: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetPassword()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            try
            {
                using (var form = new PasswordResetForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MaterialSnackBar snackBar = new MaterialSnackBar("Reset mật khẩu thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset mật khẩu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ManageRoles()
        {
            var user = GetSelectedUser();
            if (user == null) return;

            try
            {
                using (var form = new UserRoleManagementForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật quyền thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quản lý quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Người dùng";
            this.BackColor = Color.White;
            this.Size = new Size(1600, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupSearchPanel();
            SetupDataGrid();
            SetupFooter();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            };

            titleLabel = new Label
            {
                Text = "👥 QUẢN LÝ NGƯỜI DÙNG",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 10, 20, 10)
            };

            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Employee filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Clear button

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 35,
                Margin = new Padding(0, 5, 10, 5)
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "✅ Hoạt động", "❌ Vô hiệu" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Employee Filter ComboBox
            employeeFilterComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            employeeFilterComboBox.Items.AddRange(new[] { "Tất cả loại", "👤 Có nhân viên", "🔧 Không có nhân viên" });
            employeeFilterComboBox.SelectedIndex = 0;
            employeeFilterComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(employeeFilterComboBox, 2, 0);
            searchContainer.Controls.Add(searchButton, 3, 0);
            searchContainer.Controls.Add(clearButton, 4, 0);

            searchPanel.Controls.Add(searchContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
        }

        private void SetupDataGrid()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            userDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                AllowUserToResizeColumns = true,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 40 },
                ScrollBars = ScrollBars.Both,
                AutoGenerateColumns = false
            };

            SetupDataGridStyles();
            SetupDataGridColumns();
            SetupDataGridEvents();

            gridPanel.Controls.Add(userDataGridView);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var footerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            activateButton = CreateActionButton("✅ KÍCH HOẠT", Color.FromArgb(76, 175, 80));
            deactivateButton = CreateActionButton("❌ VÔ HIỆU", Color.FromArgb(244, 67, 54));
            resetPasswordButton = CreateActionButton("🔑 RESET MK", Color.FromArgb(156, 39, 176));
            manageRolesButton = CreateActionButton("👑 QUYỀN", Color.FromArgb(0, 150, 136));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;
            resetPasswordButton.Enabled = false;
            manageRolesButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(activateButton);
            buttonsPanel.Controls.Add(deactivateButton);
            buttonsPanel.Controls.Add(resetPasswordButton);
            buttonsPanel.Controls.Add(manageRolesButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "📊 Đang tải..."
            };

            statsPanel.Controls.Add(statisticsLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 5),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(110, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void SetupSearchTextBoxEvents()
        {
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) =>
            {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };
        }

        private void SetupDataGridStyles()
        {
            userDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            userDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(10, 10, 10, 10),
                WrapMode = DataGridViewTriState.False
            };

            userDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            userDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "UserID", HeaderText = "ID", Width = 70, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "Username", HeaderText = "Tên đăng nhập", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "FullName", HeaderText = "Họ tên", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Email", HeaderText = "Email", Width = 180, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "EmployeeName", HeaderText = "Nhân viên", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "DepartmentName", HeaderText = "Phòng ban", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "IsActiveDisplay", HeaderText = "Trạng thái", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "LastLoginDisplay", HeaderText = "Đăng nhập cuối", Width = 140, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "CreatedAtDisplay", HeaderText = "Ngày tạo", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 80,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                userDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            userDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = userDataGridView.SelectedRows.Count > 0;
                bool singleSelection = userDataGridView.SelectedRows.Count == 1;

                editButton.Enabled = singleSelection;
                viewButton.Enabled = singleSelection;
                deleteButton.Enabled = singleSelection;
                resetPasswordButton.Enabled = singleSelection;
                manageRolesButton.Enabled = singleSelection;
                activateButton.Enabled = hasSelection;
                deactivateButton.Enabled = hasSelection;
            };

            userDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewUser();
            };

            userDataGridView.CellFormatting += UserDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddUser();
            editButton.Click += (s, e) => EditUser();
            viewButton.Click += (s, e) => ViewUser();
            deleteButton.Click += (s, e) => DeleteUser();
            activateButton.Click += (s, e) => ActivateUser();
            deactivateButton.Click += (s, e) => DeactivateUser();
            resetPasswordButton.Click += (s, e) => ResetPassword();
            manageRolesButton.Click += (s, e) => ManageRoles();
        }
        #endregion
    }
}