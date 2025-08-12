namespace Kooco.Pikachu.InboxManagement;

public static class NotificationConsts
{
    public const string DefaultSorting = "CreationTime DESC";
    public const int MaxTitleLength = 256;
    public const int MaxMessageLength = 256;
    public const int MaxParamsJsonLength = 1024;
    public const int MaxEntityIdLength = 50;
}

public static class NotificationKeys
{
    public const string Prefix = "Notification:";
    public static class Orders
    {
        public const string OrderPrefix = Prefix + "Orders:";
        public const string CreatedTitle = OrderPrefix + "CreatedTitle";
        public const string CreatedMessage = OrderPrefix + "CreatedMessage";
    }
}

public static class NotificationParams
{
    public static class Orders
    {
        public const string OrderId = "OrderId";
        public const string OrderNo = "OrderNo";
    }
}