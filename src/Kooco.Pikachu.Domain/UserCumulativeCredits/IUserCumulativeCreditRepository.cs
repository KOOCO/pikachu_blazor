using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserCumulativeCredits;

public interface IUserCumulativeCreditRepository : IRepository<UserCumulativeCredit, Guid>
{
    Task<UserCumulativeCredit?> FirstOrDefaultByUserIdAsync(Guid userId);
    Task<long> GetCountAsync(Guid? userId, int? minTotalAmount, int? maxTotalAmount, int? minTotalDeductions,
        int? maxTotalDeductions, int? minTotalRefunds, int? maxTotalRefunds);
    Task<List<UserCumulativeCredit>> GetListAsync(int skipCount, int maxResultCount, string sorting, Guid? userId,
        int? minTotalAmount, int? maxTotalAmount, int? minTotalDeductions, int? maxTotalDeductions,
        int? minTotalRefunds, int? maxTotalRefunds);
    Task<IQueryable<UserCumulativeCredit>> GetFilteredQueryableAsync(Guid? userId, int? minTotalAmount, int? maxTotalAmount,
        int? minTotalDeductions, int? maxTotalDeductions, int? minTotalRefunds, int? maxTotalRefunds);
}
