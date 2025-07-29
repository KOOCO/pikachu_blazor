using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Emails;

public interface IEmailAppService : IApplicationService
{
    Task SendLogisticsEmailAsync(Guid orderId, string? deliveryNo = "", DeliveryMethod? deliveryMethod = null);
    Task SendOrderStatusEmailAsync(Guid id, OrderStatus? orderStatus = null, ShippingStatus? shippingStatus = null);
    Task SendOrderUpdateEmailAsync(Guid id);
    Task SendRefundEmailAsync(Guid id, double amount);
    Task SendMergeOrderEmailAsync(List<Guid> ordersToMergeIds, Guid mergedOrderId);
    Task SendSplitOrderEmailAsync(Guid orderToSplitId, Guid splitOrderId);
    Task SendShoppingCreditGrantEmailAsync(Guid userId, decimal amount);
    Task SendWalletRechargeEmailAsync(string email, string tenantName, decimal amount, string transactionType, decimal currentBalance);
    Task SendWalletDeductedEmailAsync(string email, string tenantName, decimal amount, string transactionType, decimal currentBalance);
}
