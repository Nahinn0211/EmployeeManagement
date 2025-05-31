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
    public partial class UserRoleManagementForm : Form
    {
        #region Fields
        private UserBLL userBLL;
        private User user;
        private List<Role> allRoles;
        private List<Role> currentUserRoles;

        // Controls
        private Label titleLabel;
        private Label userInfoLabel;
        private CheckedListBox rolesCheckedListBox;
        private Label selectedRolesLabel;
        private Button selectAllButton;
        private Button deselectAllButton;
        private Button saveButton;
        private Button cancelButton;
        #endregion

        #region Constructor
        public UserRoleManagementForm(User userToManage)
        {
            InitializeComponent();
            userBLL = new UserBLL();
            user = userToManage ?? throw new ArgumentNullException(nameof(userToManage));
            SetupForm();
            LoadData();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Quản lý quyền người dùng";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            SetupControls();
        }

        private void SetupControls()
        {
            // Title
            titleLabel = new Label
            {
                Text = "👑 QUẢN LÝ QUYỀN NGƯỜI DÙNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20),
                Size = new Size(500, 30)
            };

            // User info
            userInfoLabel = new Label
            {
                Text = $"Người dùng: {user.Username} ({user.FullName ?? "Chưa có tên"})",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 60),
                Size = new Size(500, 25)
            };

            // Roles label
            var rolesLabel = new Label
            {
                Text = "Chọn quyền cho người dùng:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 100),
                Size = new Size(200, 25)
            };

            // Control buttons
            selectAllButton = new Button
            {
                Text = "Chọn tất cả",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(350, 95),
                Size = new Size(100, 30),
                FlatAppearance = { BorderSize = 0 }
            };
            selectAllButton.Click += SelectAllButton_Click;

            deselectAllButton = new Button
            {
                Text = "Bỏ chọn tất cả",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(460, 95),
                Size = new Size(100, 30),
                FlatAppearance = { BorderSize = 0 }
            };
            deselectAllButton.Click += DeselectAllButton_Click;

            // Roles CheckedListBox
            rolesCheckedListBox = new CheckedListBox
            {
                Location = new Point(30, 135),
                Size = new Size(530, 200),
                Font = new Font("Segoe UI", 10),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            rolesCheckedListBox.ItemCheck += RolesCheckedListBox_ItemCheck;

            // Selected roles summary
            selectedRolesLabel = new Label
            {
                Location = new Point(30, 350),
                Size = new Size(530, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                Text = "Chưa chọn quyền nào"
            };

            // Save button
            saveButton = new Button
            {
                Text = "💾 Lưu thay đổi",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(350, 410),
                Size = new Size(150, 40),
                FlatAppearance = { BorderSize = 0 }
            };
            saveButton.Click += SaveButton_Click;

            // Cancel button
            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(510, 410),
                Size = new Size(100, 40),
                FlatAppearance = { BorderSize = 0 }
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                titleLabel, userInfoLabel, rolesLabel,
                selectAllButton, deselectAllButton,
                rolesCheckedListBox, selectedRolesLabel,
                saveButton, cancelButton
            });
        }
        #endregion

        #region Data Loading
        private void LoadData()
        {
            try
            {
                // Load all roles
                allRoles = userBLL.GetAllRoles();

                // Load current user roles
                currentUserRoles = userBLL.GetUserRoles(user.UserID);

                // Populate CheckedListBox
                rolesCheckedListBox.Items.Clear();
                foreach (var role in allRoles)
                {
                    var roleModel = new RoleDropdownModel
                    {
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        Description = role.Description ?? ""
                    };

                    int index = rolesCheckedListBox.Items.Add(roleModel);

                    // Check if user currently has this role
                    bool isAssigned = currentUserRoles.Any(r => r.RoleID == role.RoleID);
                    rolesCheckedListBox.SetItemChecked(index, isAssigned);
                }

                UpdateSelectedRolesDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                rolesCheckedListBox.SetItemChecked(i, true);
            }
            UpdateSelectedRolesDisplay();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                rolesCheckedListBox.SetItemChecked(i, false);
            }
            UpdateSelectedRolesDisplay();
        }

        private void RolesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Use BeginInvoke to ensure the checked state is updated before we read it
            this.BeginInvoke(new Action(() => UpdateSelectedRolesDisplay()));
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRoleIds = GetSelectedRoleIds();

                if (userBLL.UpdateUserRoles(user.UserID, selectedRoleIds))
                {
                    var selectedRoleNames = GetSelectedRoleNames();
                    string message = selectedRoleNames.Count == 0
                        ? "Đã xóa tất cả quyền của người dùng!"
                        : $"Cập nhật quyền thành công!\nQuyền được gán: {string.Join(", ", selectedRoleNames)}";

                    MessageBox.Show(message, "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật quyền. Vui lòng thử lại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật quyền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateSelectedRolesDisplay()
        {
            var selectedRoles = GetSelectedRoleNames();

            if (selectedRoles.Count == 0)
            {
                selectedRolesLabel.Text = "❌ Chưa chọn quyền nào - Người dùng sẽ không có quyền truy cập nào";
                selectedRolesLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
            else
            {
                selectedRolesLabel.Text = $"✅ Đã chọn {selectedRoles.Count} quyền: {string.Join(", ", selectedRoles)}";
                selectedRolesLabel.ForeColor = Color.FromArgb(76, 175, 80);
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

        private List<string> GetSelectedRoleNames()
        {
            var selectedRoleNames = new List<string>();

            for (int i = 0; i < rolesCheckedListBox.Items.Count; i++)
            {
                if (rolesCheckedListBox.GetItemChecked(i) && rolesCheckedListBox.Items[i] is RoleDropdownModel roleModel)
                {
                    selectedRoleNames.Add(roleModel.RoleName);
                }
            }

            return selectedRoleNames;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                // Check if there are unsaved changes
                var currentSelection = GetSelectedRoleIds();
                var originalSelection = currentUserRoles.Select(r => r.RoleID).ToList();

                if (!AreListsEqual(currentSelection, originalSelection))
                {
                    var result = MessageBox.Show(
                        "Bạn có thay đổi chưa được lưu. Bạn có muốn thoát mà không lưu?",
                        "Xác nhận thoát",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            base.OnFormClosing(e);
        }

        private bool AreListsEqual(List<int> list1, List<int> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            var sortedList1 = list1.OrderBy(x => x).ToList();
            var sortedList2 = list2.OrderBy(x => x).ToList();

            for (int i = 0; i < sortedList1.Count; i++)
            {
                if (sortedList1[i] != sortedList2[i])
                    return false;
            }

            return true;
        }
        #endregion
    }
}