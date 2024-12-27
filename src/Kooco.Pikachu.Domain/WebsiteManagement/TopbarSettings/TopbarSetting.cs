using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public TopbarStyleSettings TopbarStyleSettings { get; set; }
    public Guid? TenantId { get; set; }

    public virtual ICollection<TopbarSettingLink> Links { get; set; }

    public TopbarSetting(
        Guid id,
        TopbarStyleSettings topbarStyleSettings
        ) : base(id)
    {
        TopbarStyleSettings = topbarStyleSettings;
        Links = new List<TopbarSettingLink>();
    }

    public TopbarSettingLink AddLink(Guid id, TopbarLinkSettings topbarLinkSettings, int index, string title, string? url)
    {
        var link = new TopbarSettingLink(id, topbarLinkSettings, index, title, url);
        Links.Add(link);
        return link;
    }
}
