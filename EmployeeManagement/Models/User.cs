//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Models
//{
//    public class User
//    {
//        public int UserID { get; set; }
//        public string Username { get; set; }
//        public string Password { get; set; }
//        public string FullName { get; set; }
//        public string Email { get; set; }
//        public string Phone { get; set; }
//        public string Role { get; set; }  // Admin, Manager, Employee
//        public bool IsActive { get; set; }
//        public DateTime LastLogin { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        // Navigation Properties
//        public virtual Employee Employee { get; set; }
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;

namespace EmployeeManagement.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Trong thực tế, nên lưu mật khẩu đã được hash
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}