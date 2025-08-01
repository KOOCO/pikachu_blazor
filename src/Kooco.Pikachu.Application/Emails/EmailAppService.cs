﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Kooco.Pikachu.Emails;

public class EmailAppService(IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository,
    ITenantSettingsAppService tenantSettingsAppService, IEmailSender emailSender, IBackgroundJobManager backgroundJobManager,
    IConfiguration configuration, IdentityUserManager userManager) : PikachuAppService, IEmailAppService
{
    public async Task SendLogisticsEmailAsync(Guid orderId, string? deliveryNo = "", DeliveryMethod? deliveryMethod = null)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var order = await orderRepository.GetAsync(orderId);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {L[order.ShippingStatus.ToString()]}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/delivery_status.html");
            DateTime creationTime = order.CreationTime;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");

            var trackingUrls = new Dictionary<string, string>
            {
                { "SevenToEleven", "https://eservice.7-11.com.tw/e-tracking/search.aspx" },
                { "FamilyMart", "https://fmec.famiport.com.tw/FP_Entrance/QueryBox" },
                { "TCatDelivery", "https://www.t-cat.com.tw/inquire/trace.aspx" },
                { "PostOffice", "https://postserv.post.gov.tw/pstmail/" },
                { "BlackCat", "https://www.t-cat.com.tw/inquire/trace.aspx" }
            };

            var trackingUrl = trackingUrls
                .FirstOrDefault(x => deliveryMethod != null
                && deliveryMethod.ToString()!.Contains(x.Key, StringComparison.OrdinalIgnoreCase)).Value
                ?? string.Empty;

            body = body.Replace("{{TrackingUrlDisplay}}", string.IsNullOrEmpty(trackingUrl) ? "none" : "block");
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNumber}}", order.OrderNo);
            body = body.Replace("{{ShippingStatus}}", L[order.ShippingStatus.ToString()]);
            body = body.Replace("{{OrderDate}}", formattedTime);
            body = body.Replace("{{DeliveryNumber}}", deliveryNo);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{PaymentMethod}}", L[order.PaymentMethod.ToString()]);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            body = body.Replace("{{RecipientAddress}}", GetAddress(order));
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{OrderNotes}}", order.Remarks);

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";
            string orderUrl = $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}";

            body = body.Replace("{{OrderUrl}}", orderUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{TrackingUrl}}", trackingUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendOrderStatusEmailAsync(Guid id, OrderStatus? orderStatus = null, ShippingStatus? shippingStatus = null)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var order = await orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            string status = orderStatus == null
                ? shippingStatus.HasValue ? L[shippingStatus.ToString()] : L[order.ShippingStatus.ToString()]
                : L[orderStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/order_status.html");
            DateTime creationTime = order.CreationTime;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");

            var deliveryCost = (order.DeliveryCost ?? 0) + (order.DeliveryCostForNormal ?? 0) + (order.DeliveryCostForFreeze ?? 0) + (order.DeliveryCostForFrozen ?? 0);
            if (order.ShippingStatus == ShippingStatus.WaitingForPayment)
            {
                body = body.Replace("{{NotifyMessage}}", groupbuy.NotifyMessage);
            }
            else
            {
                body = body.Replace("{{NotifyMessage}}", "");
            }

            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNo}}", order.OrderNo);
            body = body.Replace("{{OrderDate}}", formattedTime);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            if (!groupbuy.IsEnterprise)
            {
                body = body.Replace("{{PaymentMethod}}", L[order.PaymentMethod.ToString()]);
            }
            body = body.Replace("{{PaymentStatus}}", L[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{DeliveryFee}}", $"${deliveryCost:N0}");
            body = body.Replace("{{RecipientAddress}}", GetAddress(order));
            body = body.Replace("{{ShippingStatus}}", L[order.ShippingStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);
            body = body.Replace("{{OrderStatus}}", status);

            if (order.OrderItems != null)
            {
                StringBuilder sb = new();
                string orderItemsHtml = File.ReadAllText("wwwroot/EmailTemplates/order_items.html");
                foreach (var item in order.OrderItems)
                {
                    string? itemName = "";
                    string? imageUrl = "";
                    if (item.ItemType == ItemType.Item)
                    {
                        itemName = item.Item?.ItemName;
                        imageUrl = item.Item?.Images?.FirstOrDefault()?.ImageUrl;
                    }
                    else if (item.ItemType == ItemType.SetItem)
                    {
                        itemName = item.SetItem?.SetItemName;
                        imageUrl = item.SetItem?.Images?.FirstOrDefault()?.ImageUrl;
                    }
                    else
                    {
                        itemName = item.Freebie?.ItemName;
                        imageUrl = item.Freebie?.Images?.FirstOrDefault()?.ImageUrl;
                    }

                    var itemHtml = orderItemsHtml
                        .Replace("{{ImageUrl}}", imageUrl)
                        .Replace("{{ItemName}}", itemName)
                        .Replace("{{ItemDetails}}", item.SKU)
                        .Replace("{{UnitPrice}}", $"${item.ItemPrice:N0}")
                        .Replace("{{ItemQuantity}}", item.Quantity.ToString("N0"))
                        .Replace("{{ItemTotal}}", $"${item.TotalAmount:N0}");

                    sb.Append(itemHtml);
                }

                body = body.Replace("{{OrderItems}}", sb.ToString());
            }

            body = body.Replace("{{TotalAmount}}", $"${order.TotalAmount:N0}");

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant?.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";
            string orderUrl = $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}";

            body = body.Replace("{{OrderUrl}}", orderUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendOrderUpdateEmailAsync(Guid id)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var order = await orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            string status = L[order.ShippingStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/order_update.html");

            var deliveryCost = (order.DeliveryCost ?? 0) + (order.DeliveryCostForNormal ?? 0) + (order.DeliveryCostForFreeze ?? 0) + (order.DeliveryCostForFrozen ?? 0);

            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNumber}}", order.OrderNo);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            body = body.Replace("{{PaymentMethod}}", !groupbuy.IsEnterprise ? L[order.PaymentMethod.ToString()] : "");
            body = body.Replace("{{PaymentStatus}}", L[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{DeliveryFee}}", $"${deliveryCost:N0}");
            body = body.Replace("{{RecipientAddress}}", GetAddress(order));
            body = body.Replace("{{ShippingStatus}}", L[order.ShippingStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);
            body = body.Replace("{{OrderStatus}}", status);

            if (order.OrderItems != null)
            {
                StringBuilder sb = new();
                string orderItemsHtml = File.ReadAllText("wwwroot/EmailTemplates/order_items.html");
                foreach (var item in order.OrderItems)
                {
                    string? itemName = "";
                    string? imageUrl = "";
                    if (item.ItemType == ItemType.Item)
                    {
                        itemName = item.Item?.ItemName;
                        imageUrl = item.Item?.Images?.FirstOrDefault()?.ImageUrl;
                    }
                    else if (item.ItemType == ItemType.SetItem)
                    {
                        itemName = item.SetItem?.SetItemName;
                        imageUrl = item.SetItem?.Images?.FirstOrDefault()?.ImageUrl;
                    }
                    else
                    {
                        itemName = item.Freebie?.ItemName;
                        imageUrl = item.Freebie?.Images?.FirstOrDefault()?.ImageUrl;
                    }

                    var itemHtml = orderItemsHtml
                        .Replace("{{ImageUrl}}", imageUrl)
                        .Replace("{{ItemName}}", itemName)
                        .Replace("{{ItemDetails}}", item.SKU)
                        .Replace("{{UnitPrice}}", $"${item.ItemPrice:N0}")
                        .Replace("{{ItemQuantity}}", item.Quantity.ToString("N0"))
                        .Replace("{{ItemTotal}}", $"${item.TotalAmount:N0}");

                    sb.Append(itemHtml);
                }

                body = body.Replace("{{OrderItems}}", sb.ToString());
            }

            body = body.Replace("{{TotalAmount}}", $"${order.TotalAmount:N0}");

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";
            string orderUrl = $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}";

            body = body.Replace("{{OrderUrl}}", orderUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendRefundEmailAsync(Guid id, double amount)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var order = await orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            string status = L[order.ShippingStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} 退款已完成";

            string body = File.ReadAllText("wwwroot/EmailTemplates/refund.html");

            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNumber}}", order.OrderNo);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{RefundAmount}}", amount.ToString("N0"));

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";
            string orderUrl = $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}";

            body = body.Replace("{{OrderUrl}}", orderUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendMergeOrderEmailAsync(List<Guid> ordersToMergeIds, Guid mergedOrderId)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var orderNumbers = await (await orderRepository.GetQueryableAsync())
                .Where(o => ordersToMergeIds.Contains(o.Id))
                .Select(o => o.OrderNo)
                .Distinct()
                .ToListAsync();

            var order = await orderRepository.GetAsync(mergedOrderId);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {L[order.ShippingStatus.ToString()]}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/merge_order.html");

            body = body.Replace("{{MergedOrder}}", order.OrderNo);
            body = body.Replace("{{OrderToMerge1}}", orderNumbers.FirstOrDefault());
            body = body.Replace("{{OrderToMerge2}}", orderNumbers.LastOrDefault());

            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{ShippingStatus}}", L[order.ShippingStatus.ToString()]);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{PaymentMethod}}", L[order.PaymentMethod.ToString()]);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            body = body.Replace("{{RecipientAddress}}", GetAddress(order));
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{OrderNotes}}", order.Remarks);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";

            body = body.Replace("{{MergedOrderUrl}}", $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}");
            body = body.Replace("{{OrderToMerge1Url}}", $"{groupBuyUrl}/result/{orderNumbers.FirstOrDefault()}/{order.CustomerEmail}");
            body = body.Replace("{{OrderToMerge2Url}}", $"{groupBuyUrl}/result/{orderNumbers.LastOrDefault()}/{order.CustomerEmail}");

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendSplitOrderEmailAsync(Guid orderToSplitId, Guid splitOrderId)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var splitOrder = await orderRepository.FirstOrDefaultAsync(o => o.Id == splitOrderId);

            var order = await orderRepository.GetAsync(orderToSplitId);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {L[order.ShippingStatus.ToString()]}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/split_order.html");

            body = body.Replace("{{OrderToSplit}}", order.OrderNo);
            body = body.Replace("{{SplitOrder1}}", order.OrderNo);
            body = body.Replace("{{SplitOrder2}}", splitOrder?.OrderNo);

            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{ShippingStatus}}", L[order.ShippingStatus.ToString()]);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{PaymentMethod}}", L[order.PaymentMethod.ToString()]);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            body = body.Replace("{{RecipientAddress}}", GetAddress(order));
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{OrderNotes}}", order.Remarks);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";

            body = body.Replace("{{OrderToSplitUrl}}", $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}");
            body = body.Replace("{{SplitOrder1Url}}", $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}");
            body = body.Replace("{{SplitOrder2Url}}", $"{groupBuyUrl}/result/{splitOrder?.OrderNo}/{order.CustomerEmail}");

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendShoppingCreditGrantEmailAsync(Guid userId, decimal amount)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var user = await userManager.GetByIdAsync(userId);
            string subject = "購物金發放通知";

            string body = File.ReadAllText("wwwroot/EmailTemplates/shopping_credit.html");

            body = body.Replace("{{CustomerName}}", user.Name ?? user.UserName);
            body = body.Replace("{{GrantAmount}}", amount.ToString("N2"));

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string? tenantUrl = tenantSettings?.Tenant.GetProperty<string?>(Constant.TenantUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{TenantUrl}}", tenantUrl);

            await SendAsync(user.Email, subject, body);
        }
    }

    public async Task SendVipTierUpgradeEmailAsync(List<VipTierUpgradeEmailDto> inputs)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            string body = File.ReadAllText("wwwroot/EmailTemplates/vip_tier_upgrade.html");

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string? tenantUrl = tenantSettings?.Tenant.GetProperty<string?>(Constant.TenantUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink);
            body = body.Replace("{{LineUrl}}", tenantSettings?.LineLink);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{TenantUrl}}", tenantUrl);

            foreach (var input in inputs)
            {
                if (input.NewTier == null) continue;

                var subject = $"🎉 恭喜！您已升級至 {input.NewTier?.TierName}";
                var enSubject = $"🎉 Congratulations! You've been upgraded to {input.NewTier?.TierName}";

                var requiredOrders = input.NextTier == null ? 0 : input.NextTier.OrdersCount - input.NewTier?.OrdersCount ?? 0;
                var requiredAmount = input.NextTier == null ? 0 : input.NextTier.OrdersAmount - input.NewTier?.OrdersAmount ?? 0;

                string personalizedBody = body
                    .Replace("{{CustomerName}}", input.UserName)
                    .Replace("{{PreviousTierName}}", input.PreviousTier?.TierName)
                    .Replace("{{NewTierName}}", input.NewTier?.TierName)
                    .Replace("{{NextTierName}}", input.NextTier?.TierName)
                    .Replace("{{RequiredOrders}}", requiredOrders.ToString())
                    .Replace("{{RequiredAmount}}", requiredAmount.ToString("N2"));

                await SendAsync(input.Email, subject, personalizedBody);
            }
        }
    }

    public async Task SendWalletDeductedEmailAsync(string email, string tenantName, decimal amount, string transactionType, decimal currentBalance)
    {
        {


            var subject = L["WalletDeductionNotification"]; // optional: localize subject too

            var body = $@"
    <html>
    <body style='font-family:Segoe UI, Microsoft JhengHei, Arial,sans-serif; font-size:15px; color:#333;'>
        <p>{L["WalletEmail.Greeting", tenantName]}</p>
        <p>{L["WalletEmail.DeductionBody"]}</p>

        <p><strong>{L["WalletEmail.DeductioAmount"]}</strong> ${amount.ToString("N0")}<br/>
        <strong>{L["WalletEmail.Type"]}</strong> {L["WalletTransactionType:" + transactionType.ToString()]}<br/>
        <strong>{L["WalletEmail.Balance"]}</strong> ${currentBalance.ToString("N0")}</p>

        <p>{L["WalletEmail.SupportDeduction"]}</p>
        <br/>
        <p>{L["WalletEmail.Regards"]}</p>
    </body>
    </html>";

            await emailSender.SendAsync(email, subject, body, isBodyHtml: true);
        }
    }

    public async Task SendWalletRechargeEmailAsync(string email, string tenantName, decimal amount, string transactionType, decimal currentBalance)
    {
        {


            var subject = L["WalletRechargeNotification"]; // optional: localize subject too

            var body = $@"
    <html>
    <body style='font-family:Segoe UI, Microsoft JhengHei, Arial,sans-serif; font-size:15px; color:#333;'>
        <p>{L["WalletEmail.Greeting", tenantName]}</p>
        <p>{L["WalletEmail.RechargeBody"]}</p>

        <p><strong>{L["WalletEmail.RechargeAmount"]}</strong> ${amount.ToString("N0")}<br/>
        <strong>{L["WalletEmail.Type"]}</strong> {L["WalletTransactionType:" + transactionType.ToString()]}<br/>
        <strong>{L["WalletEmail.Balance"]}</strong> ${currentBalance.ToString("N0")}</p>

        <p>{L["WalletEmail.SupportRecharge"]}</p>
        <br/>
        <p>{L["WalletEmail.RegardsRecharge"]}</p>
    </body>
    </html>";

            await emailSender.SendAsync(email, subject, body, isBodyHtml: true);
        }
    }
    
    private async Task SendAsync(string email, string subject, string body)
    {
        try
        {
            await emailSender.SendAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("This mail account has sent too many messages"))
            {
                throw;
            }

            Logger.LogException(ex);

            await backgroundJobManager.EnqueueAsync(new BackgroundEmailSendingJobArgs
            {
                To = email,
                Subject = subject,
                Body = body
            }, delay: TimeSpan.FromHours(1));

            var restClient = new RestClient(configuration["App:EmailQuotaExceededNotifyUrl"]);
            var restRequest = new RestRequest("", Method.Get);
            await restClient.ExecuteAsync(restRequest);
        }
    }

    private static string? GetAddress(Order order)
    {
        List<DeliveryMethod> cvsStoreMethods = [
            DeliveryMethod.SevenToEleven1,
            DeliveryMethod.SevenToElevenFrozen,
            DeliveryMethod.SevenToElevenC2C,
            DeliveryMethod.TCatDeliverySevenElevenNormal,
            DeliveryMethod.TCatDeliverySevenElevenFreeze,
            DeliveryMethod.TCatDeliverySevenElevenFrozen,
            DeliveryMethod.FamilyMart1,
            DeliveryMethod.FamilyMartC2C
        ];

        return (order.DeliveryMethod.HasValue && cvsStoreMethods.Contains(order.DeliveryMethod.Value))
            ? order.CVSStoreOutSide
            : $"{order.City} {order.AddressDetails}";
    }
}
