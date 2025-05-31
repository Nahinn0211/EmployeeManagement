using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.DAL
{
    public class UserDAL
    {
        private string connectionString;

        public UserDAL()
        {
            connectionString = GetConnectionString();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        #region Basic CRUD Operations

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            string query = @"
                SELECT u.UserID, u.Username, u.Password, u.Email, u.FullName, 
                       u.EmployeeID, u.IsActive, u.LastLogin, u.CreatedAt, u.UpdatedAt,
                       e.FullName as EmployeeName, e.EmployeeCode,
                       d.DepartmentName, p.PositionName
                FROM Users u
                LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                ORDER BY u.CreatedAt DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Email = reader["Email"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                EmployeeID = reader["EmployeeID"] == DBNull.Value ? null : Convert.ToInt32(reader["EmployeeID"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastLogin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            // Create Employee object if exists
                            if (user.EmployeeID.HasValue)
                            {
                                user.Employee = new Employee
                                {
                                    EmployeeID = user.EmployeeID.Value,
                                    FullName = reader["EmployeeName"].ToString(),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    Department = new Department { DepartmentName = reader["DepartmentName"].ToString() },
                                    Position = new Position { PositionName = reader["PositionName"].ToString() }
                                };
                            }

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public User GetUserById(int userId)
        {
            string query = @"
                SELECT u.UserID, u.Username, u.Password, u.Email, u.FullName, 
                       u.EmployeeID, u.IsActive, u.LastLogin, u.CreatedAt, u.UpdatedAt,
                       e.FullName as EmployeeName, e.EmployeeCode,
                       d.DepartmentName, p.PositionName
                FROM Users u
                LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE u.UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Email = reader["Email"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                EmployeeID = reader["EmployeeID"] == DBNull.Value ? null : Convert.ToInt32(reader["EmployeeID"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastLogin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };

                            if (user.EmployeeID.HasValue)
                            {
                                user.Employee = new Employee
                                {
                                    EmployeeID = user.EmployeeID.Value,
                                    FullName = reader["EmployeeName"].ToString(),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    Department = new Department { DepartmentName = reader["DepartmentName"].ToString() },
                                    Position = new Position { PositionName = reader["PositionName"].ToString() }
                                };
                            }

                            return user;
                        }
                    }
                }
            }

            return null;
        }

        public int AddUser(User user)
        {
            string query = @"
                INSERT INTO Users (Username, Password, Email, FullName, EmployeeID, IsActive, CreatedAt, UpdatedAt)
                VALUES (@Username, @Password, @Email, @FullName, @EmployeeID, @IsActive, @CreatedAt, @UpdatedAt);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FullName", user.FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeID", user.EmployeeID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool UpdateUser(User user)
        {
            string query = @"
                UPDATE Users 
                SET Username = @Username, Password = @Password, Email = @Email, 
                    FullName = @FullName, EmployeeID = @EmployeeID, IsActive = @IsActive, 
                    UpdatedAt = @UpdatedAt
                WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FullName", user.FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeID", user.EmployeeID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteUser(int userId)
        {
            // First, delete user roles
            DeleteUserRoles(userId);

            string query = "DELETE FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Validation Methods

        public bool IsUsernameExists(string username, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID != @ExcludeUserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ExcludeUserID", excludeUserId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool IsEmailExists(string email, int excludeUserId = 0)
        {
            if (string.IsNullOrEmpty(email)) return false;

            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND UserID != @ExcludeUserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@ExcludeUserID", excludeUserId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool IsEmployeeAlreadyUser(int employeeId, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE EmployeeID = @EmployeeID AND UserID != @ExcludeUserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@ExcludeUserID", excludeUserId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        #endregion

        #region User Authentication

        public User ValidateUser(string username, string password)
        {
            string query = @"
                SELECT u.UserID, u.Username, u.Password, u.Email, u.FullName, 
                       u.EmployeeID, u.IsActive, u.LastLogin, u.CreatedAt, u.UpdatedAt,
                       e.FullName as EmployeeName
                FROM Users u
                LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
                WHERE u.Username = @Username AND u.Password = @Password AND u.IsActive = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Email = reader["Email"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                EmployeeID = reader["EmployeeID"] == DBNull.Value ? null : Convert.ToInt32(reader["EmployeeID"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastLogin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET LastLogin = @LastLogin WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@LastLogin", DateTime.Now);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Role Management

        public List<Role> GetUserRoles(int userId)
        {
            List<Role> roles = new List<Role>();

            string query = @"
                SELECT r.RoleID, r.RoleName, r.Description, r.CreatedAt
                FROM Roles r
                INNER JOIN UserRoles ur ON r.RoleID = ur.RoleID
                WHERE ur.UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                RoleName = reader["RoleName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }

            return roles;
        }

        public bool AssignRoleToUser(int userId, int roleId)
        {
            string query = @"
                IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserID = @UserID AND RoleID = @RoleID)
                INSERT INTO UserRoles (UserID, RoleID, AssignedAt) VALUES (@UserID, @RoleID, @AssignedAt)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    command.Parameters.AddWithValue("@AssignedAt", DateTime.Now);

                    connection.Open();
                    return command.ExecuteNonQuery() >= 0;
                }
            }
        }

        public bool RemoveRoleFromUser(int userId, int roleId)
        {
            string query = "DELETE FROM UserRoles WHERE UserID = @UserID AND RoleID = @RoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteUserRoles(int userId)
        {
            string query = "DELETE FROM UserRoles WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    return command.ExecuteNonQuery() >= 0;
                }
            }
        }

        #endregion

        #region Statistics

        public UserStatistics GetUserStatistics()
        {
            string query = @"
                SELECT 
                    COUNT(*) as TotalUsers,
                    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as ActiveUsers,
                    SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) as InactiveUsers,
                    SUM(CASE WHEN EmployeeID IS NOT NULL THEN 1 ELSE 0 END) as UsersWithEmployee,
                    SUM(CASE WHEN EmployeeID IS NULL THEN 1 ELSE 0 END) as UsersWithoutEmployee,
                    SUM(CASE WHEN LastLogin >= DATEADD(day, -30, GETDATE()) THEN 1 ELSE 0 END) as RecentlyActiveUsers
                FROM Users";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserStatistics
                            {
                                TotalUsers = Convert.ToInt32(reader["TotalUsers"]),
                                ActiveUsers = Convert.ToInt32(reader["ActiveUsers"]),
                                InactiveUsers = Convert.ToInt32(reader["InactiveUsers"]),
                                UsersWithEmployee = Convert.ToInt32(reader["UsersWithEmployee"]),
                                UsersWithoutEmployee = Convert.ToInt32(reader["UsersWithoutEmployee"]),
                                RecentlyActiveUsers = Convert.ToInt32(reader["RecentlyActiveUsers"])
                            };
                        }
                    }
                }
            }

            return new UserStatistics();
        }

        #endregion

        #region Helper Methods

        public List<Employee> GetAvailableEmployees()
        {
            List<Employee> employees = new List<Employee>();

            string query = @"
                SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName,
                       d.DepartmentName, p.PositionName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Positions p ON e.PositionID = p.PositionID
                WHERE e.EmployeeID NOT IN (SELECT EmployeeID FROM Users WHERE EmployeeID IS NOT NULL)
                AND e.Status = N'Đang làm việc'
                ORDER BY e.FullName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Department = new Department { DepartmentName = reader["DepartmentName"].ToString() },
                                Position = new Position { PositionName = reader["PositionName"].ToString() }
                            });
                        }
                    }
                }
            }

            return employees;
        }

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();

            string query = "SELECT RoleID, RoleName, Description, CreatedAt FROM Roles ORDER BY RoleName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                RoleName = reader["RoleName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }

            return roles;
        }

        #endregion
    }
}