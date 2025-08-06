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
    public class GetTenantLogisticsFeeRecordsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? FileImportId { get; set; }
        public LogisticsFileType? FileType { get; set; }
        public WalletDeductionStatus? Status { get; set; }
    }
}
