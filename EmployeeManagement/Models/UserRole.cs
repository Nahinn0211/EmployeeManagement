using System;

namespace EmployeeManagement.Models
{
    public class UserRole
    {
        public int UserRoleID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public DateTime AssignedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}