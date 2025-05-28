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
    /// Data Access Layer cho Employee
    /// </summary>
    public class EmployeeDAL
    {
        /// <summary>
        /// Lấy connection string từ app.config
        /// </summary>
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }
        /// <summary>
        /// Lấy tất cả phòng ban từ database
        /// </summary>
        public List<Department> GetAllDepartments()
        {
            var departments = new List<Department>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            DepartmentID,
                            DepartmentName,
                            Description,
                            ManagerID,
                            CreatedAt,
                            UpdatedAt
                        FROM Departments 
                        ORDER BY DepartmentName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                departments.Add(new Department
                                {
                                    DepartmentID = reader.GetInt32("DepartmentID"),
                                    DepartmentName = reader.GetString("DepartmentName"),
                                    Description = reader.IsDBNull("Description") ? "" : reader.GetString("Description"),
                                    ManagerID = reader.IsDBNull("ManagerID") ? (int?)null : reader.GetInt32("ManagerID"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phòng ban: {ex.Message}");
            }

            return departments;
        }

        /// <summary>
        /// Lấy tất cả chức vụ từ database
        /// </summary>
        public List<Position> GetAllPositions()
        {
            var positions = new List<Position>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            PositionID,
                            PositionName,
                            Description,
                            BaseSalary,
                            CreatedAt,
                            UpdatedAt
                        FROM Positions 
                        ORDER BY PositionName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                positions.Add(new Position
                                {
                                    PositionID = reader.GetInt32("PositionID"),
                                    PositionName = reader.GetString("PositionName"),
                                    Description = reader.IsDBNull("Description") ? "" : reader.GetString("Description"),
                                    BaseSalary = reader.IsDBNull("BaseSalary") ? 0 : reader.GetDecimal("BaseSalary"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách chức vụ: {ex.Message}");
            }

            return positions;
        }

        /// <summary>
        /// Lấy thông tin nhân viên cơ bản để fill ComboBox
        /// </summary>
        public List<Employee> GetEmployeesBasicInfo()
        {
            var employees = new List<Employee>();

            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            EmployeeID,
                            EmployeeCode,
                            FullName,
                            DepartmentID,
                            PositionID,
                            Status
                        FROM Employees 
                        WHERE Status = N'Đang làm việc'
                        ORDER BY FullName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    EmployeeID = reader.GetInt32("EmployeeID"),
                                    EmployeeCode = reader.GetString("EmployeeCode"),
                                    FullName = reader.GetString("FullName"),
                                    DepartmentID = reader.GetInt32("DepartmentID"),
                                    PositionID =reader.GetInt32("PositionID"),
                                    Status = reader.GetString("Status")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}");
            }

            return employees;
        }
        /// <summary>
        /// Lấy tất cả nhân viên từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách EmployeeDTO</returns>
        public List<EmployeeDTO> GetAllEmployees()
        {
            List<EmployeeDTO> employeeList = new List<EmployeeDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName, 
                               e.Gender, e.DateOfBirth, e.IDCardNumber, e.Address, e.Phone, 
                               e.Email, e.DepartmentID, e.PositionID, e.ManagerID, 
                               e.HireDate, e.EndDate, e.Status, e.BankAccount, e.BankName, 
                               e.TaxCode, e.InsuranceCode, e.Notes, e.CreatedAt, e.UpdatedAt,
                               d.DepartmentName, p.PositionName, m.FullName AS ManagerName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                        ORDER BY e.FullName";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeDTO dto = new EmployeeDTO
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                IDCardNumber = reader["IDCardNumber"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"])
                            };

                            // Phòng ban và chức vụ
                            if (reader["DepartmentID"] != DBNull.Value)
                            {
                                dto.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                dto.DepartmentName = reader["DepartmentName"].ToString();
                            }

                            if (reader["PositionID"] != DBNull.Value)
                            {
                                dto.PositionID = Convert.ToInt32(reader["PositionID"]);
                                dto.PositionName = reader["PositionName"].ToString();
                            }

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"].ToString();
                            }

                            if (reader["Address"] != DBNull.Value)
                                dto.Address = reader["Address"].ToString();

                            if (reader["EndDate"] != DBNull.Value)
                                dto.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["BankAccount"] != DBNull.Value)
                                dto.BankAccount = reader["BankAccount"].ToString();

                            if (reader["BankName"] != DBNull.Value)
                                dto.BankName = reader["BankName"].ToString();

                            if (reader["TaxCode"] != DBNull.Value)
                                dto.TaxCode = reader["TaxCode"].ToString();

                            if (reader["InsuranceCode"] != DBNull.Value)
                                dto.InsuranceCode = reader["InsuranceCode"].ToString();

                            if (reader["Notes"] != DBNull.Value)
                                dto.Notes = reader["Notes"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            employeeList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu nhân viên: {ex.Message}", ex);
            }

            return employeeList;
        }

        /// <summary>
        /// Lấy thông tin của một nhân viên theo ID
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <returns>EmployeeDTO</returns>
        public EmployeeDTO GetEmployeeById(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName, 
                               e.Gender, e.DateOfBirth, e.IDCardNumber, e.Address, e.Phone, 
                               e.Email, e.DepartmentID, e.PositionID, e.ManagerID, 
                               e.HireDate, e.EndDate, e.Status, e.BankAccount, e.BankName, 
                               e.TaxCode, e.InsuranceCode, e.Notes, e.FaceDataPath, e.CreatedAt, e.UpdatedAt,
                               d.DepartmentName, p.PositionName, m.FullName AS ManagerName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                        WHERE e.EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EmployeeDTO dto = new EmployeeDTO
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                IDCardNumber = reader["IDCardNumber"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"])
                            };

                            // Phòng ban và chức vụ
                            if (reader["DepartmentID"] != DBNull.Value)
                            {
                                dto.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                dto.DepartmentName = reader["DepartmentName"].ToString();
                            }

                            if (reader["PositionID"] != DBNull.Value)
                            {
                                dto.PositionID = Convert.ToInt32(reader["PositionID"]);
                                dto.PositionName = reader["PositionName"].ToString();
                            }

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"].ToString();
                            }

                            if (reader["Address"] != DBNull.Value)
                                dto.Address = reader["Address"].ToString();

                            if (reader["EndDate"] != DBNull.Value)
                                dto.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["BankAccount"] != DBNull.Value)
                                dto.BankAccount = reader["BankAccount"].ToString();

                            if (reader["BankName"] != DBNull.Value)
                                dto.BankName = reader["BankName"].ToString();

                            if (reader["TaxCode"] != DBNull.Value)
                                dto.TaxCode = reader["TaxCode"].ToString();

                            if (reader["InsuranceCode"] != DBNull.Value)
                                dto.InsuranceCode = reader["InsuranceCode"].ToString();

                            if (reader["Notes"] != DBNull.Value)
                                dto.Notes = reader["Notes"].ToString();

                            if (reader["FaceDataPath"] != DBNull.Value)
                                dto.FaceDataPath = reader["FaceDataPath"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            return dto;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin nhân viên: {ex.Message}", ex);
            }

            return null; // Không tìm thấy
        }

        /// <summary>
        /// Lấy danh sách nhân viên thuộc một phòng ban
        /// </summary>
        /// <param name="departmentId">ID của phòng ban</param>
        /// <returns>Danh sách EmployeeDTO</returns>
        public List<EmployeeDTO> GetEmployeesByDepartment(int departmentId)
        {
            List<EmployeeDTO> employeeList = new List<EmployeeDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName, 
                               e.Gender, e.DateOfBirth, e.IDCardNumber, e.Address, e.Phone, 
                               e.Email, e.DepartmentID, e.PositionID, e.ManagerID, 
                               e.HireDate, e.EndDate, e.Status, e.BankAccount, e.BankName, 
                               e.TaxCode, e.InsuranceCode, e.Notes, e.CreatedAt, e.UpdatedAt,
                               d.DepartmentName, p.PositionName, m.FullName AS ManagerName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                        WHERE e.DepartmentID = @DepartmentID
                        ORDER BY e.FullName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentID", departmentId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeDTO dto = new EmployeeDTO
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                IDCardNumber = reader["IDCardNumber"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"])
                            };

                            // Phòng ban và chức vụ
                            if (reader["DepartmentID"] != DBNull.Value)
                            {
                                dto.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                dto.DepartmentName = reader["DepartmentName"].ToString();
                            }

                            if (reader["PositionID"] != DBNull.Value)
                            {
                                dto.PositionID = Convert.ToInt32(reader["PositionID"]);
                                dto.PositionName = reader["PositionName"].ToString();
                            }

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"].ToString();
                            }

                            if (reader["Address"] != DBNull.Value)
                                dto.Address = reader["Address"].ToString();

                            if (reader["EndDate"] != DBNull.Value)
                                dto.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["BankAccount"] != DBNull.Value)
                                dto.BankAccount = reader["BankAccount"].ToString();

                            if (reader["BankName"] != DBNull.Value)
                                dto.BankName = reader["BankName"].ToString();

                            if (reader["TaxCode"] != DBNull.Value)
                                dto.TaxCode = reader["TaxCode"].ToString();

                            if (reader["InsuranceCode"] != DBNull.Value)
                                dto.InsuranceCode = reader["InsuranceCode"].ToString();

                            if (reader["Notes"] != DBNull.Value)
                                dto.Notes = reader["Notes"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            employeeList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nhân viên theo phòng ban: {ex.Message}", ex);
            }

            return employeeList;
        }

        /// <summary>
        /// Thêm nhân viên mới vào cơ sở dữ liệu
        /// </summary>
        /// <param name="employee">DTO hoặc model của nhân viên</param>
        /// <returns>ID của nhân viên mới được thêm</returns>
        public int AddEmployee(object employee)
        {
            Employee emp;

            // Chuyển đổi từ DTO nếu cần
            if (employee is EmployeeDTO dto)
            {
                emp = dto.ToEmployee();
            }
            else if (employee is Employee)
            {
                emp = (Employee)employee;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Employee hoặc EmployeeDTO", nameof(employee));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Employees (
                            EmployeeCode, FirstName, LastName, Gender, DateOfBirth, 
                            IDCardNumber, Address, Phone, Email, DepartmentID, 
                            PositionID, ManagerID, HireDate, Status,
                            BankAccount, BankName, TaxCode, InsuranceCode, Notes, 
                            FaceDataPath, CreatedAt, UpdatedAt
                        ) VALUES (
                            @EmployeeCode, @FirstName, @LastName, @Gender, @DateOfBirth,
                            @IDCardNumber, @Address, @Phone, @Email, @DepartmentID,
                            @PositionID, @ManagerID, @HireDate, @Status,
                            @BankAccount, @BankName, @TaxCode, @InsuranceCode, @Notes,
                            @FaceDataPath, GETDATE(), GETDATE()
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Thêm các tham số
                    command.Parameters.AddWithValue("@EmployeeCode", emp.EmployeeCode);
                    command.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    command.Parameters.AddWithValue("@LastName", emp.LastName);
                    command.Parameters.AddWithValue("@Gender", emp.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                    command.Parameters.AddWithValue("@IDCardNumber", emp.IDCardNumber);
                    command.Parameters.AddWithValue("@Phone", emp.Phone);
                    command.Parameters.AddWithValue("@Email", emp.Email);
                    command.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    command.Parameters.AddWithValue("@Status", emp.Status);

                    // Các tham số có thể null
                    if (string.IsNullOrEmpty(emp.Address))
                        command.Parameters.AddWithValue("@Address", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Address", emp.Address);

                    command.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);

                    command.Parameters.AddWithValue("@PositionID", emp.PositionID);

                    if (emp.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", emp.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    if (string.IsNullOrEmpty(emp.BankAccount))
                        command.Parameters.AddWithValue("@BankAccount", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@BankAccount", emp.BankAccount);

                    if (string.IsNullOrEmpty(emp.BankName))
                        command.Parameters.AddWithValue("@BankName", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@BankName", emp.BankName);

                    if (string.IsNullOrEmpty(emp.TaxCode))
                        command.Parameters.AddWithValue("@TaxCode", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@TaxCode", emp.TaxCode);

                    if (string.IsNullOrEmpty(emp.InsuranceCode))
                        command.Parameters.AddWithValue("@InsuranceCode", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@InsuranceCode", emp.InsuranceCode);

                    if (string.IsNullOrEmpty(emp.Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", emp.Notes);

                    if (string.IsNullOrEmpty(emp.FaceDataPath))
                        command.Parameters.AddWithValue("@FaceDataPath", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@FaceDataPath", emp.FaceDataPath);

                    connection.Open();
                    int newEmployeeId = Convert.ToInt32(command.ExecuteScalar());
                    return newEmployeeId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm nhân viên vào cơ sở dữ liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="employee">DTO hoặc model của nhân viên</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateEmployee(object employee)
        {
            Employee emp;

            // Chuyển đổi từ DTO nếu cần
            if (employee is EmployeeDTO dto)
            {
                emp = dto.ToEmployee();
            }
            else if (employee is Employee)
            {
                emp = (Employee)employee;
            }
            else
            {
                throw new ArgumentException("Tham số phải là Employee hoặc EmployeeDTO", nameof(employee));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Employees SET
                            FirstName = @FirstName,
                            LastName = @LastName,
                            Gender = @Gender,
                            DateOfBirth = @DateOfBirth,
                            IDCardNumber = @IDCardNumber,
                            Address = @Address,
                            Phone = @Phone,
                            Email = @Email,
                            DepartmentID = @DepartmentID,
                            PositionID = @PositionID,
                            ManagerID = @ManagerID,
                            HireDate = @HireDate,
                            EndDate = @EndDate,
                            Status = @Status,
                            BankAccount = @BankAccount,
                            BankName = @BankName,
                            TaxCode = @TaxCode,
                            InsuranceCode = @InsuranceCode,
                            Notes = @Notes,
                            FaceDataPath = @FaceDataPath,
                            UpdatedAt = GETDATE()
                        WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Thêm các tham số
                    command.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    command.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    command.Parameters.AddWithValue("@LastName", emp.LastName);
                    command.Parameters.AddWithValue("@Gender", emp.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                    command.Parameters.AddWithValue("@IDCardNumber", emp.IDCardNumber);
                    command.Parameters.AddWithValue("@Phone", emp.Phone);
                    command.Parameters.AddWithValue("@Email", emp.Email);
                    command.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    command.Parameters.AddWithValue("@Status", emp.Status);

                    // Các tham số có thể null
                    if (string.IsNullOrEmpty(emp.Address))
                        command.Parameters.AddWithValue("@Address", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Address", emp.Address);

                    command.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);

                    command.Parameters.AddWithValue("@PositionID", emp.PositionID);

                    if (emp.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", emp.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    if (emp.EndDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", emp.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    if (string.IsNullOrEmpty(emp.BankAccount))
                        command.Parameters.AddWithValue("@BankAccount", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@BankAccount", emp.BankAccount);

                    if (string.IsNullOrEmpty(emp.BankName))
                        command.Parameters.AddWithValue("@BankName", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@BankName", emp.BankName);

                    if (string.IsNullOrEmpty(emp.TaxCode))
                        command.Parameters.AddWithValue("@TaxCode", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@TaxCode", emp.TaxCode);

                    if (string.IsNullOrEmpty(emp.InsuranceCode))
                        command.Parameters.AddWithValue("@InsuranceCode", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@InsuranceCode", emp.InsuranceCode);

                    if (string.IsNullOrEmpty(emp.Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", emp.Notes);

                    if (string.IsNullOrEmpty(emp.FaceDataPath))
                        command.Parameters.AddWithValue("@FaceDataPath", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@FaceDataPath", emp.FaceDataPath);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật thông tin nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật phòng ban cho nhân viên
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <param name="departmentId">ID của phòng ban mới</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateEmployeeDepartment(int employeeId, int departmentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Employees SET
                            DepartmentID = @DepartmentID,
                            UpdatedAt = GETDATE()
                        WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@DepartmentID", departmentId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật phòng ban cho nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái của nhân viên
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <param name="status">Trạng thái mới</param>
        /// <param name="endDate">Ngày kết thúc (nếu nghỉ việc)</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateEmployeeStatus(int employeeId, string status, DateTime? endDate = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Employees SET
                            Status = @Status,
                            EndDate = @EndDate,
                            UpdatedAt = GETDATE()
                        WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@Status", status);

                    if (endDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", endDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa nhân viên khỏi cơ sở dữ liệu
        /// </summary>
        /// <param name="employeeId">ID của nhân viên cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeleteEmployee(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Kiểm tra ràng buộc trước khi xóa
                    string checkQuery = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Employees WHERE ManagerID = @EmployeeID) AS ManagedEmployees,
                            (SELECT COUNT(*) FROM Departments WHERE ManagerID = @EmployeeID) AS ManagedDepartments,
                            (SELECT COUNT(*) FROM Projects WHERE ManagerID = @EmployeeID) AS ManagedProjects,
                            (SELECT COUNT(*) FROM Tasks WHERE AssignedToID = @EmployeeID) AS AssignedTasks,
                            (SELECT COUNT(*) FROM ProjectEmployees WHERE EmployeeID = @EmployeeID) AS ProjectParticipations";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();

                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int managedEmployees = Convert.ToInt32(reader["ManagedEmployees"]);
                            int managedDepartments = Convert.ToInt32(reader["ManagedDepartments"]);
                            int managedProjects = Convert.ToInt32(reader["ManagedProjects"]);
                            int assignedTasks = Convert.ToInt32(reader["AssignedTasks"]);
                            int projectParticipations = Convert.ToInt32(reader["ProjectParticipations"]);

                            if (managedEmployees > 0 || managedDepartments > 0 || managedProjects > 0 ||
                                assignedTasks > 0 || projectParticipations > 0)
                            {
                                StringBuilder errorMessage = new StringBuilder("Không thể xóa nhân viên vì còn liên kết với:\n");

                                if (managedEmployees > 0)
                                    errorMessage.AppendLine($"- {managedEmployees} nhân viên đang được quản lý bởi người này");

                                if (managedDepartments > 0)
                                    errorMessage.AppendLine($"- {managedDepartments} phòng ban đang được quản lý bởi người này");

                                if (managedProjects > 0)
                                    errorMessage.AppendLine($"- {managedProjects} dự án đang được quản lý bởi người này");

                                if (assignedTasks > 0)
                                    errorMessage.AppendLine($"- {assignedTasks} công việc được giao cho người này");

                                if (projectParticipations > 0)
                                    errorMessage.AppendLine($"- {projectParticipations} dự án mà người này tham gia");

                                errorMessage.AppendLine("\nVui lòng cập nhật các liên kết trước khi xóa.");
                                throw new Exception(errorMessage.ToString());
                            }
                        }
                    }

                    // Nếu không có ràng buộc, tiến hành xóa
                    string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm nhân viên theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Từ khóa tìm kiếm</param>
        /// <param name="departmentId">ID phòng ban (nếu có)</param>
        /// <param name="status">Trạng thái nhân viên (nếu có)</param>
        /// <returns>Danh sách nhân viên thỏa mãn điều kiện</returns>
        public List<EmployeeDTO> SearchEmployees(string searchText, int? departmentId = null, string status = null)
        {
            List<EmployeeDTO> employeeList = new List<EmployeeDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName, 
                               e.Gender, e.DateOfBirth, e.IDCardNumber, e.Address, e.Phone, 
                               e.Email, e.DepartmentID, e.PositionID, e.ManagerID, 
                               e.HireDate, e.EndDate, e.Status, e.BankAccount, e.BankName, 
                               e.TaxCode, e.InsuranceCode, e.Notes, e.CreatedAt, e.UpdatedAt,
                               d.DepartmentName, p.PositionName, m.FullName AS ManagerName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                        WHERE 1=1");

                    SqlCommand command = new SqlCommand();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        queryBuilder.Append(@" AND (
                            e.FullName LIKE @SearchText OR 
                            e.EmployeeCode LIKE @SearchText OR
                            e.Phone LIKE @SearchText OR
                            e.Email LIKE @SearchText OR
                            e.IDCardNumber LIKE @SearchText)");
                        command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    }

                    if (departmentId.HasValue)
                    {
                        queryBuilder.Append(" AND e.DepartmentID = @DepartmentID");
                        command.Parameters.AddWithValue("@DepartmentID", departmentId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        queryBuilder.Append(" AND e.Status = @Status");
                        command.Parameters.AddWithValue("@Status", status);
                    }

                    queryBuilder.Append(" ORDER BY e.FullName");

                    command.CommandText = queryBuilder.ToString();
                    command.Connection = connection;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeDTO dto = new EmployeeDTO
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                IDCardNumber = reader["IDCardNumber"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"])
                            };

                            // Phòng ban và chức vụ
                            if (reader["DepartmentID"] != DBNull.Value)
                            {
                                dto.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                dto.DepartmentName = reader["DepartmentName"].ToString();
                            }

                            if (reader["PositionID"] != DBNull.Value)
                            {
                                dto.PositionID = Convert.ToInt32(reader["PositionID"]);
                                dto.PositionName = reader["PositionName"].ToString();
                            }

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                            {
                                dto.ManagerID = Convert.ToInt32(reader["ManagerID"]);
                                dto.ManagerName = reader["ManagerName"].ToString();
                            }

                            if (reader["Address"] != DBNull.Value)
                                dto.Address = reader["Address"].ToString();

                            if (reader["EndDate"] != DBNull.Value)
                                dto.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["BankAccount"] != DBNull.Value)
                                dto.BankAccount = reader["BankAccount"].ToString();

                            if (reader["BankName"] != DBNull.Value)
                                dto.BankName = reader["BankName"].ToString();

                            if (reader["TaxCode"] != DBNull.Value)
                                dto.TaxCode = reader["TaxCode"].ToString();

                            if (reader["InsuranceCode"] != DBNull.Value)
                                dto.InsuranceCode = reader["InsuranceCode"].ToString();

                            if (reader["Notes"] != DBNull.Value)
                                dto.Notes = reader["Notes"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                dto.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            employeeList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhân viên: {ex.Message}", ex);
            }

            return employeeList;
        }

        /// <summary>
        /// Tìm kiếm nhân viên có thể làm quản lý
        /// </summary>
        /// <param name="searchText">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhân viên</returns>
        public List<EmployeeDTO> SearchPotentialManagers(string searchText)
        {
            List<EmployeeDTO> employeeList = new List<EmployeeDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.Gender, 
                               e.Email, e.Phone, d.DepartmentName, p.PositionName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        WHERE e.Status = N'Đang làm việc'
                          AND (
                              e.FullName LIKE @SearchText OR
                              e.EmployeeCode LIKE @SearchText OR
                              e.Email LIKE @SearchText OR
                              e.Phone LIKE @SearchText
                          )
                        ORDER BY e.FullName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeDTO dto = new EmployeeDTO
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString()
                            };

                            if (reader["DepartmentName"] != DBNull.Value)
                                dto.DepartmentName = reader["DepartmentName"].ToString();

                            if (reader["PositionName"] != DBNull.Value)
                                dto.PositionName = reader["PositionName"].ToString();

                            employeeList.Add(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhân viên: {ex.Message}", ex);
            }

            return employeeList;
        }

        /// <summary>
        /// Kiểm tra quyền quản lý của một nhân viên
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <returns>Danh sách vai trò quản lý</returns>
        public List<ManagerRole> GetManagerRoles(int employeeId)
        {
            List<ManagerRole> roles = new List<ManagerRole>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        -- Kiểm tra các phòng ban mà nhân viên này quản lý
                        SELECT 
                            'Department' AS RoleType,
                            d.DepartmentID AS EntityID,
                            d.DepartmentName AS EntityName,
                            (SELECT COUNT(*) FROM Employees WHERE DepartmentID = d.DepartmentID) AS MemberCount
                        FROM Departments d
                        WHERE d.ManagerID = @EmployeeID
                        
                        UNION ALL
                        
                        -- Kiểm tra các dự án mà nhân viên này quản lý
                        SELECT 
                            'Project' AS RoleType,
                            p.ProjectID AS EntityID,
                            p.ProjectName AS EntityName,
                            (SELECT COUNT(*) FROM ProjectEmployees WHERE ProjectID = p.ProjectID) AS MemberCount
                        FROM Projects p
                        WHERE p.ManagerID = @EmployeeID
                        
                        ORDER BY RoleType, EntityName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ManagerRole role = new ManagerRole
                            {
                                RoleType = reader["RoleType"].ToString(),
                                EntityID = Convert.ToInt32(reader["EntityID"]),
                                EntityName = reader["EntityName"].ToString(),
                                MemberCount = Convert.ToInt32(reader["MemberCount"])
                            };

                            roles.Add(role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra vai trò quản lý: {ex.Message}", ex);
            }

            return roles;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của nhân viên bao gồm cả thông tin liên quan
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <returns>EmployeeDetailDTO</returns>
        public EmployeeDetailDTO GetEmployeeDetails(int employeeId)
        {
            try
            {
                // Lấy thông tin cơ bản của nhân viên
                var employeeDTO = GetEmployeeById(employeeId);
                if (employeeDTO == null)
                    return null;

                // Chuyển đổi sang EmployeeDetailDTO
                var detailDTO = EmployeeDetailDTO.FromEmployeeDTO(employeeDTO);

                // Lấy các vai trò quản lý
                detailDTO.ManagerRoles = GetManagerRoles(employeeId);

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Lấy danh sách dự án của nhân viên
                    string projectsQuery = @"
                        SELECT p.ProjectID, p.ProjectCode, p.ProjectName, p.StartDate, p.EndDate, 
                               p.Status, pe.RoleInProject
                        FROM Projects p
                        JOIN ProjectEmployees pe ON p.ProjectID = pe.ProjectID
                        WHERE pe.EmployeeID = @EmployeeID
                        ORDER BY p.StartDate DESC";

                    SqlCommand projectsCommand = new SqlCommand(projectsQuery, connection);
                    projectsCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = projectsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Tạo ProjectDTO từ dữ liệu đọc được và thêm vào danh sách
                            // Chi tiết ProjectDTO sẽ được triển khai sau
                            // Đây chỉ là placeholder
                            detailDTO.Projects.Add(new ProjectDTO());
                        }
                    }

                    // Lấy danh sách công việc được giao
                    string tasksQuery = @"
                        SELECT t.TaskID, t.TaskCode, t.TaskName, t.Description, 
                               t.StartDate, t.DueDate, t.CompletedDate, t.Status, t.Priority
                        FROM Tasks t
                        WHERE t.AssignedToID = @EmployeeID
                        ORDER BY t.DueDate DESC";

                    SqlCommand tasksCommand = new SqlCommand(tasksQuery, connection);
                    tasksCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = tasksCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Tạo TaskDTO từ dữ liệu đọc được và thêm vào danh sách
                            // Chi tiết TaskDTO sẽ được triển khai sau
                            // Đây chỉ là placeholder
                            detailDTO.Tasks.Add(new TaskDTO());
                        }
                    }

                    // Lấy lịch sử chấm công gần đây
                    string attendanceQuery = @"
                        SELECT TOP 10 AttendanceID, CheckInTime, CheckOutTime, WorkingHours, Status
                        FROM Attendance
                        WHERE EmployeeID = @EmployeeID
                        ORDER BY CheckInTime DESC";

                    SqlCommand attendanceCommand = new SqlCommand(attendanceQuery, connection);
                    attendanceCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = attendanceCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Tạo AttendanceDTO từ dữ liệu đọc được và thêm vào danh sách
                            // Chi tiết AttendanceDTO sẽ được triển khai sau
                            // Đây chỉ là placeholder
                            detailDTO.RecentAttendance.Add(new AttendanceDTO());
                        }
                    }

                    // Lấy lịch sử lương gần đây
                    string salaryQuery = @"
                        SELECT TOP 12 SalaryID, Month, Year, BaseSalary, Allowance, Bonus, Deduction, NetSalary
                        FROM Salary
                        WHERE EmployeeID = @EmployeeID
                        ORDER BY Year DESC, Month DESC";

                    SqlCommand salaryCommand = new SqlCommand(salaryQuery, connection);
                    salaryCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = salaryCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Tạo SalaryDTO từ dữ liệu đọc được và thêm vào danh sách
                            // Chi tiết SalaryDTO sẽ được triển khai sau
                            // Đây chỉ là placeholder
                            detailDTO.RecentSalary.Add(new SalaryDTO());
                        }
                    }
                }

                return detailDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin chi tiết nhân viên: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Mô tả vai trò quản lý của nhân viên
    /// </summary>
    public class ManagerRole
    {
        public string RoleType { get; set; } // Department, Project, etc.
        public int EntityID { get; set; }
        public string EntityName { get; set; }
        public int MemberCount { get; set; }
    }
}