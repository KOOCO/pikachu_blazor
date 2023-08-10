using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class SetItemDetails : FullAuditedAggregateRoot<Guid>,IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public SetItem SetItem { get; set; }
        public Item Item { get; set; }
    }
}
