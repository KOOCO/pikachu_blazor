using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class UserCumulativeFinancial : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid UserId { get; set; }
    public int TotalSpent { get; private set; }
    public int TotalPaid { get; private set; }
    public int TotalUnpaid { get; private set; }
    public int TotalRefunded { get; private set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public UserCumulativeFinancial(
        Guid id,
        Guid userId,
        int totalSpent,
        int totalPaid,
        int totalUnpaid,
        int totalRefunded
        ) : base(id)
    {
        UserId = userId;
        SetTotalSpent(totalSpent);
        SetTotalPaid(totalPaid);
        SetTotalUnpaid(totalUnpaid);
        SetTotalRefunded(totalRefunded);
    }

    public UserCumulativeFinancial ChangeTotalSpent(int totalSpent)
    {
        SetTotalSpent(totalSpent);
        return this;
    }

    private void SetTotalSpent(int totalSpent)
    {
        TotalSpent = Check.Range(totalSpent, nameof(TotalSpent), 0, int.MaxValue);
    }

    public UserCumulativeFinancial ChangeTotalPaid(int totalPaid)
    {
        SetTotalPaid(totalPaid);
        return this;
    }

    private void SetTotalPaid(int totalPaid)
    {
        TotalPaid = Check.Range(totalPaid, nameof(TotalPaid), 0, int.MaxValue);
    }

    public UserCumulativeFinancial ChangeTotalUnpaid(int totalUnpaid)
    {
        SetTotalUnpaid(totalUnpaid);
        return this;
    }

    private void SetTotalUnpaid(int totalUnpaid)
    {
        TotalUnpaid = Check.Range(totalUnpaid, nameof(TotalUnpaid), 0, int.MaxValue);
    }

    public UserCumulativeFinancial ChangeTotalRefunded(int totalRefunded)
    {
        SetTotalRefunded(totalRefunded);
        return this;
    }

    private void SetTotalRefunded(int totalRefunded)
    {
        TotalRefunded = Check.Range(totalRefunded, nameof(TotalRefunded), 0, int.MaxValue);
    }
}
