using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class FooterSettingDto : FullAuditedEntityDto<Guid>
{
    public List<FooterSettingSectionDto> Sections { get; set; }
    public Guid? TenantId { get; set; }
}
