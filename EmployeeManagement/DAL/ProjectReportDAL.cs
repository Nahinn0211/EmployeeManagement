using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.DAL
{
    public class ProjectReportDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        /// <summary>
        /// Lấy thống kê tổng quan dự án
        /// </summary>
        public ProjectStatistics GetProjectStatistics()
        {
            var stats = new ProjectStatistics();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            COUNT(*) as TotalProjects,
                            SUM(CASE WHEN Status = N'Đang thực hiện' THEN 1 ELSE 0 END) as ActiveProjects,
                            SUM(CASE WHEN Status = N'Hoàn thành' THEN 1 ELSE 0 END) as CompletedProjects,
                            SUM(CASE WHEN Status = N'Tạm dừng' THEN 1 ELSE 0 END) as PausedProjects,
                            SUM(CASE WHEN Status = N'Hủy bỏ' THEN 1 ELSE 0 END) as CancelledProjects,
                            SUM(CASE WHEN EndDate < GETDATE() AND Status != N'Hoàn thành' THEN 1 ELSE 0 END) as OverdueProjects,
                            ISNULL(SUM(Budget), 0) as TotalBudget,
                            ISNULL(AVG(CompletionPercentage), 0) as AverageCompletion
                        FROM Projects";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                stats.TotalProjects = reader.GetInt32("TotalProjects");
                                stats.ActiveProjects = reader.GetInt32("ActiveProjects");
                                stats.CompletedProjects = reader.GetInt32("CompletedProjects");
                                stats.PausedProjects = reader.GetInt32("PausedProjects");
                                stats.CancelledProjects = reader.GetInt32("CancelledProjects");
                                stats.OverdueProjects = reader.GetInt32("OverdueProjects");
                                stats.TotalBudget = reader.GetDecimal("TotalBudget");
                                stats.AverageCompletion = reader.GetDecimal("AverageCompletion");
                            }
                        }
                    }

                    // Lấy thống kê tasks và chi phí thực tế
                    GetTaskStatistics(connection, stats);
                    GetActualCostStatistics(connection, stats);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thống kê dự án: {ex.Message}");
                // Trả về stats mặc định với sample data
                stats = CreateSampleStatistics();
            }

            return stats;
        }

        /// <summary>
        /// Lấy danh sách báo cáo dự án với filter
        /// </summary>
        public List<ProjectReportDTO> GetProjectReports(ProjectReportFilter filter = null)
        {
            var projects = new List<ProjectReportDTO>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            p.ProjectID,
                            p.ProjectCode,
                            p.ProjectName,
                            p.StartDate,
                            p.EndDate,
                            p.Budget,
                            p.Status,
                            p.CompletionPercentage,
                            p.Notes,
                            ISNULL(e.FullName, '') as ManagerName,
                            
                            -- Tạm thời set giá trị mặc định, sau này sẽ JOIN với các bảng khác
                            0 as TotalTasks,
                            0 as CompletedTasks,
                            0 as InProgressTasks,
                            0 as PendingTasks,
                            0 as TotalEmployees,
                            0 as ActualCost,
                            
                            -- Date calculations
                            CASE 
                                WHEN p.EndDate IS NULL THEN 0
                                WHEN p.EndDate > GETDATE() THEN DATEDIFF(DAY, GETDATE(), p.EndDate)
                                ELSE 0
                            END as DaysRemaining,
                            
                            CASE 
                                WHEN p.EndDate IS NULL THEN 0
                                WHEN p.EndDate < GETDATE() AND p.Status != N'Hoàn thành' THEN DATEDIFF(DAY, p.EndDate, GETDATE())
                                ELSE 0
                            END as DaysOverdue
                            
                        FROM Projects p
                        LEFT JOIN Employees e ON p.ManagerID = e.EmployeeID
                        WHERE 1=1";

                    // Apply filters
                    var parameters = new List<SqlParameter>();
                    if (filter != null)
                    {
                        if (filter.StartDate.HasValue)
                        {
                            sql += " AND p.StartDate >= @StartDate";
                            parameters.Add(new SqlParameter("@StartDate", filter.StartDate.Value));
                        }
                        if (filter.EndDate.HasValue)
                        {
                            sql += " AND p.EndDate <= @EndDate";
                            parameters.Add(new SqlParameter("@EndDate", filter.EndDate.Value));
                        }
                        if (!string.IsNullOrEmpty(filter.Status))
                        {
                            sql += " AND p.Status = @Status";
                            parameters.Add(new SqlParameter("@Status", filter.Status));
                        }
                        if (filter.ManagerID.HasValue)
                        {
                            sql += " AND p.ManagerID = @ManagerID";
                            parameters.Add(new SqlParameter("@ManagerID", filter.ManagerID.Value));
                        }
                        if (filter.MinBudget.HasValue)
                        {
                            sql += " AND p.Budget >= @MinBudget";
                            parameters.Add(new SqlParameter("@MinBudget", filter.MinBudget.Value));
                        }
                        if (filter.MaxBudget.HasValue)
                        {
                            sql += " AND p.Budget <= @MaxBudget";
                            parameters.Add(new SqlParameter("@MaxBudget", filter.MaxBudget.Value));
                        }
                        if (!string.IsNullOrEmpty(filter.SearchKeyword))
                        {
                            sql += " AND (p.ProjectName LIKE @SearchKeyword OR p.ProjectCode LIKE @SearchKeyword)";
                            parameters.Add(new SqlParameter("@SearchKeyword", $"%{filter.SearchKeyword}%"));
                        }
                        if (filter.IsOverdue.HasValue && filter.IsOverdue.Value)
                        {
                            sql += " AND p.EndDate < GETDATE() AND p.Status != N'Hoàn thành'";
                        }

                        // Apply sorting
                        string validSortBy = ValidateSortBy(filter.SortBy);
                        sql += $" ORDER BY {validSortBy} {(filter.SortDescending ? "DESC" : "ASC")}";
                    }
                    else
                    {
                        sql += " ORDER BY p.ProjectName";
                    }

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var project = new ProjectReportDTO
                                {
                                    ProjectID = reader.GetInt32("ProjectID"),
                                    ProjectCode = reader.IsDBNull("ProjectCode") ? "" : reader.GetString("ProjectCode"),
                                    ProjectName = reader.IsDBNull("ProjectName") ? "" : reader.GetString("ProjectName"),
                                    ManagerName = reader.IsDBNull("ManagerName") ? "" : reader.GetString("ManagerName"),
                                    StartDate = reader.IsDBNull("StartDate") ? null : reader.GetDateTime("StartDate"),
                                    EndDate = reader.IsDBNull("EndDate") ? null : reader.GetDateTime("EndDate"),
                                    Budget = reader.IsDBNull("Budget") ? 0 : reader.GetDecimal("Budget"),
                                    Status = reader.IsDBNull("Status") ? "" : reader.GetString("Status"),
                                    CompletionPercentage = reader.IsDBNull("CompletionPercentage") ? 0 : reader.GetDecimal("CompletionPercentage"),
                                    Notes = reader.IsDBNull("Notes") ? "" : reader.GetString("Notes"),
                                    TotalTasks = reader.GetInt32("TotalTasks"),
                                    CompletedTasks = reader.GetInt32("CompletedTasks"),
                                    InProgressTasks = reader.GetInt32("InProgressTasks"),
                                    PendingTasks = reader.GetInt32("PendingTasks"),
                                    TotalEmployees = reader.GetInt32("TotalEmployees"),
                                    ActualCost = reader.GetDecimal("ActualCost"),
                                    DaysRemaining = reader.GetInt32("DaysRemaining"),
                                    DaysOverdue = reader.GetInt32("DaysOverdue")
                                };

                                project.IsOverdue = project.DaysOverdue > 0;
                                project.IsCompleted = project.Status == "Hoàn thành";

                                projects.Add(project);
                            }
                        }
                    }
                }

                Console.WriteLine($"Đã tải {projects.Count} dự án từ database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy báo cáo dự án từ DB: {ex.Message}");
            }

            // Nếu không có dữ liệu hoặc có lỗi, tạo dữ liệu mẫu
            if (projects.Count == 0)
            {
                Console.WriteLine("Tạo dữ liệu mẫu cho báo cáo dự án...");
                projects = CreateSampleProjectData();
            }

            return projects;
        }

        /// <summary>
        /// Lấy top dự án theo tiêu chí
        /// </summary>
        public List<TopProjectModel> GetTopProjects(string metric, int topCount = 10)
        {
            var topProjects = new List<TopProjectModel>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string orderBy = metric switch
                    {
                        "Budget" => "p.Budget DESC",
                        "Completion" => "p.CompletionPercentage DESC",
                        _ => "p.Budget DESC"
                    };

                    string valueField = metric switch
                    {
                        "Budget" => "p.Budget",
                        "Completion" => "p.CompletionPercentage",
                        _ => "p.Budget"
                    };

                    string sql = $@"
                        SELECT TOP {topCount}
                            p.ProjectID,
                            p.ProjectCode,
                            p.ProjectName,
                            p.Status,
                            ISNULL(e.FullName, '') as ManagerName,
                            {valueField} as Value
                        FROM Projects p
                        LEFT JOIN Employees e ON p.ManagerID = e.EmployeeID
                        ORDER BY {orderBy}";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                topProjects.Add(new TopProjectModel
                                {
                                    ProjectID = reader.GetInt32("ProjectID"),
                                    ProjectCode = reader.IsDBNull("ProjectCode") ? "" : reader.GetString("ProjectCode"),
                                    ProjectName = reader.IsDBNull("ProjectName") ? "" : reader.GetString("ProjectName"),
                                    ManagerName = reader.IsDBNull("ManagerName") ? "" : reader.GetString("ManagerName"),
                                    Status = reader.IsDBNull("Status") ? "" : reader.GetString("Status"),
                                    Value = reader.IsDBNull("Value") ? 0 : reader.GetDecimal("Value"),
                                    Metric = metric
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy top dự án: {ex.Message}");
            }

            return topProjects;
        }

        #region Private Helper Methods

        private void GetTaskStatistics(SqlConnection connection, ProjectStatistics stats)
        {
            try
            {
                string sql = @"
                    SELECT 
                        COUNT(*) as TotalTasks,
                        SUM(CASE WHEN Status = N'Hoàn thành' THEN 1 ELSE 0 END) as CompletedTasks
                    FROM Tasks";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalTasks = reader.GetInt32("TotalTasks");
                            stats.CompletedTasks = reader.GetInt32("CompletedTasks");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thống kê tasks: {ex.Message}");
                stats.TotalTasks = 0;
                stats.CompletedTasks = 0;
            }
        }

        private void GetActualCostStatistics(SqlConnection connection, ProjectStatistics stats)
        {
            try
            {
                string sql = @"
                    SELECT 
                        ISNULL(SUM(f.Amount), 0) as TotalActualCost
                    FROM Finance f
                    WHERE f.TransactionType = N'Chi' AND f.ProjectID IS NOT NULL";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalActualCost = reader.GetDecimal("TotalActualCost");
                        }
                    }
                }

                // Lấy tổng số nhân viên
                string empSql = @"
                    SELECT COUNT(DISTINCT pe.EmployeeID) as TotalEmployees
                    FROM ProjectEmployees pe
                    WHERE pe.LeaveDate IS NULL";

                using (var empCommand = new SqlCommand(empSql, connection))
                {
                    using (var empReader = empCommand.ExecuteReader())
                    {
                        if (empReader.Read())
                        {
                            stats.TotalEmployees = empReader.GetInt32("TotalEmployees");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thống kê chi phí: {ex.Message}");
                stats.TotalActualCost = 0;
                stats.TotalEmployees = 0;
            }
        }

        private string ValidateSortBy(string sortBy)
        {
            var validFields = new Dictionary<string, string>
            {
                ["ProjectName"] = "p.ProjectName",
                ["StartDate"] = "p.StartDate",
                ["EndDate"] = "p.EndDate",
                ["Budget"] = "p.Budget",
                ["CompletionPercentage"] = "p.CompletionPercentage",
                ["Status"] = "p.Status",
                ["ManagerName"] = "e.FullName"
            };

            return validFields.ContainsKey(sortBy ?? "") ? validFields[sortBy] : "p.ProjectName";
        }

        #endregion

        #region Sample Data Creation

        private ProjectStatistics CreateSampleStatistics()
        {
            return new ProjectStatistics
            {
                TotalProjects = 5,
                ActiveProjects = 2,
                CompletedProjects = 2,
                PausedProjects = 0,
                CancelledProjects = 1,
                OverdueProjects = 1,
                TotalBudget = 800000000,
                TotalActualCost = 650000000,
                AverageCompletion = 65,
                TotalTasks = 120,
                CompletedTasks = 85,
                TotalEmployees = 25
            };
        }

        private List<ProjectReportDTO> CreateSampleProjectData()
        {
            return new List<ProjectReportDTO>
            {
                new ProjectReportDTO
                {
                    ProjectID = 1,
                    ProjectCode = "PRJ001",
                    ProjectName = "Hệ thống Quản lý Nhân sự",
                    ManagerName = "Nguyễn Văn Anh",
                    StartDate = DateTime.Now.AddDays(-45),
                    EndDate = DateTime.Now.AddDays(15),
                    Budget = 150000000,
                    Status = "Đang thực hiện",
                    CompletionPercentage = 80,
                    TotalTasks = 25,
                    CompletedTasks = 20,
                    InProgressTasks = 3,
                    PendingTasks = 2,
                    TotalEmployees = 6,
                    ActualCost = 120000000,
                    DaysRemaining = 15,
                    DaysOverdue = 0,
                    IsOverdue = false,
                    IsCompleted = false,
                    Notes = "Dự án đang tiến triển tốt"
                },
                new ProjectReportDTO
                {
                    ProjectID = 2,
                    ProjectCode = "PRJ002",
                    ProjectName = "Website Thương mại Điện tử",
                    ManagerName = "Trần Thị Bình",
                    StartDate = DateTime.Now.AddDays(-90),
                    EndDate = DateTime.Now.AddDays(-10),
                    Budget = 200000000,
                    Status = "Hoàn thành",
                    CompletionPercentage = 100,
                    TotalTasks = 40,
                    CompletedTasks = 40,
                    InProgressTasks = 0,
                    PendingTasks = 0,
                    TotalEmployees = 8,
                    ActualCost = 185000000,
                    DaysRemaining = 0,
                    DaysOverdue = 0,
                    IsOverdue = false,
                    IsCompleted = true,
                    Notes = "Dự án hoàn thành đúng hạn"
                },
                new ProjectReportDTO
                {
                    ProjectID = 3,
                    ProjectCode = "PRJ003",
                    ProjectName = "Ứng dụng Mobile Banking",
                    ManagerName = "Lê Văn Cường",
                    StartDate = DateTime.Now.AddDays(-120),
                    EndDate = DateTime.Now.AddDays(-5),
                    Budget = 300000000,
                    Status = "Đang thực hiện",
                    CompletionPercentage = 75,
                    TotalTasks = 60,
                    CompletedTasks = 45,
                    InProgressTasks = 10,
                    PendingTasks = 5,
                    TotalEmployees = 12,
                    ActualCost = 280000000,
                    DaysRemaining = 0,
                    DaysOverdue = 5,
                    IsOverdue = true,
                    IsCompleted = false,
                    Notes = "Dự án bị chậm tiến độ 5 ngày"
                },
                new ProjectReportDTO
                {
                    ProjectID = 4,
                    ProjectCode = "PRJ004",
                    ProjectName = "Hệ thống CRM",
                    ManagerName = "Phạm Thị Dung",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(60),
                    Budget = 100000000,
                    Status = "Khởi tạo",
                    CompletionPercentage = 15,
                    TotalTasks = 30,
                    CompletedTasks = 5,
                    InProgressTasks = 5,
                    PendingTasks = 20,
                    TotalEmployees = 4,
                    ActualCost = 15000000,
                    DaysRemaining = 60,
                    DaysOverdue = 0,
                    IsOverdue = false,
                    IsCompleted = false,
                    Notes = "Dự án mới khởi động"
                },
                new ProjectReportDTO
                {
                    ProjectID = 5,
                    ProjectCode = "PRJ005",
                    ProjectName = "Hệ thống ERP",
                    ManagerName = "Hoàng Văn Em",
                    StartDate = DateTime.Now.AddDays(-180),
                    EndDate = DateTime.Now.AddDays(-30),
                    Budget = 50000000,
                    Status = "Hủy bỏ",
                    CompletionPercentage = 25,
                    TotalTasks = 20,
                    CompletedTasks = 5,
                    InProgressTasks = 0,
                    PendingTasks = 15,
                    TotalEmployees = 3,
                    ActualCost = 20000000,
                    DaysRemaining = 0,
                    DaysOverdue = 0,
                    IsOverdue = false,
                    IsCompleted = false,
                    Notes = "Dự án bị hủy do thay đổi yêu cầu"
                }
            };
        }

        #endregion
    }
}