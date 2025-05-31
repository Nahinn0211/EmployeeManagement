using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagement.GUI.Employee
{
    public partial class EmployeeDetail : Form
    {
        private Models.Entity.Employee employee;
        private bool viewOnly;
        private TableLayoutPanel mainTableLayout;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;

        // Header controls
        private Label titleLabel;
        private PictureBox employeeIcon;

        // Content controls
        private TabControl tabControl;
        private TabPage basicInfoTab;
        private TabPage detailsTab;

        // Basic info tab controls
        private TextBox employeeCodeTextBox;
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private ComboBox genderComboBox;
        private DateTimePicker dateOfBirthPicker;
        private TextBox idCardTextBox;
        private TextBox phoneTextBox;
        private TextBox emailTextBox;
        private ComboBox departmentComboBox;
        private ComboBox positionComboBox;
        private ComboBox managerComboBox;
        private DateTimePicker hireDatePicker;
        private ComboBox statusComboBox;

        // Details tab controls
        private TextBox addressTextBox;
        private TextBox bankAccountTextBox;
        private TextBox bankNameTextBox;
        private TextBox taxCodeTextBox;
        private TextBox insuranceCodeTextBox;
        private Button uploadPhotoButton;
        private PictureBox photoPreview;
        private TextBox notesTextBox;

        // Footer controls
        private Button saveButton;
        private Button editButton;
        private Button closeButton;

        private readonly string[] genders = { "Nam", "Nữ", "Khác" };
        private readonly string[] departments = { "Ban giám đốc", "Phòng Nhân sự", "Phòng Kế toán", "Phòng IT", "Phòng Kinh doanh" };
        private readonly string[] positions = { "Giám đốc", "Trưởng phòng", "Nhân viên cấp cao", "Nhân viên", "Thực tập sinh" };
        private readonly string[] managers = { "Không có", "Nguyễn Văn A", "Trần Thị B", "Lê Văn C", "Phạm Thị D" };
        private readonly string[] statuses = { "Đang làm việc", "Tạm nghỉ", "Đã nghỉ việc" };

        // Validation
        private ErrorProvider errorProvider;
 
         public EmployeeDetail(Models.Entity.Employee employee, bool viewOnly = false)
        {
             this.employee = employee;
            this.viewOnly = viewOnly;
            this.Text = viewOnly ? "Xem chi tiết nhân viên" : "Chỉnh sửa nhân viên";

            SetupForm();
            LoadEmployeeData();

            if (viewOnly)
            {
                SetControlsReadOnly();
            }
        }
       
        private void SetupForm()
        {
            this.Size = new Size(1000, 800);
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

            // Employee icon
            employeeIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(33, 150, 243),
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateEmployeeIcon();

            // Title label
            titleLabel = new Label
            {
                Text = viewOnly ? "👁️ XEM CHI TIẾT NHÂN VIÊN" : "✏️ CHỈNH SỬA NHÂN VIÊN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(100, 20),
                Size = new Size(600, 40),
                AutoEllipsis = true
            };

            var subtitleLabel = new Label
            {
                Text = viewOnly ? "Thông tin chi tiết hồ sơ nhân viên" : "Chỉnh sửa thông tin nhân viên",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                Location = new Point(100, 55),
                Size = new Size(600, 25)
            };

            headerPanel.Controls.Add(employeeIcon);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            mainTableLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateEmployeeIcon()
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(33, 150, 243));
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    var text = viewOnly ? "👁️" : "✏️";
                    var size = g.MeasureString(text, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            employeeIcon.Image = bmp;
        }

        private void SetupContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Tab control
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
            SetupDetailsTab();

            tabControl.TabPages.Add(basicInfoTab);
            tabControl.TabPages.Add(detailsTab);

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

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var basicLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 600,
                ColumnCount = 4,
                RowCount = 8,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            // Column widths
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            basicLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Row heights
            for (int i = 0; i < 8; i++)
                basicLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));

            // Employee Code (Required)
            basicLayout.Controls.Add(CreateLabel("Mã nhân viên *:", true), 0, 0);
            employeeCodeTextBox = CreateTextBox();
            employeeCodeTextBox.Leave += EmployeeCodeTextBox_Leave;
            basicLayout.Controls.Add(employeeCodeTextBox, 1, 0);

            // Status
            basicLayout.Controls.Add(CreateLabel("Trạng thái:", true), 2, 0);
            statusComboBox = CreateComboBox(statuses);
            statusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;
            basicLayout.Controls.Add(statusComboBox, 3, 0);

            // First Name (Required)
            basicLayout.Controls.Add(CreateLabel("Họ *:", true), 0, 1);
            firstNameTextBox = CreateTextBox();
            firstNameTextBox.Leave += FirstNameTextBox_Leave;
            basicLayout.Controls.Add(firstNameTextBox, 1, 1);

            // Last Name (Required)
            basicLayout.Controls.Add(CreateLabel("Tên *:", true), 2, 1);
            lastNameTextBox = CreateTextBox();
            lastNameTextBox.Leave += LastNameTextBox_Leave;
            basicLayout.Controls.Add(lastNameTextBox, 3, 1);

            // Gender
            basicLayout.Controls.Add(CreateLabel("Giới tính:", true), 0, 2);
            genderComboBox = CreateComboBox(genders);
            basicLayout.Controls.Add(genderComboBox, 1, 2);

            // Date of Birth
            basicLayout.Controls.Add(CreateLabel("Ngày sinh:", true), 2, 2);
            dateOfBirthPicker = CreateDateTimePicker();
            dateOfBirthPicker.MaxDate = DateTime.Now.AddYears(-18); // Phải ít nhất 18 tuổi
            basicLayout.Controls.Add(dateOfBirthPicker, 3, 2);

            // ID Card (Required)
            basicLayout.Controls.Add(CreateLabel("Số CMND/CCCD *:", true), 0, 3);
            idCardTextBox = CreateTextBox();
            idCardTextBox.Leave += IdCardTextBox_Leave;
            idCardTextBox.KeyPress += NumbersOnly_KeyPress;
            basicLayout.Controls.Add(idCardTextBox, 1, 3);

            // Phone (Required)
            basicLayout.Controls.Add(CreateLabel("Số điện thoại *:", true), 2, 3);
            phoneTextBox = CreateTextBox();
            phoneTextBox.Leave += PhoneTextBox_Leave;
            phoneTextBox.KeyPress += NumbersOnly_KeyPress;
            basicLayout.Controls.Add(phoneTextBox, 3, 3);

            // Email
            basicLayout.Controls.Add(CreateLabel("Email *:", true), 0, 4);
            emailTextBox = CreateTextBox();
            emailTextBox.Leave += EmailTextBox_Leave;
            basicLayout.Controls.Add(emailTextBox, 1, 4);

            // Department (Required)
            basicLayout.Controls.Add(CreateLabel("Phòng ban *:", true), 2, 4);
            departmentComboBox = CreateComboBox(departments);
            basicLayout.Controls.Add(departmentComboBox, 3, 4);

            // Position (Required)
            basicLayout.Controls.Add(CreateLabel("Chức vụ *:", true), 0, 5);
            positionComboBox = CreateComboBox(positions);
            basicLayout.Controls.Add(positionComboBox, 1, 5);

            // Manager
            basicLayout.Controls.Add(CreateLabel("Quản lý trực tiếp:", true), 2, 5);
            managerComboBox = CreateComboBox(managers);
            basicLayout.Controls.Add(managerComboBox, 3, 5);

            // Hire Date (Required)
            basicLayout.Controls.Add(CreateLabel("Ngày vào làm *:", true), 0, 6);
            hireDatePicker = CreateDateTimePicker();
            basicLayout.Controls.Add(hireDatePicker, 1, 6);

            scrollPanel.Controls.Add(basicLayout);
            basicInfoTab.Controls.Add(scrollPanel);
        }

        private void SetupDetailsTab()
        {
            detailsTab = new TabPage
            {
                Text = "Thông tin chi tiết",
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Color.White
            };

            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

            // Address
            var addressPanel = CreateSectionPanel("Địa chỉ:", out addressTextBox, true);
            detailsLayout.Controls.Add(addressPanel, 0, 0);
            detailsLayout.SetColumnSpan(addressPanel, 2);

            // Bank Account
            var bankAccountPanel = CreateSectionPanel("Số tài khoản ngân hàng:", out bankAccountTextBox, false);
            detailsLayout.Controls.Add(bankAccountPanel, 0, 1);

            // Bank Name
            var bankNamePanel = CreateSectionPanel("Tên ngân hàng:", out bankNameTextBox, false);
            detailsLayout.Controls.Add(bankNamePanel, 1, 1);

            // Tax Code
            var taxCodePanel = CreateSectionPanel("Mã số thuế:", out taxCodeTextBox, false);
            detailsLayout.Controls.Add(taxCodePanel, 0, 2);

            // Insurance Code
            var insuranceCodePanel = CreateSectionPanel("Mã bảo hiểm xã hội:", out insuranceCodeTextBox, false);
            detailsLayout.Controls.Add(insuranceCodePanel, 1, 2);

            // Photo and Notes
            var photoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var photoLabel = new Label
            {
                Text = "Ảnh nhân viên:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            photoPreview = new PictureBox
            {
                Size = new Size(150, 180),
                Location = new Point(10, 40),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(240, 240, 240),
                Image = null
            };

            uploadPhotoButton = new Button
            {
                Text = "📷 Tải ảnh lên",
                Size = new Size(150, 35),
                Location = new Point(10, 230),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            uploadPhotoButton.FlatAppearance.BorderSize = 0;
            uploadPhotoButton.Click += UploadPhotoButton_Click;

            photoPanel.Controls.Add(uploadPhotoButton);
            photoPanel.Controls.Add(photoPreview);
            photoPanel.Controls.Add(photoLabel);

            var notesPanel = CreateSectionPanel("Ghi chú:", out notesTextBox, true);

            detailsLayout.Controls.Add(photoPanel, 0, 3);
            detailsLayout.Controls.Add(notesPanel, 1, 3);

            detailsTab.Controls.Add(detailsLayout);
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

            if (viewOnly)
            {
                // Trong chế độ xem, hiển thị nút Chỉnh sửa và Đóng
                editButton = CreateFooterButton("✏️ Chỉnh sửa", Color.FromArgb(255, 152, 0));
                editButton.Click += EditButton_Click;

                closeButton = CreateFooterButton("❌ Đóng", Color.FromArgb(158, 158, 158));
                closeButton.Click += (s, e) => this.Close();

                buttonPanel.Controls.Add(editButton);
                buttonPanel.Controls.Add(closeButton);
            }
            else
            {
                // Trong chế độ chỉnh sửa, hiển thị nút Lưu và Đóng
                saveButton = CreateFooterButton("💾 Lưu thay đổi", Color.FromArgb(76, 175, 80));
                saveButton.Click += SaveButton_Click;

                closeButton = CreateFooterButton("❌ Hủy", Color.FromArgb(158, 158, 158));
                closeButton.Click += (s, e) => this.Close();

                buttonPanel.Controls.Add(saveButton);
                buttonPanel.Controls.Add(closeButton);
            }

            // Progress indicator
            var progressPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.Transparent
            };

            var progressLabel = new Label
            {
                Text = viewOnly ? "🔍 Đang xem thông tin nhân viên" : "💡 Tip: Các trường có dấu (*) là bắt buộc",
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

        private ComboBox CreateComboBox(string[] items)
        {
            var combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            combo.Items.AddRange(items);
            return combo;
        }

        private DateTimePicker CreateDateTimePicker()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5)
            };
        }

        private Panel CreateSectionPanel(string labelText, out TextBox textBox, bool isMultiline = false)
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

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                Multiline = isMultiline,
                ScrollBars = isMultiline ? ScrollBars.Vertical : ScrollBars.None,
                Margin = new Padding(0, 5, 0, 0)
            };

            panel.Controls.Add(textBox);
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

        private void EmployeeCodeTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidateEmployeeCode();
        }

        private void FirstNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidateFirstName();
        }

        private void LastNameTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidateLastName();
        }

        private void IdCardTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidateIdCard();
        }

        private void PhoneTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidatePhone();
        }

        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            if (!viewOnly) ValidateEmail();
        }

        private void NumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow digits and backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the status is "Đã nghỉ việc", set the EndDate to today
            if (statusComboBox.Text == "Đã nghỉ việc" && !viewOnly)
            {
                var result = MessageBox.Show(
                    "Bạn đã chọn trạng thái 'Đã nghỉ việc'.\nBạn có muốn thiết lập ngày kết thúc là ngày hôm nay không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    employee.EndDate = DateTime.Now.Date;
                    // You can display this information somewhere if needed
                }
            }
        }

        private void UploadPhotoButton_Click(object sender, EventArgs e)
        {
            if (viewOnly) return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn ảnh nhân viên";
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        photoPreview.Image = new Bitmap(openFileDialog.FileName);
                        // Trong thực tế, bạn cần lưu đường dẫn của ảnh hoặc chuyển đổi ảnh thành dữ liệu để lưu vào DB
                        employee.FaceDataPath = openFileDialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tải ảnh lên: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            // Chuyển từ chế độ xem sang chế độ chỉnh sửa
            this.DialogResult = DialogResult.Cancel;
            this.Close();

            // Mở form chỉnh sửa
            using (var editForm = new EmployeeDetail(this.employee, false))
            {
                editForm.ShowDialog();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveEmployee();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void LoadEmployeeData()
        {
            // Hiển thị thông tin nhân viên vào các controls
            employeeCodeTextBox.Text = employee.EmployeeCode;
            firstNameTextBox.Text = employee.FirstName;
            lastNameTextBox.Text = employee.LastName;

            // Set giá trị cho ComboBox gender
            for (int i = 0; i < genderComboBox.Items.Count; i++)
            {
                if (genderComboBox.Items[i].ToString() == employee.Gender)
                {
                    genderComboBox.SelectedIndex = i;
                    break;
                }
            }

            dateOfBirthPicker.Value = employee.DateOfBirth;
            idCardTextBox.Text = employee.IDCardNumber;
            phoneTextBox.Text = employee.Phone;
            emailTextBox.Text = employee.Email;

            // Set giá trị cho các comboboxes
            if (employee.DepartmentID > 0 && employee.DepartmentID <= departmentComboBox.Items.Count)
                departmentComboBox.SelectedIndex = employee.DepartmentID - 1;

            if (employee.PositionID > 0 && employee.PositionID <= positionComboBox.Items.Count)
                positionComboBox.SelectedIndex = employee.PositionID - 1;

            if (employee.ManagerID == null)
                managerComboBox.SelectedIndex = 0;
            else if (employee.ManagerID > 0 && employee.ManagerID < managerComboBox.Items.Count)
                managerComboBox.SelectedIndex = (int)employee.ManagerID;

            // Set giá trị cho ComboBox status
            // Set giá trị cho ComboBox status
            for (int i = 0; i < statusComboBox.Items.Count; i++)
            {
                if (statusComboBox.Items[i].ToString() == employee.Status)
                {
                    statusComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Set hire date
            hireDatePicker.Value = employee.HireDate;

            // Load thông tin chi tiết
            addressTextBox.Text = employee.Address;
            bankAccountTextBox.Text = employee.BankAccount;
            bankNameTextBox.Text = employee.BankName;
            taxCodeTextBox.Text = employee.TaxCode;
            insuranceCodeTextBox.Text = employee.InsuranceCode;
 
            // Load ảnh nhân viên nếu có
            if (!string.IsNullOrWhiteSpace(employee.FaceDataPath))
            {
                try
                {
                    photoPreview.Image = new Bitmap(employee.FaceDataPath);
                }
                catch
                {
                    // Nếu không tải được ảnh, bỏ qua
                    photoPreview.Image = null;
                }
            }
        }

        private void SetControlsReadOnly()
        {
            // Set all input controls to read-only
            employeeCodeTextBox.ReadOnly = true;
            firstNameTextBox.ReadOnly = true;
            lastNameTextBox.ReadOnly = true;
            idCardTextBox.ReadOnly = true;
            phoneTextBox.ReadOnly = true;
            emailTextBox.ReadOnly = true;
            addressTextBox.ReadOnly = true;
            bankAccountTextBox.ReadOnly = true;
            bankNameTextBox.ReadOnly = true;
            taxCodeTextBox.ReadOnly = true;
            insuranceCodeTextBox.ReadOnly = true;
            notesTextBox.ReadOnly = true;

            // Disable combo boxes
            genderComboBox.Enabled = false;
            departmentComboBox.Enabled = false;
            positionComboBox.Enabled = false;
            managerComboBox.Enabled = false;
            statusComboBox.Enabled = false;

            // Disable date pickers
            dateOfBirthPicker.Enabled = false;
            hireDatePicker.Enabled = false;

            // Disable photo upload
            uploadPhotoButton.Enabled = false;
        }

        private bool ValidateEmployeeCode()
        {
            errorProvider.SetError(employeeCodeTextBox, "");

            if (string.IsNullOrWhiteSpace(employeeCodeTextBox.Text))
            {
                errorProvider.SetError(employeeCodeTextBox, "Mã nhân viên không được để trống");
                return false;
            }

            if (employeeCodeTextBox.Text.Length < 3)
            {
                errorProvider.SetError(employeeCodeTextBox, "Mã nhân viên phải có ít nhất 3 ký tự");
                return false;
            }

            return true;
        }

        private bool ValidateFirstName()
        {
            errorProvider.SetError(firstNameTextBox, "");

            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                errorProvider.SetError(firstNameTextBox, "Họ không được để trống");
                return false;
            }

            return true;
        }

        private bool ValidateLastName()
        {
            errorProvider.SetError(lastNameTextBox, "");

            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                errorProvider.SetError(lastNameTextBox, "Tên không được để trống");
                return false;
            }

            return true;
        }

        private bool ValidateIdCard()
        {
            errorProvider.SetError(idCardTextBox, "");

            if (string.IsNullOrWhiteSpace(idCardTextBox.Text))
            {
                errorProvider.SetError(idCardTextBox, "Số CMND/CCCD không được để trống");
                return false;
            }

            string text = idCardTextBox.Text.Trim();
            if (text.Length != 9 && text.Length != 12)
            {
                errorProvider.SetError(idCardTextBox, "Số CMND/CCCD phải có 9 hoặc 12 chữ số");
                return false;
            }

            return true;
        }

        private bool ValidatePhone()
        {
            errorProvider.SetError(phoneTextBox, "");

            if (string.IsNullOrWhiteSpace(phoneTextBox.Text))
            {
                errorProvider.SetError(phoneTextBox, "Số điện thoại không được để trống");
                return false;
            }

            string text = phoneTextBox.Text.Trim();
            if (text.Length < 10 || text.Length > 11)
            {
                errorProvider.SetError(phoneTextBox, "Số điện thoại phải có 10 hoặc 11 chữ số");
                return false;
            }

            return true;
        }

        private bool ValidateEmail()
        {
            errorProvider.SetError(emailTextBox, "");

            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                errorProvider.SetError(emailTextBox, "Email không được để trống");
                return false;
            }

            string email = emailTextBox.Text.Trim();
            if (!email.Contains("@") || !email.Contains("."))
            {
                errorProvider.SetError(emailTextBox, "Email không hợp lệ");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            isValid &= ValidateEmployeeCode();
            isValid &= ValidateFirstName();
            isValid &= ValidateLastName();
            isValid &= ValidateIdCard();
            isValid &= ValidatePhone();
            isValid &= ValidateEmail();

            if (!isValid)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đã nhập!", "Lỗi validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedIndex = 0; // Switch to basic info tab
            }

            return isValid;
        }
 
         private void SaveEmployee()
        {
            try
            {
                employee.EmployeeCode = employeeCodeTextBox.Text.Trim();
                employee.FirstName = firstNameTextBox.Text.Trim();
                employee.LastName = lastNameTextBox.Text.Trim();
                employee.Gender = genderComboBox.Text;
                employee.DateOfBirth = dateOfBirthPicker.Value;
                employee.IDCardNumber = idCardTextBox.Text.Trim();
                employee.Address = addressTextBox.Text.Trim();
                employee.Phone = phoneTextBox.Text.Trim();
                employee.Email = emailTextBox.Text.Trim();
                employee.DepartmentID = departmentComboBox.SelectedIndex + 1;
                employee.PositionID = positionComboBox.SelectedIndex + 1;

                if (managerComboBox.SelectedIndex > 0)
                    employee.ManagerID = managerComboBox.SelectedIndex;
                else
                    employee.ManagerID = null;

                employee.HireDate = hireDatePicker.Value;
                employee.Status = statusComboBox.Text;
                employee.BankAccount = bankAccountTextBox.Text.Trim();
                employee.BankName = bankNameTextBox.Text.Trim();
                employee.TaxCode = taxCodeTextBox.Text.Trim();
                employee.InsuranceCode = insuranceCodeTextBox.Text.Trim();
                 employee.UpdatedAt = DateTime.Now;

                // Do something with the updated employee object here
                // For example, update the database
                // EmployeeController.UpdateEmployee(employee);

                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
        
    }
}