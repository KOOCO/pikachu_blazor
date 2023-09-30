using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using System;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDetailsDto
    {
        public Guid GroupBuyItemGroupId { get; set; }
        public Guid ItemId { get; set; }
        public int SortOrder { get; set; }
        public ItemDto Item { get; set; }
    }
}
