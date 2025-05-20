using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int? ManagerID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Employee Manager { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}