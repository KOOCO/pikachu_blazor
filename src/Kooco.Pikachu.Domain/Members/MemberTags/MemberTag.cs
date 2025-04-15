using Kooco.Pikachu.TierManagement;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Members.MemberTags;

public class MemberTag : CreationAuditedEntity<Guid>, IMultiTenant
{
    public Guid UserId { get; set; }

    [MaxLength(MemberTagConsts.MemberTagNameMaxLength)]
    public string Name { get; private set; }
    public bool IsEnabled { get; set; }
    public bool IsSystemAssigned { get; set; }
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
        bool isEnabled,
        bool isSystemAssigned,
        Guid? vipTierId = null
        ) : base(id)
    {
        UserId = userId;
        SetName(name);
        IsEnabled = isEnabled;
        IsSystemAssigned = isSystemAssigned;
        VipTierId = vipTierId;
    }

    internal void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(Name), MemberTagConsts.MemberTagNameMaxLength);
    }
}
