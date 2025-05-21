using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagement.GUI.Auth;
using EmployeeManagement.GUI.Projects; // THÊM USING CHO PROJECTS
using MaterialSkin;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI
{
    public partial class MainForm : MaterialForm
    {
        private Form currentChildForm = null;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private readonly MaterialSkinManager materialSkinManager;
        private Button selectedButton = null;

        public MainForm()
        {
            // Khởi tạo Material Skin
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
            CreateLayout();
            SetupEventHandlers();

            // Hiển thị Dashboard mặc định
            this.Load += (s, e) => OpenChildForm(new DashboardForm());
        }

        private void SetupForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Hệ thống Quản lý Nhân viên";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 700);
            this.Sizable = true;
        }

        private void CreateLayout()
        {
            // Tạo sidebar panel với Material Design
            sidebarPanel = new Panel
            {
                Width = 300,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(5)
            };

            // Tạo content panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            // Thêm vào form theo thứ tự đúng
            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);

            // Tạo menu trong sidebar
            CreateSidebarMenu();
        }

        private void CreateSidebarMenu()
        {
            // Header với Material Card
            var headerCard = new MaterialCard
            {
                Height = 120,
                Dock = DockStyle.Top,
                BackColor = materialSkinManager.ColorScheme.PrimaryColor,
                Depth = 0,
                ForeColor = Color.FromArgb(222, 0, 0, 0),
                Margin = new Padding(5),
                MouseState = MaterialSkin.MouseState.HOVER,
                Padding = new Padding(20)
            };

            var logoLabel = new MaterialLabel
            {
                Text = "EMS",
                Font = new Font("Roboto", 32, FontStyle.Bold),
                FontType = MaterialSkinManager.fontType.H3,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 10),
                Size = new Size(260, 50),
                Depth = 0
            };

            var subtitleLabel = new MaterialLabel
            {
                Text = "Employee Management System",
                Font = new Font("Roboto", 14),
                FontType = MaterialSkinManager.fontType.Body1,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 65),
                Size = new Size(260, 25),
                Depth = 0
            };

            headerCard.Controls.Add(logoLabel);
            headerCard.Controls.Add(subtitleLabel);

            // Menu container
            var menuContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(10)
            };

            // Menu items với Material Buttons
            var menuItems = new[]
           {
                 new { Text = "Bảng điều khiển", Icon = "📊", Key = "Dashboard" },

                // Nhóm Quản lý nhân sự
                new { Text = "Quản lý Nhân viên", Icon = "👥", Key = "Employee" },
                new { Text = "Quản lý Phòng ban", Icon = "🏢", Key = "Department" },
                new { Text = "Quản lý Chức vụ", Icon = "⭐", Key = "Position" },
        
                // Nhóm Quản lý dự án
                new { Text = "Quản lý Dự án", Icon = "📋", Key = "Project" },
                new { Text = "Quản lý Công việc", Icon = "✅", Key = "Task" },
                new { Text = "Quản lý Khách hàng", Icon = "🤝", Key = "Customer" },
        
                // Nhóm Quản lý tài liệu
                new { Text = "Quản lý Tài liệu", Icon = "📁", Key = "Document" },
        
                // Nhóm Chấm công & Lương
                new { Text = "Chấm công", Icon = "⏰", Key = "Attendance" },
                new { Text = "Quản lý Lương", Icon = "💰", Key = "Salary" },
        
                // Nhóm Tài chính
                new { Text = "Quản lý Tài chính", Icon = "💵", Key = "Finance" },
                new { Text = "Thu chi Dự án", Icon = "📝", Key = "ProjectFinance" },
        
                // Nhóm Báo cáo
                new { Text = "Báo cáo Nhân sự", Icon = "📈", Key = "HRReport" },
                new { Text = "Báo cáo Dự án", Icon = "📊", Key = "ProjectReport" },
                new { Text = "Báo cáo Tài chính", Icon = "📉", Key = "FinanceReport" },
        
                // Nhóm Quản trị
                new { Text = "Quản lý Người dùng", Icon = "👤", Key = "UserManagement" },
                new { Text = "Phân quyền", Icon = "🔒", Key = "Permission" },
                new { Text = "Cài đặt Hệ thống", Icon = "⚙️", Key = "Settings" }
            };


            int yPos = 20;
            foreach (var item in menuItems)
            {
                var button = CreateMaterialMenuButton(item.Text, item.Icon, item.Key);
                button.Location = new Point(10, yPos);
                menuContainer.Controls.Add(button);
                yPos += 70;
            }

            // Footer với user info
            var footerCard = new MaterialCard
            {
                Height = 150,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                Depth = 0,
                Margin = new Padding(5),
                Padding = new Padding(15)
            };

            var userIcon = new MaterialLabel
            {
                Text = "👤",
                Font = new Font("Segoe UI", 24),
                Location = new Point(20, 20),
                Size = new Size(40, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var userLabel = new MaterialLabel
            {
                Text = "Admin",
                Font = new Font("Roboto", 16, FontStyle.Bold),
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(70, 20),
                Size = new Size(200, 25),
                Depth = 0
            };

            var statusLabel = new MaterialLabel
            {
                Text = "● Trực tuyến",
                Font = new Font("Roboto", 12),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = Color.Green,
                Location = new Point(70, 50),
                Size = new Size(200, 20),
                Depth = 0
            };

            var logoutButton = new MaterialButton
            {
                Text = "🚪  ĐĂNG XUẤT",
                Size = new Size(260, 40),
                Location = new Point(10, 90),
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = false,
                BackColor = Color.FromArgb(244, 67, 54),
                HighEmphasis = true,
                Tag = "Logout"
            };
            logoutButton.Click += MenuButton_Click;

            footerCard.Controls.Add(userIcon);
            footerCard.Controls.Add(userLabel);
            footerCard.Controls.Add(statusLabel);
            footerCard.Controls.Add(logoutButton);

            // Thêm tất cả vào sidebar
            sidebarPanel.Controls.Add(menuContainer);
            sidebarPanel.Controls.Add(headerCard);
            sidebarPanel.Controls.Add(footerCard);
        }

        private MaterialButton CreateMaterialMenuButton(string text, string icon, string key)
        {
            var button = new MaterialButton
            {
                Text = $"  {icon}   {text}",
                Size = new Size(280, 55),
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = false,
                HighEmphasis = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoSize = false,
                Cursor = Cursors.Hand,
                Tag = key,
                Font = new Font("Roboto", 14),
                Depth = 0
            };

            // Styling cho button
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(20, 10, 10, 10);

            button.Click += MenuButton_Click;
            button.MouseEnter += (s, e) =>
            {
                if (button != selectedButton)
                {
                    button.BackColor = Color.FromArgb(33, 150, 243, 50);
                }
            };
            button.MouseLeave += (s, e) =>
            {
                if (button != selectedButton)
                {
                    button.BackColor = Color.Transparent;
                }
            };

            return button;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button == null) return;

            var menuKey = button.Tag.ToString();

            // Update selected button style
            if (menuKey != "Logout")
            {
                if (selectedButton != null)
                {
                    selectedButton.BackColor = Color.Transparent;
                    selectedButton.ForeColor = materialSkinManager.ColorScheme.TextColor;
                }

                selectedButton = button;
                button.BackColor = Color.FromArgb(33, 150, 243, 100);
                button.ForeColor = materialSkinManager.ColorScheme.PrimaryColor;
            }

            try
            {
                switch (menuKey)
                {
                    case "Dashboard":
                        OpenChildForm(new DashboardForm());
                        break;
                    case "Employee":
                        OpenChildForm(new Employee.EmployeeListForm());
                        break;
                    case "Department":
                        OpenChildForm(new DepartmentForm());
                        break;
                    case "Projects":
                        OpenChildForm(new Projects.ProjectListForm());
                        break;
                    case "Attendance":
                        OpenChildForm(new AttendanceForm());
                        break;
                    case "Payroll":
                        OpenChildForm(new PayrollForm());
                        break;
                    case "Reports":
                        OpenChildForm(new ReportsForm());
                        break;
                    case "Settings":
                        OpenChildForm(new SettingsForm());
                        break;
                    case "Logout":
                        PerformLogout();
                        break;
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi: {ex.Message}", "OK", true);
                snackBar.Show(this);
            }
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                // Đóng form hiện tại
                if (currentChildForm != null)
                {
                    currentChildForm.Close();
                    currentChildForm.Dispose();
                }

                // Wrap child form in a MaterialCard for better appearance
                var cardContainer = new MaterialCard
                {
                    Dock = DockStyle.Fill,
                    Depth = 0,
                    ForeColor = Color.FromArgb(222, 0, 0, 0),
                    Location = new Point(0, 0),
                    MouseState = MaterialSkin.MouseState.HOVER,
                    Padding = new Padding(20)
                };

                // Thiết lập form con
                currentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                childForm.BackColor = Color.White;

                // Thêm vào card container
                cardContainer.Controls.Add(childForm);

                // Thêm vào content panel
                contentPanel.Controls.Clear();
                contentPanel.Controls.Add(cardContainer);
                childForm.Show();

                // Cập nhật tiêu đề
                this.Text = $"Hệ thống Quản lý Nhân viên - {childForm.Text}";
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi khi mở form: {ex.Message}", "OK", true);
                snackBar.Show(this);
            }
        }

        // CẬP NHẬT CHỨC NĂNG LOGOUT ĐỂ HIỂN THỊ LOGINFORM
        private void PerformLogout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Đóng form con hiện tại
                    if (currentChildForm != null)
                    {
                        currentChildForm.Close();
                        currentChildForm.Dispose();
                        currentChildForm = null;
                    }

                    // Ẩn MainForm
                    this.Hide();

                    // Tạo và hiển thị LoginForm
                    var loginForm = new LoginForm();

                    // Đăng ký sự kiện để xử lý khi LoginForm đóng
                    loginForm.FormClosed += (sender, e) =>
                    {
                        var form = sender as LoginForm;

                        if (form?.DialogResult == DialogResult.OK)
                        {
                            // Nếu đăng nhập thành công, hiển thị lại MainForm
                            this.Show();
                            this.BringToFront();
                            this.WindowState = FormWindowState.Maximized;

                            // Reset về Dashboard
                            selectedButton = null;
                            OpenChildForm(new DashboardForm());

                            // Reset button selection
                            foreach (Control control in sidebarPanel.Controls)
                            {
                                if (control is Panel panel)
                                {
                                    foreach (Control btn in panel.Controls)
                                    {
                                        if (btn is MaterialButton materialBtn && materialBtn.Tag?.ToString() == "Dashboard")
                                        {
                                            materialBtn.BackColor = Color.FromArgb(33, 150, 243, 100);
                                            materialBtn.ForeColor = materialSkinManager.ColorScheme.PrimaryColor;
                                            selectedButton = materialBtn;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Nếu không đăng nhập hoặc hủy, thoát ứng dụng
                            Application.Exit();
                        }
                    };

                    // Hiển thị LoginForm
                    loginForm.Show();
                }
                catch (Exception ex)
                {
                    MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi khi đăng xuất: {ex.Message}", "OK", true);
                    snackBar.Show(this);
                    this.Show(); // Hiển thị lại MainForm nếu có lỗi
                }
            }
        }

        private void SetupEventHandlers()
        {
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
            this.Activate();
            this.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
                currentChildForm.Dispose();
            }

            materialSkinManager.RemoveFormToManage(this);
            base.OnFormClosed(e);
            Application.Exit();
        }

        // Method để cập nhật thông tin user sau khi đăng nhập
        public void UpdateUserInfo(string username)
        {
            try
            {
                var footerCard = sidebarPanel.Controls.OfType<MaterialCard>().LastOrDefault();
                if (footerCard != null)
                {
                    var userLabel = footerCard.Controls.OfType<MaterialLabel>()
                                   .FirstOrDefault(l => l.Font.Bold);
                    if (userLabel != null)
                    {
                        userLabel.Text = username;
                    }
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi cập nhật thông tin user: {ex.Message}", "OK", true);
                snackBar.Show(this);
            }
        }
    }

    // Các form con được cải thiện với Material Design (giữ nguyên như code gốc)
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
            this.Text = "Bảng điều khiển";
            this.BackColor = Color.White;
            CreateDashboardContent();
        }

        private void CreateDashboardContent()
        {
            // Main title
            var titleLabel = new Label
            {
                Text = "Bảng điều khiển",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            // Stats cards container
            var statsPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(1000, 200),
                BackColor = Color.Transparent
            };

            // Create stat cards
            var statCards = new[]
            {
                new { Title = "Tổng nhân viên", Value = "150", Icon = "👥", Color = Color.FromArgb(33, 150, 243) },
                new { Title = "Có mặt hôm nay", Value = "142", Icon = "✅", Color = Color.FromArgb(76, 175, 80) },
                new { Title = "Nghỉ phép", Value = "5", Icon = "🏖️", Color = Color.FromArgb(255, 152, 0) },
                new { Title = "Vắng mặt", Value = "3", Icon = "❌", Color = Color.FromArgb(244, 67, 54) }
            };

            for (int i = 0; i < statCards.Length; i++)
            {
                var card = CreateStatCard(statCards[i].Title, statCards[i].Value, statCards[i].Icon, statCards[i].Color);
                card.Location = new Point(i * 240, 0);
                statsPanel.Controls.Add(card);
            }

            this.Controls.Add(titleLabel);
            this.Controls.Add(statsPanel);
        }

        private Panel CreateStatCard(string title, string value, string icon, Color color)
        {
            var card = new Panel
            {
                Size = new Size(220, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 36),
                ForeColor = color,
                Location = new Point(20, 20),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Roboto", 24, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(100, 20),
                Size = new Size(100, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Roboto", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 100),
                Size = new Size(180, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);

            // Add shadow effect
            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), rect);
            };

            return card;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "DashboardForm";
            this.ResumeLayout(false);
        }
    }

    public partial class EmployeeForm : Form
    {
        public EmployeeForm()
        {
            InitializeComponent();
            this.Text = "Quản lý Nhân viên";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Quản lý Nhân viên",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Quản lý thông tin và hồ sơ nhân viên",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "EmployeeForm";
            this.ResumeLayout(false);
        }
    }

    public partial class DepartmentForm : Form
    {
        public DepartmentForm()
        {
            InitializeComponent();
            this.Text = "Quản lý Phòng ban";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Quản lý Phòng ban",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Quản lý cơ cấu tổ chức và phòng ban",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "DepartmentForm";
            this.ResumeLayout(false);
        }
    }

    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
            this.Text = "Chấm công";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Quản lý Chấm công",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Theo dõi giờ làm việc và chấm công nhân viên",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "AttendanceForm";
            this.ResumeLayout(false);
        }
    }

    public partial class PayrollForm : Form
    {
        public PayrollForm()
        {
            InitializeComponent();
            this.Text = "Tính lương";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Quản lý Tính lương",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Tính toán và quản lý lương nhân viên",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "PayrollForm";
            this.ResumeLayout(false);
        }
    }

    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
            this.Text = "Báo cáo";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Báo cáo và Thống kê",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Tạo báo cáo và phân tích dữ liệu",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "ReportsForm";
            this.ResumeLayout(false);
        }
    }

    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.Text = "Cài đặt";
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Cài đặt Hệ thống",
                Font = new Font("Roboto", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Cấu hình và tùy chỉnh hệ thống",
                Font = new Font("Roboto", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "SettingsForm";
            this.ResumeLayout(false);
        }
    }
}