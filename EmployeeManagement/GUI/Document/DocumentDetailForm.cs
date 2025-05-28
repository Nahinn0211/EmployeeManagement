using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagement.BLL;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.GUI.Document
{
    public partial class DocumentDetailForm : Form
    {
        #region Fields
        private DocumentBLL documentBLL;
        private Models.Entity.Document document;
        private bool isReadOnly;
        private bool isEditMode;
        private string selectedFilePath;

        // Layout controls
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox documentIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage fileInfoTab;

        // Basic info controls
        private TextBox documentCodeTextBox;
        private TextBox documentNameTextBox;
        private ComboBox documentTypeComboBox;
        private TextBox descriptionTextBox;

        // Relationship controls
        private ComboBox projectComboBox;
        private ComboBox customerComboBox;
        private ComboBox employeeComboBox;

        // File info controls
        private TextBox filePathTextBox;
        private Button browseFileButton;
        private Button openFileButton;
        private Label fileSizeLabel;
        private Label fileTypeLabel;
        private PictureBox filePreviewPictureBox;

        // Footer controls
        private Button saveButton;
        private Button cancelButton;
        private Button resetButton;

        // Validation
        private ErrorProvider errorProvider;

        // Data for dropdowns
        private System.Collections.Generic.List<Models.Entity.Project> projects;
        private System.Collections.Generic.List<Models.Entity.Customer> customers;
        private System.Collections.Generic.List<Models.Entity.Employee> employees;
        #endregion

        #region Constructors
        public DocumentDetailForm()
        {
            InitializeComponent();
            documentBLL = new DocumentBLL();
            document = new Models.Entity.Document();
            isEditMode = false;
            isReadOnly = false;
            LoadDropdownData();
            SetupForm();
            SetDefaultValues();
        }

        public DocumentDetailForm(Models.Entity.Document existingDocument, bool readOnly = false)
        {
            InitializeComponent();
            documentBLL = new DocumentBLL();
            document = existingDocument ?? throw new ArgumentNullException(nameof(existingDocument));
            isEditMode = true;
            isReadOnly = readOnly;
            LoadDropdownData();
            SetupForm();
            LoadDocumentData();
        }

        public Models.Entity.Document UpdatedDocument => document;
        #endregion

        #region Form Setup
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

        private void SetupForm()
        {
            this.Text = isReadOnly ? "Xem chi tiết tài liệu" :
                       isEditMode ? "Chỉnh sửa tài liệu" : "Thêm tài liệu mới";
            this.Size = new Size(1000, 750);
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

            // Document icon
            documentIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateDocumentIcon();

            // Title label
            string titleText = isReadOnly ? "👁️ CHI TIẾT TÀI LIỆU" :
                              isEditMode ? "✏️ CHỈNH SỬA TÀI LIỆU" : "📁 THÊM TÀI LIỆU MỚI";

            titleLabel = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            string subtitleText = isReadOnly ? "Xem thông tin chi tiết tài liệu" :
                                 isEditMode ? "Cập nhật thông tin tài liệu" : "Nhập thông tin để thêm tài liệu mới";

            var subtitleLabel = new Label
            {
                Text = subtitleText,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(documentIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateDocumentIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string iconText = isReadOnly ? "👁️" : isEditMode ? "✏️" : "📁";
                    var size = g.MeasureString(iconText, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(iconText, font, brush, x, y);
                }
            }
            documentIcon.Image = bmp;
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                ItemSize = new Size(150, 40),
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };

            tabControl.DrawItem += TabControl_DrawItem;

            SetupBasicInfoTab();
            SetupFileInfoTab();

            tabControl.TabPages.Add(basicInfoTab);
            tabControl.TabPages.Add(fileInfoTab);

            contentPanel.Controls.Add(tabControl);
            mainTableLayout.Controls.Add(contentPanel, 0, 1);
        }

        private void SetupBasicInfoTab()
        {
            basicInfoTab = new TabPage
            {
                Text = "Thông tin cơ bản",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            for (int i = 0; i < 6; i++)
                basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            // Document Code (Required)
            var documentCodePanel = CreateInputPanel("Mã tài liệu *:", out documentCodeTextBox);
            documentCodeTextBox.Leave += DocumentCodeTextBox_Leave;
            documentCodeTextBox.ReadOnly = isEditMode; // Không cho edit mã khi đang sửa
            basicLayout.Controls.Add(documentCodePanel, 0, 0);

            // Document Type
            var documentTypePanel = CreateComboPanel("Loại tài liệu:", out documentTypeComboBox);
            documentTypeComboBox.Items.AddRange(DocumentTypes.Types.Select(t => DocumentTypes.GetDisplayName(t)).ToArray());
            if (documentTypeComboBox.Items.Count > 0)
                documentTypeComboBox.SelectedIndex = 0;
            basicLayout.Controls.Add(documentTypePanel, 1, 0);

            // Document Name (Required) - spanning 2 columns
            var documentNamePanel = CreateInputPanel("Tên tài liệu *:", out documentNameTextBox, false, true);
            documentNameTextBox.Leave += DocumentNameTextBox_Leave;
            basicLayout.Controls.Add(documentNamePanel, 0, 1);
            basicLayout.SetColumnSpan(documentNamePanel, 2);

            // Project
            var projectPanel = CreateComboPanel("Dự án:", out projectComboBox);
            projectComboBox.Items.Add("Không chọn");
            foreach (var project in projects)
            {
                projectComboBox.Items.Add(project.ProjectName);
            }
            projectComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndexChanged += RelatedComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(projectPanel, 0, 2);

            // Customer
            var customerPanel = CreateComboPanel("Khách hàng:", out customerComboBox);
            customerComboBox.Items.Add("Không chọn");
            foreach (var customer in customers)
            {
                customerComboBox.Items.Add(customer.CompanyName);
            }
            customerComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndexChanged += RelatedComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(customerPanel, 1, 2);

            // Employee - spanning 2 columns
            var employeePanel = CreateComboPanel("Nhân viên:", out employeeComboBox);
            employeeComboBox.Items.Add("Không chọn");
            foreach (var employee in employees)
            {
                employeeComboBox.Items.Add(employee.FullName);
            }
            employeeComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndexChanged += RelatedComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(employeePanel, 0, 3);
            basicLayout.SetColumnSpan(employeePanel, 2);

            // Description - spanning 2 columns and remaining rows
            var descriptionPanel = CreateInputPanel("Mô tả:", out descriptionTextBox, true);
            basicLayout.Controls.Add(descriptionPanel, 0, 4);
            basicLayout.SetColumnSpan(descriptionPanel, 2);
            basicLayout.SetRowSpan(descriptionPanel, 2);

            basicInfoTab.Controls.Add(basicLayout);
        }

        private void SetupFileInfoTab()
        {
            fileInfoTab = new TabPage
            {
                Text = "Thông tin file",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var fileLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            fileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            fileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // File Path
            var filePathPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var filePathLabel = new Label
            {
                Text = "Đường dẫn file:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var filePathContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 0)
            };

            filePathTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Margin = new Padding(0, 0, 80, 0)
            };

            browseFileButton = new Button
            {
                Text = "📁 Chọn",
                Size = new Size(70, 25),
                Location = new Point(filePathContainer.Width - 75, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Enabled = !isReadOnly
            };
            browseFileButton.FlatAppearance.BorderSize = 0;
            browseFileButton.Click += BrowseFileButton_Click;

            filePathContainer.Controls.Add(filePathTextBox);
            filePathContainer.Controls.Add(browseFileButton);

            filePathPanel.Controls.Add(filePathContainer);
            filePathPanel.Controls.Add(filePathLabel);

            fileLayout.Controls.Add(filePathPanel, 0, 0);

            // File Info Panel
            var fileInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var fileInfoLabel = new Label
            {
                Text = "Thông tin file:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            fileTypeLabel = new Label
            {
                Text = "Loại: Chưa chọn",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            fileSizeLabel = new Label
            {
                Text = "Kích thước: 0 KB",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            fileInfoPanel.Controls.Add(fileSizeLabel);
            fileInfoPanel.Controls.Add(fileTypeLabel);
            fileInfoPanel.Controls.Add(fileInfoLabel);

            fileLayout.Controls.Add(fileInfoPanel, 1, 0);

            // Open File Button - spanning 2 columns
            var openFilePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            openFileButton = new Button
            {
                Text = "📂 Mở file",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(139, 69, 19),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Enabled = false
            };
            openFileButton.FlatAppearance.BorderSize = 0;
            openFileButton.Click += OpenFileButton_Click;

            openFilePanel.Controls.Add(openFileButton);
            fileLayout.Controls.Add(openFilePanel, 0, 1);
            fileLayout.SetColumnSpan(openFilePanel, 2);

            // File Preview
            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var previewLabel = new Label
            {
                Text = "Xem trước:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            filePreviewPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 5, 0, 0)
            };

            previewPanel.Controls.Add(filePreviewPictureBox);
            previewPanel.Controls.Add(previewLabel);

            fileLayout.Controls.Add(previewPanel, 0, 2);
            fileLayout.SetColumnSpan(previewPanel, 2);
            fileLayout.SetRowSpan(previewPanel, 2);

            fileInfoTab.Controls.Add(fileLayout);
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

            if (!isReadOnly)
            {
                string saveText = isEditMode ? "💾 Cập nhật" : "💾 Tạo tài liệu";
                saveButton = CreateFooterButton(saveText, Color.FromArgb(76, 175, 80));
                saveButton.Click += SaveButton_Click;

                resetButton = CreateFooterButton("🔄 Đặt lại", Color.FromArgb(255, 152, 0));
                resetButton.Click += ResetButton_Click;

                buttonPanel.Controls.Add(saveButton);
                buttonPanel.Controls.Add(resetButton);
            }

            cancelButton = CreateFooterButton(isReadOnly ? "❌ Đóng" : "❌ Hủy", Color.FromArgb(158, 158, 158));
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(cancelButton);

            // Progress indicator
            var progressPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                BackColor = Color.Transparent
            };

            string tipText = isReadOnly ? "💡 Chế độ xem - Không thể chỉnh sửa" : "💡 Tip: Chọn file để upload hoặc để trống nếu chỉ lưu thông tin";
            var progressLabel = new Label
            {
                Text = tipText,
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
        private Panel CreateInputPanel(string labelText, out TextBox textBox, bool isMultiline = false, bool spanColumns = false)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = labelText.Contains("*") ? Color.FromArgb(220, 38, 38) : Color.FromArgb(64, 64, 64)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = isMultiline,
                ScrollBars = isMultiline ? ScrollBars.Vertical : ScrollBars.None,
                Margin = new Padding(0, 5, 0, 0),
                ReadOnly = isReadOnly
            };

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);

            return panel;
        }

        private Panel CreateComboPanel(string labelText, out ComboBox comboBox)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            comboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 0, 0),
                Enabled = !isReadOnly
            };

            panel.Controls.Add(comboBox);
            panel.Controls.Add(label);

            return panel;
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
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = (TabControl)sender;
            var tabPage = tabControl.TabPages[e.Index];
            var tabRect = tabControl.GetTabRect(e.Index);

            var brush = e.State == DrawItemState.Selected
                ? new SolidBrush(Color.FromArgb(33, 150, 243))
                : new SolidBrush(Color.FromArgb(240, 240, 240));

            e.Graphics.FillRectangle(brush, tabRect);

            var textColor = e.State == DrawItemState.Selected ? Color.White : Color.FromArgb(64, 64, 64);
            var textBrush = new SolidBrush(textColor);
            var font = new Font("Segoe UI", 10, FontStyle.Bold);

            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            e.Graphics.DrawString(tabPage.Text, font, textBrush, tabRect, stringFormat);

            brush.Dispose();
            textBrush.Dispose();
            font.Dispose();
        }

        private void DocumentCodeTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly && !isEditMode)
                ValidateDocumentCode();
        }

        private void DocumentNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!isReadOnly)
                ValidateDocumentName();
        }

        private void RelatedComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReadOnly) return;

            var senderCombo = sender as ComboBox;

            // Ensure only one relationship is selected
            if (senderCombo == projectComboBox && projectComboBox.SelectedIndex > 0)
            {
                customerComboBox.SelectedIndex = 0;
                employeeComboBox.SelectedIndex = 0;
            }
            else if (senderCombo == customerComboBox && customerComboBox.SelectedIndex > 0)
            {
                projectComboBox.SelectedIndex = 0;
                employeeComboBox.SelectedIndex = 0;
            }
            else if (senderCombo == employeeComboBox && employeeComboBox.SelectedIndex > 0)
            {
                projectComboBox.SelectedIndex = 0;
                customerComboBox.SelectedIndex = 0;
            }
        }

        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn file tài liệu";
                openFileDialog.Filter = "Tất cả file được hỗ trợ|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.jpg;*.jpeg;*.png;*.gif;*.zip;*.rar;*.7z;*.txt|" +
                                       "PDF files (*.pdf)|*.pdf|" +
                                       "Word files (*.doc;*.docx)|*.doc;*.docx|" +
                                       "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|" +
                                       "PowerPoint files (*.ppt;*.pptx)|*.ppt;*.pptx|" +
                                       "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|" +
                                       "Archive files (*.zip;*.rar;*.7z)|*.zip;*.rar;*.7z|" +
                                       "Text files (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (documentBLL.ValidateFileUpload(openFileDialog.FileName))
                        {
                            selectedFilePath = openFileDialog.FileName;
                            filePathTextBox.Text = Path.GetFileName(selectedFilePath);

                            UpdateFileInfo();
                            LoadFilePreview();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"File không hợp lệ: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (isEditMode && document.FileExists)
                {
                    documentBLL.OpenDocument(document.DocumentID);
                }
                else if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
                {
                    System.Diagnostics.Process.Start(selectedFilePath);
                }
                else
                {
                    MessageBox.Show("Không có file để mở!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveDocument();

                    if (isEditMode)
                    {
                        if (documentBLL.UpdateDocument(document, selectedFilePath))
                        {
                            MessageBox.Show("Cập nhật tài liệu thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        // Set current user as uploaded by (you may need to get this from session/login)
                        document.UploadedByID = 1; // Replace with actual current user ID

                        int newDocumentId = documentBLL.AddDocument(document, selectedFilePath);
                        document.DocumentID = newDocumentId;
                        MessageBox.Show("Thêm tài liệu mới thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu tài liệu: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đặt lại tất cả thông tin?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (isEditMode)
                    LoadDocumentData();
                else
                    ResetForm();
            }
        }
        #endregion

        #region File Management
        private void UpdateFileInfo()
        {
            if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
            {
                var fileInfo = new FileInfo(selectedFilePath);

                fileTypeLabel.Text = $"Loại: {fileInfo.Extension.TrimStart('.').ToUpper()}";
                fileSizeLabel.Text = $"Kích thước: {GetFormattedFileSize(fileInfo.Length)}";
                openFileButton.Enabled = true;
            }
            else if (isEditMode && document.FileExists)
            {
                fileTypeLabel.Text = $"Loại: {document.FileType}";
                fileSizeLabel.Text = $"Kích thước: {document.FileSize}";
                openFileButton.Enabled = true;
            }
            else
            {
                fileTypeLabel.Text = "Loại: Chưa chọn";
                fileSizeLabel.Text = "Kích thước: 0 KB";
                openFileButton.Enabled = false;
            }
        }

        private void LoadFilePreview()
        {
            try
            {
                string filePath = selectedFilePath ?? document.FilePath;

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    filePreviewPictureBox.Image = CreateFileTypeIcon("Unknown");
                    return;
                }

                string extension = Path.GetExtension(filePath).ToLower();

                // For image files, show actual preview
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    try
                    {
                        filePreviewPictureBox.Image = new Bitmap(filePath);
                        return;
                    }
                    catch
                    {
                        // Fall through to show file type icon
                    }
                }

                // For other files, show file type icon
                filePreviewPictureBox.Image = CreateFileTypeIcon(extension);
            }
            catch (Exception ex)
            {
                // Show default icon on error
                filePreviewPictureBox.Image = CreateFileTypeIcon("Unknown");
            }
        }

        private Bitmap CreateFileTypeIcon(string extension)
        {
            var bitmap = new Bitmap(200, 150);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(248, 249, 250));

                string iconText = extension.ToUpper() switch
                {
                    ".PDF" => "📄",
                    ".DOC" or ".DOCX" => "📝",
                    ".XLS" or ".XLSX" => "📊",
                    ".PPT" or ".PPTX" => "📈",
                    ".ZIP" or ".RAR" or ".7Z" => "📦",
                    ".TXT" => "📄",
                    _ => "📁"
                };

                using (var font = new Font("Segoe UI", 48))
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                {
                    var size = g.MeasureString(iconText, font);
                    var x = (bitmap.Width - size.Width) / 2;
                    var y = (bitmap.Height - size.Height) / 2 - 10;
                    g.DrawString(iconText, font, brush, x, y);
                }

                string typeText = extension.ToUpper().TrimStart('.');
                if (string.IsNullOrEmpty(typeText)) typeText = "FILE";

                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(64, 64, 64)))
                {
                    var size = g.MeasureString(typeText, font);
                    var x = (bitmap.Width - size.Width) / 2;
                    var y = bitmap.Height / 2 + 30;
                    g.DrawString(typeText, font, brush, x, y);
                }
            }

            return bitmap;
        }

        private string GetFormattedFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
        #endregion

        #region Validation
        private bool ValidateDocumentCode()
        {
            errorProvider.SetError(documentCodeTextBox, "");

            if (string.IsNullOrWhiteSpace(documentCodeTextBox.Text))
            {
                errorProvider.SetError(documentCodeTextBox, "Mã tài liệu không được để trống");
                return false;
            }

            if (documentCodeTextBox.Text.Length < 3)
            {
                errorProvider.SetError(documentCodeTextBox, "Mã tài liệu phải có ít nhất 3 ký tự");
                return false;
            }

            // Kiểm tra trùng lặp
            try
            {
                int excludeId = isEditMode ? document.DocumentID : 0;
                if (documentBLL.IsDocumentCodeExists(documentCodeTextBox.Text.Trim(), excludeId))
                {
                    errorProvider.SetError(documentCodeTextBox, "Mã tài liệu này đã tồn tại");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra mã tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateDocumentName()
        {
            errorProvider.SetError(documentNameTextBox, "");

            if (string.IsNullOrWhiteSpace(documentNameTextBox.Text))
            {
                errorProvider.SetError(documentNameTextBox, "Tên tài liệu không được để trống");
                return false;
            }

            if (documentNameTextBox.Text.Length > 200)
            {
                errorProvider.SetError(documentNameTextBox, "Tên tài liệu không được vượt quá 200 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (!isEditMode) // Chỉ validate document code khi thêm mới
                isValid &= ValidateDocumentCode();

            isValid &= ValidateDocumentName();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Switch to basic info tab
            }

            return isValid;
        }
        #endregion

        #region Data Operations
        private void SetDefaultValues()
        {
            if (!isEditMode)
            {
                documentCodeTextBox.Text = documentBLL.GenerateDocumentCode();
                if (documentTypeComboBox.Items.Count > 0)
                    documentTypeComboBox.SelectedIndex = 0;

                projectComboBox.SelectedIndex = 0;
                customerComboBox.SelectedIndex = 0;
                employeeComboBox.SelectedIndex = 0;
            }
        }

        private void LoadDocumentData()
        {
            if (document == null) return;

            try
            {
                documentCodeTextBox.Text = document.DocumentCode;
                documentNameTextBox.Text = document.DocumentName;
                descriptionTextBox.Text = document.Description;
                filePathTextBox.Text = document.GetFileName();

                // Set document type
                if (!string.IsNullOrEmpty(document.DocumentType))
                {
                    string displayName = DocumentTypes.GetDisplayName(document.DocumentType);
                    int typeIndex = documentTypeComboBox.Items.IndexOf(displayName);
                    if (typeIndex >= 0)
                        documentTypeComboBox.SelectedIndex = typeIndex;
                }

                // Set relationships
                projectComboBox.SelectedIndex = 0;
                customerComboBox.SelectedIndex = 0;
                employeeComboBox.SelectedIndex = 0;

                if (document.ProjectID.HasValue)
                {
                    var project = projects.FirstOrDefault(p => p.ProjectID == document.ProjectID.Value);
                    if (project != null)
                    {
                        int index = projectComboBox.Items.IndexOf(project.ProjectName);
                        if (index >= 0) projectComboBox.SelectedIndex = index;
                    }
                }
                else if (document.CustomerID.HasValue)
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerID == document.CustomerID.Value);
                    if (customer != null)
                    {
                        int index = customerComboBox.Items.IndexOf(customer.CompanyName);
                        if (index >= 0) customerComboBox.SelectedIndex = index;
                    }
                }
                else if (document.EmployeeID.HasValue)
                {
                    var employee = employees.FirstOrDefault(e => e.EmployeeID == document.EmployeeID.Value);
                    if (employee != null)
                    {
                        int index = employeeComboBox.Items.IndexOf(employee.FullName);
                        if (index >= 0) employeeComboBox.SelectedIndex = index;
                    }
                }

                UpdateFileInfo();
                LoadFilePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tài liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            // Clear all text boxes
            documentCodeTextBox.Clear();
            documentNameTextBox.Clear();
            descriptionTextBox.Clear();
            filePathTextBox.Clear();

            // Reset combo boxes
            if (documentTypeComboBox.Items.Count > 0)
                documentTypeComboBox.SelectedIndex = 0;

            projectComboBox.SelectedIndex = 0;
            customerComboBox.SelectedIndex = 0;
            employeeComboBox.SelectedIndex = 0;

            // Clear file selection
            selectedFilePath = null;

            // Clear error provider
            errorProvider.Clear();

            // Reset file info
            UpdateFileInfo();
            LoadFilePreview();

            // Set default values again
            SetDefaultValues();

            // Return to first tab
            tabControl.SelectedIndex = 0;
        }

        private void SaveDocument()
        {
            try
            {
                document.DocumentCode = documentCodeTextBox.Text.Trim();
                document.DocumentName = documentNameTextBox.Text.Trim();
                document.Description = descriptionTextBox.Text.Trim();

                // Set document type
                if (documentTypeComboBox.SelectedIndex >= 0)
                {
                    string selectedDisplay = documentTypeComboBox.Text;
                    document.DocumentType = DocumentTypes.Types.FirstOrDefault(t =>
                        DocumentTypes.GetDisplayName(t) == selectedDisplay) ?? "Other";
                }

                // Set relationships (only one can be set)
                document.ProjectID = null;
                document.CustomerID = null;
                document.EmployeeID = null;

                if (projectComboBox.SelectedIndex > 0)
                {
                    document.ProjectID = projects[projectComboBox.SelectedIndex - 1].ProjectID;
                }
                else if (customerComboBox.SelectedIndex > 0)
                {
                    document.CustomerID = customers[customerComboBox.SelectedIndex - 1].CustomerID;
                }
                else if (employeeComboBox.SelectedIndex > 0)
                {
                    document.EmployeeID = employees[employeeComboBox.SelectedIndex - 1].EmployeeID;
                }

                if (!isEditMode)
                {
                    document.CreatedAt = DateTime.Now;
                }
                document.UpdatedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thông tin tài liệu: {ex.Message}", ex);
            }
        }
        #endregion
    }
}