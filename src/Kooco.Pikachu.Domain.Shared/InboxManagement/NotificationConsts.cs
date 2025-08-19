using System;

namespace Kooco.Pikachu.InboxManagement;

#pragma warning disable RS0016 // Add public types and members to the declared API
public static class NotificationConsts
{
    public const string DefaultSorting = "NotificationTimeUtc DESC";
    public const int MaxTitleLength = 256;
    public const int MaxMessageLength = 256;
    public const int MaxParamsJsonLength = 2048;
    public const int MaxEntityNameLength = 1024;
    public const int MaxEntityIdLength = 50;
}

public static class NotificationKeys
{
    public const string Prefix = "Notification:";
    public const string UnreadCount = Prefix + "UnreadCount";
    public const string TenantGroup = "Tenant:{0}";
    public const string HostGroup = "Host";

    public static string NotificationGroup(Guid? tenantId)
    {
        return tenantId.HasValue
            ? string.Format(TenantGroup, tenantId.ToString())
            : HostGroup;
    }

    public static class Orders
    {
        public const string OrderPrefix = Prefix + "Orders:";

        public const string CreatedTitle = OrderPrefix + "CreatedTitle";
        public const string CreatedMessage = OrderPrefix + "CreatedMessage";

        public const string UpdatedTitle = OrderPrefix + "UpdatedTitle";
        public const string UpdatedMessage = OrderPrefix + "UpdatedMessage";

        public const string ManualBankTransferTitle = OrderPrefix + "ManualBankTransferTitle";
        public const string ManualBankTransferMessage = OrderPrefix + "ManualBankTransferMessage";

        public const string ManualBankTransferConfirmedTitle = OrderPrefix + "ManualBankTransferConfirmedTitle";
        public const string ManualBankTransferConfirmedMessage = OrderPrefix + "ManualBankTransferConfirmedMessage";

        public const string PaymentMethodUpdatedTitle = OrderPrefix + "PaymentMethodUpdatedTitle";
        public const string PaymentMethodUpdatedMessage = OrderPrefix + "PaymentMethodUpdatedMessage";

        public const string OrdersMergedTitle = OrderPrefix + "OrdersMergedTitle";
        public const string OrdersMergedMessage = OrderPrefix + "OrdersMergedMessage";

        public const string OrdersSplitTitle = OrderPrefix + "OrdersSplitTitle";
        public const string OrdersSplitMessage = OrderPrefix + "OrdersSplitMessage";

        public const string RefundRequestedTitle = OrderPrefix + "RefundRequestedTitle";
        public const string RefundRequestedMessage = OrderPrefix + "RefundRequestedMessage";

        public const string ShippingStatusUpdatedTitle = OrderPrefix + "ShippingStatusUpdatedTitle";
        public const string ShippingStatusUpdatedMessage = OrderPrefix + "ShippingStatusUpdatedMessage";

        public const string OrderStatusUpdatedTitle = OrderPrefix + "OrderStatusUpdatedTitle";
        public const string OrderStatusUpdatedMessage = OrderPrefix + "OrderStatusUpdatedMessage";

        public const string ReturnRequestedTitle = OrderPrefix + "ReturnRequestedTitle";
        public const string ReturnRequestedMessage = OrderPrefix + "ReturnRequestedMessage";

        public const string ExchangeRequestedTitle = OrderPrefix + "ExchangeRequestedTitle";
        public const string ExchangeRequestedMessage = OrderPrefix + "ExchangeRequestedMessage";

        public const string ReturnStatusUpdatedTitle = OrderPrefix + "ReturnStatusUpdatedTitle";
        public const string ReturnStatusUpdatedMessage = OrderPrefix + "ReturnStatusUpdatedMessage";

        public const string OrderItemsUpdatedTitle = OrderPrefix + "OrderItemsUpdatedTitle";
        public const string OrderItemsUpdatedMessage = OrderPrefix + "OrderItemsUpdatedMessage";

        public const string OrderCancelledTitle = OrderPrefix + "OrderCancelledTitle";
        public const string OrderCancelledMessage = OrderPrefix + "OrderCancelledMessage";

        public const string InvoiceVoidedTitle = OrderPrefix + "InvoiceVoidedTitle";
        public const string InvoiceVoidedMessage = OrderPrefix + "InvoiceVoidedMessage";

        public const string CreditNoteIssuedTitle = OrderPrefix + "CreditNoteIssuedTitle";
        public const string CreditNoteIssuedMessage = OrderPrefix + "CreditNoteIssuedMessage";

        public const string ShippingDetailsUpdatedTitle = OrderPrefix + "ShippingDetailsUpdatedTitle";
        public const string ShippingDetailsUpdatedMessage = OrderPrefix + "ShippingDetailsUpdatedMessage";

        public const string OrderCompletedTitle = OrderPrefix + "OrderCompletedTitle";
        public const string OrderCompletedMessage = OrderPrefix + "OrderCompletedMessage";

        public const string OrderClosedTitle = OrderPrefix + "OrderClosedTitle";
        public const string OrderClosedMessage = OrderPrefix + "OrderClosedMessage";

        public const string InactiveOrdersClosedTitle = OrderPrefix + "InactiveOrdersClosedTitle";
        public const string InactiveOrdersClosedMessage = OrderPrefix + "InactiveOrdersClosedMessage";

        public const string PaymentProcessedTitle = OrderPrefix + "PaymentProcessedTitle";
        public const string PaymentProcessedMessage = OrderPrefix + "PaymentProcessedMessage";

        public const string OrderExpiredTitle = OrderPrefix + "OrderExpiredTitle";
        public const string OrderExpiredMessage = OrderPrefix + "OrderExpiredMessage";

        public const string NewOrderMessageTitle = OrderPrefix + "NewOrderMessageTitle";
        public const string NewOrderMessageMessage = OrderPrefix + "NewOrderMessageMessage";
    }
}

public static class NotificationParams
{
    public const string OrderId = "OrderId";
    public const string OrderNo = "OrderNo";
    public const string UserName = "UserName";
    public const string Count = "Count";

    public const string PreviousPaymentMethod = "PreviousPaymentMethod";
    public const string PaymentMethod = "PaymentMethod";

    public const string PreviousShippingStatus = "PreviousShippingStatus";
    public const string ShippingStatus = "ShippingStatus";

    public const string PreviousOrderStatus = "PreviousOrderStatus";
    public const string OrderStatus = "OrderStatus";

    public const string PreviousReturnStatus = "PreviousReturnStatus";
    public const string ReturnStatus = "ReturnStatus";
}