using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.Entity;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.GUI.Reports
{
    public partial class HRReportForm : Form
    {
        #region Fields
        private readonly HRReportBLL hrReportBLL;
        private readonly EmployeeBLL employeeBLL;
        private DashboardMetricsDTO currentMetrics;
        private List<EmployeeReportDTO> currentEmployeeData;
        #endregion

        #region Constructor
        public HRReportForm()
        {
            hrReportBLL = new HRReportBLL();
            employeeBLL = new EmployeeBLL();
            InitializeComponent();
        }
        #endregion

        #region Form Events
        /// <summary>
        /// Form Load Event - Initialize data
        /// </summary>
        private async void HRReportForm_Load(object sender, EventArgs e)
        {
            await LoadInitialData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Logger.LogInfo("HRReportForm opened");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Logger.LogInfo("HRReportForm closed");
        }
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Load initial data when form opens
        /// </summary>
        private async System.Threading.Tasks.Task LoadInitialData()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Initialize combo boxes with default values
                InitializeComboBoxes();

                // Load combo box data from database
                await LoadComboBoxData();

                // Load dashboard data
                await LoadDashboardData();

                Logger.LogInfo("HRReportForm loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading HRReportForm: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Initialize combo boxes with default values
        /// </summary>
        private void InitializeComboBoxes()
        {
            try
            {
                // Initialize gender combo box
                cmbGender.Items.Clear();
                cmbGender.Items.AddRange(new[] { "Tất cả", "Nam", "Nữ" });
                cmbGender.SelectedIndex = 0;

                // Initialize birthday month combo box
                cmbBirthdayMonth.Items.Clear();
                for (int i = 1; i <= 12; i++)
                    cmbBirthdayMonth.Items.Add($"Tháng {i}");
                cmbBirthdayMonth.SelectedIndex = DateTime.Now.Month - 1;

                // Initialize birthday year combo box
                cmbBirthdayYear.Items.Clear();
                for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year + 1; i++)
                    cmbBirthdayYear.Items.Add(i.ToString());
                cmbBirthdayYear.SelectedItem = DateTime.Now.Year.ToString();

                // Initialize attendance month combo box
                cmbAttendanceMonth.Items.Clear();
                for (int i = 1; i <= 12; i++)
                    cmbAttendanceMonth.Items.Add($"Tháng {i}");
                cmbAttendanceMonth.SelectedIndex = DateTime.Now.Month - 1;

                // Initialize attendance year combo box
                cmbAttendanceYear.Items.Clear();
                for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year; i++)
                    cmbAttendanceYear.Items.Add(i.ToString());
                cmbAttendanceYear.SelectedItem = DateTime.Now.Year.ToString();

                // Set default date range for hiring date filter
                dtpHireFrom.Value = DateTime.Now.AddYears(-10);
                dtpHireTo.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error initializing combo boxes: {ex.Message}");
            }
        }

        /// <summary>
        /// Load combo box data from database
        /// </summary>
        private async System.Threading.Tasks.Task LoadComboBoxData()
        {
            try
            {
                // Load departments from database
                var departments = await System.Threading.Tasks.Task.Run(() => employeeBLL.GetAllDepartments());
                cmbDepartment.Items.Clear();
                cmbDepartment.Items.Add(new { Text = "Tất cả", Value = -1 });
                foreach (var dept in departments)
                {
                    cmbDepartment.Items.Add(new { Text = dept.DepartmentName, Value = dept.DepartmentID });
                }
                cmbDepartment.DisplayMember = "Text";
                cmbDepartment.ValueMember = "Value";
                cmbDepartment.SelectedIndex = 0;

                // Load positions from database
                var positions = await System.Threading.Tasks.Task.Run(() => employeeBLL.GetAllPositions());
                cmbPosition.Items.Clear();
                cmbPosition.Items.Add(new { Text = "Tất cả", Value = -1 });
                foreach (var pos in positions)
                {
                    cmbPosition.Items.Add(new { Text = pos.PositionName, Value = pos.PositionID });
                }
                cmbPosition.DisplayMember = "Text";
                cmbPosition.ValueMember = "Value";
                cmbPosition.SelectedIndex = 0;

                // Load status options
                cmbStatus.Items.Clear();
                cmbStatus.Items.AddRange(new[] { "Tất cả", "Đang làm việc", "Đã nghỉ việc", "Tạm nghỉ" });
                cmbStatus.SelectedIndex = 0;

                Logger.LogInfo("Loaded combo box data from database successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading combo box data: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu combo box: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Dashboard Methods
        /// <summary>
        /// Load dashboard data and create visualizations
        /// </summary>
        private async System.Threading.Tasks.Task LoadDashboardData()
        {
            try
            {
                currentMetrics = await System.Threading.Tasks.Task.Run(() =>
                {
                    var stats = hrReportBLL.GetHRStatistics();
                    var employees = hrReportBLL.GetEmployeeReports();
                    var departments = hrReportBLL.GetDepartmentReports();
                    var birthdays = hrReportBLL.GetBirthdayReports(DateTime.Now.Month, DateTime.Now.Year);

                    return new DashboardMetricsDTO
                    {
                        GeneralStats = new HRStatisticsDTO
                        {
                            TotalEmployees = stats.TotalEmployees,
                            ActiveEmployees = stats.ActiveEmployees,
                            InactiveEmployees = stats.InactiveEmployees,
                            NewHires = stats.NewHires,
                            Resignations = stats.Resignations,
                            TurnoverRate = stats.TurnoverRate,
                            AverageAge = stats.AverageAge,
                            AverageWorkingYears = stats.AverageWorkingYears,
                            AverageSalary = stats.AverageSalary,
                            AverageAttendanceRate = stats.AverageAttendanceRate,
                            AveragePerformanceScore = stats.AveragePerformanceScore,
                            MaleCount = stats.MaleCount,
                            FemaleCount = stats.FemaleCount
                        },
                        TopPerformers = employees.OrderByDescending(e => e.PerformanceScore).Take(5)
                            .Select(e => new EmployeeReportDTO
                            {
                                EmployeeID = e.EmployeeID,
                                EmployeeCode = e.EmployeeCode,
                                FullName = e.FullName,
                                Department = e.Department,
                                Position = e.Position,
                                PerformanceScore = e.PerformanceScore
                            }).ToList(),
                        DepartmentBreakdown = departments.Take(5).Select(d => new DepartmentReportDTO
                        {
                            DepartmentID = d.DepartmentID,
                            DepartmentName = d.DepartmentName,
                            TotalEmployees = d.TotalEmployees,
                            EmployeePercentage = d.EmployeePercentage
                        }).ToList(),
                        UpcomingBirthdays = birthdays.Take(10).Select(b => new BirthdayReportDTO
                        {
                            EmployeeID = b.EmployeeID,
                            EmployeeCode = b.EmployeeCode,
                            FullName = b.FullName,
                            Department = b.Department,
                            DateOfBirth = b.DateOfBirth,
                            DaysUntilBirthday = b.DaysUntilBirthday
                        }).ToList()
                    };
                });

                CreateStatsCards(currentMetrics);
                CreateDashboardCharts();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading dashboard data: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create statistics cards on dashboard
        /// </summary>
        private void CreateStatsCards(DashboardMetricsDTO metrics)
        {
            try
            {
                statsPanel.Controls.Clear();

                var stats = new[]
                {
                    new { Title = "Tổng nhân viên", Value = metrics.GeneralStats.TotalEmployees.ToString(), Color = Color.FromArgb(24, 144, 255) },
                    new { Title = "Đang làm việc", Value = metrics.GeneralStats.ActiveEmployees.ToString(), Color = Color.FromArgb(82, 196, 26) },
                    new { Title = "Nhân viên mới", Value = metrics.GeneralStats.NewHires.ToString(), Color = Color.FromArgb(250, 140, 22) },
                    new { Title = "Tỷ lệ nghỉ việc", Value = $"{metrics.GeneralStats.TurnoverRate:F1}%", Color = Color.FromArgb(245, 34, 45) },
                    new { Title = "Tuổi TB", Value = $"{metrics.GeneralStats.AverageAge:F0}", Color = Color.FromArgb(114, 46, 209) },
                    new { Title = "Lương TB", Value = $"{metrics.GeneralStats.AverageSalary:N0}", Color = Color.FromArgb(19, 194, 194) }
                };

                for (int i = 0; i < stats.Length; i++)
                {
                    var card = CreateStatsCard(stats[i].Title, stats[i].Value, stats[i].Color);
                    card.Location = new Point(20 + (i * 220), 20);
                    statsPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating stats cards: {ex.Message}");
            }
        }

        /// <summary>
        /// Create individual stats card
        /// </summary>
        private Panel CreateStatsCard(string title, string value, Color color)
        {
            var card = new Panel
            {
                Size = new Size(200, 160),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var colorBar = new Panel
            {
                Size = new Size(200, 4),
                BackColor = color,
                Dock = DockStyle.Top
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(15, 20),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 50),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { colorBar, titleLabel, valueLabel });
            return card;
        }

        /// <summary>
        /// Create dashboard charts and panels
        /// </summary>
        private void CreateDashboardCharts()
        {
            try
            {
                chartsPanel.Controls.Clear();

                // Top performers panel
                var topPerformersPanel = CreateTopPerformersPanel();
                topPerformersPanel.Location = new Point(20, 20);
                topPerformersPanel.Size = new Size(400, 300);

                // Department breakdown panel
                var departmentPanel = CreateDepartmentBreakdownPanel();
                departmentPanel.Location = new Point(450, 20);
                departmentPanel.Size = new Size(400, 300);

                // Upcoming birthdays panel
                var birthdaysPanel = CreateUpcomingBirthdaysPanel();
                birthdaysPanel.Location = new Point(20, 340);
                birthdaysPanel.Size = new Size(830, 200);

                chartsPanel.Controls.AddRange(new Control[] { topPerformersPanel, departmentPanel, birthdaysPanel });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating dashboard charts: {ex.Message}");
            }
        }

        /// <summary>
        /// Create top performers panel
        /// </summary>
        private Panel CreateTopPerformersPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = "TOP NHÂN VIÊN XUẤT SẮC",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 158, 255),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(15, 50),
                Size = new Size(370, 230),
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("STT", 40);
            listView.Columns.Add("Tên nhân viên", 180);
            listView.Columns.Add("Phòng ban", 100);
            listView.Columns.Add("Điểm", 50);

            if (currentMetrics?.TopPerformers != null)
            {
                for (int i = 0; i < currentMetrics.TopPerformers.Count; i++)
                {
                    var emp = currentMetrics.TopPerformers[i];
                    var item = new ListViewItem((i + 1).ToString());
                    item.SubItems.Add(emp.FullName);
                    item.SubItems.Add(emp.Department);
                    item.SubItems.Add(emp.PerformanceScore.ToString("F1"));
                    listView.Items.Add(item);
                }
            }

            panel.Controls.AddRange(new Control[] { titleLabel, listView });
            return panel;
        }

        /// <summary>
        /// Create department breakdown panel
        /// </summary>
        private Panel CreateDepartmentBreakdownPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = "PHÂN BỐ THEO PHÒNG BAN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 158, 255),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(15, 50),
                Size = new Size(370, 230),
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("Phòng ban", 200);
            listView.Columns.Add("Số NV", 80);
            listView.Columns.Add("Tỷ lệ", 70);

            if (currentMetrics?.DepartmentBreakdown != null)
            {
                foreach (var dept in currentMetrics.DepartmentBreakdown)
                {
                    var item = new ListViewItem(dept.DepartmentName);
                    item.SubItems.Add(dept.TotalEmployees.ToString());
                    item.SubItems.Add($"{dept.EmployeePercentage:F1}%");
                    listView.Items.Add(item);
                }
            }

            panel.Controls.AddRange(new Control[] { titleLabel, listView });
            return panel;
        }

        /// <summary>
        /// Create upcoming birthdays panel
        /// </summary>
        private Panel CreateUpcomingBirthdaysPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = "SINH NHẬT SẮP TỚI",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 158, 255),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(15, 50),
                Size = new Size(800, 130),
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("Nhân viên", 200);
            listView.Columns.Add("Phòng ban", 150);
            listView.Columns.Add("Ngày sinh", 100);
            listView.Columns.Add("Trạng thái", 120);

            if (currentMetrics?.UpcomingBirthdays != null)
            {
                foreach (var birthday in currentMetrics.UpcomingBirthdays)
                {
                    var item = new ListViewItem(birthday.FullName);
                    item.SubItems.Add(birthday.Department);
                    item.SubItems.Add(birthday.DateOfBirth.ToString("dd/MM"));
                    item.SubItems.Add(birthday.BirthdayStatus);

                    if (birthday.DaysUntilBirthday == 0)
                        item.BackColor = Color.LightYellow;
                    else if (birthday.DaysUntilBirthday <= 7)
                        item.BackColor = Color.LightBlue;

                    listView.Items.Add(item);
                }
            }

            panel.Controls.AddRange(new Control[] { titleLabel, listView });
            return panel;
        }
        #endregion

        #region Employee Reports Methods
        /// <summary>
        /// Load employee reports with optional filter
        /// </summary>
        private async System.Threading.Tasks.Task LoadEmployeeReports(HRReportFilterDTO filter = null)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var filterModel = filter != null ? new HRReportFilter
                {
                    DepartmentID = filter.DepartmentID,
                    PositionID = filter.PositionID,
                    Gender = filter.Gender,
                    Status = filter.Status,
                    MinAge = filter.MinAge,
                    MaxAge = filter.MaxAge,
                    HireDateFrom = filter.HireDateFrom,
                    HireDateTo = filter.HireDateTo,
                    MinSalary = filter.MinSalary,
                    MaxSalary = filter.MaxSalary,
                    SearchKeyword = filter.SearchKeyword,
                    SortBy = filter.SortBy,
                    SortDescending = filter.SortDescending
                } : null;

                var employees = await System.Threading.Tasks.Task.Run(() => hrReportBLL.GetEmployeeReports(filterModel));

                currentEmployeeData = employees.Select(e => new EmployeeReportDTO
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Gender = e.Gender,
                    Age = e.Age,
                    Department = e.Department,
                    Position = e.Position,
                    Manager = e.Manager,
                    HireDate = e.HireDate,
                    WorkingDays = e.WorkingDays,
                    Status = e.Status,
                    Phone = e.Phone,
                    Email = e.Email,
                    CurrentSalary = e.CurrentSalary,
                    TotalProjects = e.TotalProjects,
                    CompletedProjects = e.CompletedProjects,
                    AttendanceRate = e.AttendanceRate,
                    PerformanceScore = e.PerformanceScore
                }).ToList();

                dgvEmployees.DataSource = currentEmployeeData;
                ConfigureEmployeeGrid();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading employee reports: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải báo cáo nhân viên: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configure employee grid columns and formatting
        /// </summary>
        private void ConfigureEmployeeGrid()
        {
            if (dgvEmployees.Columns.Count == 0) return;

            // FIX: Kiểm tra column tồn tại trước khi cấu hình
            SafeConfigureColumn(dgvEmployees, "EmployeeID", visible: false);
            SafeConfigureColumn(dgvEmployees, "EmployeeCode", "Mã NV", 80);
            SafeConfigureColumn(dgvEmployees, "FullName", "Họ và tên", 150);
            SafeConfigureColumn(dgvEmployees, "Gender", "Giới tính", 80);
            SafeConfigureColumn(dgvEmployees, "Age", "Tuổi", 60);
            SafeConfigureColumn(dgvEmployees, "Department", "Phòng ban", 120);
            SafeConfigureColumn(dgvEmployees, "Position", "Chức vụ", 120);
            SafeConfigureColumn(dgvEmployees, "Manager", "Quản lý", 120);
            SafeConfigureColumn(dgvEmployees, "HireDate", "Ngày vào làm", 100, "dd/MM/yyyy");
            SafeConfigureColumn(dgvEmployees, "WorkingDays", "Số ngày LV", 90);
            SafeConfigureColumn(dgvEmployees, "WorkingYears", "Số năm LV", 90, "F1");
            SafeConfigureColumn(dgvEmployees, "Status", "Trạng thái", 100);
            SafeConfigureColumn(dgvEmployees, "Phone", "Điện thoại", 100);
            SafeConfigureColumn(dgvEmployees, "Email", "Email", 150);
            SafeConfigureColumn(dgvEmployees, "CurrentSalary", "Lương hiện tại", 100, "N0");
            SafeConfigureColumn(dgvEmployees, "TotalProjects", "Số dự án", 80);
            SafeConfigureColumn(dgvEmployees, "CompletedProjects", "DA hoàn thành", 100);
            SafeConfigureColumn(dgvEmployees, "AttendanceRate", "Tỷ lệ CC (%)", 90, "F1");
            SafeConfigureColumn(dgvEmployees, "PerformanceScore", "Điểm hiệu suất", 100, "F1");

            // Color coding based on status and performance
            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                if (row.Cells["Status"]?.Value?.ToString() == "Đã nghỉ việc")
                {
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                }
                else if (row.Cells["Status"]?.Value?.ToString() == "Đang làm việc")
                {
                    // FIX: Thêm kiểm tra null và safe cast
                    var attendanceRateValue = row.Cells["AttendanceRate"]?.Value;
                    if (attendanceRateValue != null)
                    {
                        decimal attendanceRate = 0;
                        if (decimal.TryParse(attendanceRateValue.ToString(), out attendanceRate))
                        {
                            if (attendanceRate < 80)
                                row.DefaultCellStyle.BackColor = Color.LightPink;
                            else if (attendanceRate >= 95)
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper method để cấu hình column một cách an toàn
        /// </summary>
        private void SafeConfigureColumn(DataGridView grid, string columnName, string headerText = null, int width = -1, string format = null, bool? visible = null)
        {
            if (grid.Columns.Contains(columnName))
            {
                var column = grid.Columns[columnName];

                if (visible.HasValue)
                    column.Visible = visible.Value;

                if (!string.IsNullOrEmpty(headerText))
                    column.HeaderText = headerText;

                if (width > 0)
                    column.Width = width;

                if (!string.IsNullOrEmpty(format))
                    column.DefaultCellStyle.Format = format;
            }
        }
        #endregion

        #region Department Reports Methods
        /// <summary>
        /// Load department reports
        /// </summary>
        private async System.Threading.Tasks.Task LoadDepartmentReports()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var departments = await System.Threading.Tasks.Task.Run(() => hrReportBLL.GetDepartmentReports());

                var departmentData = departments.Select(d => new DepartmentReportDTO
                {
                    DepartmentID = d.DepartmentID,
                    DepartmentName = d.DepartmentName,
                    ManagerName = d.ManagerName,
                    TotalEmployees = d.TotalEmployees,
                    ActiveEmployees = d.ActiveEmployees,
                    MaleEmployees = d.MaleEmployees,
                    FemaleEmployees = d.FemaleEmployees,
                    EmployeePercentage = d.EmployeePercentage,
                    AverageAge = d.AverageAge,
                    AverageWorkingYears = d.AverageWorkingYears,
                    AverageSalary = d.AverageSalary,
                    TotalSalaryCost = d.TotalSalaryCost
                }).ToList();

                dgvDepartments.DataSource = departmentData;
                ConfigureDepartmentGrid();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading department reports: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải báo cáo phòng ban: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configure department grid columns
        /// </summary>
        private void ConfigureDepartmentGrid()
        {
            if (dgvDepartments.Columns.Count == 0) return;

            // FIX: Sử dụng SafeConfigureColumn
            SafeConfigureColumn(dgvDepartments, "DepartmentID", visible: false);
            SafeConfigureColumn(dgvDepartments, "DepartmentName", "Tên phòng ban", 150);
            SafeConfigureColumn(dgvDepartments, "ManagerName", "Trưởng phòng", 150);
            SafeConfigureColumn(dgvDepartments, "TotalSalaryCost", "Tổng chi phí lương", 120, "N0");
        }
        #endregion

        #region Birthday Reports Methods
        /// <summary>
        /// Load birthday reports for selected month/year
        /// </summary>
        private async System.Threading.Tasks.Task LoadBirthdayReports()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                int month = cmbBirthdayMonth.SelectedIndex + 1;
                int year = int.Parse(cmbBirthdayYear.SelectedItem.ToString());

                var birthdays = await System.Threading.Tasks.Task.Run(() => hrReportBLL.GetBirthdayReports(month, year));

                var birthdayData = birthdays.Select(b => new BirthdayReportDTO
                {
                    EmployeeID = b.EmployeeID,
                    EmployeeCode = b.EmployeeCode,
                    FullName = b.FullName,
                    Department = b.Department,
                    Position = b.Position,
                    DateOfBirth = b.DateOfBirth,
                    Age = b.Age,
                    Phone = b.Phone,
                    Email = b.Email,
                    DaysUntilBirthday = b.DaysUntilBirthday
                }).ToList();

                dgvBirthdays.DataSource = birthdayData;
                ConfigureBirthdayGrid();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading birthday reports: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải danh sách sinh nhật: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configure birthday grid columns and formatting
        /// </summary>
        private void ConfigureBirthdayGrid()
        {
            if (dgvBirthdays.Columns.Count == 0) return;

            // FIX: Sử dụng SafeConfigureColumn
            SafeConfigureColumn(dgvBirthdays, "EmployeeID", visible: false);
            SafeConfigureColumn(dgvBirthdays, "EmployeeCode", "Mã NV", 80);
            SafeConfigureColumn(dgvBirthdays, "FullName", "Họ và tên", 150);
            SafeConfigureColumn(dgvBirthdays, "Department", "Phòng ban", 120);
            SafeConfigureColumn(dgvBirthdays, "Position", "Chức vụ", 120);
            SafeConfigureColumn(dgvBirthdays, "DateOfBirth", "Ngày sinh", 100, "dd/MM/yyyy");
            SafeConfigureColumn(dgvBirthdays, "Age", "Tuổi", 60);
            SafeConfigureColumn(dgvBirthdays, "Phone", "Điện thoại", 100);
            SafeConfigureColumn(dgvBirthdays, "Email", "Email", 150);
            SafeConfigureColumn(dgvBirthdays, "DaysUntilBirthday", "Số ngày còn lại", 100);
            SafeConfigureColumn(dgvBirthdays, "BirthdayStatus", "Trạng thái", 120);

            // Color coding for upcoming birthdays
            foreach (DataGridViewRow row in dgvBirthdays.Rows)
            {
                // FIX: Thêm kiểm tra null và safe cast
                var daysUntilValue = row.Cells["DaysUntilBirthday"]?.Value;
                if (daysUntilValue != null)
                {
                    int daysUntil = 0;
                    if (int.TryParse(daysUntilValue.ToString(), out daysUntil))
                    {
                        if (daysUntil == 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.Gold;
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                            row.DefaultCellStyle.Font = new Font(dgvBirthdays.Font, FontStyle.Bold);
                        }
                        else if (daysUntil <= 7 && daysUntil > 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightBlue;
                        }
                        else if (daysUntil <= 15 && daysUntil > 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
        }
        #endregion

        #region Attendance Reports Methods
        /// <summary>
        /// Load attendance reports for selected month/year
        /// </summary>
        private async System.Threading.Tasks.Task LoadAttendanceReports()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                int month = cmbAttendanceMonth.SelectedIndex + 1;
                int year = int.Parse(cmbAttendanceYear.SelectedItem.ToString());

                var attendance = await System.Threading.Tasks.Task.Run(() => hrReportBLL.GetAttendanceReports(month, year));

                var attendanceData = attendance.Select(a => new AttendanceReportDTO
                {
                    EmployeeID = a.EmployeeID,
                    EmployeeCode = a.EmployeeCode,
                    FullName = a.FullName,
                    Department = a.Department,
                    WorkingDays = a.WorkingDays,
                    PresentDays = a.PresentDays,
                    AbsentDays = a.AbsentDays,
                    LateDays = a.LateDays,
                    TotalWorkingHours = a.TotalWorkingHours,
                    OvertimeHours = a.OvertimeHours
                }).ToList();

                dgvAttendance.DataSource = attendanceData;
                ConfigureAttendanceGrid();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading attendance reports: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải báo cáo chấm công: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configure attendance grid columns and formatting
        /// </summary>
        private void ConfigureAttendanceGrid()
        {
            if (dgvAttendance.Columns.Count == 0) return;

            // FIX: Sử dụng SafeConfigureColumn
            SafeConfigureColumn(dgvAttendance, "EmployeeID", visible: false);
            SafeConfigureColumn(dgvAttendance, "EmployeeCode", "Mã NV", 80);
            SafeConfigureColumn(dgvAttendance, "FullName", "Họ và tên", 150);
            SafeConfigureColumn(dgvAttendance, "Department", "Phòng ban", 120);
            SafeConfigureColumn(dgvAttendance, "WorkingDays", "Ngày làm việc", 100);
            SafeConfigureColumn(dgvAttendance, "PresentDays", "Ngày có mặt", 100);
            SafeConfigureColumn(dgvAttendance, "AbsentDays", "Ngày vắng", 80);
            SafeConfigureColumn(dgvAttendance, "LateDays", "Ngày đi muộn", 100);
            SafeConfigureColumn(dgvAttendance, "TotalWorkingHours", "Tổng giờ LV", 100, "F1");
            SafeConfigureColumn(dgvAttendance, "OvertimeHours", "Giờ tăng ca", 100, "F1");
            SafeConfigureColumn(dgvAttendance, "AttendanceRate", "Tỷ lệ chấm công (%)", 120, "F1");

            // Color coding based on attendance rate
            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                // FIX: Thêm kiểm tra null và safe cast
                var attendanceRateValue = row.Cells["AttendanceRate"]?.Value;
                if (attendanceRateValue != null)
                {
                    decimal attendanceRate = 0;
                    if (decimal.TryParse(attendanceRateValue.ToString(), out attendanceRate))
                    {
                        if (attendanceRate < 70)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightPink;
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        }
                        else if (attendanceRate < 85)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                        else if (attendanceRate >= 95)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle tab selection change
        /// </summary>
        private async void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (tabControl.SelectedTab.Name)
                {
                    case "tabEmployees":
                        if (currentEmployeeData == null)
                            await LoadEmployeeReports();
                        break;
                    case "tabDepartments":
                        await LoadDepartmentReports();
                        break;
                    case "tabBirthdays":
                        await LoadBirthdayReports();
                        break;
                    case "tabAttendance":
                        await LoadAttendanceReports();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in tab change: {ex.Message}");
                MessageBox.Show($"Lỗi khi chuyển tab: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle filter button click
        /// </summary>
        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var filter = new HRReportFilterDTO();

                // Department filter - FIX: kiểm tra null safety tốt hơn
                if (cmbDepartment.SelectedItem != null)
                {
                    try
                    {
                        dynamic selectedDept = cmbDepartment.SelectedItem;
                        var value = selectedDept.Value;

                        // FIX: Safe cast để tránh lỗi InvalidCastException
                        if (value is int intValue && intValue != -1)
                            filter.DepartmentID = intValue;
                        else if (int.TryParse(value?.ToString(), out int parsedValue) && parsedValue != -1)
                            filter.DepartmentID = parsedValue;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"Error parsing department filter: {ex.Message}");
                    }
                }

                // Position filter - FIX: kiểm tra null safety tốt hơn
                if (cmbPosition.SelectedItem != null)
                {
                    try
                    {
                        dynamic selectedPos = cmbPosition.SelectedItem;
                        var value = selectedPos.Value;

                        // FIX: Safe cast để tránh lỗi InvalidCastException
                        if (value is int intValue && intValue != -1)
                            filter.PositionID = intValue;
                        else if (int.TryParse(value?.ToString(), out int parsedValue) && parsedValue != -1)
                            filter.PositionID = parsedValue;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"Error parsing position filter: {ex.Message}");
                    }
                }

                // Gender filter
                if (cmbGender.SelectedItem != null && cmbGender.SelectedItem.ToString() != "Tất cả")
                {
                    filter.Gender = cmbGender.SelectedItem.ToString();
                }

                // Status filter
                if (cmbStatus.SelectedItem != null && cmbStatus.SelectedItem.ToString() != "Tất cả")
                {
                    filter.Status = cmbStatus.SelectedItem.ToString();
                }

                // Date filters
                filter.HireDateFrom = dtpHireFrom.Value.Date;
                filter.HireDateTo = dtpHireTo.Value.Date;

                // Search keyword
                filter.SearchKeyword = txtSearch.Text?.Trim() ?? string.Empty;

                await LoadEmployeeReports(filter);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error applying filter: {ex.Message}");
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle clear filter button click
        /// </summary>
        private async void BtnClearFilter_Click(object sender, EventArgs e)
        {
            try
            {
                cmbDepartment.SelectedIndex = 0;
                cmbPosition.SelectedIndex = 0;
                cmbGender.SelectedIndex = 0;
                cmbStatus.SelectedIndex = 0;
                dtpHireFrom.Value = DateTime.Now.AddYears(-10);
                dtpHireTo.Value = DateTime.Now;
                txtSearch.Text = "";

                await LoadEmployeeReports();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error clearing filter: {ex.Message}");
                MessageBox.Show($"Lỗi khi xóa bộ lọc: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle refresh button click
        /// </summary>
        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadInitialData();

                // Refresh current tab
                switch (tabControl.SelectedTab.Name)
                {
                    case "tabEmployees":
                        await LoadEmployeeReports();
                        break;
                    case "tabDepartments":
                        await LoadDepartmentReports();
                        break;
                    case "tabBirthdays":
                        await LoadBirthdayReports();
                        break;
                    case "tabAttendance":
                        await LoadAttendanceReports();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error refreshing data: {ex.Message}");
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle export to Excel button click
        /// </summary>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentEmployeeData == null || !currentEmployeeData.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Lưu báo cáo Excel",
                    FileName = $"BaoCaoNhanSu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var employeeModels = currentEmployeeData.Select(e => new EmployeeReportModel
                    {
                        EmployeeID = e.EmployeeID,
                        EmployeeCode = e.EmployeeCode,
                        FullName = e.FullName,
                        Gender = e.Gender,
                        Age = e.Age,
                        Department = e.Department,
                        Position = e.Position,
                        Manager = e.Manager,
                        HireDate = e.HireDate,
                        WorkingDays = e.WorkingDays,
                        Status = e.Status,
                        Phone = e.Phone,
                        Email = e.Email,
                        CurrentSalary = e.CurrentSalary,
                        TotalProjects = e.TotalProjects,
                        CompletedProjects = e.CompletedProjects,
                        AttendanceRate = e.AttendanceRate,
                        PerformanceScore = e.PerformanceScore
                    }).ToList();

                    bool success = hrReportBLL.ExportToExcel(employeeModels, saveDialog.FileName);

                    if (success)
                    {
                        MessageBox.Show("Xuất file Excel thành công!", "Thông báo",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (MessageBox.Show("Bạn có muốn mở file vừa tạo?", "Xác nhận",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi xuất file Excel!", "Lỗi",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error exporting to Excel: {ex.Message}");
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle print button click
        /// </summary>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng in báo cáo đang được phát triển!", "Thông báo",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handle load birthdays button click
        /// </summary>
        private async void BtnLoadBirthdays_Click(object sender, EventArgs e)
        {
            await LoadBirthdayReports();
        }

        /// <summary>
        /// Handle load attendance button click
        /// </summary>
        private async void BtnLoadAttendance_Click(object sender, EventArgs e)
        {
            await LoadAttendanceReports(); 
        }
        #endregion
    }
}
