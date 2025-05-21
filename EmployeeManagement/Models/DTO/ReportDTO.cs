using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{
    public class MonthlyFinanceReport
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName => $"Tháng {Month:00}/{Year}";
        public decimal Income { get; set; }     // Thêm property này
        public decimal Expense { get; set; }    // Thêm property này
        public decimal Balance => Income - Expense; // Calculated property
        public int TransactionCount { get; set; }
        public decimal AverageTransaction => TransactionCount > 0 ? (Income + Expense) / TransactionCount : 0;
    }
    //public class MonthlyFinanceReport
    //{
    //    public int Month { get; set; }
    //    public int Year { get; set; }
    //    public decimal Income { get; set; }
    //    public decimal Expense { get; set; }
    //    public decimal Balance => Income - Expense;
    //    public string MonthName => new DateTime(Year, Month, 1).ToString("MM/yyyy");
    //}

}
