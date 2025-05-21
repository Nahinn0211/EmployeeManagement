using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactTitle { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Đang hợp tác";
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Document> Documents { get; set; } = new List<Document>();
        public List<Finance> Finances { get; set; } = new List<Finance>();

        // Computed properties
        public string StatusDisplay => GetStatusDisplayText(Status);
        public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy");
        public string UpdatedAtDisplay => UpdatedAt.ToString("dd/MM/yyyy HH:mm");

        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Đang hợp tác" => "🤝 Đang hợp tác",
                "Tạm dừng" => "⏸️ Tạm dừng",
                "Ngừng hợp tác" => "🚫 Ngừng hợp tác",
                _ => status
            };
        }
    }

   
}