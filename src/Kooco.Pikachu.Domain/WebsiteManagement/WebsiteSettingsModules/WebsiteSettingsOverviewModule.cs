using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsOverviewModule : Entity<Guid>, IMultiTenant
{
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? SubTitle { get; set; }
    public string? BodyText { get; set; }
    public bool IsButtonEnable { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonLink { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(WebsiteSettingsId))]
    public virtual WebsiteSettings WebsiteSettings { get; set; }

    public WebsiteSettingsOverviewModule(
        Guid id,
        Guid websiteSettingsId,
        string title,
        string image,
        string? subTitle,
        string? bodyText,
        bool isButtonEnable,
        string? buttonText,
        string? buttonLink
        ) : base(id)
    {
        WebsiteSettingsId = websiteSettingsId;
        Title = title;
        Image = image;
        SubTitle = subTitle;
        BodyText = bodyText;
        IsButtonEnable = isButtonEnable;
        ButtonText = buttonText;
        ButtonLink = buttonLink;
    }
}
