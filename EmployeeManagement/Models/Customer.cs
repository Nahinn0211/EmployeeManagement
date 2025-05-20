//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Models
//{
//    public class Customer
//    {
//        public int CustomerID { get; set; }
//        public string CustomerCode { get; set; }
//        public string CustomerName { get; set; }
//        public string Address { get; set; }
//        public string Phone { get; set; }
//        public string Email { get; set; }
//        public string ContactPerson { get; set; }  // Người liên hệ
//        public string TaxCode { get; set; }
//        public string Status { get; set; }  // Active, Inactive
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        // Navigation Properties
//        public virtual ICollection<Project> Projects { get; set; }
//        public virtual ICollection<Document> Documents { get; set; }
//    }
//}


using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Document> Documents { get; set; } = new List<Document>();
        public List<Finance> Finances { get; set; } = new List<Finance>();
    }
}