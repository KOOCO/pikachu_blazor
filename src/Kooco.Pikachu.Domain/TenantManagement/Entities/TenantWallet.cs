using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TenantManagement.Entities;
public sealed class TenantWallet : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public required Guid? TenantId { get; set; }
    public required decimal WalletBalance { get; set; }

    public ICollection<TenantWalletTransaction>? TenantWalletTransactions { get; set; }
}