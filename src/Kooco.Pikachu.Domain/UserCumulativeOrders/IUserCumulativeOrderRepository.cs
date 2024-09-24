using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserCumulativeOrders;

public interface IUserCumulativeOrderRepository : IRepository<UserCumulativeOrder, Guid>
{
    Task<UserCumulativeOrder?> FirstOrDefaultByUserIdAsync(Guid userId);
    Task<long> GetCountAsync(Guid? userId, int? minTotalOrders, int? maxTotalOrders, int? minTotalExchanges,
        int? maxTotalExchanges, int? minTotalReturns, int? maxTotalReturns);
    Task<List<UserCumulativeOrder>> GetListAsync(int skipCount, int maxResultCount, string sorting, Guid? userId,
        int? minTotalOrders, int? maxTotalOrders, int? minTotalExchanges, int? maxTotalExchanges,
        int? minTotalReturns, int? maxTotalReturns);
    Task<IQueryable<UserCumulativeOrder>> GetFilteredQueryableAsync(Guid? userId, int? minTotalOrders, int? maxTotalOrders,
        int? minTotalExchanges, int? maxTotalExchanges, int? minTotalReturns, int? maxTotalReturns);
}
