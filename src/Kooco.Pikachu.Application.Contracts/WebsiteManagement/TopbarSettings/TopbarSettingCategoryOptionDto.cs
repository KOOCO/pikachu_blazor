using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingCategoryOptionDto : EntityDto<Guid>
{
    public TopbarCategoryLinkOption TopbarCategoryLinkOption { get; set; }
    public int Index { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public Guid TopbarSettingLinkId { get; set; }
    public Guid? TenantId { get; set; }
}
