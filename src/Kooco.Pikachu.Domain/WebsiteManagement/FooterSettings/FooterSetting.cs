using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public ICollection<FooterSettingSection> Sections { get; set; }

    public FooterSetting(Guid id) : base(id)
    {
        Sections = new List<FooterSettingSection>();
    }

    public FooterSettingSection AddSection(Guid id, FooterSettingsPosition footerSettingsPosition, string title,
        FooterSettingsType footerSettingsType, string text, string imageUrl, string imageName)
    {
        var section = new FooterSettingSection(id, footerSettingsPosition, title,
            footerSettingsType, text, imageUrl, imageName, Id);
        Sections.Add(section);
        return section;
    }
}
