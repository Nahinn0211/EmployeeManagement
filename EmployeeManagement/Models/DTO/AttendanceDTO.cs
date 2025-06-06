
using System;

namespace EmployeeManagement.Models.DTO
{



    /// <summary>
    /// Tiêu chí tìm kiếm chấm công
    /// </summary>
    public class AttendanceSearchCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Status { get; set; }
        public string? CheckInMethod { get; set; }

        public AttendanceSearchCriteria()
        {
            FromDate = DateTime.Now.AddDays(-30);
            ToDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Báo cáo chấm công hàng ngày
    /// </summary>
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

        // Display properties
        public string CheckInTimeDisplay => CheckInTime.ToString("dd/MM/yyyy HH:mm:ss");
        public string CheckOutTimeDisplay => CheckOutTime?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa chấm công ra";
        public string WorkingHoursDisplay => $"{WorkingHours:F2}h";
        public string StatusDisplay => GetStatusDisplay(Status);

        private static string GetStatusDisplay(string status)
        {
            return status switch
            {
                "Đã chấm công vào" => "⏰ Đã vào",
                "Đủ giờ" => "✅ Đủ giờ",
                "Thiếu giờ" => "⚠️ Thiếu giờ",
                "Về sớm" => "🔴 Về sớm",
                "Đã chấm công ra" => "🏁 Đã ra",
                _ => status
            };
        }
    }

    /// <summary>
    /// Báo cáo tổng hợp chấm công
    /// </summary>
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

        // Display properties
        public string AttendanceRateDisplay => $"{AttendanceRate:F1}%";
        public string TotalWorkingHoursDisplay => $"{TotalWorkingHours:F1}h";
        public string AverageWorkingHoursDisplay => $"{AverageWorkingHours:F1}h";
    }

    /// <summary>
    /// Báo cáo chấm công bằng khuôn mặt
    /// </summary>
    public class FaceRecognitionAttendanceReportItem
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public decimal Confidence { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Display properties
        public string CheckInTimeDisplay => CheckInTime.ToString("dd/MM/yyyy HH:mm:ss");
        public string ConfidenceDisplay => $"{Confidence:F1}%";
        public string ConfidenceClass => Confidence >= 90 ? "text-success" :
                                       Confidence >= 70 ? "text-warning" : "text-danger";
    }

    /// <summary>
    /// Thống kê chấm công
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

        // Display properties
        public string AttendanceRateDisplay => $"{AttendanceRate:F1}%";
        public string PunctualityRateDisplay => $"{PunctualityRate:F1}%";
        public string TotalWorkingHoursDisplay => $"{TotalWorkingHours:F1}h";
        public string AverageWorkingHoursDisplay => $"{AverageWorkingHours:F1}h";
    }




}