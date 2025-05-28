using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.Entity
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public List<User> Users { get; set; } = new List<User>();
    }
}