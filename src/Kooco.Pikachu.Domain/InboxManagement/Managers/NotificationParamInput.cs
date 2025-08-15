using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.InboxManagement.Managers;

public class NotificationParamInput
{
    public string? OrderIdStr { get; private set; }
    public string? OrderNo { get; private set; }
    public string? UserName { get; private set; }
    public PaymentMethods? OldPaymentMethod { get; private set; }
    public PaymentMethods? NewPaymentMethod { get; private set; }

    private NotificationParamInput() { }

    public static NotificationParamInput ForOrderCreated(Guid orderId, string orderNo)
        => new()
        {
            OrderIdStr = orderId.ToString(),
            OrderNo = orderNo
        };

    public static NotificationParamInput ForBankTransferConfirmed(Guid orderId, string orderNo, string userName)
        => new()
        {
            OrderIdStr = orderId.ToString(),
            OrderNo = orderNo,
            UserName = userName
        };

    public static NotificationParamInput ForPaymentMethodUpdated(
        Guid orderId, 
        string orderNo, 
        PaymentMethods? oldMethod, 
        PaymentMethods? newMethod, 
        string userName
        )
        => new()
        {
            OrderIdStr = orderId.ToString(),
            OrderNo = orderNo,
            OldPaymentMethod = oldMethod,
            NewPaymentMethod = newMethod,
            UserName = userName
        };
}
