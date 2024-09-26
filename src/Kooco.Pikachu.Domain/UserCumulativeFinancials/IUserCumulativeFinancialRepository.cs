using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public interface IUserCumulativeFinancialRepository : IRepository<UserCumulativeFinancial, Guid>
{
    Task<UserCumulativeFinancial?> FirstOrDefaultByUserIdAsync(Guid userId);

    Task<long> GetCountAsync(Guid? userId, int? minTotalSpent, int? maxTotalSpent, int? minTotalPaid, int? maxTotalPaid,
        int? minTotalUnpaid, int? maxTotalUnpaid, int? minTotalRefunded, int? maxTotalRefunded);

    Task<List<UserCumulativeFinancial>> GetListAsync(int skipCount, int maxResultCount, string sorting,
        Guid? userId, int? minTotalSpent, int? maxTotalSpent, int? minTotalPaid, int? maxTotalPaid,
        int? minTotalUnpaid, int? maxTotalUnpaid, int? minTotalRefunded, int? maxTotalRefunded);

    Task<IQueryable<UserCumulativeFinancial>> GetFilteredQueryableAsync(Guid? userId, int? minTotalSpent,
        int? maxTotalSpent, int? minTotalPaid, int? maxTotalPaid, int? minTotalUnpaid, int? maxTotalUnpaid,
        int? minTotalRefunded, int? maxTotalRefunded);
}
