using Kooco.Pikachu.Tenants.Requests;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Tenants;
public interface ITenantWalletAppService
{
    Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task<List<TenantWalletTransactionDto>> GetWalletTransactionsAsync(Guid walletId);
    Task<IRemoteStreamContent> ExportWalletTransactionsAsync(Guid walletId, List<Guid>? selectedIds = null);
}
