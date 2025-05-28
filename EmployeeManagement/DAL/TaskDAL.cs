using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.DAL
{
    public class TaskDAL
    {
        private static string GetConnectionString()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy connection string: {ex.Message}", ex);
            }
        }

        public List<WorkTask> GetAllTasks()
        {
            var tasks = new List<WorkTask>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, p.ProjectCode, 
                               e.FullName as AssignedToName, e.EmployeeCode
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        ORDER BY t.CreatedAt DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new WorkTask
                                {
                                    TaskID = Convert.ToInt32(reader["TaskID"]),
                                    TaskCode = reader["TaskCode"]?.ToString() ?? "",
                                    TaskName = reader["TaskName"]?.ToString() ?? "",
                                    Description = reader["Description"]?.ToString() ?? "",
                                    ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                    Status = reader["Status"]?.ToString() ?? "Chưa bắt đầu",
                                    Priority = reader["Priority"]?.ToString() ?? "Trung bình",
                                    CompletionPercentage = reader["CompletionPercentage"] != DBNull.Value ?
                                        Convert.ToDecimal(reader["CompletionPercentage"]) : 0,
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                    Notes = reader["Notes"]?.ToString() ?? ""
                                };

                                // Handle nullable fields
                                if (reader["AssignedToID"] != DBNull.Value)
                                    task.AssignedToID = Convert.ToInt32(reader["AssignedToID"]);

                                if (reader["StartDate"] != DBNull.Value)
                                    task.StartDate = Convert.ToDateTime(reader["StartDate"]);

                                if (reader["DueDate"] != DBNull.Value)
                                    task.DueDate = Convert.ToDateTime(reader["DueDate"]);

                                if (reader["CompletedDate"] != DBNull.Value)
                                    task.CompletedDate = Convert.ToDateTime(reader["CompletedDate"]);

                                // Navigation properties - Project
                                if (reader["ProjectName"] != DBNull.Value)
                                {
                                    task.Project = new Project
                                    {
                                        ProjectID = task.ProjectID,
                                        ProjectName = reader["ProjectName"].ToString() ?? "",
                                        ProjectCode = reader["ProjectCode"]?.ToString() ?? ""
                                    };
                                }

                                // Navigation properties - Employee
                                if (reader["AssignedToName"] != DBNull.Value && task.AssignedToID.HasValue)
                                {
                                    task.AssignedTo = new Employee
                                    {
                                        EmployeeID = task.AssignedToID.Value,
                                        FullName = reader["AssignedToName"].ToString() ?? "",
                                        EmployeeCode = reader["EmployeeCode"]?.ToString() ?? ""
                                    };
                                }

                                tasks.Add(task);
                            }
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

        public WorkTask? GetTaskById(int taskId)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, p.ProjectCode,
                               e.FullName as AssignedToName, e.EmployeeCode
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        WHERE t.TaskID = @TaskID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskID", taskId);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var task = new WorkTask
                                {
                                    TaskID = Convert.ToInt32(reader["TaskID"]),
                                    TaskCode = reader["TaskCode"]?.ToString() ?? "",
                                    TaskName = reader["TaskName"]?.ToString() ?? "",
                                    Description = reader["Description"]?.ToString() ?? "",
                                    ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                    Status = reader["Status"]?.ToString() ?? "Chưa bắt đầu",
                                    Priority = reader["Priority"]?.ToString() ?? "Trung bình",
                                    CompletionPercentage = reader["CompletionPercentage"] != DBNull.Value ?
                                        Convert.ToDecimal(reader["CompletionPercentage"]) : 0,
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                    Notes = reader["Notes"]?.ToString() ?? ""
                                };

                                // Handle nullable fields
                                if (reader["AssignedToID"] != DBNull.Value)
                                    task.AssignedToID = Convert.ToInt32(reader["AssignedToID"]);

                                if (reader["StartDate"] != DBNull.Value)
                                    task.StartDate = Convert.ToDateTime(reader["StartDate"]);

                                if (reader["DueDate"] != DBNull.Value)
                                    task.DueDate = Convert.ToDateTime(reader["DueDate"]);

                                if (reader["CompletedDate"] != DBNull.Value)
                                    task.CompletedDate = Convert.ToDateTime(reader["CompletedDate"]);

                                // Navigation properties - Project
                                if (reader["ProjectName"] != DBNull.Value)
                                {
                                    task.Project = new Project
                                    {
                                        ProjectID = task.ProjectID,
                                        ProjectName = reader["ProjectName"].ToString() ?? "",
                                        ProjectCode = reader["ProjectCode"]?.ToString() ?? ""
                                    };
                                }

                                // Navigation properties - Employee
                                if (reader["AssignedToName"] != DBNull.Value && task.AssignedToID.HasValue)
                                {
                                    task.AssignedTo = new Employee
                                    {
                                        EmployeeID = task.AssignedToID.Value,
                                        FullName = reader["AssignedToName"].ToString() ?? "",
                                        EmployeeCode = reader["EmployeeCode"]?.ToString() ?? ""
                                    };
                                }

                                return task;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin công việc: {ex.Message}", ex);
            }
        }

        public string GenerateTaskCode()
        {
            const string prefix = "TASK";
            int nextNumber = 1;

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(TaskCode, 5, LEN(TaskCode)) AS CodeNumber
                        FROM Tasks 
                        WHERE TaskCode LIKE 'TASK%' AND LEN(TaskCode) > 4
                        AND ISNUMERIC(SUBSTRING(TaskCode, 5, LEN(TaskCode))) = 1
                        ORDER BY CAST(SUBSTRING(TaskCode, 5, LEN(TaskCode)) AS INT) DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int lastNumber))
                        {
                            nextNumber = lastNumber + 1;
                        }
                    }
                }
            }
            catch
            {
                // If error, use timestamp
                return prefix + DateTime.Now.ToString("yyyyMMddHHmm");
            }

            return prefix + nextNumber.ToString("D4");
        }

        public int AddTask(WorkTask task)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
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

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskCode", task.TaskCode ?? "");
                        command.Parameters.AddWithValue("@TaskName", task.TaskName ?? "");
                        command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ProjectID", task.ProjectID);
                        command.Parameters.AddWithValue("@Status", task.Status ?? "Chưa bắt đầu");
                        command.Parameters.AddWithValue("@Priority", task.Priority ?? "Trung bình");
                        command.Parameters.AddWithValue("@CompletionPercentage", task.CompletionPercentage);

                        // Handle nullable fields
                        command.Parameters.AddWithValue("@AssignedToID",
                            task.AssignedToID.HasValue ? task.AssignedToID.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@StartDate",
                            task.StartDate.HasValue ? task.StartDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DueDate",
                            task.DueDate.HasValue ? task.DueDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", task.Notes ?? (object)DBNull.Value);

                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
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
                using (var connection = new SqlConnection(GetConnectionString()))
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

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskID", task.TaskID);
                        command.Parameters.AddWithValue("@TaskName", task.TaskName ?? "");
                        command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ProjectID", task.ProjectID);
                        command.Parameters.AddWithValue("@Status", task.Status ?? "Chưa bắt đầu");
                        command.Parameters.AddWithValue("@Priority", task.Priority ?? "Trung bình");
                        command.Parameters.AddWithValue("@CompletionPercentage", task.CompletionPercentage);

                        // Handle nullable fields
                        command.Parameters.AddWithValue("@AssignedToID",
                            task.AssignedToID.HasValue ? task.AssignedToID.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@StartDate",
                            task.StartDate.HasValue ? task.StartDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DueDate",
                            task.DueDate.HasValue ? task.DueDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompletedDate",
                            task.CompletedDate.HasValue ? task.CompletedDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", task.Notes ?? (object)DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
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
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM Tasks WHERE TaskID = @TaskID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskID", taskId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                            throw new Exception("Không tìm thấy công việc để xóa");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa công việc: {ex.Message}", ex);
            }
        }

        public List<Project> GetAllProjects()
        {
            var projects = new List<Project>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT ProjectID, ProjectCode, ProjectName, Status, 
                               StartDate, EndDate, Budget, CompletionPercentage
                        FROM Projects 
                        WHERE Status NOT IN (N'Hủy bỏ', N'Đã hoàn thành')
                        ORDER BY ProjectName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var project = new Project
                                {
                                    ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                    ProjectCode = reader["ProjectCode"]?.ToString() ?? "",
                                    ProjectName = reader["ProjectName"]?.ToString() ?? "",
                                    Status = reader["Status"]?.ToString() ?? ""
                                };

                                // Add other properties if needed
                                if (reader["StartDate"] != DBNull.Value)
                                    project.StartDate = Convert.ToDateTime(reader["StartDate"]);

                                if (reader["EndDate"] != DBNull.Value)
                                    project.EndDate = Convert.ToDateTime(reader["EndDate"]);

                                if (reader["Budget"] != DBNull.Value)
                                    project.Budget = Convert.ToDecimal(reader["Budget"]);

                                if (reader["CompletionPercentage"] != DBNull.Value)
                                    project.CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]);

                                projects.Add(project);
                            }
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
            var employees = new List<Employee>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.DepartmentID,
                               d.DepartmentName, p.PositionName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        WHERE e.Status = N'Đang làm việc'
                        ORDER BY e.FullName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                                    FullName = reader["FullName"]?.ToString() ?? "",
                                    DepartmentID = reader["DepartmentID"] != DBNull.Value ?
                                        Convert.ToInt32(reader["DepartmentID"]) : 0
                                };

                                // Add department and position info if available
                                if (reader["DepartmentName"] != DBNull.Value)
                                {
                                    employee.Department = new Department
                                    {
                                        DepartmentID = employee.DepartmentID,
                                        DepartmentName = reader["DepartmentName"].ToString() ?? ""
                                    };
                                }

                                if (reader["PositionName"] != DBNull.Value)
                                {
                                    employee.Position = new Position
                                    {
                                        PositionName = reader["PositionName"].ToString() ?? ""
                                    };
                                }

                                employees.Add(employee);
                            }
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

        public List<WorkTask> GetTasksByProject(int projectId)
        {
            var tasks = new List<WorkTask>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, p.ProjectCode,
                               e.FullName as AssignedToName, e.EmployeeCode
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        WHERE t.ProjectID = @ProjectID
                        ORDER BY t.CreatedAt DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectID", projectId);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new WorkTask
                                {
                                    TaskID = Convert.ToInt32(reader["TaskID"]),
                                    TaskCode = reader["TaskCode"]?.ToString() ?? "",
                                    TaskName = reader["TaskName"]?.ToString() ?? "",
                                    ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                    Status = reader["Status"]?.ToString() ?? "Chưa bắt đầu",
                                    Priority = reader["Priority"]?.ToString() ?? "Trung bình",
                                    CompletionPercentage = reader["CompletionPercentage"] != DBNull.Value ?
                                        Convert.ToDecimal(reader["CompletionPercentage"]) : 0,
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                };

                                // Handle nullable fields and navigation properties
                                if (reader["Description"] != DBNull.Value)
                                    task.Description = reader["Description"].ToString();

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

                                // Navigation properties
                                if (reader["ProjectName"] != DBNull.Value)
                                {
                                    task.Project = new Project
                                    {
                                        ProjectID = task.ProjectID,
                                        ProjectName = reader["ProjectName"].ToString() ?? "",
                                        ProjectCode = reader["ProjectCode"]?.ToString() ?? ""
                                    };
                                }

                                if (reader["AssignedToName"] != DBNull.Value && task.AssignedToID.HasValue)
                                {
                                    task.AssignedTo = new Employee
                                    {
                                        EmployeeID = task.AssignedToID.Value,
                                        FullName = reader["AssignedToName"].ToString() ?? "",
                                        EmployeeCode = reader["EmployeeCode"]?.ToString() ?? ""
                                    };
                                }

                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải công việc theo dự án: {ex.Message}", ex);
            }

            return tasks;
        }

        public List<WorkTask> GetTasksByEmployee(int employeeId)
        {
            var tasks = new List<WorkTask>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.ProjectID, t.AssignedToID, t.StartDate, t.DueDate, 
                               t.CompletedDate, t.Status, t.Priority, t.CompletionPercentage, 
                               t.Notes, t.CreatedAt, t.UpdatedAt,
                               p.ProjectName, p.ProjectCode,
                               e.FullName as AssignedToName, e.EmployeeCode
                        FROM Tasks t
                        LEFT JOIN Projects p ON t.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        WHERE t.AssignedToID = @EmployeeID
                        ORDER BY t.CreatedAt DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new WorkTask
                                {
                                    TaskID = Convert.ToInt32(reader["TaskID"]),
                                    TaskCode = reader["TaskCode"]?.ToString() ?? "",
                                    TaskName = reader["TaskName"]?.ToString() ?? "",
                                    ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                    Status = reader["Status"]?.ToString() ?? "Chưa bắt đầu",
                                    Priority = reader["Priority"]?.ToString() ?? "Trung bình",
                                    CompletionPercentage = reader["CompletionPercentage"] != DBNull.Value ?
                                        Convert.ToDecimal(reader["CompletionPercentage"]) : 0,
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                };

                                // Handle nullable fields similar to GetTasksByProject
                                if (reader["Description"] != DBNull.Value)
                                    task.Description = reader["Description"].ToString();

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

                                // Navigation properties
                                if (reader["ProjectName"] != DBNull.Value)
                                {
                                    task.Project = new Project
                                    {
                                        ProjectID = task.ProjectID,
                                        ProjectName = reader["ProjectName"].ToString() ?? "",
                                        ProjectCode = reader["ProjectCode"]?.ToString() ?? ""
                                    };
                                }

                                if (reader["AssignedToName"] != DBNull.Value && task.AssignedToID.HasValue)
                                {
                                    task.AssignedTo = new Employee
                                    {
                                        EmployeeID = task.AssignedToID.Value,
                                        FullName = reader["AssignedToName"].ToString() ?? "",
                                        EmployeeCode = reader["EmployeeCode"]?.ToString() ?? ""
                                    };
                                }

                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải công việc theo nhân viên: {ex.Message}", ex);
            }

            return tasks;
        }

        public bool TaskCodeExists(string taskCode, int? excludeTaskId = null)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Tasks WHERE TaskCode = @TaskCode";

                    if (excludeTaskId.HasValue)
                        query += " AND TaskID != @TaskID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskCode", taskCode ?? "");

                        if (excludeTaskId.HasValue)
                            command.Parameters.AddWithValue("@TaskID", excludeTaskId.Value);

                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã công việc: {ex.Message}", ex);
            }
        }
    }
}