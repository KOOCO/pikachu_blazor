using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class LogisticsFeeFileImportDto : FullAuditedEntityDto<Guid>
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public LogisticsFileType FileType { get; set; } // ECPay or TCAT
        public int TotalRecords { get; set; }
        public decimal TotalAmount { get; set; }
        public FileProcessingStatus BatchStatus { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public decimal SuccessRate => TotalRecords > 0 ? (decimal)SuccessfulRecords / TotalRecords * 100 : 0;
        public string? ProcessingNotes { get; set; }
        public Guid UploadedByUserId { get; set; } // Who uploaded the file
        public DateTime? ProcessingStartedAt { get; set; }
        public DateTime? ProcessingCompletedAt { get; set; }
        public List<TenantLogisticsFeeFileProcessingSummaryDto> TenantSummaries { get; set; }
    }
}
