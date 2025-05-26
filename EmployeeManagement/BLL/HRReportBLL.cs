using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.BLL
{
    public class HRReportBLL
    {
        private readonly HRReportDAL hrReportDAL;

        public HRReportBLL()
        {
            hrReportDAL = new HRReportDAL();
        }

        /// <summary>
        /// Lấy thống kê tổng quan nhân sự
        /// </summary>
        public HRStatistics GetHRStatistics()
        {
            try
            {
                var stats = hrReportDAL.GetHRStatistics();

                Logger.LogInfo($"Lấy thống kê nhân sự: {stats.TotalEmployees} nhân viên tổng cộng");

                return stats;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetHRStatistics: {ex.Message}");
                throw new Exception($"Không thể lấy thống kê nhân sự: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo nhân viên với filter và validation
        /// </summary>
        public List<EmployeeReportModel> GetEmployeeReports(HRReportFilter filter = null)
        {
            try
            {
                // Validate filter
                if (filter != null)
                {
                    ValidateFilter(filter);
                }

                var reports = hrReportDAL.GetEmployeeReports(filter);

                // Process additional business logic
                foreach (var report in reports)
                {
                    ProcessEmployeeReport(report);
                }

                Logger.LogInfo($"Lấy báo cáo nhân viên: {reports.Count} nhân viên");

                return reports;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetEmployeeReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo nhân viên: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo theo phòng ban
        /// </summary>
        public List<DepartmentReportModel> GetDepartmentReports()
        {
            try
            {
                var reports = hrReportDAL.GetDepartmentReports();

                // Add additional calculations
                foreach (var dept in reports)
                {
                    ProcessDepartmentReport(dept);
                }

                Logger.LogInfo($"Lấy báo cáo phòng ban: {reports.Count} phòng ban");

                return reports;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetDepartmentReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo phòng ban: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo sinh nhật tháng
        /// </summary>
        public List<BirthdayReportModel> GetBirthdayReports(int month, int year)
        {
            try
            {
                // Validate inputs
                if (month < 1 || month > 12)
                    throw new ArgumentException("Tháng phải từ 1 đến 12");

                if (year < 1900 || year > DateTime.Now.Year + 1)
                    throw new ArgumentException("Năm không hợp lệ");

                var birthdays = hrReportDAL.GetBirthdayReports(month, year);

                Logger.LogInfo($"Lấy danh sách sinh nhật tháng {month}/{year}: {birthdays.Count} nhân viên");

                return birthdays.OrderBy(b => b.DaysUntilBirthday).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetBirthdayReports: {ex.Message}");
                throw new Exception($"Không thể lấy danh sách sinh nhật: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo theo độ tuổi
        /// </summary>
        public List<AgeGroupReportModel> GetAgeGroupReports()
        {
            try
            {
                var ageGroups = hrReportDAL.GetAgeGroupReports();

                Logger.LogInfo($"Lấy báo cáo theo độ tuổi: {ageGroups.Count} nhóm tuổi");

                return ageGroups;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetAgeGroupReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo theo độ tuổi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy top nhân viên theo tiêu chí
        /// </summary>
        public List<EmployeeReportModel> GetTopEmployees(string criteria, int topCount = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(criteria))
                    throw new ArgumentException("Tiêu chí không được để trống");

                if (topCount <= 0 || topCount > 50)
                    throw new ArgumentException("Số lượng top phải từ 1 đến 50");

                var allEmployees = GetEmployeeReports();

                var topEmployees = criteria.ToLower() switch
                {
                    "salary" => allEmployees.OrderByDescending(e => e.CurrentSalary).Take(topCount).ToList(),
                    "performance" => allEmployees.OrderByDescending(e => e.PerformanceScore).Take(topCount).ToList(),
                    "projects" => allEmployees.OrderByDescending(e => e.TotalProjects).Take(topCount).ToList(),
                    "attendance" => allEmployees.OrderByDescending(e => e.AttendanceRate).Take(topCount).ToList(),
                    "experience" => allEmployees.OrderByDescending(e => e.WorkingDays).Take(topCount).ToList(),
                    _ => throw new ArgumentException($"Tiêu chí '{criteria}' không được hỗ trợ")
                };

                Logger.LogInfo($"Lấy top {topCount} nhân viên theo {criteria}: {topEmployees.Count} kết quả");

                return topEmployees;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetTopEmployees: {ex.Message}");
                throw new Exception($"Không thể lấy top nhân viên: {ex.Message}");
            }
        }

        /// <summary>
        /// Xuất báo cáo ra Excel
        /// </summary>
        public bool ExportToExcel(List<EmployeeReportModel> employees, string filePath)
        {
            try
            {
                var excelExporter = new ExcelExport();

                var exportData = employees.Select(e => new
                {
                    MaNV = e.EmployeeCode,
                    HoTen = e.FullName,
                    GioiTinh = e.Gender,
                    Tuoi = e.Age,
                    PhongBan = e.Department,
                    ChucVu = e.Position,
                    NgayVaoLam = e.HireDate.ToString("dd/MM/yyyy"),
                    SoNamLamViec = e.WorkingYears,
                    TrangThai = e.Status,
                    Luong = e.CurrentSalary.ToString("N0"),
                    SoDienThoai = e.Phone,
                    Email = e.Email,
                    SoDuAn = e.TotalProjects,
                    DuAnHoanThanh = e.CompletedProjects,
                    TyLeChamCong = $"{e.AttendanceRate:F1}%",
                    DiemHieuSuat = $"{e.PerformanceScore:F1}"
                }).ToList();

                bool result = excelExporter.ExportToExcel(exportData, filePath, "Báo cáo Nhân sự");

                if (result)
                {
                    Logger.LogInfo($"Xuất Excel báo cáo nhân sự thành công: {filePath}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - ExportToExcel: {ex.Message}");
                throw new Exception($"Không thể xuất file Excel: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính toán dashboard metrics cho nhân sự
        /// </summary>
        public Dictionary<string, object> GetDashboardMetrics()
        {
            try
            {
                var stats = GetHRStatistics();
                var employees = GetEmployeeReports();
                var departments = GetDepartmentReports();
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var birthdays = GetBirthdayReports(currentMonth, currentYear);

                var metrics = new Dictionary<string, object>
                {
                    ["TotalEmployees"] = stats.TotalEmployees,
                    ["ActiveEmployees"] = stats.ActiveEmployees,
                    ["NewHires"] = stats.NewHires,
                    ["TurnoverRate"] = stats.TurnoverRate,
                    ["AverageAge"] = stats.AverageAge,
                    ["AverageSalary"] = stats.AverageSalary,
                    ["AverageAttendanceRate"] = stats.AverageAttendanceRate,
                    ["GenderRatio"] = stats.GenderRatio,

                    // Top performers
                    ["TopPerformers"] = GetTopEmployees("performance", 5),
                    ["TopEarners"] = GetTopEmployees("salary", 5),
                    ["HighestAttendance"] = GetTopEmployees("attendance", 5),

                    // Department breakdown
                    ["DepartmentBreakdown"] = departments.Take(5).ToList(),

                    // Upcoming birthdays
                    ["UpcomingBirthdays"] = birthdays.Take(10).ToList(),

                    // Age distribution
                    ["AgeDistribution"] = GetAgeGroupReports(),

                    // Recent activities
                    ["RecentHires"] = employees.Where(e => e.WorkingDays <= 90).Take(5).ToList(),
                    ["LongTermEmployees"] = employees.Where(e => e.WorkingYears >= 5).Take(5).ToList()
                };

                return metrics;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetDashboardMetrics: {ex.Message}");
                throw new Exception($"Không thể tính toán dashboard metrics: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo chấm công tháng
        /// </summary>
        public List<AttendanceReportModel> GetAttendanceReports(int month, int year)
        {
            try
            {
                // Mock data for attendance report
                var employees = GetEmployeeReports();
                var attendanceReports = new List<AttendanceReportModel>();

                foreach (var emp in employees.Where(e => e.Status == "Đang làm việc"))
                {
                    var workingDays = GetWorkingDaysInMonth(month, year);
                    var presentDays = (int)(workingDays * emp.AttendanceRate / 100);
                    var absentDays = workingDays - presentDays;

                    attendanceReports.Add(new AttendanceReportModel
                    {
                        EmployeeID = emp.EmployeeID,
                        EmployeeCode = emp.EmployeeCode,
                        FullName = emp.FullName,
                        Department = emp.Department,
                        WorkingDays = workingDays,
                        PresentDays = presentDays,
                        AbsentDays = absentDays,
                        LateDays = Math.Max(0, absentDays / 3),
                        TotalWorkingHours = presentDays * 8,
                        OvertimeHours = presentDays * 0.5m
                    });
                }

                Logger.LogInfo($"Lấy báo cáo chấm công tháng {month}/{year}: {attendanceReports.Count} nhân viên");

                return attendanceReports.OrderBy(a => a.FullName).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi BLL - GetAttendanceReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo chấm công: {ex.Message}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Validate filter input
        /// </summary>
        private void ValidateFilter(HRReportFilter filter)
        {
            if (filter.MinAge.HasValue && filter.MinAge < 0)
                throw new ArgumentException("Tuổi tối thiểu không thể âm");

            if (filter.MaxAge.HasValue && filter.MaxAge > 100)
                throw new ArgumentException("Tuổi tối đa không hợp lệ");

            if (filter.MinAge.HasValue && filter.MaxAge.HasValue && filter.MinAge > filter.MaxAge)
                throw new ArgumentException("Tuổi tối thiểu không thể lớn hơn tuổi tối đa");

            if (filter.HireDateFrom.HasValue && filter.HireDateTo.HasValue && filter.HireDateFrom > filter.HireDateTo)
                throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");

            if (filter.MinSalary.HasValue && filter.MinSalary < 0)
                throw new ArgumentException("Lương tối thiểu không thể âm");

            if (filter.MaxSalary.HasValue && filter.MaxSalary < 0)
                throw new ArgumentException("Lương tối đa không thể âm");

            var validSortFields = new[] { "FullName", "Age", "HireDate", "CurrentSalary", "Department", "Position" };
            if (!string.IsNullOrEmpty(filter.SortBy) && !validSortFields.Contains(filter.SortBy))
                throw new ArgumentException($"Trường sắp xếp không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validSortFields)}");
        }

        /// <summary>
        /// Process additional business logic for employee report
        /// </summary>
        private void ProcessEmployeeReport(EmployeeReportModel report)
        {
            // Add business rules và calculations
            if (report.WorkingDays < 30 && report.Status == "Đang làm việc")
            {
                Logger.LogInfo($"Nhân viên mới: {report.FullName} ({report.WorkingDays} ngày làm việc)");
            }

            if (report.AttendanceRate < 80)
            {
                Logger.LogWarning($"Chấm công thấp: {report.FullName} ({report.AttendanceRate:F1}%)");
            }

            if (report.PerformanceScore >= 90)
            {
                Logger.LogInfo($"Hiệu suất cao: {report.FullName} ({report.PerformanceScore:F1} điểm)");
            }
        }

        /// <summary>
        /// Process additional business logic for department report
        /// </summary>
        private void ProcessDepartmentReport(DepartmentReportModel dept)
        {
            // Add business analysis
            if (dept.TotalEmployees == 0)
            {
                Logger.LogWarning($"Phòng ban {dept.DepartmentName} không có nhân viên");
            }

            if (dept.AverageAge > 50)
            {
                Logger.LogInfo($"Phòng ban {dept.DepartmentName} có độ tuổi trung bình cao ({dept.AverageAge:F1})");
            }
        }

        /// <summary>
        /// Tính số ngày làm việc trong tháng
        /// </summary>
        private int GetWorkingDaysInMonth(int month, int year)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var workingDays = 0;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        #endregion
    }
}