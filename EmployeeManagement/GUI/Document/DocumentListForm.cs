using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;
using MaterialSkin.Controls;

namespace EmployeeManagement.GUI.Document
{
    public partial class DocumentListForm : Form
    {
        #region Fields
        private DocumentBLL documentBLL;
        private List<Models.Document> documents;
        private List<Models.Document> filteredDocuments;
        private readonly string searchPlaceholder = "🔍 Tìm kiếm theo tên tài liệu, mã tài liệu, mô tả...";

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;

        // Search controls
        private TextBox searchTextBox;
        private ComboBox documentTypeComboBox;
        private ComboBox projectComboBox;
        private ComboBox customerComboBox;
        private ComboBox employeeComboBox;
        private Button searchButton;
        private Button clearButton;

        // Grid controls
        private DataGridView documentDataGridView;

        // Footer controls
        private Button addButton;
        private Button editButton;
        private Button viewButton;
        private Button deleteButton;
        private Button openButton;
        private Button downloadButton;
        private Label statisticsLabel;

        // Data for dropdowns
        private List<Models.Project> projects;
        private List<Models.Customer> customers;
        private List<Models.Employee> employees;
        #endregion

        #region Constructor
        public DocumentListForm()
        {
            InitializeComponent();
            documentBLL = new DocumentBLL();
            LoadDropdownData();
            InitializeLayout();
            LoadDocumentsFromDatabase();
        }
        #endregion

