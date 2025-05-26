using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// DTO cho thống kê tổng quan nhân sự
    /// </summary>
    public class HRStatisticsDTO
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int NewHires { get; set; }
        public int Resignations { get; set; }
        public decimal TurnoverRate { get; set; }
        public decimal AverageAge { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal AverageAttendanceRate { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public string GenderRatio => TotalEmployees > 0 ? $"{MaleCount}:{FemaleCount}" : "0:0";
    }

    /// <summary>
    /// DTO cho báo cáo nhân viên chi tiết
    /// </summary>
    public class EmployeeReportDTO
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public int WorkingDays { get; set; }
        public decimal WorkingYears => WorkingDays / 365m;
        public string Status { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal CurrentSalary { get; set; }
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public decimal AttendanceRate { get; set; }
        public decimal PerformanceScore { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo phòng ban
    /// </summary>
    public class DepartmentReportDTO
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int MaleEmployees { get; set; }
        public int FemaleEmployees { get; set; }
        public decimal EmployeePercentage { get; set; }
        public decimal AverageAge { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal TotalSalaryCost { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo sinh nhật
    /// </summary>
    public class BirthdayReportDTO
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DaysUntilBirthday { get; set; }

        public string BirthdayStatus { get;  set; }

        

        public void UpdateBirthdayStatus()
        {
            BirthdayStatus = DaysUntilBirthday == 0 ? "Hôm nay" :
                             DaysUntilBirthday > 0 ? $"Còn {DaysUntilBirthday} ngày" :
                             $"Đã qua {Math.Abs(DaysUntilBirthday)} ngày";
        }
    }

    /// <summary>
    /// DTO cho báo cáo theo nhóm tuổi
    /// </summary>
    public class AgeGroupReportDTO
    {
        public string AgeGroup { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AveragePerformanceScore { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo chấm công
    /// </summary>
    public class AttendanceReportDTO
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public decimal TotalWorkingHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal AttendanceRate => WorkingDays > 0 ? (decimal)PresentDays / WorkingDays * 100 : 0;
    }

    /// <summary>
    /// DTO cho filter báo cáo nhân sự
    /// </summary>
    public class HRReportFilterDTO
    {
        public int? DepartmentID { get; set; }
        public int? PositionID { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string SearchKeyword { get; set; } = string.Empty;
        public string SortBy { get; set; } = "FullName";
        public bool SortDescending { get; set; } = false;
    }

    /// <summary>
    /// DTO cho dashboard metrics
    /// </summary>
    public class DashboardMetricsDTO
    {
        public HRStatisticsDTO GeneralStats { get; set; } = new HRStatisticsDTO();
        public List<EmployeeReportDTO> TopPerformers { get; set; } = new List<EmployeeReportDTO>();
        public List<EmployeeReportDTO> TopEarners { get; set; } = new List<EmployeeReportDTO>();
        public List<EmployeeReportDTO> HighestAttendance { get; set; } = new List<EmployeeReportDTO>();
        public List<DepartmentReportDTO> DepartmentBreakdown { get; set; } = new List<DepartmentReportDTO>();
        public List<BirthdayReportDTO> UpcomingBirthdays { get; set; } = new List<BirthdayReportDTO>();
        public List<AgeGroupReportDTO> AgeDistribution { get; set; } = new List<AgeGroupReportDTO>();
        public List<EmployeeReportDTO> RecentHires { get; set; } = new List<EmployeeReportDTO>();
        public List<EmployeeReportDTO> LongTermEmployees { get; set; } = new List<EmployeeReportDTO>();
    }

    /// <summary>
    /// DTO cho project report
    /// </summary>
    public class ProjectReportDTO
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

        // Calculated properties
        public decimal TaskCompletionRate => TotalTasks > 0 ? (decimal)CompletedTasks / TotalTasks * 100 : 0;
        public decimal BudgetUtilization => Budget > 0 ? ActualCost / Budget * 100 : 0;
        public string ProjectDuration => StartDate.HasValue && EndDate.HasValue ?
            $"{(EndDate.Value - StartDate.Value).Days} ngày" : "Chưa xác định";
    }

    /// <summary>
    /// DTO cho export data
    /// </summary>
    public class ExportDataDTO
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}