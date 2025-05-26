using System;

namespace EmployeeManagement.Models
{
    /// <summary>
    /// Model báo cáo chi tiết nhân viên
    /// </summary>
    public class EmployeeReportModel
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
        public string Status { get; set; } = string.Empty;
        public decimal CurrentSalary { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public decimal AttendanceRate { get; set; }
        public decimal PerformanceScore { get; set; }

        // Tính toán
        public int WorkingYears => WorkingDays / 365;
        public string WorkingDuration => WorkingDays >= 365 ?
            $"{WorkingYears} năm {(WorkingDays % 365) / 30} tháng" :
            $"{WorkingDays / 30} tháng";
        public string StatusColor => Status switch
        {
            "Đang làm việc" => "#4CAF50",
            "Nghỉ phép" => "#FF9800",
            "Đã nghỉ việc" => "#F44336",
            _ => "#757575"
        };
        public decimal ProjectSuccessRate => TotalProjects > 0 ?
            (decimal)CompletedProjects / TotalProjects * 100 : 0;
    }

    /// <summary>
    /// Model thống kê tổng quan nhân sự
    /// </summary>
    public class HRStatistics
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

        // Tính toán
        public decimal GenderRatio => TotalEmployees > 0 ?
            (decimal)MaleCount / TotalEmployees * 100 : 0;
        public decimal RetentionRate => TotalEmployees > 0 ?
            100 - TurnoverRate : 0;
        public decimal NewHireRate => TotalEmployees > 0 ?
            (decimal)NewHires / TotalEmployees * 100 : 0;
    }

    /// <summary>
    /// Model báo cáo theo phòng ban
    /// </summary>
    public class DepartmentReportModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int MaleEmployees { get; set; }
        public int FemaleEmployees { get; set; }
        public decimal AverageAge { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AverageAttendanceRate { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public decimal TotalSalaryCost { get; set; }
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }

        // Tính toán
        public decimal GenderRatio => TotalEmployees > 0 ?
            (decimal)MaleEmployees / TotalEmployees * 100 : 0;
        public decimal ProjectSuccessRate => TotalProjects > 0 ?
            (decimal)CompletedProjects / TotalProjects * 100 : 0;
        public decimal EmployeePercentage { get; set; }
    }

    /// <summary>
    /// Model báo cáo theo chức vụ
    /// </summary>
    public class PositionReportModel
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public decimal AverageAge { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public decimal TotalSalaryCost { get; set; }

        // Tính toán
        public decimal SalaryRange => MaxSalary - MinSalary;
        public decimal PositionPercentage { get; set; }
    }

    /// <summary>
    /// Model báo cáo sinh nhật tháng
    /// </summary>
    public class BirthdayReportModel
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
        public string BirthdayStatus => DaysUntilBirthday == 0 ? "Hôm nay" :
                                       DaysUntilBirthday > 0 ? $"Còn {DaysUntilBirthday} ngày" :
                                       $"Đã qua {Math.Abs(DaysUntilBirthday)} ngày";
    }

    /// <summary>
    /// Model báo cáo theo độ tuổi
    /// </summary>
    public class AgeGroupReportModel
    {
        public string AgeGroup { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal AverageWorkingYears { get; set; }
        public decimal AveragePerformanceScore { get; set; }
    }

    /// <summary>
    /// Filter cho báo cáo nhân sự
    /// </summary>
    public class HRReportFilter
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
    /// Model báo cáo chấm công
    /// </summary>
    public class AttendanceReportModel
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

        // Tính toán
        public decimal AttendanceRate => WorkingDays > 0 ?
            (decimal)PresentDays / WorkingDays * 100 : 0;
        public decimal AbsenceRate => WorkingDays > 0 ?
            (decimal)AbsentDays / WorkingDays * 100 : 0;
        public decimal AverageWorkingHoursPerDay => PresentDays > 0 ?
            TotalWorkingHours / PresentDays : 0;
    }
}