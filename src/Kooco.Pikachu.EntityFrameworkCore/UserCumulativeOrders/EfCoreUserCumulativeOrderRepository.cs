using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class EfCoreUserCumulativeOrderRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, UserCumulativeOrder, Guid>(dbContextProvider), IUserCumulativeOrderRepository
{
    public async Task<UserCumulativeOrder?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<long> GetCountAsync(Guid? userId, int? minTotalOrders, int? maxTotalOrders, int? minTotalExchanges,
        int? maxTotalExchanges, int? minTotalReturns, int? maxTotalReturns)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalOrders, maxTotalOrders, minTotalExchanges,
            maxTotalExchanges, minTotalReturns, maxTotalReturns);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserCumulativeOrder>> GetListAsync(int skipCount, int maxResultCount, string sorting, Guid? userId,
        int? minTotalOrders, int? maxTotalOrders, int? minTotalExchanges, int? maxTotalExchanges,
        int? minTotalReturns, int? maxTotalReturns)
    {
        var queryable = await GetFilteredQueryableAsync(userId, minTotalOrders, maxTotalOrders, minTotalExchanges,
            maxTotalExchanges, minTotalReturns, maxTotalReturns);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<UserCumulativeOrder>> GetFilteredQueryableAsync(Guid? userId, int? minTotalOrders,
        int? maxTotalOrders, int? minTotalExchanges, int? maxTotalExchanges, int? minTotalReturns, int? maxTotalReturns)
    {
        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(minTotalOrders.HasValue, x => x.TotalOrders >= minTotalOrders)
            .WhereIf(maxTotalOrders.HasValue, x => x.TotalOrders <= maxTotalOrders)
            .WhereIf(minTotalExchanges.HasValue, x => x.TotalExchanges >= minTotalExchanges)
            .WhereIf(maxTotalExchanges.HasValue, x => x.TotalExchanges <= maxTotalExchanges)
            .WhereIf(minTotalReturns.HasValue, x => x.TotalReturns >= minTotalReturns)
            .WhereIf(maxTotalReturns.HasValue, x => x.TotalReturns <= maxTotalReturns);
    }
}
