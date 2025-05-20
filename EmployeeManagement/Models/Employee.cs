using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; }  // Active, Inactive, On Leave
        public string ImagePath { get; set; }  // Đường dẫn hình ảnh nhân viên
        public int UserID { get; set; }  // Foreign Key
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual User User { get; set; }

        // Navigation Properties
        public virtual ICollection<Project> ManagedProjects { get; set; }
        public virtual ICollection<Task> AssignedTasks { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
