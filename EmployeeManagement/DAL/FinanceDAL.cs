using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.DAL
{
    public class FinanceDAL
    {
        #region Connection
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }
        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy tất cả giao dịch tài chính
        /// </summary>
        public List<Finance> GetAllFinances()
        {
            List<Finance> finances = new List<Finance>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT f.FinanceID, f.TransactionCode, f.Amount, f.TransactionType, 
                               f.Category, f.ProjectID, f.EmployeeID, f.CustomerID, 
                               f.TransactionDate, f.Description, f.PaymentMethod, 
                               f.ReferenceNo, f.Status, f.RecordedByID, f.CreatedAt,
                               p.ProjectName, e.FullName as EmployeeName, 
                               c.CompanyName as CustomerName, u.FullName as RecordedByName
                        FROM Finance f
                        LEFT JOIN Projects p ON f.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON f.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON f.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON f.RecordedByID = u.UserID
                        ORDER BY f.TransactionDate DESC, f.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            finances.Add(MapReaderToFinance(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách giao dịch tài chính: {ex.Message}", ex);
            }

            return finances;
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo ID
        /// </summary>
        public Finance GetFinanceById(int financeId)
        {
            Finance finance = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT f.FinanceID, f.TransactionCode, f.Amount, f.TransactionType, 
                               f.Category, f.ProjectID, f.EmployeeID, f.CustomerID, 
                               f.TransactionDate, f.Description, f.PaymentMethod, 
                               f.ReferenceNo, f.Status, f.RecordedByID, f.CreatedAt,
                               p.ProjectName, e.FullName as EmployeeName, 
                               c.CompanyName as CustomerName, u.FullName as RecordedByName
                        FROM Finance f
                        LEFT JOIN Projects p ON f.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON f.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON f.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON f.RecordedByID = u.UserID
                        WHERE f.FinanceID = @FinanceID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FinanceID", financeId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            finance = MapReaderToFinance(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin giao dịch tài chính: {ex.Message}", ex);
            }

            return finance;
        }

        /// <summary>
        /// Thêm giao dịch tài chính mới
        /// </summary>
        public int InsertFinance(Finance finance)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Finance (
                            TransactionCode, Amount, TransactionType, Category, ProjectID, 
                            EmployeeID, CustomerID, TransactionDate, Description, PaymentMethod, 
                            ReferenceNo, Status, RecordedByID, CreatedAt
                        ) VALUES (
                            @TransactionCode, @Amount, @TransactionType, @Category, @ProjectID,
                            @EmployeeID, @CustomerID, @TransactionDate, @Description, @PaymentMethod,
                            @ReferenceNo, @Status, @RecordedByID, @CreatedAt
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddFinanceParameters(command, finance);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    connection.Open();
                    int newFinanceId = Convert.ToInt32(command.ExecuteScalar());
                    return newFinanceId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin giao dịch tài chính
        /// </summary>
        public bool UpdateFinance(Finance finance)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Finance SET
                            Amount = @Amount,
                            TransactionType = @TransactionType,
                            Category = @Category,
                            ProjectID = @ProjectID,
                            EmployeeID = @EmployeeID,
                            CustomerID = @CustomerID,
                            TransactionDate = @TransactionDate,
                            Description = @Description,
                            PaymentMethod = @PaymentMethod,
                            ReferenceNo = @ReferenceNo,
                            Status = @Status
                        WHERE FinanceID = @FinanceID";

                    SqlCommand command = new SqlCommand(query, connection);
                    AddFinanceParameters(command, finance);
                    command.Parameters.AddWithValue("@FinanceID", finance.FinanceID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa giao dịch tài chính
        /// </summary>
        public bool DeleteFinance(int financeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM Finance WHERE FinanceID = @FinanceID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FinanceID", financeId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa giao dịch tài chính: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation & Constraints

        /// <summary>
        /// Kiểm tra mã giao dịch có tồn tại không
        /// </summary>
        public bool IsTransactionCodeExists(string transactionCode, int excludeFinanceId = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Finance WHERE TransactionCode = @TransactionCode";

                    if (excludeFinanceId > 0)
                    {
                        query += " AND FinanceID != @FinanceID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TransactionCode", transactionCode);

                    if (excludeFinanceId > 0)
                    {
                        command.Parameters.AddWithValue("@FinanceID", excludeFinanceId);
                    }

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã giao dịch: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search & Filter

        /// <summary>
        /// Tìm kiếm giao dịch tài chính theo điều kiện
        /// </summary>
        public List<Finance> SearchFinances(string searchText = "", string transactionType = "",
            string category = "", string status = "", int? projectId = null, int? customerId = null,
            int? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<Finance> finances = new List<Finance>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT f.FinanceID, f.TransactionCode, f.Amount, f.TransactionType, 
                               f.Category, f.ProjectID, f.EmployeeID, f.CustomerID, 
                               f.TransactionDate, f.Description, f.PaymentMethod, 
                               f.ReferenceNo, f.Status, f.RecordedByID, f.CreatedAt,
                               p.ProjectName, e.FullName as EmployeeName, 
                               c.CompanyName as CustomerName, u.FullName as RecordedByName
                        FROM Finance f
                        LEFT JOIN Projects p ON f.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON f.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON f.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON f.RecordedByID = u.UserID
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            f.TransactionCode LIKE @SearchText OR 
                            f.Description LIKE @SearchText OR 
                            f.ReferenceNo LIKE @SearchText OR
                            p.ProjectName LIKE @SearchText OR
                            c.CompanyName LIKE @SearchText OR
                            e.FullName LIKE @SearchText)");
                        parameters.Add(new SqlParameter("@SearchText", $"%{searchText}%"));
                    }

                    if (!string.IsNullOrWhiteSpace(transactionType))
                    {
                        queryBuilder.Append(" AND f.TransactionType = @TransactionType");
                        parameters.Add(new SqlParameter("@TransactionType", transactionType));
                    }

                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        queryBuilder.Append(" AND f.Category = @Category");
                        parameters.Add(new SqlParameter("@Category", category));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        queryBuilder.Append(" AND f.Status = @Status");
                        parameters.Add(new SqlParameter("@Status", status));
                    }

                    if (projectId.HasValue && projectId.Value > 0)
                    {
                        queryBuilder.Append(" AND f.ProjectID = @ProjectID");
                        parameters.Add(new SqlParameter("@ProjectID", projectId.Value));
                    }

                    if (customerId.HasValue && customerId.Value > 0)
                    {
                        queryBuilder.Append(" AND f.CustomerID = @CustomerID");
                        parameters.Add(new SqlParameter("@CustomerID", customerId.Value));
                    }

                    if (employeeId.HasValue && employeeId.Value > 0)
                    {
                        queryBuilder.Append(" AND f.EmployeeID = @EmployeeID");
                        parameters.Add(new SqlParameter("@EmployeeID", employeeId.Value));
                    }

                    if (fromDate.HasValue)
                    {
                        queryBuilder.Append(" AND f.TransactionDate >= @FromDate");
                        parameters.Add(new SqlParameter("@FromDate", fromDate.Value.Date));
                    }

                    if (toDate.HasValue)
                    {
                        queryBuilder.Append(" AND f.TransactionDate <= @ToDate");
                        parameters.Add(new SqlParameter("@ToDate", toDate.Value.Date.AddDays(1).AddSeconds(-1)));
                    }

                    queryBuilder.Append(" ORDER BY f.TransactionDate DESC, f.CreatedAt DESC");

                    SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            finances.Add(MapReaderToFinance(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm giao dịch tài chính: {ex.Message}", ex);
            }

            return finances;
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo dự án
        /// </summary>
        public List<Finance> GetFinancesByProject(int projectId)
        {
            return SearchFinances("", "", "", "", projectId);
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo khách hàng
        /// </summary>
        public List<Finance> GetFinancesByCustomer(int customerId)
        {
            return SearchFinances("", "", "", "", null, customerId);
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo nhân viên
        /// </summary>
        public List<Finance> GetFinancesByEmployee(int employeeId)
        {
            return SearchFinances("", "", "", "", null, null, employeeId);
        }

        /// <summary>
        /// Lấy giao dịch theo trạng thái
        /// </summary>
        public List<Finance> GetFinancesByStatus(string status)
        {
            return SearchFinances("", "", "", status);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Tạo mã giao dịch tự động
        /// </summary>
        public string GenerateTransactionCode(string transactionType)
        {
            string prefix = transactionType == "Thu" ? "TN" : "TC"; // TN = Thu Nhap, TC = Tieu Chi
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP 1 SUBSTRING(TransactionCode, 3, LEN(TransactionCode)) AS CodeNumber
                        FROM Finance 
                        WHERE TransactionCode LIKE @Prefix AND ISNUMERIC(SUBSTRING(TransactionCode, 3, LEN(TransactionCode))) = 1
                        ORDER BY CAST(SUBSTRING(TransactionCode, 3, LEN(TransactionCode)) AS INT) DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Prefix", prefix + "%");

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

            return prefix + nextNumber.ToString("D6");
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
                    string query = "SELECT CustomerID, CompanyName FROM Customers ORDER BY CompanyName";
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
        /// Map SqlDataReader to Finance object
        /// </summary>
        private Finance MapReaderToFinance(SqlDataReader reader)
        {
            var finance = new Finance
            {
                FinanceID = Convert.ToInt32(reader["FinanceID"]),
                TransactionCode = reader["TransactionCode"].ToString(),
                Amount = Convert.ToDecimal(reader["Amount"]),
                TransactionType = reader["TransactionType"].ToString(),
                Category = reader["Category"].ToString(),
                ProjectID = reader["ProjectID"] == DBNull.Value ? null : Convert.ToInt32(reader["ProjectID"]),
                EmployeeID = reader["EmployeeID"] == DBNull.Value ? null : Convert.ToInt32(reader["EmployeeID"]),
                CustomerID = reader["CustomerID"] == DBNull.Value ? null : Convert.ToInt32(reader["CustomerID"]),
                TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                PaymentMethod = reader["PaymentMethod"]?.ToString() ?? string.Empty,
                ReferenceNo = reader["ReferenceNo"]?.ToString() ?? string.Empty,
                Status = reader["Status"].ToString(),
                RecordedByID = Convert.ToInt32(reader["RecordedByID"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };

            // Set navigation properties if available
            if (reader["ProjectName"] != DBNull.Value)
            {
                finance.Project = new Project { ProjectName = reader["ProjectName"].ToString() };
            }

            if (reader["EmployeeName"] != DBNull.Value)
            {
                finance.Employee = new Employee { FullName = reader["EmployeeName"].ToString() };
            }

            if (reader["CustomerName"] != DBNull.Value)
            {
                finance.Customer = new Customer { CompanyName = reader["CustomerName"].ToString() };
            }

            if (reader["RecordedByName"] != DBNull.Value)
            {
                finance.RecordedBy = new User { FullName = reader["RecordedByName"].ToString() };
            }

            return finance;
        }

        /// <summary>
        /// Thêm parameters chung cho Finance
        /// </summary>
        private void AddFinanceParameters(SqlCommand command, Finance finance)
        {
            command.Parameters.AddWithValue("@TransactionCode", finance.TransactionCode);
            command.Parameters.AddWithValue("@Amount", finance.Amount);
            command.Parameters.AddWithValue("@TransactionType", finance.TransactionType);
            command.Parameters.AddWithValue("@Category", finance.Category);
            command.Parameters.AddWithValue("@TransactionDate", finance.TransactionDate);
            command.Parameters.AddWithValue("@Description", (object)finance.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@PaymentMethod", (object)finance.PaymentMethod ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReferenceNo", (object)finance.ReferenceNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", finance.Status);
            command.Parameters.AddWithValue("@RecordedByID", finance.RecordedByID);

            // Handle nullable foreign keys
            command.Parameters.AddWithValue("@ProjectID", (object)finance.ProjectID ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmployeeID", (object)finance.EmployeeID ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerID", (object)finance.CustomerID ?? DBNull.Value);
        }

        #endregion

        #region Statistics & Reports - Basic Methods

        /// <summary>
        /// Lấy thống kê tài chính cơ bản
        /// </summary>
        public (decimal TotalIncome, decimal TotalExpense, decimal Balance, int TotalTransactions) GetFinanceStatistics(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT 
                            SUM(CASE WHEN TransactionType = N'Thu' THEN Amount ELSE 0 END) as TotalIncome,
                            SUM(CASE WHEN TransactionType = N'Chi' THEN Amount ELSE 0 END) as TotalExpense,
                            COUNT(*) as TotalTransactions
                        FROM Finance
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (fromDate.HasValue)
                    {
                        queryBuilder.Append(" AND TransactionDate >= @FromDate");
                        parameters.Add(new SqlParameter("@FromDate", fromDate.Value.Date));
                    }

                    if (toDate.HasValue)
                    {
                        queryBuilder.Append(" AND TransactionDate <= @ToDate");
                        parameters.Add(new SqlParameter("@ToDate", toDate.Value.Date.AddDays(1).AddSeconds(-1)));
                    }

                    SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal totalIncome = reader["TotalIncome"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalIncome"]);
                            decimal totalExpense = reader["TotalExpense"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalExpense"]);
                            int totalTransactions = Convert.ToInt32(reader["TotalTransactions"]);
                            decimal balance = totalIncome - totalExpense;

                            return (totalIncome, totalExpense, balance, totalTransactions);
                        }
                    }
                }

                return (0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê theo category
        /// </summary>
        public Dictionary<string, decimal> GetCategoryStatistics(string transactionType, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var statistics = new Dictionary<string, decimal>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT Category, SUM(Amount) as TotalAmount
                        FROM Finance
                        WHERE TransactionType = @TransactionType");

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@TransactionType", transactionType));

                    if (fromDate.HasValue)
                    {
                        queryBuilder.Append(" AND TransactionDate >= @FromDate");
                        parameters.Add(new SqlParameter("@FromDate", fromDate.Value.Date));
                    }

                    if (toDate.HasValue)
                    {
                        queryBuilder.Append(" AND TransactionDate <= @ToDate");
                        parameters.Add(new SqlParameter("@ToDate", toDate.Value.Date.AddDays(1).AddSeconds(-1)));
                    }

                    queryBuilder.Append(" GROUP BY Category ORDER BY TotalAmount DESC");

                    SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["Category"].ToString();
                            decimal amount = Convert.ToDecimal(reader["TotalAmount"]);
                            statistics[category] = amount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê theo danh mục: {ex.Message}", ex);
            }

            return statistics;
        }

        /// <summary>
        /// Lấy thống kê theo tháng
        /// </summary>
        public List<MonthlyFinanceReportDTO> GetMonthlyStatistics(int year)
        {
            var reports = new List<MonthlyFinanceReportDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    MONTH(TransactionDate) as Month,
                    SUM(CASE WHEN TransactionType = N'Thu' THEN Amount ELSE 0 END) as Income,
                    SUM(CASE WHEN TransactionType = N'Chi' THEN Amount ELSE 0 END) as Expense
                FROM Finance
                WHERE YEAR(TransactionDate) = @Year
                GROUP BY MONTH(TransactionDate)
                ORDER BY MONTH(TransactionDate)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Year", year);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new MonthlyFinanceReportDTO
                            {
                                Month = Convert.ToInt32(reader["Month"]),
                                Year = year,
                                Income = Convert.ToDecimal(reader["Income"]),
                                Expense = Convert.ToDecimal(reader["Expense"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê theo tháng: {ex.Message}", ex);
            }

            return reports;
        }

        /// <summary>
        /// Lấy thống kê tài chính theo dự án
        /// </summary>
        public List<ProjectFinanceReportDTO> GetProjectFinanceStatistics()
        {
            var reports = new List<ProjectFinanceReportDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            p.ProjectID,
                            p.ProjectName,
                            SUM(CASE WHEN f.TransactionType = N'Thu' THEN f.Amount ELSE 0 END) as Income,
                            SUM(CASE WHEN f.TransactionType = N'Chi' THEN f.Amount ELSE 0 END) as Expense,
                            p.Budget
                        FROM Projects p
                        LEFT JOIN Finance f ON p.ProjectID = f.ProjectID
                        GROUP BY p.ProjectID, p.ProjectName, p.Budget
                        ORDER BY p.ProjectName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new ProjectFinanceReportDTO
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectName = reader["ProjectName"].ToString(),
                                Income = reader["Income"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Income"]),
                                Expense = reader["Expense"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Expense"]),
                                Budget = reader["Budget"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Budget"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính dự án: {ex.Message}", ex);
            }

            return reports;
        }

        #endregion

        #region Missing Methods - Now Implemented

        /// <summary>
        /// Từ chối giao dịch với lý do
        /// </summary>
        public bool RejectFinance(int financeId, string rejectReason)
        {
            string query = @"
                UPDATE Finance 
                SET Status = N'Từ chối', 
                    Description = CASE 
                        WHEN Description IS NULL OR Description = '' 
                        THEN N'Lý do từ chối: ' + @RejectReason
                        ELSE Description + CHAR(13) + CHAR(10) + N'Lý do từ chối: ' + @RejectReason
                    END
                WHERE FinanceID = @FinanceID AND Status = N'Chờ duyệt'";

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FinanceID", financeId);
                        command.Parameters.AddWithValue("@RejectReason", rejectReason ?? "");

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không thể từ chối giao dịch. Giao dịch có thể đã được xử lý hoặc không tồn tại.");
                        }

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi từ chối giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Duyệt giao dịch tài chính
        /// </summary>
        public bool ApproveFinance(int financeId, int approvedByUserId)
        {
            string query = @"
                UPDATE Finance 
                SET Status = N'Đã duyệt'
                WHERE FinanceID = @FinanceID AND Status = N'Chờ duyệt'";

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FinanceID", financeId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi duyệt giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính với DTO đầy đủ
        /// </summary>
        public FinanceStatistics GetFinanceStatistics()
        {
            try
            {
                var (totalIncome, totalExpense, balance, totalTransactions) = GetFinanceStatistics(null, null);

                return new FinanceStatistics
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = balance,
                    TotalTransactions = totalTransactions,
                    IncomePercentage = totalIncome + totalExpense > 0 ? (totalIncome / (totalIncome + totalExpense)) * 100 : 0,
                    ExpensePercentage = totalIncome + totalExpense > 0 ? (totalExpense / (totalIncome + totalExpense)) * 100 : 0,
                    ProfitMargin = totalIncome > 0 ? (balance / totalIncome) * 100 : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính theo khoảng thời gian
        /// </summary>
        public FinanceStatistics GetFinanceStatisticsByDateRange(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var (totalIncome, totalExpense, balance, totalTransactions) = GetFinanceStatistics(fromDate, toDate);

                return new FinanceStatistics
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = balance,
                    TotalTransactions = totalTransactions,
                    FromDate = fromDate,
                    ToDate = toDate,
                    IncomePercentage = totalIncome + totalExpense > 0 ? (totalIncome / (totalIncome + totalExpense)) * 100 : 0,
                    ExpensePercentage = totalIncome + totalExpense > 0 ? (totalExpense / (totalIncome + totalExpense)) * 100 : 0,
                    ProfitMargin = totalIncome > 0 ? (balance / totalIncome) * 100 : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính theo thời gian: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy top giao dịch lớn nhất
        /// </summary>
        public List<Finance> GetTopTransactions(int top = 10, string transactionType = "")
        {
            var finances = new List<Finance>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP(@Top) f.FinanceID, f.TransactionCode, f.Amount, f.TransactionType, 
                               f.Category, f.ProjectID, f.EmployeeID, f.CustomerID, 
                               f.TransactionDate, f.Description, f.PaymentMethod, 
                               f.ReferenceNo, f.Status, f.RecordedByID, f.CreatedAt,
                               p.ProjectName, e.FullName as EmployeeName, 
                               c.CompanyName as CustomerName, u.FullName as RecordedByName
                        FROM Finance f
                        LEFT JOIN Projects p ON f.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON f.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON f.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON f.RecordedByID = u.UserID
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(transactionType))
                    {
                        query += " AND f.TransactionType = @TransactionType";
                    }

                    query += " ORDER BY f.Amount DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Top", top);

                    if (!string.IsNullOrEmpty(transactionType))
                    {
                        command.Parameters.AddWithValue("@TransactionType", transactionType);
                    }

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            finances.Add(MapReaderToFinance(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy top giao dịch: {ex.Message}", ex);
            }

            return finances;
        }

        /// <summary>
        /// Lấy giao dịch gần đây
        /// </summary>
        public List<Finance> GetRecentTransactions(int days = 7, int limit = 50)
        {
            var finances = new List<Finance>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT TOP(@Limit) f.FinanceID, f.TransactionCode, f.Amount, f.TransactionType, 
                               f.Category, f.ProjectID, f.EmployeeID, f.CustomerID, 
                               f.TransactionDate, f.Description, f.PaymentMethod, 
                               f.ReferenceNo, f.Status, f.RecordedByID, f.CreatedAt,
                               p.ProjectName, e.FullName as EmployeeName, 
                               c.CompanyName as CustomerName, u.FullName as RecordedByName
                        FROM Finance f
                        LEFT JOIN Projects p ON f.ProjectID = p.ProjectID
                        LEFT JOIN Employees e ON f.EmployeeID = e.EmployeeID
                        LEFT JOIN Customers c ON f.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON f.RecordedByID = u.UserID
                        WHERE f.TransactionDate >= DATEADD(DAY, -@Days, GETDATE())
                        ORDER BY f.TransactionDate DESC, f.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Days", days);
                    command.Parameters.AddWithValue("@Limit", limit);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            finances.Add(MapReaderToFinance(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch gần đây: {ex.Message}", ex);
            }

            return finances;
        }

        /// <summary>
        /// Lấy danh sách dự án có giao dịch tài chính
        /// </summary>
        public List<Project> GetProjectsWithFinances()
        {
            var projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT DISTINCT p.ProjectID, p.ProjectName, p.Budget
                        FROM Projects p
                        INNER JOIN Finance f ON p.ProjectID = f.ProjectID
                        ORDER BY p.ProjectName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectName = reader["ProjectName"].ToString(),
                                Budget = Convert.ToDecimal(reader["Budget"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dự án có tài chính: {ex.Message}", ex);
            }

            return projects;
        }

        /// <summary>
        /// Cập nhật ngân sách dự án
        /// </summary>
        public bool UpdateProjectBudget(int projectId, decimal newBudget)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "UPDATE Projects SET Budget = @Budget WHERE ProjectID = @ProjectID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Budget", newBudget);
                    command.Parameters.AddWithValue("@ProjectID", projectId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật ngân sách dự án: {ex.Message}", ex);
            }
        }

        #endregion

        #region Project Finance Methods - Placeholders

        /// <summary>
        /// Lấy thống kê tài chính chi tiết theo dự án
        /// </summary>
        public ProjectFinanceStatistics GetProjectFinanceStatistics(int projectId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            p.ProjectID,
                            p.ProjectName,
                            p.Budget,
                            SUM(CASE WHEN f.TransactionType = N'Thu' THEN f.Amount ELSE 0 END) as TotalIncome,
                            SUM(CASE WHEN f.TransactionType = N'Chi' THEN f.Amount ELSE 0 END) as TotalExpense,
                            COUNT(f.FinanceID) as TransactionCount,
                            MAX(f.TransactionDate) as LastTransactionDate
                        FROM Projects p
                        LEFT JOIN Finance f ON p.ProjectID = f.ProjectID
                        WHERE p.ProjectID = @ProjectID
                        GROUP BY p.ProjectID, p.ProjectName, p.Budget";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", projectId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var income = reader["TotalIncome"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalIncome"]);
                            var expense = reader["TotalExpense"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalExpense"]);
                            var budget = reader["Budget"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Budget"]);

                            return new ProjectFinanceStatistics
                            {
                                ProjectID = projectId,
                                ProjectName = reader["ProjectName"].ToString(),
                                TotalIncome = income,
                                TotalExpense = expense,
                                Balance = income - expense,
                                Budget = budget,
                                BudgetUtilization = budget > 0 ? (expense / budget) * 100 : 0,
                                TransactionCount = Convert.ToInt32(reader["TransactionCount"]),
                                LastTransactionDate = reader["LastTransactionDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastTransactionDate"])
                            };
                        }
                    }
                }

                return new ProjectFinanceStatistics { ProjectID = projectId };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy báo cáo tài chính chi tiết theo dự án
        /// </summary>
        public ProjectFinanceReport GetProjectFinanceReport(int projectId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var finances = GetFinancesByProject(projectId);

                if (fromDate.HasValue)
                    finances = finances.Where(f => f.TransactionDate >= fromDate.Value).ToList();
                if (toDate.HasValue)
                    finances = finances.Where(f => f.TransactionDate <= toDate.Value).ToList();

                var income = finances.Where(f => f.TransactionType == "Thu").Sum(f => f.Amount);
                var expense = finances.Where(f => f.TransactionType == "Chi").Sum(f => f.Amount);

                return new ProjectFinanceReport
                {
                    ProjectID = projectId,
                    TotalIncome = income,
                    TotalExpense = expense,
                    Balance = income - expense,
                    Transactions = finances,
                    FromDate = fromDate,
                    ToDate = toDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra ngân sách dự án
        /// </summary>
        public BudgetCheckResult CheckProjectBudget(int projectId)
        {
            try
            {
                var statistics = GetProjectFinanceStatistics(projectId);

                return new BudgetCheckResult
                {
                    ProjectID = projectId,
                    ProjectName = statistics.ProjectName,
                    Budget = statistics.Budget,
                    SpentAmount = statistics.TotalExpense,
                    RemainingBudget = statistics.Budget - statistics.TotalExpense,
                    BudgetUtilization = statistics.BudgetUtilization,
                    IsOverBudget = statistics.TotalExpense > statistics.Budget,
                    IsNearBudgetLimit = statistics.BudgetUtilization > 80,
                    Status = statistics.TotalExpense > statistics.Budget ? "Vượt ngân sách" :
                            statistics.BudgetUtilization > 80 ? "Gần hết ngân sách" : "Bình thường"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra ngân sách dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính toán ROI của dự án
        /// </summary>
        public ProjectROIResult CalculateProjectROI(int projectId)
        {
            try
            {
                var statistics = GetProjectFinanceStatistics(projectId);

                var roi = statistics.TotalExpense > 0 ?
                    (statistics.TotalIncome - statistics.TotalExpense) / statistics.TotalExpense : 0;

                return new ProjectROIResult
                {
                    ProjectID = projectId,
                    ProjectName = statistics.ProjectName,
                    TotalInvestment = statistics.TotalExpense,
                    TotalReturn = statistics.TotalIncome,
                    ROI = roi,
                    ROIPercentage = roi * 100,
                    ROICategory = roi > 0.2m ? "Cao" : roi > 0.1m ? "Trung bình" : "Thấp",
                    CalculationDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính ROI dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy xu hướng tài chính theo tháng của dự án
        /// </summary>
        public List<MonthlyProjectFinance> GetProjectFinanceTrend(int projectId, int months = 12)
        {
            var trends = new List<MonthlyProjectFinance>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            YEAR(TransactionDate) as Year,
                            MONTH(TransactionDate) as Month,
                            SUM(CASE WHEN TransactionType = N'Thu' THEN Amount ELSE 0 END) as Income,
                            SUM(CASE WHEN TransactionType = N'Chi' THEN Amount ELSE 0 END) as Expense,
                            COUNT(*) as TransactionCount
                        FROM Finance
                        WHERE ProjectID = @ProjectID 
                            AND TransactionDate >= DATEADD(MONTH, -@Months, GETDATE())
                        GROUP BY YEAR(TransactionDate), MONTH(TransactionDate)
                        ORDER BY YEAR(TransactionDate), MONTH(TransactionDate)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectID", projectId);
                    command.Parameters.AddWithValue("@Months", months);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var income = Convert.ToDecimal(reader["Income"]);
                            var expense = Convert.ToDecimal(reader["Expense"]);

                            trends.Add(new MonthlyProjectFinance
                            {
                                ProjectID = projectId,
                                Year = Convert.ToInt32(reader["Year"]),
                                Month = Convert.ToInt32(reader["Month"]),
                                Income = income,
                                Expense = expense,
                                Balance = income - expense,
                                TransactionCount = Convert.ToInt32(reader["TransactionCount"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy xu hướng tài chính dự án: {ex.Message}", ex);
            }

            return trends;
        }

        /// <summary>
        /// Lấy giao dịch với bộ lọc nâng cao cho dự án
        /// </summary>
        public List<Finance> GetProjectFinancesWithFilter(int projectId, ProjectFinanceFilter filter)
        {
            try
            {
                return SearchFinances("", filter.TransactionType, filter.Category, filter.Status,
                    projectId, null, null, filter.FromDate, filter.ToDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lọc giao dịch dự án: {ex.Message}", ex);
            }
        }

        #endregion

        #region Analytics Methods

        /// <summary>
        /// Dự báo tài chính (Placeholder)
        /// </summary>
        public EmployeeManagement.Models.DTO.FinanceForecast GetFinanceForecast(int months)
        {
            return new EmployeeManagement.Models.DTO.FinanceForecast
            {
                MonthlyForecasts = new List<MonthlyForecast>(),
                PredictedTotalIncome = 0,
                PredictedTotalExpense = 0,
                PredictedBalance = 0,
                ConfidenceLevel = "Low",
                Assumptions = new List<string> { "Dự báo chưa được triển khai" },
                ForecastDate = DateTime.Now
            };
        }

        /// <summary>
        /// Phân tích hiệu suất (Placeholder)
        /// </summary>
        public EmployeeManagement.Models.DTO.PerformanceAnalysis GetPerformanceAnalysis(DateTime fromDate, DateTime toDate, string comparisonPeriod)
        {
            var currentStats = GetFinanceStatisticsByDateRange(fromDate, toDate);

            return new EmployeeManagement.Models.DTO.PerformanceAnalysis
            {
                FromDate = fromDate,
                ToDate = toDate,
                CurrentPeriod = currentStats,
                ComparisonPeriod = new FinanceStatistics(),
                IncomeChange = 0,
                ExpenseChange = 0,
                ProfitabilityChange = 0,
                OverallPerformance = "Stable",
                KeyInsights = new List<string> { "Phân tích hiệu suất chưa được triển khai đầy đủ" },
                Metrics = new List<PerformanceMetric>()
            };
        }

        /// <summary>
        /// Phát hiện bất thường (Basic Implementation)
        /// </summary>
        public List<EmployeeManagement.Models.DTO.AnomalyDetectionResult> DetectAnomalousTransactions(int days)
        {
            var anomalies = new List<EmployeeManagement.Models.DTO.AnomalyDetectionResult>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        WITH Stats AS (
                            SELECT 
                                AVG(CAST(Amount AS FLOAT)) as AvgAmount,
                                STDEV(CAST(Amount AS FLOAT)) as StdAmount
                            FROM Finance 
                            WHERE TransactionDate >= DATEADD(DAY, -@Days, GETDATE())
                                AND Status != N'Từ chối' AND Status != N'Hủy'
                        ),
                        Outliers AS (
                            SELECT f.FinanceID, f.TransactionCode, f.Amount, f.TransactionDate,
                                   ABS(f.Amount - s.AvgAmount) / NULLIF(s.StdAmount, 0) as ZScore
                            FROM Finance f
                            CROSS JOIN Stats s
                            WHERE f.TransactionDate >= DATEADD(DAY, -@Days, GETDATE())
                                AND f.Status != N'Từ chối' AND f.Status != N'Hủy'
                                AND s.StdAmount > 0
                                AND ABS(f.Amount - s.AvgAmount) / s.StdAmount > 2
                        )
                        SELECT TOP 50 * FROM Outliers ORDER BY ZScore DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Days", days);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var zScore = reader["ZScore"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ZScore"]);
                            anomalies.Add(new EmployeeManagement.Models.DTO.AnomalyDetectionResult
                            {
                                FinanceID = Convert.ToInt32(reader["FinanceID"]),
                                TransactionCode = reader["TransactionCode"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                AnomalyType = "Amount Outlier",
                                Severity = Math.Min(zScore / 5, 1),
                                Description = "Số tiền giao dịch bất thường so với mức trung bình",
                                Reasons = new List<string> { $"Giao dịch lệch {zScore:F1} độ lệch chuẩn so với trung bình" },
                                Confidence = Math.Min(zScore / 3, 1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi phát hiện bất thường: {ex.Message}", ex);
            }

            return anomalies;
        }

        #endregion
    }
}