using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Utilities
{
    /// <summary>
    /// Helper class để quản lý quyền truy cập menu theo role
    /// </summary>
    public static class MenuPermissionHelper
    {
        /// <summary>
        /// Dictionary chứa quyền truy cập menu theo role
        /// Key: RoleName, Value: List of allowed menu keys
        /// </summary>
        private static readonly Dictionary<string, List<string>> RolePermissions = new Dictionary<string, List<string>>
        {
            // ADMIN - Có toàn quyền truy cập tất cả chức năng
            ["Admin"] = new List<string>
            {
                "Dashboard",
                
                // Quản lý nhân sự
                "Employee", "Department", "Position",
                
                // Quản lý dự án
                "Project", "Task", "Customer",
                
                // Quản lý tài liệu
                "Document",
                
                // Chấm công & Lương
                "Attendance", "Salary",
                
                // Tài chính
                "Finance", "ProjectFinance",
                
                // Báo cáo
                "HRReport", "ProjectReport", "FinanceReport",
                
                // Quản trị hệ thống
                "UserManagement", "Permission"
            },

            // MANAGER - Quyền quản lý cấp trung, không có quyền quản trị hệ thống
            ["Manager"] = new List<string>
            {
                "Dashboard",
                
                // Quản lý nhân sự (hạn chế)
                "Employee", "Department", "Position",
                
                // Quản lý dự án
                "Project", "Task", "Customer",
                
                // Quản lý tài liệu
                "Document",
                
                // Chấm công & Lương
                "Attendance", "Salary",
                
                // Tài chính (chỉ xem)
                "Finance", "ProjectFinance",
                
                // Báo cáo
                "HRReport", "ProjectReport", "FinanceReport"
                
                // Không có UserManagement, Permission
            },

            // USER - Quyền cơ bản cho nhân viên thường
            ["User"] = new List<string>
            {
                "Dashboard",
                
                // Chỉ xem thông tin nhân sự
                "Employee",
                
                // Xem dự án được phân công
                "Project", "Task",
                
                // Xem tài liệu
                "Document",
                
                // Chấm công (chỉ của bản thân)
                "Attendance",
                
                // Xem lương (chỉ của bản thân)
                "Salary"
                
                // Không có quyền quản lý, báo cáo, tài chính
            },

            // HR - Chuyên về nhân sự
            ["HR"] = new List<string>
            {
                "Dashboard",
                
                // Toàn quyền nhân sự
                "Employee", "Department", "Position",
                
                // Chấm công & Lương
                "Attendance", "Salary",
                
                // Báo cáo nhân sự
                "HRReport",
                
                // Quản lý tài liệu nhân sự
                "Document"
                
                // Không có quyền dự án, tài chính, quản trị
            }
        };

        /// <summary>
        /// Kiểm tra xem user có quyền truy cập menu không
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <param name="menuKey">Key của menu cần kiểm tra</param>
        /// <returns>True nếu có quyền, False nếu không có quyền</returns>
        public static bool HasPermission(string roleName, string menuKey)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(menuKey))
                return false;

            // Admin luôn có quyền truy cập mọi thứ
            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            // Kiểm tra quyền theo role
            if (RolePermissions.ContainsKey(roleName))
            {
                return RolePermissions[roleName].Contains(menuKey);
            }

            // Mặc định không có quyền nếu role không được định nghĩa
            return false;
        }

        /// <summary>
        /// Lấy danh sách menu được phép truy cập theo role
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <returns>Danh sách các menu key được phép truy cập</returns>
        public static List<string> GetAllowedMenus(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return new List<string>();

            // Admin có quyền truy cập tất cả
            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RolePermissions["Admin"];

            // Trả về danh sách menu theo role
            if (RolePermissions.ContainsKey(roleName))
                return RolePermissions[roleName];

            // Mặc định chỉ có Dashboard nếu role không được định nghĩa
            return new List<string> { "Dashboard" };
        }

        /// <summary>
        /// Kiểm tra xem có phải Admin không
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <returns>True nếu là Admin</returns>
        public static bool IsAdmin(string roleName)
        {
            return roleName?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Kiểm tra xem có phải Manager không
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <returns>True nếu là Manager</returns>
        public static bool IsManager(string roleName)
        {
            return roleName?.Equals("Manager", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Kiểm tra xem có phải HR không
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <returns>True nếu là HR</returns>
        public static bool IsHR(string roleName)
        {
            return roleName?.Equals("HR", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Kiểm tra xem có phải User thường không
        /// </summary>
        /// <param name="roleName">Tên role của user</param>
        /// <returns>True nếu là User thường</returns>
        public static bool IsUser(string roleName)
        {
            return roleName?.Equals("User", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Lấy mô tả về quyền hạn của role
        /// </summary>
        /// <param name="roleName">Tên role</param>
        /// <returns>Mô tả quyền hạn</returns>
        public static string GetRoleDescription(string roleName)
        {
            return roleName switch
            {
                "Admin" => "Quản trị viên hệ thống - Toàn quyền truy cập",
                "Manager" => "Quản lý - Quyền quản lý cấp trung",
                "User" => "Người dùng thường - Quyền cơ bản",
                "HR" => "Nhân sự - Chuyên về quản lý nhân sự",
                _ => "Quyền hạn không xác định"
            };
        }

        /// <summary>
        /// Lấy màu sắc đại diện cho role (để hiển thị UI)
        /// </summary>
        /// <param name="roleName">Tên role</param>
        /// <returns>Mã màu hex</returns>
        public static string GetRoleColor(string roleName)
        {
            return roleName switch
            {
                "Admin" => "#F44336",      // Đỏ - Quyền cao nhất
                "Manager" => "#FF9800",    // Cam - Quyền quản lý
                "HR" => "#9C27B0",        // Tím - Chuyên về HR
                "User" => "#4CAF50",       // Xanh lá - User thường
                _ => "#9E9E9E"             // Xám - Không xác định
            };
        }

        /// <summary>
        /// Lấy icon đại diện cho role
        /// </summary>
        /// <param name="roleName">Tên role</param>
        /// <returns>Icon emoji</returns>
        public static string GetRoleIcon(string roleName)
        {
            return roleName switch
            {
                "Admin" => "👑",
                "Manager" => "👔",
                "HR" => "👥",
                "User" => "👤",
                _ => "❓"
            };
        }
    }
}