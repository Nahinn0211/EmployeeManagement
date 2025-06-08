using System;
using System.Windows.Forms;

namespace EmployeeManagement.GUI.Employee
{
    partial class EmployeeListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainTableLayout = new TableLayoutPanel();
            headerPanel = new Panel();
            searchPanel = new Panel();
            gridPanel = new Panel();
            footerPanel = new Panel();
            titleLabel = new Label();
            searchTextBox = new TextBox();
            statusComboBox = new ComboBox();
            departmentComboBox = new ComboBox();
            searchButton = new Button();
            clearButton = new Button();
            employeeDataGridView = new DataGridView();
            addButton = new Button();
            editButton = new Button();
            viewButton = new Button();
            deleteButton = new Button();
            statisticsLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)employeeDataGridView).BeginInit();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.Size = new Size(200, 100);
            mainTableLayout.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(200, 100);
            headerPanel.TabIndex = 0;
            // 
            // searchPanel
            // 
            searchPanel.Location = new Point(0, 0);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new Size(200, 100);
            searchPanel.TabIndex = 0;
            // 
            // gridPanel
            // 
            gridPanel.Location = new Point(0, 0);
            gridPanel.Name = "gridPanel";
            gridPanel.Size = new Size(200, 100);
            gridPanel.TabIndex = 0;
            // 
            // footerPanel
            // 
            footerPanel.Location = new Point(0, 0);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(200, 100);
            footerPanel.TabIndex = 0;
            // 
            // titleLabel
            // 
            titleLabel.Location = new Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(100, 23);
            titleLabel.TabIndex = 0;
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new Point(0, 0);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(100, 27);
            searchTextBox.TabIndex = 0;
            // 
            // statusComboBox
            // 
            statusComboBox.Location = new Point(0, 0);
            statusComboBox.Name = "statusComboBox";
            statusComboBox.Size = new Size(121, 28);
            statusComboBox.TabIndex = 0;
            // 
            // departmentComboBox
            // 
            departmentComboBox.Location = new Point(0, 0);
            departmentComboBox.Name = "departmentComboBox";
            departmentComboBox.Size = new Size(121, 28);
            departmentComboBox.TabIndex = 0;
            // 
            // searchButton
            // 
            searchButton.Location = new Point(0, 0);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(75, 23);
            searchButton.TabIndex = 0;
            // 
            // clearButton
            // 
            clearButton.Location = new Point(0, 0);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(75, 23);
            clearButton.TabIndex = 0;
            // 
            // employeeDataGridView
            // 
            employeeDataGridView.ColumnHeadersHeight = 29;
            employeeDataGridView.Location = new Point(0, 0);
            employeeDataGridView.Name = "employeeDataGridView";
            employeeDataGridView.RowHeadersWidth = 51;
            employeeDataGridView.Size = new Size(240, 150);
            employeeDataGridView.TabIndex = 0;
            // 
            // addButton
            // 
            addButton.Location = new Point(0, 0);
            addButton.Name = "addButton";
            addButton.Size = new Size(75, 23);
            addButton.TabIndex = 0;
            // 
            // editButton
            // 
            editButton.Location = new Point(0, 0);
            editButton.Name = "editButton";
            editButton.Size = new Size(75, 23);
            editButton.TabIndex = 0;
            // 
            // viewButton
            // 
            viewButton.Location = new Point(0, 0);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(75, 23);
            viewButton.TabIndex = 0;
            // 
            // deleteButton
            // 
            deleteButton.Location = new Point(0, 0);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(75, 23);
            deleteButton.TabIndex = 0;
            // 
            // statisticsLabel
            // 
            statisticsLabel.Location = new Point(0, 0);
            statisticsLabel.Name = "statisticsLabel";
            statisticsLabel.Size = new Size(100, 23);
            statisticsLabel.TabIndex = 0;
            // 
            // EmployeeListForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1268, 596);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "EmployeeListForm";
            Padding = new Padding(23, 27, 23, 27);
            Text = "Quản lý Nhân viên";
            ((System.ComponentModel.ISupportInitialize)employeeDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // Khai báo các fields sử dụng trong form
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.ComboBox statusComboBox;
        private System.Windows.Forms.ComboBox departmentComboBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.DataGridView employeeDataGridView;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button viewButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label statisticsLabel;
    }
}