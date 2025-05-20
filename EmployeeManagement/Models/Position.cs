using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class Position
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        public string Description { get; set; }
        public decimal BaseSalary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}