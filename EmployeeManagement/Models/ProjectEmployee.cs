using System;

namespace EmployeeManagement.Models
{
    public class ProjectEmployee
    {
        public int ProjectEmployeeID { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
        public string RoleInProject { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee Employee { get; set; }
    }
}