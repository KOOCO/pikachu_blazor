using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderHistories;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.TenantEmailing;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;

namespace Kooco.Pikachu.OrderDeliveries
{
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
        private readonly IElectronicInvoiceAppService _electronicInvoiceAppService;
        private readonly IElectronicInvoiceSettingRepository _electronicInvoiceSettingRepository;
        private readonly OrderHistoryManager _orderHistoryManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public OrderDeliveryAppService(IOrderDeliveryRepository orderDeliveryRepository, IStringLocalizer<PikachuResource> l, IOrderRepository orderRepository,
            IEmailSender emailSender, IGroupBuyRepository groupBuyRepositor, IRepository<TenantEmailSettings, Guid> tenantEmailSettingsRepository,
              ISettingManager settingManager, IElectronicInvoiceAppService electronicInvoiceAppService, IElectronicInvoiceSettingRepository electronicInvoiceSettingRepository,
              IBackgroundJobManager backgroundJobManager, IEmailAppService emailAppService,OrderHistoryManager orderHistoryManager)
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

            if (oldDeliveryMethod != order.DeliveryMethod)
            {
                changes.Add(L["DeliveryMethodChanged", _l[oldDeliveryMethod.ToString()].Value, _l[order.DeliveryMethod.ToString()].Value]); // Localized message
            }

            if (oldDeliveryNo != order.DeliveryNo)
            {
                changes.Add(L["DeliveryNumberChanged", oldDeliveryNo ?? "None", order.DeliveryNo]); // Localized message
            }

            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            if (changes.Any())
            {
                string logMessage = string.Join(", ", changes); // Combine localized messages

                await _orderHistoryManager.AddOrderHistoryAsync(
                    order.OrderId,
                    "OrderShippingUpdated", // Localization key
                    new object[] { logMessage, currentUserName }, // Pass changes and editor
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
        private async Task SendEmailAsync(Guid id, Guid DeliveryOrderId, OrderStatus? orderStatus = null)
        {
            var delivers = await _orderDeliveryRepository.GetWithDetailsAsync(id);
            var deliveryOrder = delivers.Where(x => x.Id == DeliveryOrderId).FirstOrDefault();
            var order = await _orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await _groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            var emailSettings = await _tenantEmailSettingsRepository.FirstOrDefaultAsync();

            string status = orderStatus == null ? _l[order.ShippingStatus.ToString()] : _l[orderStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            if (emailSettings != null && !string.IsNullOrEmpty(emailSettings.Subject))
            {
                subject = emailSettings.Subject;
            }

            string body = File.ReadAllText("wwwroot/EmailTemplates/email.html");
            DateTime creationTime = order.CreationTime;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");

            if (emailSettings != null)
            {
                if (!string.IsNullOrEmpty(emailSettings.Greetings))
                {
                    body = body.Replace("{{Greetings}}", emailSettings.Greetings);
                }
                else
                {
                    body = body.Replace("{{Greetings}}", "");
                }

                if (!string.IsNullOrEmpty(emailSettings.Footer))
                {
                    body = body.Replace("{{Footer}}", emailSettings.Footer);
                }
            }

            body = body.Replace("{{NotifyMessage}}", groupbuy.NotifyMessage);
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNo}}", order.OrderNo);
            body = body.Replace("{{OrderDate}}", formattedTime);
            if (deliveryOrder.DeliveryNo != null)
            {
                body = body.Replace("{{DeliveryNo}}", deliveryOrder.DeliveryNo);
            }
            else
            {
                string pattern = @"<span class=""spacer""></span>\s*<p>貨運號碼</p>\s*<p>\{\{DeliveryNo\}\}</p>";

                // Replace the matched pattern with an empty string
                body = Regex.Replace(body, pattern, "");
            }

            var deliveryCost = (order.DeliveryCost ?? 0) + (order.DeliveryCostForNormal ?? 0) + (order.DeliveryCostForFreeze ?? 0) + (order.DeliveryCostForFrozen ?? 0);

            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            if (!groupbuy.IsEnterprise)
            {
                body = body.Replace("{{PaymentMethod}}", _l[order.PaymentMethod.ToString()]);
            }
            body = body.Replace("{{PaymentStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", _l[deliveryOrder.DeliveryMethod.ToString()]);
            body = body.Replace("{{DeliveryFee}}", $"${deliveryCost}");
            body = body.Replace("{{RecipientAddress}}", $"{order.City} {order.AddressDetails}");
            body = body.Replace("{{ShippingStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            if (deliveryOrder.Items != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in deliveryOrder.Items)
                {
                    string itemName = "";
                    if (item.ItemType == ItemType.Item)
                    {
                        itemName = item.Item?.ItemName;
                    }
                    else if (item.ItemType == ItemType.SetItem)
                    {
                        itemName = item.SetItem?.SetItemName;
                    }
                    else
                    {
                        itemName = item.Freebie?.ItemName;
                    }

                    itemName += $" {item.ItemPrice:N0} x {item.Quantity}";
                    sb.Append(
                        $@"
                    <tr>
                        <td>{itemName}</td>
                        <td>{_l[item.DeliveryTemperature.ToString()]}</td>
                        <td>${item.ItemPrice:N0}</td>
                        <td>{item.Quantity}</td>
                            <td>${item.DeliveryTemperatureCost}</td>
                        <td>${item.TotalAmount:N0}</td>
                    </tr>"
                    );
                }

                body = body.Replace("{{OrderItems}}", sb.ToString());
            }

            body = body.Replace("{{TotalAmount}}", $"${order.TotalAmount:N0}");

            var defaultFromEmail = await _settingManager.GetOrNullGlobalAsync("Abp.Mailing.DefaultFromAddress");
            var defaultFromName = await _settingManager.GetOrNullGlobalAsync("Abp.Mailing.DefaultFromDisplayName");
            MailAddress from = new(defaultFromEmail, emailSettings?.SenderName ?? defaultFromName);
            MailAddress to = new(order.CustomerEmail);

            MailMessage mailMessage = new(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await _emailSender.SendAsync(mailMessage);
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
                new object[] { _l[oldShippingStatus.ToString()].Value, _l[order.ShippingStatus.ToString()].Value }, // Localized placeholders
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
                            GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
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
    new object[] { _l[oldShippingStatus.ToString()].Value, _l[order.ShippingStatus.ToString()].Value }, // Localized placeholders
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
     new object[] { _l[oldShippingStatus.ToString()].Value, _l[order.ShippingStatus.ToString()].Value }, // Localized placeholders
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
                    logMessage = L["OrderShippedWithShippingChange",
                                   _l[oldDeliveryStatus.ToString()].Value, _l[delivery.DeliveryStatus.ToString()].Value,
                                   _l[oldShippingStatus.ToString()].Value, _l[order.ShippingStatus.ToString()].Value];
                }
                else
                {
                    logMessage = L["OrderShippedWithoutShippingChange",
                                   _l[oldDeliveryStatus.ToString()].Value, _l[delivery.DeliveryStatus.ToString()].Value];
                }

                await _orderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "OrderShipped", // Localization key
                    new object[] { logMessage }, // Pass localized log message
                    currentUserId,
                    currentUserName
                );

                //await SendEmailAsync(order.Id, delivery.DeliveryNo);
            }

        }
    }
}
