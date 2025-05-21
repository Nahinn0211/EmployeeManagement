using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EmployeeManagement.GUI.Auth
{
    public partial class LoginForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        private bool isLoginSuccessful = false; // Flag để theo dõi trạng thái đăng nhập

        public LoginForm()
        {
            // Khởi tạo Material Skin trước
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600,
                Primary.Blue700,
                Primary.Blue200,
                Accent.LightBlue200,
                TextShade.WHITE
            );

            InitializeComponent();
            SetupForm();
            CreateAllControls();
            SetupEventHandlers();
        }

        private void SetupForm()
        {
            this.Size = new Size(600, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Sizable = false;
            this.Text = "Hệ thống Quản lý Nhân viên - Đăng nhập";
            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;
        }

        private void CreateAllControls()
        {
            CreateMainCard();
            CreateLogoAndLabels();
            CreateInputControls();
            CreateButtons();
            CreateThemeControls();
            SetTabOrder();
        }

        private void CreateMainCard()
        {
            mainCard = new MaterialCard
            {
                Size = new Size(500, 580),
                Location = new Point(50, 80),
                BackColor = Color.FromArgb(255, 255, 255),
                Depth = 0,
                ForeColor = Color.FromArgb(222, 0, 0, 0),
                Margin = new Padding(14),
                MouseState = MaterialSkin.MouseState.HOVER,
                Padding = new Padding(20)
            };
            this.Controls.Add(mainCard);
        }

        private void CreateLogoAndLabels()
        {
            // Logo
            logoIcon = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(210, 30),
                Size = new Size(80, 80),
                SizeMode = PictureBoxSizeMode.CenterImage,
                TabStop = false,
                Image = CreateLogo()
            };
            mainCard.Controls.Add(logoIcon);

            // Tiêu đề
            titleLabel = new MaterialLabel
            {
                AutoSize = false,
                Depth = 0,
                Font = new Font("Roboto", 28F, FontStyle.Bold, GraphicsUnit.Pixel),
                FontType = MaterialSkin.MaterialSkinManager.fontType.H4,
                Location = new Point(25, 130),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(450, 45),
                Text = "Chào mừng trở lại",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = materialSkinManager.ColorScheme.PrimaryColor
            };
            mainCard.Controls.Add(titleLabel);

            // Phụ đề
            subtitleLabel = new MaterialLabel
            {
                AutoSize = false,
                Depth = 0,
                Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel),
                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(25, 180),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(450, 30),
                Text = "Vui lòng đăng nhập vào tài khoản của bạn",
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainCard.Controls.Add(subtitleLabel);
        }

        private void CreateInputControls()
        {
            // Tên đăng nhập
            txtUsername = new MaterialTextBox
            {
                BorderStyle = BorderStyle.None,
                Depth = 0,
                Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel),
                Hint = "Tên đăng nhập",
                Location = new Point(50, 240),
                MaxLength = 50,
                MouseState = MaterialSkin.MouseState.OUT,
                Size = new Size(400, 50),
                Text = ""
            };
            mainCard.Controls.Add(txtUsername);

            // Mật khẩu
            txtPassword = new MaterialTextBox
            {
                BorderStyle = BorderStyle.None,
                Depth = 0,
                Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel),
                Hint = "Mật khẩu",
                Location = new Point(50, 320),
                MaxLength = 50,
                MouseState = MaterialSkin.MouseState.OUT,
                Password = true,
                Size = new Size(400, 50),
                Text = ""
            };
            mainCard.Controls.Add(txtPassword);

            // Checkbox ghi nhớ
            chkRememberMe = new MaterialCheckbox
            {
                AutoSize = true,
                Depth = 0,
                Location = new Point(50, 400),
                Margin = new Padding(0),
                MouseLocation = new Point(-1, -1),
                MouseState = MaterialSkin.MouseState.HOVER,
                ReadOnly = false,
                Text = "Ghi nhớ đăng nhập",
                UseVisualStyleBackColor = true
            };
            mainCard.Controls.Add(chkRememberMe);
        }

        private void CreateButtons()
        {
            // Quên mật khẩu
            btnForgotPassword = new MaterialButton
            {
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default,
                Depth = 0,
                HighEmphasis = false,
                Location = new Point(270, 405),
                Margin = new Padding(4, 6, 4, 6),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(180, 30),
                Text = "QUÊN MẬT KHẨU?",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text,
                UseAccentColor = true,
                UseVisualStyleBackColor = true
            };
            mainCard.Controls.Add(btnForgotPassword);

            // Nút đăng nhập
            btnLogin = new MaterialButton
            {
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default,
                Depth = 0,
                HighEmphasis = true,
                Location = new Point(50, 460),
                Margin = new Padding(4, 6, 4, 6),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(400, 48),
                Text = "ĐĂNG NHẬP",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = false,
                UseVisualStyleBackColor = true
            };
            mainCard.Controls.Add(btnLogin);

            // Đường phân cách
            divider = new MaterialDivider
            {
                BackColor = Color.FromArgb(30, 0, 0, 0),
                Depth = 0,
                Location = new Point(50, 530),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(400, 1)
            };
            mainCard.Controls.Add(divider);

            // Nút thoát
            btnExit = new MaterialButton
            {
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default,
                Depth = 0,
                HighEmphasis = false,
                Location = new Point(50, 550),
                Margin = new Padding(4, 6, 4, 6),
                MouseState = MaterialSkin.MouseState.HOVER,
                Size = new Size(400, 40),
                Text = "THOÁT ỨNG DỤNG",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text,
                UseAccentColor = false,
                UseVisualStyleBackColor = true
            };
            mainCard.Controls.Add(btnExit);

            // Thanh tiến trình
            progressBar = new ProgressBar
            {
                Location = new Point(100, 680),
                Size = new Size(400, 5),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            this.Controls.Add(progressBar);
        }

        private void CreateThemeControls()
        {
            // Giao diện sáng
            btnLightTheme = new MaterialButton
            {
                AutoSize = false,
                Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default,
                Depth = 0,
                HighEmphasis = false,
                Location = new Point(50, 700),
                Size = new Size(80, 32),
                Text = "SÁNG",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined,
                UseAccentColor = false
            };
            this.Controls.Add(btnLightTheme);

            // Giao diện tối
            btnDarkTheme = new MaterialButton
            {
                AutoSize = false,
                Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default,
                Depth = 0,
                HighEmphasis = false,
                Location = new Point(140, 700),
                Size = new Size(80, 32),
                Text = "TỐI",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined,
                UseAccentColor = false
            };
            this.Controls.Add(btnDarkTheme);

            // Bảng màu
            btnBlueScheme = new MaterialButton
            {
                AutoSize = false,
                Depth = 0,
                Location = new Point(340, 700),
                Size = new Size(70, 32),
                Text = "XANH",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text
            };
            this.Controls.Add(btnBlueScheme);

            btnGreenScheme = new MaterialButton
            {
                AutoSize = false,
                Depth = 0,
                Location = new Point(420, 700),
                Size = new Size(70, 32),
                Text = "LỤC",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text
            };
            this.Controls.Add(btnGreenScheme);

            btnOrangeScheme = new MaterialButton
            {
                AutoSize = false,
                Depth = 0,
                Location = new Point(500, 700),
                Size = new Size(80, 32),
                Text = "CAM",
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text
            };
            this.Controls.Add(btnOrangeScheme);
        }

        private void SetTabOrder()
        {
            txtUsername.TabIndex = 0;
            txtPassword.TabIndex = 1;
            chkRememberMe.TabIndex = 2;
            btnLogin.TabIndex = 3;
            this.AcceptButton = btnLogin;
        }

        private void SetupEventHandlers()
        {
            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += BtnExit_Click;
            btnForgotPassword.Click += BtnForgotPassword_Click;
            btnLightTheme.Click += (s, e) => SwitchTheme(MaterialSkinManager.Themes.LIGHT);
            btnDarkTheme.Click += (s, e) => SwitchTheme(MaterialSkinManager.Themes.DARK);
            btnBlueScheme.Click += (s, e) => SwitchColorScheme(Primary.Blue600);
            btnGreenScheme.Click += (s, e) => SwitchColorScheme(Primary.Green600);
            btnOrangeScheme.Click += (s, e) => SwitchColorScheme(Primary.Orange600);
            this.Load += (s, e) => LoadSettings();
            this.Shown += (s, e) => txtUsername.Focus();
        }

        private Image CreateLogo()
        {
            var logo = new Bitmap(80, 80);
            using (var g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(materialSkinManager.ColorScheme.PrimaryColor))
                {
                    g.FillEllipse(brush, 10, 10, 60, 60);
                }

                using (var whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(whiteBrush, 25, 25, 30, 30);

                    using (var darkBrush = new SolidBrush(materialSkinManager.ColorScheme.PrimaryColor))
                    {
                        // Cửa sổ
                        g.FillRectangle(darkBrush, 28, 28, 6, 6);
                        g.FillRectangle(darkBrush, 36, 28, 6, 6);
                        g.FillRectangle(darkBrush, 44, 28, 6, 6);
                        g.FillRectangle(darkBrush, 28, 36, 6, 6);
                        g.FillRectangle(darkBrush, 36, 36, 6, 6);
                        g.FillRectangle(darkBrush, 44, 36, 6, 6);
                        // Cửa ra vào
                        g.FillRectangle(darkBrush, 32, 44, 16, 11);
                    }
                }
            }
            return logo;
        }

        // Xử lý sự kiện
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("Vui lòng nhập tên đăng nhập", "Cảnh báo");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("Vui lòng nhập mật khẩu", "Cảnh báo");
                txtPassword.Focus();
                return;
            }

            SetControlsEnabled(false);

            try
            {
                await System.Threading.Tasks.Task.Delay(1500); // Mô phỏng quá trình xác thực

                if (await AuthenticateAsync(username, password))
                {
                    if (chkRememberMe.Checked)
                        SaveUsername(username);
                    else
                        ClearSavedUsername();

                    ShowMessage("Đăng nhập thành công! Chào mừng bạn trở lại.", "Thành công");
                    await System.Threading.Tasks.Task.Delay(1500);

                    // Đánh dấu đăng nhập thành công
                    isLoginSuccessful = true;

                    // Ẩn LoginForm trước
                    this.Hide();

                    // Dispose MaterialSkin của LoginForm trước khi hiển thị MainForm
                    materialSkinManager.RemoveFormToManage(this);

                    // Tạo và hiển thị MainForm
                    var mainForm = new MainForm();
                    mainForm.WindowState = FormWindowState.Maximized;

                    // Hiển thị MainForm và chạy như form chính
                    mainForm.ShowDialog();

                    // Sau khi MainForm đóng, thoát ứng dụng
                    Application.Exit();
                }
                else
                {
                    ShowMessage("Tên đăng nhập hoặc mật khẩu không chính xác", "Lỗi");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Đăng nhập thất bại: {ex.Message}", "Lỗi");
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void BtnForgotPassword_Click(object sender, EventArgs e)
        {
            ShowMessage("Chức năng quên mật khẩu sẽ được triển khai tại đây.\n\nVui lòng liên hệ với quản trị viên hệ thống.", "Thông tin");
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnLogin.Enabled)
                btnLogin.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        // Phương thức hỗ trợ
        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            chkRememberMe.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnForgotPassword.Enabled = enabled;
            progressBar.Visible = !enabled;
            this.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        private void SwitchTheme(MaterialSkinManager.Themes theme)
        {
            materialSkinManager.Theme = theme;
            var themeText = theme == MaterialSkinManager.Themes.LIGHT ? "sáng" : "tối";
            ShowMessage($"Đã chuyển sang giao diện {themeText}", "Thông tin");
        }

        private void SwitchColorScheme(Primary primaryColor)
        {
            materialSkinManager.ColorScheme = new ColorScheme(
                primaryColor, primaryColor, primaryColor,
                Accent.LightBlue200, TextShade.WHITE);

            logoIcon.Image = CreateLogo();
            titleLabel.ForeColor = materialSkinManager.ColorScheme.PrimaryColor;
            ShowMessage("Bảng màu đã được cập nhật", "Thông tin");
        }

        private void ShowMessage(string message, string type)
        {
            var icon = type switch
            {
                "Thành công" => MessageBoxIcon.Information,
                "Cảnh báo" => MessageBoxIcon.Warning,
                "Lỗi" => MessageBoxIcon.Error,
                _ => MessageBoxIcon.Information
            };
            MessageBox.Show(message, "Hệ thống Quản lý Nhân viên", MessageBoxButtons.OK, icon);
        }

        private async Task<bool> AuthenticateAsync(string username, string password)
        {
            await System.Threading.Tasks.Task.Delay(1000);
            var validCredentials = new Dictionary<string, string>
            {
                { "admin", "123456" },
                { "user", "password" },
                { "manager", "manager123" }
            };
            return validCredentials.ContainsKey(username.ToLower()) &&
                   validCredentials[username.ToLower()] == password;
        }

        // Quản lý cài đặt
        private void LoadSettings()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EmployeeManagement"))
                {
                    if (key?.GetValue("RememberMe") is true)
                    {
                        txtUsername.Text = key.GetValue("Username")?.ToString() ?? "";
                        chkRememberMe.Checked = true;
                        if (!string.IsNullOrEmpty(txtUsername.Text))
                            txtPassword.Focus();
                    }
                }
            }
            catch { /* Bỏ qua lỗi */ }
        }

        private void SaveUsername(string username)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EmployeeManagement"))
                {
                    key.SetValue("Username", username);
                    key.SetValue("RememberMe", true);
                }
            }
            catch { /* Bỏ qua lỗi */ }
        }

        private void ClearSavedUsername()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\EmployeeManagement", true))
                {
                    key?.DeleteValue("Username", false);
                    key?.DeleteValue("RememberMe", false);
                }
            }
            catch { /* Bỏ qua lỗi */ }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Nếu đăng nhập thành công, cho phép đóng form mà không hỏi
            if (isLoginSuccessful)
            {
                base.OnFormClosing(e);
                return;
            }

            // Nếu user click X và chưa đăng nhập thành công
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }
    }
}