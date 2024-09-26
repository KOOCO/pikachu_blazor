using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class UserCumulativeFinancialManager(IUserCumulativeFinancialRepository userCumulativeFinancialRepository) : DomainService
{
    public async Task<UserCumulativeFinancial> CreateAsync(Guid userId, int totalSpent, int totalPaid, int totalUnpaid, int totalRefunded)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.Range(totalSpent, nameof(totalSpent), 0, int.MaxValue);
        Check.Range(totalPaid, nameof(totalPaid), 0, int.MaxValue);
        Check.Range(totalUnpaid, nameof(totalUnpaid), 0, int.MaxValue);
        Check.Range(totalRefunded, nameof(totalRefunded), 0, int.MaxValue);

        var userCumulativeFinancial = new UserCumulativeFinancial(GuidGenerator.Create(), userId, totalSpent,
            totalPaid, totalUnpaid, totalRefunded);
        await userCumulativeFinancialRepository.InsertAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial> UpdateAsync(UserCumulativeFinancial userCumulativeFinancial, Guid userId,
        int totalSpent, int totalPaid, int totalUnpaid, int totalRefunded)
    {
        Check.NotNull(userCumulativeFinancial, nameof(userCumulativeFinancial));
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.Range(totalSpent, nameof(totalSpent), 0, int.MaxValue);
        Check.Range(totalPaid, nameof(totalPaid), 0, int.MaxValue);
        Check.Range(totalUnpaid, nameof(totalUnpaid), 0, int.MaxValue);
        Check.Range(totalRefunded, nameof(totalRefunded), 0, int.MaxValue);

        userCumulativeFinancial.UserId = userId;
        userCumulativeFinancial.ChangeTotalSpent(totalSpent);
        userCumulativeFinancial.ChangeTotalPaid(totalPaid);
        userCumulativeFinancial.ChangeTotalUnpaid(totalUnpaid);
        userCumulativeFinancial.ChangeTotalRefunded(totalRefunded);

        await userCumulativeFinancialRepository.UpdateAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial> ChangeTotalSpentAsync(UserCumulativeFinancial userCumulativeFinancial, int totalSpent)
    {
        Check.NotNull(userCumulativeFinancial, nameof(userCumulativeFinancial));
        Check.Range(totalSpent, nameof(totalSpent), 0, int.MaxValue);

        userCumulativeFinancial.ChangeTotalSpent(totalSpent);
        await userCumulativeFinancialRepository.UpdateAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial> ChangeTotalPaidAsync(UserCumulativeFinancial userCumulativeFinancial, int totalPaid)
    {
        Check.NotNull(userCumulativeFinancial, nameof(userCumulativeFinancial));
        Check.Range(totalPaid, nameof(totalPaid), 0, int.MaxValue);

        userCumulativeFinancial.ChangeTotalPaid(totalPaid);
        await userCumulativeFinancialRepository.UpdateAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial> ChangeTotalUnpaidAsync(UserCumulativeFinancial userCumulativeFinancial, int totalUnpaid)
    {
        Check.NotNull(userCumulativeFinancial, nameof(userCumulativeFinancial));
        Check.Range(totalUnpaid, nameof(totalUnpaid), 0, int.MaxValue);

        userCumulativeFinancial.ChangeTotalUnpaid(totalUnpaid);
        await userCumulativeFinancialRepository.UpdateAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial> ChangeTotalRefundedAsync(UserCumulativeFinancial userCumulativeFinancial, int totalRefunded)
    {
        Check.NotNull(userCumulativeFinancial, nameof(userCumulativeFinancial));
        Check.Range(totalRefunded, nameof(totalRefunded), 0, int.MaxValue);

        userCumulativeFinancial.ChangeTotalRefunded(totalRefunded);
        await userCumulativeFinancialRepository.UpdateAsync(userCumulativeFinancial);
        return userCumulativeFinancial;
    }

    public async Task<UserCumulativeFinancial?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var userCumulativeFinancial = await userCumulativeFinancialRepository.FirstOrDefaultByUserIdAsync(userId);
        return userCumulativeFinancial;
    }
}
