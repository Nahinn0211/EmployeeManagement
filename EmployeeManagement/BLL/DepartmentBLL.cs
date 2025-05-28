using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.BLL
{
    /// <summary>
    /// Business Logic Layer cho Department
    /// </summary>
    public class DepartmentBLL
    {
        private readonly DepartmentDAL _departmentDAL;
        private readonly EmployeeDAL _employeeDAL;

        public DepartmentBLL()
        {
            _departmentDAL = new DepartmentDAL();
            _employeeDAL = new EmployeeDAL();
        }

        /// <summary>
        /// Lấy tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách DepartmentDTO</returns>
        public List<DepartmentDTO> GetAllDepartments()
        {
            try
            {
                return _departmentDAL.GetAllDepartments();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin của một phòng ban theo ID
        /// </summary>
        /// <param name="departmentId">ID của phòng ban</param>
        /// <returns>DepartmentDTO</returns>
        public DepartmentDTO GetDepartmentById(int departmentId)
        {
            try
            {
                return _departmentDAL.GetDepartmentById(departmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm phòng ban mới
        /// </summary>
        /// <param name="departmentDTO">Đối tượng DepartmentDTO</param>
        /// <returns>ID của phòng ban mới được thêm</returns>
        public int AddDepartment(DepartmentDTO departmentDTO)
        {
            try
            {
                // Validation
                ValidateDepartment(departmentDTO);

                // Thêm vào database
                return _departmentDAL.AddDepartment(departmentDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin phòng ban
        /// </summary>
        /// <param name="departmentDTO">Đối tượng DepartmentDTO</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateDepartment(DepartmentDTO departmentDTO)
        {
            try
            {
                // Validation
                ValidateDepartment(departmentDTO);

                // Cập nhật trong database
                return _departmentDAL.UpdateDepartment(departmentDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa phòng ban
        /// </summary>
        /// <param name="departmentId">ID của phòng ban cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeleteDepartment(int departmentId)
        {
            try
            {
                // Kiểm tra xem phòng ban có thể xóa được không
                ValidateDepartmentDeletion(departmentId);

                return _departmentDAL.DeleteDepartment(departmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm phòng ban theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Văn bản tìm kiếm</param>
        /// <param name="hasManager">Lọc theo trạng thái quản lý</param>
        /// <returns>Danh sách DepartmentDTO</returns>
        public List<DepartmentDTO> SearchDepartments(string searchText, bool? hasManager = null)
        {
            try
            {
                return _departmentDAL.SearchDepartments(searchText, hasManager);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên có thể được phân làm quản lý
        /// </summary>
        /// <returns>Danh sách employee</returns>
        public List<Employee> GetPotentialManagers()
        {
            try
            {
                return _departmentDAL.GetPotentialManagers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách quản lý tiềm năng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong một phòng ban
        /// </summary>
        /// <param name="departmentId">ID của phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        public List<EmployeeDTO> GetEmployeesInDepartment(int departmentId)
        {
            try
            {
                return _employeeDAL.GetEmployeesByDepartment(departmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên trong phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê về phòng ban
        /// </summary>
        /// <returns>Thống kê phòng ban</returns>
        public DepartmentStatisticsDTO GetDepartmentStatistics()
        {
            try
            {
                var departments = _departmentDAL.GetAllDepartments();
                var employees = _employeeDAL.GetAllEmployees();

                return new DepartmentStatisticsDTO
                {
                    TotalDepartments = departments.Count,
                    DepartmentsWithManager = departments.Count(d => d.ManagerID.HasValue),
                    DepartmentsWithoutManager = departments.Count(d => !d.ManagerID.HasValue),
                    TotalEmployees = employees.Count,
                    DepartmentWithMostEmployees = departments.OrderByDescending(d => d.EmployeeCount).FirstOrDefault()?.DepartmentName ?? "Không có",
                    MaxEmployeesCount = departments.Max(d => d.EmployeeCount),
                    AverageEmployeesPerDepartment = departments.Count > 0 ?
                        (double)employees.Count / departments.Count : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Chuyển nhân viên sang phòng ban khác
        /// </summary>
        /// <param name="employeeId">ID nhân viên</param>
        /// <param name="newDepartmentId">ID phòng ban mới</param>
        /// <returns>true nếu thành công</returns>
        public bool TransferEmployee(int employeeId, int newDepartmentId)
        {
            try
            {
                // Kiểm tra nhân viên có tồn tại không
                var employee = _employeeDAL.GetEmployeeById(employeeId);
                if (employee == null)
                    throw new ArgumentException($"Không tìm thấy nhân viên với ID {employeeId}");

                // Kiểm tra phòng ban có tồn tại không
                var department = _departmentDAL.GetDepartmentById(newDepartmentId);
                if (department == null)
                    throw new ArgumentException($"Không tìm thấy phòng ban với ID {newDepartmentId}");

                // Thực hiện chuyển phòng ban
                return _employeeDAL.UpdateEmployeeDepartment(employeeId, newDepartmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi chuyển nhân viên sang phòng ban khác: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Chỉ định quản lý cho phòng ban
        /// </summary>
        /// <param name="departmentId">ID phòng ban</param>
        /// <param name="managerId">ID nhân viên quản lý</param>
        /// <returns>true nếu thành công</returns>
        public bool AssignManager(int departmentId, int? managerId)
        {
            try
            {
                // Kiểm tra phòng ban có tồn tại không
                var department = _departmentDAL.GetDepartmentById(departmentId);
                if (department == null)
                    throw new ArgumentException($"Không tìm thấy phòng ban với ID {departmentId}");

                // Nếu gỡ bỏ quản lý (managerId = null)
                if (!managerId.HasValue)
                {
                    department.ManagerID = null;
                    return _departmentDAL.UpdateDepartment(department);
                }

                // Kiểm tra nhân viên có tồn tại không
                var manager = _employeeDAL.GetEmployeeById(managerId.Value);
                if (manager == null)
                    throw new ArgumentException($"Không tìm thấy nhân viên với ID {managerId}");

                // Kiểm tra nhân viên có đang ở trạng thái "Đang làm việc" không
                if (manager.Status != "Đang làm việc")
                    throw new ArgumentException("Chỉ nhân viên đang làm việc mới có thể được chỉ định làm quản lý");

                // Cập nhật quản lý cho phòng ban
                department.ManagerID = managerId;
                return _departmentDAL.UpdateDepartment(department);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi chỉ định quản lý cho phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu phòng ban
        /// </summary>
        /// <param name="departmentDTO">Đối tượng DepartmentDTO cần kiểm tra</param>
        private void ValidateDepartment(DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null)
                throw new ArgumentNullException(nameof(departmentDTO), "Dữ liệu phòng ban không được phép null");

            if (string.IsNullOrWhiteSpace(departmentDTO.DepartmentName))
                throw new ArgumentException("Tên phòng ban không được để trống", nameof(departmentDTO.DepartmentName));

            if (departmentDTO.DepartmentName.Length > 100)
                throw new ArgumentException("Tên phòng ban không được vượt quá 100 ký tự", nameof(departmentDTO.DepartmentName));

            if (departmentDTO.Description?.Length > 255)
                throw new ArgumentException("Mô tả phòng ban không được vượt quá 255 ký tự", nameof(departmentDTO.Description));

            // Kiểm tra manager nếu có
            if (departmentDTO.ManagerID.HasValue)
            {
                var manager = _employeeDAL.GetEmployeeById(departmentDTO.ManagerID.Value);
                if (manager == null)
                    throw new ArgumentException($"Không tìm thấy nhân viên quản lý với ID {departmentDTO.ManagerID.Value}", nameof(departmentDTO.ManagerID));

                if (manager.Status != "Đang làm việc")
                    throw new ArgumentException("Chỉ nhân viên đang làm việc mới có thể được chỉ định làm quản lý", nameof(departmentDTO.ManagerID));
            }

            // Kiểm tra trùng tên khi thêm mới
            if (departmentDTO.DepartmentID == 0)
            {
                var departments = _departmentDAL.GetAllDepartments();
                if (departments.Exists(d => d.DepartmentName.Equals(departmentDTO.DepartmentName, StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Tên phòng ban đã tồn tại", nameof(departmentDTO.DepartmentName));
            }
            // Kiểm tra trùng tên khi cập nhật
            else
            {
                var departments = _departmentDAL.GetAllDepartments();
                if (departments.Exists(d => d.DepartmentName.Equals(departmentDTO.DepartmentName, StringComparison.OrdinalIgnoreCase)
                                      && d.DepartmentID != departmentDTO.DepartmentID))
                    throw new ArgumentException("Tên phòng ban đã tồn tại", nameof(departmentDTO.DepartmentName));
            }
        }

        /// <summary>
        /// Kiểm tra xem phòng ban có thể xóa được không
        /// </summary>
        /// <param name="departmentId">ID của phòng ban cần xóa</param>
        private void ValidateDepartmentDeletion(int departmentId)
        {
            // Kiểm tra phòng ban có tồn tại không
            var department = _departmentDAL.GetDepartmentById(departmentId);
            if (department == null)
                throw new ArgumentException($"Không tìm thấy phòng ban với ID {departmentId}");

            // Kiểm tra có nhân viên trong phòng ban không
            if (department.EmployeeCount > 0)
                throw new InvalidOperationException($"Không thể xóa phòng ban vì còn {department.EmployeeCount} nhân viên thuộc phòng ban này. Vui lòng chuyển tất cả nhân viên sang phòng ban khác trước khi xóa.");

            // Kiểm tra có phòng ban khác sử dụng quản lý từ phòng ban này không
            if (department.ManagerID.HasValue)
            {
                var departments = _departmentDAL.GetAllDepartments();
                var relatedDepartments = departments.Count(d => d.ManagerID == department.ManagerID && d.DepartmentID != departmentId);

                if (relatedDepartments > 0)
                    throw new InvalidOperationException($"Không thể xóa phòng ban vì có {relatedDepartments} phòng ban khác đang sử dụng quản lý từ phòng ban này. Vui lòng thay đổi quản lý cho các phòng ban đó trước khi xóa.");
            }
        }
    }

   
}