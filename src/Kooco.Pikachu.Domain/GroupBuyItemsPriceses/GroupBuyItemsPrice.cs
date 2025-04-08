using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class GroupBuyItemsPrice:FullAuditedAggregateRoot<Guid>
    {
        public Guid? SetItemId { get; set; }
        public Guid? GroupBuyId { get; set; }
        public float GroupBuyPrice { get; set; }
        public Guid? ItemDetailId { get; set; }
        // Constructor for EF Core
        private GroupBuyItemsPrice() { }

        // Custom constructor
        public GroupBuyItemsPrice(Guid id, Guid? setItemId, Guid? groupBuyId, float groupBuyPrice, Guid? itemDetailId)
            : base(id)
        {
            SetItemId = setItemId;
            GroupBuyId = groupBuyId;
            GroupBuyPrice = groupBuyPrice;
            ItemDetailId = itemDetailId;
        }

        // Update method for encapsulation
        public void Update(Guid? setItemId, Guid? groupBuyId, float groupBuyPrice, Guid? itemDetailId)
        {
            SetItemId = setItemId;
            GroupBuyId = groupBuyId;
            GroupBuyPrice = groupBuyPrice;
            ItemDetailId = itemDetailId;
        }

    }
}
