using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{
    // DTO for displaying salary in DataGridView
    public class SalaryDisplayModel
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear => $"{Month:00}/{Year}";
        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
        public string NetSalaryDisplay => $"{NetSalary:#,##0} VNĐ";
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateDisplay => PaymentDate?.ToString("dd/MM/yyyy") ?? "Chưa thanh toán";
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentStatusDisplay => GetPaymentStatusDisplay(PaymentStatus);
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        private static string GetPaymentStatusDisplay(string status)
        {
            return status switch
            {
                "Chưa thanh toán" => "⏳ Chưa thanh toán",
                "Đã thanh toán" => "✅ Đã thanh toán",
                "Thanh toán một phần" => "🔄 Thanh toán một phần",
                "Đã hủy" => "❌ Đã hủy",
                _ => status
            };
        }
    }

    // DTO for salary creation
    public class SalaryCreateModel
    {
        public int EmployeeID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary => BaseSalary + Allowance + Bonus - Deduction;
        public DateTime? PaymentDate { get; set; }
        public string PaymentStatus { get; set; } = "Chưa thanh toán";
        public string Notes { get; set; } = string.Empty;
    }

    // DTO for salary statistics
    public class SalaryStatistics
    {
        public int TotalRecords { get; set; }
        public int PaidRecords { get; set; }
        public int UnpaidRecords { get; set; }
        public int PartiallyPaidRecords { get; set; }
        public int CancelledRecords { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalUnpaidAmount { get; set; }
        public decimal AverageSalary { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
    }

    // DTO for salary search criteria
    public class SalarySearchCriteria
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? PaymentStatus { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }

    // DTO for employee dropdown
    public class EmployeeDropdownItem
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string DisplayText => $"{EmployeeCode} - {FullName} ({DepartmentName})";
    }

    // DTO for salary report
    public class SalaryReportData
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear => $"Tháng {Month:00}/{Year}";
        public int EmployeeCount { get; set; }
        public decimal TotalBaseSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal AverageSalary => EmployeeCount > 0 ? TotalNetSalary / EmployeeCount : 0;
    }

    // DTO for department salary summary
    public class DepartmentSalarySummary
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AverageSalary => EmployeeCount > 0 ? TotalSalary / EmployeeCount : 0;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }

    // Constants for salary management
    public static class SalaryConstants
    {
        public static readonly string[] PaymentStatuses = {
            "Chưa thanh toán",
            "Đã thanh toán",
            "Thanh toán một phần",
            "Đã hủy"
        };

        public static readonly string[] Months = {
            "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
            "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
            "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
        };

        public static string GetPaymentStatusDisplay(string status)
        {
            return status switch
            {
                "Chưa thanh toán" => "⏳ Chưa thanh toán",
                "Đã thanh toán" => "✅ Đã thanh toán",
                "Thanh toán một phần" => "🔄 Thanh toán một phần",
                "Đã hủy" => "❌ Đã hủy",
                _ => status
            };
        }

        public static Color GetPaymentStatusColor(string status)
        {
            return status switch
            {
                "Chưa thanh toán" => Color.FromArgb(255, 152, 0),
                "Đã thanh toán" => Color.FromArgb(76, 175, 80),
                "Thanh toán một phần" => Color.FromArgb(33, 150, 243),
                "Đã hủy" => Color.FromArgb(244, 67, 54),
                _ => Color.FromArgb(64, 64, 64)
            };
        }
    }

    // DTO for validation result
    public class SalaryValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    // DTO for delete validation
    public class SalaryDeleteValidation
    {
        public bool CanDelete { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}