using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.BLL
{
    public class AttendanceBLL
    {
        private readonly AttendanceDAL attendanceDAL;
        private readonly EmployeeDAL employeeDAL;

        public AttendanceBLL()
        {
            attendanceDAL = new AttendanceDAL();
            employeeDAL = new EmployeeDAL();
        }

        /// <summary>
        /// Chấm công vào
        /// </summary>
        public bool CheckIn(Attendance attendance)
        {
            try
            {
                // Validate input
                if (attendance == null || attendance.EmployeeID <= 0)
                {
                    throw new ArgumentException("Thông tin chấm công không hợp lệ");
                }

                // Check if employee exists and is active
                var employee = employeeDAL.GetEmployeeById(attendance.EmployeeID);
                if (employee == null)
                {
                    throw new ArgumentException("Nhân viên không tồn tại");
                }

                if (employee.Status != "Đang làm việc")
                {
                    throw new ArgumentException("Nhân viên không trong trạng thái làm việc");
                }

                // Check if already checked in today
                var todayAttendance = GetEmployeeTodayAttendance(attendance.EmployeeID);
                if (todayAttendance != null)
                {
                    throw new ArgumentException("Nhân viên đã chấm công vào hôm nay");
                }

                // Set default values
                attendance.CheckInTime = DateTime.Now;
                attendance.Status = "Đã chấm công vào";
                attendance.CreatedAt = DateTime.Now;
                attendance.WorkingHours = 0; // Will be calculated when check out

                // Save to database
                return attendanceDAL.Insert(attendance);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in CheckIn: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Chấm công ra
        /// </summary>
        public bool CheckOut(int employeeId, DateTime checkOutTime)
        {
            try
            {
                // Get today's attendance record
                var todayAttendance = GetEmployeeTodayAttendance(employeeId);
                if (todayAttendance == null)
                {
                    throw new ArgumentException("Nhân viên chưa chấm công vào hôm nay");
                }

                if (todayAttendance.CheckOutTime.HasValue)
                {
                    throw new ArgumentException("Nhân viên đã chấm công ra hôm nay");
                }

                // Calculate working hours
                var workingHours = (checkOutTime - todayAttendance.CheckInTime).TotalHours;
                if (workingHours < 0)
                {
                    throw new ArgumentException("Thời gian chấm công ra không thể nhỏ hơn thời gian chấm công vào");
                }

                // Update attendance record
                todayAttendance.CheckOutTime = checkOutTime;
                todayAttendance.WorkingHours = (decimal)workingHours;

                // Determine status based on working hours
                if (workingHours >= 8)
                {
                    todayAttendance.Status = "Đủ giờ";
                }
                else if (workingHours >= 4)
                {
                    todayAttendance.Status = "Thiếu giờ";
                }
                else
                {
                    todayAttendance.Status = "Về sớm";
                }

                return attendanceDAL.Update(todayAttendance);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in CheckOut: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin chấm công hôm nay của nhân viên
        /// </summary>
        public Attendance GetEmployeeTodayAttendance(int employeeId)
        {
            try
            {
                return attendanceDAL.GetEmployeeTodayAttendance(employeeId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetEmployeeTodayAttendance: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách chấm công hôm nay
        /// </summary>
        public List<AttendanceDisplayModel> GetTodayAttendance()
        {
            try
            {
                var attendances = attendanceDAL.GetTodayAttendance();
                var displayModels = new List<AttendanceDisplayModel>();

                foreach (var attendance in attendances)
                {
                    var employee = employeeDAL.GetEmployeeById(attendance.EmployeeID);
                    if (employee != null)
                    {
                        displayModels.Add(new AttendanceDisplayModel
                        {
                            AttendanceID = attendance.AttendanceID,
                            EmployeeID = attendance.EmployeeID,
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeName = employee.FullName,
                            CheckInTime = attendance.CheckInTime,
                            CheckOutTime = attendance.CheckOutTime,
                            WorkingHours = attendance.WorkingHours,
                            Status = attendance.Status,
                            CheckInMethod = attendance.CheckInMethod,
                            Notes = attendance.Notes
                        });
                    }
                }

                return displayModels.OrderBy(x => x.CheckInTime).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetTodayAttendance: {ex.Message}");
                return new List<AttendanceDisplayModel>();
            }
        }

        /// <summary>
        /// Lấy lịch sử chấm công của nhân viên
        /// </summary>
        public List<AttendanceDisplayModel> GetEmployeeAttendanceHistory(int employeeId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendances = attendanceDAL.GetEmployeeAttendanceHistory(employeeId, fromDate, toDate);
                var displayModels = new List<AttendanceDisplayModel>();
                var employee = employeeDAL.GetEmployeeById(employeeId);

                if (employee != null)
                {
                    foreach (var attendance in attendances)
                    {
                        displayModels.Add(new AttendanceDisplayModel
                        {
                            AttendanceID = attendance.AttendanceID,
                            EmployeeID = attendance.EmployeeID,
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeName = employee.FullName,
                            CheckInTime = attendance.CheckInTime,
                            CheckOutTime = attendance.CheckOutTime,
                            WorkingHours = attendance.WorkingHours,
                            Status = attendance.Status,
                            CheckInMethod = attendance.CheckInMethod,
                            Notes = attendance.Notes
                        });
                    }
                }

                return displayModels.OrderByDescending(x => x.CheckInTime).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetEmployeeAttendanceHistory: {ex.Message}");
                return new List<AttendanceDisplayModel>();
            }
        }

        /// <summary>
        /// Lấy tất cả chấm công trong khoảng thời gian
        /// </summary>
        public List<AttendanceDisplayModel> GetAllAttendanceInDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendances = attendanceDAL.GetAllAttendanceInDateRange(fromDate, toDate);
                var displayModels = new List<AttendanceDisplayModel>();

                foreach (var attendance in attendances)
                {
                    var employee = employeeDAL.GetEmployeeById(attendance.EmployeeID);
                    if (employee != null)
                    {
                        displayModels.Add(new AttendanceDisplayModel
                        {
                            AttendanceID = attendance.AttendanceID,
                            EmployeeID = attendance.EmployeeID,
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeName = employee.FullName,
                            CheckInTime = attendance.CheckInTime,
                            CheckOutTime = attendance.CheckOutTime,
                            WorkingHours = attendance.WorkingHours,
                            Status = attendance.Status,
                            CheckInMethod = attendance.CheckInMethod,
                            Notes = attendance.Notes
                        });
                    }
                }

                return displayModels.OrderByDescending(x => x.CheckInTime).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetAllAttendanceInDateRange: {ex.Message}");
                return new List<AttendanceDisplayModel>();
            }
        }

        /// <summary>
        /// Lấy báo cáo chấm công theo tháng
        /// </summary>
        public List<AttendanceReportModel> GetMonthlyAttendanceReport(int month, int year)
        {
            try
            {
                var monthlyStats = attendanceDAL.GetMonthlyAttendanceStats(month, year);
                var reportModels = new List<AttendanceReportModel>();

                foreach (var stat in monthlyStats)
                {
                    reportModels.Add(new AttendanceReportModel
                    {
                        EmployeeID = stat.EmployeeID,
                        EmployeeCode = stat.EmployeeCode,
                        FullName = stat.FullName,
                        Department = stat.Department,
                        WorkingDays = stat.WorkingDays,
                        PresentDays = stat.PresentDays,
                        AbsentDays = stat.AbsentDays,
                        LateDays = stat.LateDays,
                        TotalWorkingHours = stat.TotalWorkingHours,
                        OvertimeHours = stat.OvertimeHours
                    });
                }

                return reportModels.OrderBy(r => r.FullName).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetMonthlyAttendanceReport: {ex.Message}");
                return new List<AttendanceReportModel>();
            }
        }

        /// <summary>
        /// Lấy thống kê chấm công
        /// </summary>
        public AttendanceStatistics GetAttendanceStatistics(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendances = attendanceDAL.GetAttendanceByDateRange(fromDate, toDate);
                var allActiveEmployees = employeeDAL.GetAllEmployees()
                    .Where(e => e.Status == "Đang làm việc").ToList();

                var statistics = new AttendanceStatistics
                {
                    TotalRecords = attendances.Count,
                    TotalEmployees = allActiveEmployees.Count,
                    PresentEmployees = attendances.GroupBy(a => a.EmployeeID).Count(),
                    OnTimeCheckins = attendances.Count(a => a.CheckInTime.TimeOfDay <= new TimeSpan(8, 0, 0)),
                    LateCheckins = attendances.Count(a => a.CheckInTime.TimeOfDay > new TimeSpan(8, 0, 0)),
                    EarlyCheckouts = attendances.Count(a => a.CheckOutTime.HasValue &&
                        a.CheckOutTime.Value.TimeOfDay < new TimeSpan(17, 0, 0)),
                    TotalWorkingHours = (double)attendances.Sum(a => a.WorkingHours),
                    AverageWorkingHours = attendances.Any() ? (double)attendances.Average(a => a.WorkingHours) : 0
                };

                statistics.AbsentEmployees = statistics.TotalEmployees - statistics.PresentEmployees;
                statistics.AttendanceRate = statistics.TotalEmployees > 0 ?
                    (decimal)statistics.PresentEmployees / statistics.TotalEmployees * 100 : 0;
                statistics.PunctualityRate = statistics.PresentEmployees > 0 ?
                    (decimal)statistics.OnTimeCheckins / statistics.PresentEmployees * 100 : 0;

                return statistics;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetAttendanceStatistics: {ex.Message}");
                return new AttendanceStatistics();
            }
        }

        /// <summary>
        /// Cập nhật bản ghi chấm công
        /// </summary>
        public bool UpdateAttendance(Attendance attendance)
        {
            try
            {
                if (attendance == null || attendance.AttendanceID <= 0)
                {
                    throw new ArgumentException("Thông tin chấm công không hợp lệ");
                }

                // Recalculate working hours if both check-in and check-out are set
                if (attendance.CheckOutTime.HasValue)
                {
                    var workingHours = (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalHours;
                    attendance.WorkingHours = (decimal)workingHours;
                }

                return attendanceDAL.Update(attendance);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in UpdateAttendance: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Xóa bản ghi chấm công
        /// </summary>
        public bool DeleteAttendance(int attendanceId)
        {
            try
            {
                return attendanceDAL.Delete(attendanceId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in DeleteAttendance: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem nhân viên có thể chấm công không
        /// </summary>
        public AttendanceValidationResult ValidateAttendance(int employeeId)
        {
            try
            {
                var result = new AttendanceValidationResult { IsValid = true };

                // Check if employee exists
                var employee = employeeDAL.GetEmployeeById(employeeId);
                if (employee == null)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Nhân viên không tồn tại";
                    return result;
                }

                // Check if employee is active
                if (employee.Status != "Đang làm việc")
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Nhân viên không trong trạng thái làm việc";
                    return result;
                }

                // Check attendance for today
                var todayAttendance = GetEmployeeTodayAttendance(employeeId);
                if (todayAttendance != null)
                {
                    result.HasCheckedIn = true;
                    result.CheckInTime = todayAttendance.CheckInTime;

                    if (todayAttendance.CheckOutTime.HasValue)
                    {
                        result.HasCheckedOut = true;
                        result.CheckOutTime = todayAttendance.CheckOutTime;
                        result.WorkingHours = todayAttendance.WorkingHours;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in ValidateAttendance: {ex.Message}");
                return new AttendanceValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Lỗi kiểm tra thông tin chấm công"
                };
            }
        }
    }

    /// <summary>
    /// Model hiển thị chấm công
    /// </summary>
    public class AttendanceDisplayModel
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal WorkingHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CheckInMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Display properties
        public string CheckInTimeDisplay => CheckInTime.ToString("HH:mm:ss");
        public string CheckOutTimeDisplay => CheckOutTime?.ToString("HH:mm:ss") ?? "--:--:--";
        public string WorkingHoursDisplay => $"{WorkingHours:F1}h";
        public string StatusDisplay => GetStatusDisplay(Status);

        private static string GetStatusDisplay(string status)
        {
            return status switch
            {
                "Đã chấm công vào" => "⏰ Đã vào",
                "Đủ giờ" => "✅ Đủ giờ",
                "Thiếu giờ" => "⚠️ Thiếu giờ",
                "Về sớm" => "🔴 Về sớm",
                "Đã chấm công ra" => "🏁 Đã ra",
                _ => status
            };
        }
    }

    /// <summary>
    /// Model báo cáo chấm công
    /// </summary>
    public class AttendanceReportModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public decimal TotalWorkingHours { get; set; }
        public decimal OvertimeHours { get; set; }

        // Calculated properties
        public decimal AttendanceRate => WorkingDays > 0 ? (decimal)PresentDays / WorkingDays * 100 : 0;
        public decimal AbsenceRate => WorkingDays > 0 ? (decimal)AbsentDays / WorkingDays * 100 : 0;
        public decimal AverageWorkingHoursPerDay => PresentDays > 0 ? TotalWorkingHours / PresentDays : 0;
        public string AttendanceRateDisplay => $"{AttendanceRate:F1}%";
        public string TotalWorkingHoursDisplay => $"{TotalWorkingHours:F1}h";
        public string OvertimeHoursDisplay => $"{OvertimeHours:F1}h";
    }

    /// <summary>
    /// Kết quả kiểm tra chấm công
    /// </summary>
    public class AttendanceValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasCheckedIn { get; set; }
        public bool HasCheckedOut { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal WorkingHours { get; set; }
    }
}