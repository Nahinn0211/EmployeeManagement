using System;
using System.Collections.Generic;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.Utilities
{
    public static class UserSession
    {
        // Thông tin cơ bản của người dùng đăng nhập
        public static int? UserId { get; private set; }
        public static string Username { get; private set; }
        public static string UserRole { get; private set; }
        public static List<string> UserRoles { get; private set; } = new List<string>();
        public static DateTime LoginTime { get; private set; }

        // Kiểm tra đăng nhập
        public static bool IsLoggedIn => UserId.HasValue && !string.IsNullOrEmpty(Username);

        /// <summary>
        /// Đăng nhập và lưu thông tin session
        /// </summary>
        public static void Login(int userId, string username, string primaryRole = "User", List<string> roles = null)
        {
            UserId = userId;
            Username = username;
            UserRole = primaryRole;
            UserRoles = roles ?? new List<string> { primaryRole };
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
            UserRole = null;
            UserRoles?.Clear();
            LoginTime = default;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập
        /// </summary>
        public static bool HasRole(string roleName)
        {
            return UserRoles?.Contains(roleName) == true;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập menu
        /// </summary>
        public static bool HasMenuPermission(string menuKey)
        {
            return PermissionManager.HasMenuPermission(UserRole, menuKey);
        }

        /// <summary>
        /// Lấy thông tin session dưới dạng string
        /// </summary>
        public static string GetSessionInfo()
        {
            if (!IsLoggedIn)
                return "Không có session nào đang hoạt động";
            return $"User: {Username} (ID: {UserId}) | Role: {UserRole} | Login: {LoginTime:dd/MM/yyyy HH:mm}";
        }
    }
}