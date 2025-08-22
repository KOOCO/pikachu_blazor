using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantLogisticsFeeGetListInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; } // optional: filter by tenant name (Contains)
    }
}
