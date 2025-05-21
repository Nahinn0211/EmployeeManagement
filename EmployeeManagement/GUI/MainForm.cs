using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.GUI.Auth;
using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
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
        private readonly AuthBLL authBLL;
        private Button selectedButton = null;

        public MainForm()
        {
            authBLL = new AuthBLL();

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

        private void ShowLoginForm()
        {
            var loginForm = new LoginForm();

            loginForm.FormClosed += (sender, e) =>
            {
                var form = sender as LoginForm;
                if (UserSession.IsLoggedIn)
                {
                    // Nếu đăng nhập thành công, khởi tạo lại MainForm
                    this.Show();
                    this.WindowState = FormWindowState.Maximized;
                    this.BringToFront();

                    // Cập nhật thông tin user
                    UpdateUserInfo(UserSession.Username);

                    // Reset về Dashboard
                    OpenChildForm(new DashboardForm());
                }
                else
                {
                    // Nếu không đăng nhập, thoát ứng dụng
                    Application.Exit();
                }
            };

            loginForm.Show();
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

            // Footer với user info (sử dụng thông tin từ UserSession)
            var footerCard = CreateUserInfoFooter();

            // Thêm tất cả vào sidebar
            sidebarPanel.Controls.Add(menuContainer);
            sidebarPanel.Controls.Add(headerCard);
            sidebarPanel.Controls.Add(footerCard);
        }

        private MaterialCard CreateUserInfoFooter()
        {
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
                Text = UserSession.IsLoggedIn ? UserSession.Username : "Guest",
                Font = new Font("Roboto", 16, FontStyle.Bold),
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(70, 20),
                Size = new Size(200, 25),
                Depth = 0,
                Name = "UserNameLabel" // Đặt tên để dễ tìm khi cập nhật
            };

            var statusLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? "● Trực tuyến" : "● Ngoại tuyến",
                Font = new Font("Roboto", 12),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = UserSession.IsLoggedIn ? Color.Green : Color.Red,
                Location = new Point(70, 50),
                Size = new Size(200, 20),
                Depth = 0,
                Name = "StatusLabel"
            };

            var sessionInfoLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? $"Đăng nhập: {UserSession.LoginTime:HH:mm}" : "",
                Font = new Font("Roboto", 10),
                FontType = MaterialSkinManager.fontType.Caption,
                ForeColor = Color.Gray,
                Location = new Point(70, 70),
                Size = new Size(200, 15),
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
            footerCard.Controls.Add(sessionInfoLabel);
            footerCard.Controls.Add(logoutButton);

            return footerCard;
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
                    // Nhóm Chung
                    case "Dashboard":
                        OpenChildForm(new DashboardForm());
                        break;

                    // Nhóm Quản lý nhân sự
                    case "Employee":
                        OpenChildForm(new Employee.EmployeeListForm());
                        ShowUnderDevelopment("Quản lý Nhân viên");
                        break;
                    case "Department":
                        OpenChildForm(new Department.DepartmentListForm());
                        ShowUnderDevelopment("Quản lý Phòng ban");
                        break;
                    case "Position":
                        OpenChildForm(new Position.PositionListForm());
                        ShowUnderDevelopment("Quản lý Chức vụ");
                        break;

                    // Nhóm Quản lý dự án
                    case "Project":
                        OpenChildForm(new Projects.ProjectListForm());
                        ShowUnderDevelopment("Quản lý Dự án");
                        break;
                    case "Task":
                        OpenChildForm(new EmployeeManagement.GUI.Task.TaskListForm());
                        ShowUnderDevelopment("Quản lý Công việc");
                        break;
                    case "Customer":
                         OpenChildForm(new Customer.CustomerListForm());
                        ShowUnderDevelopment("Quản lý Khách hàng");
                        break;

                    // Nhóm Quản lý tài liệu
                    case "Document":
                        OpenChildForm(new Document.DocumentListForm());
                        ShowUnderDevelopment("Quản lý Tài liệu");
                        break;

                    // Nhóm Chấm công & Lương
                    case "Attendance":
                        // OpenChildForm(new Attendance.AttendanceForm());
                        ShowUnderDevelopment("Chấm công");
                        break;
                    case "Salary":
                         OpenChildForm(new Salary.SalaryListForm());
                        ShowUnderDevelopment("Quản lý Lương");
                        break;

                    // Nhóm Tài chính
                    case "Finance":
                        OpenChildForm(new Finance.FinanceListForm());
                        ShowUnderDevelopment("Quản lý Tài chính");
                        break;
                    case "ProjectFinance":
                         OpenChildForm(new Finance.ProjectFinanceForm());
                        ShowUnderDevelopment("Thu chi Dự án");
                        break;

                    // Nhóm Báo cáo
                    case "HRReport":
                        // OpenChildForm(new Reports.HRReportForm());
                        ShowUnderDevelopment("Báo cáo Nhân sự");
                        break;
                    case "ProjectReport":
                        OpenChildForm(new Reports.ProjectReportForm());
                        ShowUnderDevelopment("Báo cáo Dự án");
                        break;
                    case "FinanceReport":
                        OpenChildForm(new Reports.FinanceReportForm());
                        ShowUnderDevelopment("Báo cáo Tài chính");
                        break;

                    // Nhóm Quản trị
                    case "UserManagement":
                        OpenChildForm(new Admin.UserManagementForm());
                        ShowUnderDevelopment("Quản lý Người dùng");
                        break;
                    case "Permission":
                        //OpenChildForm(new Admin.PermissionForm());
                        ShowUnderDevelopment("Phân quyền");
                        break;
                    case "Settings":
                        //OpenChildForm(new Admin.SettingsForm());
                        ShowUnderDevelopment("Cài đặt Hệ thống");
                        break;

                    // Đăng xuất hệ thống
                    case "Logout":
                        PerformLogout();
                        break;
                    default:
                        ShowUnderDevelopment(menuKey);
                        break;
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi: {ex.Message}", "OK", true);
                snackBar.Show(this);
             }
        }

        private void ShowUnderDevelopment(string featureName)
        {
            MaterialSnackBar snackBar = new MaterialSnackBar($"Chức năng '{featureName}' đang được phát triển", "OK", true);
            snackBar.Show(this);
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

        // CHỨC NĂNG LOGOUT ĐÃ CẬP NHẬT
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

                    // Đăng xuất qua AuthBLL
                    authBLL.Logout();

                    // Ẩn MainForm
                    this.Hide();

                    // Hiển thị lại LoginForm
                    ShowLoginForm();
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

            // Đăng xuất trước khi thoát
            if (UserSession.IsLoggedIn)
            {
                authBLL.Logout();
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
                    var userLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "UserNameLabel");
                    if (userLabel != null)
                    {
                        userLabel.Text = username;
                    }

                    var statusLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "StatusLabel");
                    if (statusLabel != null)
                    {
                        statusLabel.Text = "● Trực tuyến";
                        statusLabel.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}", ex);
            }
        }
    }

    // Form Dashboard được cải thiện
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

            // Welcome message với thông tin user
            var welcomeLabel = new Label
            {
                Text = UserSession.IsLoggedIn ?
                    $"Chào mừng trở lại, {UserSession.Username}!" :
                    "Chào mừng bạn!",
                Font = new Font("Roboto", 16),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 70),
                AutoSize = true
            };

            // Stats cards container
            var statsPanel = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(1000, 200),
                BackColor = Color.Transparent
            };

            // Create stat cards với dữ liệu mẫu
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

            // Session info
            var sessionLabel = new Label
            {
                Text = UserSession.IsLoggedIn ?
                    $"Phiên đăng nhập: {UserSession.LoginTime:dd/MM/yyyy HH:mm:ss}" :
                    "Không có phiên đăng nhập",
                Font = new Font("Roboto", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(30, 350),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(welcomeLabel);
            this.Controls.Add(statsPanel);
            this.Controls.Add(sessionLabel);
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
}