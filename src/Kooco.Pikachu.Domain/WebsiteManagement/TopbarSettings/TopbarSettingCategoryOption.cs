using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingCategoryOption : Entity<Guid>, IMultiTenant
{
    public TopbarCategoryLinkOption TopbarCategoryLinkOption { get; set; }
    public int Index { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public Guid TopbarSettingLinkId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TopbarSettingLinkId))]
    public virtual TopbarSettingLink TopbarSettingLink { get; set; }

    public TopbarSettingCategoryOption(
        Guid id,
        TopbarCategoryLinkOption topbarCategoryLinkOption,
        int index,
        string title,
        string link,
        Guid topbarSettingLinkId
        ) : base(id)
    {
        TopbarCategoryLinkOption = topbarCategoryLinkOption;
        Index = index;
        TopbarSettingLinkId = topbarSettingLinkId;
        SetTitle(title);
        SetLink(link);
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(Title), TopbarSettingsConsts.MaxTitleLength);
    }

    public void SetLink(string link)
    {
        Link = Check.NotNullOrWhiteSpace(link, nameof(Link), TopbarSettingsConsts.MaxUrlLength);
    }
}