using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using EmployeeManagement.Models;

namespace EmployeeManagement.DAL
{
    public class DocumentDAL
    {
        #region Connection
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }
        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy tất cả tài liệu
        /// </summary>
        public List<Document> GetAllDocuments()
        {
            List<Document> documents = new List<Document>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT d.DocumentID, d.DocumentCode, d.DocumentName, d.Description,
                               d.FilePath, d.FileType, d.ProjectID, d.EmployeeID, d.CustomerID,
                               d.UploadedByID, d.DocumentType, d.CreatedAt, d.UpdatedAt,
                               p.ProjectName, e.FullName as EmployeeName, c.CompanyName as CustomerName,
                               u.FullName as UploadedByName
                        FROM Documents d
                        LEFT JOIN Projects p ON d.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON d.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON d.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON d.UploadedByID = u.UserID
                        ORDER BY d.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            documents.Add(MapReaderToDocument(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách tài liệu: {ex.Message}", ex);
            }

            return documents;
        }

        /// <summary>
        /// Lấy tài liệu theo ID
        /// </summary>
        public Document GetDocumentById(int documentId)
        {
            Document document = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT d.DocumentID, d.DocumentCode, d.DocumentName, d.Description,
                               d.FilePath, d.FileType, d.ProjectID, d.EmployeeID, d.CustomerID,
                               d.UploadedByID, d.DocumentType, d.CreatedAt, d.UpdatedAt,
                               p.ProjectName, e.FullName as EmployeeName, c.CompanyName as CustomerName,
                               u.FullName as UploadedByName
                        FROM Documents d
                        LEFT JOIN Projects p ON d.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON d.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON d.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON d.UploadedByID = u.UserID
                        WHERE d.DocumentID = @DocumentID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DocumentID", documentId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            document = MapReaderToDocument(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin tài liệu: {ex.Message}", ex);
            }

