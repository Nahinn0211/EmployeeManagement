using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Finance
    {
        public int FinanceID { get; set; }
        public string TransactionCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }  // Income, Expense
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }  // Salary, Project, Office, Other
        public int? ProjectID { get; set; }  // Foreign Key (Project), nullable
        public int CreatedBy { get; set; }  // Foreign Key (User)
        public string Status { get; set; }  // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual Project Project { get; set; }
        public virtual User Creator { get; set; }
    }
}
