using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class EfCoreUserCumulativeFinancialRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, UserCumulativeFinancial, Guid>(dbContextProvider), IUserCumulativeFinancialRepository
{
    public async Task<UserCumulativeFinancial?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<long> GetCountAsync(Guid? userId, int? minTotalSpent, int? maxTotalSpent, int? minTotalPaid, int? maxTotalPaid, int? minTotalUnpaid,
        int? maxTotalUnpaid, int? minTotalRefunded, int? maxTotalRefunded)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalSpent, maxTotalSpent, minTotalPaid, maxTotalPaid, minTotalUnpaid,
            maxTotalUnpaid, minTotalRefunded, maxTotalRefunded);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserCumulativeFinancial>> GetListAsync(int skipCount, int maxResultCount, string sorting, Guid? userId, int? minTotalSpent,
        int? maxTotalSpent, int? minTotalPaid, int? maxTotalPaid, int? minTotalUnpaid, int? maxTotalUnpaid, int? minTotalRefunded, int? maxTotalRefunded)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalSpent, maxTotalSpent, minTotalPaid, maxTotalPaid, minTotalUnpaid,
            maxTotalUnpaid, minTotalRefunded, maxTotalRefunded);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<UserCumulativeFinancial>> GetFilteredQueryableAsync(Guid? userId, int? minTotalSpent, int? maxTotalSpent,
        int? minTotalPaid, int? maxTotalPaid, int? minTotalUnpaid, int? maxTotalUnpaid, int? minTotalRefunded, int? maxTotalRefunded)
    {
        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(minTotalSpent.HasValue, x => x.TotalSpent >= minTotalSpent)
            .WhereIf(maxTotalSpent.HasValue, x => x.TotalSpent <= maxTotalSpent)
            .WhereIf(minTotalPaid.HasValue, x => x.TotalPaid >= minTotalPaid)
            .WhereIf(maxTotalPaid.HasValue, x => x.TotalPaid <= maxTotalPaid)
            .WhereIf(minTotalUnpaid.HasValue, x => x.TotalUnpaid >= minTotalUnpaid)
            .WhereIf(maxTotalUnpaid.HasValue, x => x.TotalUnpaid <= maxTotalUnpaid)
            .WhereIf(minTotalRefunded.HasValue, x => x.TotalRefunded <= minTotalRefunded)
            .WhereIf(maxTotalRefunded.HasValue, x => x.TotalRefunded <= maxTotalRefunded);
    }
}
