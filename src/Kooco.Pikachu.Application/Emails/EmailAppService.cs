using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;

namespace Kooco.Pikachu.Emails;

public class EmailAppService(IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository,
    ITenantSettingsAppService tenantSettingsAppService, IEmailSender emailSender) : PikachuAppService, IEmailAppService
{
    public async Task SendLogisticsEmailAsync(Guid orderId, string? deliveryNo = "")
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
            body = body.Replace("{{RecipientAddress}}", $"{order.City} {order.AddressDetails}");
            body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{OrderNotes}}", order.Remarks);

            var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();

            string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{groupbuy.Id}";
            string orderUrl = $"{groupBuyUrl}/result/{order.OrderNo}/{order.CustomerEmail}";

            body = body.Replace("{{OrderUrl}}", orderUrl);

            body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.Facebook);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.Instagram);
            body = body.Replace("{{LineUrl}}", tenantSettings?.Line);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await emailSender.SendAsync(order.CustomerEmail, subject, body);
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
            body = body.Replace("{{RecipientAddress}}", $"{order.City} {order.AddressDetails}");
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
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.Facebook);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.Instagram);
            body = body.Replace("{{LineUrl}}", tenantSettings?.Line);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await emailSender.SendAsync(order.CustomerEmail, subject, body);
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
            body = body.Replace("{{RecipientAddress}}", $"{order.City} {order.AddressDetails}");
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
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.Facebook);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.Instagram);
            body = body.Replace("{{LineUrl}}", tenantSettings?.Line);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await emailSender.SendAsync(order.CustomerEmail, subject, body);
        }
    }

    public async Task SendRefundEmailAsync(Guid id, double amount)
    {
        using (CultureHelper.Use(CultureInfo.GetCultureInfo("zh-Hant")))
        {
            var order = await orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            string status = L[order.ShippingStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

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
            body = body.Replace("{{FacebookUrl}}", tenantSettings?.Facebook);
            body = body.Replace("{{InstagramUrl}}", tenantSettings?.Instagram);
            body = body.Replace("{{LineUrl}}", tenantSettings?.Line);

            body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
            body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
            body = body.Replace("{{GroupBuyUrl}}", groupBuyUrl);

            await emailSender.SendAsync(order.CustomerEmail, subject, body);
        }
    }
}
