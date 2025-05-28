using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.DTO; // Sửa namespace
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.BLL
{
    public class ProjectReportBLL
    {
        private readonly ProjectReportDAL projectReportDAL;

        public ProjectReportBLL()
        {
            projectReportDAL = new ProjectReportDAL();
        }

        /// <summary>
        /// Lấy thống kê tổng quan dự án
        /// </summary>
        public ProjectStatistics GetProjectStatistics()
        {
            try
            {
                var stats = projectReportDAL.GetProjectStatistics();

                // Log thông tin
                Console.WriteLine($"Lấy thống kê dự án: {stats.TotalProjects} dự án tổng cộng");

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - GetProjectStatistics: {ex.Message}");
                throw new Exception($"Không thể lấy thống kê dự án: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo dự án với filter và validation
        /// </summary>
        public List<ProjectReportDTO> GetProjectReports(ProjectReportFilter filter = null)
        {
            try
            {
                // Validate filter
                if (filter != null)
                {
                    ValidateFilter(filter);
                }

                var reports = projectReportDAL.GetProjectReports(filter);

                // Process additional business logic
                foreach (var report in reports)
                {
                    ProcessProjectReport(report);
                }

                Console.WriteLine($"Lấy báo cáo dự án: {reports.Count} dự án");

                return reports;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - GetProjectReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo dự án: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy top dự án theo tiêu chí
        /// </summary>
        public List<TopProjectModel> GetTopProjects(string metric, int topCount = 10)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(metric))
                    throw new ArgumentException("Tiêu chí không được để trống");

                if (topCount <= 0 || topCount > 50)
                    throw new ArgumentException("Số lượng top phải từ 1 đến 50");

                var validMetrics = new[] { "Budget", "Completion", "Tasks", "Employees" };
                if (!validMetrics.Contains(metric))
                    throw new ArgumentException($"Tiêu chí không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validMetrics)}");

                var topProjects = projectReportDAL.GetTopProjects(metric, topCount);

                Console.WriteLine($"Lấy top {topCount} dự án theo {metric}: {topProjects.Count} kết quả");

                return topProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - GetTopProjects: {ex.Message}");
                throw new Exception($"Không thể lấy top dự án: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo dự án theo Manager
        /// </summary>
        public List<ManagerProjectReport> GetManagerProjectReports()
        {
            try
            {
                var allProjects = projectReportDAL.GetProjectReports();

                var managerReports = allProjects
                    .Where(p => !string.IsNullOrEmpty(p.ManagerName))
                    .GroupBy(p => p.ManagerName)
                    .Select(g => new ManagerProjectReport
                    {
                        ManagerName = g.Key,
                        TotalProjects = g.Count(),
                        CompletedProjects = g.Count(p => p.IsCompleted),
                        ActiveProjects = g.Count(p => p.Status == "Đang thực hiện"),
                        TotalBudget = g.Sum(p => p.Budget),
                        AverageCompletion = g.Average(p => p.CompletionPercentage)
                    })
                    .OrderByDescending(m => m.TotalProjects)
                    .ToList();

                Console.WriteLine($"Lấy báo cáo theo Manager: {managerReports.Count} manager");

                return managerReports;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - GetManagerProjectReports: {ex.Message}");
                throw new Exception($"Không thể lấy báo cáo theo Manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Xuất báo cáo ra Excel
        /// </summary>
        public bool ExportToExcel(List<ProjectReportDTO> projects, string filePath)
        {
            try
            {
                // Tạm thời return true, sẽ implement sau
                Console.WriteLine($"Export to Excel: {filePath} với {projects.Count} dự án");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - ExportToExcel: {ex.Message}");
                throw new Exception($"Không thể xuất file Excel: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính toán dashboard metrics
        /// </summary>
        public Dictionary<string, object> GetDashboardMetrics()
        {
            try
            {
                var stats = GetProjectStatistics();
                var projects = GetProjectReports();

                var metrics = new Dictionary<string, object>
                {
                    ["TotalProjects"] = stats.TotalProjects,
                    ["ActiveProjects"] = stats.ActiveProjects,
                    ["CompletedProjects"] = stats.CompletedProjects,
                    ["OverdueProjects"] = stats.OverdueProjects,
                    ["ProjectCompletionRate"] = stats.ProjectCompletionRate,
                    ["TaskCompletionRate"] = stats.TaskCompletionRate,
                    ["TotalBudget"] = stats.TotalBudget,
                    ["TotalActualCost"] = stats.TotalActualCost,
                    ["BudgetVariance"] = stats.BudgetVariance,
                    ["AverageCompletion"] = stats.AverageCompletion,

                    // Top projects
                    ["TopProjectsByBudget"] = GetTopProjects("Budget", 5),
                    ["TopProjectsByCompletion"] = GetTopProjects("Completion", 5),

                    // Recent activities
                    ["RecentProjects"] = projects.OrderByDescending(p => p.StartDate).Take(5).ToList(),
                    ["OverdueProjects"] = projects.Where(p => p.IsOverdue).Take(5).ToList()
                };

                return metrics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi BLL - GetDashboardMetrics: {ex.Message}");
                throw new Exception($"Không thể tính toán dashboard metrics: {ex.Message}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Validate filter input
        /// </summary>
        private void ValidateFilter(ProjectReportFilter filter)
        {
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                if (filter.StartDate > filter.EndDate)
                    throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
            }

            if (filter.MinBudget.HasValue && filter.MinBudget < 0)
                throw new ArgumentException("Ngân sách tối thiểu không thể âm");

            if (filter.MaxBudget.HasValue && filter.MaxBudget < 0)
                throw new ArgumentException("Ngân sách tối đa không thể âm");

            if (filter.MinBudget.HasValue && filter.MaxBudget.HasValue && filter.MinBudget > filter.MaxBudget)
                throw new ArgumentException("Ngân sách tối thiểu không thể lớn hơn ngân sách tối đa");

            if (filter.MinCompletion.HasValue && (filter.MinCompletion < 0 || filter.MinCompletion > 100))
                throw new ArgumentException("Tiến độ tối thiểu phải từ 0 đến 100");

            if (filter.MaxCompletion.HasValue && (filter.MaxCompletion < 0 || filter.MaxCompletion > 100))
                throw new ArgumentException("Tiến độ tối đa phải từ 0 đến 100");

            var validSortFields = new[] { "ProjectName", "StartDate", "EndDate", "Budget", "CompletionPercentage", "Status" };
            if (!string.IsNullOrEmpty(filter.SortBy) && !validSortFields.Contains(filter.SortBy))
                throw new ArgumentException($"Trường sắp xếp không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validSortFields)}");
        }

        /// <summary>
        /// Process additional business logic for project report
        /// </summary>
        private void ProcessProjectReport(ProjectReportDTO report)
        {
            // Set additional flags and calculations
            report.IsOverdue = report.DaysOverdue > 0;
            report.IsCompleted = report.Status == "Hoàn thành";

            // Add business rules
            if (report.CompletionPercentage >= 100 && report.Status != "Hoàn thành")
            {
                // Log inconsistency
                Console.WriteLine($"Cảnh báo: Dự án {report.ProjectCode} có tiến độ 100% nhưng trạng thái chưa hoàn thành");
            }

            if (report.ActualCost > report.Budget * 1.2m) // Vượt ngân sách 20%
            {
                Console.WriteLine($"Cảnh báo: Dự án {report.ProjectCode} vượt ngân sách: {report.BudgetUtilization:F1}%");
            }
        }

        #endregion
    }
}