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

            // Giảm heights để giao diện gọn gàng hơn
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));   // Header - giảm từ 80
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Search - giảm từ 100  
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));   // Footer - giảm từ 80

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 5, 10, 5) // Giảm padding
            };

            titleLabel = new Label
            {
                Text = "🏢 QUẢN LÝ KHÁCH HÀNG",
                Font = new Font("Segoe UI", 20, FontStyle.Bold), // Giảm từ 24 xuống 20
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
                Padding = new Padding(15, 12, 15, 12)
            };

            // Main container - chia 70% cho filters, 30% cho buttons
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0)
            };

            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Filter controls  
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Buttons

            // === FILTER CONTROLS CONTAINER ===
            var filtersContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65)); // Search box
            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); // Status

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 32,
                Margin = new Padding(0, 6, 12, 6),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 32,
                Margin = new Padding(6, 6, 12, 6),
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "🤝 Đang hợp tác", "⏸️ Tạm dừng", "🚫 Ngừng hợp tác" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Add filters to container
            filtersContainer.Controls.Add(searchTextBox, 0, 0);
            filtersContainer.Controls.Add(statusComboBox, 1, 0);

            // === BUTTONS CONTAINER ===
            var buttonsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 0, 0, 0),
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = false
            };

            // Search Button
            searchButton = new Button
            {
                Text = "🔍 TÌM KIẾM",
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 136, 229);
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = new Button
            {
                Text = "🗑️ XÓA BỘ LỌC",
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 6, 0, 6),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 57, 53);
            clearButton.Click += ClearFilters;

            buttonsContainer.Controls.Add(searchButton);
            buttonsContainer.Controls.Add(clearButton);

            // Add to main container
            mainContainer.Controls.Add(filtersContainer, 0, 0);
            mainContainer.Controls.Add(buttonsContainer, 1, 0);

            searchPanel.Controls.Add(mainContainer);
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

        // ==========================================
        // FOOTER ĐÃ LOẠI BỎ STATISTICS LABEL
        // ==========================================
        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 12, 20, 12)
            };

            // Chỉ có buttons panel, không có statistics
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = false
            };

            addButton = CreateCompactActionButton("➕ THÊM KHÁCH HÀNG", Color.FromArgb(76, 175, 80));
            editButton = CreateCompactActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateCompactActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(33, 150, 243));
            deleteButton = CreateCompactActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            exportButton = CreateCompactActionButton("📊 XUẤT EXCEL", Color.FromArgb(76, 175, 80));

            // Set sizes
            addButton.Size = new Size(150, 38);
            editButton.Size = new Size(120, 38);
            viewButton.Size = new Size(130, 38);
            deleteButton.Size = new Size(80, 38);
            exportButton.Size = new Size(120, 38);

            // Set initial states
            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(exportButton);

            footerPanel.Controls.Add(buttonsPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        // RESPONSIVE BUTTONS - Thay đổi text theo screen size
        private void AdjustButtonsForScreenSize()
        {
            if (this.Width < 1000)
            {
                // Screen nhỏ - text ngắn
                addButton.Text = "➕ THÊM";
                editButton.Text = "✏️ SỬA";
                viewButton.Text = "👁️ XEM";
                deleteButton.Text = "🗑️ XÓA";
                exportButton.Text = "📊 XUẤT";

                addButton.Size = new Size(80, 38);
                editButton.Size = new Size(70, 38);
                viewButton.Size = new Size(70, 38);
                deleteButton.Size = new Size(70, 38);
                exportButton.Size = new Size(80, 38);

                searchButton.Text = "TÌM";
                clearButton.Text = "XÓA";
                searchButton.Size = new Size(60, 32);
                clearButton.Size = new Size(60, 32);
            }
            else if (this.Width < 1300)
            {
                // Screen vừa - text trung bình
                addButton.Text = "➕ THÊM MỚI";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";
                exportButton.Text = "📊 XUẤT EXCEL";

                addButton.Size = new Size(110, 38);
                editButton.Size = new Size(110, 38);
                viewButton.Size = new Size(120, 38);
                deleteButton.Size = new Size(80, 38);
                exportButton.Size = new Size(110, 38);

                searchButton.Text = "🔍 TÌM KIẾM";
                clearButton.Text = "🗑️ XÓA BỘ LỌC";
                searchButton.Size = new Size(100, 32);
                clearButton.Size = new Size(110, 32);
            }
            else
            {
                // Screen lớn - text đầy đủ
                addButton.Text = "➕ THÊM KHÁCH HÀNG";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";
                exportButton.Text = "📊 XUẤT EXCEL";

                addButton.Size = new Size(150, 38);
                editButton.Size = new Size(120, 38);
                viewButton.Size = new Size(130, 38);
                deleteButton.Size = new Size(80, 38);
                exportButton.Size = new Size(120, 38);

                searchButton.Text = "🔍 TÌM KIẾM";
                clearButton.Text = "🗑️ XÓA BỘ LỌC";
                searchButton.Size = new Size(110, 32);
                clearButton.Size = new Size(120, 32);
            }
        }

        private Button CreateCompactActionButton(string text, Color backColor)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(120, 38),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            button.FlatAppearance.BorderSize = 0;

            // Hover effects
            Color hoverColor = ControlPaint.Dark(backColor, 0.1f);
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState != FormWindowState.Minimized)
            {
                AdjustButtonsForScreenSize();
            }
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
                searchTextBox.BackColor = Color.FromArgb(250, 250, 250);
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
                searchTextBox.BackColor = Color.White;
            };

            searchTextBox.TextChanged += (s, e) =>
            {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };

            searchTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyFilters();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
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

    public class CustomerDisplayModel
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactTitle { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ProjectCount { get; set; }
    }
}