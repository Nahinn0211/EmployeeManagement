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
    public partial class RoleUserManagementForm : Form
    {
        #region Fields
        private PermissionBLL permissionBLL;
        private Role role;
        private List<User> usersWithRole;
        private List<User> usersWithoutRole;

        // Controls
        private Label titleLabel;
        private Label roleInfoLabel;
        private TabControl tabControl;
        private TabPage assignedUsersTab;
        private TabPage availableUsersTab;

        // Assigned users tab
        private DataGridView assignedUsersGrid;
        private Button removeSelectedButton;
        private Button removeAllButton;
        private Label assignedCountLabel;

        // Available users tab
        private DataGridView availableUsersGrid;
        private Button assignSelectedButton;
        private Button assignAllButton;
        private Label availableCountLabel;
        private TextBox searchTextBox;

        // Footer
        private Button saveButton;
        private Button cancelButton;
        private Label summaryLabel;
        #endregion

        #region Constructor
        public RoleUserManagementForm(Role roleToManage)
        {
            InitializeComponent();
            permissionBLL = new PermissionBLL();
            role = roleToManage ?? throw new ArgumentNullException(nameof(roleToManage));
            SetupForm();
            LoadData();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Quản lý người dùng theo quyền";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            SetupControls();
        }

        private void SetupControls()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));   // Role info
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Tabs
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Footer

            SetupHeader(mainLayout);
            SetupRoleInfo(mainLayout);
            SetupTabs(mainLayout);
            SetupFooter(mainLayout);

            this.Controls.Add(mainLayout);
        }

        private void SetupHeader(TableLayoutPanel parent)
        {
            titleLabel = new Label
            {
                Text = "👥 QUẢN LÝ NGƯỜI DÙNG THEO QUYỀN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            parent.Controls.Add(titleLabel, 0, 0);
        }

        private void SetupRoleInfo(TableLayoutPanel parent)
        {
            var rolePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            roleInfoLabel = new Label
            {
                Text = $"🔑 Quyền: {role.RoleName} - {role.Description ?? "Không có mô tả"}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            rolePanel.Controls.Add(roleInfoLabel);
            parent.Controls.Add(rolePanel, 0, 1);
        }

        private void SetupTabs(TableLayoutPanel parent)
        {
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                ItemSize = new Size(200, 40),
                SizeMode = TabSizeMode.Fixed
            };

            SetupAssignedUsersTab();
            SetupAvailableUsersTab();

            tabControl.TabPages.Add(assignedUsersTab);
            tabControl.TabPages.Add(availableUsersTab);

            parent.Controls.Add(tabControl, 0, 2);
        }

        private void SetupAssignedUsersTab()
        {
            assignedUsersTab = new TabPage
            {
                Text = "Người dùng đã có quyền",
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var assignedLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            assignedLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Controls
            assignedLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            assignedLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Count

            // Controls panel
            var assignedControlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent
            };

            removeSelectedButton = new Button
            {
                Text = "➖ Xóa đã chọn",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 },
                Enabled = false
            };
            removeSelectedButton.Click += RemoveSelectedButton_Click;

            removeAllButton = new Button
            {
                Text = "🗑️ Xóa tất cả",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            removeAllButton.Click += RemoveAllButton_Click;

            assignedControlsPanel.Controls.Add(removeSelectedButton);
            assignedControlsPanel.Controls.Add(removeAllButton);

            // Grid
            assignedUsersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 },
                AutoGenerateColumns = false
            };

            SetupAssignedUsersGridColumns();
            assignedUsersGrid.SelectionChanged += AssignedUsersGrid_SelectionChanged;

            // Count label
            assignedCountLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "0 người dùng"
            };

            assignedLayout.Controls.Add(assignedControlsPanel, 0, 0);
            assignedLayout.Controls.Add(assignedUsersGrid, 0, 1);
            assignedLayout.Controls.Add(assignedCountLabel, 0, 2);

            assignedUsersTab.Controls.Add(assignedLayout);
        }

        private void SetupAvailableUsersTab()
        {
            availableUsersTab = new TabPage
            {
                Text = "Người dùng chưa có quyền",
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var availableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            availableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Search
            availableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Controls
            availableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            availableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));   // Count

            // Search panel
            var searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Text = "🔍 Tìm kiếm người dùng...",
                ForeColor = Color.Gray,
                Margin = new Padding(0, 5, 0, 5)
            };
            SetupSearchEvents();

            searchPanel.Controls.Add(searchTextBox);

            // Controls panel
            var availableControlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent
            };

            assignSelectedButton = new Button
            {
                Text = "➕ Gán đã chọn",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 },
                Enabled = false
            };
            assignSelectedButton.Click += AssignSelectedButton_Click;

            assignAllButton = new Button
            {
                Text = "📦 Gán tất cả",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            assignAllButton.Click += AssignAllButton_Click;

            availableControlsPanel.Controls.Add(assignSelectedButton);
            availableControlsPanel.Controls.Add(assignAllButton);

            // Grid
            availableUsersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 },
                AutoGenerateColumns = false
            };

            SetupAvailableUsersGridColumns();
            availableUsersGrid.SelectionChanged += AvailableUsersGrid_SelectionChanged;

            // Count label
            availableCountLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "0 người dùng"
            };

            availableLayout.Controls.Add(searchPanel, 0, 0);
            availableLayout.Controls.Add(availableControlsPanel, 0, 1);
            availableLayout.Controls.Add(availableUsersGrid, 0, 2);
            availableLayout.Controls.Add(availableCountLabel, 0, 3);

            availableUsersTab.Controls.Add(availableLayout);
        }

        private void SetupFooter(TableLayoutPanel parent)
        {
            var footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var footerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            // Summary
            summaryLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Chưa có thay đổi"
            };

            // Buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = Color.Transparent
            };

            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Size = new Size(100, 45),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            saveButton = new Button
            {
                Text = "💾 Lưu thay đổi",
                Size = new Size(130, 45),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            saveButton.Click += SaveButton_Click;

            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(saveButton);

            footerLayout.Controls.Add(summaryLabel, 0, 0);
            footerLayout.Controls.Add(buttonPanel, 1, 0);

            footerPanel.Controls.Add(footerLayout);
            parent.Controls.Add(footerPanel, 0, 3);
        }
        #endregion

        #region Grid Setup
        private void SetupAssignedUsersGridColumns()
        {
            assignedUsersGrid.Columns.Clear();

            var columns = new[]
            {
                new { Name = "Username", HeaderText = "Tên đăng nhập", Width = 120 },
                new { Name = "FullName", HeaderText = "Họ tên", Width = 150 },
                new { Name = "Email", HeaderText = "Email", Width = 180 },
                new { Name = "EmployeeName", HeaderText = "Nhân viên", Width = 150 },
                new { Name = "DepartmentName", HeaderText = "Phòng ban", Width = 120 },
                new { Name = "IsActiveDisplay", HeaderText = "Trạng thái", Width = 100 }
            };

            foreach (var col in columns)
            {
                assignedUsersGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic
                });
            }

            SetupGridStyles(assignedUsersGrid);
        }

        private void SetupAvailableUsersGridColumns()
        {
            availableUsersGrid.Columns.Clear();

            var columns = new[]
            {
                new { Name = "Username", HeaderText = "Tên đăng nhập", Width = 120 },
                new { Name = "FullName", HeaderText = "Họ tên", Width = 150 },
                new { Name = "Email", HeaderText = "Email", Width = 180 },
                new { Name = "EmployeeName", HeaderText = "Nhân viên", Width = 150 },
                new { Name = "DepartmentName", HeaderText = "Phòng ban", Width = 120 },
                new { Name = "IsActiveDisplay", HeaderText = "Trạng thái", Width = 100 }
            };

            foreach (var col in columns)
            {
                availableUsersGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic
                });
            }

            SetupGridStyles(availableUsersGrid);
        }

        private void SetupGridStyles(DataGridView grid)
        {
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 5, 8, 5),
                Font = new Font("Segoe UI", 9)
            };

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(8, 8, 8, 8)
            };

            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }
        #endregion

        #region Data Loading
        private void LoadData()
        {
            try
            {
                usersWithRole = permissionBLL.GetUsersByRole(role.RoleID);
                usersWithoutRole = permissionBLL.GetUsersWithoutRole(role.RoleID);

                LoadAssignedUsersGrid();
                LoadAvailableUsersGrid();
                UpdateCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAssignedUsersGrid()
        {
            var displayModels = usersWithRole.Select(u => new UserRoleDisplayModel
            {
                UserID = u.UserID,
                Username = u.Username,
                FullName = u.FullName ?? "",
                Email = u.Email ?? "",
                EmployeeName = u.Employee?.FullName ?? "Chưa liên kết",
                DepartmentName = u.Employee?.Department?.DepartmentName ?? "",
                IsActive = u.IsActive,
                IsActiveDisplay = u.IsActive ? "✅ Hoạt động" : "❌ Vô hiệu"
            }).ToList();

            assignedUsersGrid.DataSource = displayModels;
        }

        private void LoadAvailableUsersGrid()
        {
            var filteredUsers = FilterAvailableUsers();
            var displayModels = filteredUsers.Select(u => new UserRoleDisplayModel
            {
                UserID = u.UserID,
                Username = u.Username,
                FullName = u.FullName ?? "",
                Email = u.Email ?? "",
                EmployeeName = u.Employee?.FullName ?? "Chưa liên kết",
                DepartmentName = u.Employee?.Department?.DepartmentName ?? "",
                IsActive = u.IsActive,
                IsActiveDisplay = u.IsActive ? "✅ Hoạt động" : "❌ Vô hiệu"
            }).ToList();

            availableUsersGrid.DataSource = displayModels;
        }

        private List<User> FilterAvailableUsers()
        {
            string searchText = searchTextBox.Text;
            if (searchText == "🔍 Tìm kiếm người dùng..." || string.IsNullOrWhiteSpace(searchText))
                return usersWithoutRole;

            searchText = searchText.ToLower();
            return usersWithoutRole.Where(u =>
                u.Username.ToLower().Contains(searchText) ||
                (u.FullName ?? "").ToLower().Contains(searchText) ||
                (u.Email ?? "").ToLower().Contains(searchText) ||
                (u.Employee?.FullName ?? "").ToLower().Contains(searchText)
            ).ToList();
        }

        private void UpdateCounts()
        {
            assignedCountLabel.Text = $"✅ {usersWithRole.Count} người dùng có quyền này";
            availableCountLabel.Text = $"⏳ {usersWithoutRole.Count} người dùng chưa có quyền";

            summaryLabel.Text = "💡 Chọn người dùng và sử dụng các nút để thay đổi phân quyền";
        }
        #endregion

        #region Event Handlers
        private void SetupSearchEvents()
        {
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == "🔍 Tìm kiếm người dùng...")
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = "🔍 Tìm kiếm người dùng...";
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) => LoadAvailableUsersGrid();
        }

        private void AssignedUsersGrid_SelectionChanged(object sender, EventArgs e)
        {
            removeSelectedButton.Enabled = assignedUsersGrid.SelectedRows.Count > 0;
        }

        private void AvailableUsersGrid_SelectionChanged(object sender, EventArgs e)
        {
            assignSelectedButton.Enabled = availableUsersGrid.SelectedRows.Count > 0;
        }

        private void AssignSelectedButton_Click(object sender, EventArgs e)
        {
            var selectedUsers = GetSelectedUsers(availableUsersGrid, usersWithoutRole);
            if (selectedUsers.Count == 0) return;

            foreach (var user in selectedUsers)
            {
                usersWithRole.Add(user);
                usersWithoutRole.Remove(user);
            }

            LoadAssignedUsersGrid();
            LoadAvailableUsersGrid();
            UpdateCounts();
        }

        private void AssignAllButton_Click(object sender, EventArgs e)
        {
            if (usersWithoutRole.Count == 0) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn gán quyền '{role.RoleName}' cho tất cả {usersWithoutRole.Count} người dùng?",
                "Xác nhận gán quyền",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                usersWithRole.AddRange(usersWithoutRole);
                usersWithoutRole.Clear();

                LoadAssignedUsersGrid();
                LoadAvailableUsersGrid();
                UpdateCounts();
            }
        }

        private void RemoveSelectedButton_Click(object sender, EventArgs e)
        {
            var selectedUsers = GetSelectedUsers(assignedUsersGrid, usersWithRole);
            if (selectedUsers.Count == 0) return;

            foreach (var user in selectedUsers)
            {
                usersWithoutRole.Add(user);
                usersWithRole.Remove(user);
            }

            LoadAssignedUsersGrid();
            LoadAvailableUsersGrid();
            UpdateCounts();
        }

        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            if (usersWithRole.Count == 0) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa quyền '{role.RoleName}' khỏi tất cả {usersWithRole.Count} người dùng?",
                "Xác nhận xóa quyền",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                usersWithoutRole.AddRange(usersWithRole);
                usersWithRole.Clear();

                LoadAssignedUsersGrid();
                LoadAvailableUsersGrid();
                UpdateCounts();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current users with this role from database
                var currentUsersWithRole = permissionBLL.GetUsersByRole(role.RoleID);

                // Find users to add (in usersWithRole but not in currentUsersWithRole)
                var usersToAdd = usersWithRole
                    .Where(u => !currentUsersWithRole.Any(c => c.UserID == u.UserID))
                    .Select(u => u.UserID)
                    .ToList();

                // Find users to remove (in currentUsersWithRole but not in usersWithRole)
                var usersToRemove = currentUsersWithRole
                    .Where(c => !usersWithRole.Any(u => u.UserID == c.UserID))
                    .Select(c => c.UserID)
                    .ToList();

                // Apply changes
                bool success = true;

                if (usersToAdd.Count > 0)
                {
                    success &= permissionBLL.AssignRoleToMultipleUsers(usersToAdd, role.RoleID);
                }

                if (usersToRemove.Count > 0)
                {
                    success &= permissionBLL.RemoveRoleFromMultipleUsers(usersToRemove, role.RoleID);
                }

                if (success)
                {
                    MessageBox.Show($"Cập nhật phân quyền thành công!\n\n" +
                                  $"• Đã gán quyền cho {usersToAdd.Count} người dùng\n" +
                                  $"• Đã xóa quyền của {usersToRemove.Count} người dùng",
                                  "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật phân quyền!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thay đổi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Helper Methods
        private List<User> GetSelectedUsers(DataGridView grid, List<User> sourceList)
        {
            var selectedUsers = new List<User>();

            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                if (row.DataBoundItem is UserRoleDisplayModel model)
                {
                    var user = sourceList.FirstOrDefault(u => u.UserID == model.UserID);
                    if (user != null)
                        selectedUsers.Add(user);
                }
            }

            return selectedUsers;
        }
        #endregion
    }
}