using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.DTO
{

    // Display model for DataGridView
    public class DocumentDisplayModel
    {
        public int DocumentID { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string RelatedTo { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool FileExists { get; set; }
    }

    // Enum for document types
    public static class DocumentTypes
    {
        public static readonly string[] Types = {
            "Contract",     // Hợp đồng
            "Report",       // Báo cáo
            "Invoice",      // Hóa đơn
            "Proposal",     // Đề xuất
            "Manual",       // Hướng dẫn
            "Certificate",  // Chứng chỉ
            "Image",        // Hình ảnh
            "Other"         // Khác
        };

        public static string GetDisplayName(string type)
        {
            return type switch
            {
                "Contract" => "📋 Hợp đồng",
                "Report" => "📊 Báo cáo",
                "Invoice" => "💰 Hóa đơn",
                "Proposal" => "📄 Đề xuất",
                "Manual" => "📖 Hướng dẫn",
                "Certificate" => "🏆 Chứng chỉ",
                "Image" => "🖼️ Hình ảnh",
                "Other" => "📁 Khác",
                _ => type
            };
        }
    }

}
