using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingLink : Entity<Guid>, IMultiTenant
{
    public TopbarLinkSettings TopbarLinkSettings { get; set; }
    public int Index { get; set; }
    public string Title { get; private set; }
    public string? Url { get; private set; }
    public Guid TopbarSettingId { get; set; }
    public Guid? TenantId { get; set; }

    public virtual ICollection<TopbarSettingCategoryOption> CategoryOptions { get; set; }

    [ForeignKey(nameof(TopbarSettingId))]
    public virtual TopbarSetting TopbarSetting { get; set; }

    public TopbarSettingLink(
        Guid id,
        TopbarLinkSettings topbarLinkSettings,
        int index,
        string title,
        string? url,
        Guid topbarSettingId
        ) : base(id)
    {
        TopbarLinkSettings = topbarLinkSettings;
        Index = index;
        TopbarSettingId = topbarSettingId;
        SetTitle(title);
        SetUrl(url);
        CategoryOptions = new List<TopbarSettingCategoryOption>();
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(Title), TopbarSettingsConsts.MaxTitleLength);
    }

    public void SetUrl(string? url)
    {
        if (TopbarLinkSettings == TopbarLinkSettings.Link)
        {
            Url = Check.NotNullOrWhiteSpace(url, nameof(Url), TopbarSettingsConsts.MaxUrlLength);
        }
    }

    public TopbarSettingCategoryOption AddCategoryOption(Guid id, TopbarCategoryLinkOption topbarCategoryLinkOption, int index, string title, string link)
    {
        var categoryOption = new TopbarSettingCategoryOption(id, topbarCategoryLinkOption, index, title, link, Id);
        CategoryOptions.Add(categoryOption);
        return categoryOption;
    }
}