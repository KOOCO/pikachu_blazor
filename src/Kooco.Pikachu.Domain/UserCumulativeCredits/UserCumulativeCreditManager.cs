using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UserCumulativeCreditManager(IUserCumulativeCreditRepository userCumulativeCreditRepository) : DomainService
{
    public async Task<UserCumulativeCredit> CreateAsync(Guid userId, int totalAmount, int totalDeductions, int totalRefunds)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        var userCumulativeCredit = new UserCumulativeCredit(GuidGenerator.Create(), userId,
            totalAmount, totalDeductions, totalRefunds);
        await userCumulativeCreditRepository.InsertAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit> UpdateAsync(UserCumulativeCredit userCumulativeCredit, Guid userId,
        int totalAmount, int totalDeductions, int totalRefunds)
    {
        Check.NotNull(userCumulativeCredit, nameof(userCumulativeCredit));
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));

        userCumulativeCredit.UserId = userId;
        userCumulativeCredit.ChangeTotalAmount(totalAmount);
        userCumulativeCredit.ChangeTotalDeductions(totalDeductions);
        userCumulativeCredit.ChangeTotalRefunds(totalRefunds);

        await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit?> FindByUserIdAsync(Guid userId)
    {
        var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        return userCumulativeCredit;
    }
}
