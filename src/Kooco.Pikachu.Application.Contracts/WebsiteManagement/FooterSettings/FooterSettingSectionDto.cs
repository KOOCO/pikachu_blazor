using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSettingSectionDto : Entity<Guid>
{
    public FooterSettingsPosition FooterSettingsPosition { get; set; }
    public string Title { get; set; }
    public FooterSettingsType? FooterSettingsType { get; set; }
    public string Text { get; set; }
    public string ImageUrl { get; set; }
    public string ImageName { get; set; }
    public List<FooterSettingLinkDto> Links { get; set; }
    public Guid FooterSettingId { get; set; }
    public Guid? TenantId { get; set; }

    public TenantSocialMediaDto? SocialMedia { get; set; }
    public TenantCustomerServiceDto? CustomerService { get; set; }
}
