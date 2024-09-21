using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserShoppingCredits;

public class UserShoppingCreditManager(IUserShoppingCreditRepository userShoppingCreditRepository) : DomainService
{
    public async Task<UserShoppingCredit> CreateAsync(Guid userId, int amount, int currentRemainingCredits,
        string? transactionDescription, DateTime? expirationDate, bool isActive)
    {
        Check.Positive(amount, nameof(amount));
        Check.Positive(currentRemainingCredits, nameof(currentRemainingCredits));
        Check.Length(transactionDescription, nameof(transactionDescription), UserShoppingCreditConsts.MaxTransactionDescriptionLength);

        if (expirationDate?.Date < DateTime.Today)
        {
            throw new ExpirationDateCannotBePastException();
        }

        var userShoppingCredit = new UserShoppingCredit(GuidGenerator.Create(), userId, amount,
            currentRemainingCredits, transactionDescription, expirationDate, isActive);

        await userShoppingCreditRepository.InsertAsync(userShoppingCredit);

        return userShoppingCredit;
    }

    public async Task<UserShoppingCredit> UpdateAsync(UserShoppingCredit userShoppingCredit, Guid userId, int amount,
        int currentRemainingCredits, string? transactionDescription, DateTime? expirationDate, bool isActive)
    {
        Check.NotNull(userShoppingCredit, nameof(userShoppingCredit));
        Check.Positive(amount, nameof(amount));
        Check.Positive(currentRemainingCredits, nameof(currentRemainingCredits));
        Check.Length(transactionDescription, nameof(transactionDescription), UserShoppingCreditConsts.MaxTransactionDescriptionLength);

        userShoppingCredit.UserId = userId;

        userShoppingCredit.ChangeAmount(amount);
        userShoppingCredit.ChangeCurrentRemainingCredits(currentRemainingCredits);
        userShoppingCredit.ChangeTransactionDescription(transactionDescription);
        userShoppingCredit.SetIsActive(isActive);

        if (expirationDate != userShoppingCredit.ExpirationDate)
        {
            userShoppingCredit.ChangeExpirationDate(expirationDate);
        }

        await userShoppingCreditRepository.UpdateAsync(userShoppingCredit);

        return userShoppingCredit;
    }

    public async Task<UserShoppingCredit> SetIsActiveAsync(UserShoppingCredit userShoppingCredit, bool isActive)
    {
        Check.NotNull(userShoppingCredit, nameof(userShoppingCredit));

        userShoppingCredit.SetIsActive(isActive);
        await userShoppingCreditRepository.UpdateAsync(userShoppingCredit);
        return userShoppingCredit;
    }
}
