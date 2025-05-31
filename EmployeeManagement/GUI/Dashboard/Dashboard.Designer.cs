namespace EmployeeManagement.GUI
{
    partial class DashboardForm
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
      
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label welcomeLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Panel avatarPanel;
        private System.Windows.Forms.Panel statsContainerPanel;
        private System.Windows.Forms.Panel chartContainerPanel;
        private System.Windows.Forms.Panel leftChartPanel;
        private System.Windows.Forms.Panel rightChartPanel;
        private System.Windows.Forms.Panel quickActionsPanel;
        private System.Windows.Forms.Label quickActionsTitle;
        private System.Windows.Forms.Panel recentActivitiesPanel;
        private System.Windows.Forms.Label recentActivitiesTitle;
        private System.Windows.Forms.Timer updateTimer;
    }
    #endregion
}