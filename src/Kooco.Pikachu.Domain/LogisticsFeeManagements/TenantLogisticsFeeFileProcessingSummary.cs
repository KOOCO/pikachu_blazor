using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class TenantLogisticsFeeFileProcessingSummary : FullAuditedEntity<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid FileImportId { get; set; }
        public int TenantTotalRecords { get; set; }
        public decimal TenantTotalAmount { get; set; }
        public int TenantSuccessfulRecords { get; set; }
        public int TenantFailedRecords { get; set; }
        public DateTime ProcessedAt { get; set; }
        public decimal TenantSuccessRate => TenantTotalRecords > 0 ? (decimal)TenantSuccessfulRecords / TenantTotalRecords * 100 : 0;
        public FileProcessingStatus TenantBatchStatus { get; set; }
        [NotMapped]
        public Guid WalletId { get; set; }
        [NotMapped]
        public string TenantName { get; set; }
        [NotMapped]
        public decimal WalletBalance { get; set; }
        // Navigation properties
        public LogisticsFeeFileImport? LogisticsFeeFileImport { get; set; }

        public TenantLogisticsFeeFileProcessingSummary() { }

        public TenantLogisticsFeeFileProcessingSummary(
            Guid id,
            Guid fileImportId,
            Guid tenantId
        ) : base(id)
        {
            FileImportId = fileImportId;
            TenantId = tenantId;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}