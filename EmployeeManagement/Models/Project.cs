using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; }  // Planning, In Progress, Completed, On Hold
        public int CustomerID { get; set; }  // Foreign Key
        public int ManagerID { get; set; }  // Foreign Key (Employee)
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual Customer Customer { get; set; }
        public virtual Employee Manager { get; set; }

        // Navigation Properties
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Finance> Finances { get; set; }
    }
}
