using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingDto : FullAuditedEntityDto<Guid>
{
    public TopbarStyleSettings TopbarStyleSettings { get; set; }
    public Guid? TenantId { get; set; }
    public List<TopbarSettingLinkDto> Links { get; set; }
}
