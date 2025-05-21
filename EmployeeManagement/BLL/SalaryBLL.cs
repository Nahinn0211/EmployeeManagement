using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.BLL
{
    public class SalaryBLL
    {
        private SalaryDAL salaryDAL;

        public SalaryBLL()
        {
            salaryDAL = new SalaryDAL();
        }

        #region CRUD Operations
        public List<Salary> GetAllSalaries()
        {
            try
            {
                return salaryDAL.GetAllSalaries();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách lương: {ex.Message}", ex);
            }
        }

        public Salary GetSalaryById(int salaryId)
        {
            try
            {
                if (salaryId <= 0)
                    throw new ArgumentException("ID lương không hợp lệ");

                return salaryDAL.GetSalaryById(salaryId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin lương: {ex.Message}", ex);
            }
        }

        public int AddSalary(Salary salary)
        {
            try
            {
                var validation = ValidateSalary(salary);
                if (!validation.IsValid)
                    throw new ArgumentException(validation.ErrorMessage);

                // Check for duplicate
                if (salaryDAL.IsSalaryExists(salary.EmployeeID, salary.Month, salary.Year))
                    throw new ArgumentException($"Đã tồn tại bảng lương cho nhân viên này trong tháng {salary.Month}/{salary.Year}");

                // Calculate net salary
                salary.CalculateNetSalary();
                salary.CreatedAt = DateTime.Now;

                return salaryDAL.AddSalary(salary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm bảng lương: {ex.Message}", ex);
            }
        }

        public bool UpdateSalary(Salary salary)
        {
            try
            {
                var validation = ValidateSalary(salary);
                if (!validation.IsValid)
                    throw new ArgumentException(validation.ErrorMessage);

                // Check if salary can be edited
                var existingSalary = salaryDAL.GetSalaryById(salary.SalaryID);
                if (existingSalary == null)
                    throw new ArgumentException("Không tìm thấy bảng lương");

                if (!existingSalary.CanEdit())
                    throw new ArgumentException("Không thể chỉnh sửa bảng lương đã thanh toán");

                // Check for duplicate (excluding current record)
                if (salaryDAL.IsSalaryExists(salary.EmployeeID, salary.Month, salary.Year, salary.SalaryID))
                    throw new ArgumentException($"Đã tồn tại bảng lương cho nhân viên này trong tháng {salary.Month}/{salary.Year}");

                // Calculate net salary
                salary.CalculateNetSalary();

                return salaryDAL.UpdateSalary(salary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật bảng lương: {ex.Message}", ex);
            }
        }

        public bool DeleteSalary(int salaryId)
        {
            try
            {
                if (salaryId <= 0)
                    throw new ArgumentException("ID lương không hợp lệ");

                var canDelete = CanDeleteSalary(salaryId);
                if (!canDelete.CanDelete)
                    throw new ArgumentException(canDelete.Reason);

                return salaryDAL.DeleteSalary(salaryId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa bảng lương: {ex.Message}", ex);
            }
        }
        #endregion

        #region Search and Filter
        public List<Salary> SearchSalaries(string employeeName, string employeeCode, string departmentName,
            string paymentStatus, int? month, int? year, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return salaryDAL.SearchSalaries(employeeName, employeeCode, departmentName,
                    paymentStatus, month, year, fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm bảng lương: {ex.Message}", ex);
            }
        }

        public List<Salary> GetSalariesByEmployee(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("ID nhân viên không hợp lệ");

                return salaryDAL.GetSalariesByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch sử lương nhân viên: {ex.Message}", ex);
            }
        }

        public List<Salary> GetSalariesByMonthYear(int month, int year)
        {
            try
            {
                if (month < 1 || month > 12)
                    throw new ArgumentException("Tháng không hợp lệ");

                if (year < 2000 || year > DateTime.Now.Year + 1)
                    throw new ArgumentException("Năm không hợp lệ");

                return salaryDAL.GetSalariesByMonthYear(month, year);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy bảng lương theo tháng: {ex.Message}", ex);
            }
        }
        #endregion

        #region Display Data
        public List<SalaryDisplayModel> GetSalariesForDisplay()
        {
            try
            {
                var salaries = GetAllSalaries();
                return salaries.Select(s => new SalaryDisplayModel
                {
                    SalaryID = s.SalaryID,
                    EmployeeID = s.EmployeeID,
                    EmployeeCode = s.Employee?.EmployeeCode ?? "",
                    EmployeeName = s.Employee?.FullName ?? "",
                    DepartmentName = "", // Will be loaded from employee details if needed
                    PositionName = "", // Will be loaded from employee details if needed
                    Month = s.Month,
                    Year = s.Year,
                    BaseSalary = s.BaseSalary,
                    Allowance = s.Allowance,
                    Bonus = s.Bonus,
                    Deduction = s.Deduction,
                    NetSalary = s.NetSalary,
                    PaymentDate = s.PaymentDate,
                    PaymentStatus = s.PaymentStatus,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu hiển thị: {ex.Message}", ex);
            }
        }

        public List<EmployeeDropdownItem> GetEmployeesForDropdown()
        {
            try
            {
                return salaryDAL.GetEmployeesForDropdown();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }
        #endregion

        #region Validation
        public SalaryValidationResult ValidateSalary(Salary salary)
        {
            var result = new SalaryValidationResult();
            var errors = new List<string>();

            // Validate employee
            if (salary.EmployeeID <= 0)
                errors.Add("Vui lòng chọn nhân viên");

            // Validate month
            if (salary.Month < 1 || salary.Month > 12)
                errors.Add("Tháng phải từ 1 đến 12");

            // Validate year
            if (salary.Year < 2000 || salary.Year > DateTime.Now.Year + 1)
                errors.Add($"Năm phải từ 2000 đến {DateTime.Now.Year + 1}");

            // Validate salary amounts
            if (salary.BaseSalary < 0)
                errors.Add("Lương cơ bản không được âm");

            if (salary.Allowance < 0)
                errors.Add("Phụ cấp không được âm");

            if (salary.Bonus < 0)
                errors.Add("Thưởng không được âm");

            if (salary.Deduction < 0)
                errors.Add("Khấu trừ không được âm");

            // Validate payment status
            if (string.IsNullOrEmpty(salary.PaymentStatus) ||
                !SalaryConstants.PaymentStatuses.Contains(salary.PaymentStatus))
                errors.Add("Trạng thái thanh toán không hợp lệ");

            // Validate payment date
            if (salary.PaymentDate.HasValue && salary.PaymentDate.Value > DateTime.Now)
                errors.Add("Ngày thanh toán không được lớn hơn ngày hiện tại");

            // Business rules
            var totalSalary = salary.BaseSalary + salary.Allowance + salary.Bonus - salary.Deduction;
            if (totalSalary < 0)
                errors.Add("Tổng lương thực nhận không được âm");

            // Validate future salary
            var salaryDate = new DateTime(salary.Year, salary.Month, 1);
            if (salaryDate > DateTime.Now.AddMonths(1))
                errors.Add("Không thể tạo bảng lương quá xa trong tương lai");

            result.IsValid = errors.Count == 0;
            result.ValidationErrors = errors;
            result.ErrorMessage = string.Join("; ", errors);

            return result;
        }

        public SalaryDeleteValidation CanDeleteSalary(int salaryId)
        {
            try
            {
                var salary = salaryDAL.GetSalaryById(salaryId);
                if (salary == null)
                {
                    return new SalaryDeleteValidation
                    {
                        CanDelete = false,
                        Reason = "Không tìm thấy bảng lương"
                    };
                }

                if (salary.PaymentStatus == "Đã thanh toán")
                {
                    return new SalaryDeleteValidation
                    {
                        CanDelete = false,
                        Reason = "Không thể xóa bảng lương đã thanh toán"
                    };
                }

                return new SalaryDeleteValidation
                {
                    CanDelete = true,
                    Reason = ""
                };
            }
            catch (Exception ex)
            {
                return new SalaryDeleteValidation
                {
                    CanDelete = false,
                    Reason = $"Lỗi khi kiểm tra: {ex.Message}"
                };
            }
        }
        #endregion

        #region Statistics and Reports
        public SalaryStatistics GetSalaryStatistics()
        {
            try
            {
                return salaryDAL.GetSalaryStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê lương: {ex.Message}", ex);
            }
        }

        public List<SalaryReportData> GetSalaryReportData(int year)
        {
            try
            {
                var reportData = new List<SalaryReportData>();

                for (int month = 1; month <= 12; month++)
                {
                    var salaries = GetSalariesByMonthYear(month, year);

                    reportData.Add(new SalaryReportData
                    {
                        Month = month,
                        Year = year,
                        EmployeeCount = salaries.Count,
                        TotalBaseSalary = salaries.Sum(s => s.BaseSalary),
                        TotalAllowance = salaries.Sum(s => s.Allowance),
                        TotalBonus = salaries.Sum(s => s.Bonus),
                        TotalDeduction = salaries.Sum(s => s.Deduction),
                        TotalNetSalary = salaries.Sum(s => s.NetSalary)
                    });
                }

                return reportData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo lương: {ex.Message}", ex);
            }
        }

        public List<DepartmentSalarySummary> GetDepartmentSalarySummary(int month, int year)
        {
            try
            {
                // This would need to be implemented in DAL with proper department joins
                // For now, return empty list
                return new List<DepartmentSalarySummary>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo lương theo phòng ban: {ex.Message}", ex);
            }
        }
        #endregion

        #region Payment Operations
        public bool MarkSalaryAsPaid(int salaryId, DateTime paymentDate)
        {
            try
            {
                var salary = GetSalaryById(salaryId);
                if (salary == null)
                    throw new ArgumentException("Không tìm thấy bảng lương");

                if (salary.PaymentStatus == "Đã thanh toán")
                    throw new ArgumentException("Bảng lương đã được thanh toán");

                salary.MarkAsPaid(paymentDate);
                return UpdateSalary(salary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đánh dấu thanh toán: {ex.Message}", ex);
            }
        }

        public bool MarkSalaryAsPartiallyPaid(int salaryId, DateTime paymentDate)
        {
            try
            {
                var salary = GetSalaryById(salaryId);
                if (salary == null)
                    throw new ArgumentException("Không tìm thấy bảng lương");

                salary.MarkAsPartiallyPaid(paymentDate);
                return UpdateSalary(salary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đánh dấu thanh toán một phần: {ex.Message}", ex);
            }
        }

        public bool CancelSalary(int salaryId)
        {
            try
            {
                var salary = GetSalaryById(salaryId);
                if (salary == null)
                    throw new ArgumentException("Không tìm thấy bảng lương");

                if (salary.PaymentStatus == "Đã thanh toán")
                    throw new ArgumentException("Không thể hủy bảng lương đã thanh toán");

                salary.Cancel();
                return UpdateSalary(salary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi hủy bảng lương: {ex.Message}", ex);
            }
        }
        #endregion

        #region Bulk Operations
        public int CreateMonthlySalariesForAllEmployees(int month, int year)
        {
            try
            {
                var employees = GetEmployeesForDropdown();
                int created = 0;

                foreach (var employee in employees)
                {
                    // Check if salary already exists
                    if (!salaryDAL.IsSalaryExists(employee.EmployeeID, month, year))
                    {
                        var salary = new Salary
                        {
                            EmployeeID = employee.EmployeeID,
                            Month = month,
                            Year = year,
                            BaseSalary = employee.BaseSalary,
                            Allowance = 0,
                            Bonus = 0,
                            Deduction = 0,
                            PaymentStatus = "Chưa thanh toán",
                            Notes = $"Tự động tạo cho tháng {month}/{year}"
                        };

                        salary.CalculateNetSalary();

                        if (AddSalary(salary) > 0)
                            created++;
                    }
                }

                return created;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo bảng lương hàng loạt: {ex.Message}", ex);
            }
        }

        public bool UpdatePaymentStatusBatch(List<int> salaryIds, string newStatus, DateTime? paymentDate = null)
        {
            try
            {
                int successCount = 0;

                foreach (var salaryId in salaryIds)
                {
                    var salary = GetSalaryById(salaryId);
                    if (salary != null && salary.CanEdit())
                    {
                        salary.PaymentStatus = newStatus;
                        if (paymentDate.HasValue)
                            salary.PaymentDate = paymentDate.Value;

                        if (UpdateSalary(salary))
                            successCount++;
                    }
                }

                return successCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái hàng loạt: {ex.Message}", ex);
            }
        }
        #endregion
    }
}