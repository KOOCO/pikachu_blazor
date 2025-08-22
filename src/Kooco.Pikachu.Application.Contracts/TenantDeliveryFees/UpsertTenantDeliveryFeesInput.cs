using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class UpsertTenantDeliveryFeesInput
    {
        public Guid? TenantId { get; set; }
        public List<TenantDeliveryFeeItemInput> Items { get; set; } = new();
    }
}
