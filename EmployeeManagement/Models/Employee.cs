//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Models
//{
//    public class Employee
//    {
//        public int EmployeeID { get; set; }
//        public string EmployeeCode { get; set; }
//        public string FullName { get; set; }
//        public DateTime DateOfBirth { get; set; }
//        public string Gender { get; set; }
//        public string Address { get; set; }
//        public string Phone { get; set; }
//        public string Email { get; set; }
//        public string Department { get; set; }
//        public string Position { get; set; }
//        public DateTime JoinDate { get; set; }
//        public string Status { get; set; }  // Active, Inactive, On Leave
//        public string ImagePath { get; set; }  // Đường dẫn hình ảnh nhân viên
//        public int UserID { get; set; }  // Foreign Key
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        // Foreign Key References
//        public virtual User User { get; set; }

//        // Navigation Properties
//        public virtual ICollection<Project> ManagedProjects { get; set; }
//        public virtual ICollection<Task> AssignedTasks { get; set; }
//        public virtual ICollection<Salary> Salaries { get; set; }
//        public virtual ICollection<Attendance> Attendances { get; set; }
//    }
//}


using System;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IDCardNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int DepartmentID { get; set; }
        public int PositionID { get; set; }
        public int? ManagerID { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string TaxCode { get; set; }
        public string InsuranceCode { get; set; }
        public string FaceDataPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
