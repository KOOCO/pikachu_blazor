using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class TenantLogisticsFeeFileProcessingSummaryDto : FullAuditedEntityDto<Guid>
    {
        public Guid FileImportId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public int TenantRecordCount { get; set; }
        public int TenantSuccessfulDeductions { get; set; }
        public int TenantFailedDeductions { get; set; }
        public decimal TenantTotalAmount { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
