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

                // Cập nhật thống kê
                UpdateStatistics();
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
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

        private void UpdateStatistics()
        {
            var total = filteredEmployees.Count;
            var active = filteredEmployees.Count(e => e.Status == "Đang làm việc");
            var onLeave = filteredEmployees.Count(e => e.Status == "Tạm nghỉ");
            var inactive = filteredEmployees.Count(e => e.Status == "Đã nghỉ việc");

            statisticsLabel.Text = $"📊 Tổng: {total} | 👤 Đang làm việc: {active} | ⏸️ Tạm nghỉ: {onLeave} | 🚫 Đã nghỉ việc: {inactive}";
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

            // Define row heights
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Footer

            this.Controls.Add(mainTableLayout);
        }

        private void SetupHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            };

            titleLabel = new Label
            {
                Text = "👥 QUẢN LÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void SetupSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Search controls container
            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Status filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));  // Department filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f)); // Clear button

            // Search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Text = searchPlaceholder,
                ForeColor = Color.Gray,
                Height = 35,
                Margin = new Padding(0, 5, 10, 5)
            };
            SetupSearchTextBoxEvents();

            // Status ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Đang làm việc", "Tạm nghỉ", "Đã nghỉ việc" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Department ComboBox - Tải phòng ban từ database
            departmentComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5),
                DisplayMember = "DepartmentName",
                ValueMember = "DepartmentID"
            };

            // Thêm lựa chọn "Tất cả phòng ban"
            departmentComboBox.Items.Add("Tất cả phòng ban");

            // Thêm phòng ban từ database
            var departments = LoadDepartments();
            foreach (var department in departments)
            {
                departmentComboBox.Items.Add(department.DepartmentName);
            }

            departmentComboBox.SelectedIndex = 0;
            departmentComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(departmentComboBox, 2, 0);
            searchContainer.Controls.Add(searchButton, 3, 0);
            searchContainer.Controls.Add(clearButton, 4, 0);

            searchPanel.Controls.Add(searchContainer);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
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

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20, 15, 20, 15)
            };

            var footerContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM NHÂN VIÊN", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "📊 Đang tải..."
            };

            statsPanel.Controls.Add(statisticsLabel);

            footerContainer.Controls.Add(buttonsPanel, 0, 0);
            footerContainer.Controls.Add(statsPanel, 1, 0);

            footerPanel.Controls.Add(footerContainer);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 5),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(140, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 15, 0),
                FlatAppearance = { BorderSize = 0 }
            };
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
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = searchPlaceholder;
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            searchTextBox.TextChanged += (s, e) =>
            {
                if (searchTextBox.Text != searchPlaceholder)
                    ApplyFilters();
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