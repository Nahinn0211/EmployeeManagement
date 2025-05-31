using System;
using System.Windows.Forms;
using EmployeeManagement.GUI.Auth;
using EmployeeManagement.Utilities;

namespace EmployeeManagement
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

          
            try
            {
                // Chạy LoginForm làm form chính
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                 MessageBox.Show(
                    $"Có lỗi nghiêm trọng xảy ra:\n{ex.Message}\n\nVui lòng khởi động lại ứng dụng.",
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Đảm bảo đăng xuất session khi thoát
                if (UserSession.IsLoggedIn)
                {
                    UserSession.Logout();
                }

             }
        }
    }
}