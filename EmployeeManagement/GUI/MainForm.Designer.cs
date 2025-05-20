using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI
{
    partial class MainForm
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
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Name = "MainForm";
            this.Text = "Hệ thống Quản lý Nhân viên";
            this.ResumeLayout(false);
        }

        #endregion
    }
}