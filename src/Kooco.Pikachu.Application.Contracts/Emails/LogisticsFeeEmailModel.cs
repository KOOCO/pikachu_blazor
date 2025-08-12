using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Emails
{
    public class LogisticsFeeEmailModel
    {
        public string Email { get; set; }
        public string TenantName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime ProcessingDate { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulDeductions { get; set; }
        public int FailedDeductions { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
