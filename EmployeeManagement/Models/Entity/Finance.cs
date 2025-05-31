using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.Entity
{
    public class Finance
    {
        public int FinanceID { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;  // Thu, Chi
        public string Category { get; set; } = string.Empty;  // Loại thu chi
        public int? ProjectID { get; set; }
        public int? EmployeeID { get; set; }
        public int? CustomerID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string ReferenceNo { get; set; } = string.Empty;
        public string Status { get; set; } = "Đã ghi nhận";
        public int RecordedByID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public User RecordedBy { get; set; }

        // Computed properties
        public string TransactionTypeDisplay => GetTransactionTypeDisplay(TransactionType);
        public string CategoryDisplay => GetCategoryDisplay(Category);
        public string StatusDisplay => GetStatusDisplay(Status);
        public string PaymentMethodDisplay => GetPaymentMethodDisplay(PaymentMethod);
        public string AmountDisplay => Amount.ToString("#,##0.00") + " VNĐ";
        public string AmountColorClass => TransactionType == "Thu" ? "text-success" : "text-danger";
        public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string TransactionDateDisplay => TransactionDate.ToString("dd/MM/yyyy");
        public string RelatedToDisplay => GetRelatedToDisplay();

        // Helper methods
        private string GetTransactionTypeDisplay(string type)
        {
            return type switch
            {
                "Thu" => "💰 Thu",
                "Chi" => "💸 Chi",
                _ => type
            };
        }

        private string GetCategoryDisplay(string category)
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

        private string GetStatusDisplay(string status)
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

        private string GetPaymentMethodDisplay(string method)
        {
            return method switch
            {
                "Cash" => "💵 Tiền mặt",
                "BankTransfer" => "🏦 Chuyển khoản",
                "Card" => "💳 Thẻ",
                "Check" => "📄 Séc",
                "EWallet" => "📱 Ví điện tử",
                "Other" => "📁 Khác",
                _ => method
            };
        }

        private string GetRelatedToDisplay()
        {
            if (ProjectID.HasValue && Project != null)
                return $"Dự án: {Project.ProjectName}";
            if (CustomerID.HasValue && Customer != null)
                return $"Khách hàng: {Customer.CompanyName}";
            if (EmployeeID.HasValue && Employee != null)
                return $"Nhân viên: {Employee.FullName}";
            return "Chung";
        }
    }

    // Display model for DataGridView
    public class FinanceDisplayModel
    {
        public int FinanceID { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountDisplay { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RelatedTo { get; set; } = string.Empty;
        public string RecordedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Static classes for dropdowns
    public static class TransactionTypes
    {
        public static readonly string[] Types = { "Thu", "Chi" };

        public static string GetDisplayName(string type)
        {
            return type switch
            {
                "Thu" => "💰 Thu",
                "Chi" => "💸 Chi",
                _ => type
            };
        }
    }
    /// <summary>
    /// Dự báo theo tháng
    /// </summary>
    public class MonthlyForecast
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal PredictedIncome { get; set; }
        public decimal PredictedExpense { get; set; }
        public decimal PredictedBalance { get; set; }
        public string Trend { get; set; }

        public string MonthDisplay => new DateTime(Year, Month, 1).ToString("MM/yyyy");
        public string TrendDisplay => Trend switch
        {
            "Up" => "📈 Tăng",
            "Down" => "📉 Giảm",
            "Stable" => "➡️ Ổn định",
            _ => Trend
        };

        public string TrendClass => Trend switch
        {
            "Up" => "text-success",
            "Down" => "text-danger",
            "Stable" => "text-info",
            _ => "text-secondary"
        };
    }
    /// <summary>
    /// Dự báo tài chính
    /// </summary>
    public class FinanceForecast
    {
        public List<MonthlyForecast> MonthlyForecasts { get; set; }
        public decimal PredictedTotalIncome { get; set; }
        public decimal PredictedTotalExpense { get; set; }
        public decimal PredictedBalance { get; set; }
        public string ConfidenceLevel { get; set; }
        public List<string> Assumptions { get; set; }
        public DateTime ForecastDate { get; set; }
    }


}