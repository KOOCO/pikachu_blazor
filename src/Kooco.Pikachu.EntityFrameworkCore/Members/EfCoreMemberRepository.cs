using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Kooco.Pikachu.Members;

public class EfCoreMemberRepository(IDbContextProvider<PikachuDbContext> pikachuDbContextProvider,
    IDbContextProvider<IIdentityDbContext> dbContextProvider) : EfCoreIdentityUserRepository(dbContextProvider), IMemberRepository
{
    protected virtual Task<PikachuDbContext> GetPikachuDbContextAsync()
    {
        return pikachuDbContextProvider.GetDbContextAsync();
    }

    public async Task<long> GetCountAsync(string? filter)
    {
        var queryable = await GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(filter), q => q.UserName.Contains(filter)
            || q.PhoneNumber.Contains(filter) || q.Email.Contains(filter));
        return queryable.Count();
    }

    public async Task<List<IdentityUser>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
    {
        var queryable = await GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(filter), q => q.UserName.Contains(filter)
            || q.PhoneNumber.Contains(filter) || q.Email.Contains(filter));
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }


    public async Task<long> GetMemberCreditRecordCountAsync(string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo,
        DateTime? expirationTimeFrom, DateTime? expirationTimeTo, int? minRemainingCredits, int? maxRemainingCredits,
        int? minAmount, int? maxAmount, Guid? userId)
    {
        var queryable = await GetMemberCreditRecordQueryableAsync(filter, usageTimeFrom, usageTimeTo, expirationTimeFrom, expirationTimeTo,
            minRemainingCredits, maxRemainingCredits, minAmount, maxAmount, userId);
        return await queryable.LongCountAsync();
    }

    public async Task<List<MemberCreditRecordModel>> GetMemberCreditRecordListAsync(int skipCount, int maxResultCount,
        string sorting, string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom,
        DateTime? expirationTimeTo, int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId)
    {
        var queryable = await GetMemberCreditRecordQueryableAsync(filter, usageTimeFrom, usageTimeTo, expirationTimeFrom, expirationTimeTo,
            minRemainingCredits, maxRemainingCredits, minAmount, maxAmount, userId);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<MemberCreditRecordModel>> GetMemberCreditRecordQueryableAsync(string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom, DateTime? expirationTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId)
    {
        var dbContext = await GetPikachuDbContextAsync();

        var memberCredits = dbContext.UserShoppingCredits
                            .Where(x => userId != null && x.UserId == userId)
                            .Join(dbContext.Orders, credits => credits.Id, orders => orders.CreditDeductionRecordId, (credits, orders) => new { credits, orders })
                            .WhereIf(usageTimeFrom.HasValue, x => x.orders.CreationTime >= usageTimeFrom)
                            .WhereIf(usageTimeTo.HasValue, x => x.orders.CreationTime <= usageTimeTo)
                            .WhereIf(expirationTimeFrom.HasValue, x => x.credits.ExpirationDate >= expirationTimeFrom)
                            .WhereIf(expirationTimeTo.HasValue, x => x.credits.ExpirationDate <= expirationTimeTo)
                            .WhereIf(minRemainingCredits.HasValue, x => x.credits.CurrentRemainingCredits >= minRemainingCredits)
                            .WhereIf(maxRemainingCredits.HasValue, x => x.credits.CurrentRemainingCredits <= maxRemainingCredits)
                            .WhereIf(minAmount.HasValue, x => x.orders.CreditDeductionAmount >= minAmount)
                            .WhereIf(maxAmount.HasValue, x => x.orders.CreditDeductionAmount <= maxAmount)
                            .WhereIf(maxAmount.HasValue, x => x.orders.CreditDeductionAmount <= maxAmount)
                            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.credits.TransactionDescription != null && x.credits.TransactionDescription.Contains(filter));

        return memberCredits
            .Select(x => new MemberCreditRecordModel
            {
                Id = x.credits.Id,
                UsageTime = x.orders.CreationTime,
                TransactionDescription = x.credits.TransactionDescription,
                Amount = x.orders.CreditDeductionAmount,
                ExpirationDate = x.credits.ExpirationDate,
                RemainingCredits = x.credits.CurrentRemainingCredits
            });
    }
}
