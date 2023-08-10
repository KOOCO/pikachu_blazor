namespace Kooco.Pikachu.Permissions;

public static class PikachuPermissions
{
    public const string GroupName = "GroupBuySetting";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public const string GroupBuyList = "團購列表";
    public const string GroupBuyReport = "團購報表";
    public const string GoodsList = "商品列表";
    public const string GoodsGroupingSetting = "商品組合設定";
    public const string StockReport = "庫存報表";
    public const string FreebieSetting = "贈品設定";
    public const string POList = "訂單列表";
    public const string POReturningList = "退換貨列表";
    public const string RefundAuditList = "退款審核列表";
    public const string PaymentGatewaySetting = "金流商設定";
    public const string InvoiceSetting = "電子發票設定";
    public const string LogisticsSetting = "物流商設定";
    public const string TenentList = "商戶列表";
    public const string TenentBillReport = "商戶帳單報表";
    public const string PermissionSetting = "權限管理";
    /// <summary>
    /// 
    /// </summary>
    public class Item
    {
        public const string Default = GroupName + ".Item";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    /// <summary>
    /// 
    /// </summary>
    public class SetItem
    {
        public const string Default = GroupName + ".SetItem";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    /// <summary>
    /// 
    /// </summary>
    public class SetItemDetails
    {
        public const string Default = GroupName + ".SetItemDetails";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
