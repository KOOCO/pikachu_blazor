using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupPurchaseOverviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyItemGroupModuleDetailsDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid GroupBuyId { get; set; }
    public int SortOrder { get; set; }
    public GroupBuyModuleType GroupBuyModuleType { get; set; }
    public string GroupBuyModuleTypeName { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? ProductGroupModuleTitle { get; set; }
    public string? ProductGroupModuleImageSize { get; set; }
    public ICollection<GroupBuyItemGroupDetailsDto> ItemGroupDetails { get; set; }
    public List<List<string>> CarouselModulesImages { get; set; }
    public List<List<string>> BannerModulesImages { get; set; }
    public List<GroupPurchaseOverviewDto> GroupPurchaseOverviewModules { get; set; }
    public List<GroupBuyOrderInstructionDto> GetGroupBuyOrderInstructionModules { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
