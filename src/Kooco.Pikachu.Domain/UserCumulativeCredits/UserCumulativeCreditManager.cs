using Kooco.Pikachu.UserShoppingCredits;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UserCumulativeCreditManager(IUserCumulativeCreditRepository userCumulativeCreditRepository,IUserShoppingCreditRepository userShoppingCreditRepository) : DomainService
{
    public async Task<UserCumulativeCredit> CreateAsync(Guid userId, int totalAmount, int totalDeductions, int totalRefunds)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.Range(totalAmount, nameof(totalAmount), 0, int.MaxValue);
        Check.Range(totalDeductions, nameof(totalDeductions), 0, int.MaxValue);
        Check.Range(totalRefunds, nameof(totalRefunds), 0, int.MaxValue);

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
        Check.Range(totalAmount, nameof(totalAmount), 0, int.MaxValue);
        Check.Range(totalDeductions, nameof(totalDeductions), 0, int.MaxValue);
        Check.Range(totalRefunds, nameof(totalRefunds), 0, int.MaxValue);

        userCumulativeCredit.UserId = userId;
        userCumulativeCredit.ChangeTotalAmount(totalAmount);
        userCumulativeCredit.ChangeTotalDeductions(totalDeductions);
        userCumulativeCredit.ChangeTotalRefunds(totalRefunds);

        await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit> ChangeTotalAmountAsync(UserCumulativeCredit userCumulativeCredit, int totalAmount)
    {
        Check.NotNull(userCumulativeCredit, nameof(userCumulativeCredit));
        Check.Range(totalAmount, nameof(totalAmount), 0, int.MaxValue);

        userCumulativeCredit.ChangeTotalAmount(totalAmount);
        await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit> ChangeTotalDeductionsAsync(UserCumulativeCredit userCumulativeCredit, int totalDeductions)
    {
        Check.NotNull(userCumulativeCredit, nameof(userCumulativeCredit));
        Check.Range(totalDeductions, nameof(totalDeductions), 0, int.MaxValue);

        userCumulativeCredit.ChangeTotalDeductions(totalDeductions);
        await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit> ChangeTotalRefundsAsync(UserCumulativeCredit userCumulativeCredit, int totalRefunds)
    {
        Check.NotNull(userCumulativeCredit, nameof(userCumulativeCredit));
        Check.Range(totalRefunds, nameof(totalRefunds), 0, int.MaxValue);

        userCumulativeCredit.ChangeTotalRefunds(totalRefunds);
        await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        return userCumulativeCredit;
    }

    public async Task<UserCumulativeCredit?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultByUserIdAsync(userId);
        var userCredit =  (await userShoppingCreditRepository.GetQueryableAsync())
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefault();
        if(userCumulativeCredit!=null)
        userCumulativeCredit.CurrentCreditBalance = userCredit.CurrentRemainingCredits;
        return userCumulativeCredit;
    }
}
