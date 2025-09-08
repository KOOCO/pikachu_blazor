using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.InboxManagement.Managers;
using Kooco.Pikachu.InventoryManagement;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ReturnAndExchange;

public class ReturnAndExchangeAppService : PikachuAppService, IReturnAndExchangeAppService
{
    protected IOrderRepository OrderRepository { get; }
    protected OrderManager OrderManager { get; }
    protected IOrderDeliveryRepository OrderDeliveryRepository { get; }
    protected IRepository<OrderItem, Guid> OrderItemRepository { get; }
    protected OrderHistoryManager OrderHistoryManager { get; }
    protected OrderBuilder OrderBuilder { get; }
    protected NotificationManager NotificationManager { get; }
    protected InventoryLogManager InventoryLogManager { get; }
    protected IItemDetailsRepository ItemDetailsRepository { get; }
    protected ISetItemRepository SetItemRepository { get; }

    public ReturnAndExchangeAppService(
        IOrderRepository orderRepository,
        OrderManager orderManager,
        IOrderDeliveryRepository orderDeliveryRepository,
        IRepository<OrderItem, Guid> orderItemRepository,
        OrderHistoryManager orderHistoryManager,
        OrderBuilder orderBuilder,
        NotificationManager notificationManager,
        InventoryLogManager inventoryLogManager,
        IItemDetailsRepository itemDetailsRepository,
        ISetItemRepository setItemRepository
        )
    {
        OrderRepository = orderRepository;
        OrderManager = orderManager;
        OrderDeliveryRepository = orderDeliveryRepository;
        OrderItemRepository = orderItemRepository;
        OrderHistoryManager = orderHistoryManager;
        OrderBuilder = orderBuilder;
        NotificationManager = notificationManager;
        InventoryLogManager = inventoryLogManager;
        ItemDetailsRepository = itemDetailsRepository;
        SetItemRepository = setItemRepository;
    }

    public async Task ReturnOrderAsync(Guid id)
    {
        var order = await OrderRepository.GetAsync(id);

        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ShippingStatusBeforeReturn = order.ShippingStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Returned;
        order.ShippingStatus = ShippingStatus.Return;

        await OrderRepository.UpdateAsync(order);

        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        await OrderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "OrderReturnInitiated",
            [
                L[oldReturnStatus.ToString()].Name,
                L[order.ReturnStatus.ToString()].Name,
                L[oldOrderStatus.ToString()].Name,
                L[order.OrderStatus.ToString()].Name
            ],
            currentUserId,
            currentUserName
        );

        await OrderHistoryManager.AddOrderHistoryAsync(
             order.Id,
             "ShippingStatusChanged",
             [
                L[order.ShippingStatusBeforeReturn.ToString()].Value,
                L[order.ShippingStatus.ToString()].Value
             ],
             currentUserId,
             currentUserName
        );

