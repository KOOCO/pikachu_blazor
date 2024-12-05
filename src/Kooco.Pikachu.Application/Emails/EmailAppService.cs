using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.TenantManagement;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Emailing;

namespace Kooco.Pikachu.Emails;

public class EmailAppService(IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository,
    ITenantSettingsAppService tenantSettingsAppService, IEmailSender emailSender) : PikachuAppService, IEmailAppService
{
    public async Task SendLogisticsEmailAsync(OrderDeliveryDto input)
    {
        var order = await orderRepository.GetAsync(input.OrderId);
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
        body = body.Replace("{{DeliveryNumber}}", input.DeliveryNo);
        body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
        body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
        body = body.Replace("{{PaymentMethod}}", L[order.PaymentMethod.ToString()]);
        body = body.Replace("{{RecipientName}}", order.RecipientName);
        body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
        body = body.Replace("{{RecipientAddress}}", order.AddressDetails);
        body = body.Replace("{{ShippingMethod}}", $"{L[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
        body = body.Replace("{{OrderNotes}}", order.Remarks);

        var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();
        body = body.Replace("{{LogoUrl}}", tenantSettings?.LogoUrl);
        body = body.Replace("{{FacebookUrl}}", tenantSettings?.Facebook);
        body = body.Replace("{{InstagramUrl}}", tenantSettings?.Instagram);
        body = body.Replace("{{LineUrl}}", tenantSettings?.Line);

        body = body.Replace("{{CurrentYear}}", DateTime.Today.ToString("yyyy"));
        body = body.Replace("{{CompanyName}}", tenantSettings?.CompanyName);
        body = body.Replace("{{GroupBuyUrl}}", tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.TrimEnd('/') + "/" + groupbuy.Id);

        await emailSender.SendAsync(order.CustomerEmail, subject, body);
    }
}
