using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Permissions;

public static class PikachuPermissions
{
    public const string GroupBuyManagement = "GroupBuyManagement";
    public const string ProductManagement = "ProductManagement";
    public const string OrderManagement = "OrderManagement";
    public const string PaymentManagement = "PaymentManagement";
    public const string LogisticsManagement = "LogisticsManagement";
    public const string SystemManagement = "SystemManagement";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public const string GroupBuyList = "團購列表";
    public const string GroupBuyReport = "團購報表";
    public const string GroupBuyPageConfig = "團購頁面設定";
    public const string GoodsList = "商品列表";
    public const string InventoryReport = "庫存報表";
    public const string ReturnExchangeOrder = "退換貨列表";
    public const string GoodsGroupingSetting = "商品組合設定";
    //public const string StockReport = "庫存報表";
    public const string FreebieSetting = "贈品設定";
    public const string POList = "訂單列表";
    //public const string POReturningList = "退換貨列表";
    public const string RefundAuditList = "退款審核列表";
    public const string PaymentGatewaySetting = "金流商設定";
    public const string InvoiceSetting = "電子發票設定";
    public const string LogisticsSetting = "物流商設定";
    public const string TenentList = "商戶列表";
    public const string TenentBillReport = "商戶帳單報表";
    public const string PermissionSetting = "權限管理";
    public const string CashFlowDealerSettings = "CashFlowDealerSettings";
    public const string CashFlowReconciliationStatement = "金流對帳表";
    public const string SaleReport = "SaleReport";
    public const string EmailSettings = "EmailSettings";
    public const string ReportNotification = "ReportNotification";
    public const string DeliveryTemperatureCost = "DeliveryTemperatureCost";

    public const string MembersManagement = "MembersManagement";
    public const string WebSiteSettings = "WebsiteSettings";
    public const string Promotions = "Promotions";

    public const string WebsiteManagementGroup = "WebsiteManagement";
    public const string EdmManagementGroup = "EdmMangement";
    /// <summary>
    /// 
    /// </summary>
    public class Item
    {
        public const string Default = ProductManagement + ".Item";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    /// <summary>
    /// 
    /// </summary>
    public class SetItem
    {
        public const string Default = ProductManagement + ".SetItem";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    /// <summary>
    /// 
    /// </summary>
    public class SetItemDetails
    {
        public const string Default = ProductManagement + ".SetItemDetails";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public class Orders
    {
        public const string Default = OrderManagement + ".Orders";
        public const string AddStoreComment = OrderManagement + ".AddStoreComments";
    }

    public class Refund
    {
        public const string Default = OrderManagement + ".Refunds";
        public const string Create = Default + ".Create";
        public const string RefundOrderProcess = Default + "RefundProcess";
    }

    public static class Members
    {
        public const string Default = MembersManagement + ".Members";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class UserAddresses
    {
        public const string Default = MembersManagement + ".UserAddresses";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string SetIsDefault = Default + ".SetIsDefault";
    }

    public static class UserShoppingCredits
    {
        public const string Default = MembersManagement + ".UserShoppingCredits";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string SetIsActive = Default + ".SetIsActive";
    }

    public static class ShopCarts
    {
        public const string Default = MembersManagement + ".ShopCarts";
    }

    public static class AddOnProducts
    {
        public const string Default = ".AddOnProducts";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";

    }
    public static class DiscountCodes
    {
        public const string Default = ".DiscountCodes";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";

    }
    public static class ShoppingCredits
    {
        public const string Default = ".ShoppingCredits";
    }

    public static class Campaigns
    {
        public const string Default = ".Campaigns";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Invoices
    {
        public const string Default = "Pikachu.Invoices";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Void = Default + ".Void";
    }

    public static class TenantSettings
    {
        public const string Default = ".TenantSettings";
        public const string Edit = Default + ".Edit";
    }

    public static class TenantWallet
    {
        public const string Default = TenantManagementPermissions.GroupName + ".TenantWalletList";
        public const string Edit = Default + ".Edit";

    }
    public static class TenantWalletTransactions
    {
        public const string Default =".TenantWalletTransactions";
       

    }

    public static class WebsiteManagement
    {
        public const string Default = WebsiteManagementGroup;
        public const string WebsiteBasicSettings = Default + ".WebsiteBasicSettings";
        public const string TopbarSettings = Default + ".TopbarSettings";
        public const string FooterSettings = Default + ".FooterSettings";

        public static class WebsiteSettings
        {
            public const string Default = WebsiteManagementGroup + ".WebsiteSettings";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }

    public static class ProductCategories
    {
        public const string Default = ProductManagement + ".ProductCategories";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class VipTierSettings
    {
        public const string Default = MembersManagement + ".VipTierSettings";
    }

    public static class EdmManagement
    {
        public const string Default = EdmManagementGroup + ".Edm";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
