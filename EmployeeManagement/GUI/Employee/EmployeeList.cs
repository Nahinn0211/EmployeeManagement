using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.GUI.Projects;
using EmployeeManagement.Models;

namespace EmployeeManagement.GUI.Employee
{
    public partial class EmployeeListForm : Form
    {
        #region Fields
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;
        private Label titleLabel;
        private TextBox searchTextBox;
        private ComboBox statusComboBox;
        private ComboBox departmentComboBox;
        private Button searchButton;
        private Button clearButton;
        private DataGridView employeeDataGridView;
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Label statisticsLabel;

        private List<Models.Employee> employees;
        private List<Models.Employee> filteredEmployees;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên nhân viên, mã nhân viên...";
        #endregion

        #region Constructor
        public EmployeeListForm()
        {
            InitializeComponent();
            InitializeLayout();
            InitializeData();
            LoadEmployees();
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Nhân viên";
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

            // Define row heights
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
                Text = "👥 QUẢN LÝ NHÂN VIÊN",
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

            // Search controls container
            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Department filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Clear button

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
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Đang làm việc", "Tạm nghỉ", "Đã nghỉ việc" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Department ComboBox
            departmentComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            departmentComboBox.Items.Add("Tất cả phòng ban");
            departmentComboBox.Items.AddRange(new[] { "Ban giám đốc", "Phòng Nhân sự", "Phòng Kế toán", "Phòng IT", "Phòng Kinh doanh" });
            departmentComboBox.SelectedIndex = 0;
            departmentComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(departmentComboBox, 2, 0);
            searchContainer.Controls.Add(searchButton, 3, 0);
            searchContainer.Controls.Add(clearButton, 4, 0);

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

            employeeDataGridView = new DataGridView
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

            gridPanel.Controls.Add(employeeDataGridView);
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

            addButton = CreateActionButton("➕ THÊM NHÂN VIÊN", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

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
        #endregion

        #region Control Helpers
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
            searchTextBox.GotFocus += (s, e) => {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) => {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };
        }

        private void SetupDataGridStyles()
        {
            employeeDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            employeeDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
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

            employeeDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            employeeDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "EmployeeCode", HeaderText = "Mã nhân viên", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "FullName", HeaderText = "Họ và tên", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Gender", HeaderText = "Giới tính", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "DateOfBirth", HeaderText = "Ngày sinh", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "Phone", HeaderText = "Điện thoại", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Email", HeaderText = "Email", Width = 180, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Department", HeaderText = "Phòng ban", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Position", HeaderText = "Chức vụ", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter },
                new { Name = "HireDate", HeaderText = "Ngày vào làm", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter }
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
                    DefaultCellStyle = { Alignment = col.Alignment }
                };

                if (col.Name == "DateOfBirth" || col.Name == "HireDate")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                employeeDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            employeeDataGridView.SelectionChanged += (s, e) => {
                bool hasSelection = employeeDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            employeeDataGridView.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    ViewEmployee();
            };

            employeeDataGridView.CellFormatting += EmployeeDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddEmployee();
            editButton.Click += (s, e) => EditEmployee();
            viewButton.Click += (s, e) => ViewEmployee();
            deleteButton.Click += (s, e) => DeleteEmployee();
        }
        #endregion

        #region Data Management
        private void InitializeData()
        {
            employees = new List<Models.Employee>
            {
                new Models.Employee
                {
                    EmployeeID = 1,
                    EmployeeCode = "NV001",
                    FirstName = "Nguyễn",
                    LastName = "Văn A",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(1985, 5, 10),
                    Phone = "0912345678",
                    Email = "nguyenvana@example.com",
                    DepartmentID = 1,
                    PositionID = 1,
                    Status = "Đang làm việc",
                    HireDate = new DateTime(2020, 1, 1),
                    Address = "Hà Nội",
                    IDCardNumber = "123456789",
                    CreatedAt = DateTime.Now.AddDays(-500)
                },
                new Models.Employee
                {
                    EmployeeID = 2,
                    EmployeeCode = "NV002",
                    FirstName = "Trần",
                    LastName = "Thị B",
                    Gender = "Nữ",
                    DateOfBirth = new DateTime(1988, 10, 15),
                    Phone = "0987654321",
                    Email = "tranthib@example.com",
                    DepartmentID = 2,
                    PositionID = 2,
                    Status = "Đang làm việc",
                    HireDate = new DateTime(2020, 2, 1),
                    Address = "Hà Nội",
                    IDCardNumber = "987654321",
                    CreatedAt = DateTime.Now.AddDays(-450)
                },
                new Models.Employee
                {
                    EmployeeID = 3,
                    EmployeeCode = "NV003",
                    FirstName = "Lê",
                    LastName = "Văn C",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(1990, 3, 20),
                    Phone = "0923456789",
                    Email = "levanc@example.com",
                    DepartmentID = 3,
                    PositionID = 2,
                    Status = "Đang làm việc",
                    HireDate = new DateTime(2020, 3, 1),
                    Address = "Hải Phòng",
                    IDCardNumber = "456789123",
                    CreatedAt = DateTime.Now.AddDays(-400)
                },
                new Models.Employee
                {
                    EmployeeID = 4,
                    EmployeeCode = "NV004",
                    FirstName = "Phạm",
                    LastName = "Thị D",
                    Gender = "Nữ",
                    DateOfBirth = new DateTime(1992, 7, 25),
                    Phone = "0934567891",
                    Email = "phamthid@example.com",
                    DepartmentID = 4,
                    PositionID = 2,
                    Status = "Tạm nghỉ",
                    HireDate = new DateTime(2020, 4, 1),
                    Address = "Đà Nẵng",
                    IDCardNumber = "789123456",
                    CreatedAt = DateTime.Now.AddDays(-350)
                },
                new Models.Employee
                {
                    EmployeeID = 5,
                    EmployeeCode = "NV005",
                    FirstName = "Hoàng",
                    LastName = "Văn E",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(1995, 12, 30),
                    Phone = "0945678912",
                    Email = "hoangvane@example.com",
                    DepartmentID = 5,
                    PositionID = 2,
                    Status = "Đã nghỉ việc",
                    HireDate = new DateTime(2020, 5, 1),
                    EndDate = new DateTime(2023, 8, 15),
                    Address = "Hồ Chí Minh",
                    IDCardNumber = "321654987",
                    CreatedAt = DateTime.Now.AddDays(-300)
                }
            };

            filteredEmployees = new List<Models.Employee>(employees);
        }

