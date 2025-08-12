using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class TenantLogisticsFeeRecordDto : FullAuditedEntityDto<Guid>
    {
        public Guid FileImportId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string OrderNumber { get; set; }
        public decimal LogisticFee { get; set; }
        public LogisticsFileType FileType { get; set; }
        public WalletDeductionStatus DeductionStatus { get; set; }
        public string FailureReason { get; set; }
        public DateTime ProcessedAt { get; set; }
        public Guid? TenantWalletTransactionId { get; set; }
        public DateTime? DeductionDate { get; set; }
    }
}
