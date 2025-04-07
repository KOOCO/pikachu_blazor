using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupCreateUpdateDto
    {
        public Guid? Id { get; set; }

        public Guid? TenantId { get; set; }
        public Guid GroupBuyId { get; set; }

        public int SortOrder { get; set; }

        public GroupBuyModuleType GroupBuyModuleType { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? ProductGroupModuleTitle { get; set; }
        public string? ProductGroupModuleImageSize { get; set; }
        public int? ModuleNumber { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public ICollection<GroupBuyItemGroupDetailCreateUpdateDto> ItemDetails { get; set; }

        public GroupBuyItemGroupCreateUpdateDto()
        {
            ItemDetails = new List<GroupBuyItemGroupDetailCreateUpdateDto>();
        }
    }
}
