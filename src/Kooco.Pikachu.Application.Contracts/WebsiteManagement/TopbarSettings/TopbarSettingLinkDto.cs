using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingLinkDto : EntityDto<Guid>
{
    public TopbarLinkSettings TopbarLinkSettings { get; set; }
    public int Index { get; set; }
    public string Title { get; private set; }
    public string? Url { get; private set; }
    public Guid TopbarSettingId { get; set; }
    public Guid? TenantId { get; set; }
    public List<TopbarSettingCategoryOptionDto> CategoryOptions { get; set; }
}
