using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSettingLinkDto : Entity<Guid>
{
    public int Index { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public Guid FooterSettingSectionId { get; set; }
    public Guid? TenantId { get; set; }
}