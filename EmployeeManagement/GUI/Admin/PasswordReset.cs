using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.GUI.Admin
{
    public partial class PasswordResetForm : Form
    {
        #region Fields
        private UserBLL userBLL;
        private User user;

        // Controls
        private Label titleLabel;
        private Label userInfoLabel;
        private TextBox newPasswordTextBox;
        private TextBox confirmPasswordTextBox;
        private CheckBox generateRandomCheckBox;
        private CheckBox showPasswordCheckBox;
        private Button generateButton;
        private Button saveButton;
        private Button cancelButton;
        private ErrorProvider errorProvider;
        #endregion

        #region Constructor
        public PasswordResetForm(User userToReset)
        {
            InitializeComponent();
            userBLL = new UserBLL();
            user = userToReset ?? throw new ArgumentNullException(nameof(userToReset));
            SetupForm();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Reset mật khẩu";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

            SetupControls();
        }

        private void SetupControls()
        {
            // Title
            titleLabel = new Label
            {
                Text = "🔑 RESET MẬT KHẨU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20),
                Size = new Size(400, 30)
            };

            // User info
            userInfoLabel = new Label
            {
                Text = $"Đang reset mật khẩu cho: {user.Username} ({user.FullName ?? "Chưa có tên"})",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 60),
                Size = new Size(400, 25)
            };

            // Generate random checkbox
            generateRandomCheckBox = new CheckBox
            {
                Text = "Tạo mật khẩu ngẫu nhiên",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 100),
                Size = new Size(200, 25),
                Checked = true
            };
            generateRandomCheckBox.CheckedChanged += GenerateRandomCheckBox_CheckedChanged;

            // Generate button
            generateButton = new Button
            {
                Text = "🎲 Tạo mật khẩu",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(250, 95),
                Size = new Size(150, 35),
                FlatAppearance = { BorderSize = 0 }
            };
            generateButton.Click += GenerateButton_Click;

            // New password
            var newPasswordLabel = new Label
            {
                Text = "Mật khẩu mới:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 150),
                Size = new Size(100, 25)
            };

            newPasswordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 175),
                Size = new Size(400, 30),
                UseSystemPasswordChar = true,
                ReadOnly = true // Initially read-only when random generation is selected
            };
            newPasswordTextBox.Leave += NewPasswordTextBox_Leave;

            // Confirm password
            var confirmPasswordLabel = new Label
            {
                Text = "Xác nhận mật khẩu:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 215),
                Size = new Size(150, 25)
            };

            confirmPasswordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 240),
                Size = new Size(400, 30),
                UseSystemPasswordChar = true,
                ReadOnly = true // Initially read-only when random generation is selected
            };
            confirmPasswordTextBox.Leave += ConfirmPasswordTextBox_Leave;

            // Show password checkbox
            showPasswordCheckBox = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 280),
                Size = new Size(150, 25)
            };
            showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;

            // Save button
            saveButton = new Button
            {
                Text = "💾 Reset mật khẩu",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(200, 320),
                Size = new Size(150, 40),
                FlatAppearance = { BorderSize = 0 }
            };
            saveButton.Click += SaveButton_Click;

            // Cancel button
            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(360, 320),
                Size = new Size(100, 40),
                FlatAppearance = { BorderSize = 0 }
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                titleLabel, userInfoLabel, generateRandomCheckBox, generateButton,
                newPasswordLabel, newPasswordTextBox,
                confirmPasswordLabel, confirmPasswordTextBox,
                showPasswordCheckBox, saveButton, cancelButton
            });

            // Generate initial password
            GenerateRandomPassword();
        }
        #endregion

        #region Event Handlers
        private void GenerateRandomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool isRandom = generateRandomCheckBox.Checked;
            generateButton.Enabled = isRandom;
            newPasswordTextBox.ReadOnly = isRandom;
            confirmPasswordTextBox.ReadOnly = isRandom;

            if (isRandom)
            {
                GenerateRandomPassword();
            }
            else
            {
                newPasswordTextBox.Clear();
                confirmPasswordTextBox.Clear();
                newPasswordTextBox.Focus();
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            GenerateRandomPassword();
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newPasswordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
            confirmPasswordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        }

        private void NewPasswordTextBox_Leave(object sender, EventArgs e)
        {
            ValidateNewPassword();
        }

        private void ConfirmPasswordTextBox_Leave(object sender, EventArgs e)
        {
            ValidateConfirmPassword();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    string newPassword = newPasswordTextBox.Text;

                    if (userBLL.ResetPassword(user.UserID, newPassword))
                    {
                        string message = "Reset mật khẩu thành công!";
                        if (generateRandomCheckBox.Checked)
                        {
                            message += $"\n\nMật khẩu mới: {newPassword}\nVui lòng lưu lại mật khẩu này!";
                        }

                        MessageBox.Show(message, "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Không thể reset mật khẩu. Vui lòng thử lại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi reset mật khẩu: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Helper Methods
        private void GenerateRandomPassword()
        {
            string randomPassword = userBLL.GenerateRandomPassword(8);
            newPasswordTextBox.Text = randomPassword;
            confirmPasswordTextBox.Text = randomPassword;
        }

        private bool ValidateNewPassword()
        {
            errorProvider.SetError(newPasswordTextBox, "");

            if (string.IsNullOrWhiteSpace(newPasswordTextBox.Text))
            {
                errorProvider.SetError(newPasswordTextBox, "Mật khẩu mới không được để trống");
                return false;
            }

            if (newPasswordTextBox.Text.Length < 6)
            {
                errorProvider.SetError(newPasswordTextBox, "Mật khẩu phải có ít nhất 6 ký tự");
                return false;
            }

            if (newPasswordTextBox.Text.Length > 50)
            {
                errorProvider.SetError(newPasswordTextBox, "Mật khẩu không được vượt quá 50 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateConfirmPassword()
        {
            errorProvider.SetError(confirmPasswordTextBox, "");

            if (newPasswordTextBox.Text != confirmPasswordTextBox.Text)
            {
                errorProvider.SetError(confirmPasswordTextBox, "Xác nhận mật khẩu không khớp");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            isValid &= ValidateNewPassword();
            isValid &= ValidateConfirmPassword();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }
        #endregion
    }
}