        #region Database Methods
        private void LoadDropdownData()
        {
            try
            {
                projects = documentBLL.GetProjectsForDropdown();
                customers = documentBLL.GetCustomersForDropdown();
                employees = documentBLL.GetEmployeesForDropdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dropdown: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDocumentsFromDatabase()
        {
            try
            {
                documents = documentBLL.GetAllDocuments();
                filteredDocuments = new List<Models.Document>(documents);
                LoadDocumentsToGrid();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ cơ sở dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Data Management
        private void LoadDocumentsToGrid()
        {
            try
            {
                var dataSource = filteredDocuments.Select(d => new DocumentDisplayModel
                {
                    DocumentID = d.DocumentID,
                    DocumentCode = d.DocumentCode,
                    DocumentName = d.DocumentName,
                    FileType = d.FileTypeDisplay,
                    DocumentType = d.DocumentTypeDisplay,
                    FileSize = d.FileSize,
                    RelatedTo = d.RelatedToDisplay,
                    UploadedBy = d.UploadedBy?.FullName ?? "N/A",
                    CreatedAt = d.CreatedAt,
                    FileExists = d.FileExists
                }).ToList();

                documentDataGridView.DataSource = dataSource;
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
                string documentType = GetSelectedDocumentType();
                int? projectId = GetSelectedProjectId();
                int? customerId = GetSelectedCustomerId();
                int? employeeId = GetSelectedEmployeeId();

                filteredDocuments = documents.Where(d =>
                    (string.IsNullOrEmpty(searchText) ||
                     d.DocumentName.ToLower().Contains(searchText) ||
                     d.DocumentCode.ToLower().Contains(searchText) ||
                     d.Description.ToLower().Contains(searchText)) &&
                    (string.IsNullOrEmpty(documentType) || d.DocumentType == documentType) &&
                    (!projectId.HasValue || d.ProjectID == projectId) &&
                    (!customerId.HasValue || d.CustomerID == customerId) &&
                    (!employeeId.HasValue || d.EmployeeID == employeeId)
                ).ToList();

                LoadDocumentsToGrid();
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
            documentTypeComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndex = 0;
            filteredDocuments = new List<Models.Document>(documents);
            LoadDocumentsToGrid();
        }

        private void UpdateStatistics()
        {
            var stats = documentBLL.GetDocumentStatistics();
            var filtered = filteredDocuments.Count;

            statisticsLabel.Text = $"📊 Hiển thị: {filtered} | Tổng: {stats.Total} | 📂 Dự án: {stats.Projects} | 🏢 Khách hàng: {stats.Customers} | 👥 Nhân viên: {stats.Employees} | 📁 Chung: {stats.General}";
        }
        #endregion

        #region Helper Methods
        private string GetSelectedDocumentType()
        {
            if (documentTypeComboBox.SelectedIndex <= 0) return "";

            var selectedText = documentTypeComboBox.Text;
            return DocumentTypes.Types.FirstOrDefault(t => DocumentTypes.GetDisplayName(t) == selectedText) ?? "";
        }

        private int? GetSelectedProjectId()
        {
            if (projectComboBox.SelectedIndex <= 0) return null;
            return projects[projectComboBox.SelectedIndex - 1].ProjectID;
        }

        private int? GetSelectedCustomerId()
        {
            if (customerComboBox.SelectedIndex <= 0) return null;
            return customers[customerComboBox.SelectedIndex - 1].CustomerID;
        }

        private int? GetSelectedEmployeeId()
        {
            if (employeeComboBox.SelectedIndex <= 0) return null;
            return employees[employeeComboBox.SelectedIndex - 1].EmployeeID;
        }

        private Models.Document GetSelectedDocument()
        {
            if (documentDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = documentDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is DocumentDisplayModel displayModel)
                {
                    return documents.FirstOrDefault(d => d.DocumentID == displayModel.DocumentID);
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        private void DocumentDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = documentDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName == "FileType" && e.Value != null)
            {
                var fileType = e.Value.ToString();
                e.CellStyle.ForeColor = fileType.Contains("PDF") ? Color.FromArgb(220, 38, 38) :
                                       fileType.Contains("Word") ? Color.FromArgb(59, 130, 246) :
                                       fileType.Contains("Excel") ? Color.FromArgb(34, 197, 94) :
                                       fileType.Contains("PowerPoint") ? Color.FromArgb(249, 115, 22) :
                                       fileType.Contains("Hình ảnh") ? Color.FromArgb(168, 85, 247) :
                                       Color.FromArgb(64, 64, 64);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (columnName == "FileExists" && e.Value != null)
            {
                bool fileExists = (bool)e.Value;
                e.Value = fileExists ? "✅" : "❌";
                e.CellStyle.ForeColor = fileExists ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void AddDocument()
        {
            try
            {
                using (var form = new DocumentDetailForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadDocumentsFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Thêm tài liệu thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditDocument()
        {
            var document = GetSelectedDocument();
            if (document == null) return;

            try
            {
                using (var form = new DocumentDetailForm(document))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadDocumentsFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Cập nhật tài liệu thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewDocument()
        {
            var document = GetSelectedDocument();
            if (document == null) return;

            try
            {
                using (var form = new DocumentDetailForm(document, true))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteDocument()
        {
            var document = GetSelectedDocument();
            if (document == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa tài liệu '{document.DocumentName}'?\nFile vật lý cũng sẽ bị xóa. Hành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (documentBLL.DeleteDocument(document.DocumentID))
                    {
                        LoadDocumentsFromDatabase();
                        MaterialSnackBar snackBar = new MaterialSnackBar("Xóa tài liệu thành công!", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenDocument()
        {
            var document = GetSelectedDocument();
            if (document == null) return;

            try
            {
                if (!document.FileExists)
                {
                    MessageBox.Show("File tài liệu không tồn tại hoặc đã bị xóa!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                documentBLL.OpenDocument(document.DocumentID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadDocument()
        {
            var document = GetSelectedDocument();
            if (document == null) return;

            try
            {
                if (!document.FileExists)
                {
                    MessageBox.Show("File tài liệu không tồn tại hoặc đã bị xóa!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Chọn thư mục để lưu file";

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (documentBLL.DownloadDocument(document.DocumentID, folderDialog.SelectedPath))
                        {
                            MaterialSnackBar snackBar = new MaterialSnackBar("Download tài liệu thành công!", "OK", true);
                            snackBar.Show(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi download tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Layout Setup
        private void InitializeLayout()
        {
            this.Text = "Quản lý Tài liệu";
            this.BackColor = Color.White;
            this.Size = new Size(1600, 900);
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

            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));  // Search
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
                Text = "📁 QUẢN LÝ TÀI LIỆU",
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

            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Define column widths
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));  // Row 1: Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));  // Row 1: Document type
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));  // Row 1: Buttons

            // Define row heights
            searchContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50));  // First row
            searchContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50));  // Second row

            // First row - Search and Document Type
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

            documentTypeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 35,
                Margin = new Padding(5, 5, 10, 5)
            };
            documentTypeComboBox.Items.Add("Tất cả loại tài liệu");
            foreach (var type in DocumentTypes.Types)
            {
                documentTypeComboBox.Items.Add(DocumentTypes.GetDisplayName(type));
            }
            documentTypeComboBox.SelectedIndex = 0;
            documentTypeComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Margin = new Padding(5)
            };

            searchButton = CreateStyledButton("🔍", Color.FromArgb(33, 150, 243), new Size(45, 35));
            searchButton.Click += (s, e) => ApplyFilters();

            clearButton = CreateStyledButton("🗑️", Color.FromArgb(244, 67, 54), new Size(45, 35));
            clearButton.Click += ClearFilters;

            buttonsPanel.Controls.Add(searchButton);
            buttonsPanel.Controls.Add(clearButton);

            searchContainer.Controls.Add(searchTextBox, 0, 0);
            searchContainer.Controls.Add(documentTypeComboBox, 1, 0);
            searchContainer.Controls.Add(buttonsPanel, 2, 0);

            // Second row - Related filters
            var filtersPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new Padding(0, 5, 0, 0)
            };

            filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            // Project ComboBox
            projectComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(0, 0, 5, 0)
            };
            projectComboBox.Items.Add("Tất cả dự án");
            foreach (var project in projects)
            {
                projectComboBox.Items.Add(project.ProjectName);
            }
            projectComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Customer ComboBox
            customerComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 5, 0)
            };
            customerComboBox.Items.Add("Tất cả khách hàng");
            foreach (var customer in customers)
            {
                customerComboBox.Items.Add(customer.CompanyName);
            }
            customerComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            // Employee ComboBox
            employeeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 0, 0)
            };
            employeeComboBox.Items.Add("Tất cả nhân viên");
            foreach (var employee in employees)
            {
                employeeComboBox.Items.Add(employee.FullName);
            }
            employeeComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();

            filtersPanel.Controls.Add(projectComboBox, 0, 0);
            filtersPanel.Controls.Add(customerComboBox, 1, 0);
            filtersPanel.Controls.Add(employeeComboBox, 2, 0);

            searchContainer.Controls.Add(filtersPanel, 0, 1);
            searchContainer.SetColumnSpan(filtersPanel, 3);

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

            documentDataGridView = new DataGridView
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

            gridPanel.Controls.Add(documentDataGridView);
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

            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Buttons
            footerContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Statistics

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            addButton = CreateActionButton("➕ THÊM", Color.FromArgb(76, 175, 80));
            editButton = CreateActionButton("✏️ SỬA", Color.FromArgb(255, 152, 0));
            viewButton = CreateActionButton("👁️ XEM", Color.FromArgb(33, 150, 243));
            deleteButton = CreateActionButton("🗑️ XÓA", Color.FromArgb(244, 67, 54));
            openButton = CreateActionButton("📂 MỞ", Color.FromArgb(139, 69, 19));
            downloadButton = CreateActionButton("💾 TẢI", Color.FromArgb(75, 85, 99));

            editButton.Enabled = false;
            viewButton.Enabled = false;
            deleteButton.Enabled = false;
            openButton.Enabled = false;
            downloadButton.Enabled = false;

            SetupButtonEvents();

            buttonsPanel.Controls.Add(addButton);
            buttonsPanel.Controls.Add(editButton);
            buttonsPanel.Controls.Add(viewButton);
            buttonsPanel.Controls.Add(deleteButton);
            buttonsPanel.Controls.Add(openButton);
            buttonsPanel.Controls.Add(downloadButton);

            // Statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            statisticsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
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

        private Button CreateStyledButton(string text, Color backColor, Size size)
        {
            return new Button
            {
                Text = text,
                Size = size,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(2),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(90, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
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
            documentDataGridView.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                SelectionBackColor = Color.FromArgb(33, 150, 243, 80),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 6, 8, 6),
                Font = new Font("Segoe UI", 9)
            };

            documentDataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(33, 150, 243),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(8, 8, 8, 8),
                WrapMode = DataGridViewTriState.False
            };

            documentDataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };
        }

        private void SetupDataGridColumns()
        {
            documentDataGridView.Columns.Clear();

            var columns = new[]
            {
                new { Name = "DocumentID", HeaderText = "ID", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = false },
                new { Name = "DocumentCode", HeaderText = "Mã TL", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "DocumentName", HeaderText = "Tên tài liệu", Width = 250, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "FileType", HeaderText = "Loại file", Width = 100, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "DocumentType", HeaderText = "Loại TL", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "FileSize", HeaderText = "Kích thước", Width = 80, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "RelatedTo", HeaderText = "Liên quan", Width = 180, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "UploadedBy", HeaderText = "Người tải", Width = 120, Alignment = DataGridViewContentAlignment.MiddleLeft, Visible = true },
                new { Name = "CreatedAt", HeaderText = "Ngày tạo", Width = 120, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true },
                new { Name = "FileExists", HeaderText = "File", Width = 60, Alignment = DataGridViewContentAlignment.MiddleCenter, Visible = true }
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
                    MinimumWidth = 60,
                    Resizable = DataGridViewTriState.True,
                    DefaultCellStyle = { Alignment = col.Alignment },
                    Visible = col.Visible
                };

                if (col.Name == "CreatedAt")
                    column.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                documentDataGridView.Columns.Add(column);
            }
        }

        private void SetupDataGridEvents()
        {
            documentDataGridView.SelectionChanged += (s, e) =>
            {
                bool hasSelection = documentDataGridView.SelectedRows.Count > 0;
                editButton.Enabled = hasSelection;
                viewButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
                openButton.Enabled = hasSelection;
                downloadButton.Enabled = hasSelection;
            };

            documentDataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    OpenDocument();
            };

            documentDataGridView.CellFormatting += DocumentDataGridView_CellFormatting;
        }

        private void SetupButtonEvents()
        {
            addButton.Click += (s, e) => AddDocument();
            editButton.Click += (s, e) => EditDocument();
            viewButton.Click += (s, e) => ViewDocument();
            deleteButton.Click += (s, e) => DeleteDocument();
            openButton.Click += (s, e) => OpenDocument();
            downloadButton.Click += (s, e) => DownloadDocument();
        }
        #endregion
    }
}