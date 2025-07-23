using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.Orders;

public class OrderConsts
{
    public static readonly List<ShippingStatus> CompletedShippingStatus = [ShippingStatus.Completed, ShippingStatus.Closed];
}
public static class OrderNotificationNames
{
    public const string NewMessage = "Order.NewMessage";
    public const string MessageRead = "Order.MessageRead";
    public const string ManualPaymentPending = "Order.ManualPaymentPending";
    public const string PaymentConfirmed = "Order.PaymentConfirmed";
}
