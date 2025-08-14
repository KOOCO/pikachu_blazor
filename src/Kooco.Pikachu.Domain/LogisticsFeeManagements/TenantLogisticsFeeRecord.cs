using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Tenants.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class TenantLogisticsFeeRecord : FullAuditedEntity<Guid>
    {
        public Guid? TenantId { get; set; }
        public string OrderNumber { get; set; }
        public decimal LogisticFee { get; set; }
        public WalletDeductionStatus DeductionStatus { get; set; }
        public DateTime? DeductionDate { get; set; }
        public string? FailureReason { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime? LastRetryDate { get; set; }
        public LogisticsFileType FileType { get; set; } // ECPay or TCAT (denormalized for easy filtering)
        public DateTime ProcessedAt { get; set; }

        // Foreign Keys
        public Guid FileImportId { get; set; }
        public Guid? TenantWalletTransactionId { get; set; }

        // Navigation properties
        public LogisticsFeeFileImport? LogisticsFeeFileImport { get; set; }
        public TenantWalletTransaction? TenantWalletTransaction { get; set; }
        public TenantLogisticsFeeRecord() { }

        public TenantLogisticsFeeRecord(
            Guid id,
            Guid fileImportId,
            Guid tenantId,
            string orderNumber,
            decimal logisticFee,
            LogisticsFileType fileType
        ) : base(id)
        {
            FileImportId = fileImportId;
            TenantId = tenantId;
            OrderNumber = orderNumber;
            LogisticFee = logisticFee;
            FileType = fileType;
            DeductionStatus = WalletDeductionStatus.Pending;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkAsDeducted(Guid walletTransactionId)
        {
            DeductionStatus = WalletDeductionStatus.Completed;
            TenantWalletTransactionId = walletTransactionId;
            FailureReason = null;
        }

        public void MarkAsFailed(string reason)
        {
            DeductionStatus = WalletDeductionStatus.Failed;
            FailureReason = reason;
        }
    }
}
