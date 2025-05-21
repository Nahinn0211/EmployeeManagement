using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Finance
{
    public partial class ProjectFinanceForm : Form
    {
        #region Fields
        private FinanceBLL financeBLL;
        private ProjectBLL projectBLL;
        private List<Models.Project> projects;
        private List<Models.Finance> projectFinances;
        private List<Models.Finance> filteredFinances;
        private Models.Project selectedProject;
        private int currentUserId; // Thêm field này

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel projectSelectorPanel;
        private Panel summaryPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;

        // Project selector controls
        private ComboBox projectComboBox;
        private Button refreshButton;
        private Button exportButton;

        // Summary controls
        private Panel incomeCard;
        private Panel expenseCard;
        private Panel profitCard;
        private Panel budgetCard;

        // Grid controls
        private DataGridView financeDataGridView;

        // Footer controls
        private Button addFinanceButton;
        private Button editFinanceButton;
        private Button viewFinanceButton;
        private Button deleteFinanceButton;
        private Label totalLabel;
        #endregion

        #region Constructor
        public ProjectFinanceForm()
        {
            InitializeComponent();
            financeBLL = new FinanceBLL();
            projectBLL = new ProjectBLL();
            currentUserId = 1; // Default user ID hoặc get từ session
            LoadProjectsData();
            InitializeLayout();
        }

        public ProjectFinanceForm(int userId)
        {
            InitializeComponent();
            financeBLL = new FinanceBLL();
            projectBLL = new ProjectBLL();
            currentUserId = userId;
            LoadProjectsData();
            InitializeLayout();
        }
        #endregion

        #region Database Methods
        private void LoadProjectsData()
        {
            try
            {
                projects = projectBLL.GetAllProjects()
                    .Where(p => p.Status != "Đã hủy")
                    .OrderBy(p => p.ProjectName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                projects = new List<Models.Project>();
            }
        }

        private void LoadProjectFinances(int projectId)
        {
            try
            {
                selectedProject = projects.FirstOrDefault(p => p.ProjectID == projectId);
                if (selectedProject == null) return;

                projectFinances = financeBLL.GetFinancesByProject(projectId);
                filteredFinances = new List<Models.Finance>(projectFinances);

                LoadFinancesToGrid();
                UpdateSummaryCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tài chính dự án: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Data Management
        private void LoadFinancesToGrid()
        {
            try
            {
                var dataSource = filteredFinances.Select(f => new
                {
                    FinanceID = f.FinanceID,
                    TransactionCode = f.TransactionCode,
                    TransactionDate = f.TransactionDate,
                    TransactionType = f.TransactionTypeDisplay ?? f.TransactionType,
                    Category = f.CategoryDisplay ?? f.Category,
                    Amount = f.Amount,
                    AmountDisplay = f.AmountDisplay ?? f.Amount.ToString("#,##0") + " VNĐ",
                    PaymentMethod = f.PaymentMethodDisplay ?? f.PaymentMethod,
                    Status = f.StatusDisplay ?? f.Status,
                    Description = f.Description,
                    RecordedBy = f.RecordedBy?.FullName ?? "N/A"
                }).ToList();

                financeDataGridView.DataSource = dataSource;
                UpdateTotalLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryCards()
        {
            if (selectedProject == null || projectFinances == null)
            {
                ResetSummaryCards();
                return;
            }

            var approvedFinances = projectFinances.Where(f => f.Status == "Đã duyệt").ToList();

            decimal totalIncome = approvedFinances.Where(f => f.TransactionType == "Thu").Sum(f => f.Amount);
            decimal totalExpense = approvedFinances.Where(f => f.TransactionType == "Chi").Sum(f => f.Amount);
            decimal netProfit = totalIncome - totalExpense;
            decimal budget = selectedProject.Budget;

            UpdateSummaryCard(incomeCard, "💰 TỔNG THU", totalIncome, Color.FromArgb(76, 175, 80));
            UpdateSummaryCard(expenseCard, "💸 TỔNG CHI", totalExpense, Color.FromArgb(244, 67, 54));
            UpdateSummaryCard(profitCard, "📈 LỢI NHUẬN", netProfit, netProfit >= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54));
            UpdateSummaryCard(budgetCard, "💼 NGÂN SÁCH", budget, Color.FromArgb(33, 150, 243));

            // Update budget progress
            UpdateBudgetProgress(totalExpense, budget);
        }

        private void UpdateSummaryCard(Panel card, string title, decimal amount, Color color)
        {
            var titleLabel = card.Controls.OfType<Label>().FirstOrDefault(l => l.Tag?.ToString() == "title");
            var amountLabel = card.Controls.OfType<Label>().FirstOrDefault(l => l.Tag?.ToString() == "amount");

            if (titleLabel != null) titleLabel.Text = title;
            if (amountLabel != null)
            {
                amountLabel.Text = $"{amount:#,##0} VNĐ";
                amountLabel.ForeColor = color;
            }
        }

        private void UpdateBudgetProgress(decimal totalExpense, decimal budget)
        {
            if (budget <= 0) return;

            decimal percentUsed = (totalExpense / budget) * 100;
            var progressBar = budgetCard.Controls.OfType<ProgressBar>().FirstOrDefault();
            var percentLabel = budgetCard.Controls.OfType<Label>().FirstOrDefault(l => l.Tag?.ToString() == "percent");

            if (progressBar != null)
            {
                progressBar.Value = Math.Min((int)percentUsed, 100);

                // Change color based on usage
                if (percentUsed > 90)
                    progressBar.ForeColor = Color.FromArgb(244, 67, 54); // Red
                else if (percentUsed > 70)
                    progressBar.ForeColor = Color.FromArgb(255, 152, 0); // Orange
                else
                    progressBar.ForeColor = Color.FromArgb(76, 175, 80); // Green
            }

            if (percentLabel != null)
            {
                percentLabel.Text = $"Đã sử dụng: {percentUsed:F1}%";
                percentLabel.ForeColor = percentUsed > 90 ? Color.FromArgb(244, 67, 54) : Color.FromArgb(100, 100, 100);
            }
        }

        private void ResetSummaryCards()
        {
            UpdateSummaryCard(incomeCard, "💰 TỔNG THU", 0, Color.FromArgb(76, 175, 80));
            UpdateSummaryCard(expenseCard, "💸 TỔNG CHI", 0, Color.FromArgb(244, 67, 54));
            UpdateSummaryCard(profitCard, "📈 LỢI NHUẬN", 0, Color.FromArgb(100, 100, 100));
            UpdateSummaryCard(budgetCard, "💼 NGÂN SÁCH", 0, Color.FromArgb(33, 150, 243));
        }

        private void UpdateTotalLabel()
        {
            if (filteredFinances == null)
            {
                totalLabel.Text = "Chọn dự án để xem chi tiết tài chính";
                return;
            }

            int totalTransactions = filteredFinances.Count;
            decimal totalAmount = filteredFinances.Sum(f => f.Amount);

            totalLabel.Text = $"📊 Tổng: {totalTransactions} giao dịch | Tổng tiền: {totalAmount:#,##0} VNĐ";
        }
        #endregion

        #region Event Handlers
        private void ProjectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (projectComboBox.SelectedIndex <= 0)
            {
                selectedProject = null;
                projectFinances = new List<Models.Finance>();
                filteredFinances = new List<Models.Finance>();
                LoadFinancesToGrid();
                ResetSummaryCards();
                return;
            }

            var selectedProjectName = projectComboBox.Text;
            var project = projects.FirstOrDefault(p => p.ProjectName == selectedProjectName);

            if (project != null)
            {
                LoadProjectFinances(project.ProjectID);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                LoadProjectsData();
                SetupProjectComboBox();

                if (selectedProject != null)
                {
                    LoadProjectFinances(selectedProject.ProjectID);
                }

                MessageBox.Show("Đã làm mới dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (selectedProject == null)
            {
                MessageBox.Show("Vui lòng chọn dự án để xuất báo cáo!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // TODO: Implement export to Excel functionality
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Files|*.xlsx|All Files|*.*";
                    saveDialog.FileName = $"BaoCaoTaiChinh_{selectedProject.ProjectCode}_{DateTime.Now:yyyyMMdd}.xlsx";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // ExportToExcel(saveDialog.FileName);
                        MessageBox.Show("Xuất báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddFinanceButton_Click(object sender, EventArgs e)
        {
            if (selectedProject == null)
            {
                MessageBox.Show("Vui lòng chọn dự án trước khi thêm giao dịch!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Fixed: Sử dụng constructor với userId và project
                using (var form = new FinanceDetailForm(currentUserId, selectedProject))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadProjectFinances(selectedProject.ProjectID);
                        MessageBox.Show("Thêm giao dịch thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditFinanceButton_Click(object sender, EventArgs e)
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                // Fixed: Sử dụng constructor với finance object
                using (var form = new FinanceDetailForm(finance))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadProjectFinances(selectedProject.ProjectID);
                        MessageBox.Show("Cập nhật giao dịch thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewFinanceButton_Click(object sender, EventArgs e)
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                // Fixed: Sử dụng constructor với finance object và viewOnly = true
                using (var form = new FinanceDetailForm(finance, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteFinanceButton_Click(object sender, EventArgs e)
        {
            var finance = GetSelectedFinance();
            if (finance == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa giao dịch '{finance.TransactionCode}'?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (financeBLL.UpdateFinanceStatus(finance.FinanceID, "Hủy"))
                    {
                        LoadProjectFinances(selectedProject.ProjectID);
                        MessageBox.Show("Xóa giao dịch thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FinanceDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = financeDataGridView.SelectedRows.Count > 0;
            var selectedFinance = GetSelectedFinance();

            editFinanceButton.Enabled = hasSelection && selectedFinance?.Status != "Đã duyệt";
            viewFinanceButton.Enabled = hasSelection;
            deleteFinanceButton.Enabled = hasSelection && selectedFinance?.Status != "Đã duyệt";
        }

        private void FinanceDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                ViewFinanceButton_Click(null, null);
        }

        private void FinanceDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = financeDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "TransactionType" && e.Value != null)
            {
                var transactionType = e.Value.ToString();
                e.CellStyle.ForeColor = transactionType.Contains("Thu") ?
                    Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "AmountDisplay" && e.Value != null)
            {
                var row = financeDataGridView.Rows[e.RowIndex];
                var transactionType = row.Cells["TransactionType"].Value?.ToString();
                e.CellStyle.ForeColor = transactionType != null && transactionType.Contains("Thu") ?
                    Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status.Contains("Đã duyệt") ? Color.FromArgb(76, 175, 80) :
                                       status.Contains("Chờ duyệt") ? Color.FromArgb(255, 152, 0) :
                                       status.Contains("Từ chối") || status.Contains("Hủy") ? Color.FromArgb(244, 67, 54) :
                                       Color.FromArgb(64, 64, 64);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }
        #endregion

        #region Helper Methods
        private Models.Finance GetSelectedFinance()
        {
            if (financeDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = financeDataGridView.SelectedRows[0];
                var financeId = (int)selectedRow.Cells["FinanceID"].Value;
                return filteredFinances.FirstOrDefault(f => f.FinanceID == financeId);
            }
            return null;
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Tài chính Dự án";
            this.BackColor = Color.White;
            this.Size = new Size(1600, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupProjectSelector();
            SetupSummaryPanel();
            SetupDataGrid();
            SetupFooter();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Project Selector
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));  // Summary
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
                Text = "💼 QUẢN LÝ TÀI CHÍNH DỰ ÁN",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupProjectSelector()
        {
            projectSelectorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var selectorContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            selectorContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            selectorContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            selectorContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            selectorContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

            var projectLabel = new Label
            {
                Text = "Chọn dự án:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            projectComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(10, 5, 10, 5)
            };
            SetupProjectComboBox();
            projectComboBox.SelectedIndexChanged += ProjectComboBox_SelectedIndexChanged;

            refreshButton = new Button
            {
                Text = "🔄 Làm mới",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5),
                FlatAppearance = { BorderSize = 0 }
            };
            refreshButton.Click += RefreshButton_Click;

            exportButton = new Button
            {
                Text = "📊 Xuất BC",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5),
                FlatAppearance = { BorderSize = 0 }
            };
            exportButton.Click += ExportButton_Click;

            selectorContainer.Controls.Add(projectLabel, 0, 0);
            selectorContainer.Controls.Add(projectComboBox, 1, 0);
            selectorContainer.Controls.Add(refreshButton, 2, 0);
            selectorContainer.Controls.Add(exportButton, 3, 0);

            projectSelectorPanel.Controls.Add(selectorContainer);
            mainTableLayout.Controls.Add(projectSelectorPanel, 0, 1);
        }

        private void SetupProjectComboBox()
        {
            projectComboBox.Items.Clear();
            projectComboBox.Items.Add("-- Chọn dự án --");

            foreach (var project in projects)
            {
                projectComboBox.Items.Add(project.ProjectName);
            }

            projectComboBox.SelectedIndex = 0;
        }

        private void SetupSummaryPanel()
        {
            summaryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var summaryContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            for (int i = 0; i < 4; i++)
                summaryContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            incomeCard = CreateSummaryCard("💰 TỔNG THU", "0 VNĐ", Color.FromArgb(76, 175, 80));
            expenseCard = CreateSummaryCard("💸 TỔNG CHI", "0 VNĐ", Color.FromArgb(244, 67, 54));
            profitCard = CreateSummaryCard("📈 LỢI NHUẬN", "0 VNĐ", Color.FromArgb(100, 100, 100));
            budgetCard = CreateBudgetCard("💼 NGÂN SÁCH", "0 VNĐ", Color.FromArgb(33, 150, 243));

            summaryContainer.Controls.Add(incomeCard, 0, 0);
            summaryContainer.Controls.Add(expenseCard, 1, 0);
            summaryContainer.Controls.Add(profitCard, 2, 0);
            summaryContainer.Controls.Add(budgetCard, 3, 0);

            summaryPanel.Controls.Add(summaryContainer);
            mainTableLayout.Controls.Add(summaryPanel, 0, 2);
        }

        private Panel CreateSummaryCard(string title, string amount, Color color)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.BottomLeft,
                Tag = "title"
            };

            var amountLabel = new Label
            {
                Text = amount,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = color,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = "amount"
            };

            card.Controls.Add(amountLabel);
            card.Controls.Add(titleLabel);

            return card;
        }

        private Panel CreateBudgetCard(string title, string amount, Color color)
        {
            var card = CreateSummaryCard(title, amount, color);

            var progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.FromArgb(76, 175, 80),
                BackColor = Color.FromArgb(230, 230, 230),
                Value = 0,
                Maximum = 100
            };

            var percentLabel = new Label
            {
                Text = "Đã sử dụng: 0%",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Bottom,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = "percent"
            };

            card.Controls.Add(progressBar);
            card.Controls.Add(percentLabel);

            return card;
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
                RowTemplate = { Height = 40 },
                AutoGenerateColumns = false
            };

            SetupDataGridStyles();
            SetupDataGridColumns();
            SetupDataGridEvents();

            gridPanel.Controls.Add(financeDataGridView);
            mainTableLayout.Controls.Add(gridPanel, 0, 3);
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
                new { Name = "TransactionDate", HeaderText = "Ngày GD", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "TransactionType", HeaderText = "Loại", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Category", HeaderText = "Danh mục", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "AmountDisplay", HeaderText = "Số tiền", Width = 120, Alignment = DataGridViewContentAlignment.MiddleRight, Visible = true },
                new { Name = "PaymentMethod", HeaderText = "Phương thức", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Description", HeaderText = "Mô tả", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "RecordedBy", HeaderText = "Người ghi", Width = 100, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true }
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

        private void SetupDataGridEvents()
        {
            financeDataGridView.SelectionChanged += FinanceDataGridView_SelectionChanged;
            financeDataGridView.CellDoubleClick += FinanceDataGridView_CellDoubleClick;
            financeDataGridView.CellFormatting += FinanceDataGridView_CellFormatting;
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

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addFinanceButton = CreateActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editFinanceButton = CreateActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewFinanceButton = CreateActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteFinanceButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            editFinanceButton.Enabled = false;
            viewFinanceButton.Enabled = false;
            deleteFinanceButton.Enabled = false;

            addFinanceButton.Click += AddFinanceButton_Click;
            editFinanceButton.Click += EditFinanceButton_Click;
            viewFinanceButton.Click += ViewFinanceButton_Click;
            deleteFinanceButton.Click += DeleteFinanceButton_Click;

            buttonsPanel.Controls.Add(addFinanceButton);
            buttonsPanel.Controls.Add(editFinanceButton);
            buttonsPanel.Controls.Add(viewFinanceButton);
            buttonsPanel.Controls.Add(deleteFinanceButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            totalLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Chọn dự án để xem chi tiết tài chính"
            };

            statsPanel.Controls.Add(totalLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 4);
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
        #endregion
    }
}