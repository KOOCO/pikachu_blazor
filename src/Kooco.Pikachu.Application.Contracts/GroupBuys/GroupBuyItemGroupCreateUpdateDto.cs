using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

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
        public string? Url { get; set; }
        public ICollection<GroupBuyItemGroupDetailCreateUpdateDto> ItemDetails { get; set; }
        public ICollection<GroupBuyItemGroupImageModuleDto> ImageModules { get; set; } = [];

        public GroupBuyItemGroupCreateUpdateDto()
        {
            ItemDetails = new List<GroupBuyItemGroupDetailCreateUpdateDto>();
        }
    }

    public class GroupBuyItemGroupImageModuleDto
    {
        public Guid Id { get; set; }
        public List<GroupBuyItemGroupImageDto> Images { get; set; }
    }

    public class GroupBuyItemGroupImageDto
    {
        public Guid Id { get; set; }
        public Guid GroupBuyItemGroupImageModuleId { get; set; }
        public string Url { get; set; }
        public string BlobImageName { get; set; }
        public int SortNo { get; set; }
    }
}
