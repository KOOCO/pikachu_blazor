using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;


namespace Kooco.Pikachu.Items
{
    public class ItemTags : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
         public Guid? TenantId { get; set; }

        public Item? Item { get; set; }
        public SetItem? SetItem { get; set; }
        public string TagName { get; set; }
    }
}
