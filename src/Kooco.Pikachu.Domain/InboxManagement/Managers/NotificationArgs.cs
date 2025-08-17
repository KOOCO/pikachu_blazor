using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.InboxManagement.Managers;

public class NotificationArgs
{
    public Guid? OrderId { get; private set; }
    public string? OrderIdStr => OrderId?.ToString();
    public string? OrderNo { get; private set; }
    public string? UserName { get; private set; }
    
    public PaymentMethods? PreviousPaymentMethod { get; private set; }
    public PaymentMethods? PaymentMethod { get; private set; }
    
    public ShippingStatus? PreviousShippingStatus { get; private set; }
    public ShippingStatus? ShippingStatus { get; private set; }
    
    public OrderStatus? PreviousOrderStatus { get; private set; }
    public OrderStatus? OrderStatus { get; private set; }

    public OrderReturnStatus? PreviousReturnStatus { get; private set; }
    public OrderReturnStatus? ReturnStatus { get; private set; }

    private NotificationArgs() { }

    public static NotificationArgs ForOrderCreated(
        Guid orderId,
        string orderNo
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo
        };

    public static NotificationArgs ForOrderWithUserName(
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
        PaymentMethods? previousPaymentMethod,
        PaymentMethods? paymentMethod,
        string? userName
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            PreviousPaymentMethod = previousPaymentMethod,
            PaymentMethod = paymentMethod,
            UserName = userName
        };

    public static NotificationArgs ForShippingStatusUpdated(
        Guid orderId,
        string orderNo,
        string? userName,
        ShippingStatus? previousShippingStatus,
        ShippingStatus? shippingStatus
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName,
            PreviousShippingStatus = previousShippingStatus,
            ShippingStatus = shippingStatus
        };

    public static NotificationArgs ForOrderStatusUpdated(
        Guid orderId,
        string orderNo,
        string? userName,
        OrderStatus? previousOrderStatus,
        OrderStatus? orderStatus
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName,
            PreviousOrderStatus = previousOrderStatus,
            OrderStatus = orderStatus
        };

    public static NotificationArgs ForReturnStatusUpdated(
        Guid orderId,
        string orderNo,
        string? userName,
        OrderReturnStatus? previousReturnStatus,
        OrderReturnStatus? returnStatus
        ) => new()
        {
            OrderId = orderId,
            OrderNo = orderNo,
            UserName = userName,
            PreviousReturnStatus = previousReturnStatus,
            ReturnStatus = returnStatus
        };
}
