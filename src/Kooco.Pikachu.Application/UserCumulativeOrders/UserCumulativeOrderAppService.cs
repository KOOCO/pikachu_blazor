using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeOrders;

[Authorize]
[RemoteService(IsEnabled = false)]
public class UserCumulativeOrderAppService(UserCumulativeOrderManager userCumulativeOrderManager,
    IUserCumulativeOrderRepository userCumulativeOrderRepository) : PikachuAppService, IUserCumulativeOrderAppService
{
    public async Task<UserCumulativeOrderDto> CreateAsync(CreateUserCumulativeOrderDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeOrder = await userCumulativeOrderManager.CreateAsync(input.UserId.Value, input.TotalOrders,
            input.TotalExchanges, input.TotalReturns);
        return ObjectMapper.Map<UserCumulativeOrder, UserCumulativeOrderDto>(userCumulativeOrder);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userCumulativeOrder = await userCumulativeOrderRepository.GetAsync(id);
        await userCumulativeOrderRepository.DeleteAsync(userCumulativeOrder);
    }

    public async Task<UserCumulativeOrderDto> GetAsync(Guid id)
    {
        var userCumulativeOrder = await userCumulativeOrderRepository.GetAsync(id);
        return ObjectMapper.Map<UserCumulativeOrder, UserCumulativeOrderDto>(userCumulativeOrder);
    }

    public async Task<PagedResultDto<UserCumulativeOrderDto>> GetListAsync(GetUserCumulativeOrderListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(UserCumulativeOrder.CreationTime) + " DESC";
        }

        var totalCount = await userCumulativeOrderRepository.GetCountAsync(input.UserId, input.MinTotalOrders, input.MaxTotalOrders,
            input.MinTotalExchanges, input.MaxTotalExchanges, input.MinTotalReturns, input.MaxTotalExchanges);

        var items = await userCumulativeOrderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.UserId, input.MinTotalOrders, input.MaxTotalOrders,
            input.MinTotalExchanges, input.MaxTotalExchanges, input.MinTotalReturns, input.MaxTotalExchanges);

        return new PagedResultDto<UserCumulativeOrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<UserCumulativeOrder>, List<UserCumulativeOrderDto>>(items)
        };
    }

    public async Task<UserCumulativeOrderDto> UpdateAsync(Guid id, UpdateUserCumulativeOrderDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeOrder = await userCumulativeOrderRepository.GetAsync(id);

        await userCumulativeOrderManager.UpdateAsync(userCumulativeOrder, input.UserId.Value, input.TotalOrders,
            input.TotalExchanges, input.TotalReturns);

        return ObjectMapper.Map<UserCumulativeOrder, UserCumulativeOrderDto>(userCumulativeOrder);
    }
}
