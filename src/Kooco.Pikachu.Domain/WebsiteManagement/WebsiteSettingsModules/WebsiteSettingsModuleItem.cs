using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsModuleItem : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid WebsiteSettingsModuleId { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public ItemType ItemType { get; set; }
    public int SortOrder { get; set; }
    public string? DisplayText { get; set; }
    public int? ModuleNumber { get; set; }

    [ForeignKey(nameof(ItemId))]
    public virtual Item? Item { get; set; }

    [ForeignKey(nameof(SetItemId))]
    public virtual SetItem? SetItem { get; set; }

    [ForeignKey(nameof(WebsiteSettingsModuleId))]
    public virtual WebsiteSettingsModule WebsiteSettingsModule { get; set; }

    public WebsiteSettingsModuleItem(
        Guid id,
        Guid websiteSettingsModuleId,
        Guid? itemId,
        Guid? setItemId,
        ItemType itemType,
        int sortOrder,
        string? displayText,
        int? moduleNumber
        ) : base(id)
    {
        WebsiteSettingsModuleId = websiteSettingsModuleId;
        ItemId = itemId;
        SetItemId = setItemId;
        ItemType = itemType;
        SortOrder = sortOrder;
        DisplayText = displayText;
        ModuleNumber = moduleNumber;
    }
}
