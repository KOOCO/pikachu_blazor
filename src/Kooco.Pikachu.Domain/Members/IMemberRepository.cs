using Kooco.Pikachu.TierManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Members;
public interface IMemberRepository : IIdentityUserRepository, IRepository<IdentityUser, Guid>
{
    Task<MemberModel> GetMemberAsync(Guid memberId);

    Task<long> GetCountAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null);
    Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null);

    Task<long> CountOrdersAsync(Guid memberId);

    Task<VipTier?> CheckForVipTierAsync(Guid userId);

    Task UpdateMemberTierAsync();

    Task<long> GetMemberCreditRecordCountAsync(string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo,
        DateTime? expiryTimeFrom, DateTime? expiryTimeTo, int? minRemainingCredits, int? maxRemainingCredits,
        int? minAmount, int? maxAmount, Guid? userId);
    Task<List<MemberCreditRecordModel>> GetMemberCreditRecordListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expiryTimeFrom, DateTime? expiryTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId);

    Task<IQueryable<MemberCreditRecordModel>> GetMemberCreditRecordQueryableAsync(string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom, DateTime? expirationTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId);
    Task<List<IdentityUser>> GetBirthdayMember();
}
