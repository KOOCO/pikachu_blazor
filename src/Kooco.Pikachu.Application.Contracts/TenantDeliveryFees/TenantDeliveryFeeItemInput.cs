using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantDeliveryFeeItemInput
    {
        public DeliveryProvider DeliveryProvider { get; set; }
        public bool IsEnabled { get; set; }
        public FeeKind FeeKind { get; set; }
        public decimal? PercentValue { get; set; }
        public decimal? FixedAmount { get; set; }
    }
}
