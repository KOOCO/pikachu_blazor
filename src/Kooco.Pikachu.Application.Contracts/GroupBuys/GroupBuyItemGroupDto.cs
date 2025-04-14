using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDto : EntityDto<Guid>, IHasConcurrencyStamp
    {
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
        public string? Url { get; set; }
        public ICollection<GroupBuyItemGroupDetailsDto> ItemGroupDetails { get; set; }
        public ICollection<GroupBuyItemGroupImageModuleDto> ImageModules { get; set; } = [];
        public string? ConcurrencyStamp { get; set; }
    }
}
