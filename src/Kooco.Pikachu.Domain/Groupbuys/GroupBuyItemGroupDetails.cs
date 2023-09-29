using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyItemGroupDetails : Entity<Guid>
    {
        public Guid GroupBuyItemGroupId { get; set; }
        public Guid ItemId { get; set; }
        public int SortOrder { get; set; }

        public GroupBuyItemGroupDetails()
        {
            
        }

        public GroupBuyItemGroupDetails(
            Guid id,
            Guid groupBuyItemGroupId,
            int sortOrder,
            Guid itemId
            ) : base(id)
        {
            GroupBuyItemGroupId = groupBuyItemGroupId;
            SortOrder = sortOrder;
            ItemId = itemId;
        }
    }
}
