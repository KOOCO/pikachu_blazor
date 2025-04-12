using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders;

[AllowAnonymous]
public class OrderMessageAppService(IOrderMessageRepository orderMessageRepository) : ApplicationService, IOrderMessageAppService
{
    // Retrieve an OrderMessage by ID
    public async Task<OrderMessageDto> GetAsync(Guid id)
    {
        var orderMessage = await orderMessageRepository.GetAsync(id);
        return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
    }

    // Retrieve a paginated list of OrderMessages with filtering and sorting
    public async Task<PagedResultDto<OrderMessageDto>> GetListAsync(GetOrderMessageListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = nameof(OrderMessage.CreationTime) + " DESC";

        }
        var totalCount = await orderMessageRepository.GetCountAsync(
            input.Filter,
            input.OrderId,
            input.SenderId,
            input.IsMerchant,
            input.Timestamp);

        var orderMessages = await orderMessageRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter,
            input.OrderId,
            input.SenderId,
            input.IsMerchant,
            input.Timestamp);

        return new PagedResultDto<OrderMessageDto>(
            totalCount,
            ObjectMapper.Map<List<OrderMessage>, List<OrderMessageDto>>(orderMessages)
        );
    }

    public async Task<List<OrderMessageDto>> GetOrderMessagesAsync(Guid orderId)
    {
        return ObjectMapper.Map<List<OrderMessage>, List<OrderMessageDto>>(
            await orderMessageRepository.GetOrderMessagesAsync(orderId)
        );
    }

    // Create a new OrderMessage
    public async Task<OrderMessageDto> CreateAsync(CreateUpdateOrderMessageDto input)
    {
        var orderMessage = new OrderMessage(
            GuidGenerator.Create(),
            input.OrderId,
            input.SenderId,
            input.Message,
            input.IsMerchant);

        await orderMessageRepository.InsertAsync(orderMessage);

        return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
    }

    // Update an existing OrderMessage
    public async Task<OrderMessageDto> UpdateAsync(Guid id, CreateUpdateOrderMessageDto input)
    {
        var orderMessage = await orderMessageRepository.GetAsync(id);

        orderMessage.Message = input.Message;
        orderMessage.IsMerchant = input.IsMerchant;
        orderMessage.SenderId = input.SenderId;
        orderMessage.OrderId = input.OrderId;

        await orderMessageRepository.UpdateAsync(orderMessage);

        return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
    }

    // Delete an OrderMessage by ID
    public async Task DeleteAsync(Guid id)
    {
        await orderMessageRepository.DeleteAsync(id);
    }
}