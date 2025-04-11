using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Repositories;
public class TenantWalletRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, TenantWallet, Guid>(dbContextProvider), ITenantWalletRepository
{
    public async Task InsertAsync(TenantWalletTransaction transaction, CancellationToken ct = default)
    {
        await TenantWalletTransaction.InsertAsync(transaction, autoSave: true, cancellationToken: ct);
    }
    public async Task UpdateAsync(TenantWallet wallet, CancellationToken ct = default)
    {
        await UpdateAsync(wallet, autoSave: true, cancellationToken: ct);
    }
    public async Task<TenantWallet> GetAsync(Guid walletId)
    {
        using (MultiTenant.Disable())
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.TenantWallets.FirstAsync(w => w.Id == walletId);
        }
    }
    public async Task<decimal> GetBalanceAsync(Guid walletId)
    {
        using (MultiTenant.Disable())
        {
            var dbContext = await GetDbContextAsync();
            var wallet = dbContext.TenantWallets
                .AsNoTracking()
                .FirstOrDefault(w => w.Id == walletId);
            return wallet?.WalletBalance ?? default;
        }
    }
    public async Task<(int totalCount, List<(Tenant tenant, TenantWallet wallet)> values)> GetPagedAllAsync(
        int skipCount = default,
        int maxResultCount = default,
        string? searchTerm = default,
        CancellationToken ct = default)
    {
        if (maxResultCount <= 0) maxResultCount = 10;
        using (MultiTenant.Disable())
        {
            var totalCount = (int)await TenantRepository.GetCountAsync(searchTerm, ct);
            if (totalCount is 0)
            {
                return (default, new List<(Tenant tenant, TenantWallet wallet)>());
            }

            var pagedTenants = await TenantRepository.GetListAsync(
                sorting: nameof(Tenant.Name),
                maxResultCount: maxResultCount,
                skipCount: skipCount,
                filter: searchTerm,
                includeDetails: false,
                cancellationToken: ct
            );

            if (pagedTenants.Count is 0)
            {
                return (totalCount, new List<(Tenant tenant, TenantWallet wallet)>());
            }

            var pagedTenantIds = pagedTenants.Select(t => t.Id).ToList();
            var pagedTenantMap = pagedTenants.ToDictionary(t => t.Id);
            HashSet<Guid> pagedTenantIdHashSet = [.. pagedTenantIds];

            var dbSet = await GetDbSetAsync();
            var existingWalletsFromDb = await dbSet
                .Where(w =>
                    w.TenantId != null &&
                    pagedTenantIdHashSet.Contains(w.TenantId.Value))
                .ToListAsync(ct);

            var existingWalletMap = existingWalletsFromDb
                .ToDictionary(w => w.TenantId!.Value);

            List<(Tenant tenant, TenantWallet wallet)> resultList = new(pagedTenants.Count);
            List<TenantWallet> walletsToInsert = [];

            foreach (var tenant in pagedTenants)
            {
                TenantWallet wallet;
                if (existingWalletMap.TryGetValue(tenant.Id, out var existingWallet))
                {
                    wallet = existingWallet;
                }
                else
                {
                    wallet = new()
                    {
                        TenantId = tenant.Id,
                        WalletBalance = default
                    };

                    walletsToInsert.Add(wallet);
                }
                resultList.Add((tenant, wallet));
            }

            if (walletsToInsert.Count is not 0)
            {
                await InsertManyAsync(walletsToInsert, autoSave: true, cancellationToken: ct);
            }

            return (totalCount, resultList);
        }
    }
    public async Task<(Tenant? tenant, TenantWallet? wallet)> FindTenantAndWalletAsync(Guid walletId, CancellationToken ct = default)
    {
        using (MultiTenant.Disable())
        {
            var dbContext = await GetDbContextAsync();
            var queryResult = await dbContext.TenantWallets
                .AsNoTracking()
                .Where(w => w.Id == walletId)
                .Join(
                    dbContext.Tenants,
                    wallet => wallet.TenantId,
                    tenant => tenant.Id,
                    (wallet, tenant) => new { Wallet = wallet, Tenant = tenant }
                )
                .SingleOrDefaultAsync(cancellationToken: ct);

            if (queryResult is null) return (null, null);
            return (queryResult.Tenant, queryResult.Wallet);
        }
    }
    public async Task<decimal> SumDepositTransactionAmountAsync(Guid walletId, CancellationToken ct = default)
    {
        using (MultiTenant.Disable())
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.TenantWalletTransactions
                .AsNoTracking()
                .Where(w =>
                    w.TenantWalletId == walletId &&
                    w.TransactionType == WalletTransactionType.Deposit &&
                    w.DeductionStatus == WalletDeductionStatus.Completed)
                .SumAsync(x => x.TransactionAmount, cancellationToken: ct);
        }
    }
    public async Task<decimal> SumDeductionTransactionAmountAsync(Guid walletId, CancellationToken ct = default)
    {
        using (MultiTenant.Disable())
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.TenantWalletTransactions
                .AsNoTracking()
                .Where(w =>
                    w.TenantWalletId == walletId &&
                    w.TransactionType == WalletTransactionType.Deduction &&
                    w.DeductionStatus == WalletDeductionStatus.Completed)
                .SumAsync(x => x.TransactionAmount, cancellationToken: ct);
        }
    }

    public required IDataFilter<IMultiTenant> MultiTenant { get; init; }
    public required ITenantRepository TenantRepository { get; init; }
    public required IRepository<TenantWalletTransaction, Guid> TenantWalletTransaction { get; init; }
}