using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.DeliveryTempratureCosts
{
    public class DeliveryTemperatureCost : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public ItemStorageTemperature Temperature { get; set; }
        public decimal Cost { get; set; }
        public Guid? TenantId { get; set; }
    }
}
