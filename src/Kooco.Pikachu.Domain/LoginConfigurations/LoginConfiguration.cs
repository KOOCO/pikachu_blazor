using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LoginConfigurations;

public class LoginConfiguration : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string? FacebookAppId { get; set; }
    public string? FacebookAppSecret { get; set; }

    public string? LineChannelId { get; set; }
    public string? LineChannelSecret { get; set; }

    public Guid? TenantId { get; set; }

    public LoginConfiguration(
        Guid id,
        string? facebookAppId,
        string? facebookAppSecret,
        string? lineChannelId,
        string? lineChannelSecret
        ) : base(id)
    {
        FacebookAppId = facebookAppId;
        FacebookAppSecret = facebookAppSecret;
        LineChannelId = lineChannelId;
        LineChannelSecret = lineChannelSecret;
    }
}
