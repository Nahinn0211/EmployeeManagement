using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.DAL
{
    public class PermissionDAL
    {
        private string connectionString;

        public PermissionDAL()
        {
            connectionString = GetConnectionString();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        #region Role Management

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();

            string query = @"
                SELECT r.RoleID, r.RoleName, r.Description, r.CreatedAt,
                       COUNT(ur.UserID) as UserCount
                FROM Roles r
                LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID
                GROUP BY r.RoleID, r.RoleName, r.Description, r.CreatedAt
                ORDER BY r.RoleName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var role = new Role
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                RoleName = reader["RoleName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            roles.Add(role);
                        }
                    }
                }
            }

            return roles;
        }

        public Role GetRoleById(int roleId)
        {
            string query = @"
                SELECT r.RoleID, r.RoleName, r.Description, r.CreatedAt,
                       COUNT(ur.UserID) as UserCount
                FROM Roles r
                LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID
                WHERE r.RoleID = @RoleID
                GROUP BY r.RoleID, r.RoleName, r.Description, r.CreatedAt";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Role
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                RoleName = reader["RoleName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int AddRole(Role role)
        {
            string query = @"
                INSERT INTO Roles (RoleName, Description, CreatedAt)
                VALUES (@RoleName, @Description, @CreatedAt);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    command.Parameters.AddWithValue("@Description", role.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", role.CreatedAt);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool UpdateRole(Role role)
        {
            string query = @"
                UPDATE Roles 
                SET RoleName = @RoleName, Description = @Description
                WHERE RoleID = @RoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", role.RoleID);
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    command.Parameters.AddWithValue("@Description", role.Description ?? (object)DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteRole(int roleId)
        {
            // First, delete all user role assignments
            DeleteUserRolesByRole(roleId);

            string query = "DELETE FROM Roles WHERE RoleID = @RoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Role Validation

        public bool IsRoleNameExists(string roleName, int excludeRoleId = 0)
        {
            string query = "SELECT COUNT(*) FROM Roles WHERE RoleName = @RoleName AND RoleID != @ExcludeRoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleName", roleName);
                    command.Parameters.AddWithValue("@ExcludeRoleID", excludeRoleId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool CanDeleteRole(int roleId)
        {
            // Check if role is assigned to any users
            string query = "SELECT COUNT(*) FROM UserRoles WHERE RoleID = @RoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) == 0;
                }
            }
        }

        #endregion

        #region User-Role Management

        public List<User> GetUsersByRole(int roleId)
        {
            List<User> users = new List<User>();

            string query = @"
        SELECT u.UserID, u.Username, u.Email, u.FullName, u.IsActive, u.EmployeeID,
               e.FullName as EmployeeName, e.EmployeeCode,
               d.DepartmentName, p.PositionName,
               ur.AssignedAt
        FROM Users u
        INNER JOIN UserRoles ur ON u.UserID = ur.UserID
        LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
        LEFT JOIN Positions p ON e.PositionID = p.PositionID
        WHERE ur.RoleID = @RoleID
        ORDER BY u.Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                EmployeeID = reader["EmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["EmployeeID"]) : (int?)null
                            };

                            // Chỉ tạo Employee object nếu EmployeeID có giá trị
                            if (reader["EmployeeID"] != DBNull.Value)
                            {
                                user.Employee = new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    FullName = reader["EmployeeName"]?.ToString() ?? "",
                                    EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                                    Department = new Department { DepartmentName = reader["DepartmentName"]?.ToString() ?? "" },
                                    Position = new Position { PositionName = reader["PositionName"]?.ToString() ?? "" }
                                };
                            }

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public List<User> GetUsersWithoutRole(int roleId)
        {
            List<User> users = new List<User>();

            string query = @"
        SELECT u.UserID, u.Username, u.Email, u.FullName, u.IsActive, u.EmployeeID,
               e.FullName as EmployeeName, e.EmployeeCode,
               d.DepartmentName, p.PositionName
        FROM Users u
        LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
        LEFT JOIN Positions p ON e.PositionID = p.PositionID
        WHERE u.UserID NOT IN (
            SELECT UserID FROM UserRoles WHERE RoleID = @RoleID
        )
        AND u.IsActive = 1
        ORDER BY u.Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                EmployeeID = reader["EmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["EmployeeID"]) : (int?)null
                            };

                            // Chỉ tạo Employee object nếu EmployeeID có giá trị
                            if (reader["EmployeeID"] != DBNull.Value)
                            {
                                user.Employee = new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    FullName = reader["EmployeeName"]?.ToString() ?? "",
                                    EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                                    Department = new Department { DepartmentName = reader["DepartmentName"]?.ToString() ?? "" },
                                    Position = new Position { PositionName = reader["PositionName"]?.ToString() ?? "" }
                                };
                            }

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
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

        public bool AssignRoleToMultipleUsers(List<int> userIds, int roleId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserID = @UserID AND RoleID = @RoleID)
                            INSERT INTO UserRoles (UserID, RoleID, AssignedAt) VALUES (@UserID, @RoleID, @AssignedAt)";

                        foreach (int userId in userIds)
                        {
                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@RoleID", roleId);
                                command.Parameters.AddWithValue("@AssignedAt", DateTime.Now);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool RemoveRoleFromMultipleUsers(List<int> userIds, int roleId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM UserRoles WHERE UserID = @UserID AND RoleID = @RoleID";

                        foreach (int userId in userIds)
                        {
                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@RoleID", roleId);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        private bool DeleteUserRolesByRole(int roleId)
        {
            string query = "DELETE FROM UserRoles WHERE RoleID = @RoleID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleID", roleId);
                    connection.Open();
                    return command.ExecuteNonQuery() >= 0;
                }
            }
        }

        #endregion

        #region Statistics

        public PermissionStatistics GetPermissionStatistics()
        {
            var stats = new PermissionStatistics();

            string query = @"
                SELECT 
                    COUNT(DISTINCT r.RoleID) as TotalRoles,
                    COUNT(DISTINCT ur.UserID) as UsersWithRoles,
                    COUNT(DISTINCT u.UserID) as TotalUsers,
                    AVG(CAST(role_counts.RoleCount AS FLOAT)) as AvgRolesPerUser
                FROM Roles r
                LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID
                LEFT JOIN Users u ON ur.UserID = u.UserID
                CROSS JOIN (
                    SELECT ur2.UserID, COUNT(*) as RoleCount
                    FROM UserRoles ur2
                    GROUP BY ur2.UserID
                ) role_counts";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalRoles = Convert.ToInt32(reader["TotalRoles"]);
                            stats.UsersWithRoles = Convert.ToInt32(reader["UsersWithRoles"]);
                            stats.TotalUsers = Convert.ToInt32(reader["TotalUsers"]);
                            stats.AverageRolesPerUser = reader["AvgRolesPerUser"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AvgRolesPerUser"]);
                        }
                    }
                }
            }

            stats.UsersWithoutRoles = stats.TotalUsers - stats.UsersWithRoles;

            return stats;
        }

        public List<RoleUsageStatistic> GetRoleUsageStatistics()
        {
            List<RoleUsageStatistic> statistics = new List<RoleUsageStatistic>();

            string query = @"
                SELECT r.RoleID, r.RoleName, r.Description,
                       COUNT(ur.UserID) as UserCount,
                       CAST(COUNT(ur.UserID) * 100.0 / (SELECT COUNT(*) FROM Users WHERE IsActive = 1) AS DECIMAL(5,2)) as UsagePercentage
                FROM Roles r
                LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID
                LEFT JOIN Users u ON ur.UserID = u.UserID AND u.IsActive = 1
                GROUP BY r.RoleID, r.RoleName, r.Description
                ORDER BY UserCount DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statistics.Add(new RoleUsageStatistic
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                RoleName = reader["RoleName"].ToString(),
                                Description = reader["Description"].ToString(),
                                UserCount = Convert.ToInt32(reader["UserCount"]),
                                UsagePercentage = Convert.ToDecimal(reader["UsagePercentage"])
                            });
                        }
                    }
                }
            }

            return statistics;
        }

        #endregion
    }

  
}