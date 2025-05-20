namespace EmployeeManagement.GUI.Menu
{
    partial class MainMenu
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

        #region Mã được tạo bởi Component Designer

        /// <summary>
        /// Phương thức bắt buộc cho hỗ trợ Designer - không sửa đổi 
        /// nội dung của phương thức này với trình chỉnh sửa mã.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "MainMenu";
            this.Size = new System.Drawing.Size(320, 600);
            this.ResumeLayout(false);
        }

        #endregion
    }
}