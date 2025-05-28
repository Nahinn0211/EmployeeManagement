using System;

namespace EmployeeManagement.Models.Entity
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string CheckInMethod { get; set; }
        public string CheckInImage { get; set; }
        public decimal WorkingHours { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
    }
}