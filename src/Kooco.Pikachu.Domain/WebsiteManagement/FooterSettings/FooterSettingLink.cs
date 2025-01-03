using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSettingLink : Entity<Guid>, IMultiTenant
{
    public int Index { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public Guid SectionId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(SectionId))]
    public virtual FooterSettingSection Section { get; set; }

    public FooterSettingLink(
        Guid id,
        int index,
        string title,
        string url,
        Guid sectionId
        ) : base(id)
    {
        Index = index;
        Url = url;
        SectionId = sectionId;
        SetTitle(title);
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(Title), maxLength: FooterSettingsConsts.MaxTitleLength);
    }
}