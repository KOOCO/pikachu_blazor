using Kooco.Pikachu.InboxManagement.Managers;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders;

[AllowAnonymous]
public class OrderMessageAppService(
    IOrderMessageRepository orderMessageRepository, 
    IHubContext<OrderNotificationHub> hubContext,
    IOrderRepository orderRepository,
    NotificationManager notificationManager
    ) : ApplicationService, IOrderMessageAppService
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

        await orderMessageRepository.InsertAsync(orderMessage,autoSave:true);
        var unreadCount =await (await orderMessageRepository.GetQueryableAsync()).CountAsync(m => m.OrderId == input.OrderId && !m.IsRead && !m.IsMerchant);

        var order = await orderRepository.GetAsync(input.OrderId);
        await hubContext.Clients.All.SendAsync(OrderNotificationNames.NewMessage, new
        {
            OrderId = order.Id,
            MessageCount = unreadCount,
            ShippingStatus = order.ShippingStatus.ToString(),
            PaymentMethod=order.PaymentMethod.ToString()
        });

        if (!orderMessage.IsMerchant)
        {
            await notificationManager.NewOrderMessageAsync(
                NotificationArgs.ForOrder(
                    order.Id,
                    order.OrderNo
                    ));
        }

        return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
    }

    public async Task MarkAsReadAsync(Guid orderId)
    {
        var messages = await orderMessageRepository.GetListAsync(m => m.OrderId == orderId && !m.IsRead);
        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await hubContext.Clients.All.SendAsync(OrderNotificationNames.MessageRead, new
        {
            OrderId = orderId,
            MessageCount = 0
        });
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