using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Kooco.Pikachu.InboxManagement.NotificationParams;
using static Kooco.Pikachu.InboxManagement.NotificationKeys.Orders;

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

    public Task OrderUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            UpdatedTitle,
            UpdatedMessage,
            input
        );
    }

    public Task OrderItemsUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrderItemsUpdatedTitle,
            OrderItemsUpdatedMessage,
            input
            );
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

    public Task ShippingStatusUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            ShippingStatusUpdatedTitle,
            ShippingStatusUpdatedMessage,
            input
            );
    }

    public Task OrderStatusUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrderStatusUpdatedTitle,
            OrderStatusUpdatedMessage,
            input
            );
    }

    public Task ReturnRequestedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Return,
            ReturnRequestedTitle,
            ReturnRequestedMessage,
            input
            );
    }

    public Task ExchangeRequestedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Exchange,
            ExchangeRequestedTitle,
            ExchangeRequestedMessage,
            input
            );
    }

    public Task ReturnStatusUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Return,
            ReturnStatusUpdatedTitle,
            ReturnStatusUpdatedMessage,
            input
            );
    }

    public Task OrderCancelledAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrderCancelledTitle,
            OrderCancelledMessage,
            input
            );
    }

    public Task InvoiceVoidedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            InvoiceVoidedTitle,
            InvoiceVoidedMessage,
            input
            );
    }

    public Task CreditNoteIssuedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            CreditNoteIssuedTitle,
            CreditNoteIssuedMessage,
            input
            );
    }

    public Task ShippingDetailsUpdatedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            ShippingDetailsUpdatedTitle,
            ShippingDetailsUpdatedMessage,
            input
            );
    }

    public Task OrderCompletedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrderCompletedTitle,
            OrderCompletedMessage,
            input
            );
    }

    public Task OrderClosedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            OrderClosedTitle,
            OrderClosedMessage,
            input
            );
    }

    public Task InactiveOrdersClosedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Order,
            InactiveOrdersClosedTitle,
            InactiveOrdersClosedMessage,
            input
            );
    }

    public Task PaymentProcessedAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
            NotificationType.Payment,
            PaymentProcessedTitle,
            PaymentProcessedMessage,
            input
            );
    }

    public Task OrderExpiredAsync(NotificationArgs input)
    {
        return CreateOrderNotificationSafeAsync(
           NotificationType.Order,
           OrderExpiredTitle,
           OrderExpiredMessage,
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
        if (input.Count.HasValue) dict[Count] = input.Count.Value.ToString("N0");

        if (input.PreviousPaymentMethod.HasValue) dict[PreviousPaymentMethod] = input.PreviousPaymentMethod.ToString();
        if (input.PaymentMethod.HasValue) dict[PaymentMethod] = input.PaymentMethod.ToString();

        if (input.PreviousShippingStatus.HasValue) dict[PreviousShippingStatus] = input.PreviousShippingStatus.ToString();
        if (input.ShippingStatus.HasValue) dict[NotificationParams.ShippingStatus] = input.ShippingStatus.ToString();

        if (input.PreviousOrderStatus.HasValue) dict[PreviousOrderStatus] = input.PreviousOrderStatus.ToString();
        if (input.OrderStatus.HasValue) dict[NotificationParams.OrderStatus] = input.OrderStatus.ToString();

        if (input.PreviousReturnStatus.HasValue) dict[PreviousReturnStatus] = input.PreviousReturnStatus.ToString();
        if (input.ReturnStatus.HasValue) dict[ReturnStatus] = input.ReturnStatus.ToString();

        return dict;
    }
}
