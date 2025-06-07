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

namespace EmployeeManagement.GUI.Employee
{
    public partial class EmployeeListForm : Form
    {
        #region Fields
        private List<Models.Entity.Employee> employees;
        private List<Models.Entity.Employee> filteredEmployees;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên nhân viên, mã nhân viên...";
        #endregion

        #region Constructor
        public EmployeeListForm()
        {
            InitializeComponent();
            InitializeLayout();
            LoadEmployeesFromDatabase();
        }
        #endregion

        #region Database Methods
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        private void LoadEmployeesFromDatabase()
        {
            try
            {
                employees = new List<Models.Entity.Employee>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT e.EmployeeID, e.EmployeeCode, e.FirstName, e.LastName, e.FullName, 
                               e.Gender, e.DateOfBirth, e.IDCardNumber, e.Address, e.Phone, 
                               e.Email, e.DepartmentID, e.PositionID, e.ManagerID, 
                               e.HireDate, e.EndDate, e.Status, e.BankAccount, e.BankName, 
                               e.TaxCode, e.InsuranceCode, e.Notes, e.CreatedAt, e.UpdatedAt,
                               d.DepartmentName, p.PositionName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        LEFT JOIN Positions p ON e.PositionID = p.PositionID
                        ORDER BY e.EmployeeCode";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.Entity.Employee employee = new Models.Entity.Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                IDCardNumber = reader["IDCardNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                PositionID = Convert.ToInt32(reader["PositionID"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"]),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };

                            // Xử lý các trường nullable
                            if (reader["ManagerID"] != DBNull.Value)
                                employee.ManagerID = Convert.ToInt32(reader["ManagerID"]);

                            if (reader["EndDate"] != DBNull.Value)
                                employee.EndDate = Convert.ToDateTime(reader["EndDate"]);

                            if (reader["BankAccount"] != DBNull.Value)
                                employee.BankAccount = reader["BankAccount"].ToString();

                            if (reader["BankName"] != DBNull.Value)
                                employee.BankName = reader["BankName"].ToString();

                            if (reader["TaxCode"] != DBNull.Value)
                                employee.TaxCode = reader["TaxCode"].ToString();

                            if (reader["InsuranceCode"] != DBNull.Value)
                                employee.InsuranceCode = reader["InsuranceCode"].ToString();

                            if (reader["Notes"] != DBNull.Value)
                                employee.Notes = reader["Notes"].ToString();

                            if (reader["UpdatedAt"] != DBNull.Value)
                                employee.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            // Thêm thông tin phòng ban và chức vụ
                            if (reader["DepartmentName"] != DBNull.Value)
                                employee.Department = new Models.Entity.Department { DepartmentName = reader["DepartmentName"].ToString() };

                            if (reader["PositionName"] != DBNull.Value)
                                employee.Position = new Models.Entity.Position { PositionName = reader["PositionName"].ToString() };

                            employees.Add(employee);
                        }
                    }
                }

                // Thiết lập danh sách lọc ban đầu
                filteredEmployees = new List<Models.Entity.Employee>(employees);

                // Tải dữ liệu lên DataGridView
                LoadEmployeesToGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddEmployeeToDatabase(Models.Entity.Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Employees (
                            EmployeeCode, FirstName, LastName, Gender, DateOfBirth, 
                            IDCardNumber, Address, Phone, Email, DepartmentID, 
                            PositionID, ManagerID, HireDate, Status
                        ) VALUES (
                            @EmployeeCode, @FirstName, @LastName, @Gender, @DateOfBirth,
                            @IDCardNumber, @Address, @Phone, @Email, @DepartmentID,
                            @PositionID, @ManagerID, @HireDate, @Status
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@Gender", employee.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@IDCardNumber", employee.IDCardNumber);
                    command.Parameters.AddWithValue("@Address", employee.Address);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
                    command.Parameters.AddWithValue("@PositionID", employee.PositionID);
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    command.Parameters.AddWithValue("@Status", employee.Status);

                    // Xử lý các tham số có thể null
                    if (employee.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", employee.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    connection.Open();
                    int newEmployeeId = Convert.ToInt32(command.ExecuteScalar());
                    employee.EmployeeID = newEmployeeId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm nhân viên vào cơ sở dữ liệu: {ex.Message}", ex);
            }
        }

        private void UpdateEmployeeInDatabase(Models.Entity.Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        UPDATE Employees SET
                            FirstName = @FirstName,
                            LastName = @LastName,
                            Gender = @Gender,
                            DateOfBirth = @DateOfBirth,
                            IDCardNumber = @IDCardNumber,
                            Address = @Address,
                            Phone = @Phone,
                            Email = @Email,
                            DepartmentID = @DepartmentID,
                            PositionID = @PositionID,
                            ManagerID = @ManagerID,
                            HireDate = @HireDate,
                            EndDate = @EndDate,
                            Status = @Status,
                            UpdatedAt = GETDATE()
                        WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@Gender", employee.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@IDCardNumber", employee.IDCardNumber);
                    command.Parameters.AddWithValue("@Address", employee.Address);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
                    command.Parameters.AddWithValue("@PositionID", employee.PositionID);
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    command.Parameters.AddWithValue("@Status", employee.Status);

                    // Xử lý các tham số có thể null
                    if (employee.ManagerID.HasValue)
                        command.Parameters.AddWithValue("@ManagerID", employee.ManagerID.Value);
                    else
                        command.Parameters.AddWithValue("@ManagerID", DBNull.Value);

                    if (employee.EndDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", employee.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật nhân viên trong cơ sở dữ liệu: {ex.Message}", ex);
            }
        }

