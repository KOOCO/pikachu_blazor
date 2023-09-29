using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupCreateUpdateDto
    {
        public Guid? Id { get; set; }

        public Guid GroupBuyId { get; set; }

        public int SortOrder { get; set; }

        public GroupBuyModuleType GroupBuyModuleType { get; set; }
        public ICollection<GroupBuyItemGroupDetailCreateUpdateDto> ItemDetails { get; set; }

        public GroupBuyItemGroupCreateUpdateDto()
        {
            ItemDetails = new List<GroupBuyItemGroupDetailCreateUpdateDto>();
        }
    }
}
