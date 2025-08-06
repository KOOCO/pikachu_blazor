using Kooco.Pikachu.LogisticsFeeManagements;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Tenants.Entities;
public sealed class TenantWalletTransaction : FullAuditedEntity<Guid>
{
    public required WalletDeductionStatus DeductionStatus { get; set; }
    public required WalletTradingMethods TradingMethods { get; set; }
    public required WalletTransactionType TransactionType { get; set; }
    public required decimal TransactionAmount { get; set; }
    public string? TransactionNotes { get; set; }

    public Guid TenantWalletId { get; set; }
    public TenantWallet? TenantWallet { get; set; }
    public ICollection<TenantLogisticsFeeRecord>? TenantLogisticsFeeRecords { get; set; }
}