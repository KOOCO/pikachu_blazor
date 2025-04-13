using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.Tenants.ElectronicInvoiceSettings;
using Kooco.Pikachu.Tenants.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders;

[RemoteService(IsEnabled = false)]
public class OrderDeliveryAppService : PikachuAppService, IOrderDeliveryAppService
{
    public async Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid id)
    {
        var result = await OrderDeliveryRepository.GetWithDetailsAsync(id);
        return ObjectMapper.Map<List<OrderDelivery>, List<OrderDeliveryDto>>([.. result]);
    }
    public async Task<OrderDeliveryDto> GetDeliveryOrderAsync(Guid id)
    {
        var result = await OrderDeliveryRepository.GetAsync(id);
        return ObjectMapper.Map<OrderDelivery, OrderDeliveryDto>(result);
    }
    public async Task<OrderDeliveryDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        OrderDelivery order = await OrderDeliveryRepository.GetAsync(id);
        // Capture old values before updating
        var oldDeliveryMethod = order.DeliveryMethod;
        var oldDeliveryNo = order.DeliveryNo;

        order.DeliveryMethod = input.DeliveryMethod.Value;

        order.DeliveryNo = input.ShippingNumber;

        await OrderDeliveryRepository.UpdateAsync(order);

        await UnitOfWorkManager.Current.SaveChangesAsync();

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        if (oldDeliveryMethod != order.DeliveryMethod)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
                order.OrderId,
                "DeliveryMethodChanged", // Localization key
                [L[oldDeliveryMethod.ToString()].Name, L[order.DeliveryMethod.ToString()].Name], // Pass changes and editor
                currentUserId,
                currentUserName
            );
        }

        if (oldDeliveryNo != order.DeliveryNo)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
                order.OrderId,
                "DeliveryNumberChanged", // Localization key
                [oldDeliveryNo ?? "None", order.DeliveryNo], // Pass changes and editor
                currentUserId,
                currentUserName
            );
        }

        if (!order.DeliveryNo.IsNullOrEmpty())
        {
            //await SendEmailAsync(order.OrderId, order.Id);
            await EmailAppService.SendLogisticsEmailAsync(order.OrderId, order.DeliveryNo, order.DeliveryMethod);
        }

        return ObjectMapper.Map<OrderDelivery, OrderDeliveryDto>(order);
    }
    public async Task ChangeShippingStatus(Guid orderId)
    {
        Order order = await OrderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await OrderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            if (order.DeliveryMethod is DeliveryMethod.HomeDelivery)
                orderDelivery.DeliveryStatus = DeliveryStatus.Shipped;

            else if (order.DeliveryMethod is DeliveryMethod.SelfPickup)
                orderDelivery.DeliveryStatus = DeliveryStatus.Delivered;

            await OrderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (order.DeliveryMethod is DeliveryMethod.HomeDelivery &&
                                    orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Shipped))
            order.ShippingStatus = ShippingStatus.Shipped;

        else if (order.DeliveryMethod is DeliveryMethod.SelfPickup &&
                                    orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Delivered))
            order.ShippingStatus = ShippingStatus.Delivered;

        await OrderRepository.UpdateAsync(order);

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping Status Change**
        await OrderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "ShippingStatusChanged", // Localization key
            [L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name], // Localized placeholders
            currentUserId,
            currentUserName
        );
    }
    public async Task UpdateDeliveredStatus(Guid orderId)
    {
        Order order = await OrderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await OrderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            orderDelivery.DeliveryStatus = DeliveryStatus.Delivered;
            await OrderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Delivered))
        {
            order.ShippingStatus = ShippingStatus.Delivered;
            await OrderRepository.UpdateAsync(order);
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentUser.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Completed)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await OrderInvoiceAppService.CreateInvoiceAsync(order.Id);

                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new() { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Delivery Update**
        await OrderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "OrderDelivered", // Localization key
            [L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name], // Localized placeholders
            currentUserId,
            currentUserName
        );
    }
    public async Task UpdatePickedUpStatus(Guid orderId)
    {
        Order order = await OrderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await OrderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            orderDelivery.DeliveryStatus = DeliveryStatus.Completed;

            await OrderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Completed))
        {
            order.ShippingStatus = ShippingStatus.Completed;

            await OrderRepository.UpdateAsync(order);
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentUser.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Completed)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await OrderInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Pickup Completion**
            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                 "OrderPickedUp", // Localization key
                 [L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name], // Localized placeholders
                 currentUserId,
                 currentUserName
            );
        }
    }
    public async Task UpdateOrderDeliveryStatus(Guid Id)
    {
        OrderDelivery delivery = await OrderDeliveryRepository.GetAsync(Id);
        var oldDeliveryStatus = delivery.DeliveryStatus;
        delivery.DeliveryStatus = DeliveryStatus.Shipped;

        delivery.Editor = CurrentUser.Name ?? string.Empty;

        await OrderDeliveryRepository.UpdateAsync(delivery);

        List<OrderDelivery> orderDeliveries = [.. (await OrderDeliveryRepository.GetQueryableAsync()).Where(x => x.OrderId == delivery.OrderId)];

        if (!orderDeliveries.Any(a => a.DeliveryStatus != DeliveryStatus.Shipped))
        {
            Order order = await OrderRepository.GetWithDetailsAsync(delivery.OrderId);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.Shipped;

            await OrderRepository.UpdateAsync(order);
            await EmailAppService.SendLogisticsEmailAsync(delivery.OrderId, delivery.DeliveryNo, delivery.DeliveryMethod);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentUser.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await OrderInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }

            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";
            if (oldShippingStatus != order.ShippingStatus)
            {

                await OrderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "OrderShippedWithShippingChange", // Store only the localization key in ActionType
                    [
                        L[oldDeliveryStatus.ToString()].Name,
                        L[delivery.DeliveryStatus.ToString()].Name,
                        L[oldShippingStatus.ToString()].Name,
                        L[order.ShippingStatus.ToString()].Name
                    ], // Store only parameters in ActionDetails
                    currentUserId,
                    currentUserName);
            }
            else
            {
                await OrderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "OrderShippedWithOutShippingChange", // Store only the localization key in ActionType
                    [
                        L[oldDeliveryStatus.ToString()].Name,
                        L[delivery.DeliveryStatus.ToString()].Name,

                    ], // Store only parameters in ActionDetails
                    currentUserId,
                    currentUserName);
            }

            //await SendEmailAsync(order.Id, delivery.DeliveryNo);
        }
    }

    public required OrderHistoryManager OrderHistoryManager { get; init; }
    public required IEmailAppService EmailAppService { get; init; }
    public required IBackgroundJobManager BackgroundJobManager { get; init; }
    public required IOrderRepository OrderRepository { get; init; }
    public required IOrderInvoiceAppService OrderInvoiceAppService { get; init; }
    public required IOrderDeliveryRepository OrderDeliveryRepository { get; init; }
    public required ITenantTripartiteRepository TenantTripartiteRepository { get; init; }
}