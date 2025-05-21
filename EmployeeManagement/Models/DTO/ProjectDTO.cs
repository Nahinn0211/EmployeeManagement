using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{

    // Class hiển thị Project trong DataGridView
    public class ProjectDisplayModel
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public int TaskCount { get; set; }
        public string Duration { get; set; } = string.Empty;
    }

    // Class thống kê Project
    public class ProjectStatistics
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public int InitialProjects { get; set; }
        public int CancelledProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal AverageCompletion { get; set; }
        public decimal AverageBudget { get; set; }
    }

    // Class cho việc tạo mới Project
    public class ProjectCreateModel
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public int ManagerID { get; set; }
        public string Status { get; set; } = "Khởi tạo";
        public string? Notes { get; set; }
    }


    //public class ProjectFinanceReport
    //{
    //    public int ProjectID { get; set; }
    //    public string ProjectName { get; set; }
    //    public decimal TotalIncome { get; set; }
    //    public decimal TotalExpense { get; set; }
    //    public decimal Balance { get; set; }
    //    public decimal Budget { get; set; }
    //    public List<Finance> Transactions { get; set; } = new List<Finance>();
    //    public DateTime? FromDate { get; set; }
    //    public DateTime? ToDate { get; set; }
    //}

    public class ProjectFinanceReport
    {

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Finance> Transactions { get; set; } = new List<Finance>();

        public decimal TotalExpense { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Income { get; set; }  // Thêm property này
        public decimal Expense { get; set; } // Thêm property này
        public decimal Balance { get; set; }
        public decimal BudgetUsed => Budget > 0 ? (Expense / Budget * 100) : 0; // Calculated property
        public string BudgetStatus => BudgetUsed > 90 ? "Vượt ngân sách" :
                                     BudgetUsed > 70 ? "Gần hết ngân sách" :
                                     "Trong ngân sách"; // Calculated property
        public int TransactionCount { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }

}
