using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Utilities;
using System.Data;
using EmployeeManagement.GUI.Attendance;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.DAL
{
    public class AttendanceDAL
    {
        /// <summary>
        /// Lấy connection string từ app.config
        /// </summary>
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        /// <summary>
        /// Thêm bản ghi chấm công mới
        /// </summary>
        public bool Insert(Attendance attendance)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Attendance (EmployeeID, CheckInTime, CheckOutTime, CheckInMethod, 
                                              CheckInImage, WorkingHours, Status, Notes, CreatedAt)
                        VALUES (@EmployeeID, @CheckInTime, @CheckOutTime, @CheckInMethod, 
                                @CheckInImage, @WorkingHours, @Status, @Notes, @CreatedAt)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", attendance.EmployeeID);
                    command.Parameters.AddWithValue("@CheckInTime", attendance.CheckInTime);
                    command.Parameters.AddWithValue("@CheckOutTime", (object)attendance.CheckOutTime ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CheckInMethod", attendance.CheckInMethod ?? "");
                    command.Parameters.AddWithValue("@CheckInImage", attendance.CheckInImage ?? "");
                    command.Parameters.AddWithValue("@WorkingHours", attendance.WorkingHours);
                    command.Parameters.AddWithValue("@Status", attendance.Status ?? "");
                    command.Parameters.AddWithValue("@Notes", attendance.Notes ?? "");
                    command.Parameters.AddWithValue("@CreatedAt", attendance.CreatedAt);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.Insert: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật bản ghi chấm công
        /// </summary>
        public bool Update(Attendance attendance)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Attendance SET
                            CheckInTime = @CheckInTime,
                            CheckOutTime = @CheckOutTime,
                            CheckInMethod = @CheckInMethod,
                            CheckInImage = @CheckInImage,
                            WorkingHours = @WorkingHours,
                            Status = @Status,
                            Notes = @Notes
                        WHERE AttendanceID = @AttendanceID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AttendanceID", attendance.AttendanceID);
                    command.Parameters.AddWithValue("@CheckInTime", attendance.CheckInTime);
                    command.Parameters.AddWithValue("@CheckOutTime", (object)attendance.CheckOutTime ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CheckInMethod", attendance.CheckInMethod ?? "");
                    command.Parameters.AddWithValue("@CheckInImage", attendance.CheckInImage ?? "");
                    command.Parameters.AddWithValue("@WorkingHours", attendance.WorkingHours);
                    command.Parameters.AddWithValue("@Status", attendance.Status ?? "");
                    command.Parameters.AddWithValue("@Notes", attendance.Notes ?? "");

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.Update: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy chấm công hôm nay của nhân viên
        /// </summary>
        public Attendance GetEmployeeTodayAttendance(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT * FROM Attendance 
                        WHERE EmployeeID = @EmployeeID 
                        AND CAST(CheckInTime AS DATE) = CAST(GETDATE() AS DATE)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToAttendance(reader);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetEmployeeTodayAttendance: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy tất cả chấm công hôm nay
        /// </summary>
        public List<Attendance> GetTodayAttendance()
        {
            try
            {
                var attendances = new List<Attendance>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT * FROM Attendance 
                        WHERE CAST(CheckInTime AS DATE) = CAST(GETDATE() AS DATE)
                        ORDER BY CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(MapReaderToAttendance(reader));
                        }
                    }
                }
                return attendances;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetTodayAttendance: {ex.Message}");
                return new List<Attendance>();
            }
        }

        /// <summary>
        /// Lấy lịch sử chấm công của nhân viên
        /// </summary>
        public List<Attendance> GetEmployeeAttendanceHistory(int employeeId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var attendances = new List<Attendance>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT * FROM Attendance 
                        WHERE EmployeeID = @EmployeeID";

                    if (fromDate.HasValue)
                        query += " AND CheckInTime >= @FromDate";
                    if (toDate.HasValue)
                        query += " AND CheckInTime <= @ToDate";

                    query += " ORDER BY CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    if (fromDate.HasValue)
                        command.Parameters.AddWithValue("@FromDate", fromDate.Value);
                    if (toDate.HasValue)
                        command.Parameters.AddWithValue("@ToDate", toDate.Value.AddDays(1));

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(MapReaderToAttendance(reader));
                        }
                    }
                }
                return attendances;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetEmployeeAttendanceHistory: {ex.Message}");
                return new List<Attendance>();
            }
        }

        /// <summary>
        /// Lấy tất cả chấm công trong khoảng thời gian
        /// </summary>
        public List<Attendance> GetAllAttendanceInDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendances = new List<Attendance>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT * FROM Attendance 
                        WHERE CheckInTime >= @FromDate AND CheckInTime <= @ToDate
                        ORDER BY CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate.AddDays(1));

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(MapReaderToAttendance(reader));
                        }
                    }
                }
                return attendances;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetAllAttendanceInDateRange: {ex.Message}");
                return new List<Attendance>();
            }
        }

        /// <summary>
        /// Lấy chấm công theo khoảng thời gian
        /// </summary>
        public List<Attendance> GetAttendanceByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendances = new List<Attendance>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT * FROM Attendance 
                        WHERE CheckInTime >= @FromDate AND CheckInTime <= @ToDate
                        ORDER BY CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate.AddDays(1));

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(MapReaderToAttendance(reader));
                        }
                    }
                }
                return attendances;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetAttendanceByDateRange: {ex.Message}");
                return new List<Attendance>();
            }
        }

        /// <summary>
        /// Lấy thống kê chấm công theo ngày
        /// </summary>
        public AttendanceStatistics GetDailyAttendanceStatistics(DateTime date)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            COUNT(*) as TotalRecords,
                            COUNT(CASE WHEN CheckInTime <= DATEADD(HOUR, 8, CAST(@Date AS DATE)) THEN 1 END) as OnTimeCount,
                            COUNT(CASE WHEN CheckInTime > DATEADD(HOUR, 8, CAST(@Date AS DATE)) THEN 1 END) as LateCount,
                            COUNT(CASE WHEN CheckOutTime IS NOT NULL AND CheckOutTime < DATEADD(HOUR, 17, CAST(@Date AS DATE)) THEN 1 END) as EarlyCheckoutCount,
                            AVG(CAST(WorkingHours AS FLOAT)) as AvgWorkingHours,
                            SUM(CAST(WorkingHours AS FLOAT)) as TotalWorkingHours
                        FROM Attendance 
                        WHERE CAST(CheckInTime AS DATE) = @Date";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Date", date.Date);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AttendanceStatistics
                            {
                                TotalRecords = reader.GetInt32("TotalRecords"),
                                OnTimeCheckins = reader.GetInt32("OnTimeCount"),
                                LateCheckins = reader.GetInt32("LateCount"),
                                EarlyCheckouts = reader.GetInt32("EarlyCheckoutCount"),
                                AverageWorkingHours = reader.IsDBNull("AvgWorkingHours") ? 0 : reader.GetDouble("AvgWorkingHours"),
                                TotalWorkingHours = reader.IsDBNull("TotalWorkingHours") ? 0 : reader.GetDouble("TotalWorkingHours")
                            };
                        }
                    }
                }
                return new AttendanceStatistics();
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetDailyAttendanceStatistics: {ex.Message}");
                return new AttendanceStatistics();
            }
        }

        /// <summary>
        /// Kiểm tra nhân viên đã chấm công hôm nay chưa
        /// </summary>
        public bool HasCheckedInToday(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT COUNT(*) FROM Attendance 
                        WHERE EmployeeID = @EmployeeID 
                        AND CAST(CheckInTime AS DATE) = CAST(GETDATE() AS DATE)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.HasCheckedInToday: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy nhân viên chưa chấm công hôm nay
        /// </summary>
        public List<int> GetEmployeesNotCheckedInToday()
        {
            try
            {
                var employeeIds = new List<int>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID 
                        FROM Employees e
                        WHERE e.Status = N'Đang làm việc'
                        AND e.EmployeeID NOT IN (
                            SELECT DISTINCT EmployeeID 
                            FROM Attendance 
                            WHERE CAST(CheckInTime AS DATE) = CAST(GETDATE() AS DATE)
                        )";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employeeIds.Add(reader.GetInt32("EmployeeID"));
                        }
                    }
                }
                return employeeIds;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetEmployeesNotCheckedInToday: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Lấy thống kê chấm công tháng cho báo cáo
        /// </summary>
        public List<MonthlyAttendanceStats> GetMonthlyAttendanceStats(int month, int year)
        {
            try
            {
                var stats = new List<MonthlyAttendanceStats>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            e.EmployeeID,
                            e.EmployeeCode,
                            e.FullName,
                            d.DepartmentName,
                            COUNT(a.AttendanceID) as PresentDays,
                            COUNT(CASE WHEN CAST(a.CheckInTime AS TIME) > '08:30:00' THEN 1 END) as LateDays,
                            SUM(CASE WHEN a.WorkingHours IS NOT NULL THEN a.WorkingHours ELSE 0 END) as TotalWorkingHours,
                            SUM(CASE WHEN a.WorkingHours > 8 THEN a.WorkingHours - 8 ELSE 0 END) as OvertimeHours
                        FROM Employees e
                        LEFT JOIN Attendance a ON e.EmployeeID = a.EmployeeID 
                            AND MONTH(a.CheckInTime) = @Month 
                            AND YEAR(a.CheckInTime) = @Year
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        WHERE e.Status = N'Đang làm việc'
                        GROUP BY e.EmployeeID, e.EmployeeCode, e.FullName, d.DepartmentName
                        ORDER BY e.EmployeeCode";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int workingDays = GetWorkingDaysInMonth(month, year);
                            int presentDays = reader.GetInt32("PresentDays");
                            int absentDays = workingDays - presentDays;

                            stats.Add(new MonthlyAttendanceStats
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                EmployeeCode = reader.GetString("EmployeeCode"),
                                FullName = reader.GetString("FullName"),
                                Department = reader.IsDBNull("DepartmentName") ? "Chưa phân bổ" : reader.GetString("DepartmentName"),
                                WorkingDays = workingDays,
                                PresentDays = presentDays,
                                AbsentDays = absentDays,
                                LateDays = reader.GetInt32("LateDays"),
                                TotalWorkingHours = reader.GetDecimal("TotalWorkingHours"),
                                OvertimeHours = reader.GetDecimal("OvertimeHours")
                            });
                        }
                    }
                }
                return stats;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetMonthlyAttendanceStats: {ex.Message}");
                return new List<MonthlyAttendanceStats>();
            }
        }

        /// <summary>
        /// Xóa bản ghi chấm công
        /// </summary>
        public bool Delete(int attendanceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM Attendance WHERE AttendanceID = @AttendanceID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AttendanceID", attendanceId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.Delete: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy bản ghi chấm công theo ID
        /// </summary>
        public Attendance GetById(int attendanceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM Attendance WHERE AttendanceID = @AttendanceID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AttendanceID", attendanceId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToAttendance(reader);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetById: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tính số ngày làm việc trong tháng (trừ cuối tuần)
        /// </summary>
        private static int GetWorkingDaysInMonth(int month, int year)
        {
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            int workingDays = 0;

            for (var date = firstDay; date <= lastDay; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        /// <summary>
        /// Map SqlDataReader to Attendance object
        /// </summary>
        private static Attendance MapReaderToAttendance(SqlDataReader reader)
        {
            return new Attendance
            {
                AttendanceID = reader.GetInt32("AttendanceID"),
                EmployeeID = reader.GetInt32("EmployeeID"),
                CheckInTime = reader.GetDateTime("CheckInTime"),
                CheckOutTime = reader.IsDBNull("CheckOutTime") ? null : reader.GetDateTime("CheckOutTime"),
                CheckInMethod = reader.IsDBNull("CheckInMethod") ? "" : reader.GetString("CheckInMethod"),
                CheckInImage = reader.IsDBNull("CheckInImage") ? "" : reader.GetString("CheckInImage"),
                WorkingHours = reader.IsDBNull("WorkingHours") ? 0 : reader.GetDecimal("WorkingHours"),
                Status = reader.IsDBNull("Status") ? "" : reader.GetString("Status"),
                Notes = reader.IsDBNull("Notes") ? "" : reader.GetString("Notes"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };
        }


        /// <summary>
        /// Cập nhật đường dẫn dữ liệu khuôn mặt cho nhân viên
        /// </summary>
        public bool UpdateEmployeeFaceData(int employeeId, string faceDataPath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                UPDATE Employees 
                SET FaceDataPath = @FaceDataPath, UpdatedAt = GETDATE()
                WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@FaceDataPath", (object)faceDataPath ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.UpdateEmployeeFaceData: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy báo cáo chấm công hàng ngày
        /// </summary>
        public List<DailyAttendanceReportItem> GetDailyAttendanceReport(AttendanceSearchCriteria criteria)
        {
            try
            {
                var reportItems = new List<DailyAttendanceReportItem>();

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    e.EmployeeCode,
                    e.FullName as EmployeeName,
                    d.DepartmentName,
                    a.CheckInTime,
                    a.CheckOutTime,
                    a.WorkingHours,
                    a.Status,
                    a.CheckInMethod,
                    a.Notes
                FROM Attendance a
                INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE a.CheckInTime >= @FromDate AND a.CheckInTime <= @ToDate";

                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", criteria.FromDate),
                new SqlParameter("@ToDate", criteria.ToDate)
            };

                    if (criteria.DepartmentId.HasValue)
                    {
                        query += " AND e.DepartmentID = @DepartmentId";
                        parameters.Add(new SqlParameter("@DepartmentId", criteria.DepartmentId.Value));
                    }

                    if (criteria.EmployeeId.HasValue)
                    {
                        query += " AND e.EmployeeID = @EmployeeId";
                        parameters.Add(new SqlParameter("@EmployeeId", criteria.EmployeeId.Value));
                    }

                    if (!string.IsNullOrEmpty(criteria.Status))
                    {
                        query += " AND a.Status = @Status";
                        parameters.Add(new SqlParameter("@Status", criteria.Status));
                    }

                    query += " ORDER BY a.CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportItems.Add(new DailyAttendanceReportItem
                            {
                                EmployeeCode = reader.GetString("EmployeeCode"),
                                EmployeeName = reader.GetString("EmployeeName"),
                                DepartmentName = reader.IsDBNull("DepartmentName") ? "Chưa phân bổ" : reader.GetString("DepartmentName"),
                                CheckInTime = reader.GetDateTime("CheckInTime"),
                                CheckOutTime = reader.IsDBNull("CheckOutTime") ? null : reader.GetDateTime("CheckOutTime"),
                                WorkingHours = reader.GetDecimal("WorkingHours"),
                                Status = reader.GetString("Status"),
                                CheckInMethod = reader.IsDBNull("CheckInMethod") ? "" : reader.GetString("CheckInMethod"),
                                Notes = reader.IsDBNull("Notes") ? "" : reader.GetString("Notes")
                            });
                        }
                    }
                }

                return reportItems;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetDailyAttendanceReport: {ex.Message}");
                return new List<DailyAttendanceReportItem>();
            }
        }

        /// <summary>
        /// Lấy báo cáo tổng hợp chấm công
        /// </summary>
        public List<AttendanceSummaryReportItem> GetAttendanceSummaryReport(AttendanceSearchCriteria criteria)
        {
            try
            {
                var reportItems = new List<AttendanceSummaryReportItem>();

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    e.EmployeeCode,
                    e.FullName as EmployeeName,
                    d.DepartmentName,
                    COUNT(DISTINCT CAST(a.CheckInTime AS DATE)) as PresentDays,
                    COUNT(CASE WHEN CAST(a.CheckInTime AS TIME) > '08:30:00' THEN 1 END) as LateDays,
                    SUM(a.WorkingHours) as TotalWorkingHours,
                    AVG(a.WorkingHours) as AverageWorkingHours
                FROM Employees e
                LEFT JOIN Attendance a ON e.EmployeeID = a.EmployeeID 
                    AND a.CheckInTime >= @FromDate AND a.CheckInTime <= @ToDate
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE e.Status = N'Đang làm việc'";

                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", criteria.FromDate),
                new SqlParameter("@ToDate", criteria.ToDate)
            };

                    if (criteria.DepartmentId.HasValue)
                    {
                        query += " AND e.DepartmentID = @DepartmentId";
                        parameters.Add(new SqlParameter("@DepartmentId", criteria.DepartmentId.Value));
                    }

                    if (criteria.EmployeeId.HasValue)
                    {
                        query += " AND e.EmployeeID = @EmployeeId";
                        parameters.Add(new SqlParameter("@EmployeeId", criteria.EmployeeId.Value));
                    }

                    query += @" 
                GROUP BY e.EmployeeCode, e.FullName, d.DepartmentName
                ORDER BY e.EmployeeCode";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int totalDays = GetWorkingDaysInDateRange(criteria.FromDate, criteria.ToDate);
                            int presentDays = reader.IsDBNull("PresentDays") ? 0 : reader.GetInt32("PresentDays");
                            int absentDays = Math.Max(0, totalDays - presentDays);

                            reportItems.Add(new AttendanceSummaryReportItem
                            {
                                EmployeeCode = reader.GetString("EmployeeCode"),
                                EmployeeName = reader.GetString("EmployeeName"),
                                DepartmentName = reader.IsDBNull("DepartmentName") ? "Chưa phân bổ" : reader.GetString("DepartmentName"),
                                TotalDays = totalDays,
                                PresentDays = presentDays,
                                AbsentDays = absentDays,
                                LateDays = reader.IsDBNull("LateDays") ? 0 : reader.GetInt32("LateDays"),
                                TotalWorkingHours = reader.IsDBNull("TotalWorkingHours") ? 0 : reader.GetDecimal("TotalWorkingHours"),
                                AverageWorkingHours = reader.IsDBNull("AverageWorkingHours") ? 0 : reader.GetDecimal("AverageWorkingHours"),
                                AttendanceRate = presentDays > 0 ? (decimal)presentDays / totalDays * 100 : 0
                            });
                        }
                    }
                }

                return reportItems;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetAttendanceSummaryReport: {ex.Message}");
                return new List<AttendanceSummaryReportItem>();
            }
        }

        /// <summary>
        /// Lấy báo cáo chấm công bằng nhận diện khuôn mặt
        /// </summary>
        public List<FaceRecognitionAttendanceReportItem> GetFaceRecognitionAttendanceReport(AttendanceSearchCriteria criteria)
        {
            try
            {
                var reportItems = new List<FaceRecognitionAttendanceReportItem>();

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    e.EmployeeCode,
                    e.FullName as EmployeeName,
                    d.DepartmentName,
                    a.CheckInTime,
                    a.CheckInImage as ImagePath,
                    a.Status,
                    95.5 as Confidence -- Placeholder for confidence score
                FROM Attendance a
                INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE a.CheckInMethod = 'Face Recognition'
                    AND a.CheckInTime >= @FromDate 
                    AND a.CheckInTime <= @ToDate";

                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", criteria.FromDate),
                new SqlParameter("@ToDate", criteria.ToDate)
            };

                    if (criteria.DepartmentId.HasValue)
                    {
                        query += " AND e.DepartmentID = @DepartmentId";
                        parameters.Add(new SqlParameter("@DepartmentId", criteria.DepartmentId.Value));
                    }

                    if (criteria.EmployeeId.HasValue)
                    {
                        query += " AND e.EmployeeID = @EmployeeId";
                        parameters.Add(new SqlParameter("@EmployeeId", criteria.EmployeeId.Value));
                    }

                    query += " ORDER BY a.CheckInTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportItems.Add(new FaceRecognitionAttendanceReportItem
                            {
                                EmployeeCode = reader.GetString("EmployeeCode"),
                                EmployeeName = reader.GetString("EmployeeName"),
                                DepartmentName = reader.IsDBNull("DepartmentName") ? "Chưa phân bổ" : reader.GetString("DepartmentName"),
                                CheckInTime = reader.GetDateTime("CheckInTime"),
                                Confidence = reader.GetDecimal("Confidence"),
                                ImagePath = reader.IsDBNull("ImagePath") ? "" : reader.GetString("ImagePath"),
                                Status = reader.GetString("Status")
                            });
                        }
                    }
                }

                return reportItems;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetFaceRecognitionAttendanceReport: {ex.Message}");
                return new List<FaceRecognitionAttendanceReportItem>();
            }
        }

        /// <summary>
        /// Tính số ngày làm việc trong khoảng thời gian (trừ cuối tuần)
        /// </summary>
        private static int GetWorkingDaysInDateRange(DateTime fromDate, DateTime toDate)
        {
            int workingDays = 0;

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        /// <summary>
        /// Lấy nhân viên đã đăng ký khuôn mặt
        /// </summary>
        public List<EmployeeWithFaceData> GetEmployeesWithFaceData()
        {
            try
            {
                var employees = new List<EmployeeWithFaceData>();

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.FaceDataPath, 
                       d.DepartmentName, e.CreatedAt
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE e.FaceDataPath IS NOT NULL 
                    AND e.FaceDataPath != ''
                    AND e.Status = N'Đang làm việc'
                ORDER BY e.FullName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new EmployeeWithFaceData
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                EmployeeCode = reader.GetString("EmployeeCode"),
                                FullName = reader.GetString("FullName"),
                                FaceDataPath = reader.GetString("FaceDataPath"),
                                DepartmentName = reader.IsDBNull("DepartmentName") ? "Chưa phân bổ" : reader.GetString("DepartmentName"),
                                RegisterDate = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }

                return employees;
            }
            catch (Exception ex)
            {
                Logger.LogError($"AttendanceDAL.GetEmployeesWithFaceData: {ex.Message}");
                return new List<EmployeeWithFaceData>();
            }
        }

        // Support classes
        public class EmployeeWithFaceData
        {
            public int EmployeeID { get; set; }
            public string EmployeeCode { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string FaceDataPath { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public DateTime RegisterDate { get; set; }
        }

       
        /// <summary>
        /// Model cho thống kê chấm công theo tháng
        /// </summary>
        public class MonthlyAttendanceStats
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
            public decimal AverageWorkingHoursPerDay => PresentDays > 0 ? TotalWorkingHours / PresentDays : 0;
            public string AttendanceRateDisplay => $"{AttendanceRate:F1}%";
            public string TotalWorkingHoursDisplay => $"{TotalWorkingHours:F1}h";
            public string OvertimeHoursDisplay => $"{OvertimeHours:F1}h";
        }
    }
}