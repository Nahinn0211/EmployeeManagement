using System;
using System.Collections.Generic;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.Utilities
{
    /// <summary>
    /// Quản lý session người dùng - Version thống nhất
    /// </summary>
    public static class UserSession
    {
        #region Private Fields
        private static int? _currentUserId;
        private static string _currentUserName;
        private static string _currentUserRole;
        private static List<string> _userRoles = new List<string>();
        private static DateTime? _loginTime;
        #endregion

        #region Public Properties
        /// <summary>
        /// ID người dùng hiện tại
        /// </summary>
        public static int CurrentUserId
        {
            get
            {
                if (!_currentUserId.HasValue)
                    throw new InvalidOperationException("Chưa có người dùng đăng nhập");
                return _currentUserId.Value;
            }
            private set => _currentUserId = value;
        }

        /// <summary>
        /// Tên người dùng hiện tại
        /// </summary>
        public static string CurrentUserName
        {
            get => _currentUserName ?? "Unknown";
            private set => _currentUserName = value;
        }

        /// <summary>
        /// Vai trò chính của người dùng
        /// </summary>
        public static string CurrentUserRole
        {
            get => _currentUserRole ?? "User";
            private set => _currentUserRole = value;
        }

        /// <summary>
        /// Danh sách tất cả vai trò của người dùng
        /// </summary>
        public static List<string> UserRoles
        {
            get => _userRoles ?? new List<string>();
            private set => _userRoles = value ?? new List<string>();
        }

        /// <summary>
        /// Thời gian đăng nhập
        /// </summary>
        public static DateTime? LoginTime
        {
            get => _loginTime;
            private set => _loginTime = value;
        }

        /// <summary>
        /// Kiểm tra có đăng nhập hay không
        /// </summary>
        public static bool IsLoggedIn => _currentUserId.HasValue && !string.IsNullOrEmpty(_currentUserName);

        // ===== COMPATIBILITY PROPERTIES - Để tương thích với code cũ =====
        /// <summary>
        /// ID người dùng (tương thích với code cũ)
        /// </summary>
        public static int? UserId => _currentUserId;

        /// <summary>
        /// Tên đăng nhập (tương thích với code cũ)
        /// </summary>
        public static string Username => _currentUserName ?? "Unknown";

        /// <summary>
        /// Vai trò người dùng (tương thích với code cũ)
        /// </summary>
        public static string UserRole => _currentUserRole ?? "User";
        #endregion

        #region Public Methods
        /// <summary>
        /// Đăng nhập và lưu thông tin session
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="userName">Tên người dùng</param>
        /// <param name="userRole">Vai trò chính</param>
        /// <param name="roles">Danh sách vai trò (tùy chọn)</param>
        public static void SetCurrentUser(int userId, string userName, string userRole, List<string> roles = null)
        {
            _currentUserId = userId;
            _currentUserName = userName;
            _currentUserRole = userRole;
            _userRoles = roles ?? new List<string> { userRole };
            _loginTime = DateTime.Now;

            Logger.LogInfo($"User logged in: {userName} (ID: {userId}) with role: {userRole}");
        }

        /// <summary>
        /// Đăng nhập (alias cho SetCurrentUser để tương thích với code cũ)
        /// </summary>
        public static void Login(int userId, string username, string primaryRole = "User", List<string> roles = null)
        {
            SetCurrentUser(userId, username, primaryRole, roles);
        }

        /// <summary>
        /// Đăng xuất và xóa session
        /// </summary>
        public static void ClearSession()
        {
            if (IsLoggedIn)
            {
                var logoutTime = DateTime.Now;
                var sessionDuration = logoutTime - (_loginTime ?? DateTime.Now);
                Logger.LogInfo($"User logged out: {_currentUserName} (Session duration: {sessionDuration:hh\\:mm\\:ss})");
            }

            _currentUserId = null;
            _currentUserName = null;
            _currentUserRole = null;
            _userRoles?.Clear();
            _loginTime = null;
        }

        /// <summary>
        /// Đăng xuất (alias cho ClearSession để tương thích)
        /// </summary>
        public static void Logout()
        {
            ClearSession();
        }

        /// <summary>
        /// Kiểm tra có vai trò cụ thể
        /// </summary>
        /// <param name="roleName">Tên vai trò cần kiểm tra</param>
        /// <returns>True nếu có vai trò</returns>
        public static bool HasRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;

            return _userRoles?.Contains(roleName) == true ||
                   string.Equals(_currentUserRole, roleName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo permission
        /// </summary>
        /// <param name="permission">Tên permission cần kiểm tra</param>
        /// <returns>True nếu có quyền</returns>
        public static bool HasPermission(string permission)
        {
            if (!IsLoggedIn || string.IsNullOrEmpty(permission))
                return false;

            // Logic phân quyền dựa theo vai trò
            return _currentUserRole switch
            {
                "Admin" => true, // Admin có tất cả quyền
                "Manager" => permission != "DeleteUser" && permission != "SystemConfig",
                "HR" => permission.Contains("Employee") || permission.Contains("Report"),
                "User" => permission == "ViewData" || permission == "EditOwn" || permission == "ViewDashboard",
                _ => false
            };
        }

        /// <summary>
        /// Kiểm tra quyền truy cập menu
        /// </summary>
        /// <param name="menuKey">Key của menu</param>
        /// <returns>True nếu có quyền truy cập</returns>
        public static bool HasMenuPermission(string menuKey)
        {
            if (!IsLoggedIn || string.IsNullOrEmpty(menuKey))
                return false;

            try
            {
                return PermissionManager.HasMenuPermission(_currentUserRole, menuKey);
            }
            catch
            {
                // Fallback nếu PermissionManager chưa implement
                return HasPermission($"Menu_{menuKey}");
            }
        }

        /// <summary>
        /// Lấy thông tin session dưới dạng string
        /// </summary>
        /// <returns>Chuỗi mô tả session</returns>
        public static string GetSessionInfo()
        {
            if (!IsLoggedIn)
                return "Không có session nào đang hoạt động";

            var sessionDuration = DateTime.Now - (_loginTime ?? DateTime.Now);
            return $"User: {_currentUserName} (ID: {_currentUserId}) | Role: {_currentUserRole} | " +
                   $"Login: {_loginTime:dd/MM/yyyy HH:mm} | Duration: {sessionDuration:hh\\:mm\\:ss}";
        }

        /// <summary>
        /// Kiểm tra session có hết hạn không (nếu cần)
        /// </summary>
        /// <param name="timeoutMinutes">Số phút timeout (mặc định 480 = 8 giờ)</param>
        /// <returns>True nếu session hết hạn</returns>
        public static bool IsSessionExpired(int timeoutMinutes = 480)
        {
            if (!IsLoggedIn || !_loginTime.HasValue)
                return true;

            var sessionDuration = DateTime.Now - _loginTime.Value;
            return sessionDuration.TotalMinutes > timeoutMinutes;
        }

        /// <summary>
        /// Gia hạn session (cập nhật thời gian hoạt động cuối)
        /// </summary>
        public static void RefreshSession()
        {
            if (IsLoggedIn)
            {
                _loginTime = DateTime.Now;
            }
        }
        #endregion

        #region For Debugging
        /// <summary>
        /// Debug info - chỉ dùng khi develop
        /// </summary>
        public static string GetDebugInfo()
        {
            return $"SessionManager Debug:\n" +
                   $"- UserId: {_currentUserId}\n" +
                   $"- UserName: {_currentUserName}\n" +
                   $"- UserRole: {_currentUserRole}\n" +
                   $"- Roles Count: {_userRoles?.Count ?? 0}\n" +
                   $"- LoginTime: {_loginTime}\n" +
                   $"- IsLoggedIn: {IsLoggedIn}";
        }
        public static string GetDetailedPermissionInfo()
        {
            if (!IsLoggedIn)
                return "Chưa đăng nhập";

            return PermissionManager.GetPermissionDebugInfo(_currentUserRole);
        }

        /// <summary>
        /// TÙNG CHỌN - THÊM METHOD NÀY để kiểm tra nhiều quyền menu cùng lúc
        /// </summary>
        public static bool HasAnyMenuPermission(params string[] menuKeys)
        {
            if (!IsLoggedIn || menuKeys == null || menuKeys.Length == 0)
                return false;

            return menuKeys.Any(HasMenuPermission);
        }

        /// <summary>
        /// TÙNG CHỌN - THÊM METHOD NÀY để lấy menu đầu tiên có quyền
        /// </summary>
        public static string GetFirstAllowedMenu(params string[] menuKeys)
        {
            if (!IsLoggedIn || menuKeys == null || menuKeys.Length == 0)
                return null;

            return menuKeys.FirstOrDefault(HasMenuPermission);
        }
        #endregion
    }
}