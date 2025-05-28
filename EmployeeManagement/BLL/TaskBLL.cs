using System;
using System.Collections.Generic;
using EmployeeManagement.DAL;
 using EmployeeManagement.Models.Entity; 

namespace EmployeeManagement.BLL
{
    public class TaskBLL
    {
        private readonly TaskDAL taskDAL;

        public TaskBLL()
        {
            try
            {
                taskDAL = new TaskDAL();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khởi tạo TaskDAL: {ex.Message}", ex);
            }
        }

        public List<WorkTask> GetAllTasks()
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                return taskDAL.GetAllTasks();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách task: {ex.Message}", ex);
            }
        }

        public WorkTask GetTaskById(int taskId)
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                if (taskId <= 0)
                    throw new ArgumentException("TaskID không hợp lệ");

                return taskDAL.GetTaskById(taskId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy task theo ID: {ex.Message}", ex);
            }
        }

        public string GenerateTaskCode()
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                return taskDAL.GenerateTaskCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã task: {ex.Message}", ex);
            }
        }

        public int AddTask(WorkTask task)
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                ValidateTask(task);
                return taskDAL.AddTask(task);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm task: {ex.Message}", ex);
            }
        }

        public void UpdateTask(WorkTask task)
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                ValidateTask(task);
                taskDAL.UpdateTask(task);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật task: {ex.Message}", ex);
            }
        }

        public void DeleteTask(int taskId)
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                if (taskId <= 0)
                    throw new ArgumentException("TaskID không hợp lệ");

                taskDAL.DeleteTask(taskId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa task: {ex.Message}", ex);
            }
        }

        public List<Project> GetAllProjects()
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                var projects = taskDAL.GetAllProjects();
                return projects ?? new List<Project>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dự án: {ex.Message}", ex);
            }
        }

        public List<Employee> GetAvailableEmployees()
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                var employees = taskDAL.GetAvailableEmployees();
                return employees ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }

        public List<TaskDisplayModel> GetTaskDisplayModels()
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                var tasks = taskDAL.GetAllTasks();
                List<TaskDisplayModel> displayModels = new List<TaskDisplayModel>();

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        displayModels.Add(new TaskDisplayModel
                        {
                            TaskID = task.TaskID,
                            TaskCode = task.TaskCode ?? "",
                            TaskName = task.TaskName ?? "",
                            ProjectName = task.Project?.ProjectName ?? "Không xác định",
                            AssignedToName = task.AssignedTo?.FullName ?? "Chưa giao",
                            StartDate = task.StartDate,
                            DueDate = task.DueDate,
                            Status = task.Status ?? "",
                            Priority = task.Priority ?? "",
                            CompletionPercentage = task.CompletionPercentage
                        });
                    }
                }

                return displayModels;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo display models: {ex.Message}", ex);
            }
        }

        public List<WorkTask> GetTasksByProject(int projectId)
        {
            try
            {
                if (taskDAL == null)
                    throw new InvalidOperationException("TaskDAL chưa được khởi tạo");

                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return taskDAL.GetTasksByProject(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy công việc theo dự án: {ex.Message}", ex);
            }
        }

        private void ValidateTask(WorkTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task), "Task không được null");

            if (string.IsNullOrWhiteSpace(task.TaskCode))
                throw new ArgumentException("Mã công việc không được để trống");

            if (string.IsNullOrWhiteSpace(task.TaskName))
                throw new ArgumentException("Tên công việc không được để trống");

            if (task.ProjectID <= 0)
                throw new ArgumentException("Phải chọn dự án cho công việc");

            // Validate status
            var validStatuses = new[] { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trì hoãn", "Hủy bỏ" };
            if (!string.IsNullOrEmpty(task.Status) && Array.IndexOf(validStatuses, task.Status) == -1)
                throw new ArgumentException("Trạng thái không hợp lệ");

            // Validate priority
            var validPriorities = new[] { "Cao", "Trung bình", "Thấp" };
            if (!string.IsNullOrEmpty(task.Priority) && Array.IndexOf(validPriorities, task.Priority) == -1)
                throw new ArgumentException("Độ ưu tiên không hợp lệ");

            // Validate completion percentage
            if (task.CompletionPercentage < 0 || task.CompletionPercentage > 100)
                throw new ArgumentException("Tiến độ phải từ 0 đến 100%");

            // Validate dates
            if (task.StartDate.HasValue && task.DueDate.HasValue && task.StartDate > task.DueDate)
                throw new ArgumentException("Ngày bắt đầu không thể sau ngày kết thúc");

            if (task.CompletedDate.HasValue && task.StartDate.HasValue && task.CompletedDate < task.StartDate)
                throw new ArgumentException("Ngày hoàn thành không thể trước ngày bắt đầu");
        }
    }
}