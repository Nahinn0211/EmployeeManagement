using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Reports
{
    public partial class FinanceReportForm : Form
    {
        #region Fields
        private FinanceBLL financeBLL;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel controlPanel;
        private Panel contentPanel;

        // Header controls
        private Label titleLabel;

        // Control panel
        private ComboBox reportTypeComboBox;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private ComboBox yearComboBox;
        private Button generateButton;
        private Button exportButton;

        // Content panel
        private TabControl reportTabControl;
        private TabPage summaryTab;
        private TabPage detailTab;
        private TabPage chartTab;
        private TabPage projectTab;

        // Summary controls
        private Label totalIncomeLabel;
        private Label totalExpenseLabel;
        private Label balanceLabel;
        private Label transactionCountLabel;
        private DataGridView categoryDataGridView;

        // Detail controls
        private DataGridView detailDataGridView;

        // Chart controls
        private Panel chartPanel;

        // Project controls
        private DataGridView projectDataGridView;
        #endregion

        #region Constructor
        public FinanceReportForm()
        {
            InitializeComponent();
            financeBLL = new FinanceBLL();
            SetupForm();
            LoadInitialData();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Báo cáo Tài chính";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            SetupLayout();
            SetupHeader();
            SetupControlPanel();
            SetupContentPanel();
        }

        private void SetupLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Controls
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Content

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            titleLabel = new Label
            {
                Text = "📊 BÁO CÁO TÀI CHÍNH",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupControlPanel()
        {
            controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var controlContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Report type
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // From date
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // To date
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));  // Year
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.5f)); // Generate button
            controlContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.5f)); // Export button

            // Report Type
            var reportTypePanel = CreateControlPanel("Loại báo cáo:", out reportTypeComboBox);
            reportTypeComboBox.Items.AddRange(new string[]
            {
                "Tổng quan theo khoảng thời gian",
                "Báo cáo theo tháng",
                "Báo cáo theo danh mục",
                "Báo cáo dự án"
            });
            reportTypeComboBox.SelectedIndex = 0;
            reportTypeComboBox.SelectedIndexChanged += ReportTypeComboBox_SelectedIndexChanged;

            // From Date
            var fromDatePanel = CreateDateControlPanel("Từ ngày:", out fromDatePicker);
            fromDatePicker.Value = DateTime.Now.AddMonths(-1).Date;

            // To Date
            var toDatePanel = CreateDateControlPanel("Đến ngày:", out toDatePicker);
            toDatePicker.Value = DateTime.Now.Date;

            // Year
            var yearPanel = CreateControlPanel("Năm:", out yearComboBox);
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
            {
                yearComboBox.Items.Add(year.ToString());
            }
            yearComboBox.SelectedIndex = 0;

            // Generate Button
            generateButton = new Button
            {
                Text = "📊 Tạo báo cáo",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 10, 5, 10),
                FlatAppearance = { BorderSize = 0 }
            };
            generateButton.Click += GenerateButton_Click;

            // Export Button
            exportButton = new Button
            {
                Text = "📄 Xuất Excel",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(5, 10, 10, 10),
                FlatAppearance = { BorderSize = 0 }
            };
            exportButton.Click += ExportButton_Click;

            controlContainer.Controls.Add(reportTypePanel, 0, 0);
            controlContainer.Controls.Add(fromDatePanel, 1, 0);
            controlContainer.Controls.Add(toDatePanel, 2, 0);
            controlContainer.Controls.Add(yearPanel, 3, 0);
            controlContainer.Controls.Add(generateButton, 4, 0);
            controlContainer.Controls.Add(exportButton, 5, 0);

            controlPanel.Controls.Add(controlContainer);
            mainTableLayout.Controls.Add(controlPanel, 0, 1);
        }

        private void SetupContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            reportTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                ItemSize = new Size(120, 40),
                SizeMode = TabSizeMode.Fixed
            };

            SetupSummaryTab();
            SetupDetailTab();
            SetupChartTab();
            SetupProjectTab();

            reportTabControl.TabPages.Add(summaryTab);
            reportTabControl.TabPages.Add(detailTab);
            reportTabControl.TabPages.Add(chartTab);
            reportTabControl.TabPages.Add(projectTab);

            contentPanel.Controls.Add(reportTabControl);
            mainTableLayout.Controls.Add(contentPanel, 0, 2);
        }

        private void SetupSummaryTab()
        {
            summaryTab = new TabPage
            {
                Text = "Tổng quan",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var summaryLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150)); // Stats cards
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Category breakdown

            // Stats cards
            var statsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0, 0, 0, 20)
            };

            for (int i = 0; i < 4; i++)
                statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Income card
            var incomeCard = CreateStatsCard("💰 Tổng Thu", "0 VNĐ", Color.FromArgb(34, 197, 94));
            totalIncomeLabel = incomeCard.Controls.OfType<Label>().Last();

            // Expense card
            var expenseCard = CreateStatsCard("💸 Tổng Chi", "0 VNĐ", Color.FromArgb(239, 68, 68));
            totalExpenseLabel = expenseCard.Controls.OfType<Label>().Last();

            // Balance card
            var balanceCard = CreateStatsCard("📈 Số dư", "0 VNĐ", Color.FromArgb(59, 130, 246));
            balanceLabel = balanceCard.Controls.OfType<Label>().Last();

            // Transaction count card
            var countCard = CreateStatsCard("📊 Giao dịch", "0", Color.FromArgb(168, 85, 247));
            transactionCountLabel = countCard.Controls.OfType<Label>().Last();

            statsPanel.Controls.Add(incomeCard, 0, 0);
            statsPanel.Controls.Add(expenseCard, 1, 0);
            statsPanel.Controls.Add(balanceCard, 2, 0);
            statsPanel.Controls.Add(countCard, 3, 0);

            // Category breakdown
            var categoryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var categoryTitle = new Label
            {
                Text = "📊 Phân tích theo danh mục",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            categoryDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10)
            };

            categoryPanel.Controls.Add(categoryDataGridView);
            categoryPanel.Controls.Add(categoryTitle);

            summaryLayout.Controls.Add(statsPanel, 0, 0);
            summaryLayout.Controls.Add(categoryPanel, 0, 1);

            summaryTab.Controls.Add(summaryLayout);
        }

        private void SetupDetailTab()
        {
            detailTab = new TabPage
            {
                Text = "Chi tiết",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var detailTitle = new Label
            {
                Text = "📋 Chi tiết giao dịch",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            detailDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9)
            };

            detailTab.Controls.Add(detailDataGridView);
            detailTab.Controls.Add(detailTitle);
        }

        private void SetupChartTab()
        {
            chartTab = new TabPage
            {
                Text = "Biểu đồ",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var chartTitle = new Label
            {
                Text = "📈 Biểu đồ tài chính",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var chartPlaceholder = new Label
            {
                Text = "📊 Biểu đồ sẽ được hiển thị ở đây sau khi tạo báo cáo",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            chartPanel.Controls.Add(chartPlaceholder);

            chartTab.Controls.Add(chartPanel);
            chartTab.Controls.Add(chartTitle);
        }

        private void SetupProjectTab()
        {
            projectTab = new TabPage
            {
                Text = "Dự án",
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var projectTitle = new Label
            {
                Text = "💼 Báo cáo tài chính theo dự án",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            projectDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10)
            };

            projectTab.Controls.Add(projectDataGridView);
            projectTab.Controls.Add(projectTitle);
        }
        #endregion

        #region Helper Methods
        private Panel CreateControlPanel(string labelText, out ComboBox comboBox)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.Transparent
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.BottomLeft
            };

            comboBox = new ComboBox
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35
            };

            panel.Controls.Add(label);
            panel.Controls.Add(comboBox);

            return panel;
        }

        private Panel CreateDateControlPanel(string labelText, out DateTimePicker datePicker)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.Transparent
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.BottomLeft
            };

            datePicker = new DateTimePicker
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Height = 35
            };

            panel.Controls.Add(label);
            panel.Controls.Add(datePicker);

            return panel;
        }

        private Panel CreateStatsCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10),
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var accentLine = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = accentColor,
                Margin = new Padding(0, 0, 0, 10)
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(accentLine);
            card.Controls.Add(titleLabel);

            return card;
        }
        #endregion

        #region Event Handlers
        private void ReportTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable date controls based on report type
            bool enableDates = reportTypeComboBox.SelectedIndex == 0 || reportTypeComboBox.SelectedIndex == 2;
            bool enableYear = reportTypeComboBox.SelectedIndex == 1;

            fromDatePicker.Enabled = enableDates;
            toDatePicker.Enabled = enableDates;
            yearComboBox.Enabled = enableYear;
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            try
            {
                switch (reportTypeComboBox.SelectedIndex)
                {
                    case 0: // Tổng quan theo khoảng thời gian
                        GenerateOverviewReport();
                        break;
                    case 1: // Báo cáo theo tháng
                        GenerateMonthlyReport();
                        break;
                    case 2: // Báo cáo theo danh mục
                        GenerateCategoryReport();
                        break;
                    case 3: // Báo cáo dự án
                        GenerateProjectReport();
                        break;
                }

                reportTabControl.SelectedIndex = 0; // Switch to summary tab
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Chức năng xuất Excel đang được phát triển!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Report Generation
        private void LoadInitialData()
        {
            try
            {
                GenerateOverviewReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu ban đầu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateOverviewReport()
        {
            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date;

            // Get statistics
            var stats = financeBLL.GetFinanceStatistics(fromDate, toDate);

            // Update stats cards
            totalIncomeLabel.Text = $"{stats.TotalIncome:#,##0.##} VNĐ";
            totalExpenseLabel.Text = $"{stats.TotalExpense:#,##0.##} VNĐ";
            balanceLabel.Text = $"{stats.Balance:#,##0.##} VNĐ";
            transactionCountLabel.Text = stats.TotalTransactions.ToString("#,##0");

            // Update balance card color
            var balanceCard = balanceLabel.Parent.Parent;
            var accentLine = balanceCard.Controls.OfType<Panel>().FirstOrDefault();
            if (accentLine != null)
            {
                accentLine.BackColor = stats.Balance >= 0 ?
                    Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }

            // Load category breakdown
            LoadCategoryBreakdown(fromDate, toDate);

            // Load detailed transactions
            LoadDetailedTransactions(fromDate, toDate);
        }

        private void GenerateMonthlyReport()
        {
            if (!int.TryParse(yearComboBox.Text, out int year))
            {
                MessageBox.Show("Vui lòng chọn năm hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var monthlyReports = financeBLL.GetMonthlyStatistics(year);

            // Calculate totals for the year
            decimal totalIncome = monthlyReports.Sum(r => r.Income);
            decimal totalExpense = monthlyReports.Sum(r => r.Expense);
            decimal balance = totalIncome - totalExpense;

            // Update stats cards
            totalIncomeLabel.Text = $"{totalIncome:#,##0.##} VNĐ";
            totalExpenseLabel.Text = $"{totalExpense:#,##0.##} VNĐ";
            balanceLabel.Text = $"{balance:#,##0.##} VNĐ";
            transactionCountLabel.Text = "12 tháng";

            // Load monthly breakdown in category grid
            LoadMonthlyBreakdown(monthlyReports);

            // Load detailed transactions for the year
            LoadDetailedTransactions(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private void GenerateCategoryReport()
        {
            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date;

            // Get statistics
            var stats = financeBLL.GetFinanceStatistics(fromDate, toDate);

            // Update stats cards
            totalIncomeLabel.Text = $"{stats.TotalIncome:#,##0.##} VNĐ";
            totalExpenseLabel.Text = $"{stats.TotalExpense:#,##0.##} VNĐ";
            balanceLabel.Text = $"{stats.Balance:#,##0.##} VNĐ";
            transactionCountLabel.Text = stats.TotalTransactions.ToString("#,##0");

            // Load detailed category breakdown
            LoadDetailedCategoryBreakdown(fromDate, toDate);

            // Load detailed transactions
            LoadDetailedTransactions(fromDate, toDate);
        }

        private void GenerateProjectReport()
        {
            var projectReports = financeBLL.GetProjectFinanceStatistics();

            // Calculate totals
            decimal totalIncome = projectReports.Sum(r => r.Income);
            decimal totalExpense = projectReports.Sum(r => r.Expense);
            decimal balance = totalIncome - totalExpense;

            // Update stats cards
            totalIncomeLabel.Text = $"{totalIncome:#,##0.##} VNĐ";
            totalExpenseLabel.Text = $"{totalExpense:#,##0.##} VNĐ";
            balanceLabel.Text = $"{balance:#,##0.##} VNĐ";
            transactionCountLabel.Text = $"{projectReports.Count} dự án";

            // Load project data
            LoadProjectData(projectReports);

            // Load all project transactions
            LoadProjectTransactions();
        }

        private void LoadCategoryBreakdown(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var incomeCategories = financeBLL.GetCategoryStatistics("Thu", fromDate, toDate);
                var expenseCategories = financeBLL.GetCategoryStatistics("Chi", fromDate, toDate);

                var categoryData = new List<object>();

                // Add income categories
                foreach (var category in incomeCategories)
                {
                    categoryData.Add(new
                    {
                        Loại = "💰 Thu",
                        DanhMục = FinanceCategories.GetDisplayName(category.Key),
                        SốTiền = category.Value,
                        SốTiềnDisplay = $"{category.Value:#,##0.##} VNĐ",
                        TỷLệ = incomeCategories.Values.Sum() > 0 ? (category.Value / incomeCategories.Values.Sum() * 100) : 0
                    });
                }

                // Add expense categories
                foreach (var category in expenseCategories)
                {
                    categoryData.Add(new
                    {
                        Loại = "💸 Chi",
                        DanhMục = FinanceCategories.GetDisplayName(category.Key),
                        SốTiền = category.Value,
                        SốTiềnDisplay = $"{category.Value:#,##0.##} VNĐ",
                        TỷLệ = expenseCategories.Values.Sum() > 0 ? (category.Value / expenseCategories.Values.Sum() * 100) : 0
                    });
                }

                categoryDataGridView.DataSource = categoryData.OrderByDescending(x => ((dynamic)x).SốTiền).ToList();

                // Setup columns
                SetupCategoryGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải phân tích danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDetailedTransactions(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Fixed: Sử dụng parameters riêng lẻ thay vì FinanceSearchCriteria
                var transactions = financeBLL.SearchFinances("", "", "", "", null, null, null, fromDate, toDate);

                var detailData = transactions.Select(t => new
                {
                    MãGD = t.TransactionCode,
                    Ngày = t.TransactionDate.ToString("dd/MM/yyyy"),
                    Loại = t.TransactionType == "Thu" ? "💰 Thu" : "💸 Chi",
                    DanhMục = FinanceCategories.GetDisplayName(t.Category),
                    SốTiền = $"{t.Amount:#,##0.##} VNĐ",
                    MôTả = t.Description ?? "",
                    TrạngThái = t.Status
                }).ToList();

                detailDataGridView.DataSource = detailData;
                SetupDetailGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMonthlyBreakdown(List<MonthlyFinanceReportDTO> monthlyReports)
        {
            try
            {
                var monthlyData = monthlyReports.Select(r => new
                {
                    Tháng = r.MonthName,
                    Thu = $"{r.Income:#,##0.##} VNĐ",
                    Chi = $"{r.Expense:#,##0.##} VNĐ",
                    SốDư = $"{r.Balance:#,##0.##} VNĐ",
                    SốGD = r.TransactionCount,
                    TrungBình = $"{r.AverageTransaction:#,##0.##} VNĐ"
                }).ToList();

                categoryDataGridView.DataSource = monthlyData;
                SetupMonthlyGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo theo tháng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDetailedCategoryBreakdown(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var incomeCategories = financeBLL.GetCategoryStatistics("Thu", fromDate, toDate);
                var expenseCategories = financeBLL.GetCategoryStatistics("Chi", fromDate, toDate);

                var allCategories = new List<object>();

                decimal totalIncome = incomeCategories.Values.Sum();
                decimal totalExpense = expenseCategories.Values.Sum();
                decimal totalAll = totalIncome + totalExpense;

                // Add income categories with more details
                foreach (var category in incomeCategories.OrderByDescending(x => x.Value))
                {
                    allCategories.Add(new
                    {
                        Loại = "💰 Thu",
                        DanhMục = FinanceCategories.GetDisplayName(category.Key),
                        SốTiền = $"{category.Value:#,##0.##} VNĐ",
                        TỷLệTrongLoại = totalIncome > 0 ? $"{(category.Value / totalIncome * 100):0.##}%" : "0%",
                        TỷLệTổng = totalAll > 0 ? $"{(category.Value / totalAll * 100):0.##}%" : "0%"
                    });
                }

                // Add expense categories with more details
                foreach (var category in expenseCategories.OrderByDescending(x => x.Value))
                {
                    allCategories.Add(new
                    {
                        Loại = "💸 Chi",
                        DanhMục = FinanceCategories.GetDisplayName(category.Key),
                        SốTiền = $"{category.Value:#,##0.##} VNĐ",
                        TỷLệTrongLoại = totalExpense > 0 ? $"{(category.Value / totalExpense * 100):0.##}%" : "0%",
                        TỷLệTổng = totalAll > 0 ? $"{(category.Value / totalAll * 100):0.##}%" : "0%"
                    });
                }

                categoryDataGridView.DataSource = allCategories;
                SetupDetailedCategoryGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải phân tích chi tiết danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProjectData(List<ProjectFinanceReportDTO> projectReports)
        {
            try
            {
                var projectData = projectReports.Select(r => new
                {
                    DựÁn = r.ProjectName,
                    NgânSách = $"{r.Budget:#,##0.##} VNĐ",
                    Thu = $"{r.Income:#,##0.##} VNĐ",
                    Chi = $"{r.Expense:#,##0.##} VNĐ",
                    SốDư = $"{r.Balance:#,##0.##} VNĐ",
                    SửDụngNS = $"{r.BudgetUsed:0.##}%",
                    TrạngTháiNS = r.BudgetStatus,
                    SốGD = r.TransactionCount
                }).ToList();

                projectDataGridView.DataSource = projectData;
                categoryDataGridView.DataSource = projectData;

                // Setup columns for both grids
                SetupProjectColumns(projectDataGridView);
                SetupProjectColumns(categoryDataGridView);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProjectTransactions()
        {
            try
            {
                // Load all transactions that have ProjectID - Fixed: Sử dụng parameters riêng lẻ
                var allTransactions = financeBLL.SearchFinances("", "", "", "", null, null, null, null, null);
                var projectTransactions = allTransactions.Where(t => t.ProjectID.HasValue).ToList();

                var detailData = projectTransactions.Select(t => new
                {
                    DựÁn = t.Project?.ProjectName ?? "N/A",
                    MãGD = t.TransactionCode,
                    Ngày = t.TransactionDate.ToString("dd/MM/yyyy"),
                    Loại = t.TransactionType == "Thu" ? "💰 Thu" : "💸 Chi",
                    DanhMục = FinanceCategories.GetDisplayName(t.Category),
                    SốTiền = $"{t.Amount:#,##0.##} VNĐ",
                    MôTả = t.Description ?? "",
                    TrạngThái = t.Status
                }).ToList();

                detailDataGridView.DataSource = detailData;
                SetupProjectTransactionGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải giao dịch dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupCategoryGridColumns()
        {
            if (categoryDataGridView.Columns.Count > 0)
            {
                categoryDataGridView.Columns["SốTiền"].Visible = false;
                categoryDataGridView.Columns["Loại"].HeaderText = "Loại";
                categoryDataGridView.Columns["DanhMục"].HeaderText = "Danh mục";
                categoryDataGridView.Columns["SốTiềnDisplay"].HeaderText = "Số tiền";
                categoryDataGridView.Columns["TỷLệ"].HeaderText = "Tỷ lệ (%)";
                categoryDataGridView.Columns["TỷLệ"].DefaultCellStyle.Format = "0.##";
            }
        }

        private void SetupDetailGridColumns()
        {
            if (detailDataGridView.Columns.Count > 0)
            {
                detailDataGridView.Columns["MãGD"].HeaderText = "Mã GD";
                detailDataGridView.Columns["Ngày"].HeaderText = "Ngày";
                detailDataGridView.Columns["Loại"].HeaderText = "Loại";
                detailDataGridView.Columns["DanhMục"].HeaderText = "Danh mục";
                detailDataGridView.Columns["SốTiền"].HeaderText = "Số tiền";
                detailDataGridView.Columns["MôTả"].HeaderText = "Mô tả";
                detailDataGridView.Columns["TrạngThái"].HeaderText = "Trạng thái";
            }
        }

        private void SetupMonthlyGridColumns()
        {
            if (categoryDataGridView.Columns.Count > 0)
            {
                categoryDataGridView.Columns["Tháng"].HeaderText = "Tháng";
                categoryDataGridView.Columns["Thu"].HeaderText = "Thu nhập";
                categoryDataGridView.Columns["Chi"].HeaderText = "Chi tiêu";
                categoryDataGridView.Columns["SốDư"].HeaderText = "Số dư";
                categoryDataGridView.Columns["SốGD"].HeaderText = "Số GD";
                categoryDataGridView.Columns["TrungBình"].HeaderText = "TB/GD";
            }
        }

        private void SetupDetailedCategoryGridColumns()
        {
            if (categoryDataGridView.Columns.Count > 0)
            {
                categoryDataGridView.Columns["Loại"].HeaderText = "Loại";
                categoryDataGridView.Columns["DanhMục"].HeaderText = "Danh mục";
                categoryDataGridView.Columns["SốTiền"].HeaderText = "Số tiền";
                categoryDataGridView.Columns["TỷLệTrongLoại"].HeaderText = "% trong loại";
                categoryDataGridView.Columns["TỷLệTổng"].HeaderText = "% tổng";
            }
        }

        private void SetupProjectColumns(DataGridView grid)
        {
            if (grid.Columns.Count > 0)
            {
                grid.Columns["DựÁn"].HeaderText = "Dự án";
                grid.Columns["NgânSách"].HeaderText = "Ngân sách";
                grid.Columns["Thu"].HeaderText = "Thu nhập";
                grid.Columns["Chi"].HeaderText = "Chi tiêu";
                grid.Columns["SốDư"].HeaderText = "Số dư";
                grid.Columns["SửDụngNS"].HeaderText = "% Sử dụng NS";
                grid.Columns["TrạngTháiNS"].HeaderText = "Trạng thái NS";
                grid.Columns["SốGD"].HeaderText = "Số GD";
            }
        }

        private void SetupProjectTransactionGridColumns()
        {
            if (detailDataGridView.Columns.Count > 0)
            {
                detailDataGridView.Columns["DựÁn"].HeaderText = "Dự án";
                detailDataGridView.Columns["MãGD"].HeaderText = "Mã GD";
                detailDataGridView.Columns["Ngày"].HeaderText = "Ngày";
                detailDataGridView.Columns["Loại"].HeaderText = "Loại";
                detailDataGridView.Columns["DanhMục"].HeaderText = "Danh mục";
                detailDataGridView.Columns["SốTiền"].HeaderText = "Số tiền";
                detailDataGridView.Columns["MôTả"].HeaderText = "Mô tả";
                detailDataGridView.Columns["TrạngThái"].HeaderText = "Trạng thái";
            }
        }
        #endregion
    }
}