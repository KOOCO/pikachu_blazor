using System;

namespace Kooco.Pikachu.TierManagement;

public class UpdateMemberTierArgs
{
    public Guid? TenantId { get; set; }
    public bool ShouldConfigureRecurringJob { get; set; } = false;
}
