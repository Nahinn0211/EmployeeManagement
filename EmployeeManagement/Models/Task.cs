using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }  // Low, Medium, High
        public string Status { get; set; }  // Not Started, In Progress, Completed, Delayed
        public int ProjectID { get; set; }  // Foreign Key
        public int AssignedTo { get; set; }  // Foreign Key (Employee)
        public int CreatedBy { get; set; }  // Foreign Key (User)
        public int? CompletedPercent { get; set; }  // Phần trăm hoàn thành
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual Project Project { get; set; }
        public virtual Employee AssignedEmployee { get; set; }
        public virtual User Creator { get; set; }
    }
}
