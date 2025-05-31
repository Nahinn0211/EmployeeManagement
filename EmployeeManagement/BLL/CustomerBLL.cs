using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.Entity; 

namespace EmployeeManagement.BLL
{
    public class CustomerBLL
    {
        #region Fields
        private readonly CustomerDAL customerDAL;
        #endregion

        #region Constructor
        public CustomerBLL()
        {
            customerDAL = new CustomerDAL();
        }
        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy tất cả khách hàng
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            try
            {
                return customerDAL.GetAllCustomers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy danh sách khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy khách hàng theo ID
        /// </summary>
        public Customer GetCustomerById(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ");

                return customerDAL.GetCustomerById(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thông tin khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm khách hàng mới
        /// </summary>
        public int AddCustomer(Customer customer)
        {
            try
            {
                // Validate business rules
                ValidateCustomerForInsert(customer);

                // Generate customer code if not provided
                if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                {
                    customer.CustomerCode = customerDAL.GenerateCustomerCode();
                }

                // Set default values
                customer.Status = string.IsNullOrWhiteSpace(customer.Status) ? "Đang hợp tác" : customer.Status;
                customer.CreatedAt = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;

                return customerDAL.InsertCustomer(customer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi thêm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                // Validate business rules
                ValidateCustomerForUpdate(customer);

                customer.UpdatedAt = DateTime.Now;

                return customerDAL.UpdateCustomer(customer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi cập nhật khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ");

                // Kiểm tra khách hàng có tồn tại không
                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                    throw new Exception("Khách hàng không tồn tại");

                return customerDAL.DeleteCustomer(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi xóa khách hàng: {ex.Message}", ex);
            }
        }

        #endregion

        #region Business Validation

        /// <summary>
        /// Validate khách hàng khi thêm mới
        /// </summary>
        private void ValidateCustomerForInsert(Customer customer)
        {
            // Validate required fields
            ValidateRequiredFields(customer);

            // Validate business rules
            ValidateBusinessRules(customer);

            // Check duplicates
            if (!string.IsNullOrWhiteSpace(customer.CustomerCode) &&
                customerDAL.IsCustomerCodeExists(customer.CustomerCode))
            {
                throw new Exception("Mã khách hàng đã tồn tại trong hệ thống");
            }

            if (!string.IsNullOrWhiteSpace(customer.Email) &&
                customerDAL.IsEmailExists(customer.Email))
            {
                throw new Exception("Email đã được sử dụng bởi khách hàng khác");
            }
        }

        /// <summary>
        /// Validate khách hàng khi cập nhật
        /// </summary>
        private void ValidateCustomerForUpdate(Customer customer)
        {
            if (customer.CustomerID <= 0)
                throw new ArgumentException("ID khách hàng không hợp lệ");

            // Validate required fields
            ValidateRequiredFields(customer);

            // Validate business rules
            ValidateBusinessRules(customer);

            // Check duplicates (exclude current customer)
            if (!string.IsNullOrWhiteSpace(customer.CustomerCode) &&
                customerDAL.IsCustomerCodeExists(customer.CustomerCode, customer.CustomerID))
            {
                throw new Exception("Mã khách hàng đã tồn tại trong hệ thống");
            }

            if (!string.IsNullOrWhiteSpace(customer.Email) &&
                customerDAL.IsEmailExists(customer.Email, customer.CustomerID))
            {
                throw new Exception("Email đã được sử dụng bởi khách hàng khác");
            }
        }

        /// <summary>
        /// Validate các trường bắt buộc
        /// </summary>
        private void ValidateRequiredFields(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer), "Thông tin khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(customer.CompanyName))
                throw new Exception("Tên công ty không được để trống");

            if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                throw new Exception("Mã khách hàng không được để trống");
        }

        /// <summary>
        /// Validate các quy tắc nghiệp vụ
        /// </summary>
        private void ValidateBusinessRules(Customer customer)
        {
            // Validate customer code format
            if (!string.IsNullOrWhiteSpace(customer.CustomerCode))
            {
                if (customer.CustomerCode.Length < 3)
                    throw new Exception("Mã khách hàng phải có ít nhất 3 ký tự");

                if (customer.CustomerCode.Length > 20)
                    throw new Exception("Mã khách hàng không được vượt quá 20 ký tự");

                if (!Regex.IsMatch(customer.CustomerCode, @"^[A-Za-z0-9]+$"))
                    throw new Exception("Mã khách hàng chỉ được chứa chữ cái và số");
            }

            // Validate company name
            if (!string.IsNullOrWhiteSpace(customer.CompanyName))
            {
                if (customer.CompanyName.Length > 200)
                    throw new Exception("Tên công ty không được vượt quá 200 ký tự");
            }

            // Validate contact name
            if (!string.IsNullOrWhiteSpace(customer.ContactName) && customer.ContactName.Length > 100)
                throw new Exception("Tên người liên hệ không được vượt quá 100 ký tự");

            // Validate contact title
            if (!string.IsNullOrWhiteSpace(customer.ContactTitle) && customer.ContactTitle.Length > 50)
                throw new Exception("Chức vụ người liên hệ không được vượt quá 50 ký tự");

            // Validate email format
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                if (customer.Email.Length > 100)
                    throw new Exception("Email không được vượt quá 100 ký tự");

                if (!IsValidEmail(customer.Email))
                    throw new Exception("Định dạng email không hợp lệ");
            }

            // Validate phone number
            if (!string.IsNullOrWhiteSpace(customer.Phone))
            {
                if (customer.Phone.Length > 20)
                    throw new Exception("Số điện thoại không được vượt quá 20 ký tự");

                if (!IsValidPhoneNumber(customer.Phone))
                    throw new Exception("Số điện thoại không hợp lệ");
            }

            // Validate address
            if (!string.IsNullOrWhiteSpace(customer.Address) && customer.Address.Length > 255)
                throw new Exception("Địa chỉ không được vượt quá 255 ký tự");

            // Validate status
            if (!string.IsNullOrWhiteSpace(customer.Status))
            {
                var validStatuses = new[] { "Đang hợp tác", "Tạm dừng", "Ngừng hợp tác" };
                if (!validStatuses.Contains(customer.Status))
                    throw new Exception("Trạng thái khách hàng không hợp lệ");
            }
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại
        /// </summary>
        private bool IsValidPhoneNumber(string phone)
        {
            try
            {
                // Remove spaces, dashes, parentheses, and plus signs
                string cleanPhone = Regex.Replace(phone, @"[\s\-\(\)\+]", "");

                // Check if it contains only digits and has appropriate length
                return Regex.IsMatch(cleanPhone, @"^\d{10,11}$");
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Search & Filter

        /// <summary>
        /// Tìm kiếm khách hàng theo điều kiện
        /// </summary>
        public List<Customer> SearchCustomers(string searchText = "", string status = "")
        {
            try
            {
                return customerDAL.SearchCustomers(searchText?.Trim(), status?.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi tìm kiếm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lọc khách hàng theo trạng thái
        /// </summary>
        public List<Customer> GetCustomersByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return GetAllCustomers();

                return customerDAL.SearchCustomers("", status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lọc khách hàng theo trạng thái: {ex.Message}", ex);
            }
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Tạo mã khách hàng tự động
        /// </summary>
        public string GenerateCustomerCode()
        {
            try
            {
                return customerDAL.GenerateCustomerCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra khách hàng có thể xóa được không
        /// </summary>
        public (bool CanDelete, string Reason) CanDeleteCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    return (false, "ID khách hàng không hợp lệ");

                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                    return (false, "Khách hàng không tồn tại");

                var constraints = customerDAL.CheckCustomerConstraints(customerId);
                return (!constraints.HasConstraints, constraints.ErrorMessage);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi kiểm tra: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái khách hàng
        /// </summary>
        public bool UpdateCustomerStatus(int customerId, string newStatus)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ");

                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                    throw new Exception("Khách hàng không tồn tại");

                // Validate new status
                var validStatuses = new[] { "Đang hợp tác", "Tạm dừng", "Ngừng hợp tác" };
                if (!validStatuses.Contains(newStatus))
                    throw new Exception("Trạng thái mới không hợp lệ");

                customer.Status = newStatus;
                customer.UpdatedAt = DateTime.Now;

                return customerDAL.UpdateCustomer(customer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi cập nhật trạng thái: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra mã khách hàng có tồn tại không
        /// </summary>
        public bool IsCustomerCodeExists(string customerCode, int excludeCustomerId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerCode))
                    return false;

                return customerDAL.IsCustomerCodeExists(customerCode.Trim(), excludeCustomerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra email có tồn tại không
        /// </summary>
        public bool IsEmailExists(string email, int excludeCustomerId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                return customerDAL.IsEmailExists(email.Trim(), excludeCustomerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra email: {ex.Message}", ex);
            }
        }

        #endregion

        #region Statistics & Reports

        /// <summary>
        /// Lấy thống kê khách hàng
        /// </summary>
        public (int Total, int Active, int Paused, int Inactive) GetCustomerStatistics()
        {
            try
            {
                return customerDAL.GetCustomerStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách khách hàng cho ComboBox/DropDown
        /// </summary>
        public List<Customer> GetCustomersForDropdown()
        {
            try
            {
                var customers = customerDAL.SearchCustomers("", "Đang hợp tác");
                return customers.OrderBy(c => c.CompanyName).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate dữ liệu import
        /// </summary>
        public List<string> ValidateImportData(List<Customer> customers)
        {
            var errors = new List<string>();

            try
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    var customer = customers[i];
                    var rowNumber = i + 1;

                    try
                    {
                        ValidateRequiredFields(customer);
                        ValidateBusinessRules(customer);

                        // Check for duplicates in import data
                        var duplicateCode = customers
                            .Take(i)
                            .Any(c => c.CustomerCode?.Equals(customer.CustomerCode, StringComparison.OrdinalIgnoreCase) == true);

                        if (duplicateCode)
                            errors.Add($"Dòng {rowNumber}: Mã khách hàng '{customer.CustomerCode}' bị trùng trong dữ liệu import");

                        var duplicateEmail = customers
                            .Take(i)
                            .Any(c => !string.IsNullOrWhiteSpace(c.Email) &&
                                     c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase));

                        if (duplicateEmail)
                            errors.Add($"Dòng {rowNumber}: Email '{customer.Email}' bị trùng trong dữ liệu import");

                        // Check database duplicates
                        if (customerDAL.IsCustomerCodeExists(customer.CustomerCode))
                            errors.Add($"Dòng {rowNumber}: Mã khách hàng '{customer.CustomerCode}' đã tồn tại trong hệ thống");

                        if (!string.IsNullOrWhiteSpace(customer.Email) && customerDAL.IsEmailExists(customer.Email))
                            errors.Add($"Dòng {rowNumber}: Email '{customer.Email}' đã tồn tại trong hệ thống");
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

        /// <summary>
        /// Import danh sách khách hàng
        /// </summary>
        public (int SuccessCount, int ErrorCount, List<string> Errors) ImportCustomers(List<Customer> customers)
        {
            var errors = new List<string>();
            int successCount = 0;
            int errorCount = 0;

            try
            {
                // Validate all data first
                var validationErrors = ValidateImportData(customers);
                if (validationErrors.Any())
                {
                    return (0, customers.Count, validationErrors);
                }

                // Import data
                for (int i = 0; i < customers.Count; i++)
                {
                    try
                    {
                        var customer = customers[i];
                        customer.Status = string.IsNullOrWhiteSpace(customer.Status) ? "Đang hợp tác" : customer.Status;

                        if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                            customer.CustomerCode = customerDAL.GenerateCustomerCode();

                        customerDAL.InsertCustomer(customer);
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
                errorCount = customers.Count;
            }

            return (successCount, errorCount, errors);
        }

        #endregion
    }
}