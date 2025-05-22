using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Department
{
    public partial class DepartmentListForm : Form
    {
        #region Fields
        // Business Logic Layer
        private readonly DepartmentBLL _departmentBLL;

        // Data
        private List<DepartmentDTO> departments;
        private List<DepartmentDTO> filteredDepartments;

        // UI
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên phòng ban...";
        #endregion

        #region Constructor
        public DepartmentListForm()
        {
            InitializeComponent();
            _departmentBLL = new DepartmentBLL();
            InitializeLayout();
            LoadDepartmentsFromDatabase();
        }
        #endregion

        #region Data Management
        private void LoadDepartmentsFromDatabase()
        {
            try
            {
                // Sử dụng BLL để lấy danh sách phòng ban
                departments = _departmentBLL.GetAllDepartments();

                // Thiết lập danh sách lọc ban đầu
                filteredDepartments = new List<DepartmentDTO>(departments);

                // Tải dữ liệu lên DataGridView
                LoadDepartmentsToGrid();

                // Cập nhật thống kê
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDepartmentsToGrid()
        {
            try
            {
                // Chuyển đổi danh sách DTO sang DisplayModel
                var dataSource = DepartmentDisplayModel.FromDTOList(filteredDepartments);

                DepartmentDataGridView.DataSource = dataSource;
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
                bool? hasManager = null;

                // Xác định trạng thái quản lý từ combobox
                if (statusComboBox.SelectedIndex > 0)
                {
                    hasManager = statusComboBox.SelectedIndex == 1; // 1 = "Có quản lý", 2 = "Chưa có quản lý"
                }

                // Sử dụng BLL để tìm kiếm
                filteredDepartments = _departmentBLL.SearchDepartments(searchText, hasManager);

                LoadDepartmentsToGrid();
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

            // Tải lại tất cả phòng ban
            filteredDepartments = new List<DepartmentDTO>(departments);
            LoadDepartmentsToGrid();
        }

        private void UpdateStatistics()
        {
            var total = filteredDepartments.Count;
            var withManager = filteredDepartments.Count(d => d.ManagerID.HasValue);
            var withoutManager = filteredDepartments.Count(d => !d.ManagerID.HasValue);
            var totalEmployees = filteredDepartments.Sum(d => d.EmployeeCount);

            statisticsLabel.Text = $"📊 Tổng số phòng ban: {total} | 👤 Có quản lý: {withManager} | ⚠️ Chưa có quản lý: {withoutManager} | 👥 Tổng nhân viên: {totalEmployees}";
        }
        #endregion

        #region Helper Methods
        private DepartmentDTO GetSelectedDepartment()
        {
            if (DepartmentDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = DepartmentDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is DepartmentDisplayModel displayModel)
                {
                    return departments.FirstOrDefault(d => d.DepartmentID == displayModel.DepartmentID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void DepartmentDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = DepartmentDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "ManagerName" && e.Value != null)
            {
                var managerName = e.Value.ToString();
                if (managerName == "Chưa phân công")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(244, 67, 54);
                    e.CellStyle.Font = new Font(DepartmentDataGridView.DefaultCellStyle.Font, FontStyle.Italic);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
            else if (columnName == "EmployeeCount" && e.Value != null)
            {
                int count = Convert.ToInt32(e.Value);
                if (count == 0)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(244, 67, 54);
                }
                else if (count < 5)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 152, 0);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                }
            }
        }

        private void AddDepartment()
        {
            try
            {
                using (var form = new DepartmentCreate())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Chuyển đổi từ Department thành DTO
                        var newDepartment = form.CreatedDepartment;
                        var newDepartmentDTO = DepartmentDTO.FromDepartment(newDepartment);

                        // Thêm phòng ban mới vào database thông qua BLL
                        int newDepartmentId = _departmentBLL.AddDepartment(newDepartmentDTO);
                        newDepartmentDTO.DepartmentID = newDepartmentId;

                        // Tải lại danh sách phòng ban
                        LoadDepartmentsFromDatabase();

                        MessageBox.Show("Thêm phòng ban thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditDepartment()
        {
            var departmentDTO = GetSelectedDepartment();
            if (departmentDTO == null) return;

            try
            {
                using (var form = new DepartmentCreate()) // Sử dụng cùng form create với chế độ chỉnh sửa
                {
                    // Khởi tạo form với dữ liệu hiện tại của phòng ban (cần điều chỉnh DepartmentCreate)
                    // form.InitializeForEditing(departmentDTO.ToDepartment());

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Cập nhật phòng ban trong database thông qua BLL
                        var updatedDepartment = form.CreatedDepartment;
                        var updatedDepartmentDTO = DepartmentDTO.FromDepartment(updatedDepartment);
                        updatedDepartmentDTO.DepartmentID = departmentDTO.DepartmentID;

                        _departmentBLL.UpdateDepartment(updatedDepartmentDTO);

                        // Tải lại danh sách phòng ban
                        LoadDepartmentsFromDatabase();

                        MessageBox.Show("Cập nhật phòng ban thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewDepartment()
        {
            var departmentDTO = GetSelectedDepartment();
            if (departmentDTO == null) return;

            try
            {
                // Hiển thị form chi tiết (chế độ chỉ đọc)
                // Bạn cần tạo thêm form DepartmentDetail
                MessageBox.Show($"Xem chi tiết phòng ban: {departmentDTO.DepartmentName}", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteDepartment()
        {
            var departmentDTO = GetSelectedDepartment();
            if (departmentDTO == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa phòng ban '{departmentDTO.DepartmentName}'?\nHành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    // Xóa phòng ban khỏi database thông qua BLL
                    _departmentBLL.DeleteDepartment(departmentDTO.DepartmentID);

                    // Tải lại danh sách phòng ban
                    LoadDepartmentsFromDatabase();

                    MessageBox.Show("Xóa phòng ban thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa phòng ban: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Phòng ban";
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
                Text = "🏢 QUẢN LÝ PHÒNG BAN",
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
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));  // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));  // Manager filter
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.5f)); // Search button
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.5f)); // Clear button

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

            // Manager ComboBox
            statusComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Có quản lý", "Chưa có quản lý" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Search Button
            searchButton = CreateStyledButton("🔍 TÌM KIẾM", Color.FromArgb(33, 150, 243));
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button
            clearButton = CreateStyledButton("🗑️ XÓA BỘ LỌC", Color.FromArgb(244, 67, 54));
            clearButton.Click += ClearFilters;

            // Add controls to search container
            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(statusComboBox, 1, 0);
            searchContainer.Controls.Add(searchButton, 2, 0);
            searchContainer.Controls.Add(clearButton, 3, 0);

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

            DepartmentDataGridView = new DataGridView
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

            gridPanel.Controls.Add(DepartmentDataGridView);
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

            addButton = CreateActionButton("➕ THÊM PHÒNG BAN", Color.FromArgb(76, 175, 80));
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
            DepartmentDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                Font = new Font("Segoe UI", 9)
            };

            DepartmentDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(63, 81, 181),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(10, 10, 10, 10),
                WrapMode = DataGridViewTriState.False
            };

            DepartmentDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            DepartmentDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "DepartmentID", HeaderText = "ID", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "DepartmentName", HeaderText = "Tên phòng ban", Width = 200, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "Description", HeaderText = "Mô tả", Width = 300, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "ManagerName", HeaderText = "Quản lý", Width = 150, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "EmployeeCount", HeaderText = "Số nhân viên", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "CreatedAt", HeaderText = "Ngày tạo", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "LastUpdated", HeaderText = "Cập nhật cuối", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
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

                if (col.Name == "CreatedAt" || col.Name == "LastUpdated")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                DepartmentDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            DepartmentDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = DepartmentDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
            };

            DepartmentDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    ViewDepartment();
            };

            DepartmentDataGridView.CellFormatting += DepartmentDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddDepartment();
            editButton.Click += (s, e) => EditDepartment();
            viewButton.Click += (s, e) => ViewDepartment();
            deleteButton.Click += (s, e) => DeleteDepartment();
        }
        #endregion
    }
}