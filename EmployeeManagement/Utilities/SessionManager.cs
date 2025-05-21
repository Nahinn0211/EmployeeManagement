using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
using MaterialSkin.Controls;
using System;

namespace EmployeeManagement.Utilities
{
    public static class SessionManager
    {
        private static int? _currentUserId;
        private static string _currentUserName;
        private static string _currentUserRole;
        private static DateTime? _loginTime;

        public static int CurrentUserId
        {
            get
            {
                if (!_currentUserId.HasValue)
                    throw new InvalidOperationException("Chưa có người dùng đăng nhập");
                return _currentUserId.Value;
            }
            set => _currentUserId = value;
        }

        public static string CurrentUserName
        {
            get => _currentUserName ?? "Unknown";
            set => _currentUserName = value;
        }

        public static string CurrentUserRole
        {
            get => _currentUserRole ?? "User";
            set => _currentUserRole = value;
        }

        public static DateTime? LoginTime
        {
            get => _loginTime;
            set => _loginTime = value;
        }

        public static bool IsLoggedIn => _currentUserId.HasValue;

        public static void SetCurrentUser(int userId, string userName, string userRole)
        {
            _currentUserId = userId;
            _currentUserName = userName;
            _currentUserRole = userRole;
            _loginTime = DateTime.Now;
        }

        public static void ClearSession()
        {
            _currentUserId = null;
            _currentUserName = null;
            _currentUserRole = null;
            _loginTime = null;
        }

        public static bool HasPermission(string permission)
        {
            // Implement role-based permissions
            return _currentUserRole switch
            {
                "Admin" => true,
                "Manager" => permission != "DeleteUser",
                "User" => permission == "ViewData" || permission == "EditOwn",
                _ => false
            };
        }
    }
}
