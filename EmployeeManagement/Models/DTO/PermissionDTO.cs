using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// DTO hiển thị Role trong DataGridView
    /// </summary>
    public class RoleDisplayModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public string UserCountDisplay { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtDisplay { get; set; } = string.Empty;
        public bool CanDelete { get; set; }
        public string UsageStatus { get; set; } = string.Empty;

        // Computed properties
        public string UsageLevel => GetUsageLevel(UserCount);
        public string Status => CanDelete ? "✅ Có thể xóa" : "🔒 Đang sử dụng";

        private string GetUsageLevel(int count)
        {
            if (count == 0) return "🔴 Chưa sử dụng";
            if (count <= 5) return "🟡 Ít sử dụng";
            if (count <= 20) return "🟢 Bình thường";
            return "🔵 Sử dụng nhiều";
        }
    }

    /// <summary>
    /// DTO hiển thị User-Role assignment
    /// </summary>
    public class UserRoleDisplayModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IsActiveDisplay { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public string AssignedAtDisplay => AssignedAt?.ToString("dd/MM/yyyy") ?? "";

        // Computed properties
        public string DisplayText => $"{Username} - {FullName}";
        public string DepartmentPosition => string.IsNullOrEmpty(DepartmentName) ?
            PositionName : $"{DepartmentName} - {PositionName}";
    }

    /// <summary>
    /// Kết quả validation khi xóa Role
    /// </summary>
    public class RoleDeleteValidation
    {
        public bool CanDelete { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<string> AssignedUsers { get; set; } = new List<string>();
        public int UserCount => AssignedUsers.Count;

        public string DetailMessage => CanDelete ?
            "Có thể xóa quyền này" :
            $"Không thể xóa - {Reason}. Danh sách người dùng: {string.Join(", ", AssignedUsers)}";
    }

    /// <summary>
    /// Thống kê phân quyền tổng quan
    /// </summary>
    public class PermissionStatistics
    {
        public int TotalRoles { get; set; }
        public int TotalUsers { get; set; }
        public int UsersWithRoles { get; set; }
        public int UsersWithoutRoles { get; set; }
        public decimal AverageRolesPerUser { get; set; }
        public string MostUsedRole { get; set; } = string.Empty;
        public int MostUsedRoleCount { get; set; }
        public string LeastUsedRole { get; set; } = string.Empty;
        public int LeastUsedRoleCount { get; set; }

        // Computed properties
        public decimal UserCoveragePercentage => TotalUsers > 0 ?
            (decimal)UsersWithRoles / TotalUsers * 100 : 0;

        public string CoverageDisplay => $"{UserCoveragePercentage:F1}%";

        public string AvgRolesDisplay => $"{AverageRolesPerUser:F1} quyền/người";
    }

    /// <summary>
    /// Thống kê sử dụng từng Role
    /// </summary>
    public class RoleUsageStatistic
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public decimal UsagePercentage { get; set; }
        public DateTime? LastAssigned { get; set; }
        public string LastAssignedDisplay => LastAssigned?.ToString("dd/MM/yyyy") ?? "Chưa có";

        // Computed properties
        public string UsageDisplay => $"{UsagePercentage:F1}%";
        public string UsageLevel => GetUsageLevel();
        public string UsageColor => GetUsageColor();

        private string GetUsageLevel()
        {
            if (UserCount == 0) return "Không sử dụng";
            if (UsagePercentage < 10) return "Ít sử dụng";
            if (UsagePercentage < 50) return "Sử dụng bình thường";
            return "Sử dụng nhiều";
        }

        private string GetUsageColor()
        {
            if (UserCount == 0) return "#f44336"; // Red
            if (UsagePercentage < 10) return "#ff9800"; // Orange
            if (UsagePercentage < 50) return "#4caf50"; // Green
            return "#2196f3"; // Blue
        }
    }

    /// <summary>
    /// Filter cho tìm kiếm Role
    /// </summary>
    public class RoleSearchCriteria
    {
        public string SearchText { get; set; } = string.Empty;
        public string UsageLevel { get; set; } = string.Empty; // "all", "unused", "low", "normal", "high"
        public bool? CanDelete { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int? MinUsers { get; set; }
        public int? MaxUsers { get; set; }
        public string SortBy { get; set; } = "RoleName"; // RoleName, UserCount, CreatedAt
        public bool SortDescending { get; set; } = false;
    }

    /// <summary>
    /// Audit log cho thay đổi phân quyền
    /// </summary>
    public class PermissionAuditLog
    {
        public int LogID { get; set; }
        public string Action { get; set; } = string.Empty; // "Assign", "Remove", "Create", "Update", "Delete"
        public int? RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int? UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public int PerformedByUserID { get; set; }
        public string PerformedByUsername { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;

        // Computed properties
        public string ActionDisplay => GetActionDisplay();
        public string TimestampDisplay => Timestamp.ToString("dd/MM/yyyy HH:mm:ss");
        public string ActionIcon => GetActionIcon();

        private string GetActionDisplay()
        {
            return Action switch
            {
                "Assign" => "Gán quyền",
                "Remove" => "Gỡ quyền",
                "Create" => "Tạo quyền",
                "Update" => "Cập nhật quyền",
                "Delete" => "Xóa quyền",
                _ => Action
            };
        }

        private string GetActionIcon()
        {
            return Action switch
            {
                "Assign" => "➕",
                "Remove" => "➖",
                "Create" => "🆕",
                "Update" => "✏️",
                "Delete" => "🗑️",
                _ => "📝"
            };
        }
    }

    /// <summary>
    /// Bulk operation result
    /// </summary>
    public class BulkOperationResult
    {
        public int TotalItems { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public TimeSpan Duration { get; set; }

        // Computed properties
        public bool IsSuccess => FailureCount == 0;
        public decimal SuccessRate => TotalItems > 0 ?
            (decimal)SuccessCount / TotalItems * 100 : 0;
        public string SummaryMessage => $"Thành công: {SuccessCount}/{TotalItems} ({SuccessRate:F1}%)";
        public string DurationDisplay => $"{Duration.TotalSeconds:F1}s";
    }

    /// <summary>
    /// Export options cho báo cáo phân quyền
    /// </summary>
    public class PermissionExportOptions
    {
        public bool IncludeRoles { get; set; } = true;
        public bool IncludeUsers { get; set; } = true;
        public bool IncludeAssignments { get; set; } = true;
        public bool IncludeStatistics { get; set; } = true;
        public bool IncludeAuditLog { get; set; } = false;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<int> SelectedRoleIds { get; set; } = new List<int>();
        public string Format { get; set; } = "Excel"; // Excel, PDF, CSV
        public string FileName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Dashboard metrics cho phân quyền
    /// </summary>
    public class PermissionDashboardMetrics
    {
        public PermissionStatistics GeneralStats { get; set; } = new PermissionStatistics();
        public List<RoleUsageStatistic> TopUsedRoles { get; set; } = new List<RoleUsageStatistic>();
        public List<RoleUsageStatistic> UnusedRoles { get; set; } = new List<RoleUsageStatistic>();
        public List<UserRoleDisplayModel> RecentAssignments { get; set; } = new List<UserRoleDisplayModel>();
        public List<PermissionAuditLog> RecentActivities { get; set; } = new List<PermissionAuditLog>();

        // Trend data
        public List<DailyPermissionActivity> ActivityTrend { get; set; } = new List<DailyPermissionActivity>();
        public List<RoleGrowthData> RoleGrowthTrend { get; set; } = new List<RoleGrowthData>();
    }

    /// <summary>
    /// Hoạt động phân quyền theo ngày
    /// </summary>
    public class DailyPermissionActivity
    {
        public DateTime Date { get; set; }
        public int AssignmentCount { get; set; }
        public int RemovalCount { get; set; }
        public int RoleCreated { get; set; }
        public int RoleDeleted { get; set; }
        public string DateDisplay => Date.ToString("dd/MM");
        public int NetActivity => AssignmentCount - RemovalCount;
    }

    /// <summary>
    /// Dữ liệu tăng trưởng Role theo thời gian
    /// </summary>
    public class RoleGrowthData
    {
        public DateTime Period { get; set; }
        public int TotalRoles { get; set; }
        public int NewRoles { get; set; }
        public int DeletedRoles { get; set; }
        public int TotalAssignments { get; set; }
        public string PeriodDisplay => Period.ToString("MM/yyyy");
        public decimal GrowthRate { get; set; }
    }

    /// <summary>
    /// Template cho việc tạo Role nhanh
    /// </summary>
    public class RoleTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
        public string Category { get; set; } = string.Empty; // Admin, Management, Employee, etc.
        public bool IsSystemRole { get; set; } = false;
        public string Icon { get; set; } = "🔑";

        public string DisplayName => $"{Icon} {Name}";
        public string CategoryDisplay => GetCategoryDisplay();

        private string GetCategoryDisplay()
        {
            return Category switch
            {
                "Admin" => "👑 Quản trị",
                "Management" => "👥 Quản lý",
                "Employee" => "👤 Nhân viên",
                "Finance" => "💰 Tài chính",
                "HR" => "🏢 Nhân sự",
                "IT" => "💻 IT",
                _ => Category
            };
        }
    }

    /// <summary>
    /// Conflict detection cho Role assignment
    /// </summary>
    public class RoleConflictDetection
    {
        public List<ConflictRule> ConflictRules { get; set; } = new List<ConflictRule>();
        public List<DetectedConflict> DetectedConflicts { get; set; } = new List<DetectedConflict>();
        public bool HasConflicts => DetectedConflicts.Count > 0;
    }

    public class ConflictRule
    {
        public int RuleID { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public List<int> ConflictingRoleIds { get; set; } = new List<int>();
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "Warning"; // Warning, Error
    }

    public class DetectedConflict
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<int> ConflictingRoleIds { get; set; } = new List<int>();
        public string ConflictDescription { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; }
    }
}