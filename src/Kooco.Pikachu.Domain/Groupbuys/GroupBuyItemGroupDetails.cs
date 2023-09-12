using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyItemGroupDetails : Entity<Guid>
    {
        public Guid GroupBuyItemGroupId { get; set; }
        public int SortOrder { get; set; }
        public string? ItemDescription { get; set; }

        public Guid? ItemId { get; set; }
        public Item Item { get; set; }

        public Guid? ImageId { get; set; }
        public Image Image { get; set; }

        public GroupBuyItemGroupDetails()
        {
            
        }

        public GroupBuyItemGroupDetails(
            Guid id,
            Guid groupBuyItemGroupId,
            int sortOrder,
            string? itemDesciption,
            Guid? itemId,
            Guid? imageId
            ) : base(id)
        {
            GroupBuyItemGroupId = groupBuyItemGroupId;
            SortOrder = sortOrder;
            ItemDescription = itemDesciption;
            ItemId = itemId;
            ImageId = imageId;
        }
    }
}
