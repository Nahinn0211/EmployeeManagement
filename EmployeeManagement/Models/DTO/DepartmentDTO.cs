using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.DTO
{

    /// <summary>
    /// Thống kê nhân viên theo phòng ban
    /// </summary>
    public class DepartmentEmployeeStats
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
        public int ActiveEmployeeCount { get; set; }
        public bool HasManager { get; set; }
    }

    /// <summary>
    /// Data Transfer Object cho Department dùng trong hiển thị và truyền dữ liệu
    /// </summary>
    public class DepartmentDTO
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Thông tin quản lý
        public int? ManagerID { get; set; }
        public string ManagerName { get; set; } = "Chưa phân công";

        // Thông tin liên quan
        public int EmployeeCount { get; set; }

        // Thông tin thời gian
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Tạo DepartmentDTO từ Department model
        /// </summary>
        /// <param name="department">Department model</param>
        /// <param name="employeeCount">Số lượng nhân viên (optional)</param>
        /// <returns>DepartmentDTO</returns>
        public static DepartmentDTO FromDepartment(Department department, int? employeeCount = null)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            return new DepartmentDTO
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                Description = department.Description ?? string.Empty,
                ManagerID = department.ManagerID,
                ManagerName = department.Manager?.FullName ?? "Chưa phân công",
                EmployeeCount = employeeCount ?? department.Employees?.Count ?? 0,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo danh sách DepartmentDTO từ danh sách Department
        /// </summary>
        /// <param name="departments">Danh sách Department</param>
        /// <returns>Danh sách DepartmentDTO</returns>
        public static List<DepartmentDTO> FromDepartmentList(List<Department> departments)
        {
            if (departments == null)
                throw new ArgumentNullException(nameof(departments));

            List<DepartmentDTO> dtoList = new List<DepartmentDTO>();

            foreach (var department in departments)
            {
                dtoList.Add(FromDepartment(department));
            }

            return dtoList;
        }

        /// <summary>
        /// Tạo Department model từ DepartmentDTO
        /// </summary>
        /// <returns>Department model</returns>
        public Department ToDepartment()
        {
            return new Department
            {
                DepartmentID = this.DepartmentID,
                DepartmentName = this.DepartmentName,
                Description = string.IsNullOrEmpty(this.Description) ? null : this.Description,
                ManagerID = this.ManagerID,
                CreatedAt = DateTime.Now,
                UpdatedAt =  DateTime.Now,

            };
        }
    }

    /// <summary>
    /// Data Transfer Object cho hiển thị trên DataGridView
    /// </summary>
    public class DepartmentDisplayModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Tạo DepartmentDisplayModel từ DepartmentDTO
        /// </summary>
        /// <param name="dto">DepartmentDTO</param>
        /// <returns>DepartmentDisplayModel</returns>
        public static DepartmentDisplayModel FromDTO(DepartmentDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new DepartmentDisplayModel
            {
                DepartmentID = dto.DepartmentID,
                DepartmentName = dto.DepartmentName,
                Description = dto.Description,
                ManagerName = dto.ManagerName,
                EmployeeCount = dto.EmployeeCount,
                CreatedAt = dto.CreatedAt,
                LastUpdated = dto.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo danh sách DepartmentDisplayModel từ danh sách DepartmentDTO
        /// </summary>
        /// <param name="dtoList">Danh sách DepartmentDTO</param>
        /// <returns>Danh sách DepartmentDisplayModel</returns>
        public static List<DepartmentDisplayModel> FromDTOList(List<DepartmentDTO> dtoList)
        {
            if (dtoList == null)
                throw new ArgumentNullException(nameof(dtoList));

            List<DepartmentDisplayModel> displayList = new List<DepartmentDisplayModel>();

            foreach (var dto in dtoList)
            {
                displayList.Add(FromDTO(dto));
            }

            return displayList;
        }
    }
    /// <summary>
    /// DTO cho thống kê phòng ban
    /// </summary>
    public class DepartmentStatisticsDTO
    {
        public int TotalDepartments { get; set; }
        public int DepartmentsWithManager { get; set; }
        public int DepartmentsWithoutManager { get; set; }
        public int TotalEmployees { get; set; }
        public string DepartmentWithMostEmployees { get; set; }
        public int MaxEmployeesCount { get; set; }
        public double AverageEmployeesPerDepartment { get; set; }
    }

}