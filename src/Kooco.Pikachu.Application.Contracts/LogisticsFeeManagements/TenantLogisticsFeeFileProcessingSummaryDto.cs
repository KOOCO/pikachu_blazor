using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int TenantSuccessfulRecords { get; set; }
        public int TenantFailedRecords { get; set; }
        public decimal TenantTotalAmount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public Guid WalletId { get; set; }
       
        
        public decimal WalletBalance { get; set; }
    }
}