        await NotificationManager.ReturnRequestedAsync(
            NotificationArgs.ForOrderWithUserName(
                order.Id,
                order.OrderNo,
                currentUserName
            ));
    }

    public async Task ExchangeOrderAsync(Guid id)
    {
        var order = await OrderRepository.GetAsync(id);

        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ShippingStatusBeforeReturn = order.ShippingStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Exchange;
        order.ShippingStatus = ShippingStatus.Exchange;

        await OrderRepository.UpdateAsync(order);

        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        await OrderHistoryManager.AddOrderHistoryAsync(
             order.Id,
             "OrderExchangeInitiated",
             [
                L[oldReturnStatus.ToString()].Value,
                L[order.ReturnStatus.ToString()].Value,
                L[oldOrderStatus.ToString()].Value,
                L[order.OrderStatus.ToString()].Value
             ],
             currentUserId,
             currentUserName
        );

        await OrderHistoryManager.AddOrderHistoryAsync(
             order.Id,
             "ShippingStatusChanged",
             [
                L[order.ShippingStatusBeforeReturn.ToString()].Value,
                L[order.ShippingStatus.ToString()].Value
             ],
             currentUserId,
             currentUserName
        );

        await NotificationManager.ExchangeRequestedAsync(
            NotificationArgs.ForOrderWithUserName(
                order.Id,
                order.OrderNo,
                currentUserName
            ));
    }

    public async Task<OrderDto> ReturnAndExchangeItemsAsync(Guid orderId, List<OrderItemDto> orderItemsInput, bool isReturn)
    {
        orderItemsInput = orderItemsInput?.Where(x => x.IsSelected && x.SelectedQuantity > 0).ToList() ?? [];
        if (orderItemsInput.IsNullOrEmpty())
        {
            throw new UserFriendlyException("No items selected for return or exchange.");
        }

        var order = await OrderRepository.GetWithDetailsAsync(orderId);

        List<OrderItemsCreateDto> orderItems = [];

        using (CurrentTenant.Change(order.TenantId))
        {
            var clonedOrder = await OrderBuilder.CloneAsync(order);
            clonedOrder.ReturnStatus = OrderReturnStatus.Pending;
            clonedOrder.OrderStatus = isReturn ? OrderStatus.Returned : OrderStatus.Exchange;
            clonedOrder.ShippingStatus = isReturn ? ShippingStatus.Return : ShippingStatus.Exchange;
            clonedOrder.ShippingStatusBeforeReturn = order.ShippingStatus;

            List<OrderItem> deletedItem = [];
            foreach (var item in order.OrderItems)
            {
                var orderItem = orderItemsInput.FirstOrDefault(x => x.Id == item.Id);
                if (orderItem != null)
                {
                    OrderManager.AddOrderItem(
                        clonedOrder,
                        item.ItemId,
                        item.ItemDetailId,
                        item.SetItemId,
                        item.FreebieId,
                        item.ItemType,
                        clonedOrder.Id,
                        item.Spec,
                        item.ItemPrice,
                        orderItem.SelectedQuantity * item.ItemPrice,
                        orderItem.SelectedQuantity,
                        item.SKU,
                        item.DeliveryTemperature,
                        item.DeliveryTemperatureCost,
                        item.IsAddOnProduct
                    );
                    item.Quantity -= orderItem.SelectedQuantity;
                    item.TotalAmount = item.Quantity * item.ItemPrice;

                    order.TotalQuantity -= orderItem.SelectedQuantity;
                    order.TotalAmount -= orderItem.SelectedQuantity * item.ItemPrice;

                    if (isReturn)
                    {
                        await InventoryLogManager.OrderItemUnsoldAsync(order, item); 
                    }
                }
            }

            clonedOrder.TotalAmount = clonedOrder.OrderItems.Sum(x => x.TotalAmount);
            clonedOrder.TotalQuantity = clonedOrder.OrderItems.Sum(x => x.Quantity);
            await OrderRepository.InsertAsync(clonedOrder);

            if (isReturn)
            {
                foreach (var item in clonedOrder.OrderItems)
                {
                    if (item.ItemId.HasValue)
                    {
                        ItemDetails? details = await ItemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == item.ItemId && x.Id == item.ItemDetailId);
                        if (!item.ItemDetailId.HasValue || details == null)
                        {
                            throw new UserFriendlyException("Item detail not found", $"There is no item detail with provided id: {item.ItemDetailId}");
                        }

                        await InventoryLogManager.ItemSoldAsync(clonedOrder, item, details, item.Quantity, item.IsAddOnProduct);
                    }
                    if (item.SetItemId.HasValue)
                    {
                        var setItem = await SetItemRepository.GetWithDetailsAsync(item.SetItemId.Value);
                        if (setItem != null)
                        {
                            setItem.SaleableQuantity -= item.Quantity;

                            foreach (var setItemDetail in setItem.SetItemDetails)
                            {
                                var totalOrderQuantity = setItemDetail.Quantity * item.Quantity;
                                var detail = await ItemDetailsRepository.FirstOrDefaultAsync(x =>
                                            x.ItemId == setItemDetail.ItemId
                                            && x.Attribute1Value == setItemDetail.Attribute1Value
                                            && x.Attribute2Value == setItemDetail.Attribute2Value
                                            && x.Attribute3Value == setItemDetail.Attribute3Value
                                            )
                                    ?? throw new UserFriendlyException("Item detail not found", $"There is no item detail with provided id: {item.ItemDetailId} for set item id: {setItem.Id}");
                                await InventoryLogManager.ItemSoldAsync(clonedOrder, item, detail, totalOrderQuantity);
                            }
                        }
                    }
                } 
            }

            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                isReturn ? "OrderItemsReturnInitiated" : "OrderItemsExchangeInitiated",
                [
                    order.OrderNo,
                    clonedOrder.OrderNo
                ],
                currentUserId,
                currentUserName
            );

            await OrderHistoryManager.AddOrderHistoryAsync(
                clonedOrder.Id,
                isReturn ? "OrderItemsReturnCreated" : "OrderItemsExchangeCreated",
                [
                    order.OrderNo,
                    clonedOrder.OrderNo
                ],
                currentUserId,
                currentUserName
            );

            if (isReturn)
            {
                await NotificationManager.ReturnRequestedAsync(
                    NotificationArgs.ForOrderWithUserName(
                        order.Id,
                        order.OrderNo,
                        currentUserName
                    ));
            }
            else
            {
                await NotificationManager.ExchangeRequestedAsync(
                    NotificationArgs.ForOrderWithUserName(
                        order.Id,
                        order.OrderNo,
                        currentUserName
                    ));
            }
            await OrderRepository.UpdateAsync(order);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
    }
}
