using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Admin
{
    public partial class PermissionForm : Form
    {
        #region Fields
        private PermissionBLL permissionBLL;
        private List<Role> roles;
        private List<Role> filteredRoles;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên quyền, mô tả...";

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private Panel statisticsPanel;

        // Search controls
        private TextBox searchTextBox;
        private ComboBox usageFilterComboBox;
        private Button searchButton;
        private Button clearButton;

        // Grid controls
        private DataGridView roleDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button manageUsersButton;
        private Button duplicateButton;
        private Button createDefaultButton;
        private Button exportButton;
        private Label statisticsLabel;
        #endregion

        #region Constructor
        public PermissionForm()
        {
            InitializeComponent();
            permissionBLL = new PermissionBLL();
            InitializeLayout();
            LoadRolesFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadRolesFromDatabase()
        {
            try
            {
                roles = permissionBLL.GetAllRoles();
                filteredRoles = new List<Role>(roles);
                LoadRolesToGrid();
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
        private void LoadRolesToGrid()
        {
            try
            {
                var displayModels = permissionBLL.GetRolesForDisplay()
                    .Where(r => filteredRoles.Any(fr => fr.RoleID == r.RoleID))
                    .ToList();

                roleDataGridView.DataSource = displayModels;
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
                string usageFilter = usageFilterComboBox.SelectedIndex.ToString();

                filteredRoles = roles.Where(r =>
                    (string.IsNullOrEmpty(searchText) ||
                     r.RoleName.ToLower().Contains(searchText) ||
                     (r.Description ?? "").ToLower().Contains(searchText)) &&
                    (usageFilterComboBox.SelectedIndex == 0 || FilterByUsage(r, usageFilter))
                ).ToList();

                LoadRolesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool FilterByUsage(Role role, string filter)
        {
            var userCount = permissionBLL.GetUsersByRole(role.RoleID).Count;

            return filter switch
            {
                "1" => userCount == 0,           // Chưa sử dụng
                "2" => userCount > 0 && userCount <= 5,    // Ít sử dụng
                "3" => userCount > 5 && userCount <= 20,   // Sử dụng bình thường
                "4" => userCount > 20,           // Sử dụng nhiều
                _ => true
            };
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchTextBox.Text = searchPlaceholder;
            searchTextBox.ForeColor = Color.Gray;
            usageFilterComboBox.SelectedIndex = 0;
            filteredRoles = new List<Role>(roles);
            LoadRolesToGrid();
        }

        private void UpdateStatistics()
        {
            try
            {
                var stats = permissionBLL.GetPermissionStatistics();
                var filtered = filteredRoles.Count;

                statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng quyền: {stats.TotalRoles} | 👥 Người dùng có quyền: {stats.UsersWithRoles}/{stats.TotalUsers} | ⭐ TB quyền/người: {stats.AverageRolesPerUser:F1}";

                // Update statistics panel
                UpdateStatisticsPanel(stats);
            }
            catch (Exception ex)
            {
                statisticsLabel.Text = "❌ Lỗi khi tải thống kê";
            }
        }

        private void UpdateStatisticsPanel(PermissionStatistics stats)
        {
            statisticsPanel.Controls.Clear();

            var statsLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };

            // Create stat cards
            var statCards = new[]
            {
                CreateStatCard("Tổng quyền", stats.TotalRoles.ToString(), "🔑", Color.FromArgb(33, 150, 243)),
                CreateStatCard("Có quyền", stats.UsersWithRoles.ToString(), "👥", Color.FromArgb(76, 175, 80)),
                CreateStatCard("Chưa có quyền", stats.UsersWithoutRoles.ToString(), "❌", Color.FromArgb(255, 152, 0)),
                CreateStatCard("TB quyền/người", stats.AverageRolesPerUser.ToString("F1"), "⭐", Color.FromArgb(156, 39, 176))
            };

            foreach (var card in statCards)
            {
                statsLayout.Controls.Add(card);
            }

            statisticsPanel.Controls.Add(statsLayout);
        }

        private Panel CreateStatCard(string title, string value, string icon, Color color)
        {
            var card = new Panel
            {
                Size = new Size(120, 60),
                BackColor = color,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 5),
                Size = new Size(30, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 5),
                Size = new Size(70, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(10, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.AddRange(new Control[] { iconLabel, valueLabel, titleLabel });
            return card;
        }
        #endregion

        #region Layout Initialization
        private void InitializeLayout()
        {
            // Form properties
            this.Text = "Quản lý Phân quyền";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Main layout
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Footer

            InitializeHeaderPanel();
            InitializeSearchPanel();
            InitializeGridPanel();
            InitializeFooterPanel();

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);

            this.Controls.Add(mainTableLayout);
        }

        private void InitializeHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(63, 81, 181),
                Padding = new Padding(15)
            };

            // Title
            titleLabel = new Label
            {
                Text = "🔑 QUẢN LÝ PHÂN QUYỀN HỆ THỐNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Statistics panel
            statisticsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            headerPanel.Controls.Add(statisticsPanel);
            headerPanel.Controls.Add(titleLabel);
        }

        private void InitializeSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10)
            };

            var searchLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1
            };

            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Search box
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Usage filter
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // Search button
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // Clear button
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); // Spacer

            // Search textbox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Margin = new Padding(0, 5, 10, 5)
            };
            searchTextBox.Enter += SearchTextBox_Enter;
            searchTextBox.Leave += SearchTextBox_Leave;
            searchTextBox.TextChanged += (s, e) => ApplyFilters();

            // Usage filter
            usageFilterComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 10, 5)
            };
            usageFilterComboBox.Items.AddRange(new string[]
            {
                "📊 Tất cả mức độ sử dụng",
                "⚪ Chưa sử dụng (0 người)",
                "🟡 Ít sử dụng (1-5 người)",
                "🟢 Sử dụng bình thường (6-20 người)",
                "🔴 Sử dụng nhiều (>20 người)"
            });
            usageFilterComboBox.SelectedIndex = 0;
            usageFilterComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search button
            searchButton = new Button
            {
                Text = "🔍 Tìm",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 5, 5)
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear button
            clearButton = new Button
            {
                Text = "🗑️ Xóa",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 5)
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += ClearFilters;

            searchLayout.Controls.Add(searchTextBox, 0, 0);
            searchLayout.Controls.Add(usageFilterComboBox, 1, 0);
            searchLayout.Controls.Add(searchButton, 2, 0);
            searchLayout.Controls.Add(clearButton, 3, 0);

            searchPanel.Controls.Add(searchLayout);
        }

        private void InitializeGridPanel()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            roleDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                GridColor = Color.FromArgb(230, 230, 230),
                Font = new Font("Segoe UI", 9),
                RowHeadersVisible = false,
                AllowUserToResizeRows = false
            };

            // Configure grid appearance
            roleDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(63, 81, 181);
            roleDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            roleDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            roleDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            roleDataGridView.ColumnHeadersHeight = 40;

            roleDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            roleDataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
            roleDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // Add columns
            AddDataGridViewColumns();

            // Events
            roleDataGridView.SelectionChanged += RoleDataGridView_SelectionChanged;
            roleDataGridView.CellDoubleClick += (s, e) => EditRole(s, e); 
            gridPanel.Controls.Add(roleDataGridView);
        }

        private void AddDataGridViewColumns()
        {
            // Clear existing columns first
            roleDataGridView.Columns.Clear();

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RoleID", // Thêm Name property
                DataPropertyName = "RoleID",
                HeaderText = "ID",
                Width = 60,
                Visible = false
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RoleName",
                DataPropertyName = "RoleName",
                HeaderText = "🔑 Tên quyền",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.FromArgb(63, 81, 181)
                }
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                DataPropertyName = "Description",
                HeaderText = "📝 Mô tả",
                Width = 350,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UserCount",
                DataPropertyName = "UserCount",
                HeaderText = "👥 Số người dùng",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                }
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UsageStatus",
                DataPropertyName = "UsageStatus",
                HeaderText = "📊 Trạng thái sử dụng",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedAtDisplay",
                DataPropertyName = "CreatedAtDisplay",
                HeaderText = "📅 Ngày tạo",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            roleDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CanDelete",
                DataPropertyName = "CanDelete",
                HeaderText = "Xóa được",
                Width = 80,
                Visible = false
            });
        }
        private void InitializeFooterPanel()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(15, 5, 15, 5)
            };

            var footerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Buttons
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Statistics

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            // Create buttons
            addButton = CreateFooterButton("➕ Thêm quyền", Color.FromArgb(76, 175, 80), AddRole);
            editButton = CreateFooterButton("✏️ Sửa", Color.FromArgb(33, 150, 243), EditRole);
            viewButton = CreateFooterButton("👁️ Xem", Color.FromArgb(96, 125, 139), ViewRole);
            deleteButton = CreateFooterButton("🗑️ Xóa", Color.FromArgb(244, 67, 54), DeleteRole);
            manageUsersButton = CreateFooterButton("👥 Quản lý người dùng", Color.FromArgb(156, 39, 176), ManageUsers);
            duplicateButton = CreateFooterButton("📋 Nhân bản", Color.FromArgb(255, 152, 0), DuplicateRole);
            createDefaultButton = CreateFooterButton("⚙️ Tạo quyền mặc định", Color.FromArgb(121, 85, 72), CreateDefaultRoles);

            buttonPanel.Controls.AddRange(new Control[]
            {
                addButton, editButton, viewButton, deleteButton,
                manageUsersButton, duplicateButton, createDefaultButton
            });

            // Statistics label
            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Text = "📊 Đang tải thống kê..."
            };

            footerLayout.Controls.Add(buttonPanel, 0, 0);
            footerLayout.Controls.Add(statisticsLabel, 1, 0);

            footerPanel.Controls.Add(footerLayout);

            // Initially disable edit/delete buttons
            UpdateButtonStates();
        }

        private Button CreateFooterButton(string text, Color backColor, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(150, 35),
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 5),
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += clickHandler;

            return button;
        }
        #endregion

        #region Event Handlers
        private void SearchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == searchPlaceholder)
            {
                searchTextBox.Text = "";
                searchTextBox.ForeColor = Color.Black;
            }
        }

        private void SearchTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = searchPlaceholder;
                searchTextBox.ForeColor = Color.Gray;
            }
        }

        private void RoleDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = roleDataGridView.SelectedRows.Count > 0;
            editButton.Enabled = hasSelection;
            viewButton.Enabled = hasSelection;
            deleteButton.Enabled = hasSelection;
            manageUsersButton.Enabled = hasSelection;
            duplicateButton.Enabled = hasSelection;
        }
        #endregion

        #region Button Event Handlers
        private void AddRole(object sender, EventArgs e)
        {
            try
            {
                // Create a simple input dialog for role creation
                using (var dialog = new RoleEditDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var role = new Role
                        {
                            RoleName = dialog.RoleName,
                            Description = dialog.Description,
                            CreatedAt = DateTime.Now
                        };

                        permissionBLL.AddRole(role);
                        LoadRolesFromDatabase();
                        MessageBox.Show("Thêm quyền mới thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditRole(object sender, EventArgs e)
        {
            if (roleDataGridView.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = roleDataGridView.SelectedRows[0];

                // Sử dụng column name thay vì DataPropertyName
                var roleIdCell = selectedRow.Cells["RoleID"];
                if (roleIdCell?.Value == null)
                {
                    MessageBox.Show("Không thể lấy thông tin quyền được chọn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int roleId = Convert.ToInt32(roleIdCell.Value);
                var role = permissionBLL.GetRoleById(roleId);

                if (role != null)
                {
                    using (var dialog = new RoleEditDialog(role))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            role.RoleName = dialog.RoleName;
                            role.Description = dialog.Description;

                            permissionBLL.UpdateRole(role);
                            LoadRolesFromDatabase();
                            MessageBox.Show("Cập nhật quyền thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewRole(object sender, EventArgs e)
        {
            if (roleDataGridView.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = roleDataGridView.SelectedRows[0];
                var roleIdCell = selectedRow.Cells["RoleID"];

                if (roleIdCell?.Value == null) return;

                int roleId = Convert.ToInt32(roleIdCell.Value);
                var role = permissionBLL.GetRoleById(roleId);

                if (role != null)
                {
                    using (var dialog = new RoleEditDialog(role, true)) // true = view only
                    {
                        dialog.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRole(object sender, EventArgs e)
        {
            if (roleDataGridView.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = roleDataGridView.SelectedRows[0];
                var roleIdCell = selectedRow.Cells["RoleID"];
                var roleNameCell = selectedRow.Cells["RoleName"];

                if (roleIdCell?.Value == null || roleNameCell?.Value == null)
                {
                    MessageBox.Show("Không thể lấy thông tin quyền được chọn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int roleId = Convert.ToInt32(roleIdCell.Value);
                string roleName = roleNameCell.Value.ToString();

                // Check if role can be deleted
                var canDelete = permissionBLL.CanDeleteRole(roleId);
                if (!canDelete.CanDelete)
                {
                    MessageBox.Show(canDelete.Reason, "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa quyền '{roleName}'?\n\nLưu ý: Thao tác này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = permissionBLL.DeleteRole(roleId);
                    if (success)
                    {
                        LoadRolesFromDatabase();
                        MessageBox.Show("Xóa quyền thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa quyền này!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ManageUsers(object sender, EventArgs e)
        {
            if (roleDataGridView.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = roleDataGridView.SelectedRows[0];
                var roleIdCell = selectedRow.Cells["RoleID"];
                var roleNameCell = selectedRow.Cells["RoleName"];

                if (roleIdCell?.Value == null || roleNameCell?.Value == null) return;

                int roleId = Convert.ToInt32(roleIdCell.Value);
                string roleName = roleNameCell.Value.ToString();

                using (var dialog = new RoleUserManagementDialog(roleId, roleName, permissionBLL))
                {
                    dialog.ShowDialog();
                    LoadRolesFromDatabase(); // Refresh to update user counts
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quản lý người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DuplicateRole(object sender, EventArgs e)
        {
            if (roleDataGridView.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = roleDataGridView.SelectedRows[0];
                var roleIdCell = selectedRow.Cells["RoleID"];

                if (roleIdCell?.Value == null) return;

                int roleId = Convert.ToInt32(roleIdCell.Value);
                var originalRole = permissionBLL.GetRoleById(roleId);

                if (originalRole != null)
                {
                    var duplicatedRole = new Role
                    {
                        RoleName = $"{originalRole.RoleName} (Copy)",
                        Description = $"Copy of {originalRole.Description}",
                        CreatedAt = DateTime.Now
                    };

                    using (var dialog = new RoleEditDialog(duplicatedRole))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            duplicatedRole.RoleName = dialog.RoleName;
                            duplicatedRole.Description = dialog.Description;
                            permissionBLL.AddRole(duplicatedRole);
                            LoadRolesFromDatabase();
                            MessageBox.Show("Nhân bản quyền thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nhân bản quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateDefaultRoles(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Tạo các quyền mặc định cho hệ thống?\n\n" +
                    "Bao gồm: Admin, Manager, Employee, HR, Finance, IT Support, Viewer\n\n" +
                    "Lưu ý: Chỉ tạo những quyền chưa tồn tại.",
                    "Tạo quyền mặc định",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int created = permissionBLL.CreateDefaultRoles();
                    LoadRolesFromDatabase();

                    if (created > 0)
                    {
                        MessageBox.Show($"Đã tạo {created} quyền mặc định thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Tất cả quyền mặc định đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo quyền mặc định: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }

    #region Helper Dialog Classes

    public partial class RoleEditDialog : Form
    {
        public string RoleName { get; private set; }
        public string Description { get; private set; }

        private TextBox roleNameTextBox;
        private TextBox descriptionTextBox;
        private Button okButton;
        private Button cancelButton;
        private bool isViewOnly;

        public RoleEditDialog(Role role = null, bool viewOnly = false)
        {
            isViewOnly = viewOnly;
            InitializeDialog();

            if (role != null)
            {
                roleNameTextBox.Text = role.RoleName;
                descriptionTextBox.Text = role.Description ?? "";
                RoleName = role.RoleName;
                Description = role.Description ?? "";
            }

            if (isViewOnly)
            {
                this.Text = "Xem thông tin quyền";
                roleNameTextBox.ReadOnly = true;
                descriptionTextBox.ReadOnly = true;
                okButton.Visible = false;
                cancelButton.Text = "Đóng";
            }
            else
            {
                this.Text = role == null ? "Thêm quyền mới" : "Sửa quyền";
            }
        }

        private void InitializeDialog()
        {
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(20)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            // Role Name
            var nameLabel = new Label
            {
                Text = "Tên quyền:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            roleNameTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 5, 0, 5)
            };

            // Description
            var descLabel = new Label
            {
                Text = "Mô tả:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };

            descriptionTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 5, 0, 5)
            };

            // Buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0, 10, 0, 0)
            };

            cancelButton = new Button
            {
                Text = "Hủy",
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 0, 0, 0),
                DialogResult = DialogResult.Cancel
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            okButton = new Button
            {
                Text = "Lưu",
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 0, 0, 0)
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += OkButton_Click;

            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(okButton);

            mainPanel.Controls.Add(nameLabel, 0, 0);
            mainPanel.Controls.Add(roleNameTextBox, 1, 0);
            mainPanel.Controls.Add(descLabel, 0, 1);
            mainPanel.Controls.Add(descriptionTextBox, 1, 1);
            mainPanel.Controls.Add(buttonPanel, 1, 3);

            this.Controls.Add(mainPanel);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(roleNameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên quyền!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roleNameTextBox.Focus();
                return;
            }

            RoleName = roleNameTextBox.Text.Trim();
            Description = descriptionTextBox.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public partial class RoleUserManagementDialog : Form
    {
        private int roleId;
        private string roleName;
        private PermissionBLL permissionBLL;

        private DataGridView assignedUsersGrid;
        private DataGridView availableUsersGrid;
        private Button assignButton;
        private Button removeButton;
        private Button assignAllButton;
        private Button removeAllButton;

        public RoleUserManagementDialog(int roleId, string roleName, PermissionBLL permissionBLL)
        {
            this.roleId = roleId;
            this.roleName = roleName;
            this.permissionBLL = permissionBLL;
            InitializeDialog();
            LoadData();
        }

        private void InitializeDialog()
        {
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = $"Quản lý người dùng - Quyền: {roleName}";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                Padding = new Padding(15)
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            // Headers
            var assignedLabel = new Label
            {
                Text = "👥 Người dùng đã có quyền",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(76, 175, 80)
            };

            var availableLabel = new Label
            {
                Text = "🔍 Người dùng chưa có quyền",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(33, 150, 243)
            };

            // Grids
            assignedUsersGrid = CreateUserGrid();
            availableUsersGrid = CreateUserGrid();

            // Button panel
            var buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6
            };

            assignButton = CreateActionButton("▶️ Gán quyền", Color.FromArgb(76, 175, 80));
            removeButton = CreateActionButton("◀️ Gỡ quyền", Color.FromArgb(244, 67, 54));
            assignAllButton = CreateActionButton("⏩ Gán tất cả", Color.FromArgb(33, 150, 243));
            removeAllButton = CreateActionButton("⏪ Gỡ tất cả", Color.FromArgb(255, 152, 0));

            assignButton.Click += AssignButton_Click;
            removeButton.Click += RemoveButton_Click;
            assignAllButton.Click += AssignAllButton_Click;
            removeAllButton.Click += RemoveAllButton_Click;

            buttonPanel.Controls.Add(assignButton, 0, 1);
            buttonPanel.Controls.Add(removeButton, 0, 2);
            buttonPanel.Controls.Add(assignAllButton, 0, 3);
            buttonPanel.Controls.Add(removeAllButton, 0, 4);

            // Close button
            var closeButton = new Button
            {
                Text = "Đóng",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(10)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            mainLayout.Controls.Add(assignedLabel, 0, 0);
            mainLayout.Controls.Add(availableLabel, 2, 0);
            mainLayout.Controls.Add(assignedUsersGrid, 0, 1);
            mainLayout.Controls.Add(buttonPanel, 1, 1);
            mainLayout.Controls.Add(availableUsersGrid, 2, 1);
            mainLayout.Controls.Add(closeButton, 1, 2);

            this.Controls.Add(mainLayout);
        }

        private DataGridView CreateUserGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9),
                RowHeadersVisible = false
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UserID",
                HeaderText = "ID",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Username",
                HeaderText = "Tên đăng nhập",
                Width = 120
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FullName",
                HeaderText = "Họ tên",
                Width = 150
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 180
            });

            return grid;
        }

        private Button CreateActionButton(string text, Color color)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                Height = 40
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void LoadData()
        {
            try
            {
                var assignedUsers = permissionBLL.GetUsersByRole(roleId);
                var availableUsers = permissionBLL.GetUsersWithoutRole(roleId);

                assignedUsersGrid.DataSource = assignedUsers.Select(u => new
                {
                    u.UserID,
                    u.Username,
                    u.FullName,
                    u.Email
                }).ToList();

                availableUsersGrid.DataSource = availableUsers.Select(u => new
                {
                    u.UserID,
                    u.Username,
                    u.FullName,
                    u.Email
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AssignButton_Click(object sender, EventArgs e)
        {
            if (availableUsersGrid.SelectedRows.Count == 0) return;

            try
            {
                var userIds = new List<int>();
                foreach (DataGridViewRow row in availableUsersGrid.SelectedRows)
                {
                    userIds.Add(Convert.ToInt32(row.Cells["UserID"].Value));
                }

                bool success = permissionBLL.AssignRoleToMultipleUsers(userIds, roleId);
                if (success)
                {
                    LoadData();
                    MessageBox.Show("Gán quyền thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gán quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (assignedUsersGrid.SelectedRows.Count == 0) return;

            try
            {
                var userIds = new List<int>();
                foreach (DataGridViewRow row in assignedUsersGrid.SelectedRows)
                {
                    userIds.Add(Convert.ToInt32(row.Cells["UserID"].Value));
                }

                bool success = permissionBLL.RemoveRoleFromMultipleUsers(userIds, roleId);
                if (success)
                {
                    LoadData();
                    MessageBox.Show("Gỡ quyền thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gỡ quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AssignAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                var availableUsers = permissionBLL.GetUsersWithoutRole(roleId);
                if (availableUsers.Count == 0)
                {
                    MessageBox.Show("Không có người dùng nào để gán quyền!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show($"Gán quyền '{roleName}' cho tất cả {availableUsers.Count} người dùng?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var userIds = availableUsers.Select(u => u.UserID).ToList();
                    bool success = permissionBLL.AssignRoleToMultipleUsers(userIds, roleId);
                    if (success)
                    {
                        LoadData();
                        MessageBox.Show("Gán quyền cho tất cả người dùng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gán quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                var assignedUsers = permissionBLL.GetUsersByRole(roleId);
                if (assignedUsers.Count == 0)
                {
                    MessageBox.Show("Không có người dùng nào để gỡ quyền!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show($"Gỡ quyền '{roleName}' khỏi tất cả {assignedUsers.Count} người dùng?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var userIds = assignedUsers.Select(u => u.UserID).ToList();
                    bool success = permissionBLL.RemoveRoleFromMultipleUsers(userIds, roleId);
                    if (success)
                    {
                        LoadData();
                        MessageBox.Show("Gỡ quyền khỏi tất cả người dùng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gỡ quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    #endregion
}