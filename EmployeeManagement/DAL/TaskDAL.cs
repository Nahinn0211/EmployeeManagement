using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;

namespace EmployeeManagement.DAL
{
    public class TaskDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        public List<WorkTask> GetAllTasks()
        {
            List<WorkTask> tasks = new List<WorkTask>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, e.FullName as AssignedToName
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        ORDER BY t.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkTask task = new WorkTask
                            {
                                TaskID = Convert.ToInt32(reader["TaskID"]),
                                TaskCode = reader["TaskCode"].ToString(),
                                TaskName = reader["TaskName"].ToString(),
                                Description = reader["Description"].ToString(),
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                Status = reader["Status"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["AssignedToID"] != DBNull.Value)
                                task.AssignedToID = Convert.ToInt32(reader["AssignedToID"]);

                            if (reader["StartDate"] != DBNull.Value)
                                task.StartDate = Convert.ToDateTime(reader["StartDate"]);

                            if (reader["DueDate"] != DBNull.Value)
                                task.DueDate = Convert.ToDateTime(reader["DueDate"]);

                            if (reader["CompletedDate"] != DBNull.Value)
                                task.CompletedDate = Convert.ToDateTime(reader["CompletedDate"]);

                            if (reader["Notes"] != DBNull.Value)
                                task.Notes = reader["Notes"].ToString();

                            // Thông tin liên kết
                            if (reader["ProjectName"] != DBNull.Value)
                                task.Project = new Project { ProjectName = reader["ProjectName"].ToString() };

                            if (reader["AssignedToName"] != DBNull.Value)
                                task.AssignedTo = new Employee { FullName = reader["AssignedToName"].ToString() };

                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách công việc: {ex.Message}", ex);
            }

            return tasks;
        }

        public WorkTask GetTaskById(int taskId)
        {
            WorkTask task = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, e.FullName as AssignedToName
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        WHERE t.TaskID = @TaskID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", taskId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            task = new WorkTask
                            {
                                TaskID = Convert.ToInt32(reader["TaskID"]),
                                TaskCode = reader["TaskCode"].ToString(),
                                TaskName = reader["TaskName"].ToString(),
                                Description = reader["Description"].ToString(),
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                Status = reader["Status"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["AssignedToID"] != DBNull.Value)
                                task.AssignedToID = Convert.ToInt32(reader["AssignedToID"]);

                            if (reader["StartDate"] != DBNull.Value)
                                task.StartDate = Convert.ToDateTime(reader["StartDate"]);

                            if (reader["DueDate"] != DBNull.Value)
                                task.DueDate = Convert.ToDateTime(reader["DueDate"]);

                            if (reader["CompletedDate"] != DBNull.Value)
                                task.CompletedDate = Convert.ToDateTime(reader["CompletedDate"]);

                            if (reader["Notes"] != DBNull.Value)
                                task.Notes = reader["Notes"].ToString();

                            // Thông tin liên kết
                            if (reader["ProjectName"] != DBNull.Value)
                                task.Project = new Project { ProjectName = reader["ProjectName"].ToString() };

                            if (reader["AssignedToName"] != DBNull.Value)
                                task.AssignedTo = new Employee { FullName = reader["AssignedToName"].ToString() };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin công việc: {ex.Message}", ex);
            }

            return task;
        }

        public string GenerateTaskCode()
        {
            string prefix = "TASK";
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(TaskCode, 5, LEN(TaskCode)) AS CodeNumber
                        FROM Tasks 
                        WHERE TaskCode LIKE 'TASK%'
                        ORDER BY LEN(TaskCode) DESC, TaskCode DESC";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }
            catch
            {
                // Nếu có lỗi, sử dụng timestamp
                return prefix + DateTime.Now.ToString("yyyyMMddHHmm");
            }

            return prefix + nextNumber.ToString("D4");
        }

