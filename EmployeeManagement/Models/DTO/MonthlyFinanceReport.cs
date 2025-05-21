// Tạo file Models/DTO/FinanceReportModels.cs để tránh conflict namespace

using EmployeeManagement.Models.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// Báo cáo tài chính theo tháng (DTO)
    /// </summary>
    public class MonthlyFinanceReportDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName => $"Tháng {Month:00}/{Year}";
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance => Income - Expense;
        public int TransactionCount { get; set; }
        public decimal AverageTransaction => TransactionCount > 0 ? (Income + Expense) / TransactionCount : 0;
    }
    /// <summary>
    /// Báo cáo tài chính dự án (DTO) 
    /// </summary>
    //public class ProjectFinanceReportDTO
    //{
    //    public int ProjectID { get; set; }
    //    public string ProjectName { get; set; }
    //    public decimal Income { get; set; }
    //    public decimal Expense { get; set; }
    //    public decimal Budget { get; set; }
    //    public decimal NetProfit => Income - Expense;
    //    public decimal BudgetUsedPercent => Budget > 0 ? (Expense / Budget) * 100 : 0;

    //    // Computed Properties
    //    public string IncomeDisplay => Income.ToString("#,##0.00") + " VNĐ";
    //    public string ExpenseDisplay => Expense.ToString("#,##0.00") + " VNĐ";
    //    public string BudgetDisplay => Budget.ToString("#,##0.00") + " VNĐ";
    //    public string NetProfitDisplay => NetProfit.ToString("#,##0.00") + " VNĐ";
    //    public string BudgetUsedDisplay => BudgetUsedPercent.ToString("F1") + "%";
    //    public string NetProfitClass => NetProfit >= 0 ? "text-success" : "text-danger";
    //    public string BudgetUsedClass => BudgetUsedPercent > 90 ? "text-danger" :
    //                                    BudgetUsedPercent > 70 ? "text-warning" : "text-success";
    //}

    public class ProjectFinanceReportDTO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance => Income - Expense;
        public decimal BudgetUsed => Budget > 0 ? (Expense / Budget * 100) : 0;
        public string BudgetStatus => BudgetUsed > 90 ? "Vượt ngân sách" :
                                     BudgetUsed > 70 ? "Gần hết ngân sách" :
                                     "Trong ngân sách";
        public int TransactionCount { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }
}
