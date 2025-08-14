using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.InboxManagement.Managers;

public partial class NotificationManager
{
    public async Task<Notification> OrderCreatedAsync(Guid orderId, string orderNo, PaymentMethods? paymentMethod)
    {
        var (type, title, message) = OrderCreateParams(paymentMethod);
        string orderIdStr = orderId.ToString();
        var paramDict = GetOrderParamsDict(orderIdStr, orderNo);

        return await CreateAsync(
            type,
            title,
            message,
            paramDict,
            paramDict,
            paramDict,
            typeof(Order).FullName,
            orderIdStr
        );
    }

    static (NotificationType, string, string) OrderCreateParams(PaymentMethods? paymentMethod)
    {
        return paymentMethod == PaymentMethods.ManualBankTransfer
            ? (
                NotificationType.Order,
                NotificationKeys.Orders.ManualBankTransferTitle,
                NotificationKeys.Orders.ManualBankTransferMessage
            )
            : (
                NotificationType.Order,
                NotificationKeys.Orders.CreatedTitle,
                NotificationKeys.Orders.CreatedMessage
            );
    }

    static Dictionary<string, string> GetOrderParamsDict(string orderId, string orderNo)
    {
        return new Dictionary<string, string>
        {
            { NotificationParams.Orders.OrderId, orderId },
            { NotificationParams.Orders.OrderNo, orderNo }
        };
    }
}
