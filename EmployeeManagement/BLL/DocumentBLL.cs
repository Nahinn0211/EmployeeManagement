using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EmployeeManagement.DAL;
using EmployeeManagement.Models.DTO;
using EmployeeManagement.Models.Entity;

namespace EmployeeManagement.BLL
{
    public class DocumentBLL
    {
        #region Fields
        private readonly DocumentDAL documentDAL;
        private readonly string documentStoragePath;
        #endregion

        #region Constructor
        public DocumentBLL()
        {
            documentDAL = new DocumentDAL();
            // Đường dẫn lưu trữ tài liệu (có thể config trong app.config)
            documentStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents");
            EnsureDocumentDirectory();
        }
        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy tất cả tài liệu
        /// </summary>
        public List<Document> GetAllDocuments()
        {
            try
            {
                return documentDAL.GetAllDocuments();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy danh sách tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tài liệu theo ID
        /// </summary>
        public Document GetDocumentById(int documentId)
        {
            try
            {
                if (documentId <= 0)
                    throw new ArgumentException("ID tài liệu không hợp lệ");

                return documentDAL.GetDocumentById(documentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thông tin tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm tài liệu mới với file upload
        /// </summary>
        public int AddDocument(Document document, string sourceFilePath = null)
        {
            try
            {
                // Validate business rules
                ValidateDocumentForInsert(document);

                // Generate document code if not provided
                if (string.IsNullOrWhiteSpace(document.DocumentCode))
                {
                    document.DocumentCode = documentDAL.GenerateDocumentCode();
                }

                // Handle file upload
                if (!string.IsNullOrEmpty(sourceFilePath) && File.Exists(sourceFilePath))
                {
                    string destinationPath = SaveDocumentFile(sourceFilePath, document.DocumentCode);
                    document.FilePath = destinationPath;
                    document.FileType = Path.GetExtension(sourceFilePath).TrimStart('.').ToUpper();
                }

                // Set default values
                document.CreatedAt = DateTime.Now;
                document.UpdatedAt = DateTime.Now;

                return documentDAL.InsertDocument(document);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi thêm tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài liệu
        /// </summary>
        public bool UpdateDocument(Document document, string sourceFilePath = null)
        {
            try
            {
                // Validate business rules
                ValidateDocumentForUpdate(document);

                // Handle file replacement
                if (!string.IsNullOrEmpty(sourceFilePath) && File.Exists(sourceFilePath))
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
                    {
                        try
                        {
                            File.Delete(document.FilePath);
                        }
                        catch { } // Ignore deletion errors
                    }

                    // Save new file
                    string destinationPath = SaveDocumentFile(sourceFilePath, document.DocumentCode);
                    document.FilePath = destinationPath;
                    document.FileType = Path.GetExtension(sourceFilePath).TrimStart('.').ToUpper();
                }

                document.UpdatedAt = DateTime.Now;

                return documentDAL.UpdateDocument(document);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi cập nhật tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        public bool DeleteDocument(int documentId)
        {
            try
            {
                if (documentId <= 0)
                    throw new ArgumentException("ID tài liệu không hợp lệ");

                // Get document info to delete file
                var document = documentDAL.GetDocumentById(documentId);
                if (document == null)
                    throw new Exception("Tài liệu không tồn tại");

                // Delete from database first
                bool deleted = documentDAL.DeleteDocument(documentId);

                // Delete physical file if database deletion successful
                if (deleted && !string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
                {
                    try
                    {
                        File.Delete(document.FilePath);
                    }
                    catch
                    {
                        // Log warning but don't fail the operation
                    }
                }

                return deleted;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi xóa tài liệu: {ex.Message}", ex);
            }
        }

        #endregion

        #region File Management

        /// <summary>
        /// Lưu file tài liệu vào thư mục storage
        /// </summary>
        private string SaveDocumentFile(string sourceFilePath, string documentCode)
        {
            try
            {
                string fileName = $"{documentCode}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(sourceFilePath)}";
                string destinationPath = Path.Combine(documentStoragePath, fileName);

                File.Copy(sourceFilePath, destinationPath, true);

                return destinationPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đảm bảo thư mục lưu trữ tồn tại
        /// </summary>
        private void EnsureDocumentDirectory()
        {
            try
            {
                if (!Directory.Exists(documentStoragePath))
                {
                    Directory.CreateDirectory(documentStoragePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể tạo thư mục lưu trữ tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mở file tài liệu
        /// </summary>
        public bool OpenDocument(int documentId)
        {
            try
            {
                var document = documentDAL.GetDocumentById(documentId);
                if (document == null)
                    throw new Exception("Tài liệu không tồn tại");

                if (string.IsNullOrEmpty(document.FilePath) || !File.Exists(document.FilePath))
                    throw new Exception("File tài liệu không tồn tại");

                System.Diagnostics.Process.Start(document.FilePath);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi mở tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Download file tài liệu
        /// </summary>
        public bool DownloadDocument(int documentId, string destinationPath)
        {
            try
            {
                var document = documentDAL.GetDocumentById(documentId);
                if (document == null)
                    throw new Exception("Tài liệu không tồn tại");

                if (string.IsNullOrEmpty(document.FilePath) || !File.Exists(document.FilePath))
                    throw new Exception("File tài liệu không tồn tại");

                string fileName = $"{document.DocumentName}{Path.GetExtension(document.FilePath)}";
                string fullDestinationPath = Path.Combine(destinationPath, fileName);

                File.Copy(document.FilePath, fullDestinationPath, true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi download tài liệu: {ex.Message}", ex);
            }
        }

        #endregion

        #region Business Validation

        /// <summary>
        /// Validate tài liệu khi thêm mới
        /// </summary>
        private void ValidateDocumentForInsert(Document document)
        {
            // Validate required fields
            ValidateRequiredFields(document);

            // Validate business rules
            ValidateBusinessRules(document);

            // Check duplicates
            if (!string.IsNullOrWhiteSpace(document.DocumentCode) &&
                documentDAL.IsDocumentCodeExists(document.DocumentCode))
            {
                throw new Exception("Mã tài liệu đã tồn tại trong hệ thống");
            }
        }

        /// <summary>
        /// Validate tài liệu khi cập nhật
        /// </summary>
        private void ValidateDocumentForUpdate(Document document)
        {
            if (document.DocumentID <= 0)
                throw new ArgumentException("ID tài liệu không hợp lệ");

            // Validate required fields
            ValidateRequiredFields(document);

            // Validate business rules
            ValidateBusinessRules(document);

            // Check duplicates (exclude current document)
            if (!string.IsNullOrWhiteSpace(document.DocumentCode) &&
                documentDAL.IsDocumentCodeExists(document.DocumentCode, document.DocumentID))
            {
                throw new Exception("Mã tài liệu đã tồn tại trong hệ thống");
            }
        }

        /// <summary>
        /// Validate các trường bắt buộc
        /// </summary>
        private void ValidateRequiredFields(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document), "Thông tin tài liệu không được để trống");

            if (string.IsNullOrWhiteSpace(document.DocumentName))
                throw new Exception("Tên tài liệu không được để trống");

            if (string.IsNullOrWhiteSpace(document.DocumentCode))
                throw new Exception("Mã tài liệu không được để trống");

            if (document.UploadedByID <= 0)
                throw new Exception("Người upload không hợp lệ");
        }

        /// <summary>
        /// Validate các quy tắc nghiệp vụ
        /// </summary>
        private void ValidateBusinessRules(Document document)
        {
            // Validate document code format
            if (!string.IsNullOrWhiteSpace(document.DocumentCode))
            {
                if (document.DocumentCode.Length < 3)
                    throw new Exception("Mã tài liệu phải có ít nhất 3 ký tự");

                if (document.DocumentCode.Length > 20)
                    throw new Exception("Mã tài liệu không được vượt quá 20 ký tự");

                if (!Regex.IsMatch(document.DocumentCode, @"^[A-Za-z0-9]+$"))
                    throw new Exception("Mã tài liệu chỉ được chứa chữ cái và số");
            }

            // Validate document name
            if (!string.IsNullOrWhiteSpace(document.DocumentName))
            {
                if (document.DocumentName.Length > 200)
                    throw new Exception("Tên tài liệu không được vượt quá 200 ký tự");
            }

            // Validate description
            if (!string.IsNullOrWhiteSpace(document.Description) && document.Description.Length > 500)
                throw new Exception("Mô tả không được vượt quá 500 ký tự");

            // Validate file path
            if (!string.IsNullOrWhiteSpace(document.FilePath) && document.FilePath.Length > 255)
                throw new Exception("Đường dẫn file quá dài");

            // Validate document type
            if (!string.IsNullOrWhiteSpace(document.DocumentType))
            {
                if (!DocumentTypes.Types.Contains(document.DocumentType))
                    throw new Exception("Loại tài liệu không hợp lệ");
            }

            // Validate file type
            if (!string.IsNullOrWhiteSpace(document.FileType))
            {
                var allowedTypes = new[] { "PDF", "DOC", "DOCX", "XLS", "XLSX", "PPT", "PPTX",
                                          "JPG", "JPEG", "PNG", "GIF", "ZIP", "RAR", "7Z", "TXT" };
                if (!allowedTypes.Contains(document.FileType.ToUpper()))
                    throw new Exception($"Loại file '{document.FileType}' không được hỗ trợ");
            }

            // Validate relationships (chỉ được liên kết với 1 trong 3: Project, Customer, Employee)
            int relationshipCount = 0;
            if (document.ProjectID.HasValue && document.ProjectID.Value > 0) relationshipCount++;
            if (document.CustomerID.HasValue && document.CustomerID.Value > 0) relationshipCount++;
            if (document.EmployeeID.HasValue && document.EmployeeID.Value > 0) relationshipCount++;

            if (relationshipCount > 1)
                throw new Exception("Tài liệu chỉ có thể liên kết với một đối tượng (Dự án, Khách hàng, hoặc Nhân viên)");
        }

        /// <summary>
        /// Validate file upload
        /// </summary>
        public bool ValidateFileUpload(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    throw new Exception("File không tồn tại");

                var fileInfo = new FileInfo(filePath);

                // Check file size (max 10MB)
                if (fileInfo.Length > 10 * 1024 * 1024)
                    throw new Exception("Kích thước file không được vượt quá 10MB");

                // Check file extension
                var extension = fileInfo.Extension.TrimStart('.').ToUpper();
                var allowedTypes = new[] { "PDF", "DOC", "DOCX", "XLS", "XLSX", "PPT", "PPTX",
                                          "JPG", "JPEG", "PNG", "GIF", "ZIP", "RAR", "7Z", "TXT" };

                if (!allowedTypes.Contains(extension))
                    throw new Exception($"Loại file '{extension}' không được hỗ trợ");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"File không hợp lệ: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search & Filter

        /// <summary>
        /// Tìm kiếm tài liệu theo điều kiện
        /// </summary>
        public List<Document> SearchDocuments(string searchText = "", string documentType = "",
            int? projectId = null, int? customerId = null, int? employeeId = null)
        {
            try
            {
                return documentDAL.SearchDocuments(searchText?.Trim(), documentType?.Trim(),
                    projectId, customerId, employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi tìm kiếm tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lọc tài liệu theo loại
        /// </summary>
        public List<Document> GetDocumentsByType(string documentType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentType))
                    return GetAllDocuments();

                return documentDAL.SearchDocuments("", documentType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lọc tài liệu theo loại: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tài liệu theo dự án
        /// </summary>
        public List<Document> GetDocumentsByProject(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("ID dự án không hợp lệ");

                return documentDAL.GetDocumentsByProject(projectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy tài liệu theo dự án: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tài liệu theo khách hàng
        /// </summary>
        public List<Document> GetDocumentsByCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ");

                return documentDAL.GetDocumentsByCustomer(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy tài liệu theo khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tài liệu theo nhân viên
        /// </summary>
        public List<Document> GetDocumentsByEmployee(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("ID nhân viên không hợp lệ");

                return documentDAL.GetDocumentsByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy tài liệu theo nhân viên: {ex.Message}", ex);
            }
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Tạo mã tài liệu tự động
        /// </summary>
        public string GenerateDocumentCode()
        {
            try
            {
                return documentDAL.GenerateDocumentCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra mã tài liệu có tồn tại không
        /// </summary>
        public bool IsDocumentCodeExists(string documentCode, int excludeDocumentId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentCode))
                    return false;

                return documentDAL.IsDocumentCodeExists(documentCode.Trim(), excludeDocumentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách cho dropdown
        /// </summary>
        public List<Project> GetProjectsForDropdown()
        {
            try
            {
                return documentDAL.GetProjectsForDropdown();
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
                return documentDAL.GetCustomersForDropdown();
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
                return documentDAL.GetEmployeesForDropdown();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }

        #endregion

        #region Statistics & Reports

        /// <summary>
        /// Lấy thống kê tài liệu
        /// </summary>
        public (int Total, int Projects, int Customers, int Employees, int General) GetDocumentStatistics()
        {
            try
            {
                return documentDAL.GetDocumentStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nghiệp vụ khi lấy thống kê: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê theo loại file
        /// </summary>
        public Dictionary<string, int> GetFileTypeStatistics()
        {
            try
            {
                var documents = GetAllDocuments();
                return documents
                    .Where(d => !string.IsNullOrEmpty(d.FileType))
                    .GroupBy(d => d.FileType.ToUpper())
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê loại file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê theo loại tài liệu
        /// </summary>
        public Dictionary<string, int> GetDocumentTypeStatistics()
        {
            try
            {
                var documents = GetAllDocuments();
                return documents
                    .Where(d => !string.IsNullOrEmpty(d.DocumentType))
                    .GroupBy(d => d.DocumentType)
                    .ToDictionary(g => DocumentTypes.GetDisplayName(g.Key), g => g.Count());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê loại tài liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra và làm sạch file không sử dụng
        /// </summary>
        public List<string> CleanupUnusedFiles()
        {
            var deletedFiles = new List<string>();

            try
            {
                if (!Directory.Exists(documentStoragePath))
                    return deletedFiles;

                var documents = GetAllDocuments();
                var usedFiles = documents.Where(d => !string.IsNullOrEmpty(d.FilePath))
                                        .Select(d => d.FilePath)
                                        .ToHashSet();

                var allFiles = Directory.GetFiles(documentStoragePath);

                foreach (var file in allFiles)
                {
                    if (!usedFiles.Contains(file))
                    {
                        try
                        {
                            File.Delete(file);
                            deletedFiles.Add(Path.GetFileName(file));
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi làm sạch file: {ex.Message}", ex);
            }

            return deletedFiles;
        }

        #endregion
    }
}