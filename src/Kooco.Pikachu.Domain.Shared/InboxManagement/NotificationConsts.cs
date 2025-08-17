namespace Kooco.Pikachu.InboxManagement;

#pragma warning disable RS0016 // Add public types and members to the declared API
public static class NotificationConsts
{
    public const string DefaultSorting = "CreationTime DESC";
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

    public static class Orders
    {
        public const string OrderPrefix = Prefix + "Orders:";
        public const string CreatedTitle = OrderPrefix + "CreatedTitle";
        public const string CreatedMessage = OrderPrefix + "CreatedMessage";
        public const string ManualBankTransferTitle = OrderPrefix + "ManualBankTransferTitle";
        public const string ManualBankTransferMessage = OrderPrefix + "ManualBankTransferMessage";
        public const string ManualBankTransferConfirmedTitle = OrderPrefix + "ManualBankTransferConfirmedTitle";
        public const string ManualBankTransferConfirmedMessage = OrderPrefix + "ManualBankTransferConfirmedMessage";
        public const string PaymentMethodUpdatedTitle = OrderPrefix + "PaymentMethodUpdatedTitle";
        public const string PaymentMethodUpdatedMessage = OrderPrefix + "PaymentMethodUpdatedMessage";
    }
}

public static class NotificationParams
{
    public const string OrderId = "OrderId";
    public const string OrderNo = "OrderNo";
    public const string UserName = "UserName";
    public const string OldPaymentMethod = "OldPaymentMethod";
    public const string NewPaymentMethod = "NewPaymentMethod";
}