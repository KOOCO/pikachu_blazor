using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserShoppingCredits;

public class EfCoreUserShoppingCreditRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, UserShoppingCredit, Guid>(dbContextProvider), IUserShoppingCreditRepository
{
    public async Task<long> GetCountAsync(string? filter, Guid? userId, int? minAmount, int? maxAmount,
        int? minCurrentRemainingCredits, int? maxCurrentRemainingCredits, string? transactionDescription,
        DateTime? minExpirationDate, DateTime? maxExpirationDate, bool? isActive)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, minAmount, maxAmount, minCurrentRemainingCredits,
            maxCurrentRemainingCredits, transactionDescription, minExpirationDate, maxExpirationDate, isActive);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserShoppingCredit>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        Guid? userId, int? minAmount, int? maxAmount, int? minCurrentRemainingCredits, int? maxCurrentRemainingCredits,
        string? transactionDescription, DateTime? minExpirationDate, DateTime? maxExpirationDate, bool? isActive)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, minAmount, maxAmount, minCurrentRemainingCredits,
            maxCurrentRemainingCredits, transactionDescription, minExpirationDate, maxExpirationDate, isActive);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<UserShoppingCredit>> GetFilteredQueryableAsync(string? filter, Guid? userId, int? minAmount, int? maxAmount,
        int? minCurrentRemainingCredits, int? maxCurrentRemainingCredits, string? transactionDescription,
        DateTime? minExpirationDate, DateTime? maxExpirationDate, bool? isActive)
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(minAmount.HasValue, x => x.Amount >= minAmount)
            .WhereIf(maxAmount.HasValue, x => x.Amount <= maxAmount)
            .WhereIf(minCurrentRemainingCredits.HasValue, x => x.CurrentRemainingCredits >= minCurrentRemainingCredits)
            .WhereIf(maxCurrentRemainingCredits.HasValue, x => x.CurrentRemainingCredits <= maxCurrentRemainingCredits)
            .WhereIf(!string.IsNullOrWhiteSpace(transactionDescription), x => x.TransactionDescription != null && x.TransactionDescription.Contains(transactionDescription))
            .WhereIf(minExpirationDate.HasValue, x => x.ExpirationDate >= minExpirationDate)
            .WhereIf(maxExpirationDate.HasValue, x => x.ExpirationDate <= maxExpirationDate)
            .WhereIf(isActive.HasValue, x => x.IsActive == isActive)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => (x.TransactionDescription != null && x.TransactionDescription.Contains(filter))
            || x.Amount.ToString() == filter || x.CurrentRemainingCredits.ToString() == filter);
    }

    public async Task<List<UserShoppingCredit>> GetExpirableCreditsAsync()
    {
        var expirableCredits = await (await GetQueryableAsync())
            .Where(usc => usc.IsActive && usc.ShoppingCreditType == UserShoppingCreditType.Grant
                && usc.ExpirationDate.HasValue && usc.ExpirationDate < DateTime.Today)
            .ToListAsync();

        return expirableCredits;
    }
}
