using System;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.Models
{
    public class Salary
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentStatus { get; set; } = "Chưa thanh toán";
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }

        // Display properties
        public string MonthYearDisplay => $"{Month:00}/{Year}";
        public string NetSalaryDisplay => $"{NetSalary:#,##0} VNĐ";
        public string PaymentStatusDisplay => SalaryConstants.GetPaymentStatusDisplay(PaymentStatus);
        public string PaymentDateDisplay => PaymentDate?.ToString("dd/MM/yyyy") ?? "Chưa thanh toán";

        // Calculated properties
        public decimal CalculatedNetSalary => BaseSalary + Allowance + Bonus - Deduction;
        public bool IsPaid => PaymentStatus == "Đã thanh toán";
        public bool IsOverdue => !IsPaid && DateTime.Now > new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

        // Business logic methods
        public void CalculateNetSalary()
        {
            NetSalary = BaseSalary + Allowance + Bonus - Deduction;
        }

        public bool CanEdit()
        {
            return PaymentStatus != "Đã thanh toán";
        }

        public bool CanDelete()
        {
            return PaymentStatus == "Chưa thanh toán" || PaymentStatus == "Đã hủy";
        }

        public void MarkAsPaid(DateTime paymentDate)
        {
            PaymentStatus = "Đã thanh toán";
            PaymentDate = paymentDate;
        }

        public void MarkAsPartiallyPaid(DateTime paymentDate)
        {
            PaymentStatus = "Thanh toán một phần";
            PaymentDate = paymentDate;
        }

        public void Cancel()
        {
            PaymentStatus = "Đã hủy";
            PaymentDate = null;
        }
    }
}