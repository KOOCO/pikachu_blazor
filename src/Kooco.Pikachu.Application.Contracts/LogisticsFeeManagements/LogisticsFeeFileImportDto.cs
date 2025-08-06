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
        public LogisticsFileType FileType { get; set; }
        public FileProcessingStatus BatchStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulDeductions { get; set; }
        public int FailedDeductions { get; set; }
        public DateTime UploadDate { get; set; }
        public Guid UploadedByUserId { get; set; }
        public string ProcessingNotes { get; set; }
        public DateTime? ProcessingStartedAt { get; set; }
        public DateTime? ProcessingCompletedAt { get; set; }
        public List<TenantLogisticsFeeFileProcessingSummaryDto> TenantSummaries { get; set; }
    }
}
