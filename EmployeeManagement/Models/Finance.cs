//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Models
//{
//    public class Finance
//    {
//        public int FinanceID { get; set; }
//        public string TransactionCode { get; set; }
//        public DateTime TransactionDate { get; set; }
//        public string TransactionType { get; set; }  // Income, Expense
//        public string Description { get; set; }
//        public decimal Amount { get; set; }
//        public string Category { get; set; }  // Salary, Project, Office, Other
//        public int? ProjectID { get; set; }  // Foreign Key (Project), nullable
//        public int CreatedBy { get; set; }  // Foreign Key (User)
//        public string Status { get; set; }  // Pending, Approved, Rejected
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        // Foreign Key References
//        public virtual Project Project { get; set; }
//        public virtual User Creator { get; set; }
//    }
//}

using System;

namespace EmployeeManagement.Models
{
    public class Finance
    {
        public int FinanceID { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }  // Thu, Chi
        public string Category { get; set; }  // Loại thu chi
        public int? ProjectID { get; set; }
        public int? EmployeeID { get; set; }
        public int? CustomerID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public int RecordedByID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public User RecordedBy { get; set; }
    }
}
