namespace EmployeeManagement.GUI.Finance
{
    partial class ProjectFinanceForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            // ProjectFinanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProjectFinanceForm";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Text = "Quản lý Tài chính Dự án";
            this.Load += new System.EventHandler(this.ProjectFinanceForm_Load);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// Event handler for form load
        /// </summary>
        private void ProjectFinanceForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                // Set initial focus and state
                if (projectComboBox != null && !projectComboBox.IsDisposed)
                {
                    projectComboBox.Focus();
                }

                // Update button states
                UpdateButtonStates();

                // Show welcome message
                if (projects != null && projects.Count > 0)
                {
                    totalLabel.Text = $"💡 Có {projects.Count} dự án. Chọn dự án để xem chi tiết tài chính.";
                }
                else
                {
                    totalLabel.Text = "⚠️ Chưa có dự án nào. Vui lòng tạo dự án trước.";
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Lỗi khi khởi tạo form quản lý tài chính dự án:\n{ex.Message}",
                    "Lỗi khởi tạo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Update button states based on current conditions
        /// </summary>
        private void UpdateButtonStates()
        {
            try
            {
                bool hasProjectSelected = selectedProject != null;
                bool hasFinanceSelected = financeDataGridView != null &&
                                        financeDataGridView.SelectedRows.Count > 0;

                // Add button enabled when project is selected
                if (addFinanceButton != null)
                    addFinanceButton.Enabled = hasProjectSelected;

                // Other buttons enabled when finance is selected
                if (editFinanceButton != null)
                    editFinanceButton.Enabled = hasFinanceSelected;

                if (viewFinanceButton != null)
                    viewFinanceButton.Enabled = hasFinanceSelected;

                if (deleteFinanceButton != null)
                    deleteFinanceButton.Enabled = hasFinanceSelected;

                // Export button enabled when project has data
                if (exportButton != null)
                    exportButton.Enabled = hasProjectSelected &&
                                         projectFinances != null &&
                                         projectFinances.Count > 0;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Update button states error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle keyboard shortcuts
        /// </summary>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            try
            {
                // F5 - Refresh
                if (keyData == System.Windows.Forms.Keys.F5)
                {
                    RefreshButton_Click(null, System.EventArgs.Empty);
                    return true;
                }

                // Ctrl+N - Add new finance
                if (keyData == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N))
                {
                    if (addFinanceButton != null && addFinanceButton.Enabled)
                    {
                        AddFinanceButton_Click(null, System.EventArgs.Empty);
                        return true;
                    }
                }

                // Ctrl+E - Edit selected finance
                if (keyData == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E))
                {
                    if (editFinanceButton != null && editFinanceButton.Enabled)
                    {
                        EditFinanceButton_Click(null, System.EventArgs.Empty);
                        return true;
                    }
                }

                // Ctrl+D - View details
                if (keyData == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D))
                {
                    if (viewFinanceButton != null && viewFinanceButton.Enabled)
                    {
                        ViewFinanceButton_Click(null, System.EventArgs.Empty);
                        return true;
                    }
                }

                // Delete - Delete selected finance
                if (keyData == System.Windows.Forms.Keys.Delete)
                {
                    if (deleteFinanceButton != null && deleteFinanceButton.Enabled)
                    {
                        DeleteFinanceButton_Click(null, System.EventArgs.Empty);
                        return true;
                    }
                }

                // Ctrl+S - Export report
                if (keyData == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S))
                {
                    if (exportButton != null && exportButton.Enabled)
                    {
                        ExportButton_Click(null, System.EventArgs.Empty);
                        return true;
                    }
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
        /// Handle form resize to maintain layout
        /// </summary>
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            try
            {
                // Refresh layout when resize
                if (mainTableLayout != null && !mainTableLayout.IsDisposed)
                {
                    mainTableLayout.Invalidate();
                    mainTableLayout.Update();
                }

                // Update grid layout
                if (financeDataGridView != null && !financeDataGridView.IsDisposed)
                {
                    financeDataGridView.Invalidate();
                }

                // Update summary cards layout
                if (summaryPanel != null && !summaryPanel.IsDisposed)
                {
                    summaryPanel.Invalidate();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Resize error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle form activation to refresh data if needed
        /// </summary>
        protected override void OnActivated(System.EventArgs e)
        {
            base.OnActivated(e);

            try
            {
                // Check if we need to refresh project data
                if (projects == null || projects.Count == 0)
                {
                    LoadProjectsData();
                    SetupProjectComboBox();
                }

                // Update button states
                UpdateButtonStates();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Form activation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate form state before closing
        /// </summary>
        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            try
            {
                // Clean up resources if needed
                if (projectFinances != null)
                {
                    projectFinances.Clear();
                    projectFinances = null;
                }

                if (filteredFinances != null)
                {
                    filteredFinances.Clear();
                    filteredFinances = null;
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
        /// Show context help for the form
        /// </summary>
        private void ShowContextHelp()
        {
            try
            {
                string helpText = "HƯỚNG DẪN SỬ DỤNG - QUẢN LÝ TÀI CHÍNH DỰ ÁN\n\n" +
                                "📋 CHỨC NĂNG CHÍNH:\n" +
                                "• Theo dõi thu chi theo từng dự án\n" +
                                "• Quản lý ngân sách dự án\n" +
                                "• Xuất báo cáo tài chính dự án\n\n" +
                                "⌨️ PHÍM TẮT:\n" +
                                "• F5: Làm mới dữ liệu\n" +
                                "• Ctrl+N: Thêm giao dịch mới\n" +
                                "• Ctrl+E: Chỉnh sửa giao dịch\n" +
                                "• Ctrl+D: Xem chi tiết\n" +
                                "• Delete: Xóa giao dịch\n" +
                                "• Ctrl+S: Xuất báo cáo\n\n" +
                                "💡 LƯU Ý:\n" +
                                "• Chọn dự án trước khi thêm giao dịch\n" +
                                "• Giao dịch đã duyệt không thể chỉnh sửa\n" +
                                "• Theo dõi % sử dụng ngân sách trong thẻ tóm tắt";

                System.Windows.Forms.MessageBox.Show(
                    helpText,
                    "Hướng dẫn - Quản lý Tài chính Dự án",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Show help error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle F1 key for help
        /// </summary>
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
            {
                ShowContextHelp();
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Refresh UI elements based on current state
        /// </summary>
        public void RefreshUI()
        {
            try
            {
                // Update button states
                UpdateButtonStates();

                // Update statistics
                UpdateTotalLabel();

                // Refresh grid
                if (financeDataGridView != null && !financeDataGridView.IsDisposed)
                {
                    financeDataGridView.Invalidate();
                    financeDataGridView.Refresh();
                }

                // Refresh summary cards
                if (summaryPanel != null && !summaryPanel.IsDisposed)
                {
                    UpdateSummaryCards();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Refresh UI error: {ex.Message}");
            }
        }

        /// <summary>
        /// Set form to read-only mode (if needed for certain user roles)
        /// </summary>
        public void SetReadOnlyMode(bool readOnly)
        {
            try
            {
                if (addFinanceButton != null)
                    addFinanceButton.Enabled = !readOnly;

                if (editFinanceButton != null)
                    editFinanceButton.Enabled = !readOnly;

                if (deleteFinanceButton != null)
                    deleteFinanceButton.Enabled = !readOnly;

                // View and export remain enabled
                if (readOnly)
                {
                    this.Text += " (Chỉ xem)";
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Set read-only mode error: {ex.Message}");
            }
        }
    }
}