        private void DeleteEmployeeFromDatabase(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    // Kiểm tra xem nhân viên có thể xóa được hay không
                    string checkQuery = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Employees WHERE ManagerID = @EmployeeID) AS ManagedEmployees,
                            (SELECT COUNT(*) FROM Projects WHERE ManagerID = @EmployeeID) AS ManagedProjects,
                            (SELECT COUNT(*) FROM Tasks WHERE AssignedToID = @EmployeeID) AS AssignedTasks,
                            (SELECT COUNT(*) FROM ProjectEmployees WHERE EmployeeID = @EmployeeID) AS ProjectParticipations";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int managedEmployees = Convert.ToInt32(reader["ManagedEmployees"]);
                            int managedProjects = Convert.ToInt32(reader["ManagedProjects"]);
                            int assignedTasks = Convert.ToInt32(reader["AssignedTasks"]);
                            int projectParticipations = Convert.ToInt32(reader["ProjectParticipations"]);

                            if (managedEmployees > 0 || managedProjects > 0 || assignedTasks > 0 || projectParticipations > 0)
                            {
                                StringBuilder errorMessage = new StringBuilder("Không thể xóa nhân viên vì còn liên kết với:\n");

                                if (managedEmployees > 0)
                                    errorMessage.AppendLine($"- {managedEmployees} nhân viên đang được quản lý bởi người này");

                                if (managedProjects > 0)
                                    errorMessage.AppendLine($"- {managedProjects} dự án đang được quản lý bởi người này");

                                if (assignedTasks > 0)
                                    errorMessage.AppendLine($"- {assignedTasks} công việc được giao cho người này");

                                if (projectParticipations > 0)
                                    errorMessage.AppendLine($"- {projectParticipations} dự án mà người này tham gia");

                                errorMessage.AppendLine("\nVui lòng cập nhật các liên kết trước khi xóa.");
                                throw new Exception(errorMessage.ToString());
                            }
                        }
                    }

                    // Nếu không có ràng buộc, tiến hành xóa
                    string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
            }
        }

        private List<Models.Entity.Department> LoadDepartments()
        {
            List<Models.Entity.Department> departments = new List<Models.Entity.Department>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new Models.Entity.Department
                            {
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                DepartmentName = reader["DepartmentName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return departments;
        }
        #endregion

        #region Data Management
        
        private void ApplyFilters()
        {
            try
            {
                string searchText = searchTextBox.Text == searchPlaceholder ? "" : searchTextBox.Text.ToLower();
                string statusFilter = statusComboBox.SelectedIndex == 0 ? "" : statusComboBox.Text;
                string departmentFilter = departmentComboBox.SelectedIndex == 0 ? "" : departmentComboBox.Text;

                filteredEmployees = employees.Where(e =>
                    (string.IsNullOrEmpty(searchText) ||
                     e.FullName.ToLower().Contains(searchText) ||
                     e.EmployeeCode.ToLower().Contains(searchText) ||
                     e.Email.ToLower().Contains(searchText) ||
                     e.Phone.ToLower().Contains(searchText)) &&
                    (string.IsNullOrEmpty(statusFilter) || e.Status == statusFilter) &&
                    (string.IsNullOrEmpty(departmentFilter) ||
                     e.Department?.DepartmentName == departmentFilter ||
                     GetDepartmentName(e.DepartmentID) == departmentFilter)
                ).ToList();

                LoadEmployeesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchTextBox.Text = searchPlaceholder;
            searchTextBox.ForeColor = Color.Gray;
            statusComboBox.SelectedIndex = 0;
            departmentComboBox.SelectedIndex = 0;
            filteredEmployees = new List<Models.Entity.Employee>(employees);
            LoadEmployeesToGrid();
        }

        // ==========================================
        // FIX STATISTICS BỊ MẤT Ở GÓCKER PHẢI
        // ==========================================

        // 1. SỬA LẠI PHƯƠNG THỨC SetupFooter() - ĐẢM BẢO STATISTICS HIỂN THỊ
        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 12, 20, 12)
            };

            var footerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // ĐIỀU CHỈNH TỶ LỆ CỘT - ưu tiên statistics hơn
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Buttons - giảm xuống
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Statistics - tăng lên

            // === BUTTONS PANEL ===
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = false
            };

            // Tạo action buttons COMPACT hơn để nhường chỗ cho statistics
            addButton = CreateCompactActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editButton = CreateCompactActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateCompactActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteButton = CreateCompactActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            // Set sizes NHỎ GỌN để nhường chỗ
            addButton.Size = new Size(90, 38);
            editButton.Size = new Size(70, 38);
            viewButton.Size = new Size(70, 38);
            deleteButton.Size = new Size(70, 38);

            // Set initial states
            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

            // === STATISTICS PANEL - QUAN TRỌNG ===
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 0, 0, 0) // Thêm padding left
            };

           

            statsPanel.Controls.Add(statisticsLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

       
        private void SetupFooterAlternative()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 12, 20, 12)
            };

            // === STATISTICS PANEL - ĐẶT TRƯỚC (Dock Right) ===
            var statsPanel = new Panel
            {
                Width = 450, // Fixed width để đảm bảo hiển thị
                Dock = DockStyle.Right,
                BackColor = Color.Transparent,
                Padding = new Padding(15, 0, 0, 0)
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "📊 Đang tải...",
                AutoSize = false,
                BackColor = Color.Transparent // DEBUG - để thấy boundaries
            };

            statsPanel.Controls.Add(statisticsLabel);

            // === BUTTONS PANEL - FILL PHẦN CÒN LẠI ===
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, // Fill phần còn lại
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = false
            };

            // Compact buttons
            addButton = CreateCompactActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editButton = CreateCompactActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateCompactActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteButton = CreateCompactActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            // Smaller sizes
            addButton.Size = new Size(85, 38);
            editButton.Size = new Size(70, 38);
            viewButton.Size = new Size(70, 38);
            deleteButton.Size = new Size(70, 38);

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

            // ADD TO FOOTER - THỨ TỰ QUAN TRỌNG
            footerPanel.Controls.Add(buttonsPanel); // Add buttons first (Fill)
            footerPanel.Controls.Add(statsPanel);   // Add stats second (Right)

            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        // 4. COMPACT STATISTICS - CHỈ HIỂN THỊ CON SỐ
        private void UpdateStatisticsCompact()
        {
            var total = filteredEmployees.Count;
            var active = filteredEmployees.Count(e => e.Status == "Đang làm việc");
            var onLeave = filteredEmployees.Count(e => e.Status == "Tạm nghỉ");
            var inactive = filteredEmployees.Count(e => e.Status == "Đã nghỉ việc");

            // FORMAT CỰC NGẮN
            statisticsLabel.Text = $"{total} nhân viên | {active} đang làm | {onLeave} tạm nghỉ | {inactive} đã nghỉ";

            // HOẶC CHỈ SỐ
            // statisticsLabel.Text = $"📊 {total} | 👤 {active} | ⏸️ {onLeave} | 🚫 {inactive}";
        }

        // 5. DEBUG METHOD - KIỂM TRA STATISTICS LABEL
        private void DebugStatisticsLabel()
        {
            if (statisticsLabel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Statistics Label Debug:");
                System.Diagnostics.Debug.WriteLine($"- Text: {statisticsLabel.Text}");
                System.Diagnostics.Debug.WriteLine($"- Size: {statisticsLabel.Size}");
                System.Diagnostics.Debug.WriteLine($"- Location: {statisticsLabel.Location}");
                System.Diagnostics.Debug.WriteLine($"- Visible: {statisticsLabel.Visible}");
                System.Diagnostics.Debug.WriteLine($"- Parent Size: {statisticsLabel.Parent?.Size}");
                System.Diagnostics.Debug.WriteLine($"- Dock: {statisticsLabel.Dock}");
                System.Diagnostics.Debug.WriteLine($"- Anchor: {statisticsLabel.Anchor}");
            }
        }

        // 7. SỬA LẠI LoadEmployeesToGrid() ĐỂ ĐẢM BẢO UpdateStatistics() ĐƯỢC GỌI
        private void LoadEmployeesToGrid()
        {
            try
            {
                var dataSource = filteredEmployees.Select(e => new EmployeeDisplayModel
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    Phone = e.Phone,
                    Email = e.Email,
                    Department = e.Department?.DepartmentName ?? GetDepartmentName(e.DepartmentID),
                    Position = e.Position?.PositionName ?? GetPositionName(e.PositionID),
                    Status = GetStatusDisplayText(e.Status),
                    HireDate = e.HireDate
                }).ToList();

                employeeDataGridView.DataSource = dataSource;

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        // 9. ALTERNATIVE - SIMPLE STATISTICS TRONG HEADER
        private void AddStatisticsToHeader()
        {
            var headerStatsLabel = new Label
            {
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(this.Width - 320, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "📊 Loading..."
            };

            headerPanel.Controls.Add(headerStatsLabel);

            // Update method to also update header stats
            // statisticsHeaderLabel = headerStatsLabel; // Declare this field
        }

        // 10. RESPONSIVE FOOTER LAYOUT
        private void SetupResponsiveFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 10, 15, 10)
            };

            if (this.Width < 1200)
            {
                // Small screen - stack vertically
                var verticalContainer = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 2,
                    ColumnCount = 1
                };

                verticalContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 60)); // Buttons
                verticalContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 40)); // Stats

                // Add buttons to row 0, stats to row 1
                // ... implementation
            }
            else
            {
                // Large screen - horizontal layout
                SetupFooterAlternative();
            }
        }


        #endregion

        #region Helper Methods
        private string GetDepartmentName(int departmentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT DepartmentName FROM Departments WHERE DepartmentID = @DepartmentID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentID", departmentId);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : "Không xác định";
                }
            }
            catch
            {
                return "Không xác định";
            }
        }

        private string GetPositionName(int positionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT PositionName FROM Positions WHERE PositionID = @PositionID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionID", positionId);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : "Không xác định";
                }
            }
            catch
            {
                return "Không xác định";
            }
        }

        private string GetStatusDisplayText(string status)
        {
            return status switch
            {
                "Đang làm việc" => "👤 Đang làm việc",
                "Tạm nghỉ" => "⏸️ Tạm nghỉ",
                "Đã nghỉ việc" => "🚫 Đã nghỉ việc",
                _ => status
            };
        }

        private Models.Entity.Employee GetSelectedEmployee()
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = employeeDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is EmployeeDisplayModel displayModel)
                {
                    return employees.FirstOrDefault(e => e.EmployeeID == displayModel.EmployeeID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void EmployeeDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = employeeDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    string s when s.Contains("Đang làm việc") => Color.FromArgb(76, 175, 80),
                    string s when s.Contains("Tạm nghỉ") => Color.FromArgb(255, 152, 0),
                    string s when s.Contains("Đã nghỉ việc") => Color.FromArgb(244, 67, 54),
                    _ => Color.FromArgb(64, 64, 64)
                };
            }
            else if (columnName == "Gender" && e.Value != null)
            {
                var gender = e.Value.ToString();
                if (gender == "Nam")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(33, 150, 243);
                }
                else if (gender == "Nữ")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(233, 30, 99);
                }
            }
        }

        private void AddEmployee()
        {
            try
            {
                using (var form = new EmployeeCreate())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Thêm nhân viên mới vào database
                        var newEmployee = form.CreatedEmployee;
                        AddEmployeeToDatabase(newEmployee);

                        // Tải lại danh sách nhân viên
                        LoadEmployeesFromDatabase();

                        MessageBox.Show("Thêm nhân viên thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                using (var form = new EmployeeDetail(employee))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Cập nhật nhân viên trong database
                        UpdateEmployeeInDatabase(employee);

                        // Tải lại danh sách nhân viên
                        LoadEmployeesFromDatabase();

                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                using (var form = new EmployeeDetail(employee, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteEmployee()
        {
            var employee = GetSelectedEmployee();
            if (employee == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân viên '{employee.FullName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    // Xóa nhân viên khỏi database
                    DeleteEmployeeFromDatabase(employee.EmployeeID);

                    // Tải lại danh sách nhân viên
                    LoadEmployeesFromDatabase();

                    MessageBox.Show("Xóa nhân viên thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Nhân viên";
            this.BackColor = Color.White;
            this.Size = new Size(1400, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            SetupMainLayout();
            SetupHeader();
            SetupSearchPanel();
            SetupDataGrid();
            SetupFooter();
        }

        private void SetupMainLayout()
        {
            mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };

            // Giảm heights để giao diện gọn gàng hơn
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));   // Header - giảm từ 80
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Search - giảm từ 100  
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));   // Footer - giảm từ 80

            this.Controls.Add(mainTableLayout);
        }
       
        
     

       private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 5, 10, 5) // Giảm padding
            };

            titleLabel = new Label
            {
                Text = "👥 QUẢN LÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold), // Giảm từ 24 xuống 20
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }
     

        private void SetupDataGrid()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            employeeDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                AllowUserToResizeColumns = true,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 40 },
                ScrollBars = ScrollBars.Both,
                AutoGenerateColumns = false
            };

            SetupDataGridStyles();
            SetupDataGridColumns();
            SetupDataGridEvents();

            gridPanel.Controls.Add(employeeDataGridView);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
        }

      
        private void SetupSearchTextBoxEvents()
        {
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == searchPlaceholder)
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
                searchTextBox.BackColor = Color.FromArgb(250, 250, 250);
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
                searchTextBox.BackColor = Color.White;
            };

            searchTextBox.TextChanged += (s, e) =>
            {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
            };

            // Enter key support
            searchTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyFilters();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
        }

        private void SetupDataGridStyles()
        {
            employeeDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            employeeDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(10, 10, 10, 10),
                WrapMode = DataGridViewTriState.False
            };

            employeeDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            employeeDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "EmployeeID", HeaderText = "ID", Width = 70, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "EmployeeCode", HeaderText = "Mã nhân viên", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "FullName", HeaderText = "Họ và tên", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Gender", HeaderText = "Giới tính", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "DateOfBirth", HeaderText = "Ngày sinh", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "Phone", HeaderText = "Điện thoại", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Email", HeaderText = "Email", Width = 180, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Department", HeaderText = "Phòng ban", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Position", HeaderText = "Chức vụ", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Status", HeaderText = "Trạng thái", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "HireDate", HeaderText = "Ngày vào làm", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.HeaderText,
                    DataPropertyName = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    MinimumWidth = 80,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                if (col.Name == "DateOfBirth" || col.Name == "HireDate")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";

                employeeDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            employeeDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = employeeDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            employeeDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewEmployee();
            };

            employeeDataGridView.CellFormatting += EmployeeDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddEmployee();
            editButton.Click += (s, e) => EditEmployee();
            viewButton.Click += (s, e) => ViewEmployee();
            deleteButton.Click += (s, e) => DeleteEmployee();
        }

        private void AdjustSearchButtonSizes()
        {
            if (this.Width < 1200)
            {
                // Small screen - compact buttons
                searchButton.Text = "🔍";
                clearButton.Text = "🗑️";
                searchButton.Size = new Size(40, 35);
                clearButton.Size = new Size(40, 35);
            }
            else if (this.Width < 1400)
            {
                // Medium screen - short text
                searchButton.Text = "🔍 TÌM";
                clearButton.Text = "🗑️ XÓA";
                searchButton.Size = new Size(80, 35);
                clearButton.Size = new Size(80, 35);
            }
            else
            {
                // Large screen - full text
                searchButton.Text = "🔍 TÌM KIẾM";
                clearButton.Text = "🗑️ XÓA BỘ LỌC";
                searchButton.Size = new Size(120, 35);
                clearButton.Size = new Size(120, 35);
            }
        }

        // Hook vào Resize event
        private void EmployeeListForm_Resize(object sender, EventArgs e)
        {
            AdjustSearchButtonSizes();
        }



        // ==========================================
        // FIX HIỂN THỊ TEXT CHO BUTTONS
        // ==========================================

        // 1. SỬA LẠI PHƯƠNG THỨC AdjustButtonsForScreenSize()
        private void AdjustButtonsForScreenSize()
        {
            // LUÔN HIỂN THỊ TEXT, chỉ thay đổi kích thước
            if (this.Width < 1000) // Giảm ngưỡng xuống 1000px
            {
                // Screen nhỏ - text ngắn + kích thước vừa phải
                addButton.Text = "➕ THÊM";
                editButton.Text = "✏️ SỬA";
                viewButton.Text = "👁️ XEM";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(80, 38);
                editButton.Size = new Size(70, 38);
                viewButton.Size = new Size(70, 38);
                deleteButton.Size = new Size(70, 38);

                // Search buttons cũng text ngắn
                searchButton.Text = "TÌM";
                clearButton.Text = "XÓA";
                searchButton.Size = new Size(60, 32);
                clearButton.Size = new Size(60, 32);
            }
            else if (this.Width < 1300)
            {
                // Screen vừa - text trung bình
                addButton.Text = "➕ THÊM MỚI";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(110, 38);
                editButton.Size = new Size(110, 38);
                viewButton.Size = new Size(120, 38);
                deleteButton.Size = new Size(80, 38);

                searchButton.Text = "🔍 TÌM KIẾM";
                clearButton.Text = "🗑️ XÓA BỘ LỌC";
                searchButton.Size = new Size(100, 32);
                clearButton.Size = new Size(110, 32);
            }
            else
            {
                // Screen lớn - text đầy đủ
                addButton.Text = "➕ THÊM NHÂN VIÊN";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(140, 38);
                editButton.Size = new Size(120, 38);
                viewButton.Size = new Size(130, 38);
                deleteButton.Size = new Size(80, 38);

                searchButton.Text = "🔍 TÌM KIẾM";
                clearButton.Text = "🗑️ XÓA BỘ LỌC";
                searchButton.Size = new Size(110, 32);
                clearButton.Size = new Size(120, 32);
            }
        }

        // 2. CẬP NHẬT PHƯƠNG THỨC CreateCompactActionButton() 
        private Button CreateCompactActionButton(string text, Color backColor)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(110, 38), // Tăng kích thước mặc định
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter, // Đảm bảo text căn giữa
                AutoSize = false // Tắt auto size để control được kích thước
            };

            button.FlatAppearance.BorderSize = 0;

            // Hover effects
            Color hoverColor = ControlPaint.Dark(backColor, 0.1f);
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

       
        // 4. SỬA LẠI SEARCH PANEL - ĐẢM BẢO BUTTONS CÓ TEXT
        private void SetupSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 12, 15, 12)
            };

            // Main container - chia 65% cho filters, 35% cho buttons
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0)
            };

            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65)); // Filter controls  
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); // Buttons

            // === FILTER CONTROLS CONTAINER ===
            var filtersContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Search box
            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Status
            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Department

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 32,
                Margin = new Padding(0, 6, 12, 6),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 32,
                Margin = new Padding(6, 6, 6, 6),
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Đang làm việc", "Tạm nghỉ", "Đã nghỉ việc" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Department ComboBox
            departmentComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 32,
                Margin = new Padding(6, 6, 12, 6),
                DisplayMember = "DepartmentName",
                ValueMember = "DepartmentID",
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White
            };

            // Load departments
            departmentComboBox.Items.Add("Tất cả phòng ban");
            var departments = LoadDepartments();
            foreach (var department in departments)
            {
                departmentComboBox.Items.Add(department.DepartmentName);
            }
            departmentComboBox.SelectedIndex = 0;
            departmentComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Add filters to container
            filtersContainer.Controls.Add(searchTextBox, 0, 0);
            filtersContainer.Controls.Add(statusComboBox, 1, 0);
            filtersContainer.Controls.Add(departmentComboBox, 2, 0);

            // === BUTTONS CONTAINER ===
            var buttonsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 0, 0, 0),
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = false
            };

            // Search Button với text
            searchButton = new Button
            {
                Text = "🔍 TÌM KIẾM",
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 136, 229);
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button với text
            clearButton = new Button
            {
                Text = "🗑️ XÓA BỘ LỌC",
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 6, 0, 6),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 57, 53);
            clearButton.Click += ClearFilters;

            buttonsContainer.Controls.Add(searchButton);
            buttonsContainer.Controls.Add(clearButton);

            // Add to main container
            mainContainer.Controls.Add(filtersContainer, 0, 0);
            mainContainer.Controls.Add(buttonsContainer, 1, 0);

            searchPanel.Controls.Add(mainContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
        }

        // 5. TẮT AUTO-RESIZE TRONG OnLoad VÀ OnResize
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // KHÔNG GỌI AdjustButtonsForScreenSize() ở đây nữa
            // AdjustButtonsForScreenSize(); // COMMENT OUT DÒNG NÀY
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // CHỈ GỌI KHI CẦN THIẾT
            if (this.WindowState != FormWindowState.Minimized)
            {
                AdjustButtonsForScreenSize();
            }
        }

      #endregion



    }

    public class EmployeeDisplayModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }
}