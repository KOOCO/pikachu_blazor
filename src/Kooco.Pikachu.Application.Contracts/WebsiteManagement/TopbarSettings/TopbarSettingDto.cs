using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class TopbarSettingDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public TopbarStyleSettings TopbarStyleSettings { get; set; }
    public Guid? TenantId { get; set; }
    public string ConcurrencyStamp { get; set; }
    public List<TopbarSettingLinkDto> Links { get; set; }
}
