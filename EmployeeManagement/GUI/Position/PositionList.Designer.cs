using System.Windows.Forms;

namespace EmployeeManagement.GUI.Position
{
    partial class PositionListForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PositionListForm));

            // Main components
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();

            // Left side (Form section)
            this.formPanel = new System.Windows.Forms.Panel();

            // Right side (List section)
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.filterContainer = new System.Windows.Forms.Panel();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.positionDataGridView = new System.Windows.Forms.DataGridView();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.actionPanel = new System.Windows.Forms.Panel();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.statisticsPanel = new System.Windows.Forms.Panel();

            // Form settings
            this.SuspendLayout();

            // SplitContainer setup
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();

            // Main SplitContainer config
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";

            // Left panel (Form section)
            this.mainSplitContainer.Panel1.Controls.Add(this.formPanel);
            this.mainSplitContainer.Panel1MinSize = 300;
            this.mainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(10);
            this.mainSplitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);

            // Right panel (List section)
            this.mainSplitContainer.Panel2.Controls.Add(this.mainTableLayout);
            this.mainSplitContainer.Panel2MinSize = 500;
            this.mainSplitContainer.Size = new System.Drawing.Size(1000, 600);
            this.mainSplitContainer.SplitterDistance = 300;
            this.mainSplitContainer.SplitterWidth = 5;
            this.mainSplitContainer.TabIndex = 0;

            // Form Panel
            this.formPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formPanel.BackColor = System.Drawing.Color.White;
            this.formPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formPanel.Padding = new System.Windows.Forms.Padding(10);

            // Main Table Layout (Right panel)
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.RowCount = 4;
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.BackColor = System.Drawing.Color.White;
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(10);

            // Define row heights
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));  // Header
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));  // Search
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));  // Grid
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));  // Footer

            // Header Panel
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.BackColor = System.Drawing.Color.White;

            // Title Label
            this.titleLabel.AutoSize = false;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(63, 81, 181); // Indigo color
            this.titleLabel.Text = "🏆 QUẢN LÝ CHỨC VỤ";
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.headerPanel.Controls.Add(this.titleLabel);
            this.mainTableLayout.Controls.Add(this.headerPanel, 0, 0);

            // Search Panel
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPanel.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.searchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);

            // Search controls layout
            var searchLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                BackColor = System.Drawing.Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Search controls row/column styles
            searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));  // Search box
            searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));  // Search button
            searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));  // Clear button
            searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));  // Spacer

            searchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));  // Search controls
            searchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));  // Filter controls

            // Search TextBox
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchTextBox.ForeColor = System.Drawing.Color.Gray;
            this.searchTextBox.Text = "🔍 Tìm kiếm chức vụ...";
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 5, 2);

            // Search Button
            this.searchButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.searchButton.BackColor = System.Drawing.Color.FromArgb(33, 150, 243); // Material Blue
            this.searchButton.ForeColor = System.Drawing.Color.White;
            this.searchButton.Text = "🔍 Tìm kiếm";
            this.searchButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.searchButton.FlatAppearance.BorderSize = 0;

            // Clear Button
            this.clearButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(244, 67, 54); // Material Red
            this.clearButton.ForeColor = System.Drawing.Color.White;
            this.clearButton.Text = "🗑️ Xóa bộ lọc";
            this.clearButton.Margin = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.clearButton.FlatAppearance.BorderSize = 0;

            // Filter Container
            this.filterContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterContainer.BackColor = System.Drawing.Color.Transparent;

            // Add search controls to layout
            searchLayout.Controls.Add(this.searchTextBox, 0, 0);
            searchLayout.Controls.Add(this.searchButton, 1, 0);
            searchLayout.Controls.Add(this.clearButton, 2, 0);
            searchLayout.Controls.Add(this.filterContainer, 0, 1);
            searchLayout.SetColumnSpan(this.filterContainer, 4);

            // Add layout to search panel
            this.searchPanel.Controls.Add(searchLayout);
            this.mainTableLayout.Controls.Add(this.searchPanel, 0, 1);

            // Grid Panel
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.BackColor = System.Drawing.Color.White;
            this.gridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridPanel.Padding = new System.Windows.Forms.Padding(2);

            // Position DataGridView
            this.positionDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.positionDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.positionDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.positionDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.positionDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.positionDataGridView.EnableHeadersVisualStyles = false;
            this.positionDataGridView.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);
            this.positionDataGridView.ReadOnly = true;
            this.positionDataGridView.RowHeadersVisible = false;
            this.positionDataGridView.RowTemplate.Height = 35;
            this.positionDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.positionDataGridView.ShowEditingIcon = false;
            this.positionDataGridView.AllowUserToAddRows = false;
            this.positionDataGridView.AllowUserToDeleteRows = false;
            this.positionDataGridView.AllowUserToResizeRows = false;
            this.positionDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // DataGridView Styles
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            headerStyle.BackColor = System.Drawing.Color.FromArgb(63, 81, 181); // Indigo
            headerStyle.ForeColor = System.Drawing.Color.White;
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(63, 81, 181);
            headerStyle.SelectionForeColor = System.Drawing.Color.White;
            headerStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            headerStyle.Padding = new System.Windows.Forms.Padding(5);
            this.positionDataGridView.ColumnHeadersDefaultCellStyle = headerStyle;

            System.Windows.Forms.DataGridViewCellStyle rowStyle = new System.Windows.Forms.DataGridViewCellStyle();
            rowStyle.BackColor = System.Drawing.Color.White;
            rowStyle.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            rowStyle.SelectionBackColor = System.Drawing.Color.FromArgb(63, 81, 181, 50); // Indigo with alpha
            rowStyle.SelectionForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            rowStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rowStyle.Padding = new System.Windows.Forms.Padding(5);
            this.positionDataGridView.DefaultCellStyle = rowStyle;

            System.Windows.Forms.DataGridViewCellStyle altRowStyle = new System.Windows.Forms.DataGridViewCellStyle();
            altRowStyle.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            altRowStyle.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            altRowStyle.SelectionBackColor = System.Drawing.Color.FromArgb(63, 81, 181, 50); // Indigo with alpha
            altRowStyle.SelectionForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.positionDataGridView.AlternatingRowsDefaultCellStyle = altRowStyle;

            this.gridPanel.Controls.Add(this.positionDataGridView);
            this.mainTableLayout.Controls.Add(this.gridPanel, 0, 2);

            // Footer Panel
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.footerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Footer Layout
            var footerLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = System.Drawing.Color.Transparent
            };

            footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));  // Action buttons
            footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));  // Statistics

            // Action Panel
            this.actionPanel = new System.Windows.Forms.Panel();
            this.actionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionPanel.BackColor = System.Drawing.Color.Transparent;
            this.actionPanel.Padding = new System.Windows.Forms.Padding(5);

            // Action Buttons
            this.editButton = new System.Windows.Forms.Button();
            this.editButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.editButton.BackColor = System.Drawing.Color.FromArgb(255, 152, 0); // Orange
            this.editButton.ForeColor = System.Drawing.Color.White;
            this.editButton.Text = "✏️ Sửa";
            this.editButton.Size = new System.Drawing.Size(100, 35);
            this.editButton.Location = new System.Drawing.Point(10, 5);
            this.editButton.FlatAppearance.BorderSize = 0;
            this.editButton.Enabled = false;

            this.deleteButton = new System.Windows.Forms.Button();
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.deleteButton.BackColor = System.Drawing.Color.FromArgb(244, 67, 54); // Red
            this.deleteButton.ForeColor = System.Drawing.Color.White;
            this.deleteButton.Text = "🗑️ Xóa";
            this.deleteButton.Size = new System.Drawing.Size(100, 35);
            this.deleteButton.Location = new System.Drawing.Point(120, 5);
            this.deleteButton.FlatAppearance.BorderSize = 0;
            this.deleteButton.Enabled = false;

            this.actionPanel.Controls.Add(this.deleteButton);
            this.actionPanel.Controls.Add(this.editButton);

            // Statistics Panel
            this.statisticsPanel = new System.Windows.Forms.Panel();
            this.statisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statisticsPanel.BackColor = System.Drawing.Color.Transparent;
            this.statisticsPanel.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);

            // Add panels to footer layout
            footerLayout.Controls.Add(this.actionPanel, 0, 0);
            footerLayout.Controls.Add(this.statisticsPanel, 1, 0);

            // Add layout to footer panel
            this.footerPanel.Controls.Add(footerLayout);
            this.mainTableLayout.Controls.Add(this.footerPanel, 0, 3);

            // Form Configuration
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.mainSplitContainer);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "PositionListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý Chức vụ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.positionDataGridView)).EndInit();

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.Panel formPanel;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Panel filterContainer;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.DataGridView positionDataGridView;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Panel actionPanel;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Panel statisticsPanel;
    }
}