using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Utilities
{
    public static class PermissionManager
    {
        // Định nghĩa quyền cho từng role
        private static readonly Dictionary<string, List<string>> RolePermissions = new Dictionary<string, List<string>>
        {
            ["Admin"] = new List<string>
            {
                // Tất cả quyền
                "Dashboard", "Employee", "Department", "Position", "Project", "Task", "Customer",
                "Document", "Attendance", "Salary", "Finance", "ProjectFinance",
                "HRReport", "ProjectReport", "FinanceReport", "UserManagement", "Permission"
            },
            ["Manager"] = new List<string>
            {
                // Quản lý cơ bản + báo cáo
                "Dashboard", "Employee", "Department", "Position", "Project", "Task", "Customer",
                "Document", "Attendance", "Salary", "Finance", "ProjectFinance",
                "HRReport", "ProjectReport", "FinanceReport"
            },
            ["HR"] = new List<string>
            {
                // Chuyên về nhân sự
                "Dashboard", "Employee", "Department", "Position", "Attendance", "Salary",
                "HRReport", "Document"
            },
            ["Accountant"] = new List<string>
            {
                // Chuyên về tài chính
                "Dashboard", "Finance", "ProjectFinance", "FinanceReport", "Salary", "Document"
            },
            ["ProjectManager"] = new List<string>
            {
                // Chuyên về dự án
                "Dashboard", "Project", "Task", "Customer", "ProjectFinance", "ProjectReport", "Document"
            },
            ["Employee"] = new List<string>
            {
                // Chỉ xem thông tin cơ bản
                "Dashboard", "Document"
            },
            ["User"] = new List<string>
            {
                // Quyền mặc định
                "Dashboard"
            }
        };

        /// <summary>
        /// Kiểm tra quyền truy cập menu
        /// </summary>
        public static bool HasMenuPermission(string userRole, string menuKey)
        {
            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(menuKey))
                return false;

            // Admin có tất cả quyền
            if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            // Kiểm tra quyền trong dictionary
            if (RolePermissions.TryGetValue(userRole, out var permissions))
            {
                return permissions.Contains(menuKey, StringComparer.OrdinalIgnoreCase);
            }

            // Mặc định chỉ cho phép Dashboard
            return menuKey.Equals("Dashboard", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy danh sách menu có quyền truy cập
        /// </summary>
        public static List<string> GetAllowedMenus(string userRole)
        {
            if (string.IsNullOrEmpty(userRole))
                return new List<string> { "Dashboard" };

            if (RolePermissions.TryGetValue(userRole, out var permissions))
            {
                return new List<string>(permissions);
            }

            return new List<string> { "Dashboard" };
        }

        /// <summary>
        /// Kiểm tra quyền hành động cụ thể
        /// </summary>
        public static bool HasActionPermission(string userRole, string action)
        {
            return action.ToLower() switch
            {
                "create" => userRole == "Admin" || userRole == "Manager" || userRole == "HR",
                "edit" => userRole == "Admin" || userRole == "Manager" || userRole == "HR",
                "delete" => userRole == "Admin" || userRole == "Manager",
                "view" => true, // Tất cả có thể xem (nếu có quyền truy cập menu)
                "export" => userRole == "Admin" || userRole == "Manager" || userRole == "HR" || userRole == "Accountant",
                _ => false
            };
        }

        /// <summary>
        /// Lấy mô tả role
        /// </summary>
        public static string GetRoleDescription(string role)
        {
            return role switch
            {
                "Admin" => "Quản trị viên - Toàn quyền hệ thống",
                "Manager" => "Quản lý - Quản lý toàn bộ trừ phân quyền",
                "HR" => "Nhân sự - Quản lý nhân viên và lương",
                "Accountant" => "Kế toán - Quản lý tài chính",
                "ProjectManager" => "Quản lý dự án - Quản lý dự án và khách hàng",
                "Employee" => "Nhân viên - Xem thông tin cơ bản",
                "User" => "Người dùng - Quyền hạn chế",
                _ => "Không xác định"
            };
        }
    }
}