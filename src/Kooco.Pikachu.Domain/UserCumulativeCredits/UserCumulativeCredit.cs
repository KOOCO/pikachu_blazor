using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UserCumulativeCredit : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public int TotalAmount { get; private set; }
    public int TotalDeductions { get; private set; }
    public int TotalRefunds { get; private set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public UserCumulativeCredit(
        Guid id,
        Guid userId,
        int totalAmount,
        int totalDeductions,
        int totalRefunds
        ) : base(id)
    {
        UserId = userId;
        SetTotalAmount(totalAmount);
        SetTotalDeductions(totalDeductions);
        SetTotalRefunds(totalRefunds);
    }

    public UserCumulativeCredit ChangeTotalAmount(int totalAmount)
    {
        SetTotalAmount(totalAmount);
        return this;
    }

    private void SetTotalAmount(int totalAmount)
    {
        TotalAmount = Check.Range(totalAmount, nameof(totalAmount), 0, int.MaxValue);
    }

    public UserCumulativeCredit ChangeTotalDeductions(int totalDeductions)
    {
        SetTotalDeductions(totalDeductions);
        return this;
    }

    private void SetTotalDeductions(int totalDeductions)
    {
        TotalDeductions = Check.Range(totalDeductions, nameof(totalDeductions), 0, int.MaxValue);
    }

    public UserCumulativeCredit ChangeTotalRefunds(int totalRefunds)
    {
        SetTotalRefunds(totalRefunds);
        return this;
    }

    private void SetTotalRefunds(int totalRefunds)
    {
        TotalRefunds = Check.Range(totalRefunds, nameof(totalRefunds), 0, int.MaxValue);
    }
}
