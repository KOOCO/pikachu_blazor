using Kooco.Pikachu.EdmManagement;
using Kooco.Pikachu.TierManagement;
using Kooco.Pikachu.UserShoppingCredits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Members;
public interface IMemberRepository : IIdentityUserRepository, IRepository<IdentityUser, Guid>
{
    Task<MemberModel> GetMemberAsync(Guid memberId);
    Task<MemberModel> FindMemberByEmailAsync(string Email);

    Task<long> GetCountAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null,
        DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minOrderCount = null, int? maxOrderCount = null,
        int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null);

    Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter = null, string? memberType = null,
        IEnumerable<string>? selectedMemberTags = null, DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minOrderCount = null,
        int? maxOrderCount = null, int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null);

    Task<IQueryable<MemberModel>> GetFilteredQueryableAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null,
        DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minOrderCount = null, int? maxOrderCount = null,
        int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null, IEnumerable<string>? selectedMemberTypes = null);

    Task<long> CountOrdersAsync(Guid memberId);

    Task<VipTierUpgradeEmailModel> CheckForVipTierAsync(Guid userId);

    Task<List<VipTierUpgradeEmailModel>> UpdateMemberTierAsync(CancellationToken cancellationToken = default);

    Task<long> GetMemberCreditRecordCountAsync(string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo,
        DateTime? expiryTimeFrom, DateTime? expiryTimeTo, int? minRemainingCredits, int? maxRemainingCredits,
        int? minAmount, int? maxAmount, Guid? userId, UserShoppingCreditType? shoppingCreditType);
    Task<List<MemberCreditRecordModel>> GetMemberCreditRecordListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expiryTimeFrom, DateTime? expiryTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId, UserShoppingCreditType? shoppingCreditType);

    Task<IQueryable<MemberCreditRecordModel>> GetMemberCreditRecordQueryableAsync(string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom, DateTime? expirationTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId, UserShoppingCreditType? shoppingCreditType);
    Task<List<IdentityUser>> GetBirthdayMember();

    Task<List<(Guid id, string name, string email)>> GetEdmMemberNameAndEmailAsync(bool applyToAllMembers, IEnumerable<string> memberTags);
    Task<VipTierProgressModel> GetMemberTierProgressAsync(Guid memberId);
}
