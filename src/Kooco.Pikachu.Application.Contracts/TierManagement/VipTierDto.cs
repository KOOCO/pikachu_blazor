using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TierManagement;

public class VipTierDto : EntityDto<Guid>
{
    public Tier Tier { get; set; }
    public string? TierName { get; set; }
    public int OrdersAmount { get; private set; }
    public int OrdersCount { get; private set; }
    public Guid TierSettingId { get; set; }
    public Guid? TenantId { get; set; }
}
