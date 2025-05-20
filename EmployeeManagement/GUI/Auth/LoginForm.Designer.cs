using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Auth
{
    partial class LoginForm
    {
        /// <summary>
        /// Biến thiết kế bắt buộc.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Dọn dẹp mọi tài nguyên đang được sử dụng.
        /// </summary>
        /// <param name="disposing">true nếu tài nguyên được quản lý cần được giải phóng; ngược lại, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Mã được tạo bởi Windows Form Designer

        /// <summary>
        /// Phương thức bắt buộc cho hỗ trợ Designer - không sửa đổi
        /// nội dung của phương thức này với trình chỉnh sửa mã.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 865);
            Margin = new Padding(4, 3, 4, 3);
            Name = "LoginForm";
            Padding = new Padding(4, 74, 4, 3);
            Text = "Hệ thống Quản lý Nhân viên - Đăng nhập";
            ResumeLayout(false);
        }

        #endregion

        // Khai báo các điều khiển (sẽ được khởi tạo bằng mã lệnh)
        private MaterialCard mainCard;
        private PictureBox logoIcon;
        private MaterialLabel titleLabel;
        private MaterialLabel subtitleLabel;
        private MaterialTextBox txtUsername;
        private MaterialTextBox txtPassword;
        private MaterialCheckbox chkRememberMe;
        private MaterialButton btnForgotPassword;
        private MaterialButton btnLogin;
        private MaterialDivider divider;
        private MaterialButton btnExit;
        private ProgressBar progressBar;
        private MaterialButton btnLightTheme;
        private MaterialButton btnDarkTheme;
        private MaterialButton btnBlueScheme;
        private MaterialButton btnGreenScheme;
        private MaterialButton btnOrangeScheme;
    }
}