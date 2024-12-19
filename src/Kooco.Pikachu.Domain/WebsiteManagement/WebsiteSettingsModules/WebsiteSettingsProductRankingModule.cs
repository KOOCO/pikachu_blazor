using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsProductRankingModule : Entity<Guid>, IMultiTenant
{
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
    public Guid? TenantId { get; set; }
    public virtual WebsiteSettings WebsiteSettings { get; set; }

    public WebsiteSettingsProductRankingModule(
        Guid id,
        Guid websiteSettingsId,
        string title,
        string subTitle,
        string? content,
        int? moduleNumber
        ) : base(id)
    {
        WebsiteSettingsId = websiteSettingsId;
        Title = title;
        SubTitle = subTitle;
        Content = content;
        ModuleNumber = moduleNumber;
    }
}