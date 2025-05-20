using System;
using System.Windows.Forms;
using EmployeeManagement.GUI;
using EmployeeManagement.GUI.Auth;

namespace EmployeeManagement
{
    internal static class Program
    {
        /// <summary>
        /// Điểm vào chính của ứng dụng.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                var loginForm = new LoginForm();
                Application.Run(loginForm);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ứng dụng: {ex.Message}\n\nStack trace: {ex.StackTrace}",
                    "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}