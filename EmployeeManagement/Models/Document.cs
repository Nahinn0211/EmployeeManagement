using System;
using System.IO;

namespace EmployeeManagement.Models
{
    public class Document
    {
        public int DocumentID { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public int? ProjectID { get; set; }
        public int? EmployeeID { get; set; }
        public int? CustomerID { get; set; }
        public int UploadedByID { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public User UploadedBy { get; set; }

        // Computed properties
        public string FileSize { get; set; } = "0 KB";
        public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string UpdatedAtDisplay => UpdatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FileTypeDisplay => GetFileTypeDisplay(FileType);
        public string DocumentTypeDisplay => GetDocumentTypeDisplay(DocumentType);
        public string RelatedToDisplay => GetRelatedToDisplay();
        public bool FileExists => !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath);

        // Helper methods
        private string GetFileTypeDisplay(string fileType)
        {
            return fileType?.ToUpper() switch
            {
                "PDF" => "📄 PDF",
                "DOC" or "DOCX" => "📝 Word",
                "XLS" or "XLSX" => "📊 Excel",
                "PPT" or "PPTX" => "📈 PowerPoint",
                "JPG" or "JPEG" or "PNG" or "GIF" => "🖼️ Hình ảnh",
                "ZIP" or "RAR" or "7Z" => "📦 Nén",
                "TXT" => "📄 Text",
                _ => "📁 " + fileType?.ToUpper()
            };
        }

        private string GetDocumentTypeDisplay(string docType)
        {
            return docType switch
            {
                "Contract" => "📋 Hợp đồng",
                "Report" => "📊 Báo cáo",
                "Invoice" => "💰 Hóa đơn",
                "Proposal" => "📄 Đề xuất",
                "Manual" => "📖 Hướng dẫn",
                "Certificate" => "🏆 Chứng chỉ",
                "Image" => "🖼️ Hình ảnh",
                "Other" => "📁 Khác",
                _ => docType
            };
        }

        private string GetRelatedToDisplay()
        {
            if (ProjectID.HasValue && Project != null)
                return $"Dự án: {Project.ProjectName}";
            if (CustomerID.HasValue && Customer != null)
                return $"Khách hàng: {Customer.CompanyName}";
            if (EmployeeID.HasValue && Employee != null)
                return $"Nhân viên: {Employee.FullName}";
            return "Chung";
        }

        public string GetFileExtension()
        {
            return string.IsNullOrEmpty(FilePath) ? "" : Path.GetExtension(FilePath).TrimStart('.');
        }

        public string GetFileName()
        {
            return string.IsNullOrEmpty(FilePath) ? "" : Path.GetFileName(FilePath);
        }

        public long GetFileSizeInBytes()
        {
            try
            {
                if (FileExists)
                {
                    var fileInfo = new FileInfo(FilePath);
                    return fileInfo.Length;
                }
            }
            catch { }
            return 0;
        }

        public string GetFormattedFileSize()
        {
            var bytes = GetFileSizeInBytes();
            if (bytes == 0) return "0 KB";

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }

}