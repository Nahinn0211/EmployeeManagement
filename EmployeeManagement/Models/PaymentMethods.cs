using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class PaymentMethods
    {
        public static readonly string[] Methods = {
            "Cash",         // Tiền mặt
            "BankTransfer", // Chuyển khoản
            "Card",         // Thẻ
            "Check",        // Séc
            "EWallet",      // Ví điện tử
            "Other"         // Khác
        };

        public static string GetDisplayName(string method)
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
    }
    public class CashFlowReport
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal NetFlow { get; set; }
        public decimal RunningBalance { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MM/yyyy");
        public string NetFlowDisplay => NetFlow >= 0 ? $"+{NetFlow:#,##0}" : $"{NetFlow:#,##0}";
        public string NetFlowClass => NetFlow >= 0 ? "text-success" : "text-danger";
    }

}
