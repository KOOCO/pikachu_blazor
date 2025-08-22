using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantDeliveryFeeGetListInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; } // Filter by name or description
        public Guid? TenantId { get; set; }                 // Host can pass; tenant can omit
        public DeliveryProvider? DeliveryProvider { get; set; }
        public bool? IsEnabled { get; set; }
        public FeeKind? FeeKind { get; set; }
    }
}
