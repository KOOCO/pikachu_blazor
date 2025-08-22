using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantLogisticsFeeRowDto : EntityDto<Guid>
    {
        public string TenantName { get; set; } = default!;
        public bool LogisticsFeeStatus { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
