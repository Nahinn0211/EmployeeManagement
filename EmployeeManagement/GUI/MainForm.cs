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
            // Header với Material Card (giữ nguyên)
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

            // Logo và subtitle (giữ nguyên code cũ)
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

            // Tạo menu items với phân quyền
            CreateMenuItemsWithPermissions(menuContainer);

            // Footer với user info
            var footerCard = CreateUserInfoFooter();

            // Thêm tất cả vào sidebar
            sidebarPanel.Controls.Add(menuContainer);
            sidebarPanel.Controls.Add(headerCard);
            sidebarPanel.Controls.Add(footerCard);
        }



        private void CreateMenuItemsWithPermissions(Panel menuContainer)
        {
            // Định nghĩa tất cả menu items
            var allMenuItems = new[]
            {
        new { Text = "Bảng điều khiển", Icon = "📊", Key = "Dashboard", Category = "General" },

        // Nhóm Quản lý nhân sự
        new { Text = "Quản lý Nhân viên", Icon = "👥", Key = "Employee", Category = "HR" },
        new { Text = "Quản lý Phòng ban", Icon = "🏢", Key = "Department", Category = "HR" },
        new { Text = "Quản lý Chức vụ", Icon = "⭐", Key = "Position", Category = "HR" },

        // Nhóm Quản lý dự án
        new { Text = "Quản lý Dự án", Icon = "📋", Key = "Project", Category = "Project" },
        new { Text = "Quản lý Công việc", Icon = "✅", Key = "Task", Category = "Project" },
        new { Text = "Quản lý Khách hàng", Icon = "🤝", Key = "Customer", Category = "Project" },

        // Nhóm Quản lý tài liệu
        new { Text = "Quản lý Tài liệu", Icon = "📁", Key = "Document", Category = "General" },

        // Nhóm Chấm công & Lương
        new { Text = "Chấm công", Icon = "⏰", Key = "Attendance", Category = "HR" },
        new { Text = "Quản lý Lương", Icon = "💰", Key = "Salary", Category = "Finance" },

        // Nhóm Tài chính
        new { Text = "Quản lý Tài chính", Icon = "💵", Key = "Finance", Category = "Finance" },
        new { Text = "Thu chi Dự án", Icon = "📝", Key = "ProjectFinance", Category = "Finance" },

        // Nhóm Báo cáo
        new { Text = "Báo cáo Nhân sự", Icon = "📈", Key = "HRReport", Category = "Report" },
        new { Text = "Báo cáo Dự án", Icon = "📊", Key = "ProjectReport", Category = "Report" },
        new { Text = "Báo cáo Tài chính", Icon = "📉", Key = "FinanceReport", Category = "Report" },

        // Nhóm Quản trị
        new { Text = "Quản lý Người dùng", Icon = "👤", Key = "UserManagement", Category = "Admin" },
        new { Text = "Phân quyền", Icon = "🔒", Key = "Permission", Category = "Admin" },
    };

            int yPos = 20;
            string currentCategory = "";

            foreach (var item in allMenuItems)
            {
                // Kiểm tra quyền truy cập
                if (!UserSession.HasMenuPermission(item.Key))
                    continue;

                // Thêm header cho category mới
                if (item.Category != currentCategory && item.Category != "General")
                {
                    if (yPos > 20) yPos += 10; // Thêm khoảng cách

                    var categoryLabel = new MaterialLabel
                    {
                        Text = GetCategoryTitle(item.Category),
                        Font = new Font("Roboto", 12, FontStyle.Bold),
                        FontType = MaterialSkinManager.fontType.Subtitle2,
                        ForeColor = Color.Gray,
                        Location = new Point(20, yPos),
                        Size = new Size(260, 25),
                        Depth = 0
                    };
                    menuContainer.Controls.Add(categoryLabel);
                    yPos += 35;
                    currentCategory = item.Category;
                }

                // Tạo button menu
                var button = CreateMaterialMenuButton(item.Text, item.Icon, item.Key);
                button.Location = new Point(10, yPos);
                menuContainer.Controls.Add(button);
                yPos += 70;
            }
        }
        private MaterialCard CreateUserInfoFooter()
        {
            var footerCard = new MaterialCard
            {
                Height = 170, 
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
                Name = "UserNameLabel"
            };

            var roleLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? $"Role: {UserSession.UserRole}" : "",
                Font = new Font("Roboto", 12),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = Color.Blue,
                Location = new Point(70, 45),
                Size = new Size(200, 20),
                Depth = 0,
                Name = "RoleLabel"
            };

            var statusLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? "● Trực tuyến" : "● Ngoại tuyến",
                Font = new Font("Roboto", 12),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = UserSession.IsLoggedIn ? Color.Green : Color.Red,
                Location = new Point(70, 70),
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
                Location = new Point(70, 90),
                Size = new Size(200, 15),
                Depth = 0
            };

            var logoutButton = new MaterialButton
            {
                Text = "🚪  ĐĂNG XUẤT",
                Size = new Size(260, 40),
                Location = new Point(10, 115),
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = false,
                BackColor = Color.FromArgb(244, 67, 54),
                HighEmphasis = true,
                Tag = "Logout"
            };
            logoutButton.Click += MenuButton_Click;

            footerCard.Controls.Add(userIcon);
            footerCard.Controls.Add(userLabel);
            footerCard.Controls.Add(roleLabel);
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
        private string GetCategoryTitle(string category)
        {
            return category switch
            {
                "HR" => "── NHÂN SỰ ──",
                "Project" => "── DỰ ÁN ──",
                "Finance" => "── TÀI CHÍNH ──",
                "Report" => "── BÁO CÁO ──",
                "Admin" => "── QUẢN TRỊ ──",
                _ => $"── {category.ToUpper()} ──"
            };
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
                         break;
                    case "Department":
                        OpenChildForm(new Department.DepartmentListForm());
                         break;
                    case "Position":
                        OpenChildForm(new Position.PositionListForm());
                         break;
                    // Nhóm Quản lý dự án
                    case "Project":
                        OpenChildForm(new Projects.ProjectListForm());
                         break;
                    case "Task":
                        OpenChildForm(new EmployeeManagement.GUI.Task.TaskListForm());
                         break;
                    case "Customer":
                         OpenChildForm(new Customer.CustomerListForm());
                         break;

                    // Nhóm Quản lý tài liệu
                    case "Document":
                        OpenChildForm(new Document.DocumentListForm());
                         break;

                    // Nhóm Chấm công & Lương
                    case "Attendance":
                        OpenChildForm(new Attendance.FaceAttendanceForm());
                        ShowUnderDevelopment("Chấm công");
                        break;
                    case "Salary":
                         OpenChildForm(new Salary.SalaryListForm());
                         break;

                    // Nhóm Tài chính
                    case "Finance":
                        OpenChildForm(new Finance.FinanceListForm());
                         break;
                    case "ProjectFinance":
                         OpenChildForm(new Finance.ProjectFinanceForm());
                         break;
                    // Nhóm Báo cáo
                    case "HRReport":
                         OpenChildForm(new Reports.HRReportForm());
                         break;
                    case "ProjectReport":
                        OpenChildForm(new Reports.ProjectReportForm());
                         break;
                    case "FinanceReport":
                        OpenChildForm(new Reports.FinanceReportForm());
                         break;

                    // Nhóm Quản trị
                    case "UserManagement":
                        OpenChildForm(new Admin.UserManagementForm());
                         break;
                    case "Permission":
                        OpenChildForm(new Admin.PermissionForm());
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

        public void UpdateUserInfo(string username)
        {
            try
            {
                var footerCard = sidebarPanel.Controls.OfType<MaterialCard>().LastOrDefault();
                if (footerCard != null)
                {
                    // Update username
                    var userLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "UserNameLabel");
                    if (userLabel != null)
                    {
                        userLabel.Text = username;
                    }

                    // Update role
                    var roleLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "RoleLabel");
                    if (roleLabel != null)
                    {
                        roleLabel.Text = $"Role: {UserSession.UserRole}";
                    }

                    // Update status
                    var statusLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "StatusLabel");
                    if (statusLabel != null)
                    {
                        statusLabel.Text = "● Trực tuyến";
                        statusLabel.ForeColor = Color.Green;
                    }
                }

                // Rebuild menu với quyền mới
                RebuildMenuWithPermissions();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}", ex);
            }
        }
        private void RebuildMenuWithPermissions()
        {
            try
            {
                var menuContainer = sidebarPanel.Controls
                    .OfType<Panel>()
                    .FirstOrDefault(p => p.Dock == DockStyle.Fill);

                if (menuContainer != null)
                {
                    menuContainer.Controls.Clear();
                    CreateMenuItemsWithPermissions(menuContainer);
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi rebuild menu: {ex.Message}", "OK", true);
                snackBar.Show(this);
            }
        }
    }

   
}