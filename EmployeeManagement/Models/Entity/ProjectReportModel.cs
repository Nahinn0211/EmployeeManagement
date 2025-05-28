using System;

namespace EmployeeManagement.Models.Entity
{
    /// <summary>
    /// Model hiển thị báo cáo chi tiết dự án
    /// </summary>
    public class ProjectReportModel
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal CompletionPercentage { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int TotalEmployees { get; set; }
        public decimal ActualCost { get; set; }
        public int DaysRemaining { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsCompleted { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Tính toán các chỉ số
        public decimal TaskCompletionRate => TotalTasks > 0 ? (decimal)CompletedTasks / TotalTasks * 100 : 0;
        public decimal BudgetUtilization => Budget > 0 ? ActualCost / Budget * 100 : 0;
        public string ProjectDuration => StartDate.HasValue && EndDate.HasValue ?
            $"{(EndDate.Value - StartDate.Value).Days} ngày" : "Chưa xác định";
        public string StatusColor => Status switch
        {
            "Hoàn thành" => "#4CAF50",
            "Đang thực hiện" => "#2196F3",
            "Tạm dừng" => "#FF9800",
            "Hủy bỏ" => "#F44336",
            _ => "#757575"
        };
    }

    /// <summary>
    /// Model thống kê tổng quan dự án
    /// </summary>
    public class ProjectStatistics
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int PausedProjects { get; set; }
        public int CancelledProjects { get; set; }
        public int OverdueProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalActualCost { get; set; }
        public decimal AverageCompletion { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalEmployees { get; set; }
                public int OnHoldProjects { get; set; }
        public int InitialProjects { get; set; }
        public decimal AverageBudget { get; set; }

        // Calculated properties
        public decimal ProjectCompletionRate => TotalProjects > 0 ? (decimal)CompletedProjects / TotalProjects * 100 : 0;
        public decimal TaskCompletionRate => TotalTasks > 0 ? (decimal)CompletedTasks / TotalTasks * 100 : 0;
        public decimal BudgetVariance => TotalBudget > 0 ? (TotalActualCost - TotalBudget) / TotalBudget * 100 : 0;
        public decimal OverdueRate => TotalProjects > 0 ? (decimal)OverdueProjects / TotalProjects * 100 : 0;
    }

    /// <summary>
    /// Model cho báo cáo theo thời gian
    /// </summary>
    public class ProjectTimelineReport
    {
        public DateTime Date { get; set; }
        public int ProjectsStarted { get; set; }
        public int ProjectsCompleted { get; set; }
        public int ActiveProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public string Period { get; set; } = string.Empty; // "Tháng 1/2024", "Q1 2024", etc.
    }

    /// <summary>
    /// Model cho top dự án theo tiêu chí
    /// </summary>
    public class TopProjectModel
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Metric { get; set; } = string.Empty; // "Budget", "Completion", "Duration", etc.
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho báo cáo theo Manager
    /// </summary>
    public class ManagerProjectReport
    {
        public int ManagerID { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string ManagerCode { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int ActiveProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal AverageCompletion { get; set; }
        public decimal SuccessRate => TotalProjects > 0 ? (decimal)CompletedProjects / TotalProjects * 100 : 0;
    }

    /// <summary>
    /// Filter cho báo cáo dự án
    /// </summary>
    public class ProjectReportFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ManagerID { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public decimal? MinCompletion { get; set; }
        public decimal? MaxCompletion { get; set; }
        public bool? IsOverdue { get; set; }
        public string SearchKeyword { get; set; } = string.Empty;
        public string SortBy { get; set; } = "ProjectName"; // ProjectName, StartDate, Budget, Completion
        public bool SortDescending { get; set; } = false;
    }
}