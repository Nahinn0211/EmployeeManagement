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
        public event EventHandler LogoutRequested;

        private MaterialSkinManager materialSkinManager;
        private MaterialButton selectedButton = null;
        private List<MaterialButton> menuButtons = new List<MaterialButton>();
        private Dictionary<string, Panel> parentPanels = new Dictionary<string, Panel>();
        private Dictionary<string, bool> expandStates = new Dictionary<string, bool>();

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
            CreateHierarchicalMenu();
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

        private void CreateHierarchicalMenu()
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

            // Define hierarchical menu structure
            var menuStructure = new List<object>
            {
                // Dashboard (single item)
                new { Text = "Bảng điều khiển", Icon = "📊", Key = "Dashboard", IsParent = false },
                
                // HR Management (parent with children)
                new { Text = "Quản lý Nhân sự", Icon = "👥", Key = "HRManagement", IsParent = true, Children = new[]
                {
                    new { Text = "Quản lý Nhân viên", Icon = "👤", Key = "Employee" },
                    new { Text = "Quản lý Phòng ban", Icon = "🏢", Key = "Department" },
                    new { Text = "Quản lý Chức vụ", Icon = "⭐", Key = "Position" }
                }},
                
                // Project Management (parent with children)
                new { Text = "Quản lý Dự án", Icon = "📋", Key = "ProjectManagement", IsParent = true, Children = new[]
                {
                    new { Text = "Danh sách Dự án", Icon = "📑", Key = "Project" },
                    new { Text = "Quản lý Công việc", Icon = "✅", Key = "Task" },
                    new { Text = "Quản lý Khách hàng", Icon = "🤝", Key = "Customer" }
                }},
                
                // Document Management (single item)
                new { Text = "Quản lý Tài liệu", Icon = "📁", Key = "Document", IsParent = false },
                
                // Attendance & Payroll (parent with children)
                new { Text = "Chấm công & Lương", Icon = "⏰", Key = "AttendancePayroll", IsParent = true, Children = new[]
                {
                    new { Text = "Chấm công", Icon = "📆", Key = "Attendance" },
                    new { Text = "Quản lý Lương", Icon = "💰", Key = "Salary" }
                }},
                
                // Financial Management (parent with children)
                new { Text = "Quản lý Tài chính", Icon = "💵", Key = "FinancialManagement", IsParent = true, Children = new[]
                {
                    new { Text = "Thu chi Chung", Icon = "💹", Key = "Finance" },
                    new { Text = "Thu chi Dự án", Icon = "📝", Key = "ProjectFinance" }
                }},
                
                // Reports (parent with children)
                new { Text = "Báo cáo", Icon = "📊", Key = "Reports", IsParent = true, Children = new[]
                {
                    new { Text = "Báo cáo Nhân sự", Icon = "📈", Key = "HRReport" },
                    new { Text = "Báo cáo Dự án", Icon = "📊", Key = "ProjectReport" },
                    new { Text = "Báo cáo Tài chính", Icon = "📉", Key = "FinanceReport" }
                }},
                
                // Administration (parent with children)
                new { Text = "Quản trị Hệ thống", Icon = "🔧", Key = "Administration", IsParent = true, Children = new[]
                {
                    new { Text = "Quản lý Người dùng", Icon = "👤", Key = "UserManagement" },
                    new { Text = "Phân quyền", Icon = "🔒", Key = "Permission" },
                    new { Text = "Cài đặt Hệ thống", Icon = "⚙️", Key = "Settings" }
                }}
            };

            // Build menu UI from structure
            int yPosition = 10;
            foreach (dynamic menuItem in menuStructure)
            {
                bool isParent = menuItem.IsParent;

                if (isParent)
                {
                    // Create parent menu item
                    var parentPanel = CreateParentMenuPanel(
                        menuItem.Text.ToString(),
                        menuItem.Icon.ToString(),
                        menuItem.Key.ToString()
                    );
                    parentPanel.Location = new Point(0, yPosition);
                    scrollPanel.Controls.Add(parentPanel);
                    yPosition += parentPanel.Height + 5;

                    // Save reference to parent panel
                    parentPanels[menuItem.Key.ToString()] = parentPanel;
                    expandStates[menuItem.Key.ToString()] = false;

                    // Create child container panel
                    var childrenContainer = new Panel
                    {
                        Width = 260,
                        BackColor = Color.FromArgb(35, 35, 35),
                        Tag = $"children_{menuItem.Key}",
                        Visible = false,
                        AutoSize = true,
                        Padding = new Padding(0, 5, 0, 5)
                    };

                    // Add child menu items
                    int childYPos = 5;
                    foreach (dynamic childItem in menuItem.Children)
                    {
                        var childPanel = CreateChildMenuPanel(
                            childItem.Text.ToString(),
                            childItem.Icon.ToString(),
                            childItem.Key.ToString()
                        );
                        childPanel.Location = new Point(0, childYPos);
                        childrenContainer.Controls.Add(childPanel);
                        childYPos += childPanel.Height + 5;
                    }

                    // Position and add children container
                    childrenContainer.Location = new Point(0, yPosition);
                    scrollPanel.Controls.Add(childrenContainer);
                }
                else
                {
                    // Create standalone menu item
                    var menuPanel = CreateStandaloneMenuPanel(
                        menuItem.Text.ToString(),
                        menuItem.Icon.ToString(),
                        menuItem.Key.ToString()
                    );
                    menuPanel.Location = new Point(0, yPosition);
                    scrollPanel.Controls.Add(menuPanel);
                    yPosition += menuPanel.Height + 15;
                }
            }

            menuCard.Controls.Add(scrollPanel);
            this.Controls.Add(menuCard);
        }

        private Panel CreateParentMenuPanel(string text, string icon, string key)
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
                Size = new Size(145, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                UseAccent = false
            };

            var expandIcon = new Label
            {
                Text = "▼", // Down arrow (collapsed state)
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(230, 20),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = "expandIcon"
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

            clickButton.Click += ParentMenuButton_Click;
            clickButton.MouseEnter += MenuButton_MouseEnter;
            clickButton.MouseLeave += MenuButton_MouseLeave;

            menuPanel.Controls.Add(iconPanel);
            menuPanel.Controls.Add(textLabel);
            menuPanel.Controls.Add(expandIcon);
            menuPanel.Controls.Add(clickButton);

            menuButtons.Add(clickButton);
            return menuPanel;
        }

        private Panel CreateChildMenuPanel(string text, string icon, string key)
        {
            var menuPanel = new Panel
            {
                Size = new Size(260, 50),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = key
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 14),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(35, 13),
                Size = new Size(30, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var textLabel = new MaterialLabel
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(75, 15),
                Size = new Size(175, 25),
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

            clickButton.Click += ChildMenuButton_Click;
            clickButton.MouseEnter += ChildMenuButton_MouseEnter;
            clickButton.MouseLeave += ChildMenuButton_MouseLeave;

            menuPanel.Controls.Add(iconLabel);
            menuPanel.Controls.Add(textLabel);
            menuPanel.Controls.Add(clickButton);

            menuButtons.Add(clickButton);
            return menuPanel;
        }

        private Panel CreateStandaloneMenuPanel(string text, string icon, string key)
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

            clickButton.Click += StandaloneMenuButton_Click;
            clickButton.MouseEnter += MenuButton_MouseEnter;
            clickButton.MouseLeave += MenuButton_MouseLeave;

            menuPanel.Controls.Add(iconPanel);
            menuPanel.Controls.Add(textLabel);
            menuPanel.Controls.Add(clickButton);

            menuButtons.Add(clickButton);
            return menuPanel;
        }

        private void ParentMenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button == null) return;

            string parentKey = button.Tag.ToString();
            var parentPanel = button.Parent as Panel;

            // Toggle expand state
            bool isExpanded = !expandStates[parentKey];
            expandStates[parentKey] = isExpanded;

            // Update expand icon
            var expandIcon = parentPanel.Controls.OfType<Label>()
                .FirstOrDefault(l => l.Tag?.ToString() == "expandIcon");
            if (expandIcon != null)
            {
                expandIcon.Text = isExpanded ? "▲" : "▼"; // Up arrow or down arrow
            }

            // Update parent panel styling
            parentPanel.BackColor = isExpanded
                ? Color.FromArgb(50, materialSkinManager.ColorScheme.AccentColor)
                : Color.Transparent;

            var iconPanel = parentPanel.Controls.OfType<Panel>().FirstOrDefault();
            if (iconPanel != null)
            {
                iconPanel.BackColor = isExpanded
                    ? materialSkinManager.ColorScheme.AccentColor
                    : Color.FromArgb(60, 60, 60);
            }

            // Find and toggle visibility of child container
            var scrollPanel = parentPanel.Parent;
            var childrenContainer = scrollPanel.Controls.OfType<Panel>()
                .FirstOrDefault(p => p.Tag?.ToString() == $"children_{parentKey}");

            if (childrenContainer != null)
            {
                childrenContainer.Visible = isExpanded;

                // Reposition panels below this one
                int newY = parentPanel.Location.Y + parentPanel.Height + 5;
                if (isExpanded)
                {
                    // Position child container right after parent
                    childrenContainer.Location = new Point(0, newY);
                    newY += childrenContainer.Height + 5;
                }

                // Reposition all panels below this child container
                bool foundParent = false;
                foreach (Control control in scrollPanel.Controls)
                {
                    if (control == childrenContainer) continue;
                    if (control == parentPanel)
                    {
                        foundParent = true;
                        continue;
                    }

                    if (foundParent && control.Location.Y != newY)
                    {
                        control.Location = new Point(0, newY);
                        newY += control.Height +
                            (control.Tag?.ToString().StartsWith("children_") == true ? 5 : 15);
                    }
                }
            }
        }

        private void ChildMenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button == null) return;

            string menuKey = button.Tag.ToString();

            // Reset previously selected button
            if (selectedButton != null && selectedButton != button)
            {
                var prevPanel = selectedButton.Parent;
                if (!prevPanel.Tag.ToString().StartsWith("children_"))
                {
                    prevPanel.BackColor = Color.Transparent;
                    var prevIconPanel = prevPanel.Controls.OfType<Panel>().FirstOrDefault();
                    if (prevIconPanel != null)
                        prevIconPanel.BackColor = Color.FromArgb(60, 60, 60);
                }
                else
                {
                    prevPanel.BackColor = Color.Transparent;
                }
            }

            // Set new selected button
            selectedButton = button;
            var panel = button.Parent;
            panel.BackColor = Color.FromArgb(60, materialSkinManager.ColorScheme.AccentColor);

            // Raise event
            MenuItemClicked?.Invoke(this, menuKey);
        }

        private void StandaloneMenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button == null) return;

            string menuKey = button.Tag.ToString();

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

            // Raise event
            MenuItemClicked?.Invoke(this, menuKey);
        }

        private void MenuButton_MouseEnter(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton)
            {
                var panel = button.Parent;
                panel.BackColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void MenuButton_MouseLeave(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton)
            {
                var panel = button.Parent;

                // If it's a parent menu that's expanded, keep it highlighted
                if (expandStates.ContainsKey(button.Tag.ToString()) && expandStates[button.Tag.ToString()])
                {
                    panel.BackColor = Color.FromArgb(50, materialSkinManager.ColorScheme.AccentColor);
                }
                else
                {
                    panel.BackColor = Color.Transparent;
                }
            }
        }

        private void ChildMenuButton_MouseEnter(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton)
            {
                var panel = button.Parent;
                panel.BackColor = Color.FromArgb(45, 45, 45);
            }
        }

        private void ChildMenuButton_MouseLeave(object sender, EventArgs e)
        {
            var button = sender as MaterialButton;
            if (button != selectedButton)
            {
                var panel = button.Parent;
                panel.BackColor = Color.Transparent;
            }
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
            logoutButton.Click += LogoutButton_Click;

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

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetSelectedMenu(string menuKey)
        {
            var button = menuButtons.FirstOrDefault(b => b.Tag.ToString() == menuKey);
            if (button != null)
            {
                // If this is a child menu, make sure parent is expanded
                var parentPanel = button.Parent.Parent;
                string parentKey = null;

                if (parentPanel.Tag?.ToString().StartsWith("children_") == true)
                {
                    parentKey = parentPanel.Tag.ToString().Replace("children_", "");

                    // Make sure parent is expanded
                    if (expandStates.ContainsKey(parentKey) && !expandStates[parentKey])
                    {
                        // Find parent button and click it
                        var parentButton = menuButtons.FirstOrDefault(b => b.Tag.ToString() == parentKey);
                        if (parentButton != null)
                        {
                            ParentMenuButton_Click(parentButton, EventArgs.Empty);
                        }
                    }

                    // Select child button
                    ChildMenuButton_Click(button, EventArgs.Empty);
                }
                else
                {
                    // Standalone or parent button
                    StandaloneMenuButton_Click(button, EventArgs.Empty);
                }
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