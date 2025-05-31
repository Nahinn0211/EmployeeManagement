using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{
    public class FinanceSearchCriteria
    {
        public string? TransactionCode { get; set; }
        public string? TransactionType { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public int? ProjectID { get; set; }
        public int? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
    public static class FinanceCategories
    {
        public static readonly string[] IncomeCategories = {
            "Revenue",      // Doanh thu
            "Investment",   // Đầu tư
            "Loan",         // Vay mượn
            "Other"         // Khác
        };

        public static readonly string[] ExpenseCategories = {
            "Salary",       // Lương
            "Equipment",    // Thiết bị
            "Office",       // Văn phòng
            "Marketing",    // Marketing
            "Travel",       // Di chuyển
            "Training",     // Đào tạo
            "Maintenance",  // Bảo trì
            "Utilities",    // Tiện ích
            "Insurance",    // Bảo hiểm
            "Tax",          // Thuế
            "Other"         // Khác
        };

        public static string GetDisplayName(string category)
        {
            return category switch
            {
                "Revenue" => "💵 Doanh thu",
                "Salary" => "👥 Lương",
                "Equipment" => "🔧 Thiết bị",
                "Office" => "🏢 Văn phòng",
                "Marketing" => "📢 Marketing",
                "Travel" => "✈️ Di chuyển",
                "Training" => "📚 Đào tạo",
                "Maintenance" => "🔧 Bảo trì",
                "Utilities" => "⚡ Tiện ích",
                "Insurance" => "🛡️ Bảo hiểm",
                "Tax" => "📋 Thuế",
                "Investment" => "📈 Đầu tư",
                "Loan" => "🏦 Vay mượn",
                "Other" => "📁 Khác",
                _ => category
            };
        }

        public static string[] GetCategoriesByType(string transactionType)
        {
            return transactionType switch
            {
                "Thu" => IncomeCategories,
                "Chi" => ExpenseCategories,
                _ => new string[] { }
            };
        }
    }


    public static class FinanceStatus
    {
        public static readonly string[] Statuses = {
            "Đã ghi nhận",  // Recorded
            "Chờ duyệt",    // Pending
            "Đã duyệt",     // Approved
            "Từ chối",      // Rejected
            "Hủy"           // Cancelled
        };

        public static string GetDisplayName(string status)
        {
            return status switch
            {
                "Đã ghi nhận" => "✅ Đã ghi nhận",
                "Chờ duyệt" => "⏳ Chờ duyệt",
                "Đã duyệt" => "✅ Đã duyệt",
                "Từ chối" => "❌ Từ chối",
                "Hủy" => "🚫 Hủy",
                _ => status
            };
        }
    }
    public class FinanceTrendAnalysis
    {
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit { get; set; }
        public decimal IncomeGrowthRate { get; set; }
        public decimal ExpenseGrowthRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<MonthlyFinanceReport> MonthlyReports { get; set; } = new List<MonthlyFinanceReport>();

        public string IncomeGrowthDisplay => IncomeGrowthRate >= 0 ? $"+{IncomeGrowthRate:F1}%" : $"{IncomeGrowthRate:F1}%";
        public string ExpenseGrowthDisplay => ExpenseGrowthRate >= 0 ? $"+{ExpenseGrowthRate:F1}%" : $"{ExpenseGrowthRate:F1}%";
        public string ProfitMarginDisplay => $"{ProfitMargin:F1}%";
        public string NetProfitClass => NetProfit >= 0 ? "text-success" : "text-danger";
        public string IncomeGrowthClass => IncomeGrowthRate >= 0 ? "text-success" : "text-danger";
        public string ExpenseGrowthClass => ExpenseGrowthRate <= 0 ? "text-success" : "text-warning";
    }


}
