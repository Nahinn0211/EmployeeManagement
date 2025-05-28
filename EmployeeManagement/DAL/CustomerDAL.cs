using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using EmployeeManagement.Models.Entity; 

namespace EmployeeManagement.DAL
{
    public class CustomerDAL
    {
        #region Connection
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }
        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy tất cả khách hàng
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Sửa query: Không có Projects.CustomerID trong schema hiện tại
                    string query = @"
                        SELECT c.CustomerID, c.CustomerCode, c.CompanyName, c.ContactName, 
                               c.ContactTitle, c.Address, c.Phone, c.Email, c.Status, 
                               c.Notes, c.CreatedAt, c.UpdatedAt
                        FROM Customers c
                        ORDER BY c.CompanyName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(MapReaderToCustomer(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
            }

            return customers;
        }

        /// <summary>
        /// Lấy khách hàng theo ID
        /// </summary>
        public Customer GetCustomerById(int customerId)
        {
            Customer customer = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT CustomerID, CustomerCode, CompanyName, ContactName, 
                               ContactTitle, Address, Phone, Email, Status, 
                               Notes, CreatedAt, UpdatedAt
                        FROM Customers 
                        WHERE CustomerID = @CustomerID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", customerId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = MapReaderToCustomer(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin khách hàng: {ex.Message}", ex);
            }

            return customer;
        }

        /// <summary>
        /// Thêm khách hàng mới
        /// </summary>
        public int InsertCustomer(Customer customer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Customers (
                            CustomerCode, CompanyName, ContactName, ContactTitle, 
                            Address, Phone, Email, Status, Notes, CreatedAt, UpdatedAt
                        ) VALUES (
                            @CustomerCode, @CompanyName, @ContactName, @ContactTitle,
                            @Address, @Phone, @Email, @Status, @Notes, @CreatedAt, @UpdatedAt
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddCustomerParameters(command, customer);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    connection.Open();
                    int newCustomerId = Convert.ToInt32(command.ExecuteScalar());
                    return newCustomerId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Customers SET
                            CompanyName = @CompanyName,
                            ContactName = @ContactName,
                            ContactTitle = @ContactTitle,
                            Address = @Address,
                            Phone = @Phone,
                            Email = @Email,
                            Status = @Status,
                            Notes = @Notes,
                            UpdatedAt = @UpdatedAt
                        WHERE CustomerID = @CustomerID";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddCustomerParameters(command, customer);
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Kiểm tra ràng buộc trước khi xóa
                    var constraints = CheckCustomerConstraints(customerId);
                    if (constraints.HasConstraints)
                    {
                        throw new Exception(constraints.ErrorMessage);
                    }

                    string query = "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", customerId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa khách hàng: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation & Constraints

        /// <summary>
        /// Kiểm tra mã khách hàng có tồn tại không
        /// </summary>
        public bool IsCustomerCodeExists(string customerCode, int excludeCustomerId = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Customers WHERE CustomerCode = @CustomerCode";

                    if (excludeCustomerId > 0)
                    {
                        query += " AND CustomerID != @CustomerID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerCode", customerCode);

                    if (excludeCustomerId > 0)
                    {
                        command.Parameters.AddWithValue("@CustomerID", excludeCustomerId);
                    }

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra email có tồn tại không
        /// </summary>
        public bool IsEmailExists(string email, int excludeCustomerId = 0)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";

                    if (excludeCustomerId > 0)
                    {
                        query += " AND CustomerID != @CustomerID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);

                    if (excludeCustomerId > 0)
                    {
                        command.Parameters.AddWithValue("@CustomerID", excludeCustomerId);
                    }

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra ràng buộc trước khi xóa khách hàng
        /// </summary>
        public (bool HasConstraints, string ErrorMessage) CheckCustomerConstraints(int customerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Sửa query để phù hợp với schema thực tế
                    string query = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Documents WHERE CustomerID = @CustomerID) AS DocumentCount,
                            (SELECT COUNT(*) FROM Finance WHERE CustomerID = @CustomerID) AS FinanceCount";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", customerId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int documentCount = Convert.ToInt32(reader["DocumentCount"]);
                            int financeCount = Convert.ToInt32(reader["FinanceCount"]);

                            if (documentCount > 0 || financeCount > 0)
                            {
                                StringBuilder errorMessage = new StringBuilder("Không thể xóa khách hàng vì còn liên kết với:\n");

                                if (documentCount > 0)
                                    errorMessage.AppendLine($"- {documentCount} tài liệu");

                                if (financeCount > 0)
                                    errorMessage.AppendLine($"- {financeCount} giao dịch tài chính");

                                errorMessage.AppendLine("\nVui lòng xử lý các liên kết trước khi xóa.");
                                return (true, errorMessage.ToString());
                            }
                        }
                    }
                }

                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra ràng buộc: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search & Filter

        /// <summary>
        /// Tìm kiếm khách hàng theo điều kiện
        /// </summary>
        public List<Customer> SearchCustomers(string searchText = "", string status = "")
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT c.CustomerID, c.CustomerCode, c.CompanyName, c.ContactName, 
                               c.ContactTitle, c.Address, c.Phone, c.Email, c.Status, 
                               c.Notes, c.CreatedAt, c.UpdatedAt
                        FROM Customers c
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            c.CompanyName LIKE @SearchText OR 
                            c.CustomerCode LIKE @SearchText OR 
                            c.ContactName LIKE @SearchText OR 
                            c.Email LIKE @SearchText OR 
                            c.Phone LIKE @SearchText)");
                        parameters.Add(new SqlParameter("@SearchText", $"%{searchText}%"));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        queryBuilder.Append(" AND c.Status = @Status");
                        parameters.Add(new SqlParameter("@Status", status));
                    }

                    queryBuilder.Append(" ORDER BY c.CompanyName");

                    SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(MapReaderToCustomer(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", ex);
            }

            return customers;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Tạo mã khách hàng tự động
        /// </summary>
        public string GenerateCustomerCode()
        {
            string prefix = "KH";
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(CustomerCode, 3, LEN(CustomerCode)) AS CodeNumber
                        FROM Customers 
                        WHERE CustomerCode LIKE 'KH%' AND ISNUMERIC(SUBSTRING(CustomerCode, 3, LEN(CustomerCode))) = 1
                        ORDER BY CAST(SUBSTRING(CustomerCode, 3, LEN(CustomerCode)) AS INT) DESC";

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

        /// <summary>
        /// Map SqlDataReader to Customer object
        /// </summary>
        private Customer MapReaderToCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                CustomerCode = reader["CustomerCode"].ToString(),
                CompanyName = reader["CompanyName"].ToString(),
                ContactName = reader["ContactName"]?.ToString() ?? string.Empty,
                ContactTitle = reader["ContactTitle"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString() ?? string.Empty,
                Phone = reader["Phone"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Status = reader["Status"].ToString(),
                Notes = reader["Notes"]?.ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        /// <summary>
        /// Thêm parameters chung cho Customer
        /// </summary>
        private void AddCustomerParameters(SqlCommand command, Customer customer)
        {
            command.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode);
            command.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
            command.Parameters.AddWithValue("@ContactName", (object)customer.ContactName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactTitle", (object)customer.ContactTitle ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", customer.Status);
            command.Parameters.AddWithValue("@Notes", (object)customer.Notes ?? DBNull.Value);
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Lấy thống kê khách hàng
        /// </summary>
        public (int Total, int Active, int Paused, int Inactive) GetCustomerStatistics()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            COUNT(*) as Total,
                            SUM(CASE WHEN Status = N'Đang hợp tác' THEN 1 ELSE 0 END) as Active,
                            SUM(CASE WHEN Status = N'Tạm dừng' THEN 1 ELSE 0 END) as Paused,
                            SUM(CASE WHEN Status = N'Ngừng hợp tác' THEN 1 ELSE 0 END) as Inactive
                        FROM Customers";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                Convert.ToInt32(reader["Total"]),
                                Convert.ToInt32(reader["Active"]),
                                Convert.ToInt32(reader["Paused"]),
                                Convert.ToInt32(reader["Inactive"])
                            );
                        }
                    }
                }

                return (0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê khách hàng: {ex.Message}", ex);
            }
        }

        #endregion
    }
}