using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class OrderDetails : ComponentBase
    {
        private async Task GetOrderDetailsAsync()
        {
            Order = await _orderAppService.GetWithDetailsAsync(OrderId) ?? new();
            UpdateStepByShippingStatus(Order.ShippingStatus);
            OrderHistory = await _orderAppService.GetOrderLogsAsync(OrderId);
            if (Order.DeliveryMethod is not DeliveryMethod.SelfPickup &&
                Order.DeliveryMethod is not DeliveryMethod.DeliveredByStore)
                OrderDeliveryCost = Order.DeliveryCost;

            OrderDeliveries = await _orderDeliveryAppService.GetListByOrderAsync(OrderId);

            OrderDeliveries = [.. OrderDeliveries.Where(w => w.Items.Count > 0)];

            PaymentStatus = await GetPaymentStatus();

            CustomerServiceHistory = await _OrderMessageAppService.GetOrderMessagesAsync(Order.Id);

            await InvokeAsync(StateHasChanged);
        }
    }
}