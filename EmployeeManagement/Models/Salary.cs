using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Salary
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }  // Foreign Key
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }  // Lương cơ bản
        public decimal Allowance { get; set; }  // Phụ cấp
        public decimal Bonus { get; set; }  // Thưởng
        public decimal Tax { get; set; }  // Thuế
        public decimal Insurance { get; set; }  // Bảo hiểm
        public decimal TotalSalary { get; set; }  // Tổng lương
        public string PaymentStatus { get; set; }  // Paid, Unpaid
        public DateTime? PaymentDate { get; set; }  // Ngày thanh toán (nullable)
        public int CreatedBy { get; set; }  // Foreign Key (User)
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual Employee Employee { get; set; }
        public virtual User Creator { get; set; }
    }
}
