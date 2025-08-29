using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantLogisticsFeeOverviewItem
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = default!;
        public bool PaymentFeeStatus { get; set; }
        public bool LogisticsFeeStatus { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
