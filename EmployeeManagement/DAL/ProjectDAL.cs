using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.DAL
{
    public class ProjectDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        public List<Project> GetAllProjects()
        {
            List<Project> projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT p.ProjectID, p.ProjectCode, p.ProjectName, p.Description,
                               p.StartDate, p.EndDate, p.Budget, p.ManagerID, p.Status,
                               p.CompletionPercentage, p.Notes, p.CreatedAt, p.UpdatedAt,
                               e.FullName as ManagerName
                        FROM Projects p
                        LEFT JOIN Employees e ON p.ManagerID = e.EmployeeID
                        ORDER BY p.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project project = new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectCode = reader["ProjectCode"].ToString(),
                                ProjectName = reader["ProjectName"].ToString(),
                                Budget = Convert.ToDecimal(reader["Budget"]),
                                ManagerID = Convert.ToInt32(reader["ManagerID"]),
                                Status = reader["Status"].ToString(),
                                CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["Description"] != DBNull.Value)
                                project.Description = reader["Description"].ToString();

                            if (reader["StartDate"] != DBNull.Value)
                                project.StartDate = Convert.ToDateTime(reader["StartDate"]);

                            if (reader["EndDate"] != DBNull.Value)
                                project.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["Notes"] != DBNull.Value)
                                project.Notes = reader["Notes"].ToString();

                            // Thông tin Manager
                            if (reader["ManagerName"] != DBNull.Value)
                            {
                                project.Manager = new Employee
                                {
                                    EmployeeID = project.ManagerID,
                                    FullName = reader["ManagerName"].ToString()
                                };
                            }

                            projects.Add(project);
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

        public Project GetProjectById(int projectId)
        {
            Project project = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT p.ProjectID, p.ProjectCode, p.ProjectName, p.Description,
                               p.StartDate, p.EndDate, p.Budget, p.ManagerID, p.Status,
                               p.CompletionPercentage, p.Notes, p.CreatedAt, p.UpdatedAt,
                               e.FullName as ManagerName, e.Email as ManagerEmail, e.Phone as ManagerPhone
                        FROM Projects p
                        LEFT JOIN Employees e ON p.ManagerID = e.EmployeeID
                        WHERE p.ProjectID = @ProjectID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", projectId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            project = new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectCode = reader["ProjectCode"].ToString(),
                                ProjectName = reader["ProjectName"].ToString(),
                                Budget = Convert.ToDecimal(reader["Budget"]),
                                ManagerID = Convert.ToInt32(reader["ManagerID"]),
                                Status = reader["Status"].ToString(),
                                CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["Description"] != DBNull.Value)
                                project.Description = reader["Description"].ToString();

                            if (reader["StartDate"] != DBNull.Value)
                                project.StartDate = Convert.ToDateTime(reader["StartDate"]);

                            if (reader["EndDate"] != DBNull.Value)
                                project.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["Notes"] != DBNull.Value)
                                project.Notes = reader["Notes"].ToString();

                            // Thông tin Manager chi tiết
                            if (reader["ManagerName"] != DBNull.Value)
                            {
                                project.Manager = new Employee
                                {
                                    EmployeeID = project.ManagerID,
                                    FullName = reader["ManagerName"].ToString(),
                                    Email = reader["ManagerEmail"]?.ToString(),
                                    Phone = reader["ManagerPhone"]?.ToString()
                                };
                            }
                        }
                    }
                }

                // Load related data
                if (project != null)
                {
                    project.Employees = GetProjectEmployees(project.ProjectID);
                    project.Tasks = GetProjectTasks(project.ProjectID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin dự án: {ex.Message}", ex);
            }

            return project;
        }

        public string GenerateProjectCode()
        {
            string prefix = "PRJ";
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(ProjectCode, 4, LEN(ProjectCode)) AS CodeNumber
                        FROM Projects 
                        WHERE ProjectCode LIKE 'PRJ%'
                        ORDER BY LEN(ProjectCode) DESC, ProjectCode DESC";

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
                return prefix + DateTime.Now.ToString("yyyyMMddHHmm");
            }

            return prefix + nextNumber.ToString("D4");
        }

        public int AddProject(Project project)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Projects (
                            ProjectCode, ProjectName, Description, StartDate, EndDate,
                            Budget, ManagerID, Status, CompletionPercentage, Notes,
                            CreatedAt, UpdatedAt
                        ) VALUES (
                            @ProjectCode, @ProjectName, @Description, @StartDate, @EndDate,
                            @Budget, @ManagerID, @Status, @CompletionPercentage, @Notes,
                            GETDATE(), GETDATE()
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectCode", project.ProjectCode);
                    command.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                    command.Parameters.AddWithValue("@Budget", project.Budget);
                    command.Parameters.AddWithValue("@ManagerID", project.ManagerID);
                    command.Parameters.AddWithValue("@Status", project.Status);
                    command.Parameters.AddWithValue("@CompletionPercentage", project.CompletionPercentage);

                    // Xử lý các tham số nullable
                    command.Parameters.AddWithValue("@Description", project.Description ?? (object)DBNull.Value);

                    if (project.StartDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", project.StartDate.Value);
                    else
                        command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                    if (project.EndDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", project.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    command.Parameters.AddWithValue("@Notes", project.Notes ?? (object)DBNull.Value);

                    connection.Open();
                    int newProjectId = Convert.ToInt32(command.ExecuteScalar());
                    return newProjectId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm dự án mới: {ex.Message}", ex);
            }
        }

        public void UpdateProject(Project project)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Projects SET
                            ProjectName = @ProjectName,
                            Description = @Description,
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            Budget = @Budget,
                            ManagerID = @ManagerID,
                            Status = @Status,
                            CompletionPercentage = @CompletionPercentage,
                            Notes = @Notes,
                            UpdatedAt = GETDATE()
                        WHERE ProjectID = @ProjectID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", project.ProjectID);
                    command.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                    command.Parameters.AddWithValue("@Budget", project.Budget);
                    command.Parameters.AddWithValue("@ManagerID", project.ManagerID);
                    command.Parameters.AddWithValue("@Status", project.Status);
                    command.Parameters.AddWithValue("@CompletionPercentage", project.CompletionPercentage);

                    // Xử lý các tham số nullable
                    command.Parameters.AddWithValue("@Description", project.Description ?? (object)DBNull.Value);

                    if (project.StartDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", project.StartDate.Value);
                    else
                        command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                    if (project.EndDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", project.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    command.Parameters.AddWithValue("@Notes", project.Notes ?? (object)DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật dự án: {ex.Message}", ex);
            }
        }

        public void DeleteProject(int projectId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Kiểm tra các ràng buộc trước khi xóa
                    string checkQuery = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Tasks WHERE ProjectID = @ProjectID) AS TaskCount,
                            (SELECT COUNT(*) FROM ProjectEmployees WHERE ProjectID = @ProjectID) AS EmployeeCount,
                            (SELECT COUNT(*) FROM Documents WHERE ProjectID = @ProjectID) AS DocumentCount,
                            (SELECT COUNT(*) FROM Finance WHERE ProjectID = @ProjectID) AS FinanceCount";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ProjectID", projectId);

                    connection.Open();
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int taskCount = Convert.ToInt32(reader["TaskCount"]);
                            int employeeCount = Convert.ToInt32(reader["EmployeeCount"]);
                            int documentCount = Convert.ToInt32(reader["DocumentCount"]);
                            int financeCount = Convert.ToInt32(reader["FinanceCount"]);

                            if (taskCount > 0 || employeeCount > 0 || documentCount > 0 || financeCount > 0)
                            {
                                var errorMessage = "Không thể xóa dự án vì còn liên kết với:\n";
                                if (taskCount > 0)
                                    errorMessage += $"- {taskCount} công việc\n";
                                if (employeeCount > 0)
                                    errorMessage += $"- {employeeCount} nhân viên tham gia\n";
                                if (documentCount > 0)
                                    errorMessage += $"- {documentCount} tài liệu\n";
                                if (financeCount > 0)
                                    errorMessage += $"- {financeCount} giao dịch tài chính\n";
                                errorMessage += "Vui lòng xóa các liên kết trước khi xóa dự án.";

                                throw new Exception(errorMessage);
                            }
                        }
                    }

                    // Nếu không có ràng buộc, tiến hành xóa
                    string deleteQuery = "DELETE FROM Projects WHERE ProjectID = @ProjectID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@ProjectID", projectId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa dự án: {ex.Message}", ex);
            }
        }

        public List<Employee> GetAvailableManagers()
        {
            List<Employee> managers = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.Email, e.Phone,
                               d.DepartmentName, p.PositionName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        WHERE e.Status = N'Đang làm việc'
                        ORDER BY e.FullName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            managers.Add(new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách quản lý: {ex.Message}", ex);
            }

            return managers;
        }

        public EmployeeManagement.Models.DTO.ProjectStatistics GetProjectStatistics()
        {
            EmployeeManagement.Models.DTO.ProjectStatistics stats = new EmployeeManagement.Models.DTO.ProjectStatistics();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            COUNT(*) as TotalProjects,
                            SUM(CASE WHEN Status = N'Đang thực hiện' THEN 1 ELSE 0 END) as ActiveProjects,
                            SUM(CASE WHEN Status = N'Hoàn thành' THEN 1 ELSE 0 END) as CompletedProjects,
                            SUM(CASE WHEN Status = N'Tạm dừng' THEN 1 ELSE 0 END) as OnHoldProjects,
                            SUM(CASE WHEN Status = N'Khởi tạo' THEN 1 ELSE 0 END) as InitialProjects,
                            SUM(CASE WHEN Status = N'Hủy bỏ' THEN 1 ELSE 0 END) as CancelledProjects,
                            ISNULL(SUM(Budget), 0) as TotalBudget,
                            ISNULL(AVG(Budget), 0) as AverageBudget,
                            ISNULL(AVG(CompletionPercentage), 0) as AverageCompletion
                        FROM Projects";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalProjects = Convert.ToInt32(reader["TotalProjects"]);
                            stats.ActiveProjects = Convert.ToInt32(reader["ActiveProjects"]);
                            stats.CompletedProjects = Convert.ToInt32(reader["CompletedProjects"]);
                            stats.OnHoldProjects = Convert.ToInt32(reader["OnHoldProjects"]);
                            stats.InitialProjects = Convert.ToInt32(reader["InitialProjects"]);
                            stats.CancelledProjects = Convert.ToInt32(reader["CancelledProjects"]);
                            stats.TotalBudget = Convert.ToDecimal(reader["TotalBudget"]);
                            stats.AverageBudget = Convert.ToDecimal(reader["AverageBudget"]);
                            stats.AverageCompletion = Convert.ToDecimal(reader["AverageCompletion"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thống kê dự án: {ex.Message}", ex);
            }

            return stats;
        }

        private List<Employee> GetProjectEmployees(int projectId)
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.Email, e.Phone,
                               pe.RoleInProject, pe.JoinDate
                        FROM ProjectEmployees pe
                        INNER JOIN Employees e ON pe.EmployeeID = e.EmployeeID
                        WHERE pe.ProjectID = @ProjectID AND pe.LeaveDate IS NULL
                        ORDER BY pe.JoinDate";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", projectId);
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
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nhân viên dự án: {ex.Message}", ex);
            }

            return employees;
        }

        private List<WorkTask> GetProjectTasks(int projectId)
        {
            List<WorkTask> tasks = new List<WorkTask>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Status, t.Priority,
                               t.StartDate, t.DueDate, t.CompletionPercentage,
                               e.FullName as AssignedToName
                        FROM Tasks t
                        LEFT JOIN Employees e ON t.AssignedToID = e.EmployeeID
                        WHERE t.ProjectID = @ProjectID
                        ORDER BY t.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", projectId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new WorkTask
                            {
                                TaskID = Convert.ToInt32(reader["TaskID"]),
                                TaskCode = reader["TaskCode"].ToString(),
                                TaskName = reader["TaskName"].ToString(),
                                Status = reader["Status"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"])
                            };

                            if (reader["StartDate"] != DBNull.Value)
                                task.StartDate = Convert.ToDateTime(reader["StartDate"]);

                            if (reader["DueDate"] != DBNull.Value)
                                task.DueDate = Convert.ToDateTime(reader["DueDate"]);

                            if (reader["AssignedToName"] != DBNull.Value)
                                task.AssignedTo = new Employee { FullName = reader["AssignedToName"].ToString() };

                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách công việc dự án: {ex.Message}", ex);
            }

            return tasks;
        }
    }
}