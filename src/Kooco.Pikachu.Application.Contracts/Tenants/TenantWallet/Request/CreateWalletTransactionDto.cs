using Kooco.Pikachu.TenantManagement;
using System;

namespace Kooco.Pikachu.Tenants.TenantWallet.Request;
public class CreateWalletTransactionDto
{
    public required Guid TenantWalletId { get; set; }
    public required WalletDeductionStatus DeductionStatus { get; set; }
    public required WalletTradingMethods TradingMethods { get; set; }
    public required WalletTransactionType TransactionType { get; set; }
    public required decimal TransactionAmount { get; set; }
    public string? TransactionNotes { get; set; }
}