        private void LoadEmployees()
        {
            try
            {
                var dataSource = filteredEmployees.Select(e => new EmployeeDisplayModel
                {
                    EmployeeCode = e.EmployeeCode,
                    FullName = $"{e.FirstName} {e.LastName}",
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    Phone = e.Phone,
                    Email = e.Email,
                    Department = GetDepartmentName(e.DepartmentID),
                    Position = GetPositionName(e.PositionID),
                    Status = GetStatusDisplayText(e.Status),
                    HireDate = e.HireDate
                }).ToList();

                employeeDataGridView.DataSource = dataSource;
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
                string statusFilter = statusComboBox.SelectedIndex == 0 ? "" : statusComboBox.Text;
                string departmentFilter = departmentComboBox.SelectedIndex == 0 ? "" : departmentComboBox.Text;

                filteredEmployees = employees.Where(e =>
                    (string.IsNullOrEmpty(searchText) ||
                     $"{e.FirstName} {e.LastName}".ToLower().Contains(searchText) ||
                     e.EmployeeCode.ToLower().Contains(searchText) ||
                     e.Email.ToLower().Contains(searchText) ||
                     e.Phone.ToLower().Contains(searchText)) &&
                    (string.IsNullOrEmpty(statusFilter) || e.Status == statusFilter) &&
                    (string.IsNullOrEmpty(departmentFilter) || GetDepartmentName(e.DepartmentID) == departmentFilter)
                ).ToList();

                LoadEmployees();
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
            departmentComboBox.SelectedIndex = 0;
            filteredEmployees = new List<Models.Employee>(employees);
            LoadEmployees();
        }

        private void UpdateStatistics()
        {
            var total = filteredEmployees.Count;
            var active = filteredEmployees.Count(e => e.Status == "Đang làm việc");
            var onLeave = filteredEmployees.Count(e => e.Status == "Tạm nghỉ");
            var inactive = filteredEmployees.Count(e => e.Status == "Đã nghỉ việc");

            statisticsLabel.Text = $"📊 Tổng: {total} | 👤 Đang làm việc: {active} | ⏸️ Tạm nghỉ: {onLeave} | 🚫 Đã nghỉ việc: {inactive}";
        }
        #endregion

        #region Helper Methods
        private string GetDepartmentName(int departmentId)
        {
            return departmentId switch
            {
                1 => "Ban giám đốc",
                2 => "Phòng Nhân sự",
                3 => "Phòng Kế toán",
                4 => "Phòng IT",
                5 => "Phòng Kinh doanh",
                _ => "Không xác định"
            };
        }

        private string GetPositionName(int positionId)
        {
            return positionId switch
            {
                1 => "Giám đốc",
                2 => "Trưởng phòng",
                3 => "Nhân viên cấp cao",
                4 => "Nhân viên",
                5 => "Thực tập sinh",
                _ => "Không xác định"
            };
        }

        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Đang làm việc" => "👤 Đang làm việc",
                "Tạm nghỉ" => "⏸️ Tạm nghỉ",
                "Đã nghỉ việc" => "🚫 Đã nghỉ việc",
                _ => status
            };
        }

        private Models.Employee GetSelectedEmployee()
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = employeeDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is EmployeeDisplayModel displayModel)
                {
                    return employees.FirstOrDefault(e => e.EmployeeCode == displayModel.EmployeeCode);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void EmployeeDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = employeeDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    string s when s.Contains("Đang làm việc") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Tạm nghỉ") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Đã nghỉ việc") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "Gender" && e.Value != null)
            {
                var gender = e.Value.ToString();
                if (gender == "Nam")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(33, 150, 243);
                }
                else if (gender == "Nữ")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(233, 30, 99);
                }
            }
        }

        private void AddEmployee()
        {
            try
            {
                using (var form = new EmployeeCreate())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Thêm nhân viên mới vào danh sách
                        var newEmployee = form.CreatedEmployee;
                        newEmployee.EmployeeID = employees.Count + 1; // Assign new ID
                        employees.Add(newEmployee);

                        ApplyFilters(); // Refresh grid
                        MessageBox.Show("Thêm nhân viên thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                using (var form = new EmployeeDetail(employee))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh data after editing
                        ApplyFilters();
                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                using (var form = new EmployeeDetail(employee, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân viên '{employee.FirstName} {employee.LastName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    // Remove the employee
                    employees.Remove(employee);
                    ApplyFilters();
                    MessageBox.Show("Xóa nhân viên thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }

    #region Display Models
    public class EmployeeDisplayModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }
    #endregion
}