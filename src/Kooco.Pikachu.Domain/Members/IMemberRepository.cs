using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Members;

public interface IMemberRepository : IIdentityUserRepository, IRepository<IdentityUser, Guid>
{
    Task<long> GetCountAsync(string? filter);
    Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter);

    Task<long> GetMemberCreditRecordCountAsync(string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo,
        DateTime? expiryTimeFrom, DateTime? expiryTimeTo, int? minRemainingCredits, int? maxRemainingCredits,
        int? minAmount, int? maxAmount, Guid? userId);
    Task<List<MemberCreditRecordModel>> GetMemberCreditRecordListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expiryTimeFrom, DateTime? expiryTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId);

    Task<IQueryable<MemberCreditRecordModel>> GetMemberCreditRecordQueryableAsync(string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom, DateTime? expirationTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId);
}
