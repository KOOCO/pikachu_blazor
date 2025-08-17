using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Kooco.Pikachu.InboxManagement.NotificationKeys.Orders;
using static Kooco.Pikachu.InboxManagement.NotificationParams;

namespace Kooco.Pikachu.InboxManagement.Managers;

public partial class NotificationManager
{
    private async Task<Notification?> CreateOrderNotificationSafeAsync(
        NotificationType type,
        string title,
        string message,
        NotificationArgs input
        )
    {
        var dict = GetOrderParamsDict(input);

        return await CreateSafeAsync(
            type,
            title,
            message,
            dict,
            typeof(Order).FullName,
            input.OrderIdStr,
            contextMessage: $"OrderId: {input.OrderIdStr}, OrderNo: {input.OrderNo}"
        );
    }

    public Task OrderCreatedAsync(NotificationArgs input, PaymentMethods? paymentMethod)
    {
        var (type, title, message) = OrderCreateParams(paymentMethod);
        return CreateOrderNotificationSafeAsync(type, title, message, input);
    }

    public Task ManualBankTransferConfirmedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.BankTransfer,
            ManualBankTransferConfirmedTitle,
            ManualBankTransferConfirmedMessage,
            input
        );
    }

    public Task PaymentMethodUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Payment,
            PaymentMethodUpdatedTitle,
            PaymentMethodUpdatedMessage,
            input
        );
    }

    public Task OrdersMergedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrdersMergedTitle,
            OrdersMergedMessage,
            input
        );
    }

    public Task OrderSplitAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrdersSplitTitle,
            OrdersSplitMessage,
            input
        );
    }

    public Task RefundRequestedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Refund,
            RefundRequestedTitle,
            RefundRequestedMessage,
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

    static Dictionary<string, string> GetOrderParamsDict(NotificationArgs input)
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
