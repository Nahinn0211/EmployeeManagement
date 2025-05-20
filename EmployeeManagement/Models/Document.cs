using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Document
    {
        public int DocumentID { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }  // Contract, Report, Invoice, Other
        public string FilePath { get; set; }  // Đường dẫn đến tệp
        public int CreatedBy { get; set; }  // Foreign Key (User)
        public string Status { get; set; }  // Draft, Finalized, Archived
        public int? ProjectID { get; set; }  // Foreign Key (Project), nullable
        public int? CustomerID { get; set; }  // Foreign Key (Customer), nullable
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key References
        public virtual User Creator { get; set; }
        public virtual Project Project { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
