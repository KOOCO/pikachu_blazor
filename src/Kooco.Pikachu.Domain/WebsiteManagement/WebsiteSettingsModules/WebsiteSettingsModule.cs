using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsModule : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid WebsiteSettingsId { get; set; }
    public int SortOrder { get; set; }
    public GroupBuyModuleType GroupBuyModuleType { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? ProductGroupModuleTitle { get; set; }
    public string? ProductGroupModuleImageSize { get; set; }
    public int? ModuleNumber { get; set; }

    [ForeignKey(nameof(WebsiteSettingsId))]
    public virtual WebsiteSettings WebsiteSettings { get; set; }

    public virtual ICollection<WebsiteSettingsModuleItem> ModuleItems { get; set; }

    public WebsiteSettingsModule(
        Guid id,
        Guid websiteSettingsId,
        int sortOrder,
        GroupBuyModuleType groupBuyModuleType,
        string? additionalInfo,
        string? productGroupModuleTitle,
        string? productGroupModuleImageSize,
        int? moduleNumber
        ) : base(id)
    {
        WebsiteSettingsId = websiteSettingsId;
        SortOrder = sortOrder;
        GroupBuyModuleType = groupBuyModuleType;
        AdditionalInfo = additionalInfo;
        ProductGroupModuleTitle = productGroupModuleTitle;
        ProductGroupModuleImageSize = productGroupModuleImageSize;
        ModuleNumber = moduleNumber;
        ModuleItems = new List<WebsiteSettingsModuleItem>();
    }

    public WebsiteSettingsModuleItem AddModuleItem(Guid id, Guid? itemId, Guid? setItemId, ItemType itemType, int sortOrder,
        string? displayText, int? moduleNumber)
    {
        var moduleItem = new WebsiteSettingsModuleItem(id, Id, itemId, setItemId, itemType, sortOrder, displayText, moduleNumber);
        ModuleItems.Add(moduleItem);
        return moduleItem;
    }
}
