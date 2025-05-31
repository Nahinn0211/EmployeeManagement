using EmployeeManagement.DAL;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.BLL
{
    public class AuthBLL
    {
        private readonly AuthDAL authDAL;

        public AuthBLL()
        {
            authDAL = new AuthDAL();
        }

        public LoginResponse Login(LoginRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập không được để trống"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Mật khẩu không được để trống"
                    };
                }

                // Get user from database
                var user = authDAL.GetUserByCredentials(request.Username.Trim(), request.Password);
                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không chính xác"
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên"
                    };
                }

                // Update last login time
                authDAL.UpdateLastLogin(user.UserID);

                // Get user roles
                var userRoles = authDAL.GetUserRoles(user.UserID);
                var primaryRole = authDAL.GetPrimaryUserRole(user.UserID);

                // Set session với role information
                UserSession.Login(user.UserID, user.Username, primaryRole, userRoles);

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
                    Message = "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại"
                };
            }
        }

        public void Logout()
        {
            try
            {
                if (UserSession.IsLoggedIn)
                {
                    UserSession.Logout();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}", ex);
            }
        }

        public bool IsUserLoggedIn()
        {
            return UserSession.IsLoggedIn;
        }

        public string GetCurrentUserInfo()
        {
            return UserSession.GetSessionInfo();
        }
    }
}