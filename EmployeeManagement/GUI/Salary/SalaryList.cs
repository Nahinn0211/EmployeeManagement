using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Salary
{
    public partial class SalaryListForm : Form
    {
        #region Fields
        private SalaryBLL salaryBLL;
        private List<Models.Entity.Salary> salaries;
        private List<Models.Entity.Salary> filteredSalaries;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên, mã nhân viên, phòng ban...";

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
        private ComboBox monthComboBox;
        private ComboBox yearComboBox;
        private ComboBox statusComboBox;
        private Button searchButton;
        private Button clearButton;

        // Grid controls
        private DataGridView salaryDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button paymentButton;
        private Button exportButton;
        private Button bulkCreateButton;
        private Label statisticsLabel;
        #endregion

        #region Constructor
        public SalaryListForm()
        {
            InitializeComponent();
            salaryBLL = new SalaryBLL();
            InitializeLayout();
            LoadSalariesFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadSalariesFromDatabase()
        {
            try
            {
                salaries = salaryBLL.GetAllSalaries();
                filteredSalaries = new List<Models.Entity.Salary>(salaries);
                LoadSalariesToGrid();
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
        private void LoadSalariesToGrid()
        {
            try
            {
                var dataSource = filteredSalaries.Select(s => new
                {
                    SalaryID = s.SalaryID,
                    EmployeeID = s.EmployeeID,
                    EmployeeCode = s.Employee?.EmployeeCode ?? "",
                    EmployeeName = s.Employee?.FullName ?? "",
                    MonthYear = s.MonthYearDisplay,
                    Month = s.Month,
                    Year = s.Year,
                    BaseSalary = s.BaseSalary,
                    BaseSalaryDisplay = $"{s.BaseSalary:#,##0} VNĐ",
                    Allowance = s.Allowance,
                    AllowanceDisplay = $"{s.Allowance:#,##0} VNĐ",
                    Bonus = s.Bonus,
                    BonusDisplay = $"{s.Bonus:#,##0} VNĐ",
                    Deduction = s.Deduction,
                    DeductionDisplay = $"{s.Deduction:#,##0} VNĐ",
                    NetSalary = s.NetSalary,
                    NetSalaryDisplay = s.NetSalaryDisplay,
                    PaymentDate = s.PaymentDate,
                    PaymentDateDisplay = s.PaymentDateDisplay,
                    PaymentStatus = s.PaymentStatus,
                    PaymentStatusDisplay = s.PaymentStatusDisplay,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt
                }).ToList();

                salaryDataGridView.DataSource = dataSource;
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
                int? monthFilter = monthComboBox.SelectedIndex == 0 ? null : monthComboBox.SelectedIndex;
                int? yearFilter = yearComboBox.SelectedIndex == 0 ? null : int.Parse(yearComboBox.Text);

                filteredSalaries = salaries.Where(s =>
                    (string.IsNullOrEmpty(searchText) ||
                     s.Employee?.FullName.ToLower().Contains(searchText) == true ||
                     s.Employee?.EmployeeCode.ToLower().Contains(searchText) == true) &&
                    (string.IsNullOrEmpty(statusFilter) || s.PaymentStatus == statusFilter) &&
                    (!monthFilter.HasValue || s.Month == monthFilter.Value) &&
                    (!yearFilter.HasValue || s.Year == yearFilter.Value)
                ).ToList();

                LoadSalariesToGrid();
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
            monthComboBox.SelectedIndex = 0;
            yearComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndex = 0;
            filteredSalaries = new List<Models.Entity.Salary>(salaries);
            LoadSalariesToGrid();
        }

        private void UpdateStatistics()
        {
            var stats = salaryBLL.GetSalaryStatistics();
            var filtered = filteredSalaries.Count;
            var filteredTotal = filteredSalaries.Sum(s => s.NetSalary);

            statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng: {stats.TotalRecords} | " +
                                  $"✅ Đã thanh toán: {stats.PaidRecords} | ⏳ Chưa thanh toán: {stats.UnpaidRecords} | " +
                                  $"💰 Tổng tiền: {filteredTotal:#,##0} VNĐ";
        }
        #endregion

        #region Helper Methods
        private string GetStatusValue(string statusDisplay)
        {
            return statusDisplay switch
            {
                "⏳ Chưa thanh toán" => "Chưa thanh toán",
                "✅ Đã thanh toán" => "Đã thanh toán",
                "🔄 Thanh toán một phần" => "Thanh toán một phần",
                "❌ Đã hủy" => "Đã hủy",
                _ => statusDisplay
            };
        }

        private Models.Entity.Salary GetSelectedSalary()
        {
            if (salaryDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = salaryDataGridView.SelectedRows[0];
                var salaryId = (int)selectedRow.Cells["SalaryID"].Value;
                return filteredSalaries.FirstOrDefault(s => s.SalaryID == salaryId);
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void SalaryDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = salaryDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "PaymentStatusDisplay" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    string s when s.Contains("Đã thanh toán") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Chưa thanh toán") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Thanh toán một phần") => Color.FromArgb(33, 150, 243),
                    string s when s.Contains("Đã hủy") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "NetSalaryDisplay" && e.Value != null)
            {
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                e.CellStyle.ForeColor = Color.FromArgb(34, 197, 94);
            }
        }

        private void AddSalary()
        {
            try
            {
                using (var form = new SalaryDetailForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadSalariesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Thêm bảng lương thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm bảng lương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSalary()
        {
            var salary = GetSelectedSalary();
            if (salary == null) return;

            try
            {
                using (var form = new SalaryDetailForm(salary))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadSalariesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật bảng lương thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa bảng lương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewSalary()
        {
            var salary = GetSelectedSalary();
            if (salary == null) return;

            try
            {
                using (var form = new SalaryDetailForm(salary, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết bảng lương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSalary()
        {
            var salary = GetSelectedSalary();
            if (salary == null) return;

            try
            {
                var canDelete = salaryBLL.CanDeleteSalary(salary.SalaryID);
                if (!canDelete.CanDelete)
                {
                    MessageBox.Show(canDelete.Reason, "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa bảng lương tháng {salary.Month}/{salary.Year} của nhân viên '{salary.Employee?.FullName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (salaryBLL.DeleteSalary(salary.SalaryID))
                    {
                        LoadSalariesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Xóa bảng lương thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa bảng lương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ManagePayment()
        {
            var selectedSalaries = GetSelectedSalaries();
            if (selectedSalaries.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một bảng lương!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var form = new SalaryPaymentForm(selectedSalaries))
                {
                    if (form.ShowDialog() == DialogResult.OK) 
                    {
                        LoadSalariesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật thanh toán thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quản lý thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BulkCreateSalaries()
        {
            try
            {
                using (var form = new BulkSalaryCreateForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadSalariesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Tạo bảng lương hàng loạt thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo bảng lương hàng loạt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSalaries()
        {
            try
            {
                // Implementation for export functionality
                MaterialSnackBar snackBar = new MaterialSnackBar("Chức năng xuất dữ liệu đang được phát triển", "OK", true);
                snackBar.Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Models.Entity.Salary> GetSelectedSalaries()
        {
            var selectedSalaries = new List<Models.Entity.Salary>();
            foreach (DataGridViewRow row in salaryDataGridView.SelectedRows)
            {
                var salaryId = (int)row.Cells["SalaryID"].Value;
                var salary = filteredSalaries.FirstOrDefault(s => s.SalaryID == salaryId);
                if (salary != null)
                    selectedSalaries.Add(salary);
            }
            return selectedSalaries;
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Lương";
            this.BackColor = Color.White;
            this.Size = new Size(1600, 900);
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
                Text = "💰 QUẢN LÝ LƯƠNG",
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
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Month filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Year filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Clear button

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

            // Month ComboBox
            monthComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            monthComboBox.Items.Add("Tất cả tháng");
            for (int i = 1; i <= 12; i++)
                monthComboBox.Items.Add($"Tháng {i}");
            monthComboBox.SelectedIndex = 0;
            monthComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Year ComboBox
            yearComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            yearComboBox.Items.Add("Tất cả năm");
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
                yearComboBox.Items.Add(year.ToString());
            yearComboBox.SelectedIndex = 0;
            yearComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] {
                "Tất cả trạng thái",
                "⏳ Chưa thanh toán",
                "✅ Đã thanh toán",
                "🔄 Thanh toán một phần",
                "❌ Đã hủy"
            });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(monthComboBox, 1, 0);
            searchContainer.Controls.Add(yearComboBox, 2, 0);
            searchContainer.Controls.Add(statusComboBox, 3, 0);
            searchContainer.Controls.Add(searchButton, 4, 0);
            searchContainer.Controls.Add(clearButton, 5, 0);

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

            salaryDataGridView = new DataGridView
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
                MultiSelect = true,
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

            gridPanel.Controls.Add(salaryDataGridView);
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

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            paymentButton = CreateActionButton("💳 THANH TOÁN", Color.FromArgb(156, 39, 176));
            bulkCreateButton = CreateActionButton("📋 TẠO HÀNG LOẠT", Color.FromArgb(255, 87, 34));
            exportButton = CreateActionButton("📊 XUẤT EXCEL", Color.FromArgb(76, 175, 80));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;
            paymentButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(paymentButton);
            buttonsPanel.Controls.Add(bulkCreateButton);
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
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
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
                Size = new Size(120, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
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
            salaryDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 6, 8, 6),
                Font = new Font("Segoe UI", 9)
            };

            salaryDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(8, 8, 8, 8),
                WrapMode = DataGridViewTriState.False
            };

            salaryDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            salaryDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "SalaryID", HeaderText = "ID", Width = 70, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "EmployeeCode", HeaderText = "Mã NV", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "EmployeeName", HeaderText = "Tên nhân viên", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "MonthYear", HeaderText = "Tháng/Năm", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "BaseSalaryDisplay", HeaderText = "Lương cơ bản", Width = 120, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "AllowanceDisplay", HeaderText = "Phụ cấp", Width = 100, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "BonusDisplay", HeaderText = "Thưởng", Width = 100, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "DeductionDisplay", HeaderText = "Khấu trừ", Width = 100, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "NetSalaryDisplay", HeaderText = "Lương thực nhận", Width = 130, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "PaymentStatusDisplay", HeaderText = "Trạng thái", Width = 140, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "PaymentDateDisplay", HeaderText = "Ngày thanh toán", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Notes", HeaderText = "Ghi chú", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true }
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
                    MinimumWidth = 60,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                salaryDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            salaryDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = salaryDataGridView.SelectedRows.Count > 0;
                var selectedSalary = GetSelectedSalary();

                editButton.Enabled = hasSelection && selectedSalary?.CanEdit() == true;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection && selectedSalary?.CanDelete() == true;
                paymentButton.Enabled = hasSelection;
            };

            salaryDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewSalary();
            };

            salaryDataGridView.CellFormatting += SalaryDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddSalary();
            editButton.Click += (s, e) => EditSalary();
            viewButton.Click += (s, e) => ViewSalary();
            deleteButton.Click += (s, e) => DeleteSalary();
            paymentButton.Click += (s, e) => ManagePayment();
            bulkCreateButton.Click += (s, e) => BulkCreateSalaries();
            exportButton.Click += (s, e) => ExportSalaries();
        }
        #endregion
    }
}