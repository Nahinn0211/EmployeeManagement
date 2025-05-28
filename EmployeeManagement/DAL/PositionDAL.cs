using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.DAL
{
    /// <summary>
    /// Data Access Layer cho Position
    /// </summary>
    public class PositionDAL
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo với connection string mặc định
        /// </summary>
        public PositionDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        /// <summary>
        /// Khởi tạo với connection string chỉ định
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối</param>
        public PositionDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy tất cả chức vụ từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách PositionDTO</returns>
        public List<PositionDTO> GetAllPositions()
        {
            List<PositionDTO> positionList = new List<PositionDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT p.PositionID, p.PositionName, p.Description, p.BaseSalary, 
                               p.CreatedAt, p.UpdatedAt,
                               (SELECT COUNT(*) FROM Employees WHERE PositionID = p.PositionID) AS EmployeeCount
                        FROM Positions p
                        ORDER BY p.PositionName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PositionDTO dto = new PositionDTO
                            {
                                PositionID = Convert.ToInt32(reader["PositionID"]),
                                PositionName = reader["PositionName"].ToString(),
                                BaseSalary = Convert.ToDecimal(reader["BaseSalary"]),
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["Description"] != DBNull.Value)
                                dto.Description = reader["Description"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
                            else
                                dto.UpdatedAt = dto.CreatedAt;

                            positionList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu chức vụ: {ex.Message}", ex);
            }

            return positionList;
        }

        /// <summary>
        /// Lấy thông tin của một chức vụ theo ID
        /// </summary>
        /// <param name="positionId">ID của chức vụ</param>
        /// <returns>PositionDTO</returns>
        public PositionDTO GetPositionById(int positionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT p.PositionID, p.PositionName, p.Description, p.BaseSalary, 
                               p.CreatedAt, p.UpdatedAt,
                               (SELECT COUNT(*) FROM Employees WHERE PositionID = p.PositionID) AS EmployeeCount
                        FROM Positions p
                        WHERE p.PositionID = @PositionID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionID", positionId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PositionDTO dto = new PositionDTO
                            {
                                PositionID = Convert.ToInt32(reader["PositionID"]),
                                PositionName = reader["PositionName"].ToString(),
                                BaseSalary = Convert.ToDecimal(reader["BaseSalary"]),
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["Description"] != DBNull.Value)
                                dto.Description = reader["Description"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
                            else
                                dto.UpdatedAt = dto.CreatedAt;

                            return dto;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin chức vụ: {ex.Message}", ex);
            }

            return null; // Không tìm thấy
        }

        /// <summary>
        /// Thêm chức vụ mới vào cơ sở dữ liệu
        /// </summary>
        /// <param name="position">Đối tượng Position hoặc PositionDTO</param>
        /// <returns>ID của chức vụ mới được thêm</returns>
        public int AddPosition(object position)
        {
            Position pos;

            // Chuyển đổi từ DTO nếu cần
            if (position is PositionDTO dto)
            {
                pos = dto.ToPosition();
            }
            else if (position is Position)
            {
                pos = (Position)position;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Position hoặc PositionDTO", nameof(position));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        INSERT INTO Positions (
                            PositionName, Description, BaseSalary, CreatedAt, UpdatedAt
                        ) VALUES (
                            @PositionName, @Description, @BaseSalary, GETDATE(), GETDATE()
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionName", pos.PositionName);

                    // Xử lý các tham số có thể null
                    if (string.IsNullOrEmpty(pos.Description))
                        command.Parameters.AddWithValue("@Description", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Description", pos.Description);

                    command.Parameters.AddWithValue("@BaseSalary", pos.BaseSalary);

                    connection.Open();
                    int newPositionId = Convert.ToInt32(command.ExecuteScalar());
                    return newPositionId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm chức vụ vào cơ sở dữ liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin chức vụ
        /// </summary>
        /// <param name="position">Đối tượng Position hoặc PositionDTO</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdatePosition(object position)
        {
            Position pos;

            // Chuyển đổi từ DTO nếu cần
            if (position is PositionDTO dto)
            {
                pos = dto.ToPosition();
            }
            else if (position is Position)
            {
                pos = (Position)position;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Position hoặc PositionDTO", nameof(position));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        UPDATE Positions SET
                            PositionName = @PositionName,
                            Description = @Description,
                            BaseSalary = @BaseSalary,
                            UpdatedAt = GETDATE()
                        WHERE PositionID = @PositionID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionID", pos.PositionID);
                    command.Parameters.AddWithValue("@PositionName", pos.PositionName);
                    command.Parameters.AddWithValue("@BaseSalary", pos.BaseSalary);

                    // Xử lý các tham số có thể null
                    if (string.IsNullOrEmpty(pos.Description))
                        command.Parameters.AddWithValue("@Description", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Description", pos.Description);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa chức vụ khỏi cơ sở dữ liệu
        /// </summary>
        /// <param name="positionId">ID của chức vụ cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeletePosition(int positionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Kiểm tra xem chức vụ có thể xóa được không
                    string checkQuery = @"
                        SELECT COUNT(*) FROM Employees WHERE PositionID = @PositionID";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@PositionID", positionId);

                    connection.Open();
                    int employeeCount = (int)checkCommand.ExecuteScalar();

                    if (employeeCount > 0)
                    {
                        throw new InvalidOperationException(
                            $"Không thể xóa chức vụ này vì đang được gán cho {employeeCount} nhân viên. " +
                            "Vui lòng chuyển nhân viên sang chức vụ khác trước khi xóa.");
                    }

                    // Nếu không có ràng buộc, tiến hành xóa
                    string deleteQuery = "DELETE FROM Positions WHERE PositionID = @PositionID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@PositionID", positionId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm chức vụ theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Văn bản tìm kiếm</param>
        /// <param name="minSalary">Lương cơ bản tối thiểu</param>
        /// <param name="maxSalary">Lương cơ bản tối đa</param>
        /// <returns>Danh sách PositionDTO</returns>
        public List<PositionDTO> SearchPositions(string searchText, decimal? minSalary = null, decimal? maxSalary = null)
        {
            List<PositionDTO> positionList = new List<PositionDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT p.PositionID, p.PositionName, p.Description, p.BaseSalary, 
                               p.CreatedAt, p.UpdatedAt,
                               (SELECT COUNT(*) FROM Employees WHERE PositionID = p.PositionID) AS EmployeeCount
                        FROM Positions p
                        WHERE 1=1");

                    SqlCommand command = new SqlCommand();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            p.PositionName LIKE @SearchText OR 
                            p.Description LIKE @SearchText)");
                        command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    }

                    if (minSalary.HasValue)
                    {
                        queryBuilder.Append(" AND p.BaseSalary >= @MinSalary");
                        command.Parameters.AddWithValue("@MinSalary", minSalary.Value);
                    }

                    if (maxSalary.HasValue)
                    {
                        queryBuilder.Append(" AND p.BaseSalary <= @MaxSalary");
                        command.Parameters.AddWithValue("@MaxSalary", maxSalary.Value);
                    }

                    queryBuilder.Append(" ORDER BY p.PositionName");

                    command.CommandText = queryBuilder.ToString();
                    command.Connection = connection;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PositionDTO dto = new PositionDTO
                            {
                                PositionID = Convert.ToInt32(reader["PositionID"]),
                                PositionName = reader["PositionName"].ToString(),
                                BaseSalary = Convert.ToDecimal(reader["BaseSalary"]),
                                EmployeeCount = Convert.ToInt32(reader["EmployeeCount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["Description"] != DBNull.Value)
                                dto.Description = reader["Description"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
                            else
                                dto.UpdatedAt = dto.CreatedAt;

                            positionList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm chức vụ: {ex.Message}", ex);
            }

            return positionList;
        }

        /// <summary>
        /// Kiểm tra tên chức vụ đã tồn tại chưa
        /// </summary>
        /// <param name="positionName">Tên chức vụ cần kiểm tra</param>
        /// <param name="excludePositionId">ID chức vụ cần loại trừ (nếu là cập nhật)</param>
        /// <returns>true nếu tên đã tồn tại</returns>
        public bool IsPositionNameExists(string positionName, int? excludePositionId = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Positions WHERE PositionName = @PositionName";

                    if (excludePositionId.HasValue)
                        query += " AND PositionID <> @PositionID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionName", positionName);

                    if (excludePositionId.HasValue)
                        command.Parameters.AddWithValue("@PositionID", excludePositionId.Value);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tên chức vụ: {ex.Message}", ex);
            }
        }
    }
}