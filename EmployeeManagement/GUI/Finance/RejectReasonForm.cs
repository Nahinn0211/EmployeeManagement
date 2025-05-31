using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagement.GUI.Finance
{
    public partial class RejectReasonForm : Form
    {
        #region Fields
        private Models.Entity.Finance finance;
        private TableLayoutPanel mainLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        private Label titleLabel;
        private Label financeInfoLabel;
        private Label reasonLabel;
        private TextBox reasonTextBox;
        private Button confirmButton;
        private Button cancelButton;

        public string RejectReason { get; private set; }
        #endregion

        #region Constructor
        public RejectReasonForm(Models.Entity.Finance finance)
        {
            this.finance = finance ?? throw new ArgumentNullException(nameof(finance));
            InitializeComponent();
            SetupForm();
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Từ chối giao dịch tài chính";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            SetupLayout();
            SetupHeader();
            SetupContent();
            SetupFooter();
        }

        private void SetupLayout()
        {
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Footer

            this.Controls.Add(mainLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 243, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            titleLabel = new Label
            {
                Text = "❌ TỪ CHỐI GIAO DỊCH TÀI CHÍNH",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };

            financeInfoLabel = new Label
            {
                Text = $"Mã GD: {finance.TransactionCode} | Số tiền: {finance.AmountDisplay} | {finance.TransactionTypeDisplay}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(financeInfoLabel);
            headerPanel.Controls.Add(titleLabel);

            mainLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            reasonLabel = new Label
            {
                Text = "Lý do từ chối *:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.BottomLeft
            };

            reasonTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 0),
                MaxLength = 500
            };

            // Add placeholder behavior
            SetupPlaceholderBehavior();

            contentPanel.Controls.Add(reasonTextBox);
            contentPanel.Controls.Add(reasonLabel);

            mainLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupPlaceholderBehavior()
        {
            string placeholder = "Nhập lý do từ chối giao dịch này (tối đa 500 ký tự)...";
            reasonTextBox.Text = placeholder;
            reasonTextBox.ForeColor = Color.Gray;

            reasonTextBox.GotFocus += (s, e) =>
            {
                if (reasonTextBox.Text == placeholder)
                {
                    reasonTextBox.Text = "";
                    reasonTextBox.ForeColor = Color.Black;
                }
            };

            reasonTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(reasonTextBox.Text))
                {
                    reasonTextBox.Text = placeholder;
                    reasonTextBox.ForeColor = Color.Gray;
                }
            };

            // Add character counter
            reasonTextBox.TextChanged += (s, e) =>
            {
                UpdateCharacterCounter();
            };
        }

        private void UpdateCharacterCounter()
        {
            try
            {
                if (reasonTextBox != null && reasonLabel != null)
                {
                    string placeholder = "Nhập lý do từ chối giao dịch này (tối đa 500 ký tự)...";
                    string currentText = reasonTextBox.Text == placeholder ? "" : reasonTextBox.Text;
                    int currentLength = currentText.Length;
                    int maxLength = reasonTextBox.MaxLength;

                    reasonLabel.Text = $"Lý do từ chối * ({currentLength}/{maxLength} ký tự):";

                    // Change color based on length
                    if (currentLength < 10)
                        reasonLabel.ForeColor = Color.FromArgb(220, 38, 38); // Red
                    else if (currentLength > maxLength - 50)
                        reasonLabel.ForeColor = Color.FromArgb(255, 152, 0); // Orange
                    else
                        reasonLabel.ForeColor = Color.FromArgb(64, 64, 64); // Normal
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Character counter error: {ex.Message}");
            }
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 10, 15, 10)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            confirmButton = new Button
            {
                Text = "❌ Xác nhận từ chối",
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };

            cancelButton = new Button
            {
                Text = "🔄 Hủy bỏ",
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };

            SetupButtonEvents();

            buttonPanel.Controls.Add(confirmButton);
            buttonPanel.Controls.Add(cancelButton);

            // Add shortcut info label
            var shortcutLabel = new Label
            {
                Text = "💡 Ctrl+Enter: Xác nhận | Escape: Hủy | F1: Trợ giúp",
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Width = 300
            };

            footerPanel.Controls.Add(buttonPanel);
            footerPanel.Controls.Add(shortcutLabel);

            mainLayout.Controls.Add(footerPanel, 0, 2);
        }

        private void SetupButtonEvents()
        {
            confirmButton.Click += ConfirmButton_Click;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add hover effects
            confirmButton.MouseEnter += (s, e) =>
                confirmButton.BackColor = Color.FromArgb(200, 30, 30);
            confirmButton.MouseLeave += (s, e) =>
                confirmButton.BackColor = Color.FromArgb(220, 38, 38);

            cancelButton.MouseEnter += (s, e) =>
                cancelButton.BackColor = Color.FromArgb(140, 140, 140);
            cancelButton.MouseLeave += (s, e) =>
                cancelButton.BackColor = Color.FromArgb(158, 158, 158);
        }
        #endregion

        #region Event Handlers
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (ValidateReason())
            {
                RejectReason = reasonTextBox.Text.Trim();
                this.DialogResult = DialogResult.OK;
            }
        }

        private bool ValidateReason()
        {
            string placeholder = "Nhập lý do từ chối giao dịch này (tối đa 500 ký tự)...";

            if (string.IsNullOrWhiteSpace(reasonTextBox.Text) || reasonTextBox.Text == placeholder)
            {
                MessageBox.Show("Vui lòng nhập lý do từ chối!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reasonTextBox.Focus();
                return false;
            }

            if (reasonTextBox.Text.Trim().Length < 10)
            {
                MessageBox.Show("Lý do từ chối phải có ít nhất 10 ký tự!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reasonTextBox.Focus();
                return false;
            }

            if (reasonTextBox.Text.Trim().Length > 500)
            {
                MessageBox.Show("Lý do từ chối không được vượt quá 500 ký tự!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reasonTextBox.Focus();
                return false;
            }

            return true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                return true;
            }

            if (keyData == (Keys.Control | Keys.Enter))
            {
                ConfirmButton_Click(null, null);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the finance transaction being rejected
        /// </summary>
        public Models.Entity.Finance GetFinance()
        {
            return finance;
        }

        /// <summary>
        /// Set focus to reason textbox
        /// </summary>
        public void FocusOnReason()
        {
            if (reasonTextBox != null && !reasonTextBox.IsDisposed)
            {
                reasonTextBox.Focus();
                reasonTextBox.SelectAll();
            }
        }
        #endregion
    }
}