using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
using EmployeeManagement.Models.DTO;
using System.Threading;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class FaceRegistrationForm : Form
    {
        private readonly EmployeeBLL employeeBLL;
        private readonly AttendanceBLL attendanceBLL;
        private string selectedImagePath = string.Empty;
        private Image? capturedImage;
        private CancellationTokenSource cancellationTokenSource;

        public FaceRegistrationForm()
        {
            InitializeComponent();
            employeeBLL = new EmployeeBLL();
            attendanceBLL = new AttendanceBLL();
            cancellationTokenSource = new CancellationTokenSource();
            InitializeForm();
        }

        private async void InitializeForm()
        {
            await LoadEmployees();
            await LoadRegisteredFaces();
        }

        private async System.Threading.Tasks.Task LoadEmployees()
        {
            try
            {
                var employees = await employeeBLL.GetAllEmployeesAsync();
                var activeEmployees = employees.Where(e => e.Status == "Đang làm việc").ToList();

                cmbEmployees.DisplayMember = "DisplayText";
                cmbEmployees.ValueMember = "EmployeeID";
                cmbEmployees.DataSource = activeEmployees.Select(e => new
                {
                    EmployeeID = e.EmployeeID,
                    DisplayText = $"{e.EmployeeCode} - {e.FullName}"
                }).ToList();

                cmbEmployees.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadRegisteredFaces()
        {
            try
            {
                var result = await FaceRecognitionService.GetRegisteredFacesAsync();

                listViewRegistered.Items.Clear();

                if (result.Success)
                {
                    foreach (var face in result.Faces)
                    {
                        var item = new ListViewItem(face.EmployeeId);
                        item.SubItems.Add(face.EmployeeName);
                        item.SubItems.Add(DateTime.Now.ToString("dd/MM/yyyy")); // Should be actual registration date
                        item.Tag = face;
                        listViewRegistered.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách khuôn mặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CmbEmployees_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbEmployees.SelectedValue != null)
            {
                try
                {
                    int employeeId = (int)cmbEmployees.SelectedValue;
                    var employee = await employeeBLL.GetEmployeeByIdAsync(employeeId);

                    if (employee != null)
                    {
                        txtEmployeeCode.Text = employee.EmployeeCode;
                        txtEmployeeName.Text = employee.FullName;
                        txtDepartment.Text = employee.DepartmentName;

                        ValidateForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải thông tin nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ClearEmployeeInfo();
            }
        }

        private void ClearEmployeeInfo()
        {
            txtEmployeeCode.Clear();
            txtEmployeeName.Clear();
            txtDepartment.Clear();
            ValidateForm();
        }

        private void BtnSelectImage_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            openFileDialog.Title = "Chọn ảnh khuôn mặt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    selectedImagePath = openFileDialog.FileName;
                    var image = Image.FromFile(selectedImagePath);

                    // Dispose old image safely
                    var oldImage = pictureBoxPreview.Image;
                    pictureBoxPreview.Image = image;
                    oldImage?.Dispose();

                    lblImageStatus.Text = $"Đã chọn: {Path.GetFileName(selectedImagePath)}";
                    lblImageStatus.ForeColor = Color.FromArgb(76, 175, 80);

                    ValidateForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnCaptureFromCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                // Mở form camera để chụp ảnh
                using var cameraForm = new CameraCaptureForm();
                if (cameraForm.ShowDialog() == DialogResult.OK)
                {
                    // Dispose old images safely
                    capturedImage?.Dispose();
                    capturedImage = cameraForm.CapturedImage;

                    if (capturedImage != null)
                    {
                        // Create a copy for PictureBox to avoid file locking
                        var displayImage = new Bitmap(capturedImage);

                        var oldImage = pictureBoxPreview.Image;
                        pictureBoxPreview.Image = displayImage;
                        oldImage?.Dispose();

                        // Lưu ảnh chụp vào file tạm với tên unique
                        string tempFileName = $"captured_face_{Environment.TickCount}_{DateTime.Now.Ticks}.jpg";
                        selectedImagePath = Path.Combine(Path.GetTempPath(), tempFileName);

                        // Lưu ảnh gốc (không phải copy)
                        capturedImage.Save(selectedImagePath, ImageFormat.Jpeg);

                        lblImageStatus.Text = "Đã chụp ảnh từ camera";
                        lblImageStatus.ForeColor = Color.FromArgb(76, 175, 80);

                        ValidateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi chụp ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearImage_Click(object? sender, EventArgs e)
        {
            // Clean up images safely
            var oldImage = pictureBoxPreview.Image;
            pictureBoxPreview.Image = null;
            oldImage?.Dispose();

            capturedImage?.Dispose();
            capturedImage = null;

            // Clean up temp file
            if (!string.IsNullOrEmpty(selectedImagePath) && File.Exists(selectedImagePath) &&
                selectedImagePath.Contains(Path.GetTempPath()))
            {
                try
                {
                    File.Delete(selectedImagePath);
                }
                catch { }
            }

            selectedImagePath = string.Empty;
            lblImageStatus.Text = "Chưa chọn ảnh";
            lblImageStatus.ForeColor = Color.FromArgb(117, 117, 117);
            ValidateForm();
        }

        private async void BtnRegister_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            // Cancel any previous operation
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Disable UI to prevent multiple clicks
                btnRegister.Enabled = false;
                cmbEmployees.Enabled = false;
                btnSelectImage.Enabled = false;
                btnCaptureFromCamera.Enabled = false;
                btnClearImage.Enabled = false;

                progressBarRegistration.Visible = true;
                lblStatus.Text = "Đang đăng ký khuôn mặt...";
                lblStatus.ForeColor = Color.FromArgb(33, 150, 243);

                string employeeCode = txtEmployeeCode.Text;
                string employeeName = txtEmployeeName.Text;
                string imagePath = selectedImagePath;

                // Validate image file exists and is accessible
                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException("File ảnh không tồn tại hoặc không thể truy cập");
                }

                // Register face in background thread with timeout
                var registerTask = Task.Run(async () =>
                {
                    return await FaceRecognitionService.RegisterFaceAsync(employeeCode, employeeName, imagePath);
                }, cancellationTokenSource.Token);

                // Add timeout
                var timeoutTask = Task.Delay(60000, cancellationTokenSource.Token); // 60 seconds timeout
                var completedTask = await Task.WhenAny(registerTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    cancellationTokenSource.Cancel();
                    throw new TimeoutException("Quá trình đăng ký mất quá nhiều thời gian (timeout 60s)");
                }

                var result = await registerTask;

                progressBarRegistration.Visible = false;

                if (result.Success)
                {
                    lblStatus.Text = "Đăng ký thành công!";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);

                    // Update employee face data path in database using EmployeeID
                    if (cmbEmployees.SelectedValue != null)
                    {
                        int employeeId = (int)cmbEmployees.SelectedValue;
                        await attendanceBLL.UpdateEmployeeFaceDataAsync(employeeId, result.FacePath);
                    }

                    // Refresh list
                    await LoadRegisteredFaces();

                    // Clear form
                    ClearForm();

                    MessageBox.Show("Đăng ký khuôn mặt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = $"Đăng ký thất bại: {result.Message}";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    MessageBox.Show($"Đăng ký thất bại: {result.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Đã hủy đăng ký";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
            }
            catch (TimeoutException ex)
            {
                lblStatus.Text = "Hết thời gian đăng ký";
                lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                MessageBox.Show(ex.Message, "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                progressBarRegistration.Visible = false;
                lblStatus.Text = "Lỗi đăng ký";
                lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable UI
                btnRegister.Enabled = true;
                cmbEmployees.Enabled = true;
                btnSelectImage.Enabled = true;
                btnCaptureFromCamera.Enabled = true;
                btnClearImage.Enabled = true;
                progressBarRegistration.Visible = false;
            }
        }

        private async void BtnDeleteSelected_Click(object? sender, EventArgs e)
        {
            if (listViewRegistered.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn khuôn mặt cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = listViewRegistered.SelectedItems[0];
            var face = (RegisteredFace)selectedItem.Tag;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa khuôn mặt của nhân viên {face.EmployeeName}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    lblStatus.Text = "Đang xóa...";
                    lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                    var deleteResult = await FaceRecognitionService.DeleteRegisteredFaceAsync(face.EmployeeId);

                    if (deleteResult.Success)
                    {
                        lblStatus.Text = "Xóa thành công";
                        lblStatus.ForeColor = Color.FromArgb(76, 175, 80);

                        // Update database - Tìm nhân viên theo EmployeeCode và update FaceDataPath
                        var allEmployees = await employeeBLL.GetAllEmployeesAsync();
                        var employee = allEmployees.FirstOrDefault(e => e.EmployeeCode == face.EmployeeId);

                        if (employee != null)
                        {
                            await attendanceBLL.UpdateEmployeeFaceDataAsync(employee.EmployeeID, null);
                        }

                        // Refresh list
                        await LoadRegisteredFaces();

                        MessageBox.Show("Xóa khuôn mặt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatus.Text = $"Xóa thất bại: {deleteResult.Message}";
                        lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                        MessageBox.Show($"Xóa thất bại: {deleteResult.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Lỗi xóa";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    MessageBox.Show($"Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefreshList_Click(object? sender, EventArgs e)
        {
            await LoadRegisteredFaces();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            // Cancel any ongoing operation
            cancellationTokenSource?.Cancel();
            this.Close();
        }

        private bool ValidateForm()
        {
            bool isValid = cmbEmployees.SelectedValue != null &&
                          !string.IsNullOrEmpty(selectedImagePath) &&
                          File.Exists(selectedImagePath);

            btnRegister.Enabled = isValid;

            if (isValid)
            {
                lblStatus.Text = "Sẵn sàng đăng ký";
                lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
            }
            else
            {
                lblStatus.Text = "Vui lòng chọn nhân viên và ảnh";
                lblStatus.ForeColor = Color.FromArgb(158, 158, 158);
            }

            return isValid;
        }

        private void ClearForm()
        {
            cmbEmployees.SelectedIndex = -1;
            ClearEmployeeInfo();
            BtnClearImage_Click(null, null);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Cancel any ongoing operations
                cancellationTokenSource?.Cancel();

                // Clean up images
                var previewImage = pictureBoxPreview.Image;
                pictureBoxPreview.Image = null;
                previewImage?.Dispose();

                capturedImage?.Dispose();

                // Clean up temp files
                if (!string.IsNullOrEmpty(selectedImagePath) &&
                    selectedImagePath.Contains(Path.GetTempPath()) &&
                    File.Exists(selectedImagePath))
                {
                    try
                    {
                        File.Delete(selectedImagePath);
                    }
                    catch { }
                }

                // Dispose cancellation token source
                cancellationTokenSource?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during form closing: {ex.Message}");
            }

            base.OnFormClosing(e);
        }
    }
}