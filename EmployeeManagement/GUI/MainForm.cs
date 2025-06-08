using EmployeeManagement.GUI.Auth;
using EmployeeManagement.BLL;
using EmployeeManagement.Utilities;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

            // Hiển thị Dashboard nếu có quyền khi load
            this.Load += MainForm_Load_Enhanced;
        }

        private void MainForm_Load_Enhanced(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
            this.Activate();
            this.Focus();

            // Kiểm tra và mở form phù hợp sau khi load
            if (UserSession.IsLoggedIn)
            {
                // Kiểm tra session có hết hạn không
                if (UserSession.IsSessionExpired())
                {
                    MaterialSnackBar snackBar = new MaterialSnackBar("Phiên đăng nhập đã hết hạn!", "OK", true);
                    snackBar.Show(this);
                    UserSession.ClearSession();
                    ShowLoginForm();
                    return;
                }

                if (UserSession.HasMenuPermission("Dashboard"))
                {
                    OpenChildForm(new DashboardForm());
                }
                else
                {
                    // Mở form đầu tiên có quyền truy cập
                    var allowedMenus = PermissionManager.GetAllowedMenus(UserSession.CurrentUserRole);
                    if (allowedMenus.Count > 0)
                    {
                        ShowUnderDevelopment($"Welcome! Available menus: {allowedMenus.Count}");
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new MaterialSnackBar("Bạn không có quyền truy cập menu nào!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            else
            {
                // Nếu chưa đăng nhập, hiển thị form đăng nhập
                ShowLoginForm();
            }
        }
        private void ShowLoginForm()
        {
            var loginForm = new LoginForm();
            this.Hide(); // Ẩn MainForm trước

            loginForm.FormClosed += (sender, e) =>
            {
                var form = sender as LoginForm;
                if (UserSession.IsLoggedIn)
                {
                    // Nếu đăng nhập thành công, hiển thị lại MainForm
                    this.Show();
                    this.WindowState = FormWindowState.Maximized;
                    this.BringToFront();

                    // Cập nhật thông tin user
                    UpdateUserInfo(UserSession.CurrentUserName);

                    // Mở Dashboard nếu có quyền
                    if (UserSession.HasMenuPermission("Dashboard"))
                    {
                        OpenChildForm(new DashboardForm());
                    }
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

            // Logo và subtitle
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

        // PHƯƠNG THỨC TẠO MENU VỚI PHÂN QUYỀN - tương thích với PermissionManager hiện có
        private void CreateMenuItemsWithPermissions(Panel menuContainer)
        {
            // Định nghĩa tất cả menu items với thông tin chi tiết
            var allMenuItems = new[]
            {
                new { Text = "Bảng điều khiển", Icon = "📊", Key = "Dashboard", Category = "General", Description = "Tổng quan hệ thống" },

                // Nhóm Quản lý nhân sự
                new { Text = "Quản lý Nhân viên", Icon = "👥", Key = "Employee", Category = "HR", Description = "Quản lý thông tin nhân viên" },
                new { Text = "Quản lý Phòng ban", Icon = "🏢", Key = "Department", Category = "HR", Description = "Quản lý các phòng ban" },
                new { Text = "Quản lý Chức vụ", Icon = "⭐", Key = "Position", Category = "HR", Description = "Quản lý chức vụ và cấp bậc" },

                // Nhóm Quản lý dự án
                new { Text = "Quản lý Dự án", Icon = "📋", Key = "Project", Category = "Project", Description = "Quản lý các dự án" },
                new { Text = "Quản lý Công việc", Icon = "✅", Key = "Task", Category = "Project", Description = "Quản lý công việc trong dự án" },
                new { Text = "Quản lý Khách hàng", Icon = "🤝", Key = "Customer", Category = "Project", Description = "Quản lý thông tin khách hàng" },

                // Nhóm Quản lý tài liệu
                new { Text = "Quản lý Tài liệu", Icon = "📁", Key = "Document", Category = "General", Description = "Quản lý tài liệu công ty" },

                // Nhóm Chấm công & Lương
                new { Text = "Chấm công", Icon = "⏰", Key = "Attendance", Category = "HR", Description = "Chấm công nhân viên" },
                new { Text = "Đăng ký khuôn mặt", Icon = "👤", Key = "FaceRegistration", Category = "HR", Description = "Đăng ký nhận diện khuôn mặt" },
                new { Text = "Quản lý Lương", Icon = "💰", Key = "Salary", Category = "Finance", Description = "Quản lý lương nhân viên" },

                // Nhóm Tài chính
                new { Text = "Quản lý Tài chính", Icon = "💵", Key = "Finance", Category = "Finance", Description = "Quản lý tài chính công ty" },
                new { Text = "Thu chi Dự án", Icon = "📝", Key = "ProjectFinance", Category = "Finance", Description = "Quản lý thu chi theo dự án" },

                // Nhóm Báo cáo
                new { Text = "Báo cáo Nhân sự", Icon = "📈", Key = "HRReport", Category = "Report", Description = "Báo cáo về nhân sự" },
                new { Text = "Báo cáo Dự án", Icon = "📊", Key = "ProjectReport", Category = "Report", Description = "Báo cáo tiến độ dự án" },
                new { Text = "Báo cáo Tài chính", Icon = "📉", Key = "FinanceReport", Category = "Report", Description = "Báo cáo tài chính" },

                // Nhóm Quản trị
                new { Text = "Quản lý Người dùng", Icon = "👤", Key = "UserManagement", Category = "Admin", Description = "Quản lý tài khoản người dùng" },
                new { Text = "Phân quyền", Icon = "🔒", Key = "Permission", Category = "Admin", Description = "Quản lý phân quyền hệ thống" },
            };

            int yPos = 20;
            string currentCategory = "";
            int menuCount = 0;

            foreach (var item in allMenuItems)
            {
                // Kiểm tra quyền truy cập - sử dụng PermissionManager hiện có
                if (!UserSession.IsLoggedIn || !PermissionManager.HasMenuPermission(UserSession.CurrentUserRole, item.Key))
                    continue;

                menuCount++;

                // Thêm header cho category mới
                if (item.Category != currentCategory && item.Category != "General")
                {
                    if (yPos > 20) yPos += 15; // Thêm khoảng cách

                    var categoryLabel = new MaterialLabel
                    {
                        Text = GetCategoryTitle(item.Category),
                        Font = new Font("Roboto", 11, FontStyle.Bold),
                        FontType = MaterialSkinManager.fontType.Subtitle2,
                        ForeColor = Color.FromArgb(96, 125, 139), // Material Blue Grey
                        Location = new Point(20, yPos),
                        Size = new Size(260, 25),
                        Depth = 0
                    };
                    menuContainer.Controls.Add(categoryLabel);
                    yPos += 35;
                    currentCategory = item.Category;
                }

                // Tạo button menu với tooltip
                var button = CreateMaterialMenuButton(item.Text, item.Icon, item.Key);
                button.Location = new Point(10, yPos);

                // Thêm tooltip cho button
                var toolTip = new ToolTip();
                toolTip.SetToolTip(button, $"{item.Text}\n{item.Description}");

                menuContainer.Controls.Add(button);
                yPos += 70;
            }

            // Hiển thị thông tin về quyền truy cập
            if (menuCount == 0)
            {
                var noPermissionLabel = new MaterialLabel
                {
                    Text = "⚠️ Không có quyền truy cập menu nào",
                    Font = new Font("Roboto", 12),
                    FontType = MaterialSkinManager.fontType.Body1,
                    ForeColor = Color.Orange,
                    Location = new Point(20, yPos),
                    Size = new Size(260, 50),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Depth = 0
                };
                menuContainer.Controls.Add(noPermissionLabel);
            }
            else
            {
                // Hiển thị thông tin role hiện tại
                var roleInfoLabel = new MaterialLabel
                {
                    Text = $"Role: {UserSession.CurrentUserRole} | Menus: {menuCount}",
                    Font = new Font("Roboto", 10),
                    FontType = MaterialSkinManager.fontType.Caption,
                    ForeColor = Color.Gray,
                    Location = new Point(20, yPos + 20),
                    Size = new Size(260, 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Depth = 0
                };
                menuContainer.Controls.Add(roleInfoLabel);
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
                Text = UserSession.IsLoggedIn ? UserSession.CurrentUserName : "Guest",
                Font = new Font("Roboto", 16, FontStyle.Bold),
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(70, 20),
                Size = new Size(200, 25),
                Depth = 0,
                Name = "UserNameLabel"
            };

            var roleLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? PermissionManager.GetRoleDescription(UserSession.CurrentUserRole) : "",
                Font = new Font("Roboto", 10),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = Color.Blue,
                Location = new Point(70, 45),
                Size = new Size(200, 35),
                Depth = 0,
                Name = "RoleLabel"
            };

            var statusLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn ? "● Trực tuyến" : "● Ngoại tuyến",
                Font = new Font("Roboto", 12),
                FontType = MaterialSkinManager.fontType.Body2,
                ForeColor = UserSession.IsLoggedIn ? Color.Green : Color.Red,
                Location = new Point(70, 80),
                Size = new Size(200, 20),
                Depth = 0,
                Name = "StatusLabel"
            };

            var sessionInfoLabel = new MaterialLabel
            {
                Text = UserSession.IsLoggedIn && UserSession.LoginTime.HasValue ?
                       $"Đăng nhập: {UserSession.LoginTime:HH:mm}" : "",
                Font = new Font("Roboto", 10),
                FontType = MaterialSkinManager.fontType.Caption,
                ForeColor = Color.Gray,
                Location = new Point(70, 100),
                Size = new Size(200, 15),
                Depth = 0
            };

            var logoutButton = new MaterialButton
            {
                Text = "🚪  ĐĂNG XUẤT",
                Size = new Size(260, 40),
                Location = new Point(10, 120),
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

        // MENU BUTTON CLICK - sử dụng PermissionManager
        private void MenuButton_Click(object sender, EventArgs e)
        {

            var button = sender as MaterialButton;
            if (button == null) return;

             CheckAndRefreshSession();
            if (!UserSession.IsLoggedIn) return;

            var menuKey = button.Tag.ToString();

            // Kiểm tra quyền truy cập trước khi mở form (trừ Logout)
            if (menuKey != "Logout" &&
                (!UserSession.IsLoggedIn || !PermissionManager.HasMenuPermission(UserSession.CurrentUserRole, menuKey)))
            {
                MaterialSnackBar snackBar = new MaterialSnackBar(
                    "Bạn không có quyền truy cập chức năng này!",
                    "OK",
                    true);
                snackBar.Show(this);
                return;
            }

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
                // Ghi log truy cập menu
                LogMenuAccess(menuKey);

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
                        OpenChildForm(new WorkTask.TaskListForm());
                        break;
                    case "Customer":
                        OpenChildForm(new Customer.CustomerListForm());
                        break;

                    // Nhóm Quản lý tài liệu
                    case "Document":
                        OpenChildForm(new Document.DocumentListForm());
                        break;

                    // Nhóm Chấm công & Lương
                    case "FaceRegistration":
                        OpenChildForm(new Attendance.FaceRegistrationForm());
                        break;
                    case "Attendance":
                        OpenChildForm(new Attendance.FaceRecognitionForm());
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
                Logger.LogError($"Error opening form {menuKey}: {ex.Message}");
            }
        }

        // Ghi log truy cập menu
        private void LogMenuAccess(string menuKey)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                                  $"User: {UserSession.CurrentUserName} ({UserSession.CurrentUserRole}) " +
                                  $"accessed menu: {menuKey}";

                Logger.LogInfo(logMessage);
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging menu access: {ex.Message}");
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
                Logger.LogError($"Error opening child form: {ex.Message}");
            }
        }

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

                    // Đăng xuất qua AuthBLL và UserSession
                    authBLL.Logout();
                    UserSession.ClearSession();

                    // Ẩn MainForm
                    this.Hide();

                    // Hiển thị lại LoginForm
                    ShowLoginForm();
                }
                catch (Exception ex)
                {
                    MaterialSnackBar snackBar = new MaterialSnackBar($"Lỗi khi đăng xuất: {ex.Message}", "OK", true);
                    snackBar.Show(this);
                    Logger.LogError($"Logout error: {ex.Message}");
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
                UserSession.ClearSession();
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
                        userLabel.Text = UserSession.CurrentUserName; // Sử dụng CurrentUserName thay vì username parameter
                    }

                    // Update role description
                    var roleLabel = footerCard.Controls
                        .OfType<MaterialLabel>()
                        .FirstOrDefault(l => l.Name == "RoleLabel");
                    if (roleLabel != null)
                    {
                        roleLabel.Text = PermissionManager.GetRoleDescription(UserSession.CurrentUserRole);
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
        private void CheckAndRefreshSession()
        {
            if (UserSession.IsLoggedIn)
            {
                if (UserSession.IsSessionExpired())
                {
                    MaterialSnackBar snackBar = new MaterialSnackBar("Phiên đăng nhập đã hết hạn! Vui lòng đăng nhập lại.", "OK", true);
                    snackBar.Show(this);
                    PerformLogout();
                }
                else
                {
                    // Refresh session khi có hoạt động
                    UserSession.RefreshSession();
                }
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
                Logger.LogError($"Error rebuilding menu: {ex.Message}");
            }
        }

        /// <summary>
        /// Phương thức kiểm tra quyền trước khi thực hiện hành động
        /// </summary>
        /// <param name="action">Loại hành động (create, edit, delete, view, export)</param>
        /// <returns>True nếu có quyền thực hiện</returns>
        public bool CanPerformAction(string action)
        {
            if (!UserSession.IsLoggedIn)
                return false;

            return PermissionManager.HasActionPermission(UserSession.CurrentUserRole, action);
        }

        /// <summary>
        /// Hiển thị thông báo không có quyền
        /// </summary>
        /// <param name="action">Hành động bị từ chối</param>
        public void ShowAccessDenied(string action = "")
        {
            string message = string.IsNullOrEmpty(action)
                ? "Bạn không có quyền thực hiện hành động này!"
                : $"Bạn không có quyền {action}!";

            MaterialSnackBar snackBar = new MaterialSnackBar(message, "OK", true);
            snackBar.Show(this);
        }

        /// <summary>
        /// Lấy thông tin quyền của user hiện tại
        /// </summary>
        /// <returns>Chuỗi mô tả quyền</returns>
        public string GetCurrentUserPermissionInfo()
        {
            if (!UserSession.IsLoggedIn)
                return "Chưa đăng nhập";

            var allowedMenus = PermissionManager.GetAllowedMenus(UserSession.CurrentUserRole);
            var roleDescription = PermissionManager.GetRoleDescription(UserSession.CurrentUserRole);

            return $"User: {UserSession.CurrentUserName}\n" +
                   $"Role: {roleDescription}\n" +
                   $"Menu được phép: {allowedMenus.Count}\n" +
                   $"Session: {UserSession.GetSessionInfo()}";
        }
    }
}