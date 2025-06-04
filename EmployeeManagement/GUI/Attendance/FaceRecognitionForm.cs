using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
using EmployeeManagement.Models.DTO;
using System.Media;
using AForge.Video;
using AForge.Video.DirectShow;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class FaceRecognitionForm : Form
    {
        private readonly AttendanceBLL attendanceBLL;
        private readonly EmployeeBLL employeeBLL;

        // Recognition state
        private bool isRecognizing = false;
        private DateTime recognitionStartTime;
        private int recognitionTimeoutSeconds = 30;

        // Camera components
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private bool isCameraRunning = false;
        private bool isInitializing = false;

        public FaceRecognitionForm()
        {
            InitializeComponent();
            attendanceBLL = new AttendanceBLL();
            employeeBLL = new EmployeeBLL();
        }

        private async void FaceRecognitionForm_Load(object sender, EventArgs e)
        {
            await InitializeSystemAsync();
        }

        private async Task InitializeSystemAsync()
        {
            try
            {
                lblStatus.Text = "🔍 Đang kiểm tra hệ thống...";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                // 1. Kiểm tra hệ thống Face Recognition
                await CheckFaceRecognitionSystemAsync();

                // 2. Khởi tạo camera
                await InitializeCameraAsync();

                // 3. Kích hoạt nút nếu mọi thứ OK
                if (isCameraRunning)
                {
                    EnableRecognitionButton();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khởi tạo hệ thống: {ex.Message}");
            }
        }

        private async Task CheckFaceRecognitionSystemAsync()
        {
            try
            {
                lblStatus.Text = "🔍 Đang kiểm tra Python Face Recognition...";

                var systemCheck = await Task.Run(() => FaceRecognitionService.CheckSystemReadiness());

                if (!systemCheck.IsReady)
                {
                    lblStatus.Text = "⚠️ Hệ thống Face Recognition chưa sẵn sàng";
                    lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                    var result = MessageBox.Show(
                        $"Hệ thống Face Recognition có vấn đề:\n\n{systemCheck.ErrorMessage}\n\n" +
                        "Bạn có muốn tiếp tục để test camera không?\n\n" +
                        "Lưu ý: Chức năng nhận diện sẽ không hoạt động.",
                        "Cảnh báo hệ thống",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }
                else
                {
                    lblStatus.Text = "✅ Hệ thống Face Recognition sẵn sàng";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Lỗi kiểm tra hệ thống Face Recognition";
                lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                System.Diagnostics.Debug.WriteLine($"Face Recognition check error: {ex.Message}");
            }
        }

        private async Task InitializeCameraAsync()
        {
            try
            {
                isInitializing = true;
                lblCameraStatus.Text = "📷 Đang tìm camera...";
                lblStatus.Text = "📷 Đang khởi tạo camera...";

                // Tìm camera devices
                await Task.Run(() =>
                {
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                });

                if (videoDevices == null || videoDevices.Count == 0)
                {
                    ShowNoCameraError();
                    return;
                }

                lblCameraStatus.Text = $"📷 Tìm thấy {videoDevices.Count} camera";
                lblCameraStatus.ForeColor = Color.Green;

                // Log camera info
                System.Diagnostics.Debug.WriteLine($"Found {videoDevices.Count} cameras:");
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"  Camera {i}: {videoDevices[i].Name}");
                }

                // Khởi động camera
                await StartFirstAvailableCameraAsync();
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khởi tạo camera: {ex.Message}");
            }
            finally
            {
                isInitializing = false;
            }
        }

        private async Task StartFirstAvailableCameraAsync()
        {
            for (int i = 0; i < videoDevices.Count; i++)
            {
                try
                {
                    lblCameraStatus.Text = $"📷 Đang thử camera {i + 1}...";

                    // Cleanup previous camera
                    CleanupCamera();

                    // Create new camera
                    videoSource = new VideoCaptureDevice(videoDevices[i].MonikerString);

                    // Set resolution if available
                    if (videoSource.VideoCapabilities?.Length > 0)
                    {
                        var capability = videoSource.VideoCapabilities
                            .Where(c => c.FrameSize.Width <= 640 && c.FrameSize.Height <= 480)
                            .OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height)
                            .FirstOrDefault() ?? videoSource.VideoCapabilities[0];

                        videoSource.VideoResolution = capability;
                    }

                    // Register event handler
                    videoSource.NewFrame += VideoSource_NewFrame;

                    // Start camera
                    videoSource.Start();

                    // Wait for camera to start
                    await Task.Delay(2000);

                    if (videoSource.IsRunning)
                    {
                        isCameraRunning = true;
                        lblCameraStatus.Text = $"📷 Camera {i + 1} đang hoạt động";
                        lblCameraStatus.ForeColor = Color.FromArgb(76, 175, 80);
                        lblStatus.Text = "📹 Camera sẵn sàng";
                        lblStatus.ForeColor = Color.FromArgb(76, 175, 80);

                        System.Diagnostics.Debug.WriteLine($"Camera {i} started successfully!");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Camera {i} failed: {ex.Message}");
                    continue;
                }
            }

            // Nếu không camera nào chạy được
            ShowCameraStartError();
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (this.IsDisposed || pictureBoxCamera == null) return;

                // Clone frame để tránh access violation
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Update UI trên main thread
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => UpdateCameraDisplay(frame)));
                }
                else
                {
                    UpdateCameraDisplay(frame);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Video frame error: {ex.Message}");
            }
        }

        private void UpdateCameraDisplay(Bitmap frame)
        {
            try
            {
                if (this.IsDisposed || pictureBoxCamera == null)
                {
                    frame?.Dispose();
                    return;
                }

                // Dispose old image
                var oldImage = pictureBoxCamera.Image;
                pictureBoxCamera.Image = frame;
                oldImage?.Dispose();

                // Update status during recognition
                if (isRecognizing)
                {
                    var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
                    var remaining = Math.Max(0, recognitionTimeoutSeconds - elapsed);
                    lblCameraStatus.Text = $"🔍 Đang nhận diện... ({remaining:F0}s)";
                    lblCameraStatus.ForeColor = Color.Yellow;
                }
                else if (!lblCameraStatus.Text.Contains("đang hoạt động"))
                {
                    lblCameraStatus.Text = "📷 Camera đang hoạt động";
                    lblCameraStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI update error: {ex.Message}");
                frame?.Dispose();
            }
        }

        private void EnableRecognitionButton()
        {
            btnStartRecognition.Enabled = true;
            btnStartRecognition.Text = "🚀 BẮT ĐẦU NHẬN DIỆN";
            lblStatus.Text = "✅ Hệ thống sẵn sàng chấm công";
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
        }

        private async void BtnStartRecognition_Click(object sender, EventArgs e)
        {
            if (isRecognizing) return;

            // Kiểm tra điều kiện trước khi bắt đầu
            if (!isCameraRunning)
            {
                MessageBox.Show("Camera chưa sẵn sàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await StartFaceRecognitionAsync();
        }

        private async Task StartFaceRecognitionAsync()
        {
            try
            {
                // Prepare UI
                isRecognizing = true;
                recognitionStartTime = DateTime.Now;

                btnStartRecognition.Visible = false;
                btnCancel.Visible = true;
                progressBarRecognition.Visible = true;
                progressBarRecognition.Value = 0;
                panelResult.Visible = false;

                lblStatus.Text = "🔍 Đang nhận diện khuôn mặt... Vui lòng nhìn vào camera";
                lblStatus.ForeColor = Color.FromArgb(33, 150, 243);

                // Highlight camera
                panelCamera.BackColor = Color.FromArgb(33, 150, 243);

                // Start timers
                timerRecognition.Start();
                timerUpdate.Start();

                // Call face recognition service
                var recognitionTask = FaceRecognitionService.RecognizeFromCameraAsync(recognitionTimeoutSeconds);
                var result = await recognitionTask;

                // Stop timers
                timerRecognition.Stop();
                timerUpdate.Stop();

                if (result.Success)
                {
                    await ProcessSuccessfulRecognitionAsync(result);
                }
                else
                {
                    ProcessFailedRecognition(result.Message ?? "Không nhận diện được khuôn mặt");
                }
            }
            catch (Exception ex)
            {
                timerRecognition.Stop();
                timerUpdate.Stop();
                ProcessFailedRecognition($"Lỗi: {ex.Message}");
            }
            finally
            {
                ResetRecognitionUI();
            }
        }

        private async Task ProcessSuccessfulRecognitionAsync(FaceRecognitionResult result)
        {
            try
            {
                lblStatus.Text = "🔍 Đang xử lý kết quả...";

                // Tìm nhân viên theo EmployeeCode
                var employee = await employeeBLL.GetEmployeeByCodeAsync(result.EmployeeId);

                if (employee == null)
                {
                    ProcessFailedRecognition($"Không tìm thấy nhân viên có mã: {result.EmployeeId}");
                    return;
                }

                // Kiểm tra trạng thái nhân viên
                if (employee.Status != "Đang làm việc")
                {
                    ProcessFailedRecognition($"Nhân viên {employee.FullName} không trong trạng thái làm việc");
                    return;
                }

                // Lưu chấm công
                var attendanceResult = await attendanceBLL.CreateAttendanceRecordAsync(
                    employee.EmployeeID.ToString(),
                    "Face Recognition",
                    result.AttendanceImagePath ?? ""
                );

                if (attendanceResult.Success)
                {
                    ShowSuccessResult(result, employee, attendanceResult);
                    PlaySuccessSound();
                    FlashSuccessEffect();
                }
                else
                {
                    ProcessFailedRecognition($"Lỗi lưu chấm công: {attendanceResult.Message}");
                }
            }
            catch (Exception ex)
            {
                ProcessFailedRecognition($"Lỗi xử lý: {ex.Message}");
            }
        }

        private void ShowSuccessResult(FaceRecognitionResult result, EmployeeDTO employee, AttendanceCreateResult attendanceResult)
        {
            lblStatus.Text = "✅ Chấm công thành công!";
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);

            var attendanceType = attendanceResult.AttendanceType == "CheckIn" ? "Chấm công vào" : "Chấm công ra";
            lblCameraStatus.Text = $"✅ {attendanceType} thành công";
            lblCameraStatus.ForeColor = Color.FromArgb(76, 175, 80);

            // Update result panel
            lblEmployeeInfo.Text = $"{employee.EmployeeCode} - {employee.FullName}";
            lblConfidence.Text = $"Độ tin cậy: {result.Confidence:F1}%";
            lblTime.Text = $"Thời gian: {result.Timestamp:dd/MM/yyyy HH:mm:ss}\n" +
                          $"Loại: {attendanceType}\n" +
                          $"Phòng ban: {employee.DepartmentName ?? "Chưa phân bổ"}";

            // Set employee image
            SetEmployeeImage(employee);

            panelResult.Visible = true;

            // Auto hide after 5 seconds
            var hideTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            hideTimer.Tick += (s, e) =>
            {
                hideTimer.Stop();
                hideTimer.Dispose();
                if (!this.IsDisposed)
                {
                    panelResult.Visible = false;
                    ResetStatusAfterSuccess();
                }
            };
            hideTimer.Start();
        }

        private void SetEmployeeImage(EmployeeDTO employee)
        {
            try
            {
                pictureBoxEmployee.Image?.Dispose();

                if (!string.IsNullOrEmpty(employee.FaceDataPath) && System.IO.File.Exists(employee.FaceDataPath))
                {
                    pictureBoxEmployee.Image = Image.FromFile(employee.FaceDataPath);
                }
                else
                {
                    pictureBoxEmployee.Image = CreateDefaultEmployeeImage(employee.FullName);
                }
            }
            catch
            {
                pictureBoxEmployee.Image = CreateDefaultEmployeeImage(employee.FullName);
            }
        }

        private static Bitmap CreateDefaultEmployeeImage(string name)
        {
            var bitmap = new Bitmap(100, 120);
            using var g = Graphics.FromImage(bitmap);
            g.FillRectangle(new SolidBrush(Color.FromArgb(63, 81, 181)), 0, 0, 100, 120);

            var font = new Font("Segoe UI", 12, FontStyle.Bold);
            var initials = GetInitials(name);
            var textSize = g.MeasureString(initials, font);
            var x = (100 - textSize.Width) / 2;
            var y = (120 - textSize.Height) / 2;

            g.DrawString(initials, font, Brushes.White, x, y);
            font.Dispose();

            return bitmap;
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "??";

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0][..Math.Min(2, parts[0].Length)].ToUpper();

            return (parts[0][..1] + parts[^1][..1]).ToUpper();
        }

        private void ProcessFailedRecognition(string message)
        {
            lblStatus.Text = $"❌ {message}";
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
            lblCameraStatus.Text = "❌ Nhận diện thất bại";
            lblCameraStatus.ForeColor = Color.FromArgb(244, 67, 54);

            try { SystemSounds.Hand.Play(); } catch { }

            // Auto reset after 3 seconds
            var resetTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            resetTimer.Tick += (s, e) =>
            {
                resetTimer.Stop();
                resetTimer.Dispose();
                if (!this.IsDisposed)
                {
                    ResetStatusAfterSuccess();
                }
            };
            resetTimer.Start();
        }

        private void ResetStatusAfterSuccess()
        {
            lblStatus.Text = "✅ Sẵn sàng chấm công";
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
            lblCameraStatus.Text = "📷 Camera sẵn sàng";
            lblCameraStatus.ForeColor = Color.FromArgb(76, 175, 80);
        }

        private static void PlaySuccessSound()
        {
            try { SystemSounds.Asterisk.Play(); } catch { }
        }

        private void FlashSuccessEffect()
        {
            var originalColor = panelCamera.BackColor;
            var flashTimer = new System.Windows.Forms.Timer { Interval = 200 };
            int flashCount = 0;

            flashTimer.Tick += (s, e) =>
            {
                if (this.IsDisposed)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                    return;
                }

                flashCount++;
                panelCamera.BackColor = flashCount % 2 == 0 ?
                    Color.FromArgb(76, 175, 80) : originalColor;

                if (flashCount >= 6)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                    panelCamera.BackColor = originalColor;
                }
            };
            flashTimer.Start();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (isRecognizing)
            {
                timerRecognition.Stop();
                timerUpdate.Stop();
                ResetRecognitionUI();

                lblStatus.Text = "⏹️ Đã hủy nhận diện";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                var resetTimer = new System.Windows.Forms.Timer { Interval = 2000 };
                resetTimer.Tick += (s, ev) =>
                {
                    resetTimer.Stop();
                    resetTimer.Dispose();
                    if (!this.IsDisposed)
                    {
                        ResetStatusAfterSuccess();
                    }
                };
                resetTimer.Start();
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void ShowSettingsDialog()
        {
            var form = new Form
            {
                Text = "Cài đặt hệ thống",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var txtInfo = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 20),
                Size = new Size(440, 280),
                Font = new Font("Consolas", 10),
                Text = GetSystemInfo()
            };

            var btnRestartCamera = new Button
            {
                Text = "🔄 Khởi động lại Camera",
                Location = new Point(50, 320),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnClose = new Button
            {
                Text = "Đóng",
                Location = new Point(270, 320),
                Size = new Size(100, 40),
                DialogResult = DialogResult.OK
            };

            btnRestartCamera.Click += async (s, e) =>
            {
                form.Hide();
                await RestartCameraAsync();
                txtInfo.Text = GetSystemInfo();
                form.Show();
            };

            form.Controls.Add(txtInfo);
            form.Controls.Add(btnRestartCamera);
            form.Controls.Add(btnClose);

            form.ShowDialog(this);
        }

        private async Task RestartCameraAsync()
        {
            try
            {
                lblStatus.Text = "🔄 Đang khởi động lại camera...";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                CleanupCamera();
                isCameraRunning = false;

                await Task.Delay(1000);
                await InitializeCameraAsync();

                if (isCameraRunning)
                {
                    EnableRecognitionButton();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khởi động lại camera: {ex.Message}");
            }
        }

        private string GetSystemInfo()
        {
            var info = "=== THÔNG TIN HỆ THỐNG ===\r\n\r\n";
            info += $"Camera devices: {videoDevices?.Count ?? 0}\r\n";
            info += $"Camera running: {(isCameraRunning ? "Đang chạy" : "Dừng")}\r\n";
            info += $"Is recognizing: {(isRecognizing ? "Có" : "Không")}\r\n";
            info += $"OS: {Environment.OSVersion}\r\n";
            info += $".NET Version: {Environment.Version}\r\n\r\n";

            if (videoDevices?.Count > 0)
            {
                info += "=== DANH SÁCH CAMERA ===\r\n";
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    info += $"[{i}] {videoDevices[i].Name}\r\n";
                }
                info += "\r\n";
            }

            info += "=== FACE RECOGNITION STATUS ===\r\n";
            try
            {
                var systemCheck = FaceRecognitionService.CheckSystemReadiness();
                info += $"Status: {(systemCheck.IsReady ? "Sẵn sàng" : "Chưa sẵn sàng")}\r\n";
                if (!systemCheck.IsReady)
                {
                    info += $"Error: {systemCheck.ErrorMessage}\r\n";
                }
                else
                {
                    info += $"Message: {systemCheck.Message}\r\n";
                }
            }
            catch (Exception ex)
            {
                info += $"Error checking: {ex.Message}\r\n";
            }

            return info;
        }

        private void BtnRegisterFace_Click(object sender, EventArgs e)
        {
            try
            {
                var registerForm = new FaceRegistrationForm();
                registerForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimerRecognition_Tick(object sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
            if (elapsed >= recognitionTimeoutSeconds)
            {
                timerRecognition.Stop();
                timerUpdate.Stop();
                ProcessFailedRecognition("⏰ Hết thời gian nhận diện");
                ResetRecognitionUI();
            }
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (isRecognizing)
            {
                var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
                var progress = (int)((elapsed / recognitionTimeoutSeconds) * 100);
                progressBarRecognition.Value = Math.Min(progress, 100);
            }
        }

        private void ResetRecognitionUI()
        {
            isRecognizing = false;
            btnStartRecognition.Visible = true;
            btnStartRecognition.Enabled = true;
            btnCancel.Visible = false;
            progressBarRecognition.Visible = false;
            progressBarRecognition.Value = 0;
            panelCamera.BackColor = Color.Black;
        }

        private void ShowNoCameraError()
        {
            lblCameraStatus.Text = "❌ Không tìm thấy camera";
            lblCameraStatus.ForeColor = Color.Red;
            lblStatus.Text = "❌ Cần camera để chấm công";
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
            btnStartRecognition.Enabled = false;
            btnStartRecognition.Text = "❌ KHÔNG CÓ CAMERA";

            MessageBox.Show(
                "Không tìm thấy camera!\n\n" +
                "Vui lòng kiểm tra:\n" +
                "• Camera đã được kết nối và bật\n" +
                "• Driver camera đã cài đặt đúng\n" +
                "• Quyền camera trong Windows Settings\n" +
                "• Camera không bị ứng dụng khác sử dụng\n\n" +
                "Sau khi khắc phục, hãy sử dụng nút 'Cài đặt' để khởi động lại.",
                "Không có Camera",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void ShowCameraStartError()
        {
            lblCameraStatus.Text = "❌ Không thể khởi động camera";
            lblCameraStatus.ForeColor = Color.Red;
            lblStatus.Text = "❌ Camera không khởi động được";
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
            btnStartRecognition.Enabled = false;
            btnStartRecognition.Text = "❌ CAMERA LỖI";

            MessageBox.Show(
                "Camera không thể khởi động!\n\n" +
                "Thử các cách khắc phục:\n" +
                "1. Đóng tất cả ứng dụng camera khác (Skype, Teams, Zoom)\n" +
                "2. Rút và cắm lại USB camera\n" +
                "3. Sử dụng nút 'Cài đặt' để khởi động lại\n" +
                "4. Kiểm tra Device Manager\n" +
                "5. Thử camera khác nếu có",
                "Camera Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void ShowError(string message)
        {
            lblStatus.Text = $"❌ {message}";
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
            System.Diagnostics.Debug.WriteLine($"Error: {message}");
        }

        private void CleanupCamera()
        {
            try
            {
                if (videoSource != null)
                {
                    videoSource.NewFrame -= VideoSource_NewFrame;
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                    }
                    videoSource = null;
                }
                isCameraRunning = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Camera cleanup error: {ex.Message}");
            }
        }

        private void FaceRecognitionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                isRecognizing = false;
                timerRecognition?.Stop();
                timerUpdate?.Stop();
                timerRecognition?.Dispose();
                timerUpdate?.Dispose();
                CleanupCamera();
                pictureBoxCamera?.Image?.Dispose();
                pictureBoxEmployee?.Image?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form closing cleanup error: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CleanupCamera();
                timerRecognition?.Dispose();
                timerUpdate?.Dispose();
                pictureBoxCamera?.Image?.Dispose();
                pictureBoxEmployee?.Image?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}