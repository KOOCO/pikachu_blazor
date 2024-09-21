using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.UserShoppingCredits;

public class UserShoppingCredit : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid UserId { get; set; }
    public int Amount { get; private set; }
    public int CurrentRemainingCredits { get; private set; }
    public string? TransactionDescription { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public bool IsActive { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public UserShoppingCredit(
        Guid id,
        Guid userId,
        int amount,
        int currentRemainingCredits,
        string? transactionDescription,
        DateTime? expirationDate,
        bool isActive
        ) : base(id)
    {
        UserId = userId;
        IsActive = isActive;
        SetTransactionDescription(transactionDescription);
        SetAmount(amount);
        SetCurrentRemainingCredits(currentRemainingCredits);
        SetExpirationDate(expirationDate);
    }

    public UserShoppingCredit ChangeAmount(int amount)
    {
        SetAmount(amount);
        return this;
    }

    private void SetAmount(int amount)
    {
        Amount = Check.Positive(amount, nameof(amount));
    }

    public UserShoppingCredit ChangeTransactionDescription(string? transactionDescription)
    {
        SetTransactionDescription(transactionDescription);
        return this;
    }

    private void SetTransactionDescription(string? transactionDescription)
    {
        TransactionDescription = Check.Length(transactionDescription,
            nameof(transactionDescription), UserShoppingCreditConsts.MaxTransactionDescriptionLength);
    }

    public UserShoppingCredit ChangeCurrentRemainingCredits(int currentRemainingCredits)
    {
        SetCurrentRemainingCredits(currentRemainingCredits);
        return this;
    }

    private void SetCurrentRemainingCredits(int currentRemainingCredits)
    {
        CurrentRemainingCredits = Check.Positive(currentRemainingCredits, nameof(currentRemainingCredits));
    }

    public UserShoppingCredit ChangeExpirationDate(DateTime? expirationDate)
    {
        SetExpirationDate(expirationDate);
        return this;
    }

    private void SetExpirationDate(DateTime? expirationDate)
    {
        if (expirationDate?.Date <= DateTime.Today)
        {
            throw new ExpirationDateCannotBePastException();
        }

        ExpirationDate = expirationDate;
    }

    public UserShoppingCredit SetIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }
}
