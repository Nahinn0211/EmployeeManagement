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
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isCapturing = false;

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
                    lblStatus.Text = "Không tìm thấy camera nào";
                    lblStatus.ForeColor = Color.Red;
                    btnStartCamera.Enabled = false;
                    return;
                }

                // Thêm camera vào ComboBox
                foreach (FilterInfo device in videoDevices)
                {
                    cmbCameras.Items.Add(device.Name);
                }

                if (cmbCameras.Items.Count > 0)
                {
                    cmbCameras.SelectedIndex = 0;
                    lblStatus.Text = $"Đã tìm thấy {videoDevices.Count} camera";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi khởi tạo camera: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                btnStartCamera.Enabled = false;
            }
        }

        private void BtnStartCamera_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbCameras.SelectedIndex == -1) return;

                // Tạo video source
                videoSource = new VideoCaptureDevice(videoDevices[cmbCameras.SelectedIndex].MonikerString);

                // Thiết lập resolution (tùy chọn)
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    videoSource.VideoResolution = videoSource.VideoCapabilities[0];
                }

                // Đăng ký event
                videoSource.NewFrame += VideoSource_NewFrame;

                // Bắt đầu capture
                videoSource.Start();

                isCapturing = true;
                btnStartCamera.Enabled = false;
                btnStopCamera.Enabled = true;
                btnCapture.Enabled = true;
                cmbCameras.Enabled = false;

                lblStatus.Text = "Camera đang hoạt động";
                lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động camera: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }

                isCapturing = false;
                btnStartCamera.Enabled = true;
                btnStopCamera.Enabled = false;
                btnCapture.Enabled = false;
                cmbCameras.Enabled = true;

                pictureBoxCamera.Image = null;
                lblStatus.Text = "Camera đã tắt";
                lblStatus.ForeColor = Color.FromArgb(117, 117, 117);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tắt camera: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Clone frame to avoid access violation
                var frame = (Bitmap)eventArgs.Frame.Clone();

                // Update UI on main thread
                if (pictureBoxCamera.InvokeRequired)
                {
                    pictureBoxCamera.Invoke(new Action(() =>
                    {
                        var oldImage = pictureBoxCamera.Image;
                        pictureBoxCamera.Image = frame;
                        oldImage?.Dispose();
                    }));
                }
                else
                {
                    var oldImage = pictureBoxCamera.Image;
                    pictureBoxCamera.Image = frame;
                    oldImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show MessageBox in video thread
                System.Diagnostics.Debug.WriteLine($"Video frame error: {ex.Message}");
            }
        }

        private void BtnCapture_Click(object? sender, EventArgs e)
        {
            try
            {
                if (pictureBoxCamera.Image != null)
                {
                    // Tạo bản copy của ảnh hiện tại
                    CapturedImage?.Dispose();
                    CapturedImage = new Bitmap(pictureBoxCamera.Image);

                    // Hiển thị preview
                    var oldPreview = pictureBoxPreview.Image;
                    pictureBoxPreview.Image = new Bitmap(CapturedImage);
                    oldPreview?.Dispose();

                    btnOK.Enabled = true;
                    lblStatus.Text = "Đã chụp ảnh thành công";
                    lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }
                else
                {
                    MessageBox.Show("Không có ảnh từ camera để chụp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi chụp ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            if (CapturedImage != null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Vui lòng chụp ảnh trước khi tiếp tục", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dọn dẹp camera
            StopCamera();

            // Dọn dẹp images
            pictureBoxCamera.Image?.Dispose();
            pictureBoxPreview.Image?.Dispose();

            // Chỉ dispose CapturedImage nếu Cancel
            if (this.DialogResult != DialogResult.OK)
            {
                CapturedImage?.Dispose();
                CapturedImage = null;
            }

            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Auto start camera nếu có
            if (cmbCameras.Items.Count > 0)
            {
                // Delay một chút để form load hoàn toàn
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 500;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (btnStartCamera.Enabled)
                    {
                        BtnStartCamera_Click(null, EventArgs.Empty);
                    }
                };
                timer.Start();
            }
        }
    }
}