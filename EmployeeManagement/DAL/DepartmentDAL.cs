using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.DAL
{
    /// <summary>
    /// Data Access Layer cho Department
    /// </summary>
    public class DepartmentDAL
    {
        /// <summary>
        /// Lấy connection string từ app.config
        /// </summary>
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        /// <summary>
        /// Lấy tất cả phòng ban từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách DepartmentDTO</returns>
        public List<DepartmentDTO> GetAllDepartments()
        {
            List<DepartmentDTO> departmentList = new List<DepartmentDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT d.DepartmentID, d.DepartmentName, d.Description, 
                               d.ManagerID, d.CreatedAt, d.UpdatedAt,
                               e.FullName AS ManagerName,
                               (SELECT COUNT(*) FROM Employees WHERE DepartmentID = d.DepartmentID) AS EmployeeCount
                        FROM Departments d
                        LEFT JOIN Employees e ON d.ManagerID = e.EmployeeID
                        ORDER BY d.DepartmentName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DepartmentDTO dto = new DepartmentDTO
                            {
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"] != DBNull.Value ? reader["ManagerName"].ToString() : "Không xác định";
                            }

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            departmentList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu phòng ban: {ex.Message}", ex);
            }

            return departmentList;
        }

        /// <summary>
        /// Lấy thông tin của một phòng ban theo ID
        /// </summary>
        /// <param name="departmentId">ID của phòng ban</param>
        /// <returns>DepartmentDTO</returns>
        public DepartmentDTO GetDepartmentById(int departmentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT d.DepartmentID, d.DepartmentName, d.Description, 
                               d.ManagerID, d.CreatedAt, d.UpdatedAt,
                               e.FullName AS ManagerName,
                               (SELECT COUNT(*) FROM Employees WHERE DepartmentID = d.DepartmentID) AS EmployeeCount
                        FROM Departments d
                        LEFT JOIN Employees e ON d.ManagerID = e.EmployeeID
                        WHERE d.DepartmentID = @DepartmentID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentID", departmentId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DepartmentDTO dto = new DepartmentDTO
                            {
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"] != DBNull.Value ? reader["ManagerName"].ToString() : "Không xác định";
                            }

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            return dto;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin phòng ban: {ex.Message}", ex);
            }

            return null; // Không tìm thấy
        }

        /// <summary>
        /// Thêm phòng ban mới vào cơ sở dữ liệu
        /// </summary>
        /// <param name="department">Đối tượng Department hoặc DepartmentDTO</param>
        /// <returns>ID của phòng ban mới được thêm</returns>
        public int AddDepartment(object department)
        {
            Department dept;

            // Chuyển đổi từ DTO nếu cần
            if (department is DepartmentDTO dto)
            {
                dept = dto.ToDepartment();
            }
            else if (department is Department)
            {
                dept = (Department)department;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Department hoặc DepartmentDTO", nameof(department));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Departments (
                            DepartmentName, Description, ManagerID, CreatedAt, UpdatedAt
                        ) VALUES (
                            @DepartmentName, @Description, @ManagerID, GETDATE(), GETDATE()
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentName", dept.DepartmentName);

                    // Xử lý các tham số có thể null
                    if (string.IsNullOrEmpty(dept.Description))
                        command.Parameters.AddWithValue("@Description", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Description", dept.Description);

                    if (dept.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", dept.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    connection.Open();
                    int newDepartmentId = Convert.ToInt32(command.ExecuteScalar());

                    return newDepartmentId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm phòng ban vào cơ sở dữ liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin phòng ban
        /// </summary>
        /// <param name="department">Đối tượng Department hoặc DepartmentDTO</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateDepartment(object department)
        {
            Department dept;

            // Chuyển đổi từ DTO nếu cần
            if (department is DepartmentDTO dto)
            {
                dept = dto.ToDepartment();
            }
            else if (department is Department)
            {
                dept = (Department)department;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Department hoặc DepartmentDTO", nameof(department));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Departments SET
                            DepartmentName = @DepartmentName,
                            Description = @Description,
                            ManagerID = @ManagerID,
                            UpdatedAt = GETDATE()
                        WHERE DepartmentID = @DepartmentID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentID", dept.DepartmentID);
                    command.Parameters.AddWithValue("@DepartmentName", dept.DepartmentName);

                    // Xử lý các tham số có thể null
                    if (string.IsNullOrEmpty(dept.Description))
                        command.Parameters.AddWithValue("@Description", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Description", dept.Description);

                    if (dept.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", dept.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa phòng ban khỏi cơ sở dữ liệu
        /// </summary>
        /// <param name="departmentId">ID của phòng ban cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeleteDepartment(int departmentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Kiểm tra xem phòng ban có thể xóa được hay không
                    string checkQuery = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Employees WHERE DepartmentID = @DepartmentID) AS AssignedEmployees,
                            (SELECT COUNT(*) FROM Departments WHERE ManagerID IN 
                                (SELECT EmployeeID FROM Employees WHERE DepartmentID = @DepartmentID)) AS RelatedDepartments";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@DepartmentID", departmentId);

                    connection.Open();
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int assignedEmployees = Convert.ToInt32(reader["AssignedEmployees"]);
                            int relatedDepartments = Convert.ToInt32(reader["RelatedDepartments"]);

                            if (assignedEmployees > 0 || relatedDepartments > 0)
                            {
                                StringBuilder errorMessage = new StringBuilder("Không thể xóa phòng ban vì còn liên kết với:\n");

                                if (assignedEmployees > 0)
                                    errorMessage.AppendLine($"- {assignedEmployees} nhân viên thuộc phòng ban này");

                                if (relatedDepartments > 0)
                                    errorMessage.AppendLine($"- {relatedDepartments} phòng ban khác có người quản lý thuộc phòng ban này");

                                errorMessage.AppendLine("\nVui lòng cập nhật các liên kết trước khi xóa.");
                                throw new Exception(errorMessage.ToString());
                            }
                        }
                    }

                    // Nếu không có ràng buộc, tiến hành xóa
                    string deleteQuery = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@DepartmentID", departmentId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm phòng ban theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Văn bản tìm kiếm</param>
        /// <param name="hasManager">Lọc theo trạng thái quản lý</param>
        /// <returns>Danh sách DepartmentDTO</returns>
        public List<DepartmentDTO> SearchDepartments(string searchText, bool? hasManager = null)
        {
            List<DepartmentDTO> departmentList = new List<DepartmentDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT d.DepartmentID, d.DepartmentName, d.Description, 
                               d.ManagerID, d.CreatedAt, d.UpdatedAt,
                               e.FullName AS ManagerName,
                               (SELECT COUNT(*) FROM Employees WHERE DepartmentID = d.DepartmentID) AS EmployeeCount
                        FROM Departments d
                        LEFT JOIN Employees e ON d.ManagerID = e.EmployeeID
                        WHERE 1=1");

                    SqlCommand command = new SqlCommand();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            d.DepartmentName LIKE @SearchText OR 
                            d.Description LIKE @SearchText OR
                            e.FullName LIKE @SearchText)");
                        command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    }

                    if (hasManager.HasValue)
                    {
                        if (hasManager.Value)
                            queryBuilder.Append(" AND d.ManagerID IS NOT NULL");
                        else
                            queryBuilder.Append(" AND d.ManagerID IS NULL");
                    }

                    queryBuilder.Append(" ORDER BY d.DepartmentName");

                    command.CommandText = queryBuilder.ToString();
                    command.Connection = connection;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DepartmentDTO dto = new DepartmentDTO
                            {
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"] != DBNull.Value ? reader["ManagerName"].ToString() : "Không xác định";
                            }

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            departmentList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm phòng ban: {ex.Message}", ex);
            }

            return departmentList;
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên có thể được phân làm quản lý
        /// </summary>
        /// <returns>Danh sách employee</returns>
        public List<Employee> GetPotentialManagers()
        {
            List<Employee> managers = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT EmployeeID, EmployeeCode, FullName 
                        FROM Employees 
                        WHERE Status = N'Đang làm việc' 
                        ORDER BY FullName";

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
                                FullName = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách quản lý tiềm năng: {ex.Message}", ex);
            }

            return managers;
        }
    }
}