        public int AddTask(WorkTask task)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Tasks (
                            TaskCode, TaskName, Description, ProjectID, AssignedToID,
                            StartDate, DueDate, Status, Priority, CompletionPercentage,
                            Notes, CreatedAt, UpdatedAt
                        ) VALUES (
                            @TaskCode, @TaskName, @Description, @ProjectID, @AssignedToID,
                            @StartDate, @DueDate, @Status, @Priority, @CompletionPercentage,
                            @Notes, GETDATE(), GETDATE()
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskCode", task.TaskCode);
                    command.Parameters.AddWithValue("@TaskName", task.TaskName);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProjectID", task.ProjectID);
                    command.Parameters.AddWithValue("@Status", task.Status);
                    command.Parameters.AddWithValue("@Priority", task.Priority);
                    command.Parameters.AddWithValue("@CompletionPercentage", task.CompletionPercentage);

                    // Xử lý các trường nullable
                    if (task.AssignedToID.HasValue)
                        command.Parameters.AddWithValue("@AssignedToID", task.AssignedToID.Value);
                    else
                        command.Parameters.AddWithValue("@AssignedToID", DBNull.Value);

                    if (task.StartDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", task.StartDate.Value);
                    else
                        command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                    if (task.DueDate.HasValue)
                        command.Parameters.AddWithValue("@DueDate", task.DueDate.Value);
                    else
                        command.Parameters.AddWithValue("@DueDate", DBNull.Value);

                    command.Parameters.AddWithValue("@Notes", task.Notes ?? (object)DBNull.Value);

                    connection.Open();
                    int newTaskId = Convert.ToInt32(command.ExecuteScalar());
                    return newTaskId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm công việc mới: {ex.Message}", ex);
            }
        }

        public void UpdateTask(WorkTask task)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Tasks SET
                            TaskName = @TaskName,
                            Description = @Description,
                            ProjectID = @ProjectID,
                            AssignedToID = @AssignedToID,
                            StartDate = @StartDate,
                            DueDate = @DueDate,
                            CompletedDate = @CompletedDate,
                            Status = @Status,
                            Priority = @Priority,
                            CompletionPercentage = @CompletionPercentage,
                            Notes = @Notes,
                            UpdatedAt = GETDATE()
                        WHERE TaskID = @TaskID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", task.TaskID);
                    command.Parameters.AddWithValue("@TaskName", task.TaskName);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProjectID", task.ProjectID);
                    command.Parameters.AddWithValue("@Status", task.Status);
                    command.Parameters.AddWithValue("@Priority", task.Priority);
                    command.Parameters.AddWithValue("@CompletionPercentage", task.CompletionPercentage);

                    // Xử lý các trường nullable
                    if (task.AssignedToID.HasValue)
                        command.Parameters.AddWithValue("@AssignedToID", task.AssignedToID.Value);
                    else
                        command.Parameters.AddWithValue("@AssignedToID", DBNull.Value);

                    if (task.StartDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", task.StartDate.Value);
                    else
                        command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                    if (task.DueDate.HasValue)
                        command.Parameters.AddWithValue("@DueDate", task.DueDate.Value);
                    else
                        command.Parameters.AddWithValue("@DueDate", DBNull.Value);

                    if (task.CompletedDate.HasValue)
                        command.Parameters.AddWithValue("@CompletedDate", task.CompletedDate.Value);
                    else
                        command.Parameters.AddWithValue("@CompletedDate", DBNull.Value);

                    command.Parameters.AddWithValue("@Notes", task.Notes ?? (object)DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật công việc: {ex.Message}", ex);
            }
        }

        public void DeleteTask(int taskId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM Tasks WHERE TaskID = @TaskID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", taskId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa công việc: {ex.Message}", ex);
            }
        }

        public List<Project> GetAllProjects()
        {
            List<Project> projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT ProjectID, ProjectCode, ProjectName, Status FROM Projects ORDER BY ProjectName";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectCode = reader["ProjectCode"].ToString(),
                                ProjectName = reader["ProjectName"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách dự án: {ex.Message}", ex);
            }

            return projects;
        }

        public List<Employee> GetAvailableEmployees()
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT EmployeeID, EmployeeCode, FullName, DepartmentID
                        FROM Employees
                        WHERE Status = N'Đang làm việc'
                        ORDER BY FullName";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nhân viên: {ex.Message}", ex);
            }

            return employees;
        }
    }
}