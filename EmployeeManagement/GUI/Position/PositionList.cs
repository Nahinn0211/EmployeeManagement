using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Position
{
    public partial class PositionListForm : Form
    {
        #region Fields
        // Business Logic Layer
        private readonly PositionBLL _positionBLL;

        // Data
        private List<PositionDTO> positions;
        private List<PositionDTO> filteredPositions;

        // UI
        private readonly string searchPlaceholder = "🔍 Tìm kiếm chức vụ...";
        private NumericUpDown numericMinSalary;
        private NumericUpDown numericMaxSalary;

        // Trạng thái Form
        private bool isEditMode = false;
        private int? currentEditId = null;

        // Form thêm/sửa
        private TextBox txtPositionName;
        private TextBox txtDescription;
        private NumericUpDown numericBaseSalary;
        #endregion

        #region Constructor
        public PositionListForm()
        {
            InitializeComponent();
            _positionBLL = new PositionBLL();
            SetupLayout();
            LoadPositions();
        }
        #endregion

        #region Data Management
        private void LoadPositions()
        {
            try
            {
                // Sử dụng BLL để lấy danh sách chức vụ
                positions = _positionBLL.GetAllPositions();

                // Thiết lập danh sách lọc ban đầu
                filteredPositions = new List<PositionDTO>(positions);

                // Tải dữ liệu lên DataGridView
                LoadPositionsToGrid();

                // Cập nhật thống kê
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPositionsToGrid()
        {
            try
            {
                // Chuyển đổi danh sách DTO sang DisplayModel
                var dataSource = PositionDisplayModel.FromDTOList(filteredPositions);

                positionDataGridView.DataSource = dataSource;
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
                decimal? minSalary = numericMinSalary.Value > 0 ? numericMinSalary.Value : (decimal?)null;
                decimal? maxSalary = numericMaxSalary.Value > 0 ? numericMaxSalary.Value : (decimal?)null;

                // Sử dụng BLL để tìm kiếm
                filteredPositions = _positionBLL.SearchPositions(searchText, minSalary, maxSalary);

                LoadPositionsToGrid();
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
            numericMinSalary.Value = 0;
            numericMaxSalary.Value = 0;

            // Tải lại tất cả chức vụ
            filteredPositions = new List<PositionDTO>(positions);
            LoadPositionsToGrid();
        }

        private void UpdateStatistics()
        {
            var stats = _positionBLL.GetPositionStatistics();

            statisticsLabel.Text = $"📊 Tổng chức vụ: {stats.TotalPositions} | 👤 Tổng nhân viên: {stats.TotalEmployees} | " +
                                  $"💰 Lương TB: {stats.AverageSalary.ToString("N0")} VNĐ";
        }
        #endregion

        #region Form Actions
        private void AddPosition()
        {
            try
            {
                // Kiểm tra dữ liệu nhập
                if (string.IsNullOrWhiteSpace(txtPositionName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chức vụ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng DTO
                var positionDTO = new PositionDTO
                {
                    PositionName = txtPositionName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    BaseSalary = numericBaseSalary.Value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Thêm vào cơ sở dữ liệu thông qua BLL
                _positionBLL.AddPosition(positionDTO);

                // Thông báo thành công
                MessageBox.Show("Thêm chức vụ thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm mới form
                ClearFormFields();
                LoadPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm chức vụ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePosition()
        {
            try
            {
                // Kiểm tra có ID đang sửa không
                if (!currentEditId.HasValue)
                {
                    MessageBox.Show("Không có chức vụ nào được chọn để cập nhật!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu nhập
                if (string.IsNullOrWhiteSpace(txtPositionName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chức vụ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng DTO
                var positionDTO = new PositionDTO
                {
                    PositionID = currentEditId.Value,
                    PositionName = txtPositionName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    BaseSalary = numericBaseSalary.Value,
                    UpdatedAt = DateTime.Now
                };

                // Cập nhật vào cơ sở dữ liệu thông qua BLL
                _positionBLL.UpdatePosition(positionDTO);

                // Thông báo thành công
                MessageBox.Show("Cập nhật chức vụ thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm mới form
                ClearFormFields();
                LoadPositions();

                // Đổi lại trạng thái form
                SetEditMode(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật chức vụ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeletePosition()
        {
            try
            {
                // Lấy chức vụ đang chọn
                if (positionDataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn chức vụ cần xóa!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedPosition = GetSelectedPosition();
                if (selectedPosition == null) return;

                // Xác nhận xóa
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa chức vụ '{selectedPosition.PositionName}'?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    // Xóa khỏi cơ sở dữ liệu thông qua BLL
                    _positionBLL.DeletePosition(selectedPosition.PositionID);

                    // Thông báo thành công
                    MessageBox.Show("Xóa chức vụ thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Làm mới form
                    ClearFormFields();
                    LoadPositions();

                    // Đổi lại trạng thái form
                    SetEditMode(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa chức vụ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormFields()
        {
            txtPositionName.Text = "";
            txtDescription.Text = "";
            numericBaseSalary.Value = 0;
            currentEditId = null;
        }

        private void CancelEdit()
        {
            ClearFormFields();
            SetEditMode(false);
        }

        private void SetEditMode(bool isEdit)
        {
            isEditMode = isEdit;

            saveButton.Text = isEdit ? "✅ Cập nhật" : "💾 Thêm mới";
            cancelButton.Visible = isEdit;
            formGroupLabel.Text = isEdit ? "✏️ SỬA CHỨC VỤ" : "➕ THÊM CHỨC VỤ MỚI";

            // Disable các nút trên lưới khi đang ở chế độ sửa
            editButton.Enabled = !isEdit;
            deleteButton.Enabled = !isEdit;
        }

        private PositionDTO GetSelectedPosition()
        {
            if (positionDataGridView.SelectedRows.Count > 0)
            {
                if (positionDataGridView.SelectedRows[0].DataBoundItem is PositionDisplayModel model)
                {
                    return positions.FirstOrDefault(p => p.PositionID == model.PositionID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (isEditMode)
            {
                UpdatePosition();
            }
            else
            {
                AddPosition();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelEdit();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            var selectedPosition = GetSelectedPosition();
            if (selectedPosition == null) return;

            txtPositionName.Text = selectedPosition.PositionName;
            txtDescription.Text = selectedPosition.Description;
            numericBaseSalary.Value = selectedPosition.BaseSalary;
            currentEditId = selectedPosition.PositionID;

            SetEditMode(true);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeletePosition();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearFilters(sender, e);
        }

        private void PositionDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = positionDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "BaseSalaryFormatted" && e.Value != null)
            {
                // Format lương với màu xanh lá
                e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80);
            }
            else if (columnName == "EmployeeCount" && e.Value != null)
            {
                int count = Convert.ToInt32(e.Value);
                if (count == 0)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(244, 67, 54); // Đỏ
                }
                else if (count < 5)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 152, 0); // Cam
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80); // Xanh lá
                }
            }
        }

        private void SearchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == searchPlaceholder)
            {
                searchTextBox.Text = "";
                searchTextBox.ForeColor = Color.Black;
            }
        }

        private void SearchTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = searchPlaceholder;
                searchTextBox.ForeColor = Color.Gray;
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilters();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
        #endregion

        #region Layout Setup
        private void SetupLayout()
        {
            // Initialize UI components
            InitializeFormPanel();
            InitializeSearchPanel();

            // Register events
            RegisterEvents();
        }

        private void InitializeFormPanel()
        {
            // Group Box for form inputs
            formGroupLabel = new Label
            {
                Text = "➕ THÊM CHỨC VỤ MỚI",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(33, 150, 243)
            };
            formPanel.Controls.Add(formGroupLabel);

            // Position Name
            var namePanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(5) };
            var nameLabel = new Label
            {
                Text = "Tên chức vụ *:",
                Font = new Font("Segoe UI", 9),
                Dock = DockStyle.Top,
                Height = 20
            };
            txtPositionName = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            namePanel.Controls.Add(txtPositionName);
            namePanel.Controls.Add(nameLabel);
            formPanel.Controls.Add(namePanel);

            // Description
            var descPanel = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(5) };
            var descLabel = new Label
            {
                Text = "Mô tả:",
                Font = new Font("Segoe UI", 9),
                Dock = DockStyle.Top,
                Height = 20
            };
            txtDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };
            descPanel.Controls.Add(txtDescription);
            descPanel.Controls.Add(descLabel);
            formPanel.Controls.Add(descPanel);

            // Base Salary
            var salaryPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(5) };
            var salaryLabel = new Label
            {
                Text = "Lương cơ bản:",
                Font = new Font("Segoe UI", 9),
                Dock = DockStyle.Top,
                Height = 20
            };
            numericBaseSalary = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000000000,
                ThousandsSeparator = true,
                Increment = 100000
            };
            salaryPanel.Controls.Add(numericBaseSalary);
            salaryPanel.Controls.Add(salaryLabel);
            formPanel.Controls.Add(salaryPanel);

            // Buttons
            var buttonPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(5) };
            saveButton = new Button
            {
                Text = "💾 Thêm mới",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(5, 5)
            };
            saveButton.FlatAppearance.BorderSize = 0;

            cancelButton = new Button
            {
                Text = "❌ Hủy",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Location = new Point(130, 5),
                Visible = false
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(saveButton);
            formPanel.Controls.Add(buttonPanel);
        }

        private void InitializeSearchPanel()
        {
            // Filter Panel
            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(5)
            };

            // Min Salary Filter
            var minSalaryPanel = new Panel { Size = new Size(150, 30), Margin = new Padding(5, 0, 5, 0) };
            var minSalaryLabel = new Label
            {
                Text = "Lương tối thiểu:",
                Font = new Font("Segoe UI", 8),
                Size = new Size(150, 15),
                Location = new Point(0, 0)
            };
            numericMinSalary = new NumericUpDown
            {
                Size = new Size(150, 25),
                Location = new Point(0, 15),
                Font = new Font("Segoe UI", 9),
                Minimum = 0,
                Maximum = 1000000000,
                ThousandsSeparator = true,
                Increment = 1000000
            };
            minSalaryPanel.Controls.Add(numericMinSalary);
            minSalaryPanel.Controls.Add(minSalaryLabel);

            // Max Salary Filter
            var maxSalaryPanel = new Panel { Size = new Size(150, 30), Margin = new Padding(5, 0, 10, 0) };
            var maxSalaryLabel = new Label
            {
                Text = "Lương tối đa:",
                Font = new Font("Segoe UI", 8),
                Size = new Size(150, 15),
                Location = new Point(0, 0)
            };
            numericMaxSalary = new NumericUpDown
            {
                Size = new Size(150, 25),
                Location = new Point(0, 15),
                Font = new Font("Segoe UI", 9),
                Minimum = 0,
                Maximum = 1000000000,
                ThousandsSeparator = true,
                Increment = 1000000
            };
            maxSalaryPanel.Controls.Add(numericMaxSalary);
            maxSalaryPanel.Controls.Add(maxSalaryLabel);

            filterPanel.Controls.Add(minSalaryPanel);
            filterPanel.Controls.Add(maxSalaryPanel);

            filterContainer.Controls.Add(filterPanel);

            // Update statistics label
            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "📊 Đang tải thông tin..."
            };

            statisticsPanel.Controls.Add(statisticsLabel);
        }

        private void RegisterEvents()
        {
            // Form event handlers
            this.Load += (s, e) => LoadPositions();

            // Button event handlers
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
            editButton.Click += EditButton_Click;
            deleteButton.Click += DeleteButton_Click;
            searchButton.Click += SearchButton_Click;
            clearButton.Click += ClearButton_Click;

            // Search controls events
            searchTextBox.Enter += SearchTextBox_Enter;
            searchTextBox.Leave += SearchTextBox_Leave;
            searchTextBox.KeyDown += SearchTextBox_KeyDown;

            // DataGridView events
            positionDataGridView.CellFormatting += PositionDataGridView_CellFormatting;
            positionDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = positionDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection && !isEditMode;
                deleteButton.Enabled = hasSelection && !isEditMode;
            };
            positionDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && !isEditMode)
                    EditButton_Click(s, e);
            };
        }
        #endregion

        #region Layout Fields
        private Label formGroupLabel;
        private Label statisticsLabel;
        private Button saveButton;
        private Button cancelButton;

        // Designer.cs already defines:
        // - Panel formPanel
        // - Panel filterContainer
        // - Panel statisticsPanel
        // - TextBox searchTextBox
        // - Button editButton
        // - Button deleteButton
        // - Button searchButton
        // - Button clearButton
        // - DataGridView positionDataGridView (renamed from employeeDataGridView)
        #endregion
    }
}