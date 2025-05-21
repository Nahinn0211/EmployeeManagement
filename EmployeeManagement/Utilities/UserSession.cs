using System;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.Utilities
{
    public static class UserSession
    {
        // Thông tin cơ bản của người dùng đăng nhập
        public static int? UserId { get; private set; }
        public static string Username { get; private set; }
        public static DateTime LoginTime { get; private set; }

        // Kiểm tra đăng nhập
        public static bool IsLoggedIn => UserId.HasValue && !string.IsNullOrEmpty(Username);

        /// <summary>
        /// Đăng nhập và lưu thông tin session
        /// </summary>
        public static void Login(int userId, string username)
        {
            UserId = userId;
            Username = username;
            LoginTime = DateTime.Now;

           }

        /// <summary>
        /// Đăng xuất và xóa session
        /// </summary>
        public static void Logout()
        {
            var logoutTime = DateTime.Now;
            var sessionDuration = logoutTime - LoginTime;

            // Xóa thông tin session
            UserId = null;
            Username = null;
            LoginTime = default;
        }

        /// <summary>
        /// Lấy thông tin session dưới dạng string
        /// </summary>
        public static string GetSessionInfo()
        {
            if (!IsLoggedIn)
                return "Không có session nào đang hoạt động";

            return $"User: {Username} (ID: {UserId}) | Login: {LoginTime:dd/MM/yyyy HH:mm}";
        }
    }
}
