using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.Tenants.Entities;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.SettingManagement;

namespace Kooco.Pikachu.Orders;

[RemoteService(IsEnabled = false)]
public class OrderDeliveryAppService : ApplicationService, IOrderDeliveryAppService
{
    private readonly IOrderDeliveryRepository _orderDeliveryRepository;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly IRepository<TenantEmailSettings, Guid> _tenantEmailSettingsRepository;
    private readonly IGroupBuyRepository _groupBuyRepository;
    private readonly IEmailSender _emailSender;
    private readonly IOrderRepository _orderRepository;
    private readonly ISettingManager _settingManager;
    private readonly IEmailAppService _emailAppService;
    private readonly IOrderInvoiceAppService _electronicInvoiceAppService;
    private readonly IElectronicInvoiceSettingRepository _electronicInvoiceSettingRepository;
    private readonly OrderHistoryManager _orderHistoryManager;
    private readonly IBackgroundJobManager _backgroundJobManager;
    public OrderDeliveryAppService(IOrderDeliveryRepository orderDeliveryRepository, IStringLocalizer<PikachuResource> l, IOrderRepository orderRepository,
        IEmailSender emailSender, IGroupBuyRepository groupBuyRepositor, IRepository<TenantEmailSettings, Guid> tenantEmailSettingsRepository,
          ISettingManager settingManager, IOrderInvoiceAppService electronicInvoiceAppService, IElectronicInvoiceSettingRepository electronicInvoiceSettingRepository,
          IBackgroundJobManager backgroundJobManager, IEmailAppService emailAppService, OrderHistoryManager orderHistoryManager)
    {
        _orderDeliveryRepository = orderDeliveryRepository;
        _l = l;
        _orderRepository = orderRepository;
        _emailSender = emailSender;
        _groupBuyRepository = groupBuyRepositor;
        _settingManager = settingManager;
        _tenantEmailSettingsRepository = tenantEmailSettingsRepository;
        _backgroundJobManager = backgroundJobManager;
        _electronicInvoiceAppService = electronicInvoiceAppService;
        _electronicInvoiceSettingRepository = electronicInvoiceSettingRepository;
        _emailAppService = emailAppService;
        _orderHistoryManager = orderHistoryManager;
    }
    public async Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id)
    {
        List<OrderDelivery> result = await _orderDeliveryRepository.GetWithDetailsAsync(Id);

        return ObjectMapper.Map<List<OrderDelivery>, List<OrderDeliveryDto>>([.. result]);
    }
    public async Task<OrderDeliveryDto> GetDeliveryOrderAsync(Guid Id)
    {
        var result = await _orderDeliveryRepository.GetAsync(Id);

        return ObjectMapper.Map<OrderDelivery, OrderDeliveryDto>(result);
    }
    public async Task<OrderDeliveryDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        OrderDelivery order = await _orderDeliveryRepository.GetAsync(id);
        // Capture old values before updating
        var oldDeliveryMethod = order.DeliveryMethod;
        var oldDeliveryNo = order.DeliveryNo;

        order.DeliveryMethod = input.DeliveryMethod.Value;

        order.DeliveryNo = input.ShippingNumber;

        await _orderDeliveryRepository.UpdateAsync(order);

        await UnitOfWorkManager.Current.SaveChangesAsync();
        var changes = new List<string>();
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        if (oldDeliveryMethod != order.DeliveryMethod)
        {

            await _orderHistoryManager.AddOrderHistoryAsync(
                    order.OrderId,
                    "DeliveryMethodChanged", // Localization key
            new object[] { _l[oldDeliveryMethod.ToString()].Name, _l[order.DeliveryMethod.ToString()].Name }, // Pass changes and editor
                    currentUserId,
                    currentUserName
                );

        }

        if (oldDeliveryNo != order.DeliveryNo)
        {


            await _orderHistoryManager.AddOrderHistoryAsync(
                  order.OrderId,
                  "DeliveryNumberChanged", // Localization key
            new object[] { oldDeliveryNo ?? "None", order.DeliveryNo }, // Pass changes and editor
                  currentUserId,
                  currentUserName
              );
        }






        if (!order.DeliveryNo.IsNullOrEmpty())
        {
            //await SendEmailAsync(order.OrderId, order.Id);
            await _emailAppService.SendLogisticsEmailAsync(order.OrderId, order.DeliveryNo, order.DeliveryMethod);
        }

        return ObjectMapper.Map<OrderDelivery, OrderDeliveryDto>(order);
    }

    public async Task ChangeShippingStatus(Guid orderId)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await _orderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            if (order.DeliveryMethod is DeliveryMethod.HomeDelivery)
                orderDelivery.DeliveryStatus = DeliveryStatus.Shipped;

            else if (order.DeliveryMethod is DeliveryMethod.SelfPickup)
                orderDelivery.DeliveryStatus = DeliveryStatus.Delivered;

            await _orderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (order.DeliveryMethod is DeliveryMethod.HomeDelivery &&
                                    orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Shipped))
            order.ShippingStatus = ShippingStatus.Shipped;

        else if (order.DeliveryMethod is DeliveryMethod.SelfPickup &&
                                    orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Delivered))
            order.ShippingStatus = ShippingStatus.Delivered;

        await _orderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping Status Change**
        await _orderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "ShippingStatusChanged", // Localization key
            new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
            currentUserId,
            currentUserName
        );

    }

    public async Task UpdateDeliveredStatus(Guid orderId)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await _orderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            orderDelivery.DeliveryStatus = DeliveryStatus.Delivered;

            await _orderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Delivered))
        {
            order.ShippingStatus = ShippingStatus.Delivered;
            await _orderRepository.UpdateAsync(order);
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
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
                        await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);

                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new() { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Delivery Update**
        await _orderHistoryManager.AddOrderHistoryAsync(
order.Id,
"OrderDelivered", // Localization key
new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
currentUserId,
currentUserName
);

    }

    public async Task UpdatePickedUpStatus(Guid orderId)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(orderId);
        var oldShippingStatus = order.ShippingStatus;
        List<OrderDelivery> orderDeliveries = await _orderDeliveryRepository.GetWithDetailsAsync(orderId);

        foreach (OrderDelivery orderDelivery in orderDeliveries)
        {
            orderDelivery.DeliveryStatus = DeliveryStatus.Completed;

            await _orderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        if (orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.Completed))
        {
            order.ShippingStatus = ShippingStatus.Completed;

            await _orderRepository.UpdateAsync(order);
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
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
                        await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);

                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Pickup Completion**
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "OrderPickedUp", // Localization key
 new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
 currentUserId,
 currentUserName
);

        }
    }

    public async Task UpdateOrderDeliveryStatus(Guid Id)
    {
        OrderDelivery delivery = await _orderDeliveryRepository.GetAsync(Id);
        var oldDeliveryStatus = delivery.DeliveryStatus;
        delivery.DeliveryStatus = DeliveryStatus.Shipped;

        delivery.Editor = CurrentUser.Name ?? string.Empty;

        await _orderDeliveryRepository.UpdateAsync(delivery);

        List<OrderDelivery> orderDeliveries = [.. (await _orderDeliveryRepository.GetQueryableAsync()).Where(x => x.OrderId == delivery.OrderId)];

        if (!orderDeliveries.Any(a => a.DeliveryStatus != DeliveryStatus.Shipped))
        {
            Order order = await _orderRepository.GetWithDetailsAsync(delivery.OrderId);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.Shipped;

            await _orderRepository.UpdateAsync(order);
            await _emailAppService.SendLogisticsEmailAsync(delivery.OrderId, delivery.DeliveryNo, delivery.DeliveryMethod);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History based on whether ShippingStatus changed**
            string logMessage;
            if (oldShippingStatus != order.ShippingStatus)
            {

                await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderShippedWithShippingChange", // Store only the localization key in ActionType
     new object[] {
    _l[oldDeliveryStatus.ToString()].Name,
    _l[delivery.DeliveryStatus.ToString()].Name,
    _l[oldShippingStatus.ToString()].Name,
    _l[order.ShippingStatus.ToString()].Name
     }, // Store only parameters in ActionDetails
     currentUserId,
     currentUserName
 );

            }
            else
            {
                await _orderHistoryManager.AddOrderHistoryAsync(
order.Id,
"OrderShippedWithOutShippingChange", // Store only the localization key in ActionType
new object[] {
    _l[oldDeliveryStatus.ToString()].Name,
    _l[delivery.DeliveryStatus.ToString()].Name,

}, // Store only parameters in ActionDetails
currentUserId,
currentUserName
);
            }



            //await SendEmailAsync(order.Id, delivery.DeliveryNo);
        }

    }
}
