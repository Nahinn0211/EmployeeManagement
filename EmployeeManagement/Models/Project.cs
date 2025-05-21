using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
     public class Project
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public int ManagerID { get; set; }
        public string Status { get; set; } = "Khởi tạo";
        public decimal CompletionPercentage { get; set; } = 0;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties (theo model của bạn)
        public Employee? Manager { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<WorkTask> Tasks { get; set; } = new List<WorkTask>(); // Sử dụng WorkTask thay vì Task
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Document> Documents { get; set; } = new List<Document>();
        public List<Finance> Finances { get; set; } = new List<Finance>();
    }

   
}