using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class FaceRegistrationForm : Form
    {
        private int capturedImages = 0;
        private const int REQUIRED_IMAGES = 5;

        // Camera variables
        private VideoCaptureDevice? videoSource;
        private FilterInfoCollection? videoDevices;
        private bool isCameraRunning = false;
        private PictureBox? cameraBox;
        private Button? btnStartCamera;
        private Button? btnStopCamera;
        private Button? btnCaptureImage;

        public FaceRegistrationForm()
        {
            InitializeComponent();
            LoadEmployeeList();
            CreateMainLayout();
        }

        private void CreateMainLayout()
        {
            // Header Panel
            var headerPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(155, 89, 182)
            };
            this.Controls.Add(headerPanel);

            var titleLabel = new Label
            {
                Text = "ĐĂNG KÝ KHUÔN MẶT NHÂN VIÊN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(titleLabel);

            // Main Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            this.Controls.Add(mainPanel);

            // Left Panel - Employee Selection
            var leftPanel = new Panel
            {
                Width = 300,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 10, 0)
            };
            mainPanel.Controls.Add(leftPanel);

            CreateEmployeeSelectionSection(leftPanel);

            // Center Panel - Camera
            var centerPanel = new Panel
            {
                Width = 400,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Margin = new Padding(10, 0, 10, 0)
            };
            mainPanel.Controls.Add(centerPanel);

            CreateCameraSection(centerPanel);

            // Right Panel - Captured Images
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(10, 0, 0, 0)
            };
            mainPanel.Controls.Add(rightPanel);

            CreateCapturedImagesSection(rightPanel);
        }

        private void CreateEmployeeSelectionSection(Panel parent)
        {
            // Header
            Label headerLabel = new Label
            {
                Text = "👤 CHỌN NHÂN VIÊN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 35,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 10, 0, 0)
            };
            parent.Controls.Add(headerLabel);

            // Content Panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            parent.Controls.Add(contentPanel);

            // Employee Search
            Label searchLabel = new Label
            {
                Text = "Tìm kiếm nhân viên:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(270, 20),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            contentPanel.Controls.Add(searchLabel);

            TextBox searchBox = new TextBox
            {
                Location = new Point(0, 25),
                Size = new Size(270, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Nhập mã hoặc tên nhân viên..."
            };
            contentPanel.Controls.Add(searchBox);

            // Employee List
            Label listLabel = new Label
            {
                Text = "Danh sách nhân viên:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 70),
                Size = new Size(270, 20),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            contentPanel.Controls.Add(listLabel);

            ListBox employeeList = new ListBox
            {
                Name = "employeeListBox",
                Location = new Point(0, 95),
                Size = new Size(270, 200),
                Font = new Font("Segoe UI", 9),
                IntegralHeight = false
            };
            contentPanel.Controls.Add(employeeList);

            // Selected Employee Info
            Panel infoPanel = new Panel
            {
                Location = new Point(0, 310),
                Size = new Size(270, 120),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(infoPanel);

            Label infoHeaderLabel = new Label
            {
                Text = "Thông tin nhân viên:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(250, 20),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            infoPanel.Controls.Add(infoHeaderLabel);

            Label employeeInfo = new Label
            {
                Name = "lblEmployeeInfo",
                Text = "Chưa chọn nhân viên",
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 35),
                Size = new Size(250, 75),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            infoPanel.Controls.Add(employeeInfo);

            // Registration Status
            Panel statusPanel = new Panel
            {
                Location = new Point(0, 445),
                Size = new Size(270, 60),
                BackColor = Color.FromArgb(52, 152, 219)
            };
            contentPanel.Controls.Add(statusPanel);

            Label statusLabel = new Label
            {
                Name = "lblRegistrationStatus",
                Text = "Sẵn sàng đăng ký",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            statusPanel.Controls.Add(statusLabel);
        }

        private void CreateCameraSection(Panel parent)
        {
            // Header
            Label headerLabel = new Label
            {
                Text = "📹 CAMERA ĐĂNG KÝ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 35,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 10, 0, 0)
            };
            parent.Controls.Add(headerLabel);

            // Content Panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            parent.Controls.Add(contentPanel);

            // Camera Display
            cameraBox = new PictureBox
            {
                Name = "pictureBoxCamera",
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, 0),
                Size = new Size(370, 280)
            };
            contentPanel.Controls.Add(cameraBox);

            // Camera placeholder
            var cameraPlaceholder = new Label
            {
                Name = "cameraPlaceholder",
                Text = "Vùng hiển thị camera\nNhấn 'Bật Camera' để bắt đầu",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            cameraBox.Controls.Add(cameraPlaceholder);

            // Camera Controls
            var controlsPanel = new Panel
            {
                Location = new Point(0, 295),
                Size = new Size(370, 50)
            };
            contentPanel.Controls.Add(controlsPanel);

            btnStartCamera = new Button
            {
                Text = "🎥 Bật Camera",
                Size = new Size(110, 40),
                Location = new Point(0, 5),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnStartCamera.FlatAppearance.BorderSize = 0;
            btnStartCamera.Click += BtnStartCamera_Click;
            controlsPanel.Controls.Add(btnStartCamera);

            btnStopCamera = new Button
            {
                Text = "⏹️ Tắt Camera",
                Size = new Size(110, 40),
                Location = new Point(130, 5),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnStopCamera.FlatAppearance.BorderSize = 0;
            btnStopCamera.Click += BtnStopCamera_Click;
            controlsPanel.Controls.Add(btnStopCamera);

            btnCaptureImage = new Button
            {
                Name = "btnCaptureImage",
                Text = "📸 Chụp Ảnh",
                Size = new Size(110, 40),
                Location = new Point(260, 5),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnCaptureImage.FlatAppearance.BorderSize = 0;
            btnCaptureImage.Click += BtnCaptureImage_Click;
            controlsPanel.Controls.Add(btnCaptureImage);

            // Progress Panel
            Panel progressPanel = new Panel
            {
                Location = new Point(0, 360),
                Size = new Size(370, 80),
                BackColor = Color.FromArgb(241, 196, 15)
            };
            contentPanel.Controls.Add(progressPanel);

            Label progressLabel = new Label
            {
                Name = "lblProgress",
                Text = $"Đã chụp: {capturedImages}/{REQUIRED_IMAGES} ảnh",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 15),
                Size = new Size(350, 25)
            };
            progressPanel.Controls.Add(progressLabel);

            // Progress Bar
            ProgressBar progressBar = new ProgressBar
            {
                Name = "progressBarCapture",
                Location = new Point(10, 45),
                Size = new Size(350, 20),
                Maximum = REQUIRED_IMAGES,
                Value = 0
            };
            progressPanel.Controls.Add(progressBar);

            // Instructions Panel
            Panel instructionsPanel = new Panel
            {
                Location = new Point(0, 455),
                Size = new Size(370, 80),
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(10)
            };
            contentPanel.Controls.Add(instructionsPanel);

            Label instructionsLabel = new Label
            {
                Text = "💡 Hướng dẫn:\n• Nhìn thẳng vào camera\n• Giữ khuôn mặt trong khung\n• Chụp ít nhất 5 ảnh từ các góc khác nhau",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Fill
            };
            instructionsPanel.Controls.Add(instructionsLabel);
        }

        private void CreateCapturedImagesSection(Panel parent)
        {
            // Header
            Label headerLabel = new Label
            {
                Text = "🖼️ ẢNH ĐÃ CHỤP",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Height = 35,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 10, 0, 0)
            };
            parent.Controls.Add(headerLabel);

            // Content Panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true
            };
            parent.Controls.Add(contentPanel);

            // Images Container
            FlowLayoutPanel imagesPanel = new FlowLayoutPanel
            {
                Name = "imagesPanel",
                Location = new Point(0, 0),
                Size = new Size(200, 400),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };
            contentPanel.Controls.Add(imagesPanel);

            // Action Buttons Panel
            Panel actionPanel = new Panel
            {
                Location = new Point(0, 420),
                Size = new Size(200, 120),
                BackColor = Color.FromArgb(236, 240, 241)
            };
            contentPanel.Controls.Add(actionPanel);

            // Clear All Button
            Button btnClearAll = new Button
            {
                Text = "🗑️ Xóa Tất Cả",
                Size = new Size(180, 35),
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearAll.FlatAppearance.BorderSize = 0;
            btnClearAll.Click += BtnClearAll_Click;
            actionPanel.Controls.Add(btnClearAll);

            // Save Registration Button
            Button btnSaveRegistration = new Button
            {
                Name = "btnSaveRegistration",
                Text = "💾 Lưu Đăng Ký",
                Size = new Size(180, 35),
                Location = new Point(10, 55),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSaveRegistration.FlatAppearance.BorderSize = 0;
            btnSaveRegistration.Click += BtnSaveRegistration_Click;
            actionPanel.Controls.Add(btnSaveRegistration);

            // Bottom Panel - Instructions
            Panel bottomPanel = new Panel
            {
                Location = new Point(0, 555),
                Size = new Size(200, 60),
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(10)
            };
            contentPanel.Controls.Add(bottomPanel);

            Label bottomLabel = new Label
            {
                Text = "Cần chụp tối thiểu 5 ảnh\nđể hoàn tất đăng ký",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            bottomPanel.Controls.Add(bottomLabel);
        }

        private void LoadEmployeeList()
        {
            try
            {
                // Demo data - trong thực tế sẽ load từ database
                var employeeListControls = this.Controls.Find("employeeListBox", true);
                if (employeeListControls.Length > 0 && employeeListControls[0] is ListBox employeeList)
                {
                    employeeList.Items.Clear(); // Clear existing items first
                    employeeList.Items.Add("NV001 - Nguyễn Văn An");
                    employeeList.Items.Add("NV002 - Trần Thị Bình");
                    employeeList.Items.Add("NV003 - Lê Văn Cường");
                    employeeList.Items.Add("NV004 - Phạm Thị Dung");
                    employeeList.Items.Add("NV005 - Hoàng Văn Em");
                    employeeList.Items.Add("NV006 - Ngô Thị Phương");
                    employeeList.Items.Add("NV007 - Vũ Văn Giang");
                    employeeList.Items.Add("NV008 - Đặng Thị Hương");

                    employeeList.SelectedIndexChanged += EmployeeList_SelectedIndexChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EmployeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem != null)
            {
                string selectedEmployee = listBox.SelectedItem.ToString() ?? "";

                // Update employee info display
                var infoControls = this.Controls.Find("lblEmployeeInfo", true);
                if (infoControls.Length > 0 && infoControls[0] is Label infoLabel)
                {
                    // Demo info - trong thực tế sẽ load từ database
                    var parts = selectedEmployee.Split(' ');
                    if (parts.Length >= 3) // Đảm bảo có đủ phần tử
                    {
                        string empCode = parts[0];
                        string empName = string.Join(" ", parts, 2, parts.Length - 2);

                        infoLabel.Text = $"Mã: {empCode}\n" +
                                       $"Tên: {empName}\n" +
                                       $"Phòng ban: IT\n" +
                                       $"Chức vụ: Nhân viên";
                    }
                }

                // Update status
                var statusControls = this.Controls.Find("lblRegistrationStatus", true);
                if (statusControls.Length > 0 && statusControls[0] is Label statusLabel)
                {
                    statusLabel.Text = "Đã chọn nhân viên";
                    if (statusLabel.Parent != null)
                    {
                        statusLabel.Parent.BackColor = Color.FromArgb(46, 204, 113);
                    }
                }

                // Enable capture button if camera is ready
                var captureControls = this.Controls.Find("btnCaptureImage", true);
                if (captureControls.Length > 0 && captureControls[0] is Button captureBtn)
                {
                    captureBtn.Enabled = true; // Assume camera is ready
                }
            }
        }

        private void BtnCaptureImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Capture real image from camera if available
                Bitmap? capturedImage = null;

                if (isCameraRunning && cameraBox?.Image != null)
                {
                    // Capture from camera
                    capturedImage = new Bitmap(cameraBox.Image);

                    // Save captured image
                    string captureDir = System.IO.Path.Combine(Application.StartupPath, "FaceRegistration");
                    if (!System.IO.Directory.Exists(captureDir))
                    {
                        System.IO.Directory.CreateDirectory(captureDir);
                    }

                    string fileName = $"face_{DateTime.Now:yyyyMMdd_HHmmss}_{capturedImages + 1}.jpg";
                    string filePath = System.IO.Path.Combine(captureDir, fileName);
                    capturedImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                // Increment counter
                capturedImages++;

                // Add captured image to the panel
                var imagesPanelControls = this.Controls.Find("imagesPanel", true);
                if (imagesPanelControls.Length > 0 && imagesPanelControls[0] is FlowLayoutPanel imagesPanel)
                {
                    var imageContainer = new Panel
                    {
                        Size = new Size(180, 120),
                        BackColor = Color.FromArgb(236, 240, 241),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 10)
                    };

                    // Create thumbnail
                    if (capturedImage != null)
                    {
                        var thumbnail = new PictureBox
                        {
                            Image = new Bitmap(capturedImage, new Size(160, 90)),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Location = new Point(10, 10),
                            Size = new Size(160, 90)
                        };
                        imageContainer.Controls.Add(thumbnail);

                        // Add image number label
                        var numberLabel = new Label
                        {
                            Text = $"#{capturedImages}",
                            Font = new Font("Segoe UI", 8, FontStyle.Bold),
                            ForeColor = Color.White,
                            BackColor = Color.FromArgb(52, 152, 219),
                            Location = new Point(10, 100),
                            Size = new Size(30, 15),
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        imageContainer.Controls.Add(numberLabel);
                    }
                    else
                    {
                        var imageLabel = new Label
                        {
                            Text = $"Ảnh {capturedImages}\n📸",
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            ForeColor = Color.FromArgb(52, 73, 94),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Dock = DockStyle.Fill
                        };
                        imageContainer.Controls.Add(imageLabel);
                    }

                    // Add delete button for each image
                    var deleteBtn = new Button
                    {
                        Text = "❌",
                        Size = new Size(25, 25),
                        Location = new Point(150, 5),
                        BackColor = Color.FromArgb(231, 76, 60),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 8, FontStyle.Bold),
                        Cursor = Cursors.Hand
                    };
                    deleteBtn.FlatAppearance.BorderSize = 0;
                    deleteBtn.Click += (s, ev) => {
                        imagesPanel.Controls.Remove(imageContainer);
                        capturedImages--;
                        UpdateProgress();
                        UpdateSaveButton();
                        imageContainer.Dispose(); // Clean up resources
                    };
                    imageContainer.Controls.Add(deleteBtn);

                    imagesPanel.Controls.Add(imageContainer);
                }

                UpdateProgress();
                UpdateSaveButton();

                // Show success message
                MessageBox.Show($"Đã chụp ảnh {capturedImages}!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Dispose the captured image after use
                capturedImage?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Camera Methods

        private void BtnStartCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                // Find available video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera nào!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Use first camera
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                // Set up event handler for new frames
                videoSource.NewFrame += VideoSource_NewFrame;

                // Set video resolution if available
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    videoSource.VideoResolution = videoSource.VideoCapabilities[0];
                }

                // Start capture
                videoSource.Start();

                // Update UI
                isCameraRunning = true;
                btnStartCamera!.Enabled = false;
                btnStopCamera!.Enabled = true;
                btnCaptureImage!.Enabled = true;

                // Hide placeholder text
                var placeholder = cameraBox?.Controls.Find("cameraPlaceholder", false);
                if (placeholder?.Length > 0)
                {
                    placeholder[0].Visible = false;
                }

                MessageBox.Show("Camera đã được khởi động!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động camera: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStopCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                StopCamera();
                MessageBox.Show("Camera đã được tắt!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tắt camera: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Check if form is being disposed
                if (this.IsDisposed || this.Disposing)
                    return;

                // Clone frame to avoid cross-thread issues
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Update PictureBox on UI thread safely
                if (cameraBox != null && !cameraBox.IsDisposed)
                {
                    if (cameraBox.InvokeRequired)
                    {
                        cameraBox.BeginInvoke(new MethodInvoker(() => {
                            try
                            {
                                if (!cameraBox.IsDisposed)
                                {
                                    cameraBox.Image?.Dispose(); // Dispose old image
                                    cameraBox.Image = frame;
                                }
                                else
                                {
                                    frame.Dispose(); // Dispose if control is disposed
                                }
                            }
                            catch
                            {
                                frame.Dispose();
                            }
                        }));
                    }
                    else
                    {
                        cameraBox.Image?.Dispose(); // Dispose old image
                        cameraBox.Image = frame;
                    }
                }
                else
                {
                    frame.Dispose(); // Dispose if cameraBox is null or disposed
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show MessageBox to avoid spam
                System.Diagnostics.Debug.WriteLine($"Error in VideoSource_NewFrame: {ex.Message}");
            }
        }

        private void StopCamera()
        {
            try
            {
                if (videoSource != null)
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();

                        // Wait for stop with timeout
                        var stopTime = DateTime.Now.AddSeconds(3);
                        while (videoSource.IsRunning && DateTime.Now < stopTime)
                        {
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(50);
                        }

                        // Force stop if still running
                        if (videoSource.IsRunning)
                        {
                            videoSource.Stop();
                        }
                    }

                    videoSource.NewFrame -= VideoSource_NewFrame;
                    videoSource = null;
                }

                // Update UI safely
                if (btnStartCamera != null && !btnStartCamera.IsDisposed)
                {
                    btnStartCamera.Enabled = true;
                }
                if (btnStopCamera != null && !btnStopCamera.IsDisposed)
                {
                    btnStopCamera.Enabled = false;
                }
                if (btnCaptureImage != null && !btnCaptureImage.IsDisposed)
                {
                    btnCaptureImage.Enabled = false;
                }

                // Clear image and show placeholder safely
                if (cameraBox != null && !cameraBox.IsDisposed)
                {
                    if (cameraBox.Image != null)
                    {
                        cameraBox.Image.Dispose();
                        cameraBox.Image = null;
                    }

                    var placeholder = cameraBox.Controls.Find("cameraPlaceholder", false);
                    if (placeholder?.Length > 0 && !placeholder[0].IsDisposed)
                    {
                        placeholder[0].Visible = true;
                    }
                }

                isCameraRunning = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping camera: {ex.Message}");
                // Don't show MessageBox here as it might cause more issues during form closing
            }
        }

        #endregion

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (capturedImages > 0)
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa tất cả ảnh đã chụp?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        var imagesPanelControls = this.Controls.Find("imagesPanel", true);
                        if (imagesPanelControls.Length > 0 && imagesPanelControls[0] is FlowLayoutPanel imagesPanel)
                        {
                            imagesPanel.Controls.Clear();
                        }

                        capturedImages = 0;
                        UpdateProgress();
                        UpdateSaveButton();

                        MessageBox.Show("Đã xóa tất cả ảnh!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveRegistration_Click(object sender, EventArgs e)
        {
            try
            {
                var employeeListControls = this.Controls.Find("employeeListBox", true);
                if (employeeListControls.Length == 0 || !(employeeListControls[0] is ListBox employeeList) || employeeList.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (capturedImages < REQUIRED_IMAGES)
                {
                    MessageBox.Show($"Cần chụp ít nhất {REQUIRED_IMAGES} ảnh để hoàn tất đăng ký!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Xác nhận đăng ký khuôn mặt cho nhân viên:\n{employeeList.SelectedItem}\n\nSố ảnh: {capturedImages}",
                    "Xác nhận đăng ký", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Simulate saving process
                    MessageBox.Show("Đăng ký khuôn mặt thành công!\nNhân viên có thể sử dụng chấm công bằng khuôn mặt.",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset form
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đăng ký: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProgress()
        {
            var progressControls = this.Controls.Find("lblProgress", true);
            if (progressControls.Length > 0 && progressControls[0] is Label progressLabel)
            {
                progressLabel.Text = $"Đã chụp: {capturedImages}/{REQUIRED_IMAGES} ảnh";
            }

            var progressBarControls = this.Controls.Find("progressBarCapture", true);
            if (progressBarControls.Length > 0 && progressBarControls[0] is ProgressBar progressBar)
            {
                progressBar.Value = Math.Min(capturedImages, REQUIRED_IMAGES);
            }
        }

        private void UpdateSaveButton()
        {
            var saveControls = this.Controls.Find("btnSaveRegistration", true);
            if (saveControls.Length > 0 && saveControls[0] is Button saveBtn)
            {
                saveBtn.Enabled = capturedImages >= REQUIRED_IMAGES;
                if (saveBtn.Enabled)
                {
                    saveBtn.BackColor = Color.FromArgb(46, 204, 113);
                    saveBtn.Text = "💾 Lưu Đăng Ký";
                }
                else
                {
                    saveBtn.BackColor = Color.FromArgb(149, 165, 166);
                    saveBtn.Text = $"💾 Cần thêm {REQUIRED_IMAGES - capturedImages} ảnh";
                }
            }
        }

        private void ResetForm()
        {
            try
            {
                // Clear captured images
                var imagesPanelControls = this.Controls.Find("imagesPanel", true);
                if (imagesPanelControls.Length > 0 && imagesPanelControls[0] is FlowLayoutPanel imagesPanel)
                {
                    imagesPanel.Controls.Clear();
                }

                // Reset counters
                capturedImages = 0;
                UpdateProgress();
                UpdateSaveButton();

                // Clear employee selection
                var employeeListControls = this.Controls.Find("employeeListBox", true);
                if (employeeListControls.Length > 0 && employeeListControls[0] is ListBox employeeList)
                {
                    employeeList.ClearSelected();
                }

                // Reset employee info
                var infoControls = this.Controls.Find("lblEmployeeInfo", true);
                if (infoControls.Length > 0 && infoControls[0] is Label infoLabel)
                {
                    infoLabel.Text = "Chưa chọn nhân viên";
                }

                // Reset status
                var statusControls = this.Controls.Find("lblRegistrationStatus", true);
                if (statusControls.Length > 0 && statusControls[0] is Label statusLabel)
                {
                    statusLabel.Text = "Sẵn sàng đăng ký";
                    if (statusLabel.Parent != null)
                    {
                        statusLabel.Parent.BackColor = Color.FromArgb(52, 152, 219);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset form: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Stop camera immediately
                StopCamera();

                // Small delay to ensure camera is fully stopped
                System.Threading.Thread.Sleep(100);

                // Force garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during form closing: {ex.Message}");
            }

            base.OnFormClosing(e);
        }
    }
}