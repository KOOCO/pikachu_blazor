using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserShoppingCredits;

public interface IUserShoppingCreditRepository : IRepository<UserShoppingCredit, Guid>
{
    Task<long> GetCountAsync(string? filter, Guid? userId, int? minAmount, int? maxAmount, int? minCurrentRemainingCredits,
        int? maxCurrentRemainingCredits, string? transactionDescription, DateTime? minExpirationDate,
        DateTime? maxExpirationDate, bool? isActive);

    Task<List<UserShoppingCredit>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, Guid? userId,
        int? minAmount, int? maxAmount, int? minCurrentRemainingCredits, int? maxCurrentRemainingCredits,
        string? transactionDescription, DateTime? minExpirationDate, DateTime? maxExpirationDate, bool? isActive);

    Task<IQueryable<UserShoppingCredit>> GetFilteredQueryableAsync(string? filter, Guid? userId, int? minAmount, int? maxAmount,
        int? minCurrentRemainingCredits, int? maxCurrentRemainingCredits, string? transactionDescription,
        DateTime? minExpirationDate, DateTime? maxExpirationDate, bool? isActive);
}
