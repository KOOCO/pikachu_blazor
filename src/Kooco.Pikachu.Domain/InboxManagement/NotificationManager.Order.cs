using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.InboxManagement;

public partial class NotificationManager
{
    public async Task<Notification> OrderCreatedAsync(Guid orderId, string orderNo)
    {
        string orderIdStr = orderId.ToString();
        var paramDict = GetOrderParamsDict(orderIdStr, orderNo);

        return await CreateAsync(
            NotificationType.Info,
            NotificationKeys.Orders.CreatedTitle,
            NotificationKeys.Orders.CreatedMessage,
            paramDict,
            paramDict,
            paramDict,
            NotificationEntityType.Order,
            orderIdStr
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
