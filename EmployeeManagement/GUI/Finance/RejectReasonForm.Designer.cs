namespace EmployeeManagement.GUI.Finance
{
    partial class RejectReasonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose custom controls
                if (mainLayout != null && !mainLayout.IsDisposed)
                    mainLayout.Dispose();

                if (headerPanel != null && !headerPanel.IsDisposed)
                    headerPanel.Dispose();

                if (contentPanel != null && !contentPanel.IsDisposed)
                    contentPanel.Dispose();

                if (footerPanel != null && !footerPanel.IsDisposed)
                    footerPanel.Dispose();

                if (titleLabel != null && !titleLabel.IsDisposed)
                    titleLabel.Dispose();

                if (financeInfoLabel != null && !financeInfoLabel.IsDisposed)
                    financeInfoLabel.Dispose();

                if (reasonLabel != null && !reasonLabel.IsDisposed)
                    reasonLabel.Dispose();

                if (reasonTextBox != null && !reasonTextBox.IsDisposed)
                    reasonTextBox.Dispose();

                if (confirmButton != null && !confirmButton.IsDisposed)
                    confirmButton.Dispose();

                if (cancelButton != null && !cancelButton.IsDisposed)
                    cancelButton.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // RejectReasonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RejectReasonForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Từ chối giao dịch tài chính";
            this.Load += new System.EventHandler(this.RejectReasonForm_Load);
            this.Shown += new System.EventHandler(this.RejectReasonForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RejectReasonForm_KeyDown);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// Event handler for form load
        /// </summary>
        private void RejectReasonForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                // Ensure form is properly positioned
                this.CenterToParent();

                // Set up tab order
                SetupTabOrder();

                // Apply any additional styling
                ApplyAdditionalStyling();

                // Set initial focus will be handled in Shown event
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Lỗi khi khởi tạo form từ chối: {ex.Message}",
                    "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for form shown - set focus after form is fully displayed
        /// </summary>
        private void RejectReasonForm_Shown(object sender, System.EventArgs e)
        {
            try
            {
                // Set focus to reason textbox and clear placeholder if any
                if (reasonTextBox != null && !reasonTextBox.IsDisposed)
                {
                    reasonTextBox.Focus();

                    // Clear placeholder text and set proper color
                    string placeholder = "Nhập lý do từ chối giao dịch này (tối đa 500 ký tự)...";
                    if (reasonTextBox.Text == placeholder)
                    {
                        reasonTextBox.Text = "";
                        reasonTextBox.ForeColor = System.Drawing.Color.Black;
                    }

                    // Select all text if any
                    reasonTextBox.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Form shown error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle keyboard shortcuts
        /// </summary>
        private void RejectReasonForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                // Handle Escape key to cancel
                if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    e.Handled = true;
                }
                // Handle Ctrl+Enter to confirm
                else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (confirmButton != null && confirmButton.Enabled)
                    {
                        ConfirmButton_Click(confirmButton, System.EventArgs.Empty);
                        e.Handled = true;
                    }
                }
                // Handle F1 for help (nếu cần)
                else if (e.KeyCode == System.Windows.Forms.Keys.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Key down error: {ex.Message}");
            }
        }

        /// <summary>
        /// Set up tab order for controls
        /// </summary>
        private void SetupTabOrder()
        {
            try
            {
                int tabIndex = 0;

                if (reasonTextBox != null)
                {
                    reasonTextBox.TabIndex = tabIndex++;
                    reasonTextBox.TabStop = true;
                }

                if (confirmButton != null)
                {
                    confirmButton.TabIndex = tabIndex++;
                    confirmButton.TabStop = true;
                }

                if (cancelButton != null)
                {
                    cancelButton.TabIndex = tabIndex++;
                    cancelButton.TabStop = true;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Tab order setup error: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply additional styling that might not be in the main setup
        /// </summary>
        private void ApplyAdditionalStyling()
        {
            try
            {
                // Ensure proper form styling
                this.BackColor = System.Drawing.Color.White;

                // Add subtle shadow effect (if supported)
                if (System.Environment.OSVersion.Version.Major >= 6) // Vista+
                {
                    // Can add DWM effects here if needed
                }

                // Ensure buttons have proper hover effects
                if (confirmButton != null)
                {
                    confirmButton.FlatAppearance.MouseOverBackColor =
                        System.Drawing.Color.FromArgb(200, 30, 30);
                }

                if (cancelButton != null)
                {
                    cancelButton.FlatAppearance.MouseOverBackColor =
                        System.Drawing.Color.FromArgb(140, 140, 140);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Additional styling error: {ex.Message}");
            }
        }

        /// <summary>
        /// Show help information (có thể implement sau)
        /// </summary>
        private void ShowHelp()
        {
            try
            {
                string helpText = "HƯỚNG DẪN SỬ DỤNG:\n\n" +
                                "• Nhập lý do từ chối giao dịch (tối thiểu 10 ký tự)\n" +
                                "• Lý do nên cụ thể và rõ ràng\n" +
                                "• Ấn Ctrl+Enter để xác nhận nhanh\n" +
                                "• Ấn Escape để hủy bỏ\n\n" +
                                "Giao dịch sau khi từ chối sẽ chuyển sang trạng thái 'Từ chối' " +
                                "và không thể chỉnh sửa được nữa.";

                System.Windows.Forms.MessageBox.Show(
                    helpText,
                    "Hướng dẫn - Từ chối giao dịch",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Show help error: {ex.Message}");
            }
        }

        /// <summary>
        /// Override ProcessCmdKey for additional keyboard handling
        /// </summary>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            try
            {
                // Handle Alt+F4 gracefully
                if (keyData == (System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return true;
                }

                return base.ProcessCmdKey(ref msg, keyData);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"ProcessCmdKey error: {ex.Message}");
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        /// <summary>
        /// Handle form validation when closing
        /// </summary>
        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            try
            {
                // If user is trying to close with data but hasn't confirmed
                if (this.DialogResult == System.Windows.Forms.DialogResult.None && HasValidInput())
                {
                    var result = System.Windows.Forms.MessageBox.Show(
                        "Bạn đã nhập lý do từ chối nhưng chưa xác nhận. Bạn có muốn thoát không?",
                        "Xác nhận thoát",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question,
                        System.Windows.Forms.MessageBoxDefaultButton.Button2);

                    if (result == System.Windows.Forms.DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                base.OnFormClosing(e);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Form closing error: {ex.Message}");
                base.OnFormClosing(e);
            }
        }

        /// <summary>
        /// Check if user has entered valid input
        /// </summary>
        private bool HasValidInput()
        {
            try
            {
                if (reasonTextBox == null || reasonTextBox.IsDisposed)
                    return false;

                string placeholder = "Nhập lý do từ chối giao dịch này (tối đa 500 ký tự)...";
                string text = reasonTextBox.Text?.Trim() ?? "";

                return !string.IsNullOrEmpty(text) &&
                       text != placeholder &&
                       text.Length >= 10;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"HasValidInput error: {ex.Message}");
                return false;
            }
        }
    }
}