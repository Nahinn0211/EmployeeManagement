 
using System;

namespace EmployeeManagement.Models.DTO
{



    // Các class DTO cần thiết cho báo cáo
    public class AttendanceSearchCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Status { get; set; }
    }

    public class DailyAttendanceReportItem
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal WorkingHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CheckInMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class AttendanceSummaryReportItem
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public decimal TotalWorkingHours { get; set; }
        public decimal AverageWorkingHours { get; set; }
        public decimal AttendanceRate { get; set; }
    }

    public class FaceRecognitionAttendanceReportItem
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public decimal Confidence { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    
   
    /// <summary>
    /// DTO cho thống kê chấm công
    /// </summary>
    public class AttendanceStatistics
    {
        public int TotalRecords { get; set; }
        public int TotalEmployees { get; set; }
        public int PresentEmployees { get; set; }
        public int AbsentEmployees { get; set; }
        public int OnTimeCheckins { get; set; }
        public int LateCheckins { get; set; }
        public int EarlyCheckouts { get; set; }
        public double TotalWorkingHours { get; set; }
        public double AverageWorkingHours { get; set; }
        public decimal AttendanceRate { get; set; }
        public decimal PunctualityRate { get; set; }
    }
}