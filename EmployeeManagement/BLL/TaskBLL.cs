using System;
using System.Collections.Generic;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;

namespace EmployeeManagement.BLL
{
    public class TaskBLL
    {
        private readonly TaskDAL taskDAL;

        public TaskBLL()
        {
            taskDAL = new TaskDAL();
        }

        public List<WorkTask> GetAllTasks()
        {
            return taskDAL.GetAllTasks();
        }

        public WorkTask GetTaskById(int taskId)
        {
            return taskDAL.GetTaskById(taskId);
        }

        public string GenerateTaskCode()
        {
            return taskDAL.GenerateTaskCode();
        }

        public int AddTask(WorkTask task)
        {
            ValidateTask(task);
            return taskDAL.AddTask(task);
        }

        public void UpdateTask(WorkTask task)
        {
            ValidateTask(task);
            taskDAL.UpdateTask(task);
        }

        public void DeleteTask(int taskId)
        {
            taskDAL.DeleteTask(taskId);
        }

        public List<Project> GetAllProjects()
        {
            return taskDAL.GetAllProjects();
        }

        public List<Employee> GetAvailableEmployees()
        {
            return taskDAL.GetAvailableEmployees();
        }

        public List<TaskDisplayModel> GetTaskDisplayModels()
        {
            var tasks = taskDAL.GetAllTasks();
            List<TaskDisplayModel> displayModels = new List<TaskDisplayModel>();

            foreach (var task in tasks)
            {
                displayModels.Add(new TaskDisplayModel
                {
                    TaskID = task.TaskID,
                    TaskCode = task.TaskCode,
                    TaskName = task.TaskName,
                    ProjectName = task.Project?.ProjectName ?? "Không xác định",
                    AssignedToName = task.AssignedTo?.FullName ?? "Chưa giao",
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    CompletionPercentage = task.CompletionPercentage
                });
            }

            return displayModels;
        }

        private void ValidateTask(WorkTask task)
        {
            if (string.IsNullOrWhiteSpace(task.TaskCode))
                throw new ArgumentException("Mã công việc không được để trống");

            if (string.IsNullOrWhiteSpace(task.TaskName))
                throw new ArgumentException("Tên công việc không được để trống");

            if (task.ProjectID <= 0)
                throw new ArgumentException("Phải chọn dự án cho công việc");
        }
    }
}