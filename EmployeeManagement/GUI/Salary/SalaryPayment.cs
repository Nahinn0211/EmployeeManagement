using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Salary
{
    public partial class SalaryPaymentForm : Form
    {
        #region Fields
        private SalaryBLL salaryBLL;
        private List<Models.Salary> selectedSalaries;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private Label summaryLabel;

        // Content controls
        private DataGridView salaryDataGridView;
        private GroupBox paymentGroupBox;
        private ComboBox paymentStatusComboBox;
        private DateTimePicker paymentDatePicker;
        private CheckBox setPaymentDateCheckBox;
        private TextBox notesTextBox;

        // Footer controls
        private Button applyButton;
        private Button cancelButton;
        #endregion

        #region Constructor
        public SalaryPaymentForm(List<Models.Salary> salaries)
        {
            InitializeComponent();
            salaryBLL = new SalaryBLL();
            selectedSalaries = salaries ?? throw new ArgumentNullException(nameof(salaries));
            SetupForm();
            LoadSalariesToGrid();
            UpdateSummary();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Quản lý Thanh toán Lương";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            SetupLayout();
            SetupHeader();
            SetupContent();
            SetupFooter();
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

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            titleLabel = new Label
            {
                Text = "💳 QUẢN LÝ THANH TOÁN LƯƠNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 10),
                Size = new Size(600, 30),
                AutoEllipsis = true
            };

            summaryLabel = new Label
            {
                Text = "Đang tải...",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(20, 45),
                Size = new Size(800, 25)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(summaryLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Grid
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Payment controls

            // Setup grid panel
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var gridTitle = new Label
            {
                Text = "📋 Danh sách bảng lương được chọn",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            salaryDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 30 }
            };

            SetupDataGridColumns();
            SetupDataGridStyles();

            gridPanel.Controls.Add(salaryDataGridView);
            gridPanel.Controls.Add(gridTitle);

            // Setup payment controls panel
            SetupPaymentControls(out var paymentPanel);

            contentLayout.Controls.Add(gridPanel, 0, 0);
            contentLayout.Controls.Add(paymentPanel, 1, 0);

            contentPanel.Controls.Add(contentLayout);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupPaymentControls(out Panel paymentPanel)
        {
            paymentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };

            paymentGroupBox = new GroupBox
            {
                Text = "💳 Thông tin thanh toán",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Padding = new Padding(15)
            };

            var paymentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(10)
            };

            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // Status
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Date
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Notes
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Info
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Warning

            // Payment Status
            var statusPanel = CreateInputPanel("Trạng thái thanh toán:", out var statusLabel);
            paymentStatusComboBox = new ComboBox
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30
            };
            paymentStatusComboBox.Items.AddRange(SalaryConstants.PaymentStatuses);
            paymentStatusComboBox.SelectedIndexChanged += PaymentStatusComboBox_SelectedIndexChanged;
            statusPanel.Controls.Add(paymentStatusComboBox);

            // Payment Date
            var datePanel = CreateInputPanel("Ngày thanh toán:", out var dateLabel);

            setPaymentDateCheckBox = new CheckBox
            {
                Text = "Đặt ngày thanh toán",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            setPaymentDateCheckBox.CheckedChanged += SetPaymentDateCheckBox_CheckedChanged;

            paymentDatePicker = new DateTimePicker
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Height = 30,
                Enabled = false
            };

            datePanel.Controls.Add(paymentDatePicker);
            datePanel.Controls.Add(setPaymentDateCheckBox);

            // Notes
            var notesPanel = CreateInputPanel("Ghi chú (tùy chọn):", out var notesLabel);
            notesTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "Nhập ghi chú về thanh toán..."
            };
            notesPanel.Controls.Add(notesTextBox);

            // Info panel
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var infoLabel = new Label
            {
                Text = "💡 Thay đổi sẽ áp dụng cho tất cả bảng lương được chọn",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(33, 150, 243),
                TextAlign = ContentAlignment.MiddleLeft
            };

            infoPanel.Controls.Add(infoLabel);

            // Warning panel
            var warningPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 248, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var warningLabel = new Label
            {
                Text = "⚠️ Hành động này không thể hoàn tác",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 152, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            warningPanel.Controls.Add(warningLabel);

            paymentLayout.Controls.Add(statusPanel, 0, 0);
            paymentLayout.Controls.Add(datePanel, 0, 1);
            paymentLayout.Controls.Add(notesPanel, 0, 2);
            paymentLayout.Controls.Add(infoPanel, 0, 3);
            paymentLayout.Controls.Add(warningPanel, 0, 4);

            paymentGroupBox.Controls.Add(paymentLayout);
            paymentPanel.Controls.Add(paymentGroupBox);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(25, 15, 25, 15)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            applyButton = new Button
            {
                Text = "💾 Áp dụng thay đổi",
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            applyButton.Click += ApplyButton_Click;

            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Size = new Size(100, 45),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(applyButton);
            buttonPanel.Controls.Add(cancelButton);

            footerPanel.Controls.Add(buttonPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }
        #endregion

        #region Helper Methods
        private Panel CreateInputPanel(string labelText, out Label label)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White
            };

            label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            panel.Controls.Add(label);
            return panel;
        }

        private void SetupDataGridColumns()
        {
            salaryDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "EmployeeCode", HeaderText = "Mã NV", Width = 80 },
                new { Name = "EmployeeName", HeaderText = "Tên nhân viên", Width = 150 },
                new { Name = "MonthYear", HeaderText = "Tháng/Năm", Width = 100 },
                new { Name = "NetSalary", HeaderText = "Lương thực nhận", Width = 120 },
                new { Name = "CurrentStatus", HeaderText = "Trạng thái hiện tại", Width = 130 }
            };

            foreach (var col in columns)
            {
                salaryDataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width
                });
            }
        }

        private void SetupDataGridStyles()
        {
            salaryDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Font = new Font("Segoe UI", 9)
            };

            salaryDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
        }

        private void LoadSalariesToGrid()
        {
            try
            {
                var dataSource = selectedSalaries.Select(s => new
                {
                    EmployeeCode = s.Employee?.EmployeeCode ?? "",
                    EmployeeName = s.Employee?.FullName ?? "",
                    MonthYear = s.MonthYearDisplay,
                    NetSalary = s.NetSalaryDisplay,
                    CurrentStatus = s.PaymentStatusDisplay
                }).ToList();

                salaryDataGridView.DataSource = dataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            int totalSalaries = selectedSalaries.Count;
            decimal totalAmount = selectedSalaries.Sum(s => s.NetSalary);
            var statusGroups = selectedSalaries.GroupBy(s => s.PaymentStatus)
                                              .ToDictionary(g => g.Key, g => g.Count());

            summaryLabel.Text = $"Đã chọn {totalSalaries} bảng lương | " +
                               $"Tổng tiền: {totalAmount:#,##0} VNĐ | " +
                               $"Chưa thanh toán: {statusGroups.GetValueOrDefault("Chưa thanh toán", 0)} | " +
                               $"Đã thanh toán: {statusGroups.GetValueOrDefault("Đã thanh toán", 0)}";
        }
        #endregion

        #region Event Handlers
        private void PaymentStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paymentStatusComboBox.Text == "Đã thanh toán")
            {
                setPaymentDateCheckBox.Checked = true;
                paymentDatePicker.Value = DateTime.Now;
            }
        }

        private void SetPaymentDateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            paymentDatePicker.Enabled = setPaymentDateCheckBox.Checked;
            if (setPaymentDateCheckBox.Checked)
            {
                paymentDatePicker.Value = DateTime.Now;
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Bạn có chắc chắn muốn cập nhật trạng thái thanh toán cho {selectedSalaries.Count} bảng lương?\n\n" +
                        $"Trạng thái mới: {paymentStatusComboBox.Text}\n" +
                        $"Ngày thanh toán: {(setPaymentDateCheckBox.Checked ? paymentDatePicker.Value.ToString("dd/MM/yyyy") : "Không đặt")}\n\n" +
                        "Hành động này không thể hoàn tác!",
                        "Xác nhận cập nhật",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.Yes)
                    {
                        ApplyPaymentChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi áp dụng thay đổi: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (paymentStatusComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn trạng thái thanh toán!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (setPaymentDateCheckBox.Checked && paymentDatePicker.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày thanh toán không được lớn hơn ngày hiện tại!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ApplyPaymentChanges()
        {
            try
            {
                string newStatus = paymentStatusComboBox.Text;
                DateTime? paymentDate = setPaymentDateCheckBox.Checked ? paymentDatePicker.Value : null;
                string notes = notesTextBox.Text.Trim();

                int successCount = 0;
                var errors = new List<string>();

                foreach (var salary in selectedSalaries)
                {
                    try
                    {
                        // Update salary object
                        salary.PaymentStatus = newStatus;
                        if (paymentDate.HasValue)
                            salary.PaymentDate = paymentDate.Value;
                        else if (newStatus == "Chưa thanh toán")
                            salary.PaymentDate = null;

                        if (!string.IsNullOrEmpty(notes))
                        {
                            salary.Notes = string.IsNullOrEmpty(salary.Notes)
                                ? notes
                                : salary.Notes + "\n" + notes;
                        }

                        // Update in database
                        if (salaryBLL.UpdateSalary(salary))
                        {
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Không thể cập nhật {salary.Employee?.EmployeeCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi khi cập nhật {salary.Employee?.EmployeeCode}: {ex.Message}");
                    }
                }

                // Show results
                string message = $"Cập nhật thành công {successCount}/{selectedSalaries.Count} bảng lương.";
                if (errors.Count > 0)
                {
                    message += $"\n\nLỗi:\n{string.Join("\n", errors.Take(5))}";
                    if (errors.Count > 5)
                        message += $"\n... và {errors.Count - 5} lỗi khác";
                }

                MessageBox.Show(message,
                    successCount > 0 ? "Cập nhật hoàn tất" : "Lỗi",
                    MessageBoxButtons.OK,
                    successCount > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (successCount > 0)
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thực hiện cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}