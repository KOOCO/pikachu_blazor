using Kooco.Pikachu.TenantManagement.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement;
public interface ITenantWalletRepository
{
    Task InsertAsync(TenantWalletTransaction transaction, CancellationToken ct = default);
    Task UpdateAsync(TenantWallet wallet, CancellationToken ct = default);
    Task<TenantWallet> GetAsync(Guid walletId);
    Task<decimal> GetBalanceAsync(Guid walletId);
    Task<(int totalCount, List<(Tenant tenant, TenantWallet wallet)> values)> GetPagedAllAsync(
        int skipCount = default,
        int maxResultCount = default,
        string? searchTerm = default,
        CancellationToken ct = default);
    Task<(Tenant? tenant, TenantWallet? wallet)> FindTenantAndWalletAsync(Guid walletId, CancellationToken ct = default);
    Task<decimal> SumDepositTransactionAmountAsync(Guid walletId, CancellationToken ct = default);
    Task<decimal> SumDeductionTransactionAmountAsync(Guid walletId, CancellationToken ct = default);
}