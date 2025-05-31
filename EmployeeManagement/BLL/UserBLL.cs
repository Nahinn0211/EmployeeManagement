using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.BLL
{
    public class UserBLL
    {
        private UserDAL userDAL;

        public UserBLL()
        {
            userDAL = new UserDAL();
        }

        #region Basic CRUD Operations

        public List<User> GetAllUsers()
        {
            try
            {
                return userDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng: {ex.Message}", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ");

                return userDAL.GetUserById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin người dùng: {ex.Message}", ex);
            }
        }

        public int AddUser(User user)
        {
            try
            {
                ValidateUser(user);

                // Hash password
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                return userDAL.AddUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm người dùng: {ex.Message}", ex);
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                ValidateUser(user, true);

                user.UpdatedAt = DateTime.Now;

                return userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật người dùng: {ex.Message}", ex);
            }
        }

        public UserDeleteValidation CanDeleteUser(int userId)
        {
            try
            {
                var user = userDAL.GetUserById(userId);
                if (user == null)
                {
                    return new UserDeleteValidation
                    {
                        CanDelete = false,
                        Reason = "Người dùng không tồn tại"
                    };
                }

                // Check if user has any related records
                // You can add more validation logic here based on your business rules

                return new UserDeleteValidation
                {
                    CanDelete = true,
                    Reason = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new UserDeleteValidation
                {
                    CanDelete = false,
                    Reason = $"Lỗi khi kiểm tra: {ex.Message}"
                };
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var canDelete = CanDeleteUser(userId);
                if (!canDelete.CanDelete)
                    throw new InvalidOperationException(canDelete.Reason);

                return userDAL.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa người dùng: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation

        private void ValidateUser(User user, bool isUpdate = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Thông tin người dùng không được để trống");

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Tên đăng nhập không được để trống");

            if (user.Username.Length < 3)
                throw new ArgumentException("Tên đăng nhập phải có ít nhất 3 ký tự");

            if (user.Username.Length > 50)
                throw new ArgumentException("Tên đăng nhập không được vượt quá 50 ký tự");

            if (!isUpdate && string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Mật khẩu không được để trống");

            if (!isUpdate && user.Password.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự");

            if (!string.IsNullOrEmpty(user.Email))
            {
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("Email không hợp lệ");

                if (user.Email.Length > 100)
                    throw new ArgumentException("Email không được vượt quá 100 ký tự");
            }

            if (!string.IsNullOrEmpty(user.FullName) && user.FullName.Length > 100)
                throw new ArgumentException("Họ tên không được vượt quá 100 ký tự");

            // Check for duplicates
            int excludeId = isUpdate ? user.UserID : 0;

            if (IsUsernameExists(user.Username, excludeId))
                throw new ArgumentException("Tên đăng nhập này đã tồn tại");

            if (!string.IsNullOrEmpty(user.Email) && IsEmailExists(user.Email, excludeId))
                throw new ArgumentException("Email này đã được sử dụng");

            if (user.EmployeeID.HasValue && IsEmployeeAlreadyUser(user.EmployeeID.Value, excludeId))
                throw new ArgumentException("Nhân viên này đã có tài khoản người dùng");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsUsernameExists(string username, int excludeUserId = 0)
        {
            try
            {
                return userDAL.IsUsernameExists(username, excludeUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tên đăng nhập: {ex.Message}", ex);
            }
        }

        public bool IsEmailExists(string email, int excludeUserId = 0)
        {
            try
            {
                return userDAL.IsEmailExists(email, excludeUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra email: {ex.Message}", ex);
            }
        }

        public bool IsEmployeeAlreadyUser(int employeeId, int excludeUserId = 0)
        {
            try
            {
                return userDAL.IsEmployeeAlreadyUser(employeeId, excludeUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra nhân viên: {ex.Message}", ex);
            }
        }

        #endregion

        #region Authentication

        public LoginResponse ValidateLogin(LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null)
                    throw new ArgumentNullException(nameof(loginRequest));

                if (string.IsNullOrWhiteSpace(loginRequest.Username))
                    throw new ArgumentException("Tên đăng nhập không được để trống");

                if (string.IsNullOrWhiteSpace(loginRequest.Password))
                    throw new ArgumentException("Mật khẩu không được để trống");

                string hashedPassword = HashPassword(loginRequest.Password);
                var user = userDAL.ValidateUser(loginRequest.Username, hashedPassword);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không chính xác",
                        User = null
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tài khoản đã bị vô hiệu hóa",
                        User = null
                    };
                }

                // Update last login
                userDAL.UpdateLastLogin(user.UserID);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công",
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Lỗi đăng nhập: {ex.Message}",
                    User = null
                };
            }
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = userDAL.GetUserById(userId);
                if (user == null)
                    throw new ArgumentException("Người dùng không tồn tại");

                string hashedOldPassword = HashPassword(oldPassword);
                if (user.Password != hashedOldPassword)
                    throw new ArgumentException("Mật khẩu cũ không chính xác");

                if (newPassword.Length < 6)
                    throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự");

                user.Password = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;

                return userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đổi mật khẩu: {ex.Message}", ex);
            }
        }

        public bool ResetPassword(int userId, string newPassword)
        {
            try
            {
                var user = userDAL.GetUserById(userId);
                if (user == null)
                    throw new ArgumentException("Người dùng không tồn tại");

                if (newPassword.Length < 6)
                    throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự");

                user.Password = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;

                return userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi reset mật khẩu: {ex.Message}", ex);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        #endregion

        #region Role Management

        public List<Role> GetUserRoles(int userId)
        {
            try
            {
                return userDAL.GetUserRoles(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy quyền của người dùng: {ex.Message}", ex);
            }
        }

        public bool AssignRoleToUser(int userId, int roleId)
        {
            try
            {
                return userDAL.AssignRoleToUser(userId, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gán quyền cho người dùng: {ex.Message}", ex);
            }
        }

        public bool RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                return userDAL.RemoveRoleFromUser(userId, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa quyền của người dùng: {ex.Message}", ex);
            }
        }

        public bool UpdateUserRoles(int userId, List<int> roleIds)
        {
            try
            {
                // First, remove all existing roles
                userDAL.DeleteUserRoles(userId);

                // Then, assign new roles
                foreach (int roleId in roleIds)
                {
                    userDAL.AssignRoleToUser(userId, roleId);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật quyền người dùng: {ex.Message}", ex);
            }
        }

        #endregion

        #region Account Management

        public bool ActivateUser(int userId)
        {
            try
            {
                var user = userDAL.GetUserById(userId);
                if (user == null)
                    throw new ArgumentException("Người dùng không tồn tại");

                user.IsActive = true;
                user.UpdatedAt = DateTime.Now;

                return userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kích hoạt tài khoản: {ex.Message}", ex);
            }
        }

        public bool DeactivateUser(int userId)
        {
            try
            {
                var user = userDAL.GetUserById(userId);
                if (user == null)
                    throw new ArgumentException("Người dùng không tồn tại");

                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;

                return userDAL.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi vô hiệu hóa tài khoản: {ex.Message}", ex);
            }
        }

        #endregion

        #region Statistics and Reports

        public UserStatistics GetUserStatistics()
        {
            try
            {
                return userDAL.GetUserStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê người dùng: {ex.Message}", ex);
            }
        }

        public List<UserDisplayModel> GetUsersForDisplay()
        {
            try
            {
                var users = userDAL.GetAllUsers();
                var displayModels = new List<UserDisplayModel>();

                foreach (var user in users)
                {
                    displayModels.Add(new UserDisplayModel
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email ?? "",
                        FullName = user.FullName ?? "",
                        EmployeeName = user.Employee?.FullName ?? "Chưa liên kết",
                        EmployeeCode = user.Employee?.EmployeeCode ?? "",
                        DepartmentName = user.Employee?.Department?.DepartmentName ?? "",
                        PositionName = user.Employee?.Position?.PositionName ?? "",
                        IsActive = user.IsActive,
                        IsActiveDisplay = user.IsActive ? "✅ Hoạt động" : "❌ Vô hiệu",
                        LastLogin = user.LastLogin,
                        LastLoginDisplay = user.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập",
                        CreatedAt = user.CreatedAt,
                        CreatedAtDisplay = user.CreatedAt.ToString("dd/MM/yyyy"),
                        HasEmployee = user.EmployeeID.HasValue
                    });
                }

                return displayModels;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hiển thị: {ex.Message}", ex);
            }
        }

        #endregion

        #region Helper Methods

        public List<Employee> GetAvailableEmployees()
        {
            try
            {
                return userDAL.GetAvailableEmployees();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên khả dụng: {ex.Message}", ex);
            }
        }

        public List<Role> GetAllRoles()
        {
            try
            {
                return userDAL.GetAllRoles();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách quyền: {ex.Message}", ex);
            }
        }

        public string GenerateUsername(string fullName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullName))
                    return "user" + DateTime.Now.Ticks.ToString().Substring(0, 6);

                // Convert Vietnamese to ASCII
                string username = RemoveVietnameseAccent(fullName.ToLower())
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Replace("_", "");

                // Take first 8 characters
                if (username.Length > 8)
                    username = username.Substring(0, 8);

                // Check if exists and add number if needed
                string originalUsername = username;
                int counter = 1;

                while (IsUsernameExists(username))
                {
                    username = originalUsername + counter.ToString();
                    counter++;
                }

                return username;
            }
            catch
            {
                return "user" + DateTime.Now.Ticks.ToString().Substring(0, 6);
            }
        }

        private string RemoveVietnameseAccent(string text)
        {
            string[] vietnameseChars = new string[]
            {
                "aàáạảãâầấậẩẫăằắặẳẵ",
                "AÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴ",
                "dđ", "DĐ",
                "eèéẹẻẽêềếệểễ",
                "EÈÉẸẺẼÊỀẾỆỂỄ",
                "iìíịỉĩ",
                "IÌÍỊỈĨ",
                "oòóọỏõôồốộổỗơờớợởỡ",
                "OÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠ",
                "uùúụủũưừứựửữ",
                "UÙÚỤỦŨƯỪỨỰỬỮ",
                "yỳýỵỷỹ",
                "YỲÝỴỶỸ"
            };

            for (int i = 1; i < vietnameseChars.Length; i++)
            {
                for (int j = 1; j < vietnameseChars[i].Length; j++)
                    text = text.Replace(vietnameseChars[i][j], vietnameseChars[i][0]);
            }

            return text;
        }

        public string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(validChars[random.Next(validChars.Length)]);
            }

            return sb.ToString();
        }

        #endregion
    }
}