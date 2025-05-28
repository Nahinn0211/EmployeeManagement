using EmployeeManagement.DAL;
using EmployeeManagement.Models.Entity;
using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// DTO cho thống kê nhân viên
    /// </summary>
    public class EmployeeStatisticsDTO
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TempLeaveEmployees { get; set; }
        public int ResignedEmployees { get; set; }
        public List<DepartmentEmployeeStats> DepartmentStats { get; set; } = new List<DepartmentEmployeeStats>();
        public string LargestDepartmentName { get; set; }
        public int LargestDepartmentCount { get; set; }
        public string SmallestDepartmentName { get; set; }
        public int SmallestDepartmentCount { get; set; }
        public int DepartmentsWithoutManager { get; set; }
        public double AverageEmployeesPerDepartment { get; set; }
    }


    /// <summary>
    /// Báo cáo biến động nhân sự
    /// </summary>
    public class EmployeeFluctuationReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartEmployeeCount { get; set; }
        public int EndEmployeeCount { get; set; }
        public int NewEmployees { get; set; }
        public int ResignedEmployees { get; set; }
        public int TempLeaveEmployees { get; set; }
        public int ReturnedEmployees { get; set; }
        public double FluctuationRate { get; set; } // Tỷ lệ biến động (%)
    }

    /// <summary>
    /// DTO cho thông tin kỷ niệm ngày làm việc
    /// </summary>
    public class EmployeeAnniversaryDTO
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime AnniversaryDate { get; set; }
        public int DaysRemaining { get; set; }
        public int YearsOfService { get; set; }
    }

    /// <summary>
    /// Data Transfer Object cho Employee dùng trong hiển thị và truyền dữ liệu
    /// </summary>
    public class EmployeeDTO
    {
        // Thông tin cơ bản
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string IDCardNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thông tin công việc
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? PositionID { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public int? ManagerID { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Đang làm việc";

        // Thông tin khác
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string InsuranceCode { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Thông tin thời gian
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Thông tin bổ sung
        public List<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
        public string FaceDataPath { get; set; } = string.Empty;

        /// <summary>
        /// Tạo EmployeeDTO từ Employee model
        /// </summary>
        /// <param name="employee">Employee model</param>
        /// <returns>EmployeeDTO</returns>
        public static EmployeeDTO FromEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            return new EmployeeDTO
            {
                EmployeeID = employee.EmployeeID,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                IDCardNumber = employee.IDCardNumber,
                Address = employee.Address,
                Phone = employee.Phone,
                Email = employee.Email,

                DepartmentID = employee.DepartmentID,
                DepartmentName = employee.Department?.DepartmentName ?? string.Empty,
                PositionID = employee.PositionID,
                PositionName = employee.Position?.PositionName ?? string.Empty,
                ManagerID = employee.ManagerID,
                ManagerName = employee.Manager?.FullName ?? string.Empty,

                HireDate = employee.HireDate,
                EndDate = employee.EndDate,
                Status = employee.Status,

                BankAccount = employee.BankAccount,
                BankName = employee.BankName,
                TaxCode = employee.TaxCode,
                InsuranceCode = employee.InsuranceCode,
                Notes = employee.Notes,
                FaceDataPath = employee.FaceDataPath,

                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo danh sách EmployeeDTO từ danh sách Employee
        /// </summary>
        /// <param name="employees">Danh sách Employee</param>
        /// <returns>Danh sách EmployeeDTO</returns>
        public static List<EmployeeDTO> FromEmployeeList(List<Employee> employees)
        {
            if (employees == null)
                throw new ArgumentNullException(nameof(employees));

            List<EmployeeDTO> dtoList = new List<EmployeeDTO>();

            foreach (var employee in employees)
            {
                dtoList.Add(FromEmployee(employee));
            }

            return dtoList;
        }

        /// <summary>
        /// Tạo Employee model từ EmployeeDTO
        /// </summary>
        /// <returns>Employee model</returns>
        public Employee ToEmployee()
        {
            return new Employee
            {
                EmployeeID = this.EmployeeID,
                EmployeeCode = this.EmployeeCode,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Gender = this.Gender,
                DateOfBirth = this.DateOfBirth,
                IDCardNumber = this.IDCardNumber,
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email,

                DepartmentID = this.DepartmentID ?? 0, 
                PositionID = this.PositionID ?? 0,

                ManagerID = this.ManagerID,

                HireDate = this.HireDate,
                EndDate = this.EndDate,
                Status = this.Status,

                BankAccount = this.BankAccount,
                BankName = this.BankName,
                TaxCode = this.TaxCode,
                InsuranceCode = this.InsuranceCode,
                Notes = this.Notes,
                FaceDataPath = this.FaceDataPath,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Data Transfer Object cho hiển thị nhân viên trên DataGridView
    /// </summary>
    public class EmployeeDisplayModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Tạo EmployeeDisplayModel từ EmployeeDTO
        /// </summary>
        /// <param name="dto">EmployeeDTO</param>
        /// <returns>EmployeeDisplayModel</returns>
        public static EmployeeDisplayModel FromDTO(EmployeeDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new EmployeeDisplayModel
            {
                EmployeeID = dto.EmployeeID,
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Phone = dto.Phone,
                Email = dto.Email,
                Department = dto.DepartmentName,
                Position = dto.PositionName,
                Status = dto.Status,
                HireDate = dto.HireDate,
                EndDate = dto.EndDate
            };
        }

        /// <summary>
        /// Tạo danh sách EmployeeDisplayModel từ danh sách EmployeeDTO
        /// </summary>
        /// <param name="dtoList">Danh sách EmployeeDTO</param>
        /// <returns>Danh sách EmployeeDisplayModel</returns>
        public static List<EmployeeDisplayModel> FromDTOList(List<EmployeeDTO> dtoList)
        {
            if (dtoList == null)
                throw new ArgumentNullException(nameof(dtoList));

            List<EmployeeDisplayModel> displayList = new List<EmployeeDisplayModel>();

            foreach (var dto in dtoList)
            {
                displayList.Add(FromDTO(dto));
            }

            return displayList;
        }
    }

    /// <summary>
    /// Data Transfer Object cho thông tin chi tiết nhân viên
    /// </summary>
    public class EmployeeDetailDTO : EmployeeDTO
    {
        // Thông tin bổ sung
        public List<ProjectDTO> Projects { get; set; } = new List<ProjectDTO>();
        public List<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();
        public List<AttendanceDTO> RecentAttendance { get; set; } = new List<AttendanceDTO>();
        public List<SalaryDTO> RecentSalary { get; set; } = new List<SalaryDTO>();

        /// <summary>
        /// Chuyển đổi từ EmployeeDTO sang EmployeeDetailDTO
        /// </summary>
        /// <param name="dto">EmployeeDTO</param>
        /// <returns>EmployeeDetailDTO</returns>
        public static EmployeeDetailDTO FromEmployeeDTO(EmployeeDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new EmployeeDetailDTO
            {
                EmployeeID = dto.EmployeeID,
                EmployeeCode = dto.EmployeeCode,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = dto.FullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                IDCardNumber = dto.IDCardNumber,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,

                DepartmentID = dto.DepartmentID,
                DepartmentName = dto.DepartmentName,
                PositionID = dto.PositionID,
                PositionName = dto.PositionName,
                ManagerID = dto.ManagerID,
                ManagerName = dto.ManagerName,

                HireDate = dto.HireDate,
                EndDate = dto.EndDate,
                Status = dto.Status,

                BankAccount = dto.BankAccount,
                BankName = dto.BankName,
                TaxCode = dto.TaxCode,
                InsuranceCode = dto.InsuranceCode,
                Notes = dto.Notes,
                FaceDataPath = dto.FaceDataPath,

                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,

                ManagerRoles = dto.ManagerRoles
            };
        }
    }

    /// <summary>
    /// DTO tối thiểu cho dropdown chọn nhân viên
    /// </summary>
    public class EmployeeMinimalDTO
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public override string ToString()
        {
            return $"{EmployeeCode} - {FullName}";
        }
    }

    /// <summary>
    /// Placeholder cho các DTO khác cần thiết
    /// </summary>
    public class ProjectDTO { }
    public class TaskDTO { }
    public class AttendanceDTO { }
    public class SalaryDTO { }
}