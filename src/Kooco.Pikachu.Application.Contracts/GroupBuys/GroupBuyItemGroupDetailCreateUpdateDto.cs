using Kooco.Pikachu.Images;
using System;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDetailCreateUpdateDto
    {
        public Guid GroupBuyItemGroupId { get; set; }
        public Guid ItemId { get; set; }
        public int SortOrder { get; set; }
    }
}
