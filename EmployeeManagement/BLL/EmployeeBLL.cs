using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.BLL
{
    /// <summary>
    /// Business Logic Layer cho Employee
    /// </summary>
    public class EmployeeBLL
    {
        private readonly EmployeeDAL _employeeDAL;
        private readonly DepartmentDAL _departmentDAL;

        public EmployeeBLL()
        {
            _employeeDAL = new EmployeeDAL();
            _departmentDAL = new DepartmentDAL();
        }

        /// <summary>
        /// Lấy tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách EmployeeDTO</returns>
        public List<EmployeeDTO> GetAllEmployees()
        {
            try
            {
                return _employeeDAL.GetAllEmployees();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin của một nhân viên theo ID
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <returns>EmployeeDTO</returns>
        public EmployeeDTO GetEmployeeById(int employeeId)
        {
            try
            {
                return _employeeDAL.GetEmployeeById(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của nhân viên bao gồm các thông tin liên quan
        /// </summary>
        /// <param name="employeeId">ID của nhân viên</param>
        /// <returns>EmployeeDetailDTO</returns>
        public EmployeeDetailDTO GetEmployeeDetails(int employeeId)
        {
            try
            {
                return _employeeDAL.GetEmployeeDetails(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin chi tiết nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên thuộc một phòng ban
        /// </summary>
        /// <param name="departmentId">ID của phòng ban</param>
        /// <returns>Danh sách EmployeeDTO</returns>
        public List<EmployeeDTO> GetEmployeesByDepartment(int departmentId)
        {
            try
            {
                // Kiểm tra phòng ban có tồn tại không
                var department = _departmentDAL.GetDepartmentById(departmentId);
                if (department == null)
                    throw new ArgumentException($"Không tìm thấy phòng ban với ID {departmentId}");

                return _employeeDAL.GetEmployeesByDepartment(departmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên trong phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm nhân viên mới
        /// </summary>
        /// <param name="employeeDTO">Đối tượng EmployeeDTO</param>
        /// <returns>ID của nhân viên mới được thêm</returns>
        public int AddEmployee(EmployeeDTO employeeDTO)
        {
            try
            {
                // Validation
                ValidateEmployee(employeeDTO);

                // Tạo mã nhân viên nếu chưa có
                if (string.IsNullOrEmpty(employeeDTO.EmployeeCode))
                {
                    employeeDTO.EmployeeCode = GenerateEmployeeCode();
                }

                // Thêm vào database
                return _employeeDAL.AddEmployee(employeeDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="employeeDTO">Đối tượng EmployeeDTO</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdateEmployee(EmployeeDTO employeeDTO)
        {
            try
            {
                // Validation
                ValidateEmployee(employeeDTO, true);

                // Cập nhật trong database
                return _employeeDAL.UpdateEmployee(employeeDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="employeeId">ID của nhân viên cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeleteEmployee(int employeeId)
        {
            try
            {
                // Kiểm tra xem nhân viên có tồn tại không
                var employee = _employeeDAL.GetEmployeeById(employeeId);
                if (employee == null)
                    throw new ArgumentException($"Không tìm thấy nhân viên với ID {employeeId}");

                // Kiểm tra xem nhân viên có thể xóa được không (ràng buộc)
                // Xử lý trong DAL

                return _employeeDAL.DeleteEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
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

                // Nếu nhân viên đã thuộc phòng ban này
                if (employee.DepartmentID == newDepartmentId)
                    return true;

                // Thực hiện chuyển phòng ban
                return _employeeDAL.UpdateEmployeeDepartment(employeeId, newDepartmentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi chuyển nhân viên sang phòng ban khác: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái nhân viên
        /// </summary>
        /// <param name="employeeId">ID nhân viên</param>
        /// <param name="status">Trạng thái mới</param>
        /// <param name="endDate">Ngày kết thúc (nếu nghỉ việc)</param>
        /// <returns>true nếu thành công</returns>
        public bool UpdateEmployeeStatus(int employeeId, string status, DateTime? endDate = null)
        {
            try
            {
                // Kiểm tra nhân viên có tồn tại không
                var employee = _employeeDAL.GetEmployeeById(employeeId);
                if (employee == null)
                    throw new ArgumentException($"Không tìm thấy nhân viên với ID {employeeId}");

                // Kiểm tra trạng thái hợp lệ
                if (!IsValidStatus(status))
                    throw new ArgumentException($"Trạng thái '{status}' không hợp lệ");

                // Nếu trạng thái là "Đã nghỉ việc" thì phải có ngày kết thúc
                if (status == "Đã nghỉ việc" && !endDate.HasValue)
                    endDate = DateTime.Now;

                // Nếu nhân viên là quản lý của phòng ban hoặc dự án, không cho phép đổi trạng thái thành "Đã nghỉ việc"
                if (status == "Đã nghỉ việc")
                {
                    var managerRoles = _employeeDAL.GetManagerRoles(employeeId);
                    if (managerRoles.Count > 0)
                    {
                        throw new InvalidOperationException(
                                                    "Không thể đổi trạng thái nhân viên này thành \"Đã nghỉ việc\" vì họ đang là quản lý của: " +
                                                    string.Join(", ", managerRoles.Select(r => $"{r.EntityName} ({r.RoleType})")));
                    }
                }

                // Thực hiện cập nhật trạng thái
                return _employeeDAL.UpdateEmployeeStatus(employeeId, status, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm nhân viên theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Từ khóa tìm kiếm</param>
        /// <param name="departmentId">ID phòng ban (nếu có)</param>
        /// <param name="status">Trạng thái nhân viên (nếu có)</param>
        /// <returns>Danh sách nhân viên thỏa mãn điều kiện</returns>
        public List<EmployeeDTO> SearchEmployees(string searchText, int? departmentId = null, string status = null)
        {
            try
            {
                // Nếu status được chỉ định, kiểm tra tính hợp lệ
                if (!string.IsNullOrEmpty(status) && !IsValidStatus(status))
                    throw new ArgumentException($"Trạng thái '{status}' không hợp lệ");

                // Nếu departmentId được chỉ định, kiểm tra phòng ban có tồn tại không
                if (departmentId.HasValue)
                {
                    var department = _departmentDAL.GetDepartmentById(departmentId.Value);
                    if (department == null)
                        throw new ArgumentException($"Không tìm thấy phòng ban với ID {departmentId}");
                }

                return _employeeDAL.SearchEmployees(searchText, departmentId, status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm nhân viên có thể làm quản lý
        /// </summary>
        /// <param name="searchText">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhân viên</returns>
        public List<EmployeeDTO> SearchPotentialManagers(string searchText)
        {
            try
            {
                return _employeeDAL.SearchPotentialManagers(searchText);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm quản lý tiềm năng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê nhân viên theo phòng ban
        /// </summary>
        /// <returns>Thống kê nhân viên</returns>
        public EmployeeStatisticsDTO GetEmployeeStatistics()
        {
            try
            {
                var employees = _employeeDAL.GetAllEmployees();
                var departments = _departmentDAL.GetAllDepartments();

                // Số lượng nhân viên theo trạng thái
                int totalEmployees = employees.Count;
                int activeEmployees = employees.Count(e => e.Status == "Đang làm việc");
                int tempLeaveEmployees = employees.Count(e => e.Status == "Tạm nghỉ");
                int resignedEmployees = employees.Count(e => e.Status == "Đã nghỉ việc");

                // Phân bố nhân viên theo phòng ban
                var departmentStats = departments.Select(d => new DepartmentEmployeeStats
                {
                    DepartmentID = d.DepartmentID,
                    DepartmentName = d.DepartmentName,
                    EmployeeCount = employees.Count(e => e.DepartmentID == d.DepartmentID),
                    ActiveEmployeeCount = employees.Count(e => e.DepartmentID == d.DepartmentID && e.Status == "Đang làm việc"),
                    HasManager = d.ManagerID.HasValue
                }).ToList();

                // Phòng ban có nhiều nhân viên nhất
                var largestDepartment = departmentStats.OrderByDescending(d => d.EmployeeCount).FirstOrDefault();

                // Phòng ban có ít nhân viên nhất
                var smallestDepartment = departmentStats.Where(d => d.EmployeeCount > 0).OrderBy(d => d.EmployeeCount).FirstOrDefault();

                // Phòng ban chưa có quản lý
                int departmentsWithoutManager = departmentStats.Count(d => !d.HasManager);

                return new EmployeeStatisticsDTO
                {
                    TotalEmployees = totalEmployees,
                    ActiveEmployees = activeEmployees,
                    TempLeaveEmployees = tempLeaveEmployees,
                    ResignedEmployees = resignedEmployees,
                    DepartmentStats = departmentStats,
                    LargestDepartmentName = largestDepartment?.DepartmentName ?? "Không có",
                    LargestDepartmentCount = largestDepartment?.EmployeeCount ?? 0,
                    SmallestDepartmentName = smallestDepartment?.DepartmentName ?? "Không có",
                    SmallestDepartmentCount = smallestDepartment?.EmployeeCount ?? 0,
                    DepartmentsWithoutManager = departmentsWithoutManager,
                    AverageEmployeesPerDepartment = departments.Count > 0 ? (double)totalEmployees / departments.Count : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy báo cáo biến động nhân sự trong khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Báo cáo biến động nhân sự</returns>
        public EmployeeFluctuationReport GetEmployeeFluctuationReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var employees = _employeeDAL.GetAllEmployees();

                // Số nhân viên mới
                int newEmployees = employees.Count(e => e.HireDate >= startDate && e.HireDate <= endDate);

                // Số nhân viên nghỉ việc
                int resignedEmployees = employees.Count(e =>
                    e.Status == "Đã nghỉ việc" &&
                    e.EndDate.HasValue &&
                    e.EndDate.Value >= startDate &&
                    e.EndDate.Value <= endDate);

                // Số nhân viên tạm nghỉ
                int tempLeaveEmployees = employees.Count(e =>
                    e.Status == "Tạm nghỉ" &&
                    e.UpdatedAt.HasValue &&
                    e.UpdatedAt.Value >= startDate &&
                    e.UpdatedAt.Value <= endDate);

                // Số nhân viên quay lại làm việc
                int returnedEmployees = employees.Count(e =>
                    e.Status == "Đang làm việc" &&
                    e.UpdatedAt.HasValue &&
                    e.UpdatedAt.Value >= startDate &&
                    e.UpdatedAt.Value <= endDate &&
                    e.HireDate < startDate); // Loại bỏ nhân viên mới

                // Tính tỷ lệ biến động
                int startCount = employees.Count(e => e.HireDate < startDate && (e.EndDate == null || e.EndDate > startDate));
                int endCount = startCount + newEmployees - resignedEmployees;
                double fluctuationRate = startCount > 0 ? (double)(resignedEmployees) / startCount * 100 : 0;

                return new EmployeeFluctuationReport
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    StartEmployeeCount = startCount,
                    EndEmployeeCount = endCount,
                    NewEmployees = newEmployees,
                    ResignedEmployees = resignedEmployees,
                    TempLeaveEmployees = tempLeaveEmployees,
                    ReturnedEmployees = returnedEmployees,
                    FluctuationRate = fluctuationRate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo biến động nhân sự: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra danh sách nhân viên sắp đến hạn kỷ niệm ngày làm việc
        /// </summary>
        /// <param name="daysThreshold">Số ngày trước khi đến hạn</param>
        /// <returns>Danh sách nhân viên sắp đến hạn kỷ niệm</returns>
        public List<EmployeeAnniversaryDTO> GetUpcomingWorkAnniversaries(int daysThreshold = 30)
        {
            try
            {
                var employees = _employeeDAL.GetAllEmployees();
                var today = DateTime.Now.Date;
                var result = new List<EmployeeAnniversaryDTO>();

                foreach (var employee in employees.Where(e => e.Status == "Đang làm việc"))
                {
                    // Tính ngày kỷ niệm năm nay
                    var hireDate = employee.HireDate;
                    var anniversaryThisYear = new DateTime(today.Year, hireDate.Month, hireDate.Day);

                    // Nếu ngày kỷ niệm năm nay đã qua, tính cho năm sau
                    if (anniversaryThisYear < today)
                    {
                        anniversaryThisYear = new DateTime(today.Year + 1, hireDate.Month, hireDate.Day);
                    }

                    // Tính số ngày còn lại và số năm làm việc
                    int daysRemaining = (anniversaryThisYear - today).Days;
                    int yearsOfService = anniversaryThisYear.Year - hireDate.Year;

                    // Nếu trong ngưỡng cảnh báo
                    if (daysRemaining <= daysThreshold)
                    {
                        result.Add(new EmployeeAnniversaryDTO
                        {
                            EmployeeID = employee.EmployeeID,
                            EmployeeCode = employee.EmployeeCode,
                            FullName = employee.FullName,
                            DepartmentName = employee.DepartmentName,
                            HireDate = employee.HireDate,
                            AnniversaryDate = anniversaryThisYear,
                            DaysRemaining = daysRemaining,
                            YearsOfService = yearsOfService
                        });
                    }
                }

                return result.OrderBy(a => a.DaysRemaining).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra ngày kỷ niệm làm việc: {ex.Message}", ex);
            }
        }

        #region Helper Methods
        /// <summary>
        /// Tạo mã nhân viên mới tự động
        /// </summary>
        /// <returns>Mã nhân viên</returns>
        private string GenerateEmployeeCode()
        {
            var employees = _employeeDAL.GetAllEmployees();

            string prefix = "NV";
            int maxNumber = 0;

            foreach (var employee in employees)
            {
                if (employee.EmployeeCode.StartsWith(prefix) && employee.EmployeeCode.Length > prefix.Length)
                {
                    string numberPart = employee.EmployeeCode.Substring(prefix.Length);
                    if (int.TryParse(numberPart, out int number) && number > maxNumber)
                    {
                        maxNumber = number;
                    }
                }
            }

            return $"{prefix}{(maxNumber + 1).ToString("D4")}";
        }

        /// <summary>
        /// Kiểm tra trạng thái nhân viên có hợp lệ không
        /// </summary>
        /// <param name="status">Trạng thái cần kiểm tra</param>
        /// <returns>true nếu hợp lệ</returns>
        private bool IsValidStatus(string status)
        {
            string[] validStatuses = { "Đang làm việc", "Tạm nghỉ", "Đã nghỉ việc" };
            return validStatuses.Contains(status);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu nhân viên
        /// </summary>
        /// <param name="employeeDTO">Đối tượng EmployeeDTO cần kiểm tra</param>
        /// <param name="isUpdate">true nếu là cập nhật, false nếu là thêm mới</param>
        private void ValidateEmployee(EmployeeDTO employeeDTO, bool isUpdate = false)
        {
            if (employeeDTO == null)
                throw new ArgumentNullException(nameof(employeeDTO), "Dữ liệu nhân viên không được phép null");

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(employeeDTO.FirstName))
                throw new ArgumentException("Họ không được để trống", nameof(employeeDTO.FirstName));

            if (string.IsNullOrWhiteSpace(employeeDTO.LastName))
                throw new ArgumentException("Tên không được để trống", nameof(employeeDTO.LastName));

            if (string.IsNullOrWhiteSpace(employeeDTO.IDCardNumber))
                throw new ArgumentException("Số CMND/CCCD không được để trống", nameof(employeeDTO.IDCardNumber));

            if (string.IsNullOrWhiteSpace(employeeDTO.Phone))
                throw new ArgumentException("Số điện thoại không được để trống", nameof(employeeDTO.Phone));

            if (string.IsNullOrWhiteSpace(employeeDTO.Email))
                throw new ArgumentException("Email không được để trống", nameof(employeeDTO.Email));

            if (!IsValidStatus(employeeDTO.Status))
                throw new ArgumentException($"Trạng thái '{employeeDTO.Status}' không hợp lệ", nameof(employeeDTO.Status));

            // Kiểm tra ngày sinh
            if (employeeDTO.DateOfBirth > DateTime.Now.AddYears(-18))
                throw new ArgumentException("Nhân viên phải đủ 18 tuổi", nameof(employeeDTO.DateOfBirth));

            if (employeeDTO.DateOfBirth < DateTime.Now.AddYears(-70))
                throw new ArgumentException("Ngày sinh không hợp lệ", nameof(employeeDTO.DateOfBirth));

            // Kiểm tra email
            if (!employeeDTO.Email.Contains("@") || !employeeDTO.Email.Contains("."))
                throw new ArgumentException("Email không hợp lệ", nameof(employeeDTO.Email));

            // Kiểm tra số điện thoại
            if (employeeDTO.Phone.Length < 10 || employeeDTO.Phone.Length > 11)
                throw new ArgumentException("Số điện thoại phải có 10 hoặc 11 chữ số", nameof(employeeDTO.Phone));

            // Kiểm tra CMND/CCCD
            if (employeeDTO.IDCardNumber.Length != 9 && employeeDTO.IDCardNumber.Length != 12)
                throw new ArgumentException("Số CMND/CCCD phải có 9 hoặc 12 chữ số", nameof(employeeDTO.IDCardNumber));

            // Kiểm tra phòng ban và chức vụ (nếu có)
            if (employeeDTO.DepartmentID.HasValue)
            {
                var department = _departmentDAL.GetDepartmentById(employeeDTO.DepartmentID.Value);
                if (department == null)
                    throw new ArgumentException($"Không tìm thấy phòng ban với ID {employeeDTO.DepartmentID}", nameof(employeeDTO.DepartmentID));
            }

            // Kiểm tra quản lý (nếu có)
            if (employeeDTO.ManagerID.HasValue)
            {
                var manager = _employeeDAL.GetEmployeeById(employeeDTO.ManagerID.Value);
                if (manager == null)
                    throw new ArgumentException($"Không tìm thấy quản lý với ID {employeeDTO.ManagerID}", nameof(employeeDTO.ManagerID));

                if (manager.Status != "Đang làm việc")
                    throw new ArgumentException("Quản lý phải là nhân viên đang làm việc", nameof(employeeDTO.ManagerID));

                // Kiểm tra không cho phép tạo vòng lặp quản lý
                if (isUpdate && manager.ManagerID == employeeDTO.EmployeeID)
                    throw new ArgumentException("Không thể tạo vòng lặp quản lý (A quản lý B và B quản lý A)", nameof(employeeDTO.ManagerID));
            }

            // Kiểm tra trùng lặp khi thêm mới
            if (!isUpdate)
            {
                var employees = _employeeDAL.GetAllEmployees();

                // Kiểm tra trùng email
                if (employees.Any(e => e.Email.Equals(employeeDTO.Email, StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Email đã tồn tại", nameof(employeeDTO.Email));

                // Kiểm tra trùng số điện thoại
                if (employees.Any(e => e.Phone == employeeDTO.Phone))
                    throw new ArgumentException("Số điện thoại đã tồn tại", nameof(employeeDTO.Phone));

                // Kiểm tra trùng CMND/CCCD
                if (employees.Any(e => e.IDCardNumber == employeeDTO.IDCardNumber))
                    throw new ArgumentException("Số CMND/CCCD đã tồn tại", nameof(employeeDTO.IDCardNumber));
            }
            // Kiểm tra trùng lặp khi cập nhật
            else
            {
                var employees = _employeeDAL.GetAllEmployees();

                // Kiểm tra trùng email
                if (employees.Any(e => e.Email.Equals(employeeDTO.Email, StringComparison.OrdinalIgnoreCase) && e.EmployeeID != employeeDTO.EmployeeID))
                    throw new ArgumentException("Email đã tồn tại", nameof(employeeDTO.Email));

                // Kiểm tra trùng số điện thoại
                if (employees.Any(e => e.Phone == employeeDTO.Phone && e.EmployeeID != employeeDTO.EmployeeID))
                    throw new ArgumentException("Số điện thoại đã tồn tại", nameof(employeeDTO.Phone));

                // Kiểm tra trùng CMND/CCCD
                if (employees.Any(e => e.IDCardNumber == employeeDTO.IDCardNumber && e.EmployeeID != employeeDTO.EmployeeID))
                    throw new ArgumentException("Số CMND/CCCD đã tồn tại", nameof(employeeDTO.IDCardNumber));
            }
        }
        /// <summary>
        /// Lấy danh sách tất cả phòng ban từ database
        /// </summary>
        public List<Department> GetAllDepartments()
        {
            try
            {
                return    _employeeDAL.GetAllDepartments();
            }
            catch (Exception ex)
            {
                 throw new Exception($"Không thể lấy danh sách phòng ban: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả chức vụ từ database
        /// </summary>
        public List<Position> GetAllPositions()
        {
            try
            {
                 return _employeeDAL.GetAllPositions();
            }
            catch (Exception ex)
            {
                 throw new Exception($"Không thể lấy danh sách chức vụ: {ex.Message}");
            }
        }
        #endregion
    }

   

}