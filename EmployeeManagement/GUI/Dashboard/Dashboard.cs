using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using EmployeeManagement.Utilities;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI
{
    public partial class DashboardForm : Form
    {
        #region Fields
        private readonly HRReportBLL hrReportBLL;
        private readonly EmployeeBLL employeeBLL;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.Timer dataRefreshTimer;
        private int animationStep = 0;
        private Panel statsContainer;
        private Panel chartContainer;
        private Panel quickActionsContainer;
        private DashboardMetricsDTO currentMetrics;
        #endregion

        #region Constructor
        public DashboardForm()
        {
            hrReportBLL = new HRReportBLL();
            employeeBLL = new EmployeeBLL();
            InitializeDesignerComponents();
            this.Text = "Bảng điều khiển";
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupDashboard();
            StartAnimations();
            LoadDashboardData();
        }
        #endregion

        #region Dashboard Data Loading
        /// <summary>
        /// Load dashboard data from database
        /// </summary>
        private async void LoadDashboardData()
        {
            try
            {
                currentMetrics = await System.Threading.Tasks.Task.Run(() =>
                {
                    var stats = hrReportBLL.GetHRStatistics();
                    var employees = hrReportBLL.GetEmployeeReports();
                    var departments = hrReportBLL.GetDepartmentReports();
                    var birthdays = hrReportBLL.GetBirthdayReports(DateTime.Now.Month, DateTime.Now.Year);

                    return new DashboardMetricsDTO
                    {
                        GeneralStats = new HRStatisticsDTO
                        {
                            TotalEmployees = stats.TotalEmployees,
                            ActiveEmployees = stats.ActiveEmployees,
                            InactiveEmployees = stats.InactiveEmployees,
                            NewHires = stats.NewHires,
                            Resignations = stats.Resignations,
                            TurnoverRate = stats.TurnoverRate,
                            AverageAge = stats.AverageAge,
                            AverageWorkingYears = stats.AverageWorkingYears,
                            AverageSalary = stats.AverageSalary,
                            AverageAttendanceRate = stats.AverageAttendanceRate,
                            AveragePerformanceScore = stats.AveragePerformanceScore,
                            MaleCount = stats.MaleCount,
                            FemaleCount = stats.FemaleCount
                        },
                        TopPerformers = employees.OrderByDescending(e => e.PerformanceScore).Take(5)
                            .Select(e => new EmployeeReportDTO
                            {
                                EmployeeID = e.EmployeeID,
                                EmployeeCode = e.EmployeeCode,
                                FullName = e.FullName,
                                Department = e.Department,
                                Position = e.Position,
                                PerformanceScore = e.PerformanceScore
                            }).ToList(),
                        DepartmentBreakdown = departments.Take(5).Select(d => new DepartmentReportDTO
                        {
                            DepartmentID = d.DepartmentID,
                            DepartmentName = d.DepartmentName,
                            TotalEmployees = d.TotalEmployees,
                            EmployeePercentage = d.EmployeePercentage
                        }).ToList(),
                        UpcomingBirthdays = birthdays.Take(10).Select(b => new BirthdayReportDTO
                        {
                            EmployeeID = b.EmployeeID,
                            EmployeeCode = b.EmployeeCode,
                            FullName = b.FullName,
                            Department = b.Department,
                            DateOfBirth = b.DateOfBirth,
                            DaysUntilBirthday = b.DaysUntilBirthday,
                            BirthdayStatus = GetBirthdayStatus(b.DaysUntilBirthday)
                        }).ToList()
                    };
                });

                // Update stats with real data
                UpdateStatsCards(currentMetrics);
                UpdateChartsWithRealData();
                UpdateRecentActivitiesWithRealData();

                Logger.LogInfo("Dashboard data loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading dashboard data: {ex.Message}");
                // Fallback to default data if database fails
                LoadDefaultData();
            }
        }

        private string GetBirthdayStatus(int daysUntil)
        {
            if (daysUntil == 0) return "Hôm nay";
            if (daysUntil == 1) return "Ngày mai";
            if (daysUntil <= 7) return $"Còn {daysUntil} ngày";
            if (daysUntil <= 30) return $"Còn {daysUntil} ngày";
            return "Tháng tới";
        }

        private void LoadDefaultData()
        {
            // Default data when database is not available
            currentMetrics = new DashboardMetricsDTO
            {
                GeneralStats = new HRStatisticsDTO
                {
                    TotalEmployees = 245,
                    ActiveEmployees = 238,
                    NewHires = 12,
                    TurnoverRate = 4.2m,
                    AverageAge = 32.5m,
                    AverageSalary = 15000000
                }
            };
            UpdateStatsCards(currentMetrics);
        }
        #endregion

        #region UI Setup
        private void SetupDashboard()
        {
            // Main scroll panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // Header section
            CreateHeaderSection(mainPanel);

            // Stats cards section
            CreateStatsSection(mainPanel);

            // Charts section
            CreateChartsSection(mainPanel);

            // Quick actions section
            CreateQuickActionsSection(mainPanel);

            // Recent activities section
            CreateRecentActivitiesSection(mainPanel);

            this.Controls.Add(mainPanel);
        }

        private void CreateHeaderSection(Panel parent)
        {
            var headerPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(1200, 120),
                BackColor = Color.White
            };

            // Add gradient background
            headerPanel.Paint += (s, e) =>
            {
                var rect = headerPanel.ClientRectangle;
                using (var brush = new LinearGradientBrush(rect,
                    Color.FromArgb(63, 81, 181), Color.FromArgb(48, 63, 159), 0f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Add subtle pattern
                using (var pen = new Pen(Color.FromArgb(30, 255, 255, 255), 1))
                {
                    for (int i = 0; i < rect.Width; i += 20)
                    {
                        e.Graphics.DrawLine(pen, i, 0, i + 10, rect.Height);
                    }
                }
            };

            // Welcome message
            var welcomeLabel = new Label
            {
                Text = UserSession.IsLoggedIn ?
                    $"Chào mừng trở lại, {UserSession.Username}!" :
                    "Chào mừng bạn đến với EMS!",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = $"Hôm nay là {DateTime.Now:dddd, dd MMMM yyyy} • {DateTime.Now:HH:mm}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                Location = new Point(30, 60),
                AutoSize = true
            };

            // Current time with animation
            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(1000, 30),
                Size = new Size(150, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Name = "TimeLabel"
            };

            // User avatar circle
            var avatarPanel = new Panel
            {
                Size = new Size(60, 60),
                Location = new Point(1120, 30),
                BackColor = Color.Transparent
            };
            avatarPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(255, 193, 7)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 58, 58);
                }
                using (var pen = new Pen(Color.White, 3))
                {
                    e.Graphics.DrawEllipse(pen, 0, 0, 58, 58);
                }
                // Draw user icon
                var iconFont = new Font("Segoe UI", 24, FontStyle.Bold);
                var iconBrush = new SolidBrush(Color.White);
                var userIcon = UserSession.IsLoggedIn ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
                var iconSize = e.Graphics.MeasureString(userIcon, iconFont);
                var iconX = (60 - iconSize.Width) / 2;
                var iconY = (60 - iconSize.Height) / 2;
                e.Graphics.DrawString(userIcon, iconFont, iconBrush, iconX, iconY);
            };

            headerPanel.Controls.AddRange(new Control[] {
                welcomeLabel, subtitleLabel, timeLabel, avatarPanel
            });

            parent.Controls.Add(headerPanel);
        }

        private void CreateStatsSection(Panel parent)
        {
            statsContainer = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(1200, 180),
                BackColor = Color.Transparent,
                Name = "StatsContainer"
            };

            parent.Controls.Add(statsContainer);
        }

        private void UpdateStatsCards(DashboardMetricsDTO metrics)
        {
            if (statsContainer == null) return;

            statsContainer.Controls.Clear();

            var stats = new[]
            {
                new { Title = "Tổng nhân viên", Value = metrics.GeneralStats.TotalEmployees.ToString(), Icon = "👥", Color = Color.FromArgb(33, 150, 243), Change = "+5.2%" },
                new { Title = "Đang làm việc", Value = metrics.GeneralStats.ActiveEmployees.ToString(), Icon = "✅", Color = Color.FromArgb(76, 175, 80), Change = "+2.1%" },
                new { Title = "Nhân viên mới", Value = metrics.GeneralStats.NewHires.ToString(), Icon = "👤", Color = Color.FromArgb(255, 152, 0), Change = "+12.5%" },
                new { Title = "Tỷ lệ nghỉ việc", Value = $"{metrics.GeneralStats.TurnoverRate:F1}%", Icon = "📉", Color = Color.FromArgb(244, 67, 54), Change = "-1.2%" },
                new { Title = "Tuổi TB", Value = $"{metrics.GeneralStats.AverageAge:F0}", Icon = "📊", Color = Color.FromArgb(156, 39, 176), Change = "+0.5%" },
                new { Title = "Lương TB", Value = $"{metrics.GeneralStats.AverageSalary:N0}", Icon = "💰", Color = Color.FromArgb(0, 150, 136), Change = "+8.3%" }
            };

            for (int i = 0; i < stats.Length; i++)
            {
                var card = CreateAnimatedStatsCard(stats[i].Title, stats[i].Value,
                    stats[i].Icon, stats[i].Color, stats[i].Change, i);
                card.Location = new Point((i % 3) * 400, (i / 3) * 90);
                statsContainer.Controls.Add(card);
            }
        }

        private Panel CreateAnimatedStatsCard(string title, string value, string icon, Color color, string change, int index)
        {
            var card = new Panel
            {
                Size = new Size(380, 80),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = index
            };

            // Add shadow and hover effects
            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;

                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, rect.Width, rect.Height);
                }

                // Draw card background
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Draw colored left border
                using (var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 5, rect.Height);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            // Icon
            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                Location = new Point(20, 15),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Value
            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(85, 10),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Title
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(85, 40),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Change indicator
            var changeLabel = new Label
            {
                Text = change,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = change.StartsWith("+") ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54),
                Location = new Point(290, 15),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Trend arrow
            var trendLabel = new Label
            {
                Text = change.StartsWith("+") ? "↗" : "↘",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = change.StartsWith("+") ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54),
                Location = new Point(290, 35),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Hover effects
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(248, 249, 250);
                foreach (Control ctrl in card.Controls)
                {
                    if (ctrl == valueLabel || ctrl == iconLabel)
                    {
                        ctrl.ForeColor = Color.FromArgb(Math.Max(0, color.R - 30),
                                                      Math.Max(0, color.G - 30),
                                                      Math.Max(0, color.B - 30));
                    }
                }
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
                valueLabel.ForeColor = color;
                iconLabel.ForeColor = color;
            };

            card.Controls.AddRange(new Control[] { iconLabel, valueLabel, titleLabel, changeLabel, trendLabel });
            return card;
        }

        private void CreateChartsSection(Panel parent)
        {
            chartContainer = new Panel
            {
                Location = new Point(20, 360),
                Size = new Size(1200, 300),
                BackColor = Color.Transparent,
                Name = "ChartContainer"
            };

            parent.Controls.Add(chartContainer);
        }

        private void UpdateChartsWithRealData()
        {
            if (chartContainer == null) return;

            chartContainer.Controls.Clear();

            // Left chart - Top performers from real data
            var topPerformersPanel = CreateTopPerformersPanel();
            topPerformersPanel.Location = new Point(0, 0);
            topPerformersPanel.Size = new Size(590, 300);

            // Right chart - Department breakdown from real data
            var departmentPanel = CreateDepartmentBreakdownPanel();
            departmentPanel.Location = new Point(610, 0);
            departmentPanel.Size = new Size(590, 300);

            chartContainer.Controls.AddRange(new Control[] { topPerformersPanel, departmentPanel });
        }

        private Panel CreateTopPerformersPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                var rect = panel.ClientRectangle;
                // Draw shadow and border (same as before)
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, rect.Width, rect.Height);
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "🏆 TOP NHÂN VIÊN XUẤT SẮC",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 15),
                Size = new Size(550, 25)
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 50),
                Size = new Size(550, 220),
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("STT", 40);
            listView.Columns.Add("Tên nhân viên", 250);
            listView.Columns.Add("Phòng ban", 150);
            listView.Columns.Add("Điểm", 70);

            if (currentMetrics?.TopPerformers != null)
            {
                for (int i = 0; i < currentMetrics.TopPerformers.Count; i++)
                {
                    var emp = currentMetrics.TopPerformers[i];
                    var item = new ListViewItem((i + 1).ToString());
                    item.SubItems.Add(emp.FullName);
                    item.SubItems.Add(emp.Department);
                    item.SubItems.Add(emp.PerformanceScore.ToString("F1"));

                    // Color coding for top performers
                    if (i == 0) item.BackColor = Color.Gold;
                    else if (i == 1) item.BackColor = Color.LightGray;
                    else if (i == 2) item.BackColor = Color.FromArgb(205, 127, 50);

                    listView.Items.Add(item);
                }
            }

            panel.Controls.AddRange(new Control[] { titleLabel, listView });
            return panel;
        }

        private Panel CreateDepartmentBreakdownPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                var rect = panel.ClientRectangle;
                // Draw shadow and border
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, rect.Width, rect.Height);
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "📊 PHÂN BỐ THEO PHÒNG BAN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 15),
                Size = new Size(550, 25)
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 50),
                Size = new Size(550, 220),
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("Phòng ban", 280);
            listView.Columns.Add("Số NV", 100);
            listView.Columns.Add("Tỷ lệ", 100);

            if (currentMetrics?.DepartmentBreakdown != null)
            {
                foreach (var dept in currentMetrics.DepartmentBreakdown)
                {
                    var item = new ListViewItem(dept.DepartmentName);
                    item.SubItems.Add(dept.TotalEmployees.ToString());
                    item.SubItems.Add($"{dept.EmployeePercentage:F1}%");
                    listView.Items.Add(item);
                }
            }

            panel.Controls.AddRange(new Control[] { titleLabel, listView });
            return panel;
        }

        private void CreateQuickActionsSection(Panel parent)
        {
            quickActionsContainer = new Panel
            {
                Location = new Point(20, 680),
                Size = new Size(1200, 120),
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = "⚡ Thao tác nhanh",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(0, 0),
                Size = new Size(200, 30)
            };

            var actions = new[]
            {
                new { Text = "Thêm nhân viên", Icon = "👤", Color = Color.FromArgb(33, 150, 243) },
                new { Text = "Xem báo cáo", Icon = "📊", Color = Color.FromArgb(255, 152, 0) },
                new { Text = "Quản lý lương", Icon = "💰", Color = Color.FromArgb(156, 39, 176) },
                new { Text = "Chấm công", Icon = "⏰", Color = Color.FromArgb(244, 67, 54) },
                new { Text = "Sinh nhật", Icon = "🎂", Color = Color.FromArgb(76, 175, 80) },
                new { Text = "Cài đặt", Icon = "⚙️", Color = Color.FromArgb(0, 150, 136) }
            };

            for (int i = 0; i < actions.Length; i++)
            {
                var button = CreateQuickActionButton(actions[i].Text, actions[i].Icon, actions[i].Color);
                button.Location = new Point(i * 200, 40);
                quickActionsContainer.Controls.Add(button);
            }

            quickActionsContainer.Controls.Add(titleLabel);
            parent.Controls.Add(quickActionsContainer);
        }

        private Button CreateQuickActionButton(string text, string icon, Color color)
        {
            var button = new Button
            {
                Size = new Size(180, 70),
                Text = $"{icon}\n{text}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.FlatAppearance.BorderSize = 0;

            // Hover effects
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(Math.Max(0, color.R - 30),
                                                Math.Max(0, color.G - 30),
                                                Math.Max(0, color.B - 30));
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = color;
            };

            return button;
        }

        private void CreateRecentActivitiesSection(Panel parent)
        {
            var activitiesPanel = new Panel
            {
                Location = new Point(20, 820),
                Size = new Size(1200, 250),
                BackColor = Color.White,
                Name = "RecentActivitiesPanel"
            };

            activitiesPanel.Paint += (s, e) =>
            {
                var rect = activitiesPanel.ClientRectangle;

                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, rect.Width, rect.Height);
                }

                // Draw background
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "📋 Hoạt động gần đây",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 15),
                Size = new Size(300, 30)
            };

            activitiesPanel.Controls.Add(titleLabel);
            parent.Controls.Add(activitiesPanel);

            // Load recent activities from data
            UpdateRecentActivitiesWithRealData();
        }

        private void UpdateRecentActivitiesWithRealData()
        {
            var activitiesPanel = this.Controls.Find("RecentActivitiesPanel", true).FirstOrDefault();
            if (activitiesPanel == null) return;

            // Clear existing activity labels (keep title)
            var toRemove = activitiesPanel.Controls.OfType<Label>().Where(l => l.Text.Contains("🔵") || l.Text.Contains("🟢") || l.Text.Contains("🟡") || l.Text.Contains("🔴") || l.Text.Contains("🟣")).ToList();
            foreach (var label in toRemove)
            {
                activitiesPanel.Controls.Remove(label);
            }

            var activities = new List<string>();

            // Add birthday activities if available
            if (currentMetrics?.UpcomingBirthdays != null)
            {
                var todayBirthdays = currentMetrics.UpcomingBirthdays.Where(b => b.DaysUntilBirthday == 0).ToList();
                var upcomingBirthdays = currentMetrics.UpcomingBirthdays.Where(b => b.DaysUntilBirthday > 0 && b.DaysUntilBirthday <= 3).ToList();

                foreach (var birthday in todayBirthdays)
                {
                    activities.Add($"🎂 {DateTime.Now:HH:mm} - Hôm nay là sinh nhật của {birthday.FullName} ({birthday.Department})");
                }

                foreach (var birthday in upcomingBirthdays)
                {
                    activities.Add($"🎉 Sắp tới - {birthday.FullName} sẽ có sinh nhật trong {birthday.DaysUntilBirthday} ngày");
                }
            }

            // Add general activities with real data context
            if (currentMetrics?.GeneralStats != null)
            {
                activities.Add($"🔵 {DateTime.Now.AddMinutes(-5):HH:mm} - Có {currentMetrics.GeneralStats.ActiveEmployees} nhân viên đang có mặt");
                activities.Add($"🟢 {DateTime.Now.AddMinutes(-15):HH:mm} - Tỷ lệ chấm công hôm nay: {currentMetrics.GeneralStats.AverageAttendanceRate:F1}%");
                activities.Add($"🟡 {DateTime.Now.AddMinutes(-30):HH:mm} - Đã có {currentMetrics.GeneralStats.NewHires} nhân viên mới trong tháng");

                if (currentMetrics.TopPerformers?.Any() == true)
                {
                    var topPerformer = currentMetrics.TopPerformers.First();
                    activities.Add($"🏆 {DateTime.Now.AddHours(-1):HH:mm} - {topPerformer.FullName} dẫn đầu với điểm số {topPerformer.PerformanceScore:F1}");
                }

                activities.Add($"🟣 {DateTime.Now.AddHours(-2):HH:mm} - Tổng cộng {currentMetrics.GeneralStats.TotalEmployees} nhân viên trong hệ thống");
            }

            // Default activities if no real data
            if (!activities.Any())
            {
                activities.AddRange(new[]
                {
                    $"🔵 {DateTime.Now.AddMinutes(-5):HH:mm} - Hệ thống đang hoạt động bình thường",
                    $"🟢 {DateTime.Now.AddMinutes(-15):HH:mm} - Đang tải dữ liệu từ cơ sở dữ liệu",
                    $"🟡 {DateTime.Now.AddMinutes(-30):HH:mm} - Dashboard đã được khởi tạo",
                    $"🔴 {DateTime.Now.AddHours(-1):HH:mm} - Chào mừng bạn đến với EMS",
                    $"🟣 {DateTime.Now.AddHours(-2):HH:mm} - Sẵn sàng phục vụ"
                });
            }

            // Display activities (limit to 5 most recent)
            for (int i = 0; i < Math.Min(activities.Count, 5); i++)
            {
                var activityLabel = new Label
                {
                    Text = activities[i],
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(80, 80, 80),
                    Location = new Point(30, 55 + i * 35),
                    Size = new Size(1140, 25),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                activitiesPanel.Controls.Add(activityLabel);
            }
        }
        #endregion

        #region Animations and Timers
        private void StartAnimations()
        {
            // Animation timer for visual effects
            animationTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            animationTimer.Tick += (s, e) =>
            {
                // Update time display
                var timeLabel = this.Controls.Find("TimeLabel", true).FirstOrDefault() as Label;
                if (timeLabel != null)
                {
                    timeLabel.Text = DateTime.Now.ToString("HH:mm:ss");
                }

                // Add subtle animation to stats cards
                animationStep++;
                if (statsContainer != null)
                {
                    foreach (Control card in statsContainer.Controls)
                    {
                        if (card is Panel panel && panel.Tag is int index)
                        {
                            var offset = Math.Sin((animationStep + index) * 0.1) * 2;
                            panel.Location = new Point(panel.Location.X,
                                (index / 3) * 90 + (int)offset);
                        }
                    }
                }
            };
            animationTimer.Start();

            // Data refresh timer - refresh every 5 minutes
            dataRefreshTimer = new System.Windows.Forms.Timer { Interval = 300000 }; // 5 minutes
            dataRefreshTimer.Tick += (s, e) =>
            {
                LoadDashboardData();
            };
            dataRefreshTimer.Start();
        }
        #endregion

        #region Form Events
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Logger.LogInfo("DashboardForm opened");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            dataRefreshTimer?.Stop();
            dataRefreshTimer?.Dispose();
            Logger.LogInfo("DashboardForm closed");
            base.OnFormClosed(e);
        }

        private void RefreshDashboard()
        {
            try
            {
                LoadDashboardData();
                Logger.LogInfo("Dashboard refreshed manually");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error refreshing dashboard: {ex.Message}");
                MessageBox.Show($"Lỗi khi làm mới dashboard: {ex.Message}", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Designer Components
        private void InitializeDesignerComponents()
        {
            this.SuspendLayout();

            // Form properties
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.ClientSize = new Size(1400, 800);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.MinimumSize = new Size(1200, 600);
            this.Name = "DashboardForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Bảng điều khiển";
            this.WindowState = FormWindowState.Maximized;

            this.ResumeLayout(false);
        }
        #endregion
    }
}