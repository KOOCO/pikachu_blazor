using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class LogisticsFeeFileImport : FullAuditedAggregateRoot<Guid>
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

        // Navigation properties
        public ICollection<TenantLogisticsFeeRecord>? TenantLogisticsFeeRecords { get; set; }
        public ICollection<TenantLogisticsFeeFileProcessingSummary>? LogisticsFeeTenantSummaries { get; set; }
        protected LogisticsFeeFileImport()
        {
            LogisticsFeeTenantSummaries = new List<TenantLogisticsFeeFileProcessingSummary>();
            TenantLogisticsFeeRecords = new List<TenantLogisticsFeeRecord>();
        }

        public LogisticsFeeFileImport(
            Guid id,
            string fileName,
            string originalFileName,
            string filePath,
            LogisticsFileType fileType,
            Guid uploadedByUserId
        ) : base(id)
        {
            FileName = fileName;
            OriginalFileName = originalFileName;
            FilePath = filePath;
            FileType = fileType;
            BatchStatus = FileProcessingStatus.Processing;
            UploadDate = DateTime.UtcNow;
            UploadedByUserId = uploadedByUserId;
            LogisticsFeeTenantSummaries = new List<TenantLogisticsFeeFileProcessingSummary>();
            TenantLogisticsFeeRecords = new List<TenantLogisticsFeeRecord>();
        }

        public void StartProcessing()
        {
            BatchStatus = FileProcessingStatus.Processing;
            ProcessingStartedAt = DateTime.UtcNow;
        }

        public void SuccessProcessing(string notes = null)
        {
            BatchStatus = FileProcessingStatus.BatchSuccess;
            ProcessingCompletedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(notes))
            {
                ProcessingNotes = notes;
            }
        }

        public void FailProcessing(string reason)
        {
            BatchStatus = FileProcessingStatus.BatchFailed;
            ProcessingCompletedAt = DateTime.UtcNow;
            ProcessingNotes = reason;
        }
        public void PartialSuccessProcessing(string reason)
        {
            BatchStatus = FileProcessingStatus.BatchFailed;
            ProcessingCompletedAt = DateTime.UtcNow;
            ProcessingNotes = reason;
        }
    }
}