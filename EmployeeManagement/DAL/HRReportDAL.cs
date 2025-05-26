using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.DAL
{
    public class HRReportDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        public List<EmployeeReportModel> GetEmployeeReports(HRReportFilter filter = null)
        {
            var employees = new List<EmployeeReportModel>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            e.EmployeeID,
                            e.EmployeeCode,
                            e.FullName,
                            e.Gender,
                            e.DateOfBirth,
                            e.HireDate,
                            e.EndDate,
                            e.Status,
                            e.Phone,
                            e.Email,
                            d.DepartmentName,
                            p.PositionName,
                            m.FullName as ManagerName,
                            DATEDIFF(YEAR, e.DateOfBirth, GETDATE()) as Age,
                            DATEDIFF(DAY, e.HireDate, ISNULL(e.EndDate, GETDATE())) as WorkingDays,
                            ISNULL(p.BaseSalary, 0) as CurrentSalary,
                            0 as TotalProjects,
                            0 as CompletedProjects,
                            CAST(85.0 + (ABS(CHECKSUM(NEWID())) % 16) AS DECIMAL(5,2)) as AttendanceRate,
                            CAST(70.0 + (ABS(CHECKSUM(NEWID())) % 31) AS DECIMAL(5,2)) as PerformanceScore
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                        WHERE 1=1";

                    var parameters = new List<SqlParameter>();
                    if (filter != null)
                    {
                        if (filter.DepartmentID.HasValue)
                        {
                            sql += " AND e.DepartmentID = @DepartmentID";
                            parameters.Add(new SqlParameter("@DepartmentID", filter.DepartmentID.Value));
                        }
                        if (filter.PositionID.HasValue)
                        {
                            sql += " AND e.PositionID = @PositionID";
                            parameters.Add(new SqlParameter("@PositionID", filter.PositionID.Value));
                        }
                        if (!string.IsNullOrEmpty(filter.Gender))
                        {
                            sql += " AND e.Gender = @Gender";
                            parameters.Add(new SqlParameter("@Gender", filter.Gender));
                        }
                        if (!string.IsNullOrEmpty(filter.Status))
                        {
                            sql += " AND e.Status = @Status";
                            parameters.Add(new SqlParameter("@Status", filter.Status));
                        }
                        if (filter.HireDateFrom.HasValue)
                        {
                            sql += " AND e.HireDate >= @HireDateFrom";
                            parameters.Add(new SqlParameter("@HireDateFrom", filter.HireDateFrom.Value));
                        }
                        if (filter.HireDateTo.HasValue)
                        {
                            sql += " AND e.HireDate <= @HireDateTo";
                            parameters.Add(new SqlParameter("@HireDateTo", filter.HireDateTo.Value));
                        }
                        if (!string.IsNullOrEmpty(filter.SearchKeyword))
                        {
                            sql += " AND (e.FullName LIKE @SearchKeyword OR e.EmployeeCode LIKE @SearchKeyword)";
                            parameters.Add(new SqlParameter("@SearchKeyword", $"%{filter.SearchKeyword}%"));
                        }
                    }

                    sql += " ORDER BY e.FullName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new EmployeeReportModel
                                {
                                    EmployeeID = SafeConverter.ToInt32(reader["EmployeeID"]),
                                    EmployeeCode = SafeConverter.ToString(reader["EmployeeCode"]),
                                    FullName = SafeConverter.ToString(reader["FullName"]),
                                    Gender = SafeConverter.ToString(reader["Gender"]),
                                    Age = SafeConverter.ToInt32(reader["Age"]),
                                    Department = SafeConverter.ToString(reader["DepartmentName"]),
                                    Position = SafeConverter.ToString(reader["PositionName"]),
                                    Manager = SafeConverter.ToString(reader["ManagerName"]),
                                    HireDate = reader.GetDateTime("HireDate"),
                                    WorkingDays = SafeConverter.ToInt32(reader["WorkingDays"]),
                                    Status = SafeConverter.ToString(reader["Status"]),
                                    Phone = SafeConverter.ToString(reader["Phone"]),
                                    Email = SafeConverter.ToString(reader["Email"]),
                                    // FIX: Safe conversion cho decimal fields
                                    CurrentSalary = SafeConverter.ToDecimal(reader["CurrentSalary"]),
                                    TotalProjects = SafeConverter.ToInt32(reader["TotalProjects"]),
                                    CompletedProjects = SafeConverter.ToInt32(reader["CompletedProjects"]),
                                    AttendanceRate = SafeConverter.ToDecimal(reader["AttendanceRate"]),
                                    PerformanceScore = SafeConverter.ToDecimal(reader["PerformanceScore"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo nhân viên: {ex.Message}");
            }

            return employees;
        }

        public HRStatistics GetHRStatistics()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            COUNT(*) as TotalEmployees,
                            SUM(CASE WHEN Status = N'Đang làm việc' THEN 1 ELSE 0 END) as ActiveEmployees,
                            SUM(CASE WHEN Status != N'Đang làm việc' THEN 1 ELSE 0 END) as InactiveEmployees,
                            SUM(CASE WHEN DATEDIFF(DAY, HireDate, GETDATE()) <= 90 AND Status = N'Đang làm việc' THEN 1 ELSE 0 END) as NewHires,
                            SUM(CASE WHEN EndDate IS NOT NULL AND DATEDIFF(DAY, EndDate, GETDATE()) <= 90 THEN 1 ELSE 0 END) as Resignations,
                            ISNULL(AVG(CAST(DATEDIFF(YEAR, DateOfBirth, GETDATE()) AS DECIMAL)), 0) as AverageAge,
                            ISNULL(AVG(CAST(DATEDIFF(DAY, HireDate, GETDATE()) AS DECIMAL) / 365.0), 0) as AverageWorkingYears,
                            SUM(CASE WHEN Gender = N'Nam' THEN 1 ELSE 0 END) as MaleCount,
                            SUM(CASE WHEN Gender = N'Nữ' THEN 1 ELSE 0 END) as FemaleCount
                        FROM Employees";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var stats = new HRStatistics
                                {
                                    TotalEmployees = SafeConverter.ToInt32(reader["TotalEmployees"]),
                                    ActiveEmployees = SafeConverter.ToInt32(reader["ActiveEmployees"]),
                                    InactiveEmployees = SafeConverter.ToInt32(reader["InactiveEmployees"]),
                                    NewHires = SafeConverter.ToInt32(reader["NewHires"]),
                                    Resignations = SafeConverter.ToInt32(reader["Resignations"]),
                                    // FIX: Safe conversion cho decimal
                                    AverageAge = SafeConverter.ToDecimal(reader["AverageAge"]),
                                    AverageWorkingYears = SafeConverter.ToDecimal(reader["AverageWorkingYears"]),
                                    MaleCount = SafeConverter.ToInt32(reader["MaleCount"]),
                                    FemaleCount = SafeConverter.ToInt32(reader["FemaleCount"]),
                                    // Mock data cho các field khác
                                    AverageSalary = 15000000m,
                                    AverageAttendanceRate = 87.5m,
                                    AveragePerformanceScore = 85.2m
                                };

                                if (stats.TotalEmployees > 0)
                                {
                                    stats.TurnoverRate = (decimal)stats.Resignations / stats.TotalEmployees * 100;
                                }

                                return stats;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê nhân sự: {ex.Message}");
            }

            return new HRStatistics();
        }

        public List<DepartmentReportModel> GetDepartmentReports()
        {
            var departments = new List<DepartmentReportModel>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            d.DepartmentID,
                            d.DepartmentName,
                            m.FullName as ManagerName,
                            COUNT(e.EmployeeID) as TotalEmployees,
                            SUM(CASE WHEN e.Status = N'Đang làm việc' THEN 1 ELSE 0 END) as ActiveEmployees,
                            SUM(CASE WHEN e.Gender = N'Nam' THEN 1 ELSE 0 END) as MaleEmployees,
                            SUM(CASE WHEN e.Gender = N'Nữ' THEN 1 ELSE 0 END) as FemaleEmployees,
                            ISNULL(AVG(CAST(DATEDIFF(YEAR, e.DateOfBirth, GETDATE()) AS DECIMAL)), 0) as AverageAge,
                            ISNULL(AVG(CAST(DATEDIFF(DAY, e.HireDate, GETDATE()) AS DECIMAL) / 365.0), 0) as AverageWorkingYears,
                            ISNULL(AVG(p.BaseSalary), 0) as AverageSalary,
                            ISNULL(SUM(p.BaseSalary), 0) as TotalSalaryCost
                        FROM Departments d
                        LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
                        LEFT JOIN Employees m ON d.ManagerID = m.EmployeeID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        GROUP BY d.DepartmentID, d.DepartmentName, m.FullName
                        ORDER BY d.DepartmentName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            int totalEmployeesAcrossAllDepts = 0;
                            var tempList = new List<DepartmentReportModel>();

                            while (reader.Read())
                            {
                                var dept = new DepartmentReportModel
                                {
                                    DepartmentID = SafeConverter.ToInt32(reader["DepartmentID"]),
                                    DepartmentName = SafeConverter.ToString(reader["DepartmentName"]),
                                    ManagerName = SafeConverter.ToString(reader["ManagerName"], "Chưa có"),
                                    TotalEmployees = SafeConverter.ToInt32(reader["TotalEmployees"]),
                                    ActiveEmployees = SafeConverter.ToInt32(reader["ActiveEmployees"]),
                                    MaleEmployees = SafeConverter.ToInt32(reader["MaleEmployees"]),
                                    FemaleEmployees = SafeConverter.ToInt32(reader["FemaleEmployees"]),
                                    // FIX: Safe conversion cho decimal fields
                                    AverageAge = SafeConverter.ToDecimal(reader["AverageAge"]),
                                    AverageWorkingYears = SafeConverter.ToDecimal(reader["AverageWorkingYears"]),
                                    AverageSalary = SafeConverter.ToDecimal(reader["AverageSalary"]),
                                    TotalSalaryCost = SafeConverter.ToDecimal(reader["TotalSalaryCost"])
                                };

                                totalEmployeesAcrossAllDepts += dept.TotalEmployees;
                                tempList.Add(dept);
                            }

                            // Calculate percentages
                            foreach (var dept in tempList)
                            {
                                dept.EmployeePercentage = totalEmployeesAcrossAllDepts > 0 ?
                                    (decimal)dept.TotalEmployees / totalEmployeesAcrossAllDepts * 100 : 0;
                                departments.Add(dept);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo phòng ban: {ex.Message}");
            }

            return departments;
        }

        public List<BirthdayReportModel> GetBirthdayReports(int month, int year)
        {
            var birthdays = new List<BirthdayReportModel>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            e.EmployeeID,
                            e.EmployeeCode,
                            e.FullName,
                            e.DateOfBirth,
                            e.Phone,
                            e.Email,
                            d.DepartmentName,
                            p.PositionName,
                            DATEDIFF(YEAR, e.DateOfBirth, GETDATE()) as Age,
                            DATEDIFF(DAY, GETDATE(), 
                                DATEFROMPARTS(@Year, MONTH(e.DateOfBirth), DAY(e.DateOfBirth))
                            ) as DaysUntilBirthday
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        WHERE MONTH(e.DateOfBirth) = @Month 
                          AND e.Status = N'Đang làm việc'
                        ORDER BY DAY(e.DateOfBirth)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Month", month);
                        command.Parameters.AddWithValue("@Year", year);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                birthdays.Add(new BirthdayReportModel
                                {
                                    EmployeeID = SafeConverter.ToInt32(reader["EmployeeID"]),
                                    EmployeeCode = SafeConverter.ToString(reader["EmployeeCode"]),
                                    FullName = SafeConverter.ToString(reader["FullName"]),
                                    Department = SafeConverter.ToString(reader["DepartmentName"]),
                                    Position = SafeConverter.ToString(reader["PositionName"]),
                                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                                    Age = SafeConverter.ToInt32(reader["Age"]),
                                    Phone = SafeConverter.ToString(reader["Phone"]),
                                    Email = SafeConverter.ToString(reader["Email"]),
                                    DaysUntilBirthday = SafeConverter.ToInt32(reader["DaysUntilBirthday"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sinh nhật: {ex.Message}");
            }

            return birthdays;
        }

        public List<AgeGroupReportModel> GetAgeGroupReports()
        {
            // Mock implementation
            return new List<AgeGroupReportModel>
            {
                new AgeGroupReportModel { AgeGroup = "Dưới 25 tuổi", EmployeeCount = 15, Percentage = 25.0m, AverageSalary = 8000000m, AverageWorkingYears = 1.5m, AveragePerformanceScore = 82.5m },
                new AgeGroupReportModel { AgeGroup = "25-35 tuổi", EmployeeCount = 25, Percentage = 41.7m, AverageSalary = 15000000m, AverageWorkingYears = 5.2m, AveragePerformanceScore = 85.3m },
                new AgeGroupReportModel { AgeGroup = "36-45 tuổi", EmployeeCount = 15, Percentage = 25.0m, AverageSalary = 25000000m, AverageWorkingYears = 8.7m, AveragePerformanceScore = 87.1m },
                new AgeGroupReportModel { AgeGroup = "46-55 tuổi", EmployeeCount = 4, Percentage = 6.7m, AverageSalary = 35000000m, AverageWorkingYears = 12.3m, AveragePerformanceScore = 88.9m },
                new AgeGroupReportModel { AgeGroup = "Trên 55 tuổi", EmployeeCount = 1, Percentage = 1.6m, AverageSalary = 45000000m, AverageWorkingYears = 20.1m, AveragePerformanceScore = 90.2m }
            };
        }
    }
}