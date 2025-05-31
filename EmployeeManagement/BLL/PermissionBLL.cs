using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.BLL
{
    public class PermissionBLL
    {
        private PermissionDAL permissionDAL;

        public PermissionBLL()
        {
            permissionDAL = new PermissionDAL();
        }

        #region Role Management

        public List<Role> GetAllRoles()
        {
            try
            {
                return permissionDAL.GetAllRoles();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách quyền: {ex.Message}", ex);
            }
        }

        public Role GetRoleById(int roleId)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("ID quyền không hợp lệ");

                return permissionDAL.GetRoleById(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin quyền: {ex.Message}", ex);
            }
        }

        public int AddRole(Role role)
        {
            try
            {
                ValidateRole(role);

                role.CreatedAt = DateTime.Now;
                return permissionDAL.AddRole(role);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm quyền: {ex.Message}", ex);
            }
        }

        public bool UpdateRole(Role role)
        {
            try
            {
                ValidateRole(role, true);
                return permissionDAL.UpdateRole(role);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật quyền: {ex.Message}", ex);
            }
        }

        public RoleDeleteValidation CanDeleteRole(int roleId)
        {
            try
            {
                var role = permissionDAL.GetRoleById(roleId);
                if (role == null)
                {
                    return new RoleDeleteValidation
                    {
                        CanDelete = false,
                        Reason = "Quyền không tồn tại"
                    };
                }

                // Check if role is assigned to any users
                if (!permissionDAL.CanDeleteRole(roleId))
                {
                    var users = permissionDAL.GetUsersByRole(roleId);
                    return new RoleDeleteValidation
                    {
                        CanDelete = false,
                        Reason = $"Không thể xóa quyền này vì đang được gán cho {users.Count} người dùng",
                        AssignedUsers = users.Select(u => u.Username).ToList()
                    };
                }

                return new RoleDeleteValidation
                {
                    CanDelete = true,
                    Reason = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new RoleDeleteValidation
                {
                    CanDelete = false,
                    Reason = $"Lỗi khi kiểm tra: {ex.Message}"
                };
            }
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                var canDelete = CanDeleteRole(roleId);
                if (!canDelete.CanDelete)
                    throw new InvalidOperationException(canDelete.Reason);

                return permissionDAL.DeleteRole(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa quyền: {ex.Message}", ex);
            }
        }

        public bool ForceDeleteRole(int roleId)
        {
            try
            {
                // Force delete by removing all user assignments first
                return permissionDAL.DeleteRole(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa quyền: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation

        private void ValidateRole(Role role, bool isUpdate = false)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "Thông tin quyền không được để trống");

            if (string.IsNullOrWhiteSpace(role.RoleName))
                throw new ArgumentException("Tên quyền không được để trống");

            if (role.RoleName.Length < 2)
                throw new ArgumentException("Tên quyền phải có ít nhất 2 ký tự");

            if (role.RoleName.Length > 50)
                throw new ArgumentException("Tên quyền không được vượt quá 50 ký tự");

            if (!string.IsNullOrEmpty(role.Description) && role.Description.Length > 255)
                throw new ArgumentException("Mô tả không được vượt quá 255 ký tự");

            // Check for duplicates
            int excludeId = isUpdate ? role.RoleID : 0;
            if (IsRoleNameExists(role.RoleName, excludeId))
                throw new ArgumentException("Tên quyền này đã tồn tại");

            // Check for reserved role names
            var reservedNames = new[] { "system", "admin", "root", "superuser" };
            if (reservedNames.Contains(role.RoleName.ToLower()) && !isUpdate)
                throw new ArgumentException("Tên quyền này đã được hệ thống sử dụng");
        }

        public bool IsRoleNameExists(string roleName, int excludeRoleId = 0)
        {
            try
            {
                return permissionDAL.IsRoleNameExists(roleName, excludeRoleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tên quyền: {ex.Message}", ex);
            }
        }

        #endregion

        #region User-Role Management

        public List<User> GetUsersByRole(int roleId)
        {
            try
            {
                return permissionDAL.GetUsersByRole(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng theo quyền: {ex.Message}", ex);
            }
        }

        public List<User> GetUsersWithoutRole(int roleId)
        {
            try
            {
                return permissionDAL.GetUsersWithoutRole(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng chưa có quyền: {ex.Message}", ex);
            }
        }

        public bool AssignRoleToUser(int userId, int roleId)
        {
            try
            {
                if (userId <= 0 || roleId <= 0)
                    throw new ArgumentException("ID người dùng và ID quyền phải lớn hơn 0");

                return permissionDAL.AssignRoleToUser(userId, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gán quyền cho người dùng: {ex.Message}", ex);
            }
        }

        public bool RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                if (userId <= 0 || roleId <= 0)
                    throw new ArgumentException("ID người dùng và ID quyền phải lớn hơn 0");

                return permissionDAL.RemoveRoleFromUser(userId, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa quyền của người dùng: {ex.Message}", ex);
            }
        }

        public bool AssignRoleToMultipleUsers(List<int> userIds, int roleId)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                    throw new ArgumentException("Danh sách người dùng không được để trống");

                if (roleId <= 0)
                    throw new ArgumentException("ID quyền không hợp lệ");

                return permissionDAL.AssignRoleToMultipleUsers(userIds, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gán quyền cho nhiều người dùng: {ex.Message}", ex);
            }
        }

        public bool RemoveRoleFromMultipleUsers(List<int> userIds, int roleId)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                    throw new ArgumentException("Danh sách người dùng không được để trống");

                if (roleId <= 0)
                    throw new ArgumentException("ID quyền không hợp lệ");

                return permissionDAL.RemoveRoleFromMultipleUsers(userIds, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa quyền của nhiều người dùng: {ex.Message}", ex);
            }
        }

        #endregion

        #region Display Data

        public List<RoleDisplayModel> GetRolesForDisplay()
        {
            try
            {
                var roles = permissionDAL.GetAllRoles();
                var displayModels = new List<RoleDisplayModel>();

                foreach (var role in roles)
                {
                    var userCount = permissionDAL.GetUsersByRole(role.RoleID).Count;

                    displayModels.Add(new RoleDisplayModel
                    {
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        Description = role.Description ?? "",
                        UserCount = userCount,
                        UserCountDisplay = $"{userCount} người dùng",
                        CreatedAt = role.CreatedAt,
                        CreatedAtDisplay = role.CreatedAt.ToString("dd/MM/yyyy"),
                        CanDelete = userCount == 0,
                        UsageStatus = GetUsageStatus(userCount)
                    });
                }

                return displayModels;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hiển thị: {ex.Message}", ex);
            }
        }

        public List<UserRoleDisplayModel> GetUserRoleAssignments(int roleId)
        {
            try
            {
                var users = permissionDAL.GetUsersByRole(roleId);
                return users.Select(u => new UserRoleDisplayModel
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    FullName = u.FullName ?? "",
                    Email = u.Email ?? "",
                    EmployeeName = u.Employee?.FullName ?? "Chưa liên kết",
                    DepartmentName = u.Employee?.Department?.DepartmentName ?? "",
                    PositionName = u.Employee?.Position?.PositionName ?? "",
                    IsActive = u.IsActive,
                    IsActiveDisplay = u.IsActive ? "✅ Hoạt động" : "❌ Vô hiệu"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phân quyền: {ex.Message}", ex);
            }
        }

        private string GetUsageStatus(int userCount)
        {
            if (userCount == 0) return "🔴 Chưa sử dụng";
            if (userCount <= 5) return "🟡 Ít sử dụng";
            if (userCount <= 20) return "🟢 Sử dụng bình thường";
            return "🔵 Sử dụng nhiều";
        }

        #endregion

        #region Statistics

        public PermissionStatistics GetPermissionStatistics()
        {
            try
            {
                return permissionDAL.GetPermissionStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê phân quyền: {ex.Message}", ex);
            }
        }

        public List<RoleUsageStatistic> GetRoleUsageStatistics()
        {
            try
            {
                return permissionDAL.GetRoleUsageStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê sử dụng quyền: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        public List<string> GetDefaultRoles()
        {
            return new List<string>
            {
                "Admin",
                "Manager",
                "Employee",
                "HR",
                "Finance",
                "IT Support",
                "Viewer"
            };
        }

        public RoleTemplate GetRoleTemplate(string roleName)
        {
            var templates = new Dictionary<string, RoleTemplate>
            {
                ["Admin"] = new RoleTemplate
                {
                    Name = "Admin",
                    Description = "Quản trị viên hệ thống - Có toàn quyền truy cập",
                    Permissions = new List<string> { "Full Access", "User Management", "System Configuration" }
                },
                ["Manager"] = new RoleTemplate
                {
                    Name = "Manager",
                    Description = "Quản lý - Có quyền quản lý nhân viên và dự án",
                    Permissions = new List<string> { "Employee Management", "Project Management", "Reports" }
                },
                ["Employee"] = new RoleTemplate
                {
                    Name = "Employee",
                    Description = "Nhân viên - Quyền cơ bản để làm việc",
                    Permissions = new List<string> { "View Profile", "Update Profile", "View Projects" }
                },
                ["HR"] = new RoleTemplate
                {
                    Name = "HR",
                    Description = "Nhân sự - Quản lý thông tin nhân viên",
                    Permissions = new List<string> { "Employee Management", "Attendance", "Payroll" }
                },
                ["Finance"] = new RoleTemplate
                {
                    Name = "Finance",
                    Description = "Tài chính - Quản lý tài chính và kế toán",
                    Permissions = new List<string> { "Financial Reports", "Budget Management", "Expense Management" }
                },
                ["IT Support"] = new RoleTemplate
                {
                    Name = "IT Support",
                    Description = "Hỗ trợ IT - Quản lý hệ thống và hỗ trợ kỹ thuật",
                    Permissions = new List<string> { "System Maintenance", "User Support", "Backup Management" }
                },
                ["Viewer"] = new RoleTemplate
                {
                    Name = "Viewer",
                    Description = "Người xem - Chỉ có quyền xem thông tin",
                    Permissions = new List<string> { "View Only" }
                }
            };

            return templates.ContainsKey(roleName) ? templates[roleName] : null;
        }

      
        public int CreateDefaultRoles()
        {
            try
            {
                var defaultRoles = GetDefaultRoles();
                int successCount = 0;

                foreach (var roleName in defaultRoles)
                {
                    if (!IsRoleNameExists(roleName))
                    {
                        var template = GetRoleTemplate(roleName);
                        if (template != null)
                        {
                            var role = new Role
                            {
                                RoleName = template.Name,
                                Description = template.Description,
                                CreatedAt = DateTime.Now
                            };

                            AddRole(role);
                            successCount++;
                        }
                    }
                }

                return successCount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo quyền mặc định: {ex.Message}", ex);
            }
        }
        #endregion
    }

   
}