using System;

namespace EmployeeManagement.Models
{
    public class WorkTask // Đổi tên từ Task thành WorkTask
    {
        public int TaskID { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int ProjectID { get; set; }
        public int? AssignedToID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee AssignedTo { get; set; }
    }

    // Class hiển thị Task trong DataGridView
    public class TaskDisplayModel
    {
        public int TaskID { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string ProjectName { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public decimal CompletionPercentage { get; set; }
    }
}