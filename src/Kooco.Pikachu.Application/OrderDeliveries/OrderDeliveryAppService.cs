﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Localization;
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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.SettingManagement;

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
        public OrderDeliveryAppService(IOrderDeliveryRepository orderDeliveryRepository, IStringLocalizer<PikachuResource> l, IOrderRepository orderRepository,
            IEmailSender emailSender, IGroupBuyRepository groupBuyRepositor, IRepository<TenantEmailSettings, Guid> tenantEmailSettingsRepository,
              ISettingManager settingManager)
        {
            _orderDeliveryRepository = orderDeliveryRepository;
            _l = l;
            _orderRepository= orderRepository;
            _emailSender= emailSender;
            _groupBuyRepository= groupBuyRepositor;
            _settingManager= settingManager;
            _tenantEmailSettingsRepository = tenantEmailSettingsRepository;
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

            order.DeliveryMethod = input.DeliveryMethod.Value;

            order.DeliveryNo = input.ShippingNumber;

            await _orderDeliveryRepository.UpdateAsync(order);
            
            await UnitOfWorkManager.Current.SaveChangesAsync();
            
            if (!order.DeliveryNo.IsNullOrEmpty()) await SendEmailAsync(order.OrderId, order.Id);

            return ObjectMapper.Map<OrderDelivery, OrderDeliveryDto>(order);
        }
        private async Task SendEmailAsync(Guid id,Guid DeliveryOrderId, OrderStatus? orderStatus = null)
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
            else {
                string pattern = @"<span class=""spacer""></span>\s*<p>貨運號碼</p>\s*<p>\{\{DeliveryNo\}\}</p>";

                // Replace the matched pattern with an empty string
                body = Regex.Replace(body, pattern, "");
            }
            
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
            body = body.Replace("{{DeliveryFee}}", "0");
            body = body.Replace("{{RecipientAddress}}", order.AddressDetails);
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

            body = body.Replace("{{DeliveryFee}}", "$0");
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
        public async Task UpdateOrderDeliveryStatus(Guid Id)
        {
            OrderDelivery delivery = await _orderDeliveryRepository.GetAsync(Id);

            delivery.DeliveryStatus = DeliveryStatus.Shipped;
            
            delivery.Editor = CurrentUser.Name ?? string.Empty;

            await _orderDeliveryRepository.UpdateAsync(delivery);

            List<OrderDelivery> orderDeliveries = [.. (await _orderDeliveryRepository.GetQueryableAsync()).Where(x => x.OrderId == delivery.OrderId)];

            if(!orderDeliveries.Any(a => a.DeliveryStatus != DeliveryStatus.Shipped))
            {
                Order order = await _orderRepository.GetAsync(delivery.OrderId);

                order.ShippingStatus = ShippingStatus.Shipped;
                
                await _orderRepository.UpdateAsync(order);
                
                await UnitOfWorkManager.Current.SaveChangesAsync();
                
                await SendEmailAsync(order.Id, delivery.DeliveryNo);
            }

        }
        private async Task SendEmailAsync(Guid id, string deliveryNo, OrderStatus? orderStatus = null)
        {
            Order order = await _orderRepository.GetWithDetailsAsync(id);

            GroupBuy groupbuy = await _groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);

            string status = orderStatus == null ? _l[order.ShippingStatus.ToString()] : _l[orderStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/email.html");

            DateTime creationTime = order.CreationTime;
            
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");

            body = body.Replace("{{Greetings}}", "");
            
            body = body.Replace("{{Footer}}", "");
            
            if (order.ShippingStatus is ShippingStatus.WaitingForPayment) body = body.Replace("{{NotifyMessage}}", groupbuy.NotifyMessage);
            
            else body = body.Replace("{{NotifyMessage}}", "");

            if (!deliveryNo.IsNullOrEmpty()) body = body.Replace("{{DeliveryNo}}", deliveryNo);

            else
            {
                string pattern = @"<span class=""spacer""></span>\s*<p>貨運號碼</p>\s*<p>\{\{DeliveryNo\}\}</p>";

                body = Regex.Replace(body, pattern, "");
            }

            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNo}}", order.OrderNo);
            body = body.Replace("{{OrderDate}}", formattedTime);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            
            if (!groupbuy.IsEnterprise) body = body.Replace("{{PaymentMethod}}", _l[order.PaymentMethod.ToString()]);

            body = body.Replace("{{PaymentStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", $"{_l[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{DeliveryFee}}", "0");
            body = body.Replace("{{RecipientAddress}}", order.AddressDetails);
            body = body.Replace("{{ShippingStatus}}", _l[order.ShippingStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            if (order.OrderItems != null)
            {
                StringBuilder sb = new();
                foreach (var item in order.OrderItems)
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
                        <td>${item.ItemPrice:N0}</td>
                        <td>{item.Quantity}</td>
                        <td>${item.TotalAmount:N0}</td>
                    </tr>"
                    );
                }

                body = body.Replace("{{OrderItems}}", sb.ToString());
            }

            body = body.Replace("{{DeliveryFee}}", "$0");
            body = body.Replace("{{TotalAmount}}", $"${order.TotalAmount:N0}");

            await _emailSender.SendAsync(order.CustomerEmail, subject, body);
        }
    }
}
