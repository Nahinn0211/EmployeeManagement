using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;

namespace EmployeeManagement.GUI.Menu
{
    public partial class MainMenu : UserControl
    {
        public event EventHandler<string> MenuItemClicked;
        public event EventHandler LogoutRequested; // Thêm sự kiện cho logout

        private MaterialSkinManager materialSkinManager;
        private MaterialButton selectedButton = null;
        private List<MaterialButton> menuButtons = new List<MaterialButton>();

        public MainMenu()
        {
            InitializeComponent();
            InitializeMaterial();
            SetupMenu();
        }

        private void InitializeMaterial()
        {
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600,
                Primary.Blue700,
                Primary.Blue200,
                Accent.LightBlue200,
                TextShade.WHITE
            );
        }

        private void SetupMenu()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Dock = DockStyle.Left;
            this.Width = 320;

            CreateHeader();
            CreateMenuItems();
            CreateFooter();
        }

        private void CreateHeader()
        {
            var headerCard = new MaterialCard
            {
                Height = 120,
                Dock = DockStyle.Top,
                BackColor = materialSkinManager.ColorScheme.PrimaryColor,
                Margin = new Padding(15, 15, 15, 10),
                Padding = new Padding(0)
            };

            var logoPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Fill
            };

            var logoLabel = new MaterialLabel
            {
                Text = "EMS",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                UseAccent = false
            };

            logoPanel.Controls.Add(logoLabel);

            var subtitlePanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(40, materialSkinManager.ColorScheme.DarkPrimaryColor)
            };

            var subtitleLabel = new MaterialLabel
            {
                Text = "Hệ thống quản lý nhân viên",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                UseAccent = false
            };

            subtitlePanel.Controls.Add(subtitleLabel);

            headerCard.Controls.Add(logoPanel);
            headerCard.Controls.Add(subtitlePanel);
            this.Controls.Add(headerCard);
        }

        private void CreateMenuItems()
        {
            var menuCard = new MaterialCard
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(15, 5, 15, 10),
                Padding = new Padding(20, 15, 20, 15),
                BackColor = Color.FromArgb(40, 40, 40)
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            var menuItems = new[]
            {
                new { Text = "Bảng điều khiển", Icon = "📊", Key = "Dashboard" },
                new { Text = "Quản lý Nhân viên", Icon = "👥", Key = "Employee" },
                new { Text = "Quản lý Phòng ban", Icon = "🏢", Key = "Department" },
                new { Text = "Quản lý Dự án", Icon = "📋", Key = "Projects" }, // THÊM DỰ ÁN
                new { Text = "Chấm công", Icon = "⏰", Key = "Attendance" },
                new { Text = "Tính lương", Icon = "💰", Key = "Payroll" },
                new { Text = "Báo cáo", Icon = "📈", Key = "Reports" },
                new { Text = "Cài đặt", Icon = "⚙️", Key = "Settings" }
            };

            int yPosition = 10;
            foreach (var item in menuItems)
            {
                var menuItemPanel = CreateMenuItemPanel(item.Text, item.Icon, item.Key);
                menuItemPanel.Location = new Point(0, yPosition);
                scrollPanel.Controls.Add(menuItemPanel);
                yPosition += menuItemPanel.Height + 15;
            }

            menuCard.Controls.Add(scrollPanel);
            this.Controls.Add(menuCard);
        }

        private Panel CreateMenuItemPanel(string text, string icon, string key)
        {
            var menuPanel = new Panel
            {
                Size = new Size(260, 60),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = key
            };

            var iconPanel = new Panel
            {
                Size = new Size(50, 50),
                Location = new Point(10, 5),
                BackColor = Color.FromArgb(60, 60, 60)
            };

            GraphicsPath iconPath = new GraphicsPath();
            iconPath.AddEllipse(0, 0, iconPanel.Width, iconPanel.Height);
            iconPanel.Region = new Region(iconPath);

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 18),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            iconPanel.Controls.Add(iconLabel);

            var textLabel = new MaterialLabel
            {
                Text = text,
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(75, 15),
                Size = new Size(175, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                UseAccent = false
            };

            var clickButton = new MaterialButton
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = false,
                Tag = key,
                FlatStyle = FlatStyle.Flat
            };

            clickButton.FlatAppearance.BorderSize = 0;
            clickButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            clickButton.FlatAppearance.MouseDownBackColor = Color.Transparent;

            clickButton.Click += MenuButton_Click;
            clickButton.MouseEnter += MenuButton_MouseEnter;
            clickButton.MouseLeave += MenuButton_MouseLeave;

            menuPanel.Controls.Add(iconPanel);
            menuPanel.Controls.Add(textLabel);
            menuPanel.Controls.Add(clickButton);

            menuButtons.Add(clickButton);
            return menuPanel;
        }

        private void CreateFooter()
        {
            var footerCard = new MaterialCard
            {
                Height = 150,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(45, 45, 45),
                Margin = new Padding(15, 10, 15, 15),
                Padding = new Padding(20)
            };

            var avatarPanel = new Panel
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = materialSkinManager.ColorScheme.AccentColor
            };

            GraphicsPath avatarPath = new GraphicsPath();
            avatarPath.AddEllipse(0, 0, avatarPanel.Width, avatarPanel.Height);
            avatarPanel.Region = new Region(avatarPath);

            var avatarLabel = new MaterialLabel
            {
                Text = "A",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                UseAccent = false
            };
            avatarPanel.Controls.Add(avatarLabel);

            var userLabel = new MaterialLabel
            {
                Text = "Quản trị viên",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(90, 20),
                AutoSize = true,
                UseAccent = false
            };

            var statusLabel = new MaterialLabel
            {
                Text = "● Đang trực tuyến",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.LightGreen,
                Location = new Point(90, 45),
                AutoSize = true,
                UseAccent = false
            };

            var dividerPanel = new Panel
            {
                Height = 1,
                Width = 250,
                Location = new Point(20, 90),
                BackColor = Color.FromArgb(80, 80, 80)
            };

            var logoutPanel = new Panel
            {
                Size = new Size(250, 40),
                Location = new Point(20, 100),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "Logout"
            };

            var logoutIconLabel = new Label
            {
                Text = "🚪",
                Font = new Font("Segoe UI Emoji", 14),
                ForeColor = Color.FromArgb(255, 87, 34),
                Location = new Point(10, 8),
                Size = new Size(30, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var logoutTextLabel = new MaterialLabel
            {
                Text = "Đăng xuất",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(255, 87, 34),
                Location = new Point(50, 10),
                AutoSize = true,
                UseAccent = false
            };

            var logoutButton = new MaterialButton
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = false,
                Tag = "Logout",
                FlatStyle = FlatStyle.Flat
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 255, 87, 34);
            logoutButton.Click += MenuButton_Click;

            logoutPanel.Controls.Add(logoutIconLabel);
            logoutPanel.Controls.Add(logoutTextLabel);
            logoutPanel.Controls.Add(logoutButton);

            footerCard.Controls.AddRange(new Control[]
            {
                avatarPanel,
                userLabel,
                statusLabel,
                dividerPanel,
                logoutPanel
            });

            this.Controls.Add(footerCard);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button == null) return;

            string menuKey = button.Tag.ToString();

            // Xử lý logout riêng
            if (menuKey == "Logout")
            {
                // Hiển thị hộp thoại xác nhận
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất?",
                    "Xác nhận đăng xuất",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Raise sự kiện LogoutRequested
                    LogoutRequested?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            // Reset previous selected button
            if (selectedButton != null && selectedButton != button)
            {
                var prevPanel = selectedButton.Parent;
                prevPanel.BackColor = Color.Transparent;

                var prevIconPanel = prevPanel.Controls.OfType<Panel>().FirstOrDefault();
                if (prevIconPanel != null)
                    prevIconPanel.BackColor = Color.FromArgb(60, 60, 60);
            }

            // Set new selected button
            selectedButton = button;
            var panel = button.Parent;
            panel.BackColor = Color.FromArgb(40, materialSkinManager.ColorScheme.AccentColor);

            var iconPanel = panel.Controls.OfType<Panel>().FirstOrDefault();
            if (iconPanel != null)
                iconPanel.BackColor = materialSkinManager.ColorScheme.AccentColor;

            // Raise event cho menu items khác
            MenuItemClicked?.Invoke(this, menuKey);
        }

        private void MenuButton_MouseEnter(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton && button.Tag.ToString() != "Logout")
            {
                var panel = button.Parent;
                panel.BackColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void MenuButton_MouseLeave(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton && button.Tag.ToString() != "Logout")
            {
                var panel = button.Parent;
                panel.BackColor = Color.Transparent;
            }
        }

        public void SetSelectedMenu(string menuKey)
        {
            var button = menuButtons.FirstOrDefault(b => b.Tag.ToString() == menuKey);
            if (button != null)
            {
                MenuButton_Click(button, EventArgs.Empty);
            }
        }

        public void UpdateUserInfo(string name, bool isOnline)
        {
            var footerCard = this.Controls.OfType<MaterialCard>().LastOrDefault();
            if (footerCard != null)
            {
                var userLabel = footerCard.Controls.OfType<MaterialLabel>().FirstOrDefault();
                var statusLabel = footerCard.Controls.OfType<MaterialLabel>().Skip(1).FirstOrDefault();

                if (userLabel != null) userLabel.Text = name;
                if (statusLabel != null)
                {
                    statusLabel.Text = isOnline ? "● Đang trực tuyến" : "● Ngoại tuyến";
                    statusLabel.ForeColor = isOnline ? Color.LightGreen : Color.Gray;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(brush, new Rectangle(this.Width - 5, 0, 5, this.Height));
            }
        }
    }
}