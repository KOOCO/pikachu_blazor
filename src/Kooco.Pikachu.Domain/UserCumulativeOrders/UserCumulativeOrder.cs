using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class UserCumulativeOrder : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public int TotalOrders { get; private set; }
    public int TotalExchanges { get; private set; }
    public int TotalReturns { get; private set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }

    public UserCumulativeOrder(
        Guid id,
        Guid userId,
        int totalOrders,
        int totalExchanges,
        int totalReturns
        ) : base(id)
    {
        UserId = userId;
        SetTotalOrders(totalOrders);
        SetTotalExchanges(totalExchanges);
        SetTotalReturns(totalReturns);
    }

    public UserCumulativeOrder ChangeTotalOrders(int totalOrders)
    {
        SetTotalOrders(totalOrders);
        return this;
    }

    private void SetTotalOrders(int totalOrders)
    {
        TotalOrders = Check.Range(totalOrders, nameof(TotalOrders), 0, int.MaxValue);
    }

    public UserCumulativeOrder ChangeTotalExchanges(int totalExchanges)
    {
        SetTotalExchanges(totalExchanges);
        return this;
    }

    private void SetTotalExchanges(int totalExchanges)
    {
        TotalExchanges = Check.Range(totalExchanges, nameof(TotalExchanges), 0, int.MaxValue);
    }

    public UserCumulativeOrder ChangeTotalReturns(int totalReturns)
    {
        SetTotalReturns(totalReturns);
        return this;
    }

    private void SetTotalReturns(int totalReturns)
    {
        TotalReturns = Check.Range(totalReturns, nameof(TotalReturns), 0, int.MaxValue);
    }
}
