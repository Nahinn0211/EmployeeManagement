//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Models
//{
//    public class Attendance
//    {
//        public int AttendanceID { get; set; }
//        public int EmployeeID { get; set; }  // Foreign Key
//        public DateTime Date { get; set; }
//        public DateTime? TimeIn { get; set; }  // Giờ vào (nullable)
//        public DateTime? TimeOut { get; set; }  // Giờ ra (nullable)
//        public string Status { get; set; }  // Present, Absent, Late, Leave
//        public string Note { get; set; }
//        public string ImagePathIn { get; set; }  // Đường dẫn ảnh khuôn mặt lúc vào
//        public string ImagePathOut { get; set; }  // Đường dẫn ảnh khuôn mặt lúc ra
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        // Foreign Key References
//        public virtual Employee Employee { get; set; }
//    }
//}


using System;

namespace EmployeeManagement.Models
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