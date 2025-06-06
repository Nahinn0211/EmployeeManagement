using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class AttendanceReportForm : Form
    {
        private readonly AttendanceBLL attendanceBLL;
        private readonly EmployeeBLL employeeBLL;

        public AttendanceReportForm()
        {
            InitializeComponent();
            attendanceBLL = new AttendanceBLL();
            employeeBLL = new EmployeeBLL();
            InitializeForm();
        }

        private async void InitializeForm()
        {
            await LoadDepartments();
            LoadAttendanceStatuses();
            await LoadInitialData();
        }

        private async System.Threading.Tasks.Task LoadDepartments()
        {
            try
            {
                var departments = await employeeBLL.GetAllDepartmentsAsync();

                cmbDepartment.DisplayMember = "DepartmentName";
                cmbDepartment.ValueMember = "DepartmentID";

                var departmentList = departments.ToList();
                departmentList.Insert(0, new DepartmentDTO { DepartmentID = 0, DepartmentName = "Tất cả phòng ban" });

                cmbDepartment.DataSource = departmentList;
                cmbDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phòng ban: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAttendanceStatuses()
        {
            var statuses = new[]
            {
                new { Value = "", Text = "Tất cả trạng thái" },
                new { Value = "Đã chấm công vào", Text = "Đã chấm công vào" },
                new { Value = "Đủ giờ", Text = "Đủ giờ" },
                new { Value = "Thiếu giờ", Text = "Thiếu giờ" },
                new { Value = "Về sớm", Text = "Về sớm" },
                new { Value = "Đã chấm công ra", Text = "Đã chấm công ra" }
            };

            cmbStatus.DisplayMember = "Text";
            cmbStatus.ValueMember = "Value";
            cmbStatus.DataSource = statuses;
            cmbStatus.SelectedIndex = 0;
        }

        private async void CmbDepartment_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadEmployeesByDepartment();
        }

        private async System.Threading.Tasks.Task LoadEmployeesByDepartment()
        {
            try
            {
                int? departmentId = null;
                if (cmbDepartment.SelectedValue != null && (int)cmbDepartment.SelectedValue > 0)
                {
                    departmentId = (int)cmbDepartment.SelectedValue;
                }

                var employees = await employeeBLL.GetEmployeesByDepartmentAsync(departmentId);

                cmbEmployee.DisplayMember = "DisplayText";
                cmbEmployee.ValueMember = "EmployeeID";

                var employeeList = employees.Select(e => new
                {
                    EmployeeID = e.EmployeeID,
                    DisplayText = $"{e.EmployeeCode} - {e.FullName}"
                }).ToList();

                employeeList.Insert(0, new { EmployeeID = 0, DisplayText = "Tất cả nhân viên" });

                cmbEmployee.DataSource = employeeList;
                cmbEmployee.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadInitialData()
        {
            // Set default date range (last 30 days)
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;

            await SearchAttendanceData();
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await SearchAttendanceData();
        }

        private async System.Threading.Tasks.Task SearchAttendanceData()
        {
            try
            {
                lblStatusFooter.Text = "Đang tải dữ liệu...";
                lblStatusFooter.ForeColor = Color.FromArgb(33, 150, 243);
                progressBar.Visible = true;

                var criteria = BuildSearchCriteria();

                // Load data for all tabs
                await System.Threading.Tasks.Task.WhenAll(
                    LoadDailyAttendanceData(criteria),
                    LoadSummaryData(criteria),
                    LoadFaceRecognitionData(criteria)
                );

                lblStatusFooter.Text = "Tải dữ liệu thành công";
                lblStatusFooter.ForeColor = Color.FromArgb(76, 175, 80);
            }
            catch (Exception ex)
            {
                lblStatusFooter.Text = "Lỗi tải dữ liệu";
                lblStatusFooter.ForeColor = Color.FromArgb(244, 67, 54);
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private AttendanceSearchCriteria BuildSearchCriteria()
        {
            return new AttendanceSearchCriteria
            {
                FromDate = dtpFromDate.Value.Date,
                ToDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1),
                DepartmentId = cmbDepartment.SelectedValue != null && (int)cmbDepartment.SelectedValue > 0
                    ? (int)cmbDepartment.SelectedValue : null,
                EmployeeId = cmbEmployee.SelectedValue != null && (int)cmbEmployee.SelectedValue > 0
                    ? (int)cmbEmployee.SelectedValue : null,
                Status = !string.IsNullOrEmpty(cmbStatus.SelectedValue?.ToString())
                    ? cmbStatus.SelectedValue.ToString() : null
            };
        }

        private async System.Threading.Tasks.Task LoadDailyAttendanceData(AttendanceSearchCriteria criteria)
        {
            try
            {
                var dailyData = await attendanceBLL.GetDailyAttendanceReportAsync(criteria);

                dataGridViewDaily.DataSource = dailyData;
                ConfigureDailyDataGridView();

                lblDailyStats.Text = $"Thống kê: {dailyData.Count} bản ghi chấm công";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu chấm công hàng ngày: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadSummaryData(AttendanceSearchCriteria criteria)
        {
            try
            {
                var summaryData = await attendanceBLL.GetAttendanceSummaryReportAsync(criteria);

                dataGridViewSummary.DataSource = summaryData;
                ConfigureSummaryDataGridView();

                lblSummaryStats.Text = $"Thống kê: {summaryData.Count} nhân viên";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu tổng hợp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadFaceRecognitionData(AttendanceSearchCriteria criteria)
        {
            try
            {
                var faceData = await attendanceBLL.GetFaceRecognitionAttendanceReportAsync(criteria);

                dataGridViewFaceRecognition.DataSource = faceData;
                ConfigureFaceRecognitionDataGridView();

                lblFaceStats.Text = $"Thống kê: {faceData.Count} lần chấm công bằng khuôn mặt";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu chấm công khuôn mặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDailyDataGridView()
        {
            if (dataGridViewDaily.Columns.Count == 0) return;

            if (dataGridViewDaily.Columns["EmployeeCode"] != null)
                dataGridViewDaily.Columns["EmployeeCode"].HeaderText = "Mã NV";
            if (dataGridViewDaily.Columns["EmployeeName"] != null)
                dataGridViewDaily.Columns["EmployeeName"].HeaderText = "Họ tên";
            if (dataGridViewDaily.Columns["DepartmentName"] != null)
                dataGridViewDaily.Columns["DepartmentName"].HeaderText = "Phòng ban";
            if (dataGridViewDaily.Columns["CheckInTime"] != null)
                dataGridViewDaily.Columns["CheckInTime"].HeaderText = "Giờ vào";
            if (dataGridViewDaily.Columns["CheckOutTime"] != null)
                dataGridViewDaily.Columns["CheckOutTime"].HeaderText = "Giờ ra";
            if (dataGridViewDaily.Columns["WorkingHours"] != null)
                dataGridViewDaily.Columns["WorkingHours"].HeaderText = "Giờ làm";
            if (dataGridViewDaily.Columns["Status"] != null)
                dataGridViewDaily.Columns["Status"].HeaderText = "Trạng thái";
            if (dataGridViewDaily.Columns["CheckInMethod"] != null)
                dataGridViewDaily.Columns["CheckInMethod"].HeaderText = "Phương thức";
            if (dataGridViewDaily.Columns["Notes"] != null)
                dataGridViewDaily.Columns["Notes"].HeaderText = "Ghi chú";

            // Format columns
            if (dataGridViewDaily.Columns["CheckInTime"] != null)
                dataGridViewDaily.Columns["CheckInTime"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (dataGridViewDaily.Columns["CheckOutTime"] != null)
                dataGridViewDaily.Columns["CheckOutTime"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (dataGridViewDaily.Columns["WorkingHours"] != null)
                dataGridViewDaily.Columns["WorkingHours"].DefaultCellStyle.Format = "F2";

            // Set column widths
            SetColumnWidth("EmployeeCode", 80);
            SetColumnWidth("EmployeeName", 150);
            SetColumnWidth("DepartmentName", 120);
            SetColumnWidth("CheckInTime", 130);
            SetColumnWidth("CheckOutTime", 130);
            SetColumnWidth("WorkingHours", 80);
            SetColumnWidth("Status", 100);
            SetColumnWidth("CheckInMethod", 120);
        }

        private void ConfigureSummaryDataGridView()
        {
            if (dataGridViewSummary.Columns.Count == 0) return;

            if (dataGridViewSummary.Columns["EmployeeCode"] != null)
                dataGridViewSummary.Columns["EmployeeCode"].HeaderText = "Mã NV";
            if (dataGridViewSummary.Columns["EmployeeName"] != null)
                dataGridViewSummary.Columns["EmployeeName"].HeaderText = "Họ tên";
            if (dataGridViewSummary.Columns["DepartmentName"] != null)
                dataGridViewSummary.Columns["DepartmentName"].HeaderText = "Phòng ban";
            if (dataGridViewSummary.Columns["TotalDays"] != null)
                dataGridViewSummary.Columns["TotalDays"].HeaderText = "Tổng ngày";
            if (dataGridViewSummary.Columns["PresentDays"] != null)
                dataGridViewSummary.Columns["PresentDays"].HeaderText = "Có mặt";
            if (dataGridViewSummary.Columns["AbsentDays"] != null)
                dataGridViewSummary.Columns["AbsentDays"].HeaderText = "Vắng mặt";
            if (dataGridViewSummary.Columns["LateDays"] != null)
                dataGridViewSummary.Columns["LateDays"].HeaderText = "Đi muộn";
            if (dataGridViewSummary.Columns["TotalWorkingHours"] != null)
                dataGridViewSummary.Columns["TotalWorkingHours"].HeaderText = "Tổng giờ làm";
            if (dataGridViewSummary.Columns["AverageWorkingHours"] != null)
                dataGridViewSummary.Columns["AverageWorkingHours"].HeaderText = "TB giờ/ngày";
            if (dataGridViewSummary.Columns["AttendanceRate"] != null)
                dataGridViewSummary.Columns["AttendanceRate"].HeaderText = "Tỷ lệ có mặt (%)";

            // Format columns
            if (dataGridViewSummary.Columns["TotalWorkingHours"] != null)
                dataGridViewSummary.Columns["TotalWorkingHours"].DefaultCellStyle.Format = "F2";
            if (dataGridViewSummary.Columns["AverageWorkingHours"] != null)
                dataGridViewSummary.Columns["AverageWorkingHours"].DefaultCellStyle.Format = "F2";
            if (dataGridViewSummary.Columns["AttendanceRate"] != null)
                dataGridViewSummary.Columns["AttendanceRate"].DefaultCellStyle.Format = "F1";
        }

        private void ConfigureFaceRecognitionDataGridView()
        {
            if (dataGridViewFaceRecognition.Columns.Count == 0) return;

            if (dataGridViewFaceRecognition.Columns["EmployeeCode"] != null)
                dataGridViewFaceRecognition.Columns["EmployeeCode"].HeaderText = "Mã NV";
            if (dataGridViewFaceRecognition.Columns["EmployeeName"] != null)
                dataGridViewFaceRecognition.Columns["EmployeeName"].HeaderText = "Họ tên";
            if (dataGridViewFaceRecognition.Columns["DepartmentName"] != null)
                dataGridViewFaceRecognition.Columns["DepartmentName"].HeaderText = "Phòng ban";
            if (dataGridViewFaceRecognition.Columns["CheckInTime"] != null)
                dataGridViewFaceRecognition.Columns["CheckInTime"].HeaderText = "Thời gian";
            if (dataGridViewFaceRecognition.Columns["Confidence"] != null)
                dataGridViewFaceRecognition.Columns["Confidence"].HeaderText = "Độ tin cậy (%)";
            if (dataGridViewFaceRecognition.Columns["ImagePath"] != null)
                dataGridViewFaceRecognition.Columns["ImagePath"].HeaderText = "Ảnh chấm công";
            if (dataGridViewFaceRecognition.Columns["Status"] != null)
                dataGridViewFaceRecognition.Columns["Status"].HeaderText = "Trạng thái";

            // Format columns
            if (dataGridViewFaceRecognition.Columns["CheckInTime"] != null)
                dataGridViewFaceRecognition.Columns["CheckInTime"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            if (dataGridViewFaceRecognition.Columns["Confidence"] != null)
                dataGridViewFaceRecognition.Columns["Confidence"].DefaultCellStyle.Format = "F1";

            // Add image column button for viewing attendance images
            if (!dataGridViewFaceRecognition.Columns.Contains("ViewImage"))
            {
                var viewImageColumn = new DataGridViewButtonColumn
                {
                    Name = "ViewImage",
                    HeaderText = "Xem ảnh",
                    Text = "Xem",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                };
                dataGridViewFaceRecognition.Columns.Add(viewImageColumn);

                dataGridViewFaceRecognition.CellClick += DataGridViewFaceRecognition_CellClick;
            }
        }

        private void SetColumnWidth(string columnName, int width)
        {
            if (dataGridViewDaily.Columns[columnName] != null)
                dataGridViewDaily.Columns[columnName].Width = width;
        }

        private void DataGridViewFaceRecognition_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewFaceRecognition.Columns["ViewImage"]?.Index && e.RowIndex >= 0)
            {
                var imagePath = dataGridViewFaceRecognition.Rows[e.RowIndex].Cells["ImagePath"].Value?.ToString();
                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    ShowAttendanceImage(imagePath);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy ảnh chấm công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static void ShowAttendanceImage(string imagePath)
        {
            try
            {
                using var imageForm = new Form();
                imageForm.Text = "Ảnh chấm công";
                imageForm.Size = new Size(600, 500);
                imageForm.StartPosition = FormStartPosition.CenterParent;
                imageForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                imageForm.MaximizeBox = false;
                imageForm.MinimizeBox = false;

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(imagePath)
                };

                imageForm.Controls.Add(pictureBox);
                imageForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            // Reset filters to default
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            cmbDepartment.SelectedIndex = 0;
            cmbEmployee.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
        }

        private async void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                using var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv|All Files|*.*";
                saveFileDialog.Title = "Xuất báo cáo";
                saveFileDialog.FileName = $"BaoCaoChamCong_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblStatusFooter.Text = "Đang xuất báo cáo...";
                    lblStatusFooter.ForeColor = Color.FromArgb(255, 152, 0);
                    progressBar.Visible = true;

                    var criteria = BuildSearchCriteria();
                    var success = await ExportToFile(saveFileDialog.FileName, criteria);

                    progressBar.Visible = false;

                    if (success)
                    {
                        lblStatusFooter.Text = "Xuất báo cáo thành công";
                        lblStatusFooter.ForeColor = Color.FromArgb(76, 175, 80);

                        var result = MessageBox.Show(
                            "Xuất báo cáo thành công! Bạn có muốn mở file không?",
                            "Thành công",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = saveFileDialog.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                    else
                    {
                        lblStatusFooter.Text = "Xuất báo cáo thất bại";
                        lblStatusFooter.ForeColor = Color.FromArgb(244, 67, 54);
                    }
                }
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                lblStatusFooter.Text = "Lỗi xuất báo cáo";
                lblStatusFooter.ForeColor = Color.FromArgb(244, 67, 54);
                MessageBox.Show($"Lỗi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<bool> ExportToFile(string fileName, AttendanceSearchCriteria criteria)
        {
            try
            {
                // Get all data
                var dailyData = await attendanceBLL.GetDailyAttendanceReportAsync(criteria);
                var summaryData = await attendanceBLL.GetAttendanceSummaryReportAsync(criteria);
                var faceData = await attendanceBLL.GetFaceRecognitionAttendanceReportAsync(criteria);

                // Export to CSV format
                var content = new System.Text.StringBuilder();

                // Header
                content.AppendLine($"BÁO CÁO CHẤM CÔNG");
                content.AppendLine($"Từ ngày: {criteria.FromDate:dd/MM/yyyy} đến {criteria.ToDate:dd/MM/yyyy}");
                content.AppendLine($"Xuất ngày: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                content.AppendLine();

                // Daily attendance data
                content.AppendLine("=== CHẤM CÔNG HÀNG NGÀY ===");
                content.AppendLine("Mã NV,Họ tên,Phòng ban,Giờ vào,Giờ ra,Giờ làm,Trạng thái,Phương thức,Ghi chú");

                foreach (var item in dailyData)
                {
                    content.AppendLine($"{item.EmployeeCode},{item.EmployeeName},{item.DepartmentName}," +
                        $"{item.CheckInTime:dd/MM/yyyy HH:mm},{item.CheckOutTime?.ToString("dd/MM/yyyy HH:mm") ?? ""}," +
                        $"{item.WorkingHours:F2},{item.Status},{item.CheckInMethod},{item.Notes}");
                }

                content.AppendLine();
                content.AppendLine("=== TỔNG HỢP THEO NHÂN VIÊN ===");
                content.AppendLine("Mã NV,Họ tên,Phòng ban,Tổng ngày,Có mặt,Vắng mặt,Đi muộn,Tổng giờ làm,TB giờ/ngày,Tỷ lệ có mặt(%)");

                foreach (var item in summaryData)
                {
                    content.AppendLine($"{item.EmployeeCode},{item.EmployeeName},{item.DepartmentName}," +
                        $"{item.TotalDays},{item.PresentDays},{item.AbsentDays},{item.LateDays}," +
                        $"{item.TotalWorkingHours:F2},{item.AverageWorkingHours:F2},{item.AttendanceRate:F1}");
                }

                content.AppendLine();
                content.AppendLine("=== CHẤM CÔNG BẰNG KHUÔN MẶT ===");
                content.AppendLine("Mã NV,Họ tên,Phòng ban,Thời gian,Độ tin cậy(%),Trạng thái");

                foreach (var item in faceData)
                {
                    content.AppendLine($"{item.EmployeeCode},{item.EmployeeName},{item.DepartmentName}," +
                        $"{item.CheckInTime:dd/MM/yyyy HH:mm:ss},{item.Confidence:F1},{item.Status}");
                }

                // Save to file
                await System.IO.File.WriteAllTextAsync(fileName, content.ToString(), System.Text.Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Auto-refresh timer (optional)
            var refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 300000 // 5 minutes
            };
            refreshTimer.Tick += async (s, ev) =>
            {
                if (tabControlReports.SelectedTab == tabPageDaily)
                {
                    await LoadDailyAttendanceData(BuildSearchCriteria());
                }
            };
            refreshTimer.Start();
        }
    }
}