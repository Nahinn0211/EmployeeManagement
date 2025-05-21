using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.BLL
{
    public class FinanceBLL
    {
         private readonly FinanceDAL financeDAL;
      
        public FinanceBLL()
        {
            financeDAL = new FinanceDAL();
        }
        
        /// <summary>
        /// Lấy tất cả giao dịch tài chính
        /// </summary>
        public List<Finance> GetAllFinances()
        {
            try
            {
                return financeDAL.GetAllFinances();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy danh sách giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo ID
        /// </summary>
        public Finance GetFinanceById(int financeId)
        {
            try
            {
                if (financeId <= 0)
                    throw new ArgumentException("ID giao dịch tài chính không hợp lệ");

                return financeDAL.GetFinanceById(financeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thông tin giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm giao dịch tài chính mới
        /// </summary>
        public int AddFinance(Finance finance)
        {
            try
            {
                // Validate business rules
                ValidateFinanceForInsert(finance);

                // Generate transaction code if not provided
                if (string.IsNullOrWhiteSpace(finance.TransactionCode))
                {
                    finance.TransactionCode = financeDAL.GenerateTransactionCode(finance.TransactionType);
                }

                // Set default values
                finance.Status = string.IsNullOrWhiteSpace(finance.Status) ? "Đã ghi nhận" : finance.Status;
                finance.CreatedAt = DateTime.Now;

                return financeDAL.InsertFinance(finance);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi thêm giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin giao dịch tài chính
        /// </summary>
        public bool UpdateFinance(Finance finance)
        {
            try
            {
                // Validate business rules
                ValidateFinanceForUpdate(finance);

                return financeDAL.UpdateFinance(finance);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi cập nhật giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa giao dịch tài chính
        /// </summary>
        public bool DeleteFinance(int financeId)
        {
            try
            {
                if (financeId <= 0)
                    throw new ArgumentException("ID giao dịch tài chính không hợp lệ");

                // Kiểm tra giao dịch có tồn tại không
                var finance = financeDAL.GetFinanceById(financeId);
                if (finance == null)
                    throw new Exception("Giao dịch tài chính không tồn tại");

                // Kiểm tra quyền xóa (chỉ cho phép xóa giao dịch ở một số trạng thái nhất định)
                if (finance.Status == "Đã duyệt")
                    throw new Exception("Không thể xóa giao dịch đã được duyệt");

                return financeDAL.DeleteFinance(financeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi xóa giao dịch tài chính: {ex.Message}", ex);
            }
        }

       

        /// <summary>
        /// Validate giao dịch tài chính khi thêm mới
        /// </summary>
        private void ValidateFinanceForInsert(Finance finance)
        {
            // Validate required fields
            ValidateRequiredFields(finance);

            // Validate business rules
            ValidateBusinessRules(finance);

            // Check duplicates
            if (!string.IsNullOrWhiteSpace(finance.TransactionCode) &&
                financeDAL.IsTransactionCodeExists(finance.TransactionCode))
            {
                throw new Exception("Mã giao dịch đã tồn tại trong hệ thống");
            }
        }

        /// <summary>
        /// Validate giao dịch tài chính khi cập nhật
        /// </summary>
        private void ValidateFinanceForUpdate(Finance finance)
        {
            if (finance.FinanceID <= 0)
                throw new ArgumentException("ID giao dịch tài chính không hợp lệ");

            // Validate required fields
            ValidateRequiredFields(finance);

            // Validate business rules
            ValidateBusinessRules(finance);

            // Check duplicates (exclude current finance)
            if (!string.IsNullOrWhiteSpace(finance.TransactionCode) &&
                financeDAL.IsTransactionCodeExists(finance.TransactionCode, finance.FinanceID))
            {
                throw new Exception("Mã giao dịch đã tồn tại trong hệ thống");
            }
        }

        /// <summary>
        /// Validate các trường bắt buộc
        /// </summary>
        private void ValidateRequiredFields(Finance finance)
        {
            if (finance == null)
                throw new ArgumentNullException(nameof(finance), "Thông tin giao dịch tài chính không được để trống");

            if (string.IsNullOrWhiteSpace(finance.TransactionCode))
                throw new Exception("Mã giao dịch không được để trống");

            if (string.IsNullOrWhiteSpace(finance.TransactionType))
                throw new Exception("Loại giao dịch không được để trống");

            if (string.IsNullOrWhiteSpace(finance.Category))
                throw new Exception("Danh mục không được để trống");

            if (finance.Amount <= 0)
                throw new Exception("Số tiền phải lớn hơn 0");

            if (finance.RecordedByID <= 0)
                throw new Exception("Người ghi nhận không hợp lệ");
        }

        /// <summary>
        /// Validate các quy tắc nghiệp vụ
        /// </summary>
        private void ValidateBusinessRules(Finance finance)
        {
            // Validate transaction code format
            if (!string.IsNullOrWhiteSpace(finance.TransactionCode))
            {
                if (finance.TransactionCode.Length < 3)
                    throw new Exception("Mã giao dịch phải có ít nhất 3 ký tự");

                if (finance.TransactionCode.Length > 20)
                    throw new Exception("Mã giao dịch không được vượt quá 20 ký tự");

                if (!Regex.IsMatch(finance.TransactionCode, @"^[A-Za-z0-9]+$"))
                    throw new Exception("Mã giao dịch chỉ được chứa chữ cái và số");
            }

            // Validate transaction type
            if (!TransactionTypes.Types.Contains(finance.TransactionType))
                throw new Exception("Loại giao dịch không hợp lệ");

            // Validate category based on transaction type
            var validCategories = FinanceCategories.GetCategoriesByType(finance.TransactionType);
            if (!validCategories.Contains(finance.Category))
                throw new Exception($"Danh mục '{finance.Category}' không hợp lệ cho loại giao dịch '{finance.TransactionType}'");

            // Validate amount
            if (finance.Amount > 999999999999.99m)
                throw new Exception("Số tiền quá lớn");

            // Validate transaction date
            if (finance.TransactionDate > DateTime.Now.Date.AddDays(1))
                throw new Exception("Ngày giao dịch không được là tương lai");

            if (finance.TransactionDate < new DateTime(2000, 1, 1))
                throw new Exception("Ngày giao dịch không hợp lệ");

            // Validate description length
            if (!string.IsNullOrWhiteSpace(finance.Description) && finance.Description.Length > 500)
                throw new Exception("Mô tả không được vượt quá 500 ký tự");

            // Validate payment method
            if (!string.IsNullOrWhiteSpace(finance.PaymentMethod))
            {
                if (!PaymentMethods.Methods.Contains(finance.PaymentMethod))
                    throw new Exception("Phương thức thanh toán không hợp lệ");
            }

            // Validate reference number
            if (!string.IsNullOrWhiteSpace(finance.ReferenceNo) && finance.ReferenceNo.Length > 50)
                throw new Exception("Số tham chiếu không được vượt quá 50 ký tự");

            // Validate status
            if (!string.IsNullOrWhiteSpace(finance.Status))
            {
                if (!FinanceStatus.Statuses.Contains(finance.Status))
                    throw new Exception("Trạng thái không hợp lệ");
            }

            // Validate relationships (chỉ được liên kết với 1 trong 3: Project, Customer, Employee)
            int relationshipCount = 0;
            if (finance.ProjectID.HasValue && finance.ProjectID.Value > 0) relationshipCount++;
            if (finance.CustomerID.HasValue && finance.CustomerID.Value > 0) relationshipCount++;
            if (finance.EmployeeID.HasValue && finance.EmployeeID.Value > 0) relationshipCount++;

            if (relationshipCount > 1)
                throw new Exception("Giao dịch chỉ có thể liên kết với một đối tượng (Dự án, Khách hàng, hoặc Nhân viên)");
        }

      
        /// <summary>
        /// Tìm kiếm giao dịch tài chính theo điều kiện
        /// </summary>
        public List<Finance> SearchFinances(string searchText = "", string transactionType = "",
            string category = "", string status = "", int? projectId = null, int? customerId = null,
            int? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return financeDAL.SearchFinances(searchText?.Trim(), transactionType?.Trim(),
                    category?.Trim(), status?.Trim(), projectId, customerId, employeeId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi tìm kiếm giao dịch tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo dự án
        /// </summary>
        public List<Finance> GetFinancesByProject(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return financeDAL.GetFinancesByProject(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy giao dịch tài chính theo dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo khách hàng
        /// </summary>
        public List<Finance> GetFinancesByCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ");

                return financeDAL.GetFinancesByCustomer(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy giao dịch tài chính theo khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch tài chính theo nhân viên
        /// </summary>
        public List<Finance> GetFinancesByEmployee(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("ID nhân viên không hợp lệ");

                return financeDAL.GetFinancesByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy giao dịch tài chính theo nhân viên: {ex.Message}", ex);
            }
        }

   

        /// <summary>
        /// Tạo mã giao dịch tự động
        /// </summary>
        public string GenerateTransactionCode(string transactionType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionType))
                    throw new ArgumentException("Loại giao dịch không được để trống");

                if (!TransactionTypes.Types.Contains(transactionType))
                    throw new ArgumentException("Loại giao dịch không hợp lệ");

                return financeDAL.GenerateTransactionCode(transactionType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra mã giao dịch có tồn tại không
        /// </summary>
        public bool IsTransactionCodeExists(string transactionCode, int excludeFinanceId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionCode))
                    return false;

                return financeDAL.IsTransactionCodeExists(transactionCode.Trim(), excludeFinanceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái giao dịch
        /// </summary>
        public bool UpdateFinanceStatus(int financeId, string newStatus)
        {
            try
            {
                if (financeId <= 0)
                    throw new ArgumentException("ID giao dịch tài chính không hợp lệ");

                var finance = financeDAL.GetFinanceById(financeId);
                if (finance == null)
                    throw new Exception("Giao dịch tài chính không tồn tại");

                // Validate new status
                if (!FinanceStatus.Statuses.Contains(newStatus))
                    throw new Exception("Trạng thái mới không hợp lệ");

                // Business rules for status transitions
                ValidateStatusTransition(finance.Status, newStatus);

                finance.Status = newStatus;
                return financeDAL.UpdateFinance(finance);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi cập nhật trạng thái: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate chuyển đổi trạng thái
        /// </summary>
        private void ValidateStatusTransition(string currentStatus, string newStatus)
        {
            // Define valid transitions
            var validTransitions = new Dictionary<string, string[]>
            {
                ["Chờ duyệt"] = new[] { "Đã duyệt", "Từ chối", "Hủy" },
                ["Đã ghi nhận"] = new[] { "Chờ duyệt", "Hủy" },
                ["Đã duyệt"] = new string[] { }, // Không thể chuyển từ đã duyệt
                ["Từ chối"] = new[] { "Chờ duyệt" },
                ["Hủy"] = new string[] { } // Không thể chuyển từ đã hủy
            };

            if (validTransitions.ContainsKey(currentStatus))
            {
                if (!validTransitions[currentStatus].Contains(newStatus))
                {
                    throw new Exception($"Không thể chuyển từ trạng thái '{currentStatus}' sang '{newStatus}'");
                }
            }
        }

        /// <summary>
        /// Lấy danh sách cho dropdown
        /// </summary>
        public List<Project> GetProjectsForDropdown()
        {
            try
            {
                return financeDAL.GetProjectsForDropdown();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dự án: {ex.Message}", ex);
            }
        }

        public List<Customer> GetCustomersForDropdown()
        {
            try
            {
                return financeDAL.GetCustomersForDropdown();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
            }
        }

        public List<Employee> GetEmployeesForDropdown()
        {
            try
            {
                return financeDAL.GetEmployeesForDropdown();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }

       

        /// <summary>
        /// Lấy thống kê tài chính
        /// </summary>
        public (decimal TotalIncome, decimal TotalExpense, decimal Balance, int TotalTransactions) GetFinanceStatistics(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return financeDAL.GetFinanceStatistics(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê theo danh mục
        /// </summary>
        public Dictionary<string, decimal> GetCategoryStatistics(string transactionType, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionType))
                    throw new ArgumentException("Loại giao dịch không được để trống");

                if (!TransactionTypes.Types.Contains(transactionType))
                    throw new ArgumentException("Loại giao dịch không hợp lệ");

                return financeDAL.GetCategoryStatistics(transactionType, fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê theo danh mục: {ex.Message}", ex);
            }
        }


        // Update các method signatures trong FinanceBLL.cs để tránh conflicts:

        /// <summary>
        /// Lấy thống kê theo tháng
        /// </summary>
        public List<EmployeeManagement.Models.DTO.MonthlyFinanceReportDTO> GetMonthlyStatistics(int year)
        {
            try
            {
                if (year < 2000 || year > DateTime.Now.Year + 1)
                    throw new ArgumentException("Năm không hợp lệ");

                return financeDAL.GetMonthlyStatistics(year);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê theo tháng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính theo dự án
        /// </summary>
        public List<EmployeeManagement.Models.DTO.ProjectFinanceReportDTO> GetProjectFinanceStatistics()
        {
            try
            {
                return financeDAL.GetProjectFinanceStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính toán cash flow theo tháng - UPDATED
        /// </summary>
        public List<CashFlowReport> GetCashFlowReport(int year)
        {
            try
            {
                var monthlyReports = GetMonthlyStatistics(year);
                var cashFlowReports = new List<CashFlowReport>();
                decimal runningBalance = 0;

                for (int month = 1; month <= 12; month++)
                {
                    var monthlyReport = monthlyReports.FirstOrDefault(r => r.Month == month);
                    decimal income = monthlyReport?.Income ?? 0;
                    decimal expense = monthlyReport?.Expense ?? 0;
                    decimal netFlow = income - expense;
                    runningBalance += netFlow;

                    cashFlowReports.Add(new CashFlowReport
                    {
                        Month = month,
                        Year = year,
                        Income = income,
                        Expense = expense,
                        NetFlow = netFlow,
                        RunningBalance = runningBalance
                    });
                }

                return cashFlowReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi tính toán cash flow: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phân tích xu hướng tài chính - UPDATED
        /// </summary>
        public FinanceTrendAnalysis GetFinanceTrendAnalysis(int year)
        {
            try
            {
                var monthlyReports = GetMonthlyStatistics(year);
                var previousYearReports = GetMonthlyStatistics(year - 1);

                decimal currentYearIncome = monthlyReports.Sum(r => r.Income);
                decimal currentYearExpense = monthlyReports.Sum(r => r.Expense);
                decimal previousYearIncome = previousYearReports.Sum(r => r.Income);
                decimal previousYearExpense = previousYearReports.Sum(r => r.Expense);

                decimal incomeGrowthRate = previousYearIncome > 0 ?
                    ((currentYearIncome - previousYearIncome) / previousYearIncome) * 100 : 0;

                decimal expenseGrowthRate = previousYearExpense > 0 ?
                    ((currentYearExpense - previousYearExpense) / previousYearExpense) * 100 : 0;

                return new FinanceTrendAnalysis
                {
                    Year = year,
                    TotalIncome = currentYearIncome,
                    TotalExpense = currentYearExpense,
                    NetProfit = currentYearIncome - currentYearExpense,
                    IncomeGrowthRate = incomeGrowthRate,
                    ExpenseGrowthRate = expenseGrowthRate,
                    ProfitMargin = currentYearIncome > 0 ?
                        ((currentYearIncome - currentYearExpense) / currentYearIncome) * 100 : 0,
                    MonthlyReports = monthlyReports.Select(r => new MonthlyFinanceReport
                    {
                        Month = r.Month,
                        Year = r.Year,
                        Income = r.Income,
                        Expense = r.Expense
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi phân tích xu hướng: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Validate dữ liệu import
        /// </summary>
        public List<string> ValidateImportData(List<Finance> finances)
        {
            var errors = new List<string>();

            try
            {
                for (int i = 0; i < finances.Count; i++)
                {
                    var finance = finances[i];
                    var rowNumber = i + 1;

                    try
                    {
                        ValidateRequiredFields(finance);
                        ValidateBusinessRules(finance);

                        // Check for duplicates in import data
                        var duplicateCode = finances
                            .Take(i)
                            .Any(f => f.TransactionCode?.Equals(finance.TransactionCode, StringComparison.OrdinalIgnoreCase) == true);

                        if (duplicateCode)
                            errors.Add($"Dòng {rowNumber}: Mã giao dịch '{finance.TransactionCode}' bị trùng trong dữ liệu import");

                        // Check database duplicates
                        if (financeDAL.IsTransactionCodeExists(finance.TransactionCode))
                            errors.Add($"Dòng {rowNumber}: Mã giao dịch '{finance.TransactionCode}' đã tồn tại trong hệ thống");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Dòng {rowNumber}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi validate dữ liệu import: {ex.Message}");
            }

            return errors;
        }

        // Fix cho FinanceBLL.cs - Thay thế toàn bộ phần cuối của file

        /// <summary>
        /// Từ chối giao dịch
        /// </summary>
        public bool RejectFinance(int financeId, string rejectReason)
        {
            try
            {
                return financeDAL.RejectFinance(financeId, rejectReason);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi từ chối giao dịch: {ex.Message}", ex);
            }
        }

 
        /// <summary>
        /// Lấy thống kê tài chính chi tiết theo dự án
        /// </summary>
        public ProjectFinanceStatistics GetProjectFinanceStatistics(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return financeDAL.GetProjectFinanceStatistics(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy báo cáo tài chính chi tiết theo dự án
        /// </summary>
        public ProjectFinanceReport GetProjectFinanceReport(int projectId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return financeDAL.GetProjectFinanceReport(projectId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra ngân sách dự án
        /// </summary>
        public BudgetCheckResult CheckProjectBudget(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return financeDAL.CheckProjectBudget(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra ngân sách dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật ngân sách dự án
        /// </summary>
        public bool UpdateProjectBudget(int projectId, decimal newBudget)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                if (newBudget < 0)
                    throw new ArgumentException("Ngân sách không được âm");

                return financeDAL.UpdateProjectBudget(projectId, newBudget);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật ngân sách dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách dự án có giao dịch tài chính
        /// </summary>
        public List<Project> GetProjectsWithFinances()
        {
            try
            {
                return financeDAL.GetProjectsWithFinances();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dự án có tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính toán ROI (Return on Investment) của dự án
        /// </summary>
        public ProjectROIResult CalculateProjectROI(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return financeDAL.CalculateProjectROI(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính ROI dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy xu hướng tài chính theo tháng của dự án
        /// </summary>
        public List<MonthlyProjectFinance> GetProjectFinanceTrend(int projectId, int months = 12)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                if (months <= 0 || months > 60)
                    throw new ArgumentException("Số tháng phải từ 1-60");

                return financeDAL.GetProjectFinanceTrend(projectId, months);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy xu hướng tài chính dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch với bộ lọc nâng cao cho dự án
        /// </summary>
        public List<Finance> GetProjectFinancesWithFilter(int projectId, ProjectFinanceFilter filter)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                if (filter == null)
                    filter = new ProjectFinanceFilter();

                return financeDAL.GetProjectFinancesWithFilter(projectId, filter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lọc giao dịch dự án: {ex.Message}", ex);
            }
        }

        

        /// <summary>
        /// Lấy thống kê tài chính với DTO trả về đầy đủ
        /// </summary>
        public EmployeeManagement.Models.DTO.FinanceStatistics GetFinanceStatistics()
        {
            try
            {
                return financeDAL.GetFinanceStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê tài chính theo khoảng thời gian
        /// </summary>
        public EmployeeManagement.Models.DTO.FinanceStatistics GetFinanceStatisticsByDateRange(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return financeDAL.GetFinanceStatisticsByDateRange(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tài chính theo thời gian: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy top giao dịch lớn nhất
        /// </summary>
        public List<Finance> GetTopTransactions(int top = 10, string transactionType = "")
        {
            try
            {
                if (top <= 0 || top > 100)
                    throw new ArgumentException("Số lượng top phải từ 1-100");

                if (!string.IsNullOrEmpty(transactionType) && !TransactionTypes.Types.Contains(transactionType))
                    throw new ArgumentException("Loại giao dịch không hợp lệ");

                return financeDAL.GetTopTransactions(top, transactionType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy top giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch gần đây
        /// </summary>
        public List<Finance> GetRecentTransactions(int days = 7, int limit = 50)
        {
            try
            {
                if (days <= 0 || days > 365)
                    throw new ArgumentException("Số ngày phải từ 1-365");

                if (limit <= 0 || limit > 1000)
                    throw new ArgumentException("Số lượng giới hạn phải từ 1-1000");

                return financeDAL.GetRecentTransactions(days, limit);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch gần đây: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch chờ duyệt
        /// </summary>
        public List<Finance> GetPendingApprovalTransactions()
        {
            try
            {
                return financeDAL.GetFinancesByStatus("Chờ duyệt");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch chờ duyệt: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy giao dịch theo trạng thái
        /// </summary>
        public List<Finance> GetFinancesByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    throw new ArgumentException("Trạng thái không được để trống");

                if (!FinanceStatus.Statuses.Contains(status))
                    throw new ArgumentException("Trạng thái không hợp lệ");

                return financeDAL.GetFinancesByStatus(status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch theo trạng thái: {ex.Message}", ex);
            }
        }

       

        /// <summary>
        /// Duyệt giao dịch tài chính
        /// </summary>
        public bool ApproveFinance(int financeId, int approvedByUserId)
        {
            try
            {
                if (financeId <= 0)
                    throw new ArgumentException("ID giao dịch không hợp lệ");

                if (approvedByUserId <= 0)
                    throw new ArgumentException("ID người duyệt không hợp lệ");

                var finance = financeDAL.GetFinanceById(financeId);
                if (finance == null)
                    throw new Exception("Giao dịch không tồn tại");

                if (finance.Status != "Chờ duyệt")
                    throw new Exception("Chỉ có thể duyệt giao dịch đang ở trạng thái 'Chờ duyệt'");

                return financeDAL.ApproveFinance(financeId, approvedByUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi duyệt giao dịch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Duyệt nhiều giao dịch cùng lúc
        /// </summary>
        public (int SuccessCount, int ErrorCount, List<string> Errors) ApproveMultipleFinances(List<int> financeIds, int approvedByUserId)
        {
            var errors = new List<string>();
            int successCount = 0;
            int errorCount = 0;

            try
            {
                if (financeIds == null || !financeIds.Any())
                    throw new ArgumentException("Danh sách giao dịch không được để trống");

                if (approvedByUserId <= 0)
                    throw new ArgumentException("ID người duyệt không hợp lệ");

                foreach (var financeId in financeIds)
                {
                    try
                    {
                        if (ApproveFinance(financeId, approvedByUserId))
                            successCount++;
                        else
                            errorCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"ID {financeId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi duyệt nhiều giao dịch: {ex.Message}");
                errorCount = financeIds?.Count ?? 0;
            }

            return (successCount, errorCount, errors);
        }

        /// <summary>
        /// Từ chối nhiều giao dịch cùng lúc
        /// </summary>
        public (int SuccessCount, int ErrorCount, List<string> Errors) RejectMultipleFinances(List<int> financeIds, string rejectReason)
        {
            var errors = new List<string>();
            int successCount = 0;
            int errorCount = 0;

            try
            {
                if (financeIds == null || !financeIds.Any())
                    throw new ArgumentException("Danh sách giao dịch không được để trống");

                if (string.IsNullOrWhiteSpace(rejectReason))
                    throw new ArgumentException("Lý do từ chối không được để trống");

                foreach (var financeId in financeIds)
                {
                    try
                    {
                        if (RejectFinance(financeId, rejectReason))
                            successCount++;
                        else
                            errorCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"ID {financeId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi từ chối nhiều giao dịch: {ex.Message}");
                errorCount = financeIds?.Count ?? 0;
            }

            return (successCount, errorCount, errors);
        }

        

        /// <summary>
        /// Dự đoán xu hướng tài chính dựa trên dữ liệu lịch sử
        /// </summary>
        public EmployeeManagement.Models.DTO.FinanceForecast GetFinanceForecast(int months = 6)
        {
            try
            {
                if (months <= 0 || months > 24)
                    throw new ArgumentException("Số tháng dự đoán phải từ 1-24");

                return financeDAL.GetFinanceForecast(months);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi dự đoán xu hướng tài chính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phân tích hiệu suất tài chính theo kỳ
        /// </summary>
        public EmployeeManagement.Models.DTO.PerformanceAnalysis GetPerformanceAnalysis(DateTime fromDate, DateTime toDate, string comparisonPeriod = "PreviousPeriod")
        {
            try
            {
                if (fromDate >= toDate)
                    throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");

                return financeDAL.GetPerformanceAnalysis(fromDate, toDate, comparisonPeriod);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi phân tích hiệu suất: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phát hiện các giao dịch bất thường
        /// </summary>
        public List<EmployeeManagement.Models.DTO.AnomalyDetectionResult> DetectAnomalousTransactions(int days = 30)
        {
            try
            {
                if (days <= 0 || days > 365)
                    throw new ArgumentException("Số ngày phải từ 1-365");

                return financeDAL.DetectAnomalousTransactions(days);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi phát hiện giao dịch bất thường: {ex.Message}", ex);
            }
        }

        

        /// <summary>
        /// Xuất dữ liệu tài chính dự án ra Excel
        /// </summary>
        public bool ExportProjectFinanceToExcel(int projectId, string filePath)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("Đường dẫn file không được để trống");

                var finances = GetFinancesByProject(projectId);
                var statistics = GetProjectFinanceStatistics(projectId);

                // TODO: Implement ExcelExportHelper
                return ExcelExportHelper.ExportProjectFinance(finances, statistics, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xuất báo cáo Excel: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xuất báo cáo tài chính tổng hợp ra Excel
        /// </summary>
        public bool ExportFinanceSummaryToExcel(string filePath, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("Đường dẫn file không được để trống");

                var finances = SearchFinances("", "", "", "", null, null, null, fromDate, toDate);
                var statistics = GetFinanceStatisticsByDateRange(fromDate, toDate);

                // TODO: Implement ExcelExportHelper
                return ExcelExportHelper.ExportFinanceSummary(finances, statistics, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xuất báo cáo tổng hợp: {ex.Message}", ex);
            }
        }

       

        /// <summary>
        /// Helper class cho Excel export
        /// </summary>
        public static class ExcelExportHelper
        {
            public static bool ExportProjectFinance(List<Finance> finances, ProjectFinanceStatistics statistics, string filePath)
            {
                // TODO: Implement Excel export logic
                throw new NotImplementedException("Excel export functionality not implemented yet");
            }

            public static bool ExportFinanceSummary(List<Finance> finances, EmployeeManagement.Models.DTO.FinanceStatistics statistics, string filePath)
            {
                // TODO: Implement Excel export logic
                throw new NotImplementedException("Excel export functionality not implemented yet");
            }
        }


        /// <summary>
        /// Import danh sách giao dịch tài chính
        /// </summary>
        public (int SuccessCount, int ErrorCount, List<string> Errors) ImportFinances(List<Finance> finances)
        {
            var errors = new List<string>();
            int successCount = 0;
            int errorCount = 0;

            try
            {
                // Validate all data first
                var validationErrors = ValidateImportData(finances);
                if (validationErrors.Any())
                {
                    return (0, finances.Count, validationErrors);
                }

                // Import data
                for (int i = 0; i < finances.Count; i++)
                {
                    try
                    {
                        var finance = finances[i];
                        finance.Status = string.IsNullOrWhiteSpace(finance.Status) ? "Đã ghi nhận" : finance.Status;

                        if (string.IsNullOrWhiteSpace(finance.TransactionCode))
                            finance.TransactionCode = financeDAL.GenerateTransactionCode(finance.TransactionType);

                        financeDAL.InsertFinance(finance);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Dòng {i + 1}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi import dữ liệu: {ex.Message}");
                errorCount = finances.Count;
            }

            return (successCount, errorCount, errors);
 
        }
 

    }

}




