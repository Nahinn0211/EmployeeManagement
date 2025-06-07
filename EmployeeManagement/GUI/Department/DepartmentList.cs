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
                Padding = new Padding(15, 12, 15, 12) // Giảm padding
            };

            // Main container - chia 70% cho filters, 30% cho buttons
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0)
            };

            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Filter controls  
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Buttons

            // === FILTER CONTROLS CONTAINER ===
            var filtersContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65)); // Search box
            filtersContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); // Status

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
                Margin = new Padding(6, 6, 12, 6),
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White
            };
            statusComboBox.Items.AddRange(new[] { "Tất cả trạng thái", "Có quản lý", "Chưa có quản lý" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Add filters to container
            filtersContainer.Controls.Add(searchTextBox, 0, 0);
            filtersContainer.Controls.Add(statusComboBox, 1, 0);

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

            // Search Button - FIXED SIZE
            searchButton = new Button
            {
                Text = "🔍 TÌM KIẾM",
                Size = new Size(100, 32), // Fixed size thay vì Dock.Fill
                BackColor = Color.FromArgb(63, 81, 181), // Indigo color
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
            searchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(57, 73, 171);
            searchButton.Click += (s, e) => ApplyFilters();

            // Clear Button - FIXED SIZE
            clearButton = new Button
            {
                Text = "🗑️ XÓA BỘ LỌC",
                Size = new Size(110, 32), // Fixed size thay vì Dock.Fill
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
                Padding = new Padding(20, 12, 20, 12) // Giảm padding
            };

            // Chỉ có buttons panel, không có statistics
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

            // Tạo buttons với kích thước fixed
            addButton = CreateCompactActionButton("➕ THÊM PHÒNG BAN", Color.FromArgb(76, 175, 80));
            editButton = CreateCompactActionButton("✏️ CHỈNH SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateCompactActionButton("👁️ XEM CHI TIẾT", Color.FromArgb(63, 81, 181)); // Indigo color
            deleteButton = CreateCompactActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));

            // Set fixed sizes cho buttons
            addButton.Size = new Size(150, 38);
            editButton.Size = new Size(120, 38);
            viewButton.Size = new Size(130, 38);
            deleteButton.Size = new Size(80, 38);

            // Set initial states
            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);

            footerPanel.Controls.Add(buttonsPanel);
            mainTableLayout.Controls.Add(footerPanel, 0, 3);
        }

        private Button CreateCompactActionButton(string text, Color backColor)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0), // Spacing giữa các buttons
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            button.FlatAppearance.BorderSize = 0;

            // Hover effects
            Color hoverColor = ControlPaint.Dark(backColor, 0.1f);
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

        // Responsive button sizing
        private void AdjustButtonsForScreenSize()
        {
            if (this.Width < 1000)
            {
                // Screen nhỏ - text ngắn
                addButton.Text = "➕ THÊM";
                editButton.Text = "✏️ SỬA";
                viewButton.Text = "👁️ XEM";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(80, 38);
                editButton.Size = new Size(70, 38);
                viewButton.Size = new Size(70, 38);
                deleteButton.Size = new Size(70, 38);
            }
            else if (this.Width < 1300)
            {
                // Screen vừa - text trung bình
                addButton.Text = "➕ THÊM PHÒNG BAN";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(130, 38);
                editButton.Size = new Size(110, 38);
                viewButton.Size = new Size(120, 38);
                deleteButton.Size = new Size(80, 38);
            }
            else
            {
                // Screen lớn - text đầy đủ
                addButton.Text = "➕ THÊM PHÒNG BAN";
                editButton.Text = "✏️ CHỈNH SỬA";
                viewButton.Text = "👁️ XEM CHI TIẾT";
                deleteButton.Text = "🗑️ XÓA";

                addButton.Size = new Size(150, 38);
                editButton.Size = new Size(120, 38);
                viewButton.Size = new Size(130, 38);
                deleteButton.Size = new Size(80, 38);
            }
        }

        // Hook vào resize event để tự động điều chỉnh
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState != FormWindowState.Minimized)
            {
                AdjustButtonsForScreenSize();
            }
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