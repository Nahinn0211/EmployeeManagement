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
        private bool isRecognizing = false;
        private DateTime recognitionStartTime;

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

            // Set initial UI state
            SetInitialUIState();

            // Initialize asynchronously to prevent blocking
            System.Threading.Tasks.Task.Run(InitializeFormAsync);
        }

        private void SetInitialUIState()
        {
            lblStatus.Text = "⏳ Đang khởi tạo hệ thống...";
            lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
            lblCameraStatus.Text = "📷 Đang kiểm tra camera...";
            lblCameraStatus.ForeColor = Color.Yellow;
            btnStartRecognition.Enabled = false;
            btnSettings.Enabled = true; // Keep settings always available
        }

        private async System.Threading.Tasks.Task InitializeFormAsync()
        {
            try
            {
                isInitializing = true;

                // Initialize camera in background thread
                await System.Threading.Tasks.Task.Run(() => InitializeCamera());

                // Check system readiness
                await CheckSystemReadinessAsync();

                isInitializing = false;
            }
            catch (Exception ex)
            {
                isInitializing = false;
                this.Invoke(new Action(() =>
                {
                    lblStatus.Text = $"❌ Lỗi khởi tạo: {ex.Message}";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    lblCameraStatus.Text = "❌ Khởi tạo thất bại";
                    lblCameraStatus.ForeColor = Color.Red;

                    MessageBox.Show($"Lỗi khởi tạo hệ thống: {ex.Message}\n\nVui lòng thử lại.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void InitializeCamera()
        {
            try
            {
                // Update UI from background thread
                this.Invoke(new Action(() =>
                {
                    lblCameraStatus.Text = "📷 Đang tìm camera...";
                    lblCameraStatus.ForeColor = Color.Yellow;
                }));

                System.Diagnostics.Debug.WriteLine("Starting camera detection...");

                // Lấy danh sách camera
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                System.Diagnostics.Debug.WriteLine($"Found {videoDevices.Count} video devices");

                this.Invoke(new Action(() =>
                {
                    if (videoDevices.Count == 0)
                    {
                        lblCameraStatus.Text = "❌ Không tìm thấy camera";
                        lblCameraStatus.ForeColor = Color.Red;
                        lblStatus.Text = "❌ Không có camera - Chỉ có thể test hệ thống";
                        lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                        btnStartRecognition.Enabled = false;

                        MessageBox.Show("Không tìm thấy camera!\n\nKiểm tra:\n" +
                            "• Camera đã được kết nối\n" +
                            "• Driver camera đã cài đặt\n" +
                            "• Quyền camera trong Windows Settings\n" +
                            "• Camera không bị app khác sử dụng",
                            "Lỗi Camera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Hiển thị thông tin camera tìm thấy
                    System.Diagnostics.Debug.WriteLine("Available cameras:");
                    for (int i = 0; i < videoDevices.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Camera {i}: {videoDevices[i].Name}");
                    }

                    lblCameraStatus.Text = $"📷 Tìm thấy {videoDevices.Count} camera - Đang khởi động...";
                    lblCameraStatus.ForeColor = Color.Green;

                    // Khởi động camera ngay lập tức
                    System.Threading.Tasks.Task.Run(StartCameraWithRetryAsync);
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Camera detection error: {ex.Message}");
                this.Invoke(new Action(() =>
                {
                    lblCameraStatus.Text = $"❌ Lỗi camera: {ex.Message}";
                    lblCameraStatus.ForeColor = Color.Red;
                    lblStatus.Text = "❌ Lỗi camera - Có thể test hệ thống";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    btnStartRecognition.Enabled = false;
                }));
            }
        }

        private async System.Threading.Tasks.Task StartCameraWithRetryAsync()
        {
            const int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries && !isCameraRunning && !this.IsDisposed)
            {
                retryCount++;
                System.Diagnostics.Debug.WriteLine($"Camera start attempt {retryCount}/{maxRetries}");

                try
                {
                    this.Invoke(new Action(() =>
                    {
                        lblCameraStatus.Text = $"📷 Đang khởi động camera (lần {retryCount})...";
                        lblCameraStatus.ForeColor = Color.Yellow;
                    }));

                    if (videoDevices?.Count > 0)
                    {
                        // Thử từng camera cho đến khi tìm được camera hoạt động
                        for (int i = 0; i < videoDevices.Count && !isCameraRunning; i++)
                        {
                            try
                            {
                                System.Diagnostics.Debug.WriteLine($"Trying camera {i}: {videoDevices[i].Name}");

                                // Dọn dẹp camera cũ nếu có
                                if (videoSource != null)
                                {
                                    try
                                    {
                                        videoSource.NewFrame -= VideoSource_NewFrame;
                                        if (videoSource.IsRunning)
                                        {
                                            videoSource.SignalToStop();
                                            videoSource.WaitForStop();
                                        }
                                    }
                                    catch { }
                                    videoSource = null;
                                }

                                await System.Threading.Tasks.Task.Delay(500); // Delay giữa các lần thử

                                videoSource = new VideoCaptureDevice(videoDevices[i].MonikerString);

                                // Thiết lập độ phân giải với error handling
                                try
                                {
                                    if (videoSource.VideoCapabilities?.Length > 0)
                                    {
                                        // Tìm độ phân giải phù hợp (ưu tiên 640x480, nếu không có thì chọn nhỏ nhất)
                                        var capability = videoSource.VideoCapabilities
                                            .Where(c => c.FrameSize.Width <= 640 && c.FrameSize.Height <= 480)
                                            .OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height)
                                            .FirstOrDefault() ?? videoSource.VideoCapabilities
                                            .OrderBy(c => c.FrameSize.Width * c.FrameSize.Height)
                                            .First();

                                        videoSource.VideoResolution = capability;
                                        System.Diagnostics.Debug.WriteLine($"Set resolution: {capability.FrameSize.Width}x{capability.FrameSize.Height}");
                                    }
                                }
                                catch (Exception resEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Resolution setting failed: {resEx.Message}");
                                    // Tiếp tục mà không set resolution
                                }

                                videoSource.NewFrame += VideoSource_NewFrame;

                                // Start camera với timeout dài hơn
                                var startTask = System.Threading.Tasks.Task.Run(() =>
                                {
                                    videoSource.Start();
                                    System.Diagnostics.Debug.WriteLine($"Camera {i} Start() method called");
                                });

                                var timeoutTask = System.Threading.Tasks.Task.Delay(10000); // 10 giây timeout
                                var completedTask = await System.Threading.Tasks.Task.WhenAny(startTask, timeoutTask);

                                if (completedTask == timeoutTask)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Camera {i} start timeout");
                                    continue; // Thử camera tiếp theo
                                }

                                // Đợi camera khởi động hoàn toàn
                                await System.Threading.Tasks.Task.Delay(1500);

                                // Kiểm tra camera có thực sự chạy không
                                if (videoSource.IsRunning)
                                {
                                    isCameraRunning = true;
                                    System.Diagnostics.Debug.WriteLine($"Camera {i} started successfully!");

                                    this.Invoke(new Action(() =>
                                    {
                                        lblCameraStatus.Text = $"📷 Camera sẵn sàng ({videoDevices[i].Name})";
                                        lblCameraStatus.ForeColor = Color.White;
                                    }));

                                    return; // Thành công, thoát khỏi tất cả loops
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"Camera {i} failed to start (IsRunning = false)");
                                }
                            }
                            catch (Exception camEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Camera {i} error: {camEx.Message}");
                                continue; // Thử camera tiếp theo
                            }
                        }
                    }

                    // Nếu đến đây mà vẫn chưa có camera nào chạy được
                    if (!isCameraRunning)
                    {
                        throw new Exception($"Không thể khởi động camera nào sau {retryCount} lần thử");
                    }
                    else
                    {
                        // Thành công, thoát khỏi retry loop
                        break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Camera start attempt {retryCount} failed: {ex.Message}");

                    if (retryCount < maxRetries)
                    {
                        // Đợi trước khi thử lại
                        await System.Threading.Tasks.Task.Delay(2000);
                    }
                    else
                    {
                        // Hết lần thử
                        this.Invoke(new Action(() =>
                        {
                            lblCameraStatus.Text = "❌ Không thể khởi động camera";
                            lblCameraStatus.ForeColor = Color.Red;
                            lblStatus.Text = "❌ Camera không khả dụng - Chỉ test hệ thống";
                            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);

                            // Hiển thị thông báo chi tiết
                            MessageBox.Show($"Không thể khởi động camera sau {maxRetries} lần thử!\n\n" +
                                $"Lỗi cuối: {ex.Message}\n\n" +
                                "Các bước khắc phục:\n" +
                                "1. Đóng tất cả ứng dụng khác đang dùng camera\n" +
                                "2. Kiểm tra quyền camera trong Windows Settings\n" +
                                "3. Thử rút và cắm lại camera USB\n" +
                                "4. Kiểm tra Device Manager xem camera có lỗi không\n" +
                                "5. Khởi động lại ứng dụng với quyền Administrator",
                                "Lỗi Camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                        break;
                    }
                }
            }
        }

        // Thêm method để test camera thủ công từ Settings
        private void TestCameraManually()
        {
            try
            {
                lblStatus.Text = "🔄 Đang test camera...";
                lblStatus.ForeColor = Color.Yellow;

                // Stop camera hiện tại
                StopCamera();

                // Restart camera detection
                System.Threading.Tasks.Task.Run(() =>
                {
                    System.Threading.Tasks.Task.Delay(1000).Wait(); // Đợi 1 giây
                    InitializeCamera();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi test camera: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cập nhật Settings để có nút Test Camera
        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var settingsForm = new Form
            {
                Text = "Cài đặt hệ thống",
                Size = new Size(450, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var infoLabel = new Label
            {
                Text = "Thông tin hệ thống:\n\n" +
                       $"• Camera: {videoDevices?.Count ?? 0} thiết bị\n" +
                       $"• Trạng thái camera: {(isCameraRunning ? "Đang chạy" : "Dừng")}\n" +
                       $"• Đang nhận diện: {(isRecognizing ? "Có" : "Không")}\n" +
                       $"• Face Recognition: {(FaceRecognitionService.CheckSystemReadiness().IsReady ? "Sẵn sàng" : "Chưa sẵn sàng")}\n\n" +
                       "Nếu camera không hoạt động, hãy thử:",
                Location = new Point(20, 20),
                Size = new Size(400, 180),
                Font = new Font("Segoe UI", 9)
            };

            var btnTestCamera = new Button
            {
                Text = "🔄 TEST CAMERA",
                Location = new Point(20, 220),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnCheckPermissions = new Button
            {
                Text = "⚙️ MỞ CAMERA SETTINGS",
                Location = new Point(180, 220),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 140, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnClose = new Button
            {
                Text = "Đóng",
                Location = new Point(170, 280),
                Size = new Size(100, 35),
                DialogResult = DialogResult.OK
            };

            btnTestCamera.Click += (s, ev) =>
            {
                settingsForm.Hide();
                TestCameraManually();
                settingsForm.Show();
            };

            btnCheckPermissions.Click += (s, ev) =>
            {
                try
                {
                    // Mở Camera settings trong Windows
                    System.Diagnostics.Process.Start("ms-settings:privacy-webcam");
                }
                catch
                {
                    MessageBox.Show("Không thể mở Camera Settings.\n\nVui lòng mở thủ công:\nSettings > Privacy > Camera",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            settingsForm.Controls.Add(infoLabel);
            settingsForm.Controls.Add(btnTestCamera);
            settingsForm.Controls.Add(btnCheckPermissions);
            settingsForm.Controls.Add(btnClose);

            settingsForm.ShowDialog(this);
        }
        private async System.Threading.Tasks.Task StartCameraAsync()
        {
            try
            {
                if (videoDevices?.Count > 0 && !isCameraRunning && !this.IsDisposed)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblCameraStatus.Text = "📷 Đang khởi động camera...";
                        lblCameraStatus.ForeColor = Color.Yellow;
                    }));

                    await System.Threading.Tasks.Task.Delay(200); // Small delay

                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                    // Thiết lập độ phân giải với timeout
                    if (videoSource.VideoCapabilities?.Length > 0)
                    {
                        var capability = videoSource.VideoCapabilities
                            .FirstOrDefault(c => c.FrameSize.Width == 640 && c.FrameSize.Height == 480)
                            ?? videoSource.VideoCapabilities[0];
                        videoSource.VideoResolution = capability;
                    }

                    videoSource.NewFrame += VideoSource_NewFrame;

                    // Start camera with timeout protection
                    var startTask = System.Threading.Tasks.Task.Run(() => videoSource.Start());
                    var timeoutTask = System.Threading.Tasks.Task.Delay(5000); // 5 second timeout

                    var completedTask = await System.Threading.Tasks.Task.WhenAny(startTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        throw new TimeoutException("Camera startup timeout");
                    }

                    isCameraRunning = true;

                    this.Invoke(new Action(() =>
                    {
                        lblCameraStatus.Text = "📷 Camera sẵn sàng";
                        lblCameraStatus.ForeColor = Color.White;
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    lblCameraStatus.Text = $"❌ Lỗi khởi động camera: {ex.Message}";
                    lblCameraStatus.ForeColor = Color.Red;
                    lblStatus.Text = "❌ Camera không khả dụng - Chỉ test hệ thống";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                }));
            }
        }

        private void StopCamera()
        {
            try
            {
                if (videoSource?.IsRunning == true)
                {
                    videoSource.NewFrame -= VideoSource_NewFrame;
                    videoSource.SignalToStop();

                    // Wait with timeout
                    var stopTask = System.Threading.Tasks.Task.Run(() => videoSource.WaitForStop());
                    stopTask.Wait(2000); // 2 second timeout

                    videoSource = null;
                }
                isCameraRunning = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping camera: {ex.Message}");
                isCameraRunning = false;
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (this.IsDisposed || this.Disposing) return;

                // Clone frame để tránh access violation
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Cập nhật UI trên main thread với error handling
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (!this.IsDisposed && pictureBoxCamera != null)
                            {
                                var oldImage = pictureBoxCamera.Image;
                                pictureBoxCamera.Image = frame;
                                oldImage?.Dispose();

                                // Update camera status during recognition
                                if (isRecognizing)
                                {
                                    var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
                                    var remaining = 30 - elapsed;
                                    lblCameraStatus.Text = $"🔍 Đang nhận diện... ({remaining:F0}s)";
                                    lblCameraStatus.ForeColor = Color.Yellow;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"UI update error: {ex.Message}");
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Video frame error: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task CheckSystemReadinessAsync()
        {
            try
            {
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = "🔍 Đang kiểm tra hệ thống...";
                        lblStatus.ForeColor = Color.FromArgb(33, 150, 243);
                    }));

                    await System.Threading.Tasks.Task.Delay(500); // Simulate check time

                    var checkResult = FaceRecognitionService.CheckSystemReadiness();

                    this.Invoke(new Action(() =>
                    {
                        if (!checkResult.IsReady)
                        {
                            lblStatus.Text = "⚠️ Hệ thống chưa sẵn sàng - Nhấn Cài đặt";
                            lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                            // Still enable button for testing
                            btnStartRecognition.Enabled = true;
                            btnStartRecognition.Text = "🔧 TEST HỆ THỐNG";
                        }
                        else
                        {
                            lblStatus.Text = "✅ Sẵn sàng chấm công";
                            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                            btnStartRecognition.Enabled = true;
                            btnStartRecognition.Text = "🚀 BẮT ĐẦU NHẬN DIỆN";
                        }
                    }));
                });
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    lblStatus.Text = $"❌ Lỗi kiểm tra: {ex.Message}";
                    lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    btnStartRecognition.Enabled = true;
                    btnStartRecognition.Text = "🔧 TEST HỆ THỐNG";
                }));
            }
        }

        private async void BtnStartRecognition_Click(object? sender, EventArgs e)
        {
            if (isRecognizing || isInitializing) return;

            try
            {
                // Quick UI feedback
                btnStartRecognition.Enabled = false;
                lblStatus.Text = "⏳ Đang chuẩn bị...";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);

                await System.Threading.Tasks.Task.Delay(100); // Let UI update

                isRecognizing = true;
                recognitionStartTime = DateTime.Now;

                // UI Updates
                btnStartRecognition.Visible = false;
                btnCancel.Visible = true;
                progressBarRecognition.Visible = true;
                progressBarRecognition.Value = 0;
                panelResult.Visible = false;

                lblStatus.Text = "🔍 Đang nhận diện... Vui lòng nhìn vào camera";
                lblStatus.ForeColor = Color.FromArgb(33, 150, 243);

                // Highlight camera area
                panelCamera.BackColor = Color.FromArgb(33, 150, 243);

                // Start timers
                timerRecognition.Start();
                timerUpdate.Start();

                // Start recognition with timeout
                var recognitionTask = FaceRecognitionService.RecognizeFromCameraAsync(30);
                var timeoutTask = System.Threading.Tasks.Task.Delay(35000); // 35 second total timeout

                var completedTask = await System.Threading.Tasks.Task.WhenAny(recognitionTask, timeoutTask);

                // Stop timers
                timerRecognition.Stop();
                timerUpdate.Stop();

                if (completedTask == timeoutTask)
                {
                    ProcessFailedRecognition("⏰ Hết thời gian - Hệ thống không phản hồi");
                }
                else
                {
                    var result = await recognitionTask;
                    if (result.Success)
                    {
                        await ProcessSuccessfulRecognition(result);
                    }
                    else
                    {
                        ProcessFailedRecognition(result.Message ?? "Nhận diện thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                ProcessFailedRecognition($"Lỗi: {ex.Message}");
            }
            finally
            {
                ResetUI();
            }
        }

        private async System.Threading.Tasks.Task ProcessSuccessfulRecognition(FaceRecognitionResult result)
        {
            try
            {
                lblStatus.Text = "🔍 Đang xử lý kết quả...";

                // Tìm nhân viên bằng EmployeeCode
                var allEmployees = await employeeBLL.GetAllEmployeesAsync();
                var employee = allEmployees.FirstOrDefault(e => e.EmployeeCode == result.EmployeeId);

                if (employee == null)
                {
                    ProcessFailedRecognition("Không tìm thấy thông tin nhân viên");
                    return;
                }

                // Lưu chấm công vào database
                var attendanceResult = await attendanceBLL.CreateAttendanceRecordAsync(
                    employee.EmployeeID.ToString(),
                    "Face Recognition",
                    result.AttendanceImagePath
                );

                if (attendanceResult.Success)
                {
                    ShowSuccessResult(result, employee, attendanceResult);
                    SystemSounds.Asterisk.Play();
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

        // [Phần còn lại của các method giữ nguyên như bản trước]
        private void ShowSuccessResult(FaceRecognitionResult result, EmployeeDTO employee, AttendanceCreateResult attendanceResult)
        {
            lblStatus.Text = "✅ Chấm công thành công!";
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
            lblCameraStatus.Text = $"✅ Nhận diện thành công - {attendanceResult.AttendanceType}";
            lblCameraStatus.ForeColor = Color.FromArgb(76, 175, 80);

            lblEmployeeInfo.Text = $"{employee.EmployeeCode} - {employee.FullName}";
            lblConfidence.Text = $"Độ tin cậy: {result.Confidence:F1}%";
            lblTime.Text = $"Thời gian: {result.Timestamp:dd/MM/yyyy HH:mm:ss}\nLoại: {attendanceResult.AttendanceType}";

            if (!string.IsNullOrEmpty(employee.FaceDataPath) && System.IO.File.Exists(employee.FaceDataPath))
            {
                try
                {
                    pictureBoxEmployee.Image?.Dispose();
                    pictureBoxEmployee.Image = Image.FromFile(employee.FaceDataPath);
                }
                catch
                {
                    pictureBoxEmployee.Image?.Dispose();
                    pictureBoxEmployee.Image = CreateDefaultEmployeeImage(employee.FullName);
                }
            }
            else
            {
                pictureBoxEmployee.Image?.Dispose();
                pictureBoxEmployee.Image = CreateDefaultEmployeeImage(employee.FullName);
            }

            panelResult.Visible = true;
            AutoHideResult();
        }

        private void AutoHideResult()
        {
            var timer = new System.Windows.Forms.Timer { Interval = 7000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                if (!this.IsDisposed)
                {
                    panelResult.Visible = false;
                    lblStatus.Text = "✅ Sẵn sàng chấm công";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    lblCameraStatus.Text = "📷 Camera sẵn sàng";
                    lblCameraStatus.ForeColor = Color.White;
                }
            };
            timer.Start();
        }

        private static Bitmap CreateDefaultEmployeeImage(string employeeName)
        {
            var bitmap = new Bitmap(100, 120);
            using var g = Graphics.FromImage(bitmap);
            g.FillRectangle(new SolidBrush(Color.FromArgb(63, 81, 181)), 0, 0, 100, 120);
            var font = new Font("Segoe UI", 12, FontStyle.Bold);
            var initials = GetInitials(employeeName);
            var textSize = g.MeasureString(initials, font);
            var x = (100 - textSize.Width) / 2;
            var y = (120 - textSize.Height) / 2;
            g.DrawString(initials, font, Brushes.White, x, y);
            return bitmap;
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "??";
            var parts = fullName.Split(' ');
            if (parts.Length == 1) return parts[0][..Math.Min(2, parts[0].Length)].ToUpper();
            return (parts[0][..1] + parts[^1][..1]).ToUpper();
        }

        private void FlashSuccessEffect()
        {
            var originalColor = panelCamera.BackColor;
            var flashTimer = new System.Windows.Forms.Timer { Interval = 200 };
            int flashCount = 0;

            flashTimer.Tick += (s, e) =>
            {
                if (this.IsDisposed) { flashTimer.Stop(); return; }
                flashCount++;
                panelCamera.BackColor = flashCount % 2 == 0 ? Color.FromArgb(76, 175, 80) : originalColor;
                if (flashCount >= 6)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                    panelCamera.BackColor = originalColor;
                }
            };
            flashTimer.Start();
        }

        private void ProcessFailedRecognition(string message)
        {
            lblStatus.Text = $"❌ {message}";
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
            lblCameraStatus.Text = "❌ Nhận diện thất bại";
            lblCameraStatus.ForeColor = Color.FromArgb(244, 67, 54);

            try { SystemSounds.Hand.Play(); } catch { }

            var resetTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            resetTimer.Tick += (s, e) =>
            {
                resetTimer.Stop();
                resetTimer.Dispose();
                if (!this.IsDisposed)
                {
                    lblStatus.Text = "✅ Sẵn sàng chấm công";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    lblCameraStatus.Text = "📷 Camera sẵn sàng";
                    lblCameraStatus.ForeColor = Color.White;
                }
            };
            resetTimer.Start();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            if (isRecognizing)
            {
                timerRecognition.Stop();
                timerUpdate.Stop();
                ResetUI();

                lblStatus.Text = "⏹️ Đã hủy nhận diện";
                lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
                lblCameraStatus.Text = "📷 Camera sẵn sàng";
                lblCameraStatus.ForeColor = Color.White;

                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, ev) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (!this.IsDisposed)
                    {
                        lblStatus.Text = "✅ Sẵn sàng chấm công";
                        lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    }
                };
                timer.Start();
            }
        }

      
        private void TimerRecognition_Tick(object? sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
            if (elapsed >= 30)
            {
                timerRecognition.Stop();
                timerUpdate.Stop();
                ProcessFailedRecognition("⏰ Hết thời gian nhận diện (30s)");
                ResetUI();
            }
        }

        private void TimerUpdate_Tick(object? sender, EventArgs e)
        {
            if (isRecognizing)
            {
                var elapsed = (DateTime.Now - recognitionStartTime).TotalSeconds;
                var progress = (int)((elapsed / 30.0) * 100);
                if (progressBarRecognition != null && !this.IsDisposed)
                {
                    progressBarRecognition.Value = Math.Min(progress, 100);
                }
            }
        }

        private void ResetUI()
        {
            isRecognizing = false;
            btnStartRecognition.Visible = true;
            btnStartRecognition.Enabled = true;
            btnCancel.Visible = false;
            progressBarRecognition.Visible = false;
            progressBarRecognition.Value = 0;
            panelCamera.BackColor = Color.White;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                isInitializing = false;
                isRecognizing = false;

                timerRecognition?.Stop();
                timerUpdate?.Stop();

                StopCamera();

                pictureBoxCamera?.Image?.Dispose();
                pictureBoxEmployee?.Image?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }

            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Camera initialization is handled in constructor async
        }
    }
}