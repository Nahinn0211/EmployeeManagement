using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using System.Configuration;

namespace EmployeeManagement.DAL
{
    public class SalaryDAL
    {
        private string connectionString;

        //public SalaryDAL()
        //{
        //    connectionString = DBConnection.GetConnectionString();
        //}

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        #region CRUD Operations
        public List<Salary> GetAllSalaries()
        {
            List<Salary> salaries = new List<Salary>();
            string query = @"
                SELECT s.*, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Salary s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                ORDER BY s.Year DESC, s.Month DESC, e.EmployeeCode";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Salary salary = MapReaderToSalary(reader);
                    salaries.Add(salary);
                }
            }

            return salaries;
        }

        public Salary GetSalaryById(int salaryId)
        {
            string query = @"
                SELECT s.*, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Salary s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE s.SalaryID = @SalaryID";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SalaryID", salaryId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapReaderToSalary(reader);
                }
            }

            return null;
        }

        public int AddSalary(Salary salary)
        {
            string query = @"
                INSERT INTO Salary (EmployeeID, Month, Year, BaseSalary, Allowance, Bonus, Deduction, NetSalary, PaymentDate, PaymentStatus, Notes, CreatedAt)
                VALUES (@EmployeeID, @Month, @Year, @BaseSalary, @Allowance, @Bonus, @Deduction, @NetSalary, @PaymentDate, @PaymentStatus, @Notes, @CreatedAt);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", salary.EmployeeID);
                command.Parameters.AddWithValue("@Month", salary.Month);
                command.Parameters.AddWithValue("@Year", salary.Year);
                command.Parameters.AddWithValue("@BaseSalary", salary.BaseSalary);
                command.Parameters.AddWithValue("@Allowance", salary.Allowance);
                command.Parameters.AddWithValue("@Bonus", salary.Bonus);
                command.Parameters.AddWithValue("@Deduction", salary.Deduction);
                command.Parameters.AddWithValue("@NetSalary", salary.NetSalary);
                command.Parameters.AddWithValue("@PaymentDate", (object)salary.PaymentDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@PaymentStatus", salary.PaymentStatus);
                command.Parameters.AddWithValue("@Notes", salary.Notes ?? "");
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool UpdateSalary(Salary salary)
        {
            string query = @"
                UPDATE Salary 
                SET BaseSalary = @BaseSalary, Allowance = @Allowance, Bonus = @Bonus, 
                    Deduction = @Deduction, NetSalary = @NetSalary, PaymentDate = @PaymentDate,
                    PaymentStatus = @PaymentStatus, Notes = @Notes
                WHERE SalaryID = @SalaryID";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SalaryID", salary.SalaryID);
                command.Parameters.AddWithValue("@BaseSalary", salary.BaseSalary);
                command.Parameters.AddWithValue("@Allowance", salary.Allowance);
                command.Parameters.AddWithValue("@Bonus", salary.Bonus);
                command.Parameters.AddWithValue("@Deduction", salary.Deduction);
                command.Parameters.AddWithValue("@NetSalary", salary.NetSalary);
                command.Parameters.AddWithValue("@PaymentDate", (object)salary.PaymentDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@PaymentStatus", salary.PaymentStatus);
                command.Parameters.AddWithValue("@Notes", salary.Notes ?? "");

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteSalary(int salaryId)
        {
            string query = "DELETE FROM Salary WHERE SalaryID = @SalaryID";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SalaryID", salaryId);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Search and Filter
        public List<Salary> SearchSalaries(string employeeName, string employeeCode, string departmentName,
            string paymentStatus, int? month, int? year, DateTime? fromDate, DateTime? toDate)
        {
            List<Salary> salaries = new List<Salary>();
            string query = @"
                SELECT s.*, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Salary s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(employeeName))
            {
                query += " AND (e.FirstName LIKE @EmployeeName OR e.LastName LIKE @EmployeeName OR e.FullName LIKE @EmployeeName)";
                parameters.Add(new SqlParameter("@EmployeeName", $"%{employeeName}%"));
            }

            if (!string.IsNullOrEmpty(employeeCode))
            {
                query += " AND e.EmployeeCode LIKE @EmployeeCode";
                parameters.Add(new SqlParameter("@EmployeeCode", $"%{employeeCode}%"));
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                query += " AND d.DepartmentName LIKE @DepartmentName";
                parameters.Add(new SqlParameter("@DepartmentName", $"%{departmentName}%"));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query += " AND s.PaymentStatus = @PaymentStatus";
                parameters.Add(new SqlParameter("@PaymentStatus", paymentStatus));
            }

            if (month.HasValue)
            {
                query += " AND s.Month = @Month";
                parameters.Add(new SqlParameter("@Month", month.Value));
            }

            if (year.HasValue)
            {
                query += " AND s.Year = @Year";
                parameters.Add(new SqlParameter("@Year", year.Value));
            }

            if (fromDate.HasValue)
            {
                query += " AND s.CreatedAt >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
            }

            if (toDate.HasValue)
            {
                query += " AND s.CreatedAt <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", toDate.Value));
            }

            query += " ORDER BY s.Year DESC, s.Month DESC, e.EmployeeCode";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters.ToArray());
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Salary salary = MapReaderToSalary(reader);
                    salaries.Add(salary);
                }
            }

            return salaries;
        }

        public List<Salary> GetSalariesByEmployee(int employeeId)
        {
            List<Salary> salaries = new List<Salary>();
            string query = @"
                SELECT s.*, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Salary s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE s.EmployeeID = @EmployeeID
                ORDER BY s.Year DESC, s.Month DESC";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Salary salary = MapReaderToSalary(reader);
                    salaries.Add(salary);
                }
            }

            return salaries;
        }

        public List<Salary> GetSalariesByMonthYear(int month, int year)
        {
            List<Salary> salaries = new List<Salary>();
            string query = @"
                SELECT s.*, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Salary s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE s.Month = @Month AND s.Year = @Year
                ORDER BY e.EmployeeCode";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Salary salary = MapReaderToSalary(reader);
                    salaries.Add(salary);
                }
            }

            return salaries;
        }
        #endregion

        #region Validation
        public bool IsSalaryExists(int employeeId, int month, int year, int excludeSalaryId = 0)
        {
            string query = "SELECT COUNT(*) FROM Salary WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year AND SalaryID != @ExcludeSalaryId";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@ExcludeSalaryId", excludeSalaryId);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public bool CanDeleteSalary(int salaryId)
        {
            string query = "SELECT PaymentStatus FROM Salary WHERE SalaryID = @SalaryID";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SalaryID", salaryId);

                connection.Open();
                var paymentStatus = command.ExecuteScalar()?.ToString();

                return paymentStatus == "Chưa thanh toán" || paymentStatus == "Đã hủy";
            }
        }
        #endregion

        #region Statistics
        public SalaryStatistics GetSalaryStatistics()
        {
            string query = @"
                SELECT 
                    COUNT(*) as TotalRecords,
                    SUM(CASE WHEN PaymentStatus = 'Đã thanh toán' THEN 1 ELSE 0 END) as PaidRecords,
                    SUM(CASE WHEN PaymentStatus = 'Chưa thanh toán' THEN 1 ELSE 0 END) as UnpaidRecords,
                    SUM(CASE WHEN PaymentStatus = 'Thanh toán một phần' THEN 1 ELSE 0 END) as PartiallyPaidRecords,
                    SUM(CASE WHEN PaymentStatus = 'Đã hủy' THEN 1 ELSE 0 END) as CancelledRecords,
                    SUM(NetSalary) as TotalNetSalary,
                    SUM(CASE WHEN PaymentStatus = 'Đã thanh toán' THEN NetSalary ELSE 0 END) as TotalPaidAmount,
                    SUM(CASE WHEN PaymentStatus = 'Chưa thanh toán' THEN NetSalary ELSE 0 END) as TotalUnpaidAmount,
                    AVG(NetSalary) as AverageSalary
                FROM Salary";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new SalaryStatistics
                    {
                        TotalRecords = reader.GetInt32("TotalRecords"),
                        PaidRecords = reader.GetInt32("PaidRecords"),
                        UnpaidRecords = reader.GetInt32("UnpaidRecords"),
                        PartiallyPaidRecords = reader.GetInt32("PartiallyPaidRecords"),
                        CancelledRecords = reader.GetInt32("CancelledRecords"),
                        TotalNetSalary = reader.GetDecimal("TotalNetSalary"),
                        TotalPaidAmount = reader.GetDecimal("TotalPaidAmount"),
                        TotalUnpaidAmount = reader.GetDecimal("TotalUnpaidAmount"),
                        AverageSalary = reader.IsDBNull("AverageSalary") ? 0 : reader.GetDecimal("AverageSalary"),
                        CurrentMonth = DateTime.Now.Month,
                        CurrentYear = DateTime.Now.Year
                    };
                }
            }

            return new SalaryStatistics();
        }

        public List<EmployeeDropdownItem> GetEmployeesForDropdown()
        {
            List<EmployeeDropdownItem> employees = new List<EmployeeDropdownItem>();
            string query = @"
                SELECT e.EmployeeID, e.EmployeeCode, e.FullName, 
                       d.DepartmentName, p.PositionName, p.BaseSalary
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE e.Status = N'Đang làm việc'
                ORDER BY e.EmployeeCode";

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new EmployeeDropdownItem
                    {
                        EmployeeID = reader.GetInt32("EmployeeID"),
                        EmployeeCode = reader.GetString("EmployeeCode"),
                        FullName = reader.GetString("FullName"),
                        DepartmentName = reader.IsDBNull("DepartmentName") ? "" : reader.GetString("DepartmentName"),
                        PositionName = reader.IsDBNull("PositionName") ? "" : reader.GetString("PositionName"),
                        BaseSalary = reader.IsDBNull("BaseSalary") ? 0 : reader.GetDecimal("BaseSalary")
                    });
                }
            }

            return employees;
        }
        #endregion

        #region Helper Methods
        private Salary MapReaderToSalary(SqlDataReader reader)
        {
            var salary = new Salary
            {
                SalaryID = reader.GetInt32("SalaryID"),
                EmployeeID = reader.GetInt32("EmployeeID"),
                Month = reader.GetInt32("Month"),
                Year = reader.GetInt32("Year"),
                BaseSalary = reader.GetDecimal("BaseSalary"),
                Allowance = reader.GetDecimal("Allowance"),
                Bonus = reader.GetDecimal("Bonus"),
                Deduction = reader.GetDecimal("Deduction"),
                NetSalary = reader.GetDecimal("NetSalary"),
                PaymentDate = reader.IsDBNull("PaymentDate") ? null : reader.GetDateTime("PaymentDate"),
                PaymentStatus = reader.GetString("PaymentStatus"),
                Notes = reader.IsDBNull("Notes") ? "" : reader.GetString("Notes"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };

            // Create Employee object if employee data is available
            if (reader.FieldCount > 12) // More than just salary fields
            {
                salary.Employee = new Employee
                {
                    EmployeeID = salary.EmployeeID,
                    EmployeeCode = reader.GetString("EmployeeCode"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    FullName = reader.GetString("FullName")
                };
            }

            return salary;
        }
        #endregion
    }
}