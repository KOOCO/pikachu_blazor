using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Emails;

public interface IEmailAppService : IApplicationService
{
    Task SendLogisticsEmailAsync(Guid orderId, string? deliveryNo = "");
    Task SendOrderStatusEmailAsync(Guid id, OrderStatus? orderStatus = null, ShippingStatus? shippingStatus = null);
    Task SendOrderUpdateEmailAsync(Guid id);
    Task SendRefundEmailAsync(Guid id, double amount);
    Task SendMergeOrderEmailAsync(List<Guid> ordersToMergeIds, Guid mergedOrderId);
    Task SendSplitOrderEmailAsync(Guid orderToSplitId, Guid splitOrderId);
}
