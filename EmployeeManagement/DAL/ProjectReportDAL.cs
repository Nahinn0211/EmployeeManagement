using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;

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
        public EmployeeManagement.Models.ProjectStatistics GetProjectStatistics()
        {
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
                                var stats = new EmployeeManagement.Models.ProjectStatistics
                                {
                                    TotalProjects = reader.GetInt32("TotalProjects"),
                                    ActiveProjects = reader.GetInt32("ActiveProjects"),
                                    CompletedProjects = reader.GetInt32("CompletedProjects"),
                                    PausedProjects = reader.GetInt32("PausedProjects"),
                                    CancelledProjects = reader.GetInt32("CancelledProjects"),
                                    OverdueProjects = reader.GetInt32("OverdueProjects"),
                                    TotalBudget = reader.GetDecimal("TotalBudget"),
                                    AverageCompletion = reader.GetDecimal("AverageCompletion")
                                };

                                reader.Close();

                                // Lấy thống kê tasks và chi phí thực tế
                                GetTaskStatistics(connection, stats);
                                GetActualCostStatistics(connection, stats);

                                return stats;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê dự án: {ex.Message}");
            }

            return new EmployeeManagement.Models.ProjectStatistics();
        }

        /// <summary>
        /// Lấy danh sách báo cáo dự án với filter
        /// </summary>
        public List<ProjectReportModel> GetProjectReports(ProjectReportFilter filter = null)
        {
            var projects = new List<ProjectReportModel>();

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
                            e.FirstName + ' ' + e.LastName as ManagerName,
                            
                            -- Task statistics
                            ISNULL(task_stats.TotalTasks, 0) as TotalTasks,
                            ISNULL(task_stats.CompletedTasks, 0) as CompletedTasks,
                            ISNULL(task_stats.InProgressTasks, 0) as InProgressTasks,
                            ISNULL(task_stats.PendingTasks, 0) as PendingTasks,
                            
                            -- Employee count
                            ISNULL(emp_stats.TotalEmployees, 0) as TotalEmployees,
                            
                            -- Actual cost from Finance
                            ISNULL(finance_stats.ActualCost, 0) as ActualCost,
                            
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
                        
                        -- Task statistics subquery
                        LEFT JOIN (
                            SELECT 
                                ProjectID,
                                COUNT(*) as TotalTasks,
                                SUM(CASE WHEN Status = N'Hoàn thành' THEN 1 ELSE 0 END) as CompletedTasks,
                                SUM(CASE WHEN Status = N'Đang thực hiện' THEN 1 ELSE 0 END) as InProgressTasks,
                                SUM(CASE WHEN Status = N'Chưa bắt đầu' THEN 1 ELSE 0 END) as PendingTasks
                            FROM Tasks 
                            GROUP BY ProjectID
                        ) task_stats ON p.ProjectID = task_stats.ProjectID
                        
                        -- Employee statistics subquery
                        LEFT JOIN (
                            SELECT 
                                ProjectID,
                                COUNT(DISTINCT EmployeeID) as TotalEmployees
                            FROM ProjectEmployees 
                            WHERE LeaveDate IS NULL
                            GROUP BY ProjectID
                        ) emp_stats ON p.ProjectID = emp_stats.ProjectID
                        
                        -- Finance statistics subquery
                        LEFT JOIN (
                            SELECT 
                                ProjectID,
                                SUM(CASE WHEN TransactionType = N'Chi' THEN Amount ELSE 0 END) as ActualCost
                            FROM Finance 
                            WHERE ProjectID IS NOT NULL
                            GROUP BY ProjectID
                        ) finance_stats ON p.ProjectID = finance_stats.ProjectID
                        
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
                        sql += $" ORDER BY {filter.SortBy} {(filter.SortDescending ? "DESC" : "ASC")}";
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
                                var project = new ProjectReportModel
                                {
                                    ProjectID = reader.GetInt32("ProjectID"),
                                    ProjectCode = reader.GetString("ProjectCode"),
                                    ProjectName = reader.GetString("ProjectName"),
                                    ManagerName = reader.IsDBNull("ManagerName") ? "" : reader.GetString("ManagerName"),
                                    StartDate = reader.IsDBNull("StartDate") ? null : reader.GetDateTime("StartDate"),
                                    EndDate = reader.IsDBNull("EndDate") ? null : reader.GetDateTime("EndDate"),
                                    Budget = reader.GetDecimal("Budget"),
                                    Status = reader.GetString("Status"),
                                    CompletionPercentage = reader.GetDecimal("CompletionPercentage"),
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo dự án: {ex.Message}");
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
                        "Tasks" => "task_count.TotalTasks DESC",
                        "Employees" => "emp_count.TotalEmployees DESC",
                        _ => "p.Budget DESC"
                    };

                    string valueField = metric switch
                    {
                        "Budget" => "p.Budget",
                        "Completion" => "p.CompletionPercentage",
                        "Tasks" => "ISNULL(task_count.TotalTasks, 0)",
                        "Employees" => "ISNULL(emp_count.TotalEmployees, 0)",
                        _ => "p.Budget"
                    };

                    string sql = $@"
                        SELECT TOP {topCount}
                            p.ProjectID,
                            p.ProjectCode,
                            p.ProjectName,
                            p.Status,
                            e.FirstName + ' ' + e.LastName as ManagerName,
                            {valueField} as Value
                        FROM Projects p
                        LEFT JOIN Employees e ON p.ManagerID = e.EmployeeID
                        LEFT JOIN (
                            SELECT ProjectID, COUNT(*) as TotalTasks
                            FROM Tasks GROUP BY ProjectID
                        ) task_count ON p.ProjectID = task_count.ProjectID
                        LEFT JOIN (
                            SELECT ProjectID, COUNT(DISTINCT EmployeeID) as TotalEmployees
                            FROM ProjectEmployees WHERE LeaveDate IS NULL
                            GROUP BY ProjectID
                        ) emp_count ON p.ProjectID = emp_count.ProjectID
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
                                    ProjectCode = reader.GetString("ProjectCode"),
                                    ProjectName = reader.GetString("ProjectName"),
                                    ManagerName = reader.IsDBNull("ManagerName") ? "" : reader.GetString("ManagerName"),
                                    Status = reader.GetString("Status"),
                                    Value = reader.GetDecimal("Value"),
                                    Metric = metric
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy top dự án: {ex.Message}");
            }

            return topProjects;
        }

        private void GetTaskStatistics(SqlConnection connection, EmployeeManagement.Models.ProjectStatistics stats)
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

        private void GetActualCostStatistics(SqlConnection connection, EmployeeManagement.Models.ProjectStatistics stats)
        {
            string sql = @"
                SELECT 
                    ISNULL(SUM(Amount), 0) as TotalActualCost,
                    COUNT(DISTINCT EmployeeID) as TotalEmployees
                FROM Finance f
                LEFT JOIN ProjectEmployees pe ON f.ProjectID = pe.ProjectID
                WHERE f.TransactionType = N'Chi' AND f.ProjectID IS NOT NULL";

            using (var command = new SqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats.TotalActualCost = reader.GetDecimal("TotalActualCost");
                        stats.TotalEmployees = reader.GetInt32("TotalEmployees");
                    }
                }
            }
        }
    }
}