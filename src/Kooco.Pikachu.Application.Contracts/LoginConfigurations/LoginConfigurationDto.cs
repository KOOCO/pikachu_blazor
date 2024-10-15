using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.LoginConfigurations;

public class LoginConfigurationDto : FullAuditedEntityDto<Guid>
{
    public string? FacebookAppId { get; set; }
    public string? FacebookAppSecret { get; set; }

    public string? LineChannelId { get; set; }
    public string? LineChannelSecret { get; set; }

    public Guid? TenantId { get; set; }
}