            return document;
        }

        /// <summary>
        /// Thêm tài liệu mới
        /// </summary>
        public int InsertDocument(Document document)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Documents (
                            DocumentCode, DocumentName, Description, FilePath, FileType,
                            ProjectID, EmployeeID, CustomerID, UploadedByID, DocumentType,
                            CreatedAt, UpdatedAt
                        ) VALUES (
                            @DocumentCode, @DocumentName, @Description, @FilePath, @FileType,
                            @ProjectID, @EmployeeID, @CustomerID, @UploadedByID, @DocumentType,
                            @CreatedAt, @UpdatedAt
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddDocumentParameters(command, document);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    connection.Open();
                    int newDocumentId = Convert.ToInt32(command.ExecuteScalar());
                    return newDocumentId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài liệu
        /// </summary>
        public bool UpdateDocument(Document document)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Documents SET
                            DocumentName = @DocumentName,
                            Description = @Description,
                            FilePath = @FilePath,
                            FileType = @FileType,
                            ProjectID = @ProjectID,
                            EmployeeID = @EmployeeID,
                            CustomerID = @CustomerID,
                            DocumentType = @DocumentType,
                            UpdatedAt = @UpdatedAt
                        WHERE DocumentID = @DocumentID";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddDocumentParameters(command, document);
                    command.Parameters.AddWithValue("@DocumentID", document.DocumentID);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        public bool DeleteDocument(int documentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM Documents WHERE DocumentID = @DocumentID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DocumentID", documentId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa tài liệu: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation & Constraints

        /// <summary>
        /// Kiểm tra mã tài liệu có tồn tại không
        /// </summary>
        public bool IsDocumentCodeExists(string documentCode, int excludeDocumentId = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Documents WHERE DocumentCode = @DocumentCode";

                    if (excludeDocumentId > 0)
                    {
                        query += " AND DocumentID != @DocumentID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DocumentCode", documentCode);

                    if (excludeDocumentId > 0)
                    {
                        command.Parameters.AddWithValue("@DocumentID", excludeDocumentId);
                    }

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã tài liệu: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search & Filter

        /// <summary>
        /// Tìm kiếm tài liệu theo điều kiện
        /// </summary>
        public List<Document> SearchDocuments(string searchText = "", string documentType = "",
            int? projectId = null, int? customerId = null, int? employeeId = null)
        {
            List<Document> documents = new List<Document>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT d.DocumentID, d.DocumentCode, d.DocumentName, d.Description,
                               d.FilePath, d.FileType, d.ProjectID, d.EmployeeID, d.CustomerID,
                               d.UploadedByID, d.DocumentType, d.CreatedAt, d.UpdatedAt,
                               p.ProjectName, e.FullName as EmployeeName, c.CompanyName as CustomerName,
                               u.FullName as UploadedByName
                        FROM Documents d
                        LEFT JOIN Projects p ON d.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON d.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON d.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON d.UploadedByID = u.UserID
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            d.DocumentName LIKE @SearchText OR 
                            d.DocumentCode LIKE @SearchText OR 
                            d.Description LIKE @SearchText OR
                            p.ProjectName LIKE @SearchText OR
                            c.CompanyName LIKE @SearchText OR
                            e.FullName LIKE @SearchText)");
                        parameters.Add(new SqlParameter("@SearchText", $"%{searchText}%"));
                    }

                    if (!string.IsNullOrWhiteSpace(documentType))
                    {
                        queryBuilder.Append(" AND d.DocumentType = @DocumentType");
                        parameters.Add(new SqlParameter("@DocumentType", documentType));
                    }

                    if (projectId.HasValue && projectId.Value > 0)
                    {
                        queryBuilder.Append(" AND d.ProjectID = @ProjectID");
                        parameters.Add(new SqlParameter("@ProjectID", projectId.Value));
                    }

                    if (customerId.HasValue && customerId.Value > 0)
                    {
                        queryBuilder.Append(" AND d.CustomerID = @CustomerID");
                        parameters.Add(new SqlParameter("@CustomerID", customerId.Value));
                    }

                    if (employeeId.HasValue && employeeId.Value > 0)
                    {
                        queryBuilder.Append(" AND d.EmployeeID = @EmployeeID");
                        parameters.Add(new SqlParameter("@EmployeeID", employeeId.Value));
                    }

                    queryBuilder.Append(" ORDER BY d.CreatedAt DESC");

                    SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            documents.Add(MapReaderToDocument(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm tài liệu: {ex.Message}", ex);
            }

            return documents;
        }

        /// <summary>
        /// Lấy tài liệu theo dự án
        /// </summary>
        public List<Document> GetDocumentsByProject(int projectId)
        {
            return SearchDocuments("", "", projectId);
        }

        /// <summary>
        /// Lấy tài liệu theo khách hàng
        /// </summary>
        public List<Document> GetDocumentsByCustomer(int customerId)
        {
            return SearchDocuments("", "", null, customerId);
        }

        /// <summary>
        /// Lấy tài liệu theo nhân viên
        /// </summary>
        public List<Document> GetDocumentsByEmployee(int employeeId)
        {
            return SearchDocuments("", "", null, null, employeeId);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Tạo mã tài liệu tự động
        /// </summary>
        public string GenerateDocumentCode()
        {
            string prefix = "DOC";
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(DocumentCode, 4, LEN(DocumentCode)) AS CodeNumber
                        FROM Documents 
                        WHERE DocumentCode LIKE 'DOC%' AND ISNUMERIC(SUBSTRING(DocumentCode, 4, LEN(DocumentCode))) = 1
                        ORDER BY CAST(SUBSTRING(DocumentCode, 4, LEN(DocumentCode)) AS INT) DESC";

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
        /// Lấy danh sách Projects cho dropdown
        /// </summary>
        public List<Project> GetProjectsForDropdown()
        {
            List<Project> projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT ProjectID, ProjectName FROM Projects ORDER BY ProjectName";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectName = reader["ProjectName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dự án: {ex.Message}", ex);
            }

            return projects;
        }

        /// <summary>
        /// Lấy danh sách Customers cho dropdown
        /// </summary>
        public List<Customer> GetCustomersForDropdown()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT CustomerID, CompanyName FROM Customers WHERE Status = N'Đang hợp tác' ORDER BY CompanyName";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CompanyName = reader["CompanyName"].ToString()
                            });
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
        /// Lấy danh sách Employees cho dropdown
        /// </summary>
        public List<Employee> GetEmployeesForDropdown()
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT EmployeeID, FullName FROM Employees WHERE Status = N'Đang làm việc' ORDER BY FullName";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FullName = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }

            return employees;
        }

        /// <summary>
        /// Map SqlDataReader to Document object
        /// </summary>
        private Document MapReaderToDocument(SqlDataReader reader)
        {
            var document = new Document
            {
                DocumentID = Convert.ToInt32(reader["DocumentID"]),
                DocumentCode = reader["DocumentCode"].ToString(),
                DocumentName = reader["DocumentName"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                FilePath = reader["FilePath"].ToString(),
                FileType = reader["FileType"]?.ToString() ?? string.Empty,
                ProjectID = reader["ProjectID"] == DBNull.Value ? null : Convert.ToInt32(reader["ProjectID"]),
                EmployeeID = reader["EmployeeID"] == DBNull.Value ? null : Convert.ToInt32(reader["EmployeeID"]),
                CustomerID = reader["CustomerID"] == DBNull.Value ? null : Convert.ToInt32(reader["CustomerID"]),
                UploadedByID = Convert.ToInt32(reader["UploadedByID"]),
                DocumentType = reader["DocumentType"]?.ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
            };

            // Set navigation properties if available
            if (reader["ProjectName"] != DBNull.Value)
            {
                document.Project = new Project { ProjectName = reader["ProjectName"].ToString() };
            }

            if (reader["EmployeeName"] != DBNull.Value)
            {
                document.Employee = new Employee { FullName = reader["EmployeeName"].ToString() };
            }

            if (reader["CustomerName"] != DBNull.Value)
            {
                document.Customer = new Customer { CompanyName = reader["CustomerName"].ToString() };
            }

            if (reader["UploadedByName"] != DBNull.Value)
            {
                document.UploadedBy = new User { FullName = reader["UploadedByName"].ToString() };
            }

            // Calculate file size
            document.FileSize = document.GetFormattedFileSize();

            return document;
        }

        /// <summary>
        /// Thêm parameters chung cho Document
        /// </summary>
        private void AddDocumentParameters(SqlCommand command, Document document)
        {
            command.Parameters.AddWithValue("@DocumentCode", document.DocumentCode);
            command.Parameters.AddWithValue("@DocumentName", document.DocumentName);
            command.Parameters.AddWithValue("@Description", (object)document.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@FilePath", document.FilePath);
            command.Parameters.AddWithValue("@FileType", (object)document.FileType ?? DBNull.Value);
            command.Parameters.AddWithValue("@UploadedByID", document.UploadedByID);
            command.Parameters.AddWithValue("@DocumentType", (object)document.DocumentType ?? DBNull.Value);

            // Handle nullable foreign keys
            command.Parameters.AddWithValue("@ProjectID", (object)document.ProjectID ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmployeeID", (object)document.EmployeeID ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerID", (object)document.CustomerID ?? DBNull.Value);
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Lấy thống kê tài liệu
        /// </summary>
        public (int Total, int Projects, int Customers, int Employees, int General) GetDocumentStatistics()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            COUNT(*) as Total,
                            SUM(CASE WHEN ProjectID IS NOT NULL THEN 1 ELSE 0 END) as Projects,
                            SUM(CASE WHEN CustomerID IS NOT NULL THEN 1 ELSE 0 END) as Customers,
                            SUM(CASE WHEN EmployeeID IS NOT NULL THEN 1 ELSE 0 END) as Employees,
                            SUM(CASE WHEN ProjectID IS NULL AND CustomerID IS NULL AND EmployeeID IS NULL THEN 1 ELSE 0 END) as General
                        FROM Documents";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                Convert.ToInt32(reader["Total"]),
                                Convert.ToInt32(reader["Projects"]),
                                Convert.ToInt32(reader["Customers"]),
                                Convert.ToInt32(reader["Employees"]),
                                Convert.ToInt32(reader["General"])
                            );
                        }
                    }
                }

                return (0, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài liệu: {ex.Message}", ex);
            }
        }

        #endregion
    }
}