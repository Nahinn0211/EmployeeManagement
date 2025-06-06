using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace EmployeeManagement.GUI.Attendance
{
    public partial class CameraCaptureForm : Form
    {
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private bool isCapturing = false;
        private readonly object lockObject = new object();

        public Image? CapturedImage { get; private set; }

        public CameraCaptureForm()
        {
            InitializeComponent();
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            try
            {
                // Lấy danh sách camera
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    lblStatus.Text = "❌ Không tìm thấy camera nào";
                    lblStatus.ForeColor = Color.Red;
                    btnStartCamera.Enabled = false;
                    ShowNoCameraMessage();
                    return;
                }

                // Populate camera combobox
                cmbCameras.Items.Clear();
                foreach (FilterInfo device in videoDevices)
                {
                    cmbCameras.Items.Add(device.Name);
                }

                if (cmbCameras.Items.Count > 0)
                {
                    cmbCameras.SelectedIndex = 0;
                    lblStatus.Text = $"✅ Tìm thấy {videoDevices.Count} camera";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi khởi tạo camera: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                btnStartCamera.Enabled = false;
                ShowError($"Lỗi khởi tạo camera: {ex.Message}");
            }
        }

        private void BtnStartCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbCameras.SelectedIndex == -1) return;

                lock (lockObject)
                {
                    if (isCapturing) return;

                    // Create video source
                    videoSource = new VideoCaptureDevice(videoDevices![cmbCameras.SelectedIndex].MonikerString);

                    // Set resolution
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        // Find best resolution (preferably 640x480 or similar)
                        var bestCapability = videoSource.VideoCapabilities[0];
                        foreach (var capability in videoSource.VideoCapabilities)
                        {
                            if (capability.FrameSize.Width == 640 && capability.FrameSize.Height == 480)
                            {
                                bestCapability = capability;
                                break;
                            }
                        }
                        videoSource.VideoResolution = bestCapability;
                    }

                    // Register event handler
                    videoSource.NewFrame += VideoSource_NewFrame;

                    // Start camera
                    videoSource.Start();

                    isCapturing = true;
                    btnStartCamera.Enabled = false;
                    btnStopCamera.Enabled = true;
                    btnCapture.Enabled = true;
                    cmbCameras.Enabled = false;

                    lblStatus.Text = "📹 Camera đang hoạt động";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khởi động camera: {ex.Message}");
            }
        }

        private void BtnStopCamera_Click(object? sender, EventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            try
            {
                lock (lockObject)
                {
                    if (videoSource != null)
                    {
                        // Unregister event handler
                        videoSource.NewFrame -= VideoSource_NewFrame;

                        if (videoSource.IsRunning)
                        {
                            videoSource.SignalToStop();

                            // Wait for stop in background thread
                            Task.Run(() =>
                            {
                                try
                                {
                                    videoSource?.WaitForStop();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error waiting for camera stop: {ex.Message}");
                                }
                            });
                        }

                        videoSource = null;
                    }

                    isCapturing = false;
                }

                // Update UI
                if (!this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnStartCamera.Enabled = true;
                        btnStopCamera.Enabled = false;
                        btnCapture.Enabled = false;
                        cmbCameras.Enabled = true;

                        // Clear camera display
                        var oldImage = pictureBoxCamera.Image;
                        pictureBoxCamera.Image = null;
                        oldImage?.Dispose();

                        lblStatus.Text = "⏹️ Camera đã tắt";
                        lblStatus.ForeColor = Color.FromArgb(117, 117, 117);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping camera: {ex.Message}");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (this.IsDisposed || pictureBoxCamera == null) return;

                // Clone frame to avoid access violations
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Update UI on main thread
                if (this.InvokeRequired)
                {
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            this.BeginInvoke(new Action(() => UpdateCameraDisplay(frame)));
                        }
                        catch (ObjectDisposedException)
                        {
                            frame?.Dispose();
                        }
                    }
                    else
                    {
                        frame?.Dispose();
                    }
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI update error: {ex.Message}");
                frame?.Dispose();
            }
        }

        private void BtnCapture_Click(object? sender, EventArgs e)
        {
            try
            {
                if (pictureBoxCamera.Image != null)
                {
                    // Dispose previous captured image
                    CapturedImage?.Dispose();

                    // Create copy of current image
                    CapturedImage = new Bitmap(pictureBoxCamera.Image);

                    // Show preview
                    var oldPreview = pictureBoxPreview.Image;
                    pictureBoxPreview.Image = new Bitmap(CapturedImage);
                    oldPreview?.Dispose();

                    btnOK.Enabled = true;
                    lblStatus.Text = "📸 Đã chụp ảnh thành công";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);

                    // Flash effect
                    FlashCaptureEffect();
                }
                else
                {
                    MessageBox.Show("Không có ảnh từ camera để chụp", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi chụp ảnh: {ex.Message}");
            }
        }

        private void FlashCaptureEffect()
        {
            var originalColor = pictureBoxCamera.BackColor;
            var flashTimer = new System.Windows.Forms.Timer { Interval = 100 };
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
                pictureBoxCamera.BackColor = flashCount % 2 == 0 ? Color.White : originalColor;

                if (flashCount >= 4)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                    pictureBoxCamera.BackColor = originalColor;
                }
            };
            flashTimer.Start();
        }

        private async void BtnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                if (CapturedImage == null)
                {
                    MessageBox.Show("Vui lòng chụp ảnh trước khi tiếp tục", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Disable buttons
                btnOK.Enabled = false;
                btnCancel.Enabled = false;
                btnCapture.Enabled = false;

                lblStatus.Text = "🔄 Đang xử lý...";
                lblStatus.ForeColor = Color.Blue;

                // Stop camera in background
                await Task.Run(() => StopCamera());
                await Task.Delay(500); // Wait for camera to fully stop

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi: {ex.Message}");

                // Re-enable buttons
                btnOK.Enabled = true;
                btnCancel.Enabled = true;
                btnCapture.Enabled = true;
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            try
            {
                StopCamera();
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in cancel: {ex.Message}");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ShowNoCameraMessage()
        {
            MessageBox.Show(
                "Không tìm thấy camera!\n\n" +
                "Vui lòng kiểm tra:\n" +
                "• Camera đã được kết nối và bật\n" +
                "• Driver camera đã cài đặt đúng\n" +
                "• Quyền camera trong Windows Settings\n" +
                "• Camera không bị ứng dụng khác sử dụng",
                "Không có Camera",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void ShowError(string message)
        {
            lblStatus.Text = $"❌ {message}";
            lblStatus.ForeColor = Color.Red;
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Stop camera
                StopCamera();

                // Clean up images
                pictureBoxCamera?.Image?.Dispose();
                pictureBoxPreview?.Image?.Dispose();

                // Only dispose CapturedImage if operation was cancelled
                if (this.DialogResult != DialogResult.OK)
                {
                    CapturedImage?.Dispose();
                    CapturedImage = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during form closing: {ex.Message}");
            }

            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Auto start camera if available
            if (cmbCameras.Items.Count > 0 && btnStartCamera.Enabled)
            {
                var timer = new System.Windows.Forms.Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (btnStartCamera.Enabled && !isCapturing)
                    {
                        BtnStartCamera_Click(null, EventArgs.Empty);
                    }
                };
                timer.Start();
            }
        }
    }
}