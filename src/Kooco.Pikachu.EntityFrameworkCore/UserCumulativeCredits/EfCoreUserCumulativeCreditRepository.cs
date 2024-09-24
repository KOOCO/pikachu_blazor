using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class EfCoreUserCumulativeCreditRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, UserCumulativeCredit, Guid>(dbContextProvider), IUserCumulativeCreditRepository
{
    public async Task<UserCumulativeCredit?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<long> GetCountAsync(Guid? userId, int? minTotalAmount, int? maxTotalAmount, int? minTotalDeductions,
        int? maxTotalDeductions, int? minTotalRefunds, int? maxTotalRefunds)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalAmount, maxTotalAmount, minTotalDeductions, maxTotalDeductions,
            minTotalRefunds, maxTotalRefunds);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserCumulativeCredit>> GetListAsync(int skipCount, int maxResultCount, string sorting, Guid? userId,
        int? minTotalAmount, int? maxTotalAmount, int? minTotalDeductions, int? maxTotalDeductions,
        int? minTotalRefunds, int? maxTotalRefunds)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalAmount, maxTotalAmount, minTotalDeductions, maxTotalDeductions,
            minTotalRefunds, maxTotalRefunds);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<UserCumulativeCredit>> GetFilteredQueryableAsync(Guid? userId, int? minTotalAmount,
        int? maxTotalAmount, int? minTotalDeductions, int? maxTotalDeductions, int? minTotalRefunds, int? maxTotalRefunds)
    {
        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(minTotalAmount.HasValue, x => x.TotalAmount >= minTotalAmount)
            .WhereIf(maxTotalAmount.HasValue, x => x.TotalAmount <= maxTotalAmount)
            .WhereIf(minTotalDeductions.HasValue, x => x.TotalDeductions >= minTotalDeductions)
            .WhereIf(maxTotalDeductions.HasValue, x => x.TotalDeductions <= maxTotalDeductions)
            .WhereIf(minTotalRefunds.HasValue, x => x.TotalRefunds >= minTotalRefunds)
            .WhereIf(maxTotalRefunds.HasValue, x => x.TotalRefunds <= maxTotalRefunds);
    }
}
