using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class SetItemDetails : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid SetItemId { get; set; }
        public SetItem SetItem { get; set; }
        public Item Item { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Attribute1Value { get; set; }
        public string? Attribute2Value { get; set; }
        public string? Attribute3Value { get; set; }
        protected SetItemDetails()
        {
        }

        public SetItemDetails(
            Guid id,
            Guid? tenantId,
            Guid setItemId,
            Guid itemId,
            int quantity,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value
        ) : base(id)
        {
            TenantId = tenantId;
            SetItemId = setItemId;
            ItemId = itemId;
            Quantity = quantity;
            Attribute1Value = attribute1Value;
            Attribute2Value = attribute2Value;
            Attribute3Value = attribute3Value;
        }
    }
}
