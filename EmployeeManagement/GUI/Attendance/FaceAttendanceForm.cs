using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class FaceAttendanceForm : Form
    {
        private System.Windows.Forms.Timer? clockTimer;
        private VideoCaptureDevice? videoSource;
        private FilterInfoCollection? videoDevices;
        private bool isCameraRunning = false;
        private PictureBox? cameraBox;
        private Button? btnStartCamera;
        private Button? btnStopCamera;
        private Button? btnCapture;
        private Label? statusLabel;

        public FaceAttendanceForm()
        {
            InitializeComponent();
            SetupTimer();
            CreateMainLayout();
        }

        private void SetupTimer()
        {
            clockTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 1 second
            };
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            var timeLabel = this.Controls.Find("lblCurrentTime", true);
            if (timeLabel.Length > 0 && timeLabel[0] is Label label)
            {
                label.Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
            }
        }

        private void CreateMainLayout()
        {
            // Header Panel
            var headerPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(52, 73, 94)
            };
            this.Controls.Add(headerPanel);

            var titleLabel = new Label
            {
                Text = "HỆ THỐNG CHẤM CÔNG KHUÔN MẶT",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(titleLabel);

            // Main Content Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            this.Controls.Add(mainPanel);

            // Left Panel - Camera
            var leftPanel = new Panel
            {
                Width = 580,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 10, 0)
            };
            mainPanel.Controls.Add(leftPanel);

            CreateCameraSection(leftPanel);

            // Right Panel - Employee Info
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(10, 0, 0, 0)
            };
            mainPanel.Controls.Add(rightPanel);

            CreateEmployeeInfoSection(rightPanel);
        }

        private void CreateCameraSection(Panel parent)
        {
            // Header
            var headerLabel = new Label
            {
                Text = "📹 CAMERA CHẤM CÔNG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 10, 0, 0)
            };
            parent.Controls.Add(headerLabel);

            // Camera Display
            cameraBox = new PictureBox
            {
                Name = "pictureBoxCamera",
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 350,
                Dock = DockStyle.Top,
                Margin = new Padding(20, 10, 20, 10)
            };
            parent.Controls.Add(cameraBox);

            // Add placeholder text
            var placeholderLabel = new Label
            {
                Name = "placeholderLabel",
                Text = "Vùng hiển thị camera\nNhấn 'Bật Camera' để bắt đầu",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            cameraBox.Controls.Add(placeholderLabel);

            // Camera Controls Panel
            var controlsPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                Padding = new Padding(20, 10, 20, 10)
            };
            parent.Controls.Add(controlsPanel);

            // Start Camera Button
            btnStartCamera = new Button
            {
                Text = "🎥 Bật Camera",
                Size = new Size(120, 40),
                Location = new Point(0, 20),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnStartCamera.FlatAppearance.BorderSize = 0;
            btnStartCamera.Click += BtnStartCamera_Click;
            controlsPanel.Controls.Add(btnStartCamera);

            // Stop Camera Button
            btnStopCamera = new Button
            {
                Text = "⏹️ Tắt Camera",
                Size = new Size(120, 40),
                Location = new Point(130, 20),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnStopCamera.FlatAppearance.BorderSize = 0;
            btnStopCamera.Click += BtnStopCamera_Click;
            controlsPanel.Controls.Add(btnStopCamera);

            // Capture Button
            btnCapture = new Button
            {
                Text = "📸 Chụp Ảnh",
                Size = new Size(120, 40),
                Location = new Point(260, 20),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnCapture.FlatAppearance.BorderSize = 0;
            btnCapture.Click += BtnCapture_Click;
            controlsPanel.Controls.Add(btnCapture);

            // Register Face Button
            var btnRegisterFace = new Button
            {
                Text = "👤 Đăng Ký Khuôn Mặt",
                Size = new Size(150, 40),
                Location = new Point(390, 20),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegisterFace.FlatAppearance.BorderSize = 0;
            btnRegisterFace.Click += BtnRegisterFace_Click;
            controlsPanel.Controls.Add(btnRegisterFace);

            // Status Panel
            var statusPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(20, 15, 20, 15)
            };
            parent.Controls.Add(statusPanel);

            statusLabel = new Label
            {
                Text = "📊 Trạng thái: Sẵn sàng chấm công",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            statusPanel.Controls.Add(statusLabel);
        }

        private void CreateEmployeeInfoSection(Panel parent)
        {
            // Header
            var headerLabel = new Label
            {
                Text = "👤 THÔNG TIN NHÂN VIÊN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 10, 0, 0)
            };
            parent.Controls.Add(headerLabel);

            // Content Panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 10, 20, 20)
            };
            parent.Controls.Add(contentPanel);

            // Employee Photo
            var employeePhoto = new PictureBox
            {
                Size = new Size(120, 150),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            contentPanel.Controls.Add(employeePhoto);

            // Add placeholder for photo
            var photoPlaceholder = new Label
            {
                Text = "Chưa nhận diện",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            employeePhoto.Controls.Add(photoPlaceholder);

            // Employee Info Panel
            var infoPanel = new Panel
            {
                Location = new Point(140, 0),
                Size = new Size(400, 200)
            };
            contentPanel.Controls.Add(infoPanel);

            // Employee Info Labels
            CreateInfoLabel(infoPanel, "Mã nhân viên:", "Chưa nhận diện", 0);
            CreateInfoLabel(infoPanel, "Họ và tên:", "Chưa nhận diện", 30);
            CreateInfoLabel(infoPanel, "Phòng ban:", "Chưa nhận diện", 60);
            CreateInfoLabel(infoPanel, "Chức vụ:", "Chưa nhận diện", 90);
            CreateInfoLabel(infoPanel, "Lần cuối:", "Chưa có dữ liệu", 120);

            // Time Display Panel
            var timePanel = new Panel
            {
                Location = new Point(0, 170),
                Size = new Size(500, 80),
                BackColor = Color.FromArgb(52, 73, 94)
            };
            contentPanel.Controls.Add(timePanel);

            var currentTimeLabel = new Label
            {
                Name = "lblCurrentTime",
                Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy"),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            timePanel.Controls.Add(currentTimeLabel);

            // Action Buttons Panel
            var actionPanel = new Panel
            {
                Location = new Point(0, 270),
                Size = new Size(500, 80),
                Padding = new Padding(0, 10, 0, 10)
            };
            contentPanel.Controls.Add(actionPanel);

            // Check In Button
            var btnCheckIn = new Button
            {
                Text = "🔵 CHẤM CÔNG VÀO",
                Size = new Size(160, 50),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnCheckIn.FlatAppearance.BorderSize = 0;
            actionPanel.Controls.Add(btnCheckIn);

            // Check Out Button
            var btnCheckOut = new Button
            {
                Text = "🔴 CHẤM CÔNG RA",
                Size = new Size(160, 50),
                Location = new Point(170, 0),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnCheckOut.FlatAppearance.BorderSize = 0;
            actionPanel.Controls.Add(btnCheckOut);

            // Manual Entry Button
            var btnManualEntry = new Button
            {
                Text = "✏️ Nhập Thủ Công",
                Size = new Size(160, 50),
                Location = new Point(340, 0),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnManualEntry.FlatAppearance.BorderSize = 0;
            btnManualEntry.Click += BtnManualEntry_Click;
            actionPanel.Controls.Add(btnManualEntry);

            // Instructions Panel
            var instructionsPanel = new Panel
            {
                Location = new Point(0, 370),
                Size = new Size(500, 150),
                BackColor = Color.FromArgb(241, 196, 15),
                Padding = new Padding(15)
            };
            contentPanel.Controls.Add(instructionsPanel);

            var instructionsLabel = new Label
            {
                Text = "📝 HƯỚNG DẪN SỬ DỤNG:\n" +
                       "1. Nhấn 'Bật Camera' để khởi động\n" +
                       "2. Đưa khuôn mặt vào khung hình\n" +
                       "3. Hệ thống sẽ tự động nhận diện\n" +
                       "4. Nhấn nút chấm công tương ứng\n" +
                       "5. Kiểm tra kết quả",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Fill
            };
            instructionsPanel.Controls.Add(instructionsLabel);
        }

        private static void CreateInfoLabel(Panel parent, string labelText, string valueText, int yPosition)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(0, yPosition),
                Size = new Size(100, 25),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            parent.Controls.Add(label);

            var value = new Label
            {
                Text = valueText,
                Font = new Font("Segoe UI", 10),
                Location = new Point(110, yPosition),
                Size = new Size(280, 25),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            parent.Controls.Add(value);
        }

        private void BtnRegisterFace_Click(object? sender, EventArgs e)
        {
            // Mở form đăng ký khuôn mặt
            var registrationForm = new FaceRegistrationForm();
            registrationForm.ShowDialog();
        }

        private void BtnManualEntry_Click(object? sender, EventArgs e)
        {
            // Mở form nhập chấm công thủ công
            MessageBox.Show("Chức năng nhập thủ công sẽ được triển khai!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Camera Methods

        private void BtnStartCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                // Tìm các thiết bị camera có sẵn
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera nào!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("❌ Không tìm thấy camera", Color.FromArgb(231, 76, 60));
                    return;
                }

                // Sử dụng camera đầu tiên
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                // Thiết lập event handler cho frame mới
                videoSource.NewFrame += VideoSource_NewFrame;

                // Thiết lập độ phân giải (nếu có)
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    videoSource.VideoResolution = videoSource.VideoCapabilities[0];
                }

                // Bắt đầu capture
                videoSource.Start();

                // Cập nhật UI
                isCameraRunning = true;
                btnStartCamera!.Enabled = false;
                btnStopCamera!.Enabled = true;
                btnCapture!.Enabled = true;

                // Ẩn placeholder text
                var placeholder = cameraBox?.Controls.Find("placeholderLabel", false);
                if (placeholder?.Length > 0)
                {
                    placeholder[0].Visible = false;
                }

                UpdateStatus("✅ Camera đã khởi động", Color.FromArgb(46, 204, 113));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động camera: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("❌ Lỗi khởi động camera", Color.FromArgb(231, 76, 60));
            }
        }

        private void BtnStopCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                StopCamera();
                UpdateStatus("⏹️ Camera đã tắt", Color.FromArgb(52, 152, 219));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tắt camera: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCapture_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cameraBox?.Image != null)
                {
                    // Tạo thư mục lưu ảnh nếu chưa có
                    string captureDir = System.IO.Path.Combine(Application.StartupPath, "Captures");
                    if (!System.IO.Directory.Exists(captureDir))
                    {
                        System.IO.Directory.CreateDirectory(captureDir);
                    }

                    // Tạo tên file với timestamp
                    string fileName = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    string filePath = System.IO.Path.Combine(captureDir, fileName);

                    // Lưu ảnh
                    using (var bitmap = new Bitmap(cameraBox.Image))
                    {
                        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                    MessageBox.Show($"Đã lưu ảnh: {fileName}", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatus($"📸 Đã chụp: {fileName}", Color.FromArgb(52, 152, 219));
                }
                else
                {
                    MessageBox.Show("Không có hình ảnh để chụp!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Clone frame để tránh cross-thread issues
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Cập nhật PictureBox trên UI thread
                if (cameraBox?.InvokeRequired == true)
                {
                    cameraBox.Invoke(new MethodInvoker(() => {
                        cameraBox.Image?.Dispose(); // Dispose ảnh cũ
                        cameraBox.Image = frame;
                    }));
                }
                else
                {
                    cameraBox!.Image?.Dispose(); // Dispose ảnh cũ
                    cameraBox.Image = frame;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không hiển thị MessageBox để tránh spam
                System.Diagnostics.Debug.WriteLine($"Error in VideoSource_NewFrame: {ex.Message}");
            }
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }

            // Cập nhật UI
            isCameraRunning = false;
            btnStartCamera!.Enabled = true;
            btnStopCamera!.Enabled = false;
            btnCapture!.Enabled = false;

            // Xóa hình ảnh và hiện lại placeholder
            cameraBox!.Image?.Dispose();
            cameraBox.Image = null;

            var placeholder = cameraBox.Controls.Find("placeholderLabel", false);
            if (placeholder?.Length > 0)
            {
                placeholder[0].Visible = true;
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = $"📊 {message}";
                statusLabel.ForeColor = color;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dừng camera trước khi dispose
                StopCamera();

                clockTimer?.Stop();
                clockTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}