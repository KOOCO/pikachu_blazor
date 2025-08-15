using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Kooco.Pikachu.InboxManagement.NotificationKeys.Orders;
using static Kooco.Pikachu.InboxManagement.NotificationParams;

namespace Kooco.Pikachu.InboxManagement.Managers;

public partial class NotificationManager
{
    private async Task<Notification> CreateOrderNotificationAsync(
        NotificationType type,
        string title,
        string message,
        NotificationParamInput input)
    {
        var dict = GetOrderParamsDict(input);
        return await CreateAsync(
            type,
            title,
            message,
            dict,
            typeof(Order).FullName,
            input.OrderIdStr
        );
    }

    public Task<Notification> OrderCreatedAsync(NotificationParamInput input, PaymentMethods? paymentMethod)
    {
        var (type, title, message) = OrderCreateParams(paymentMethod);
        return CreateOrderNotificationAsync(type, title, message, input);
    }

    public Task<Notification> ManualBankTransferConfirmedAsync(NotificationParamInput input)
    {
        return CreateOrderNotificationAsync(
            NotificationType.BankTransfer,
            ManualBankTransferConfirmedTitle,
            ManualBankTransferConfirmedMessage,
            input
        );
    }

    public Task<Notification> PaymentMethodUpdatedAsync(NotificationParamInput input)
    {
        return CreateOrderNotificationAsync(
            NotificationType.Payment,
            PaymentMethodUpdatedTitle,
            PaymentMethodUpdatedMessage,
            input
        );
    }

    static (NotificationType, string, string) OrderCreateParams(PaymentMethods? paymentMethod)
    {
        return paymentMethod == PaymentMethods.ManualBankTransfer
            ? (
                NotificationType.BankTransfer,
                ManualBankTransferTitle,
                ManualBankTransferMessage
            )
            : (
                NotificationType.Order,
                CreatedTitle,
                CreatedMessage
            );
    }

    static Dictionary<string, string> GetOrderParamsDict(NotificationParamInput input)
    {
        var dict = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(input.OrderIdStr)) dict[OrderId] = input.OrderIdStr;
        if (!string.IsNullOrWhiteSpace(input.OrderNo)) dict[OrderNo] = input.OrderNo;
        if (!string.IsNullOrWhiteSpace(input.UserName)) dict[UserName] = input.UserName;
        if (input.OldPaymentMethod.HasValue) dict[OldPaymentMethod] = input.OldPaymentMethod.ToString();
        if (input.NewPaymentMethod.HasValue) dict[NewPaymentMethod] = input.NewPaymentMethod.ToString();

        return dict;
    }
}
