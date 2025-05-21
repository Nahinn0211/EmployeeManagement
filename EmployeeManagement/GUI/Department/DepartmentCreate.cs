using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace EmployeeManagement.GUI.Department
{
    public partial class DepartmentCreate : Form
    {
        #region Fields
        private Models.Department department;
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox departmentIcon;

        // Content controls
        private Label departmentNameLabel;
        private TextBox departmentNameTextBox;
        private Label departmentDescriptionLabel;
        private TextBox departmentDescriptionTextBox;
        private Label managerLabel;
        private ComboBox managerComboBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        private List<Models.Employee> managers;

        // Validation
        private ErrorProvider errorProvider;
        #endregion

        #region Constructor
        public DepartmentCreate()
        {
            InitializeComponent();
            this.department = new Models.Department();
            LoadDataFromDatabase();
            SetupForm();
            SetDefaultValues();
        }
        public Models.Department CreatedDepartment => department;
        #endregion

        #region Database Methods
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                LoadManagers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadManagers()
        {
            managers = new List<Models.Employee>();

            // Thêm tùy chọn "Không có quản lý"
            managers.Add(new Models.Employee { EmployeeID = 0, FullName = "Không có" });

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                string query = @"
                    SELECT EmployeeID, FullName 
                    FROM Employees 
                    WHERE Status = N'Đang làm việc' 
                    ORDER BY FullName";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        managers.Add(new Models.Employee
                        {
                            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                            FullName = reader["FullName"].ToString()
                        });
                    }
                }
            }
        }

        private string GenerateDepartmentCode()
        {
            // Lấy mã phòng ban mới dựa trên quy tắc (ví dụ: PB + số thứ tự)
            string prefix = "PB";
            int nextNumber = 1;

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT COUNT(*) + 1 AS NextNumber
                        FROM Departments";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int lastNumber))
                    {
                        nextNumber = lastNumber;
                    }
                }
            }
            catch
            {
                // Nếu có lỗi, sử dụng timestamp
                return prefix + DateTime.Now.ToString("yyyyMMddHHmm");
            }

            return prefix + nextNumber.ToString("D2");
        }

        private void InsertDepartmentToDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Departments (
                            DepartmentName, Description, ManagerID, CreatedAt, UpdatedAt
                        ) VALUES (
                            @DepartmentName, @Description, @ManagerID, @CreatedAt, @UpdatedAt
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Thêm các tham số
                    command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                    command.Parameters.AddWithValue("@Description",
                        string.IsNullOrEmpty(department.Description) ? DBNull.Value : (object)department.Description);

                    if (department.ManagerID.HasValue && department.ManagerID.Value > 0)
                        command.Parameters.AddWithValue("@ManagerID", department.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    connection.Open();
                    int newDepartmentId = Convert.ToInt32(command.ExecuteScalar());
                    department.DepartmentID = newDepartmentId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm phòng ban vào cơ sở dữ liệu: {ex.Message}", ex);
            }
        }
        #endregion

        #region Form Setup
        private void SetupForm()
        {
            this.Text = "Tạo phòng ban mới";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

            SetupLayout();
            SetupHeader();
            SetupContent();
            SetupFooter();
        }

        private void SetupLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(25),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Define row heights
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Content
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            // Department icon
            departmentIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(63, 81, 181),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateDepartmentIcon();

            // Title label
            titleLabel = new Label
            {
                Text = "🏢 TẠO PHÒNG BAN MỚI",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            var subtitleLabel = new Label
            {
                Text = "Nhập thông tin để tạo phòng ban mới trong hệ thống",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(departmentIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateDepartmentIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(63, 81, 181));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    var text = "➕";
                    var size = g.MeasureString(text, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            departmentIcon.Image = bmp;
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 20, 10, 10)
            };

            var formLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            // Column widths
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            // Row heights
            for (int i = 0; i < 3; i++)
                formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            // Department Name (Required)
            departmentNameLabel = CreateLabel("Tên phòng ban *:", true);
            formLayout.Controls.Add(departmentNameLabel, 0, 0);

            departmentNameTextBox = CreateTextBox();
            departmentNameTextBox.Leave += DepartmentNameTextBox_Leave;
            formLayout.Controls.Add(departmentNameTextBox, 1, 0);

            // Description
            departmentDescriptionLabel = CreateLabel("Mô tả:", false);
            formLayout.Controls.Add(departmentDescriptionLabel, 0, 1);

            departmentDescriptionTextBox = CreateTextBox();
            departmentDescriptionTextBox.Multiline = true;
            departmentDescriptionTextBox.Height = 100;
            formLayout.Controls.Add(departmentDescriptionTextBox, 1, 1);

            // Manager
            managerLabel = CreateLabel("Quản lý phòng ban:", false);
            formLayout.Controls.Add(managerLabel, 0, 2);

            managerComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5),
                DisplayMember = "FullName",
                ValueMember = "EmployeeID"
            };

            foreach (var mgr in managers)
            {
                managerComboBox.Items.Add(mgr.FullName);
            }

            if (managerComboBox.Items.Count > 0)
                managerComboBox.SelectedIndex = 0;

            formLayout.Controls.Add(managerComboBox, 1, 2);

            contentPanel.Controls.Add(formLayout);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(25, 15, 25, 15)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            saveButton = CreateFooterButton("💾 Tạo phòng ban", Color.FromArgb(76, 175, 80));
            saveButton.Click += SaveButton_Click;

            resetButton = CreateFooterButton("🔄 Đặt lại", Color.FromArgb(255, 152, 0));
            resetButton.Click += ResetButton_Click;

            cancelButton = CreateFooterButton("❌ Hủy", Color.FromArgb(158, 158, 158));
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(resetButton);
            buttonPanel.Controls.Add(cancelButton);

            // Progress indicator
            var progressPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.Transparent
            };

            var progressLabel = new Label
            {
                Text = "💡 Tip: Các trường có dấu (*) là bắt buộc",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            progressPanel.Controls.Add(progressLabel);

            footerPanel.Controls.Add(buttonPanel);
            footerPanel.Controls.Add(progressPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 2);
        }
        #endregion

        #region Control Creators
        private Label CreateLabel(string text, bool isRequired = false)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = isRequired ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5)
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Height = 30
            };
        }

        private Button CreateFooterButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(140, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }
        #endregion

        #region Event Handlers
        private void DepartmentNameTextBox_Leave(object sender, EventArgs e)
        {
            ValidateDepartmentName();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveDepartment();
                    InsertDepartmentToDatabase();
                    MessageBox.Show("Thêm phòng ban mới thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu phòng ban: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả thông tin?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ResetForm();
            }
        }
        #endregion

        #region Validation
        private bool ValidateDepartmentName()
        {
            errorProvider.SetError(departmentNameTextBox, "");

            if (string.IsNullOrWhiteSpace(departmentNameTextBox.Text))
            {
                errorProvider.SetError(departmentNameTextBox, "Tên phòng ban không được để trống");
                return false;
            }

            // Kiểm tra xem tên phòng ban đã tồn tại chưa
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentName", departmentNameTextBox.Text.Trim());

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        errorProvider.SetError(departmentNameTextBox, "Tên phòng ban này đã tồn tại");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra tên phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = ValidateDepartmentName();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }
        #endregion

        #region Data Operations
        private void SetDefaultValues()
        {
            if (managerComboBox.Items.Count > 0)
                managerComboBox.SelectedIndex = 0;
        }

        private void ResetForm()
        {
            // Clear all text boxes
            departmentNameTextBox.Clear();
            departmentDescriptionTextBox.Clear();

            // Reset manager combo box
            if (managerComboBox.Items.Count > 0)
                managerComboBox.SelectedIndex = 0;

            // Clear error provider
            errorProvider.Clear();
        }

        private void SaveDepartment()
        {
            try
            {
                department.DepartmentName = departmentNameTextBox.Text.Trim();
                department.Description = departmentDescriptionTextBox.Text.Trim();

                // Lấy ID manager từ ComboBox
                if (managerComboBox.SelectedIndex > 0) // Skip "Không có" option
                    department.ManagerID = managers[managerComboBox.SelectedIndex].EmployeeID;
                else
                    department.ManagerID = null;

                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin phòng ban: {ex.Message}", ex);
            }
        }
        #endregion
    }
}