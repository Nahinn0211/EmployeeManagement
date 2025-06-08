using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
using EmployeeManagement.Models.DTO;
using System.Media;
using AForge.Video;
using AForge.Video.DirectShow;
using EmployeeManagement.DAL;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class FaceRecognitionForm : Form
    {
        private readonly AttendanceBLL attendanceBLL;
        private readonly EmployeeBLL employeeBLL;
        private readonly UserDAL userDAL;

        // Recognition state
        private bool isRecognizing = false;
        private DateTime recognitionStartTime;
        private readonly int recognitionTimeoutSeconds = 30;

        // Camera components
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private bool isCameraRunning = false;
        private readonly object cameraLock = new object();

        // Timers
        private System.Windows.Forms.Timer? recognitionTimer;
        private System.Windows.Forms.Timer? updateTimer;

        public FaceRecognitionForm()
        {
            InitializeComponent();
            attendanceBLL = new AttendanceBLL();
            employeeBLL = new EmployeeBLL();
            userDAL = new UserDAL();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Recognition timeout timer
            recognitionTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Check every second
            };
            recognitionTimer.Tick += RecognitionTimer_Tick;

            // UI update timer
            updateTimer = new System.Windows.Forms.Timer
            {
                Interval = 100 // Update UI every 100ms
            };
            updateTimer.Tick += UpdateTimer_Tick;
        }

        private async void FaceRecognitionForm_Load(object sender, EventArgs e)
        {
            await InitializeSystemAsync();

            // Hiển thị thông tin user đăng nhập
            if (UserSession.IsLoggedIn)
            {
                var currentEmployee = await GetCurrentLoggedInEmployeeAsync();
                if (currentEmployee != null)
                {
                    SetStatus($"👤 Đăng nhập: {currentEmployee.FullName} ({currentEmployee.EmployeeCode})",
                             Color.FromArgb(76, 175, 80));
                }
            }
        }

        private async Task InitializeSystemAsync()
        {
            try
            {
                SetStatus("🔍 Đang kiểm tra hệ thống...", Color.FromArgb(255, 152, 0));

                // 1. Check Face Recognition System
                await CheckFaceRecognitionSystemAsync();

                // 2. Initialize Camera
                await InitializeCameraAsync();

                // 3. Enable recognition if everything is OK
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

        private SystemCheckResult CheckSystemReadinessSync()
        {
            try
            {
                // Gọi static method từ FaceRecognitionService
                return EmployeeManagement.Utilities.FaceRecognitionService.CheckSystemReadiness();
            }
            catch (Exception ex)
            {
                return new SystemCheckResult
                {
                    IsReady = false,
                    Message = "",
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task CheckFaceRecognitionSystemAsync()
        {
            try
            {
                SetStatus("🔍 Đang kiểm tra EmguCV Face Recognition...", Color.FromArgb(255, 152, 0));

                // Gọi sync method để kiểm tra system
                var systemCheck = CheckSystemReadinessSync();

                if (!systemCheck.IsReady)
                {
                    SetStatus("⚠️ Hệ thống Face Recognition chưa sẵn sàng", Color.FromArgb(255, 152, 0));

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
                    SetStatus("✅ Hệ thống Face Recognition sẵn sàng", Color.FromArgb(76, 175, 80));
                }
            }
            catch (Exception ex)
            {
                SetStatus("❌ Lỗi kiểm tra hệ thống Face Recognition", Color.FromArgb(244, 67, 54));
                System.Diagnostics.Debug.WriteLine($"Face Recognition check error: {ex.Message}");
            }
        }

        private async Task InitializeCameraAsync()
        {
            try
            {
                SetCameraStatus("📷 Đang kiểm tra camera...", Color.FromArgb(255, 152, 0));
                SetStatus("📷 Đang khởi tạo camera...", Color.FromArgb(255, 152, 0));

                // Check if any camera devices exist
                await Task.Run(() =>
                {
                    try
                    {
                        videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error getting video devices: {ex.Message}");
                        videoDevices = null;
                    }
                });

                if (videoDevices == null || videoDevices.Count == 0)
                {
                    ShowNoCameraError();
                    return;
                }

                SetCameraStatus($"📷 Tìm thấy {videoDevices.Count} camera", Color.Green);

                // Show camera diagnostics
                System.Diagnostics.Debug.WriteLine(GetCameraDiagnostics());

                // Try to start camera with retry mechanism
                bool cameraStarted = await StartCameraWithRetryAsync();

                if (!cameraStarted)
                {
                    ShowCameraStartError();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khởi tạo camera: {ex.Message}");
            }
        }

        private async Task<bool> StartCameraWithRetryAsync(int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    SetCameraStatus($"📷 Thử khởi động camera (lần {attempt}/{maxRetries})...", Color.FromArgb(255, 152, 0));

                    // Cleanup any existing camera
                    CleanupCameraSync();
                    await Task.Delay(1000); // Wait for cleanup

                    // Try each camera device
                    for (int i = 0; i < videoDevices!.Count; i++)
                    {
                        try
                        {
                            SetCameraStatus($"📷 Đang thử camera {i + 1}...", Color.FromArgb(255, 152, 0));

                            await Task.Delay(500); // Brief delay between attempts

                            bool success = await TryStartSingleCameraAsync(i);
                            if (success)
                            {
                                isCameraRunning = true;
                                SetCameraStatus($"📷 Camera {i + 1} đang hoạt động", Color.FromArgb(76, 175, 80));
                                SetStatus("📹 Camera sẵn sàng", Color.FromArgb(76, 175, 80));
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Camera {i} failed on attempt {attempt}: {ex.Message}");
                            CleanupCameraSync();
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    SetCameraStatus($"🔄 Thử lại sau 2 giây... ({attempt}/{maxRetries})", Color.FromArgb(255, 152, 0));
                    await Task.Delay(2000);
                }
            }

            return false;
        }

        private async Task<bool> TryStartSingleCameraAsync(int deviceIndex)
        {
            try
            {
                VideoCaptureDevice? newVideoSource = null;

                // Create camera outside of lock to avoid await in lock
                try
                {
                    newVideoSource = new VideoCaptureDevice(videoDevices![deviceIndex].MonikerString);

                    // Set reasonable resolution to avoid compatibility issues
                    if (newVideoSource.VideoCapabilities?.Length > 0)
                    {
                        // Find best resolution (prefer smaller resolutions for stability)
                        var capability = newVideoSource.VideoCapabilities
                            .Where(c => c.FrameSize.Width <= 640 && c.FrameSize.Height <= 480)
                            .OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height)
                            .FirstOrDefault();

                        if (capability == null)
                        {
                            // Fallback to smallest available resolution
                            capability = newVideoSource.VideoCapabilities
                                .OrderBy(c => c.FrameSize.Width * c.FrameSize.Height)
                                .FirstOrDefault();
                        }

                        if (capability != null)
                        {
                            newVideoSource.VideoResolution = capability;
                            System.Diagnostics.Debug.WriteLine($"Camera resolution set to: {capability.FrameSize.Width}x{capability.FrameSize.Height}");
                        }
                    }

                    // Register event handler
                    newVideoSource.NewFrame += VideoSource_NewFrame;

                    // Start camera
                    newVideoSource.Start();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating/starting camera: {ex.Message}");
                    return false;
                }

                // Now assign to videoSource under lock
                lock (cameraLock)
                {
                    videoSource = newVideoSource;
                }

                 for (int wait = 0; wait < 30; wait++) // Wait up to 3 seconds
                {
                    await Task.Delay(100);

                    bool isRunning = false;
                    lock (cameraLock)
                    {
                        isRunning = videoSource?.IsRunning == true;
                    }

                    if (isRunning)
                    {
                        // Double check - wait for first frame
                        await Task.Delay(500);
                        lock (cameraLock)
                        {
                            if (videoSource?.IsRunning == true)
                            {
                                return true;
                            }
                        }
                    }
                }

                // Camera didn't start properly
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TryStartSingleCamera error: {ex.Message}");
                return false;
            }
        }

        private void CleanupCameraSync()
        {
            try
            {
                VideoCaptureDevice? sourceToCleanup = null;

                lock (cameraLock)
                {
                    sourceToCleanup = videoSource;
                    videoSource = null;
                    isCameraRunning = false;
                }

                // Cleanup outside of lock
                if (sourceToCleanup != null)
                {
                    try
                    {
                        // Remove event handler first
                        sourceToCleanup.NewFrame -= VideoSource_NewFrame;

                        if (sourceToCleanup.IsRunning)
                        {
                            sourceToCleanup.SignalToStop();

                            // Wait for stop with timeout to prevent hanging
                            var stopTask = Task.Run(() =>
                            {
                                try
                                {
                                    sourceToCleanup.WaitForStop();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"WaitForStop error: {ex.Message}");
                                }
                            });

                            if (!stopTask.Wait(3000)) // 3 second timeout
                            {
                                System.Diagnostics.Debug.WriteLine("Camera stop timeout - forcing cleanup");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Camera stop error: {ex.Message}");
                    }
                }

                // Clear any displayed frame
                if (pictureBoxCamera?.Image != null)
                {
                    var oldImage = pictureBoxCamera.Image;
                    pictureBoxCamera.Image = null;
                    oldImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        private string GetCameraDiagnostics()
        {
            var info = "=== CAMERA DIAGNOSTICS ===\n";
            info += $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
            info += $"OS: {Environment.OSVersion}\n\n";

            try
            {
                if (videoDevices == null)
                {
                    info += "Error: videoDevices is null\n";
                    return info;
                }

                info += $"Camera devices found: {videoDevices.Count}\n\n";

                for (int i = 0; i < videoDevices.Count; i++)
                {
                    info += $"Camera {i + 1}:\n";
                    info += $"  Name: {videoDevices[i].Name}\n";
                    info += $"  MonikerString: {videoDevices[i].MonikerString.Substring(0, Math.Min(50, videoDevices[i].MonikerString.Length))}...\n";

                    try
                    {
                        var testDevice = new VideoCaptureDevice(videoDevices[i].MonikerString);

                        if (testDevice.VideoCapabilities?.Length > 0)
                        {
                            info += $"  Capabilities: {testDevice.VideoCapabilities.Length}\n";
                            var cap = testDevice.VideoCapabilities[0];
                            info += $"  Default Resolution: {cap.FrameSize.Width}x{cap.FrameSize.Height}\n";
                            info += $"  Max FPS: {cap.MaximumFrameRate}\n";
                        }
                        else
                        {
                            info += "  No video capabilities available\n";
                        }

                        info += "  Status: Available for testing\n";

                        // Note: AForge VideoCaptureDevice doesn't implement IDisposable
                        testDevice = null;
                    }
                    catch (Exception ex)
                    {
                        info += $"  Status: Error - {ex.Message}\n";
                    }
                    info += "\n";
                }
            }
            catch (Exception ex)
            {
                info += $"Error getting camera diagnostics: {ex.Message}\n";
            }

            return info;
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (this.IsDisposed || pictureBoxCamera == null) return;

                // Clone frame to avoid access violation
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Update UI on main thread
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
                    SetCameraStatus($"🔍 Đang nhận diện... ({remaining:F0}s)", Color.Yellow);
                }
                else if (!lblCameraStatus.Text.Contains("đang hoạt động"))
                {
                    SetCameraStatus("📷 Camera đang hoạt động", Color.FromArgb(76, 175, 80));
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
            SetStatus("✅ Hệ thống sẵn sàng chấm công", Color.FromArgb(76, 175, 80));
        }

        private async void BtnStartRecognition_Click(object sender, EventArgs e)
        {
            if (isRecognizing) return;

            // Check conditions before starting
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

                SetStatus("🔍 Đang nhận diện khuôn mặt... Vui lòng nhìn vào camera", Color.FromArgb(33, 150, 243));

                // Highlight camera
                panelCamera.BackColor = Color.FromArgb(33, 150, 243);

                // Start timers
                recognitionTimer?.Start();
                updateTimer?.Start();

                // Call face recognition service
                var recognitionTask = EmployeeManagement.Utilities.FaceRecognitionService.RecognizeFromCameraAsync(recognitionTimeoutSeconds);
                var result = await recognitionTask;

                // Stop timers
                recognitionTimer?.Stop();
                updateTimer?.Stop();

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
                recognitionTimer?.Stop();
                updateTimer?.Stop();
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
                SetStatus("🔍 Đang xử lý kết quả...", Color.FromArgb(33, 150, 243));

                // Kiểm tra user đã đăng nhập chưa
                if (!UserSession.IsLoggedIn)
                {
                    ProcessFailedRecognition("Bạn chưa đăng nhập hệ thống");
                    return;
                }

                // Lấy thông tin nhân viên từ user đang đăng nhập
                var currentEmployee = await GetCurrentLoggedInEmployeeAsync();
                if (currentEmployee == null)
                {
                    ProcessFailedRecognition("Không tìm thấy thông tin nhân viên cho tài khoản đang đăng nhập");
                    return;
                }

                // So sánh khuôn mặt nhận diện với nhân viên đang đăng nhập
                if (result.EmployeeId != currentEmployee.EmployeeCode)
                {
                    ProcessFailedRecognition($"Khuôn mặt không khớp!\n" +
                                           $"Tài khoản đăng nhập: {currentEmployee.FullName} ({currentEmployee.EmployeeCode})\n" +
                                           $"Khuôn mặt nhận diện: {result.EmployeeId}\n" +
                                           $"Vui lòng chấm công bằng khuôn mặt của chính bạn!");
                    return;
                }

                // Kiểm tra trạng thái nhân viên
                if (currentEmployee.Status != "Đang làm việc")
                {
                    ProcessFailedRecognition($"Nhân viên {currentEmployee.FullName} không trong trạng thái làm việc");
                    return;
                }

                // Thực hiện chấm công
                var attendanceResult = await attendanceBLL.CreateAttendanceRecordAsync(
                    currentEmployee.EmployeeID.ToString(),
                    "Face Recognition + Login Verification",
                    result.AttendanceImagePath ?? ""
                );

                if (attendanceResult.Success)
                {
                    ShowSuccessResult(result, currentEmployee, attendanceResult);
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

        private int? GetEmployeeIdByUserId(int userId)
        {
            return userDAL.GetEmployeeIdByUserId(userId);
        }

        private async Task<EmployeeDTO?> GetCurrentLoggedInEmployeeAsync()
        {
            try
            {
                if (!UserSession.IsLoggedIn)
                    return null;

                int currentUserId = UserSession.CurrentUserId;

                // Lấy EmployeeID từ UserID
                var employeeId = GetEmployeeIdByUserId(currentUserId);

                if (!employeeId.HasValue)
                    return null;

                return await employeeBLL.GetEmployeeByIdAsync(employeeId.Value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current employee: {ex.Message}");
                return null;
            }
        }

        private void ShowSuccessResult(FaceRecognitionResult result, EmployeeDTO employee, AttendanceCreateResult attendanceResult)
        {
            SetStatus("✅ Chấm công thành công!", Color.FromArgb(76, 175, 80));

            var attendanceType = attendanceResult.AttendanceType == "CheckIn" ? "Chấm công vào" : "Chấm công ra";
            SetCameraStatus($"✅ {attendanceType} thành công", Color.FromArgb(76, 175, 80));

            // Thông báo thành công
            MessageBox.Show(
                $"🎉 CHẤM CÔNG THÀNH CÔNG!\n\n" +
                $"👤 Nhân viên: {employee.FullName}\n" +
                $"🏷️ Mã số: {employee.EmployeeCode}\n" +
                $"🏢 Phòng ban: {employee.DepartmentName ?? "Chưa phân bổ"}\n" +
                $"⏰ Thời gian: {result.Timestamp:dd/MM/yyyy HH:mm:ss}\n" +
                $"📋 Loại: {attendanceType}\n" +
                $"🎯 Độ tin cậy: {result.Confidence:F1}%\n\n" +
                $"Cảm ơn bạn đã sử dụng hệ thống chấm công!",
                "Chấm công thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Update result panel
            lblEmployeeInfo.Text = $"{employee.EmployeeCode} - {employee.FullName}";
            lblConfidence.Text = $"Độ tin cậy: {result.Confidence:F1}%";
            lblTime.Text = $"Thời gian: {result.Timestamp:dd/MM/yyyy HH:mm:ss}\n" +
                          $"Loại: {attendanceType}\n" +
                          $"Phòng ban: {employee.DepartmentName ?? "Chưa phân bổ"}";

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

                if (!string.IsNullOrEmpty(employee.FaceDataPath) && File.Exists(employee.FaceDataPath))
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
            SetStatus($"❌ {message}", Color.FromArgb(244, 67, 54));
            SetCameraStatus("❌ Nhận diện thất bại", Color.FromArgb(244, 67, 54));

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
            SetStatus("✅ Sẵn sàng chấm công", Color.FromArgb(76, 175, 80));
            SetCameraStatus("📷 Camera sẵn sàng", Color.FromArgb(76, 175, 80));
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
                recognitionTimer?.Stop();
                updateTimer?.Stop();
                ResetRecognitionUI();

                SetStatus("⏹️ Đã hủy nhận diện", Color.FromArgb(255, 152, 0));

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
            using var form = new Form
            {
                Text = "Cài đặt hệ thống",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var txtInfo = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 20),
                Size = new Size(540, 350),
                Font = new Font("Consolas", 9),
                Text = GetSystemInfo()
            };

            var btnTestSystem = new Button
            {
                Text = "🧪 Test Hệ thống",
                Location = new Point(20, 390),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnRestartCamera = new Button
            {
                Text = "🔄 Khởi động lại Camera",
                Location = new Point(170, 390),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnClose = new Button
            {
                Text = "Đóng",
                Location = new Point(470, 390),
                Size = new Size(90, 35),
                DialogResult = DialogResult.OK
            };

            btnTestSystem.Click += async (s, e) =>
            {
                btnTestSystem.Enabled = false;
                btnTestSystem.Text = "Đang test...";

                try
                {
                    await TestSystemAsync();
                    txtInfo.Text = GetSystemInfo();
                }
                finally
                {
                    btnTestSystem.Enabled = true;
                    btnTestSystem.Text = "🧪 Test Hệ thống";
                }
            };

            btnRestartCamera.Click += async (s, e) =>
            {
                btnRestartCamera.Enabled = false;
                btnRestartCamera.Text = "Đang khởi động...";

                try
                {
                    await RestartCameraAsync();
                    txtInfo.Text = GetSystemInfo();
                }
                finally
                {
                    btnRestartCamera.Enabled = true;
                    btnRestartCamera.Text = "🔄 Khởi động lại Camera";
                }
            };

            form.Controls.AddRange(new Control[] { txtInfo, btnTestSystem, btnRestartCamera, btnClose });
            form.ShowDialog(this);
        }

        private async Task TestSystemAsync()
        {
            try
            {
                SetStatus("🧪 Đang test hệ thống...", Color.FromArgb(255, 152, 0));

                // Gọi sync method để test
                var healthResult = CheckSystemReadinessSync();

                if (healthResult.IsReady)
                {
                    SetStatus("✅ Test hệ thống thành công", Color.FromArgb(76, 175, 80));
                }
                else
                {
                    SetStatus("❌ Test hệ thống thất bại", Color.FromArgb(244, 67, 54));
                }
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Lỗi test hệ thống: {ex.Message}", Color.FromArgb(244, 67, 54));
            }
        }

        private async Task RestartCameraAsync()
        {
            try
            {
                SetStatus("🔄 Đang khởi động lại camera...", Color.FromArgb(255, 152, 0));

                CleanupCameraSync();
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
            info += $"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\r\n";
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
                // Gọi sync method để lấy thông tin
                var systemCheck = CheckSystemReadinessSync();

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
                using var registerForm = new FaceRegistrationForm();
                registerForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RecognitionTimer_Tick(object? sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
            if (elapsed >= recognitionTimeoutSeconds)
            {
                recognitionTimer?.Stop();
                updateTimer?.Stop();
                ProcessFailedRecognition("⏰ Hết thời gian nhận diện");
                ResetRecognitionUI();
            }
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
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
            SetCameraStatus("❌ Không tìm thấy camera", Color.Red);
            SetStatus("❌ Cần camera để chấm công", Color.FromArgb(244, 67, 54));
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
            SetCameraStatus("❌ Không thể khởi động camera", Color.Red);
            SetStatus("❌ Camera không khởi động được", Color.FromArgb(244, 67, 54));
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
            SetStatus($"❌ {message}", Color.FromArgb(244, 67, 54));
            System.Diagnostics.Debug.WriteLine($"Error: {message}");
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus != null && !this.IsDisposed)
            {
                if (lblStatus.InvokeRequired)
                {
                    lblStatus.Invoke(new Action(() =>
                    {
                        lblStatus.Text = text;
                        lblStatus.ForeColor = color;
                    }));
                }
                else
                {
                    lblStatus.Text = text;
                    lblStatus.ForeColor = color;
                }
            }
        }

        private void SetCameraStatus(string text, Color color)
        {
            if (lblCameraStatus != null && !this.IsDisposed)
            {
                if (lblCameraStatus.InvokeRequired)
                {
                    lblCameraStatus.Invoke(new Action(() =>
                    {
                        lblCameraStatus.Text = text;
                        lblCameraStatus.ForeColor = color;
                    }));
                }
                else
                {
                    lblCameraStatus.Text = text;
                    lblCameraStatus.ForeColor = color;
                }
            }
        }

        private void FaceRecognitionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                isRecognizing = false;
                recognitionTimer?.Stop();
                updateTimer?.Stop();
                recognitionTimer?.Dispose();
                updateTimer?.Dispose();
                CleanupCameraSync();
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
                CleanupCameraSync();
                recognitionTimer?.Dispose();
                updateTimer?.Dispose();
                pictureBoxCamera?.Image?.Dispose();
                pictureBoxEmployee?.Image?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}