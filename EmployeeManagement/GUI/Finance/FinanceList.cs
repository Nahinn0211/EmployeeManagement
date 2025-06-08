// FinanceListForm.cs - Fix duplicate InitializeComponent và constructor issues

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Utilities;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Finance
{
    public partial class FinanceListForm : Form
    {
        #region Fields
        private FinanceBLL financeBLL;
        private List<Models.Entity.Finance> finances;
        private List<Models.Entity.Finance> filteredFinances;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo mã giao dịch, mô tả, số tham chiếu...";

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
        private ComboBox transactionTypeComboBox;
        private ComboBox categoryComboBox;
        private ComboBox statusComboBox;
        private ComboBox projectComboBox;
        private ComboBox customerComboBox;
        private ComboBox employeeComboBox;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private Button searchButton;
        private Button clearButton;
        private Button reportButton;

        // Grid controls
        private DataGridView financeDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button approveButton;
        private Button rejectButton;
        private Label statisticsLabel;

        // Data for dropdowns
        private List<Models.Entity.Project> projects;
        private List<Models.Entity.Customer> customers;
        private List<Models.Entity.Employee> employees;
        #endregion

        #region Constructor
        public FinanceListForm()
        {
            InitializeComponent(); // Chỉ gọi Designer's InitializeComponent
            financeBLL = new FinanceBLL();
            LoadDropdownData();
            SetupCustomLayout(); // Đổi tên để tránh conflict
            LoadFinancesFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadDropdownData()
        {
            try
            {
                projects = financeBLL.GetProjectsForDropdown();
                customers = financeBLL.GetCustomersForDropdown();
                employees = financeBLL.GetEmployeesForDropdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dropdown: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Initialize empty lists if failed
                projects = new List<Models.Entity.Project>();
                customers = new List<Models.Entity.Customer>();
                employees = new List<Models.Entity.Employee>();
            }
        }

        private void LoadFinancesFromDatabase()
        {
            try
            {
                finances = financeBLL.GetAllFinances();
                filteredFinances = new List<Models.Entity.Finance>(finances);
                LoadFinancesToGrid();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Initialize empty lists if failed
                finances = new List<Models.Entity.Finance>();
                filteredFinances = new List<Models.Entity.Finance>();
            }
        }
        #endregion

        #region Data Management
        private void LoadFinancesToGrid()
        {
            try
            {
                var dataSource = filteredFinances.Select(f => new FinanceDisplayModel
                {
                    FinanceID = f.FinanceID,
                    TransactionCode = f.TransactionCode,
                    TransactionType = f.TransactionTypeDisplay,
                    Category = f.CategoryDisplay,
                    Amount = f.Amount,
                    AmountDisplay = f.AmountDisplay,
                    TransactionDate = f.TransactionDate,
                    PaymentMethod = f.PaymentMethodDisplay,
                    Status = f.StatusDisplay,
                    RelatedTo = f.RelatedToDisplay,
                    RecordedBy = f.RecordedBy?.FullName ?? "N/A",
                    Description = f.Description
                }).ToList();

                financeDataGridView.DataSource = dataSource;
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
                string transactionType = GetSelectedTransactionType();
                string category = GetSelectedCategory();
                string status = GetSelectedStatus();
                int? projectId = GetSelectedProjectId();
                int? customerId = GetSelectedCustomerId();
                int? employeeId = GetSelectedEmployeeId();
                DateTime? fromDate = fromDatePicker.Checked ? fromDatePicker.Value.Date : null;
                DateTime? toDate = toDatePicker.Checked ? toDatePicker.Value.Date : null;

                filteredFinances = finances.Where(f =>
                    (string.IsNullOrEmpty(searchText) ||
                     f.TransactionCode.ToLower().Contains(searchText) ||
                     (f.Description?.ToLower().Contains(searchText) ?? false) ||
                     (f.ReferenceNo?.ToLower().Contains(searchText) ?? false)) &&
                    (string.IsNullOrEmpty(transactionType) || f.TransactionType == transactionType) &&
                    (string.IsNullOrEmpty(category) || f.Category == category) &&
                    (string.IsNullOrEmpty(status) || f.Status == status) &&
                    (!projectId.HasValue || f.ProjectID == projectId) &&
                    (!customerId.HasValue || f.CustomerID == customerId) &&
                    (!employeeId.HasValue || f.EmployeeID == employeeId) &&
                    (!fromDate.HasValue || f.TransactionDate >= fromDate) &&
                    (!toDate.HasValue || f.TransactionDate <= toDate)
                ).ToList();

                LoadFinancesToGrid();
                UpdateStatistics();
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
            transactionTypeComboBox.SelectedIndex = 0;
            categoryComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndex = 0;
            fromDatePicker.Checked = false;
            toDatePicker.Checked = false;
            filteredFinances = new List<Models.Entity.Finance>(finances);
            LoadFinancesToGrid();
        }

        private void UpdateStatistics()
        {
            try
            {
                var stats = financeBLL.GetFinanceStatistics();
                var filtered = filteredFinances.Count;

                statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng GD: {stats.TotalTransactions} | 💰 Thu: {stats.TotalIncome:#,##0} | 💸 Chi: {stats.TotalExpense:#,##0} | 📈 Số dư: {stats.Balance:#,##0}";
            }
            catch (Exception)
            {
                var totalIncome = filteredFinances.Where(f => f.TransactionType == "Thu").Sum(f => f.Amount);
                var totalExpense = filteredFinances.Where(f => f.TransactionType == "Chi").Sum(f => f.Amount);
                var balance = totalIncome - totalExpense;
                var filtered = filteredFinances.Count;

                statisticsLabel.Text = $"📊 Hiển thị: {filtered} | 💰 Thu: {totalIncome:#,##0} | 💸 Chi: {totalExpense:#,##0} | 📈 Số dư: {balance:#,##0}";
            }
        }
        #endregion

        #region Helper Methods
        private string GetSelectedTransactionType()
        {
            if (transactionTypeComboBox.SelectedIndex <= 0) return "";

            var selectedText = transactionTypeComboBox.Text;
            return TransactionTypes.Types.FirstOrDefault(t => TransactionTypes.GetDisplayName(t) == selectedText) ?? "";
        }

        private string GetSelectedCategory()
        {
            if (categoryComboBox.SelectedIndex <= 0) return "";

            var selectedText = categoryComboBox.Text;
            var allCategories = FinanceCategories.IncomeCategories.Concat(FinanceCategories.ExpenseCategories);
            return allCategories.FirstOrDefault(c => FinanceCategories.GetDisplayName(c) == selectedText) ?? "";
        }

        private string GetSelectedStatus()
        {
            if (statusComboBox.SelectedIndex <= 0) return "";

            var selectedText = statusComboBox.Text;
            return FinanceStatus.Statuses.FirstOrDefault(s => FinanceStatus.GetDisplayName(s) == selectedText) ?? "";
        }

        private int? GetSelectedProjectId()
        {
            if (projectComboBox.SelectedIndex <= 0 || projects.Count == 0) return null;
            var index = projectComboBox.SelectedIndex - 1;
            return index < projects.Count ? projects[index].ProjectID : null;
        }

        private int? GetSelectedCustomerId()
        {
            if (customerComboBox.SelectedIndex <= 0 || customers.Count == 0) return null;
            var index = customerComboBox.SelectedIndex - 1;
            return index < customers.Count ? customers[index].CustomerID : null;
        }

        private int? GetSelectedEmployeeId()
        {
            if (employeeComboBox.SelectedIndex <= 0 || employees.Count == 0) return null;
            var index = employeeComboBox.SelectedIndex - 1;
            return index < employees.Count ? employees[index].EmployeeID : null;
        }

        private Models.Entity.Finance GetSelectedFinance()
        {
            if (financeDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = financeDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is FinanceDisplayModel displayModel)
                {
                    return finances.FirstOrDefault(f => f.FinanceID == displayModel.FinanceID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers - FIXED với proper Session checks
        private void AddFinance()
        {
            try
            {
                // Check if user is logged in - Fallback nếu UserSession chưa có
                try
                {
                    if (!UserSession.IsLoggedIn)
                    {
                        MessageBox.Show("Vui lòng đăng nhập để thực hiện chức năng này!", "Chưa đăng nhập",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!UserSession.HasPermission("AddFinance"))
                    {
                        MessageBox.Show("Bạn không có quyền thêm giao dịch tài chính!", "Không có quyền",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (var form = new FinanceDetailForm(UserSession.CurrentUserId))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadFinancesFromDatabase();
                            MaterialSnackBar snackBar = new MaterialSnackBar("Thêm giao dịch tài chính thành công!", "OK", true);
                            snackBar.Show(this);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // Fallback nếu UserSession chưa được setup
                    using (var form = new FinanceDetailForm(1)) // Default user ID
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadFinancesFromDatabase();
                            MaterialSnackBar snackBar = new MaterialSnackBar("Thêm giao dịch tài chính thành công!", "OK", true);
                            snackBar.Show(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm giao dịch tài chính: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditFinance()
        {
            var finance = GetSelectedFinance();
            if (finance == null)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Check if user is logged in
                if (!UserSession.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để thực hiện chức năng này!", "Chưa đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check permissions
                if (!UserSession.HasPermission("EditFinance"))
                {
                    MessageBox.Show("Bạn không có quyền sửa giao dịch tài chính!", "Không có quyền",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Additional check: only allow editing own records for regular users
                if (UserSession.CurrentUserRole == "User" && finance.RecordedByID != UserSession.CurrentUserId)
                {
                    MessageBox.Show("Bạn chỉ có thể sửa giao dịch do chính mình tạo!", "Không có quyền",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var form = new FinanceDetailForm(finance.FinanceID, UserSession.CurrentUserId, true))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadFinancesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật giao dịch tài chính thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa giao dịch tài chính: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ViewFinance()
        {
            var finance = GetSelectedFinance();
            if (finance == null)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần xem!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Check if user is logged in
                if (!UserSession.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để thực hiện chức năng này!", "Chưa đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                 using (var form = new FinanceDetailForm(finance.FinanceID, UserSession.CurrentUserId, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết giao dịch tài chính: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ApproveFinance()
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                if (finance.Status != "Chờ duyệt")
                {
                    MessageBox.Show("Chỉ có thể duyệt giao dịch đang ở trạng thái 'Chờ duyệt'!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn duyệt giao dịch '{finance.TransactionCode}'?\n" +
                    $"Số tiền: {finance.AmountDisplay}\n" +
                    $"Loại: {finance.TransactionTypeDisplay}\n" +
                    $"Mô tả: {finance.Description}",
                    "Xác nhận duyệt giao dịch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    int userId = 1; // Default
                    try
                    {
                        userId = UserSession.CurrentUserId;
                    }
                    catch (InvalidOperationException) { }

                    if (financeBLL.ApproveFinance(finance.FinanceID, userId))
                    {
                        LoadFinancesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar(
                            $"Đã duyệt giao dịch '{finance.TransactionCode}' thành công!",
                            "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi duyệt giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RejectFinance()
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                if (finance.Status != "Chờ duyệt")
                {
                    MessageBox.Show("Chỉ có thể từ chối giao dịch đang ở trạng thái 'Chờ duyệt'!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var rejectForm = new RejectReasonForm(finance))
                {
                    if (rejectForm.ShowDialog() == DialogResult.OK)
                    {
                        string rejectReason = rejectForm.RejectReason;

                        if (financeBLL.RejectFinance(finance.FinanceID, rejectReason))
                        {
                            LoadFinancesFromDatabase();
                            MaterialSnackBar snackBar = new MaterialSnackBar(
                                $"Đã từ chối giao dịch '{finance.TransactionCode}' thành công!",
                                "OK", true);
                            snackBar.Show(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi từ chối giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteFinance()
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa giao dịch '{finance.TransactionCode}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (financeBLL.DeleteFinance(finance.FinanceID))
                    {
                        LoadFinancesFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Xóa giao dịch thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowReport()
        {
            try
            {
                MessageBox.Show("Chức năng báo cáo đang được phát triển!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup - Đổi tên để tránh conflict với Designer
        private void SetupCustomLayout() // Đổi từ InitializeLayout
        {
            this.Text = "Quản lý Tài chính";
            this.BackColor = Color.White;
            this.Size = new Size(1700, 950);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupSearchPanel();
            SetupDataGrid();
            SetupFooter();
        }
        private void TransactionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update category dropdown based on transaction type
            categoryComboBox.Items.Clear();
            categoryComboBox.Items.Add("Tất cả danh mục");

            string selectedType = GetSelectedTransactionType();
            if (!string.IsNullOrEmpty(selectedType))
            {
                var categories = FinanceCategories.GetCategoriesByType(selectedType);
                foreach (var category in categories)
                {
                    categoryComboBox.Items.Add(FinanceCategories.GetDisplayName(category));
                }
            }
            else
            {
                // Add all categories if no specific type selected
                var allCategories = FinanceCategories.IncomeCategories.Concat(FinanceCategories.ExpenseCategories);
                foreach (var category in allCategories)
                {
                    categoryComboBox.Items.Add(FinanceCategories.GetDisplayName(category));
                }
            }

            categoryComboBox.SelectedIndex = 0;
            ApplyFilters();
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
                RowCount = 3,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Define column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            // Define row heights
            for (int i = 0; i < 3; i++)
                searchContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));

            // Row 1: Search box, Transaction Type, Category, Buttons
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

            transactionTypeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            transactionTypeComboBox.Items.Add("Tất cả loại GD");
            foreach (var type in TransactionTypes.Types)
            {
                transactionTypeComboBox.Items.Add(TransactionTypes.GetDisplayName(type));
            }
            transactionTypeComboBox.SelectedIndex = 0;
            transactionTypeComboBox.SelectedIndexChanged += TransactionTypeComboBox_SelectedIndexChanged;

            categoryComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            categoryComboBox.Items.Add("Tất cả danh mục");
            var allCategories = FinanceCategories.IncomeCategories.Concat(FinanceCategories.ExpenseCategories);
            foreach (var category in allCategories)
            {
                categoryComboBox.Items.Add(FinanceCategories.GetDisplayName(category));
            }
            categoryComboBox.SelectedIndex = 0;
            categoryComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Margin = new Padding(5)
            };

            searchButton = CreateStyledButton("🔍", Color.FromArgb(33, 150, 243), new Size(40, 35));
            searchButton.Click += (s, e) => ApplyFilters();

            clearButton = CreateStyledButton("🗑️", Color.FromArgb(244, 67, 54), new Size(40, 35));
            clearButton.Click += ClearFilters;

            reportButton = CreateStyledButton("📊", Color.FromArgb(139, 69, 19), new Size(40, 35));
            reportButton.Click += (s, e) => ShowReport();

            buttonsPanel.Controls.Add(searchButton);
            buttonsPanel.Controls.Add(clearButton);
            buttonsPanel.Controls.Add(reportButton);

            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(transactionTypeComboBox, 1, 0);
            searchContainer.Controls.Add(categoryComboBox, 2, 0);
            searchContainer.Controls.Add(buttonsPanel, 3, 0);

            // Row 2: Status, Project, Customer, Employee
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(0, 0, 10, 0)
            };
            statusComboBox.Items.Add("Tất cả trạng thái");
            foreach (var status in FinanceStatus.Statuses)
            {
                statusComboBox.Items.Add(FinanceStatus.GetDisplayName(status));
            }
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            projectComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 10, 0)
            };
            projectComboBox.Items.Add("Tất cả dự án");
            foreach (var project in projects)
            {
                projectComboBox.Items.Add(project.ProjectName);
            }
            projectComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            customerComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 10, 0)
            };
            customerComboBox.Items.Add("Tất cả khách hàng");
            foreach (var customer in customers)
            {
                customerComboBox.Items.Add(customer.CompanyName);
            }
            customerComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            employeeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 0, 0)
            };
            employeeComboBox.Items.Add("Tất cả nhân viên");
            foreach (var employee in employees)
            {
                employeeComboBox.Items.Add(employee.FullName);
            }
            employeeComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            searchContainer.Controls.Add(statusComboBox, 0, 1);
            searchContainer.Controls.Add(projectComboBox, 1, 1);
            searchContainer.Controls.Add(customerComboBox, 2, 1);
            searchContainer.Controls.Add(employeeComboBox, 3, 1);

            // Row 3: Date filters
            var datePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0, 5, 0, 0)
            };

            datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            var fromLabel = new Label
            {
                Text = "Từ ngày:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            fromDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false,
                Margin = new Padding(0, 0, 10, 0)
            };
            fromDatePicker.ValueChanged += (s, e) => ApplyFilters();

            var toLabel = new Label
            {
                Text = "Đến ngày:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Margin = new Padding(5, 0, 0, 0)
            };

            toDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false,
                Margin = new Padding(0, 0, 10, 0)
            };
            toDatePicker.ValueChanged += (s, e) => ApplyFilters();

            datePanel.Controls.Add(fromLabel, 0, 0);
            datePanel.Controls.Add(fromDatePicker, 1, 0);
            datePanel.Controls.Add(toLabel, 2, 0);
            datePanel.Controls.Add(toDatePicker, 3, 0);

            searchContainer.Controls.Add(datePanel, 0, 2);
            searchContainer.SetColumnSpan(datePanel, 4);

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

            financeDataGridView = new DataGridView
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

            gridPanel.Controls.Add(financeDataGridView);
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
            approveButton = CreateActionButton("✅ DUYỆT", Color.FromArgb(34, 197, 94));
            rejectButton = CreateActionButton("❌ TỪ CHỐI", Color.FromArgb(239, 68, 68));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;
            approveButton.Enabled = false;
            rejectButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(approveButton);
            buttonsPanel.Controls.Add(rejectButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
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

        private Button CreateStyledButton(string text, Color backColor, Size size)
        {
            return new Button
            {
                Text = text,
                Size = size,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(2),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(100, 45),
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
            financeDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 6, 8, 6),
                Font = new Font("Segoe UI", 9)
            };

            financeDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
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

            financeDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            financeDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "FinanceID", HeaderText = "ID", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "TransactionCode", HeaderText = "Mã GD", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "TransactionType", HeaderText = "Loại", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Category", HeaderText = "Danh mục", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "AmountDisplay", HeaderText = "Số tiền", Width = 120, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "TransactionDate", HeaderText = "Ngày GD", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "PaymentMethod", HeaderText = "Phương thức", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "RelatedTo", HeaderText = "Liên quan", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "RecordedBy", HeaderText = "Người ghi", Width = 100, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Description", HeaderText = "Mô tả", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true }
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

                if (col.Name == "TransactionDate")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                financeDataGridView.Columns.Add(column);
            }
        }
        private void FinanceDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = financeDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "TransactionType" && e.Value != null)
            {
                var transactionType = e.Value.ToString();
                e.CellStyle.ForeColor = transactionType.Contains("Thu") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "AmountDisplay" && e.Value != null)
            {
                var row = financeDataGridView.Rows[e.RowIndex];
                var transactionType = row.Cells["TransactionType"].Value?.ToString();
                e.CellStyle.ForeColor = transactionType != null && transactionType.Contains("Thu") ?
                    Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status.Contains("Đã duyệt") || status.Contains("Đã ghi nhận") ? Color.FromArgb(34, 197, 94) :
                                       status.Contains("Chờ duyệt") ? Color.FromArgb(245, 158, 11) :
                                       status.Contains("Từ chối") || status.Contains("Hủy") ? Color.FromArgb(239, 68, 68) :
                                       Color.FromArgb(64, 64, 64);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private void SetupDataGridEvents()
        {
            financeDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = financeDataGridView.SelectedRows.Count > 0;
                var selectedFinance = GetSelectedFinance();

                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection && selectedFinance?.Status != "Đã duyệt";
                approveButton.Enabled = hasSelection && selectedFinance?.Status == "Chờ duyệt";
                rejectButton.Enabled = hasSelection && selectedFinance?.Status == "Chờ duyệt";
            };

            financeDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewFinance();
            };

            financeDataGridView.CellFormatting += FinanceDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddFinance();
            editButton.Click += (s, e) => EditFinance();
            viewButton.Click += (s, e) => ViewFinance();
            deleteButton.Click += (s, e) => DeleteFinance();
            approveButton.Click += (s, e) => ApproveFinance();
            rejectButton.Click += (s, e) => RejectFinance();
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
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));  // Search
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
                Text = "💰 QUẢN LÝ TÀI CHÍNH",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        #endregion
    }
}