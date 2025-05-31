using System;
using System.Collections.Generic;
using System.Drawing;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// DTO for displaying users in DataGridView
    /// </summary>
    public class UserDisplayModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IsActiveDisplay { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public string LastLoginDisplay { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtDisplay { get; set; } = string.Empty;
        public bool HasEmployee { get; set; }
    }

    /// <summary>
    /// DTO for user creation
    /// </summary>
    public class UserCreateModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// DTO for user update
    /// </summary>
    public class UserUpdateModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// DTO for password change
    /// </summary>
    public class PasswordChangeModel
    {
        public int UserID { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for password reset
    /// </summary>
    public class PasswordResetModel
    {
        public int UserID { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool GenerateRandom { get; set; } = false;
    }

    /// <summary>
    /// DTO for user statistics
    /// </summary>
    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int UsersWithEmployee { get; set; }
        public int UsersWithoutEmployee { get; set; }
        public int RecentlyActiveUsers { get; set; }
        public int UsersWithRoles { get; set; }
        public int UsersWithoutRoles { get; set; }

        // Calculated properties
        public decimal ActivePercentage => TotalUsers > 0 ? (decimal)ActiveUsers / TotalUsers * 100 : 0;
        public decimal EmployeeLinkedPercentage => TotalUsers > 0 ? (decimal)UsersWithEmployee / TotalUsers * 100 : 0;
        public decimal RecentActivityPercentage => TotalUsers > 0 ? (decimal)RecentlyActiveUsers / TotalUsers * 100 : 0;
    }

    /// <summary>
    /// DTO for user search criteria
    /// </summary>
    public class UserSearchCriteria
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasEmployee { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? LastLoginFrom { get; set; }
        public DateTime? LastLoginTo { get; set; }
    }

    /// <summary>
    /// DTO for employee dropdown
    /// </summary>
    public class EmployeeDropdownModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string DisplayText => $"{EmployeeCode} - {FullName} ({DepartmentName})";
    }

    /// <summary>
    /// DTO for role dropdown
    /// </summary>
    public class RoleDropdownModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DisplayText => $"{RoleName} - {Description}";
    }

    /// <summary>
    /// DTO for validation result
    /// </summary>
    public class UserValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for delete validation
    /// </summary>
    public class UserDeleteValidation
    {
        public bool CanDelete { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<string> Dependencies { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for role assignment
    /// </summary>
    public class RoleAssignmentModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<RoleItemModel> AvailableRoles { get; set; } = new List<RoleItemModel>();
        public List<int> AssignedRoleIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// DTO for role item in assignment
    /// </summary>
    public class RoleItemModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }

    /// <summary>
    /// Constants for user management
    /// </summary>
    public static class UserConstants
    {
        public static readonly string[] UserStatuses = { "Hoạt động", "Vô hiệu" };

        public static string GetStatusDisplay(bool isActive)
        {
            return isActive ? "✅ Hoạt động" : "❌ Vô hiệu";
        }

        public static Color GetStatusColor(bool isActive)
        {
            return isActive ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
        }

        public static string GetLastLoginDisplay(DateTime? lastLogin)
        {
            if (!lastLogin.HasValue)
                return "Chưa đăng nhập";

            var daysDiff = (DateTime.Now - lastLogin.Value).Days;

            if (daysDiff == 0)
                return "Hôm nay";
            else if (daysDiff == 1)
                return "Hôm qua";
            else if (daysDiff <= 7)
                return $"{daysDiff} ngày trước";
            else if (daysDiff <= 30)
                return $"{daysDiff / 7} tuần trước";
            else
                return lastLogin.Value.ToString("dd/MM/yyyy");
        }

        public static string GetAccountTypeDisplay(bool hasEmployee)
        {
            return hasEmployee ? "👤 Nhân viên" : "🔧 Hệ thống";
        }
    }

    /// <summary>
    /// DTO for user activity report
    /// </summary>
    public class UserActivityReport
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public int LoginCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DaysSinceCreated { get; set; }
        public int DaysSinceLastLogin { get; set; }
        public string ActivityStatus { get; set; } = string.Empty;

        // Calculated properties
        public string ActivityStatusDisplay => ActivityStatus switch
        {
            "Very Active" => "🟢 Rất hoạt động",
            "Active" => "🟡 Hoạt động",
            "Inactive" => "🟠 Ít hoạt động",
            "Dormant" => "🔴 Không hoạt động",
            _ => ActivityStatus
        };
    }

    /// <summary>
    /// DTO for user role summary
    /// </summary>
    public class UserRoleSummary
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> RoleNames { get; set; } = new List<string>();
        public int RoleCount { get; set; }
        public string RolesDisplay => string.Join(", ", RoleNames);
        public string RoleCountDisplay => $"{RoleCount} quyền";
    }

    /// <summary>
    /// DTO for bulk operations
    /// </summary>
    public class UserBulkOperation
    {
        public List<int> UserIds { get; set; } = new List<int>();
        public string Operation { get; set; } = string.Empty; // "Activate", "Deactivate", "Delete", "AssignRole", "RemoveRole"
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// DTO for user import
    /// </summary>
    public class UserImportModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string RoleNames { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Password { get; set; } = string.Empty;

        // Validation result
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int? EmployeeID { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}