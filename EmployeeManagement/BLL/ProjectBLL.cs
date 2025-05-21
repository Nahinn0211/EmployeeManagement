using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.BLL
{
    public class ProjectBLL
    {
        private readonly ProjectDAL projectDAL;

        public ProjectBLL()
        {
            projectDAL = new ProjectDAL();
        }

        #region Project CRUD Operations
        public List<Project> GetAllProjects()
        {
            return projectDAL.GetAllProjects();
        }

        public Project GetProjectById(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("ID dự án không hợp lệ");

            return projectDAL.GetProjectById(projectId);
        }

        public string GenerateProjectCode()
        {
            return projectDAL.GenerateProjectCode();
        }

        public int AddProject(Project project)
        {
            ValidateProject(project);
            ValidateProjectDates(project);
            ValidateProjectBudget(project);

            return projectDAL.AddProject(project);
        }

        public void UpdateProject(Project project)
        {
            if (project.ProjectID <= 0)
                throw new ArgumentException("ID dự án không hợp lệ");

            ValidateProject(project);
            ValidateProjectDates(project);
            ValidateProjectBudget(project);

            projectDAL.UpdateProject(project);
        }

        public void DeleteProject(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("ID dự án không hợp lệ");

            projectDAL.DeleteProject(projectId);
        }
        #endregion

        #region Project Business Logic
        public List<Employee> GetAvailableManagers()
        {
            return projectDAL.GetAvailableManagers();
        }

         public EmployeeManagement.Models.DTO.ProjectStatistics GetProjectStatistics()
        {
            return projectDAL.GetProjectStatistics();
        }

        public List<ProjectDisplayModel> GetProjectDisplayModels()
        {
            var projects = projectDAL.GetAllProjects();
            List<ProjectDisplayModel> displayModels = new List<ProjectDisplayModel>();

            foreach (var project in projects)
            {
                displayModels.Add(new ProjectDisplayModel
                {
                    ProjectID = project.ProjectID,
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Status = GetStatusDisplayText(project.Status),
                    Budget = FormatBudget(project.Budget),
                    Progress = $"{project.CompletionPercentage}%",
                    ManagerName = project.Manager?.FullName ?? "Chưa phân công",
                    EmployeeCount = project.Employees?.Count ?? 0,
                    TaskCount = project.Tasks?.Count ?? 0,
                    Duration = CalculateProjectDuration(project.StartDate, project.EndDate)
                });
            }

            return displayModels;
        }

        public void UpdateProjectProgress(int projectId)
        {
            var project = GetProjectById(projectId);
            if (project == null || project.Tasks == null || project.Tasks.Count == 0) return;

            // Tính tiến độ dựa trên các công việc hoàn thành
            int totalTasks = project.Tasks.Count;
            int completedTasks = project.Tasks.Count(t => t.Status == "Hoàn thành");

            decimal completionPercentage = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;
            project.CompletionPercentage = Math.Round(completionPercentage, 2);

            // Tự động cập nhật trạng thái dựa trên tiến độ
            UpdateProjectStatusBasedOnProgress(project);

            projectDAL.UpdateProject(project);
        }

        public bool CanDeleteProject(int projectId)
        {
            var project = GetProjectById(projectId);
            if (project == null) return false;

            // Kiểm tra xem dự án có công việc, nhân viên, tài liệu hoặc tài chính liên quan không
            return project.Tasks.Count == 0 &&
                   project.Employees.Count == 0 &&
                   project.Documents.Count == 0 &&
                   project.Finances.Count == 0;
        }

        public List<Project> SearchProjects(string searchText, string status = "", string managerName = "")
        {
            var allProjects = GetAllProjects();

            return allProjects.Where(p =>
                (string.IsNullOrEmpty(searchText) ||
                 p.ProjectName.ToLower().Contains(searchText.ToLower()) ||
                 p.ProjectCode.ToLower().Contains(searchText.ToLower())) &&
                (string.IsNullOrEmpty(status) || p.Status == status) &&
                (string.IsNullOrEmpty(managerName) || p.Manager?.FullName.ToLower().Contains(managerName.ToLower()) == true)
            ).ToList();
        }

        public List<Project> GetProjectsByStatus(string status)
        {
            var allProjects = GetAllProjects();
            return allProjects.Where(p => p.Status == status).ToList();
        }

        public List<Project> GetProjectsByManager(int managerId)
        {
            var allProjects = GetAllProjects();
            return allProjects.Where(p => p.ManagerID == managerId).ToList();
        }

        public decimal GetProjectBudgetTotal()
        {
            var allProjects = GetAllProjects();
            return allProjects.Sum(p => p.Budget);
        }

        public decimal GetAverageProjectCompletion()
        {
            var allProjects = GetAllProjects();
            return allProjects.Count > 0 ? allProjects.Average(p => p.CompletionPercentage) : 0;
        }
        #endregion

        #region Validation Methods
        private void ValidateProject(Project project)
        {
            if (string.IsNullOrWhiteSpace(project.ProjectCode))
                throw new ArgumentException("Mã dự án không được để trống");

            if (string.IsNullOrWhiteSpace(project.ProjectName))
                throw new ArgumentException("Tên dự án không được để trống");

            if (project.ManagerID <= 0)
                throw new ArgumentException("Phải chọn quản lý dự án");

            if (string.IsNullOrWhiteSpace(project.Status))
                throw new ArgumentException("Phải chọn trạng thái dự án");

            // Kiểm tra tính duy nhất của mã dự án
            ValidateProjectCodeUniqueness(project);
        }

        private void ValidateProjectDates(Project project)
        {
            if (project.StartDate.HasValue && project.EndDate.HasValue)
            {
                if (project.EndDate < project.StartDate)
                    throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu");

                // Kiểm tra ngày bắt đầu không được là quá khứ (trừ khi đang cập nhật)
                if (project.ProjectID == 0 && project.StartDate < DateTime.Now.Date)
                    throw new ArgumentException("Ngày bắt đầu không được là ngày quá khứ");
            }
        }

        private void ValidateProjectBudget(Project project)
        {
            if (project.Budget < 0)
                throw new ArgumentException("Ngân sách dự án không được âm");

            if (project.Budget > 999999999999) // 999 tỷ
                throw new ArgumentException("Ngân sách dự án quá lớn");
        }

        private void ValidateProjectCodeUniqueness(Project project)
        {
            var allProjects = GetAllProjects();
            var existingProject = allProjects.FirstOrDefault(p =>
                p.ProjectCode.Equals(project.ProjectCode, StringComparison.OrdinalIgnoreCase) &&
                p.ProjectID != project.ProjectID);

            if (existingProject != null)
                throw new ArgumentException($"Mã dự án '{project.ProjectCode}' đã tồn tại");
        }

        private void UpdateProjectStatusBasedOnProgress(Project project)
        {
            if (project.CompletionPercentage == 100 && project.Status != "Hoàn thành")
            {
                project.Status = "Hoàn thành";
            }
            else if (project.CompletionPercentage > 0 && project.CompletionPercentage < 100 &&
                     project.Status == "Khởi tạo")
            {
                project.Status = "Đang thực hiện";
            }
        }
        #endregion

        #region Helper Methods
        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Khởi tạo" => "🆕 Khởi tạo",
                "Đang thực hiện" => "🚀 Đang thực hiện",
                "Hoàn thành" => "✅ Hoàn thành",
                "Tạm dừng" => "⏸️ Tạm dừng",
                "Hủy bỏ" => "❌ Hủy bỏ",
                _ => status
            };
        }

        private string FormatBudget(decimal budget)
        {
            if (budget >= 1000000000) // >= 1 tỷ
                return $"{budget / 1000000000:F1} tỷ VNĐ";
            else if (budget >= 1000000) // >= 1 triệu
                return $"{budget / 1000000:F1} triệu VNĐ";
            else if (budget >= 1000) // >= 1 nghìn
                return $"{budget / 1000:F0} nghìn VNĐ";
            else
                return $"{budget:N0} VNĐ";
        }

        private string CalculateProjectDuration(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return "Chưa xác định";

            var duration = endDate.Value - startDate.Value;

            if (duration.TotalDays < 1)
                return "< 1 ngày";
            else if (duration.TotalDays <= 30)
                return $"{duration.Days} ngày";
            else if (duration.TotalDays <= 365)
                return $"{duration.Days / 30} tháng";
            else
                return $"{duration.Days / 365:F1} năm";
        }

        public string[] GetAvailableStatuses()
        {
            return new string[]
            {
                "Khởi tạo",
                "Đang thực hiện",
                "Hoàn thành",
                "Tạm dừng",
                "Hủy bỏ"
            };
        }

        public Color GetStatusColor(string status)
        {
            return status switch
            {
                "Khởi tạo" => Color.FromArgb(158, 158, 158),
                "Đang thực hiện" => Color.FromArgb(33, 150, 243),
                "Hoàn thành" => Color.FromArgb(76, 175, 80),
                "Tạm dừng" => Color.FromArgb(255, 152, 0),
                "Hủy bỏ" => Color.FromArgb(244, 67, 54),
                _ => Color.FromArgb(64, 64, 64)
            };
        }

        public string GetProjectHealthStatus(Project project)
        {
            if (!project.EndDate.HasValue)
                return "Không xác định";

            var now = DateTime.Now;
            var daysUntilDeadline = (project.EndDate.Value - now).TotalDays;

            // Fix: Convert to decimal để tránh lỗi type mismatch
            decimal expectedProgress = 0;
            if (project.StartDate.HasValue)
            {
                var totalDays = (project.EndDate.Value - project.StartDate.Value).TotalDays;
                var passedDays = (now - project.StartDate.Value).TotalDays;

                if (totalDays > 0)
                {
                    expectedProgress = (decimal)((passedDays / totalDays) * 100);

                    // Đảm bảo expectedProgress trong khoảng 0-100
                    expectedProgress = Math.Max(0, Math.Min(100, expectedProgress));
                }
            }

            if (project.Status == "Hoàn thành")
                return "✅ Hoàn thành";
            else if (project.Status == "Hủy bỏ")
                return "❌ Đã hủy";
            else if (project.Status == "Tạm dừng")
                return "⏸️ Tạm dừng";
            else if (project.CompletionPercentage >= expectedProgress)
                return "🟢 Đúng tiến độ";
            else if (daysUntilDeadline > 7)
                return "🟡 Chậm tiến độ";
            else if (daysUntilDeadline > 0)
                return "🔴 Nguy cơ trễ hạn";
            else
                return "🔴 Đã quá hạn";
        }

        // Bonus: Thêm method tính phần trăm tiến độ dự kiến
        public decimal GetExpectedProgress(Project project)
        {
            if (!project.StartDate.HasValue || !project.EndDate.HasValue)
                return 0;

            var now = DateTime.Now;
            var totalDays = (project.EndDate.Value - project.StartDate.Value).TotalDays;
            var passedDays = (now - project.StartDate.Value).TotalDays;

            if (totalDays <= 0)
                return 0;

            var expectedProgress = (decimal)((passedDays / totalDays) * 100);

            // Đảm bảo trong khoảng 0-100
            return Math.Max(0, Math.Min(100, expectedProgress));
        }

        // Bonus: Method kiểm tra dự án có đang trễ tiến độ không
        public bool IsProjectBehindSchedule(Project project)
        {
            var expectedProgress = GetExpectedProgress(project);
            return project.CompletionPercentage < expectedProgress &&
                   project.Status != "Hoàn thành" &&
                   project.Status != "Hủy bỏ";
        }

        // Bonus: Method tính số ngày còn lại
        public int GetDaysRemaining(Project project)
        {
            if (!project.EndDate.HasValue)
                return -1;

            var daysRemaining = (project.EndDate.Value - DateTime.Now).Days;
            return Math.Max(0, daysRemaining);
        }
        #endregion
    }
}