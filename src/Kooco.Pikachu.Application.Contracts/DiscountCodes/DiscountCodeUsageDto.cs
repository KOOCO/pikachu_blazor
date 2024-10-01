using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCodeUsageDto:AuditedEntityDto<Guid>
    {
        public Guid DiscountCodeId { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDiscountAmount { get; set; }
    }
}
