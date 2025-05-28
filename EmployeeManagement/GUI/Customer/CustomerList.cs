using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models; // Sửa namespace
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Customer
{
    public partial class CustomerListForm : Form
    {
        #region Fields
        private CustomerBLL customerBLL;
        private List<Models.Entity.Customer> customers; // Sửa namespace
        private List<Models.Entity.Customer> filteredCustomers; // Sửa namespace
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên công ty, mã khách hàng, người liên hệ...";

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;

        // Search controls
        private TextBox searchTextBox;
        private ComboBox statusComboBox;
        private Button searchButton;
        private Button clearButton;

        // Grid controls
        private DataGridView customerDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button exportButton;
        private Label statisticsLabel;
        #endregion

        #region Constructor
        public CustomerListForm()
        {
            InitializeComponent();
            customerBLL = new CustomerBLL();
            InitializeLayout();
            LoadCustomersFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadCustomersFromDatabase()
        {
            try
            {
                customers = customerBLL.GetAllCustomers();
                filteredCustomers = new List<Models.Entity.Customer>(customers); // Sửa namespace
                LoadCustomersToGrid();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Data Management
        private void LoadCustomersToGrid()
        {
            try
            {
                var dataSource = filteredCustomers.Select(c => new CustomerDisplayModel
                {
                    CustomerID = c.CustomerID,
                    CustomerCode = c.CustomerCode,
                    CompanyName = c.CompanyName,
                    ContactName = c.ContactName,
                    ContactTitle = c.ContactTitle,
                    Phone = c.Phone,
                    Email = c.Email,
                    Status = c.StatusDisplay,
                    CreatedAt = c.CreatedAt,
                    ProjectCount = 0 // This would be loaded from projects if needed
                }).ToList();

                customerDataGridView.DataSource = dataSource;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = searchTextBox.Text == searchPlaceholder ? "" : searchTextBox.Text.ToLower();
                string statusFilter = statusComboBox.SelectedIndex == 0 ? "" : GetStatusValue(statusComboBox.Text);

                filteredCustomers = customers.Where(c =>
                    (string.IsNullOrEmpty(searchText) ||
                     (c.CompanyName?.ToLower().Contains(searchText) ?? false) ||
                     (c.CustomerCode?.ToLower().Contains(searchText) ?? false) ||
                     (c.ContactName?.ToLower().Contains(searchText) ?? false) ||
                     (c.Email?.ToLower().Contains(searchText) ?? false) ||
                     (c.Phone?.ToLower().Contains(searchText) ?? false)) &&
                    (string.IsNullOrEmpty(statusFilter) || c.Status == statusFilter)
                ).ToList();

                LoadCustomersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchTextBox.Text = searchPlaceholder;
            searchTextBox.ForeColor = Color.Gray;
            statusComboBox.SelectedIndex = 0;
            filteredCustomers = new List<Models.Entity.Customer>(customers); // Sửa namespace
            LoadCustomersToGrid();
        }

        private void UpdateStatistics()
        {
            try
            {
                var stats = customerBLL.GetCustomerStatistics();
                var filtered = filteredCustomers.Count;

                statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng: {stats.Total} | 🤝 Đang hợp tác: {stats.Active} | ⏸️ Tạm dừng: {stats.Paused} | 🚫 Ngừng hợp tác: {stats.Inactive}";
            }
            catch (Exception ex)
            {
                statisticsLabel.Text = "📊 Lỗi khi tải thống kê";
            }
        }
        #endregion

        #region Helper Methods
        private string GetStatusValue(string statusDisplay)
        {
            return statusDisplay switch
            {
                "🤝 Đang hợp tác" => "Đang hợp tác",
                "⏸️ Tạm dừng" => "Tạm dừng",
                "🚫 Ngừng hợp tác" => "Ngừng hợp tác",
                _ => statusDisplay
            };
        }

        private Models.Entity.Customer GetSelectedCustomer() // Sửa namespace
        {
            if (customerDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = customerDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is CustomerDisplayModel displayModel)
                {
                    return customers.FirstOrDefault(c => c.CustomerID == displayModel.CustomerID);
                }
            }
            return null;
        }
        // Cập nhật các method trong CustomerListForm.cs

        private void AddCustomer()
        {
            try
            {
                using (var form = new CustomerDetailForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadCustomersFromDatabase();
                        MessageBox.Show("Thêm khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditCustomer()
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để chỉnh sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var form = new CustomerDetailForm(customer))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadCustomersFromDatabase();
                        MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewCustomer()
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xem!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var form = new CustomerDetailForm(customer, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void CustomerDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = customerDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    string s when s.Contains("Đang hợp tác") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Tạm dừng") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Ngừng hợp tác") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

     
      
    
        private void DeleteCustomer()
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var canDelete = customerBLL.CanDeleteCustomer(customer.CustomerID);
                if (!canDelete.CanDelete)
                {
                    MessageBox.Show(canDelete.Reason, "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa khách hàng '{customer.CompanyName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (customerBLL.DeleteCustomer(customer.CustomerID))
                    {
                        LoadCustomersFromDatabase();
                        MessageBox.Show("Xóa khách hàng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCustomers()
        {
            try
            {
                MessageBox.Show("Chức năng xuất dữ liệu đang được phát triển", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Khách hàng";
            this.BackColor = Color.White;
            this.Size = new Size(1400, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupSearchPanel();
            SetupDataGrid();
            SetupFooter();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            };

            titleLabel = new Label
            {
                Text = "🏢 QUẢN LÝ KHÁCH HÀNG",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 10, 20, 10)
            };

            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Clear button

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 35,
                Margin = new Padding(0, 5, 10, 5)
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "🤝 Đang hợp tác", "⏸️ Tạm dừng", "🚫 Ngừng hợp tác" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(searchButton, 2, 0);
            searchContainer.Controls.Add(clearButton, 3, 0);

            searchPanel.Controls.Add(searchContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
        }

        private void SetupDataGrid()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            customerDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                AllowUserToResizeColumns = true,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 40 },
                ScrollBars = ScrollBars.Both,
                AutoGenerateColumns = false
            };

            SetupDataGridStyles();
            SetupDataGridColumns();
            SetupDataGridEvents();

            gridPanel.Controls.Add(customerDataGridView);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var footerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM KHÁCH HÀNG", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            exportButton = CreateActionButton("📊 XUẤT EXCEL", Color.FromArgb(76, 175, 80));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(exportButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "📊 Đang tải..."
            };

            statsPanel.Controls.Add(statisticsLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 5),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(140, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 15, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void SetupSearchTextBoxEvents()
        {
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) =>
            {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };
        }

        private void SetupDataGridStyles()
        {
            customerDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            customerDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(10, 10, 10, 10),
                WrapMode = DataGridViewTriState.False
            };

            customerDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            customerDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "CustomerID", HeaderText = "ID", Width = 70, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "CustomerCode", HeaderText = "Mã khách hàng", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "CompanyName", HeaderText = "Tên công ty", Width = 250, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "ContactName", HeaderText = "Người liên hệ", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "ContactTitle", HeaderText = "Chức vụ", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Phone", HeaderText = "Điện thoại", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Email", HeaderText = "Email", Width = 180, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 140, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "CreatedAt", HeaderText = "Ngày tạo", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 80,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                if (col.Name == "CreatedAt")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                customerDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            customerDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = customerDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            customerDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewCustomer();
            };

            customerDataGridView.CellFormatting += CustomerDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddCustomer();
            editButton.Click += (s, e) => EditCustomer();
            viewButton.Click += (s, e) => ViewCustomer();
            deleteButton.Click += (s, e) => DeleteCustomer();
            exportButton.Click += (s, e) => ExportCustomers();
        }
        #endregion
    }
}