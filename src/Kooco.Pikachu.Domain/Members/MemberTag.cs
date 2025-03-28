using Kooco.Pikachu.TierManagement;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Members;

public class MemberTag : Entity<Guid>, IMultiTenant
{
    public Guid UserId { get; set; }
    public string Name { get; private set; }
    public Guid? VipTierId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual IdentityUser? User { get; set; }

    [ForeignKey(nameof(VipTierId))]
    public virtual VipTier? VipTier { get; set; }

    public MemberTag(
        Guid id,
        Guid userId,
        string name,
        Guid? vipTierId = null
        ) : base(id)
    {
        UserId = userId;
        Name = Check.NotNullOrWhiteSpace(name, nameof(Name), 128);
        VipTierId = vipTierId;
    }
}
