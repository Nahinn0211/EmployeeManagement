using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.DAL
{
    public class AuthDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        public User GetUserByCredentials(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"SELECT u.UserID, u.Username, u.Email, u.FullName, 
                                         u.EmployeeID, u.IsActive, u.LastLogin, 
                                         u.CreatedAt, u.UpdatedAt
                                  FROM Users u 
                                  WHERE u.Username = @Username 
                                    AND u.Password = @Password 
                                    AND u.IsActive = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = reader.GetInt32("UserID"),
                                    Username = reader.GetString("Username"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    FullName = reader.IsDBNull("FullName") ? null : reader.GetString("FullName"),
                                    EmployeeID = reader.IsDBNull("EmployeeID") ? (int?)null : reader.GetInt32("EmployeeID"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    LastLogin = reader.IsDBNull("LastLogin") ? (DateTime?)null : reader.GetDateTime("LastLogin"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi truy xuất thông tin đăng nhập: {ex.Message}");
            }

            return null;
        }

        public bool UpdateLastLogin(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"UPDATE Users 
                                  SET LastLogin = GETDATE(), 
                                      UpdatedAt = GETDATE() 
                                  WHERE UserID = @UserID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật thời gian đăng nhập: {ex.Message}");
            }
        }

        public List<string> GetUserRoles(int userId)
        {
            var roles = new List<string>();
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"SELECT r.RoleName 
                                  FROM UserRoles ur 
                                  INNER JOIN Roles r ON ur.RoleID = r.RoleID 
                                  WHERE ur.UserID = @UserID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(reader.GetString("RoleName"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy quyền người dùng: {ex.Message}");
            }

            return roles.Count > 0 ? roles : new List<string> { "User" };
        }

        public string GetPrimaryUserRole(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"SELECT TOP 1 r.RoleName 
                                  FROM UserRoles ur 
                                  INNER JOIN Roles r ON ur.RoleID = r.RoleID 
                                  WHERE ur.UserID = @UserID
                                  ORDER BY r.RoleID ASC"; // Admin có RoleID thấp hơn

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        var result = command.ExecuteScalar();
                        return result?.ToString() ?? "User";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy quyền chính của người dùng: {ex.Message}");
            }
        }
    }
}
