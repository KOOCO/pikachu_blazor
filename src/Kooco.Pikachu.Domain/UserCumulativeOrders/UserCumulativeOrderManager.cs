using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class UserCumulativeOrderManager(IUserCumulativeOrderRepository userCumulativeOrderRepository) : DomainService
{
    public async Task<UserCumulativeOrder> CreateAsync(Guid userId, int totalOrders, int totalExchanges, int totalReturns)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.Range(totalOrders, nameof(totalOrders), 0, int.MaxValue);
        Check.Range(totalExchanges, nameof(totalExchanges), 0, int.MaxValue);
        Check.Range(totalReturns, nameof(totalReturns), 0, int.MaxValue);

        var userCumulativeOrder = new UserCumulativeOrder(GuidGenerator.Create(), userId, totalOrders, totalExchanges, totalReturns);
        await userCumulativeOrderRepository.InsertAsync(userCumulativeOrder);
        return userCumulativeOrder;
    }

    public async Task<UserCumulativeOrder> UpdateAsync(UserCumulativeOrder userCumulativeOrder, Guid userId, int totalOrders, int totalExchanges, int totalReturns)
    {
        Check.NotNull(userCumulativeOrder, nameof(userCumulativeOrder));
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.Range(totalOrders, nameof(totalOrders), 0, int.MaxValue);
        Check.Range(totalExchanges, nameof(totalExchanges), 0, int.MaxValue);
        Check.Range(totalReturns, nameof(totalReturns), 0, int.MaxValue);

        userCumulativeOrder.UserId = userId;
        userCumulativeOrder.ChangeTotalOrders(totalOrders);
        userCumulativeOrder.ChangeTotalExchanges(totalExchanges);
        userCumulativeOrder.ChangeTotalReturns(totalReturns);
        await userCumulativeOrderRepository.UpdateAsync(userCumulativeOrder);
        return userCumulativeOrder;
    }

    public async Task<UserCumulativeOrder> ChangeTotalOrdersAsync(UserCumulativeOrder userCumulativeOrder, int totalOrders)
    {
        Check.NotNull(userCumulativeOrder, nameof(userCumulativeOrder));
        Check.Range(totalOrders, nameof(totalOrders), 0, int.MaxValue);

        userCumulativeOrder.ChangeTotalOrders(totalOrders);
        await userCumulativeOrderRepository.UpdateAsync(userCumulativeOrder);
        return userCumulativeOrder;
    }

    public async Task<UserCumulativeOrder> ChangeTotalExchangesAsync(UserCumulativeOrder userCumulativeOrder, int totalExchanges)
    {
        Check.NotNull(userCumulativeOrder, nameof(userCumulativeOrder));
        Check.Range(totalExchanges, nameof(totalExchanges), 0, int.MaxValue);

        userCumulativeOrder.ChangeTotalExchanges(totalExchanges);
        await userCumulativeOrderRepository.UpdateAsync(userCumulativeOrder);
        return userCumulativeOrder;
    }

    public async Task<UserCumulativeOrder> ChangeTotalReturnsAsync(UserCumulativeOrder userCumulativeOrder, int totalReturns)
    {
        Check.NotNull(userCumulativeOrder, nameof(userCumulativeOrder));
        Check.Range(totalReturns, nameof(totalReturns), 0, int.MaxValue);

        userCumulativeOrder.ChangeTotalReturns(totalReturns);
        await userCumulativeOrderRepository.UpdateAsync(userCumulativeOrder);
        return userCumulativeOrder;
    }

    public async Task<UserCumulativeOrder?> FirstOrDefaultByUserIdAsync(Guid userId)
    {
        var userCumulativeOrder = await userCumulativeOrderRepository.FirstOrDefaultByUserIdAsync(userId);
        return userCumulativeOrder;
    }
}
