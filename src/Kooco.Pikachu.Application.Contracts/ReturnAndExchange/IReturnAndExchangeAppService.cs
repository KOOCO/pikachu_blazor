using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ReturnAndExchange;

public interface IReturnAndExchangeAppService : IApplicationService
{
    Task ExchangeOrderAsync(Guid id);
    Task ReturnOrderAsync(Guid id);
    Task<OrderDto> ReturnAndExchangeItemsAsync(
        Guid orderId, 
        List<OrderItemDto> orderItemsInput, 
        bool isReturn,
        List<ReplacementItemDto> replacementItems
        );
    Task<List<ItemWithItemTypeDto>> GetGroupBuyItemsAsync(Guid groupBuyId);
    Task<List<ItemDetailWithItemTypeDto>> GetItemDetailsAsync(Guid groupBuyId, ItemWithItemTypeDto input);
    Task<ItemPricingDto> GetSetItemPricingAsync(Guid groupBuyId, Guid setItemId);
}