using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.InboxManagement.Managers;

public class NotificationArgs
{
    public Guid? OrderId { get; private set; }
    public string? OrderIdStr => OrderId?.ToString();
    public string? OrderNo { get; private set; }
    public string? UserName { get; private set; }
    public PaymentMethods? OldPaymentMethod { get; private set; }
    public PaymentMethods? NewPaymentMethod { get; private set; }

    private NotificationArgs() { }

    public static NotificationArgs ForOrderCreated(
        Guid orderId,
        string orderNo
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo
        };

    public static NotificationArgs ForBankTransferConfirmed(
        Guid orderId,
        string orderNo,
        string? userName
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName
        };

    public static NotificationArgs ForPaymentMethodUpdated(
        Guid orderId,
        string orderNo,
        PaymentMethods? oldMethod,
        PaymentMethods? newMethod,
        string? userName
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            OldPaymentMethod = oldMethod,
            NewPaymentMethod = newMethod,
            UserName = userName
        };

    public static NotificationArgs ForOrdersMergedOrSplit(
        Guid orderId,
        string orderNo,
        string? userName
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName
        };

    public static NotificationArgs ForOrderRefund(
        Guid orderId,
        string orderNo,
        string? userName
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName
        };
}
