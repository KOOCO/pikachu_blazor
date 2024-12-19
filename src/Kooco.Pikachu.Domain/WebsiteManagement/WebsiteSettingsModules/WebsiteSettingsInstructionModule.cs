using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsInstructionModule : Entity<Guid>, IMultiTenant
{
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? BodyText { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(WebsiteSettingsId))]
    public virtual WebsiteSettings WebsiteSettings { get; set; }

    public WebsiteSettingsInstructionModule(
        Guid id,
        Guid websiteSettingsId,
        string title,
        string image,
        string? bodyText
        ) : base(id)
    {
        WebsiteSettingsId = websiteSettingsId;
        Title = title;
        Image = image;
        BodyText = bodyText;
    }
}
