using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Trong thực tế, nên lưu mật khẩu đã được hash
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }

 
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public string Token { get; set; } // Để mở rộng sau này
    }
}