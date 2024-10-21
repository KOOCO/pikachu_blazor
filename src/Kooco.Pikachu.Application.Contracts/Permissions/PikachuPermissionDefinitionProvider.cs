using Kooco.Pikachu.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Permissions;

public class PikachuPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PikachuPermissions.GroupBuyManagement);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(PikachuPermissions.MyPermission1, L("Permission:MyPermission1"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyList, L("Permission:GroupBuyList"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyReport, L("Permission:GroupBuyReport"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyPageConfig, L("Permission:GroupBuyPageConfig"));


        var myGroup1 = context.AddGroup(PikachuPermissions.ProductManagement);
        myGroup1.AddPermission(PikachuPermissions.InventoryReport, L("Permission:InventoryReport"));

        myGroup1.AddPermission(PikachuPermissions.GoodsList, L("Permission:ProductList"));
        myGroup1.AddPermission(PikachuPermissions.GoodsGroupingSetting, L("Permission:ProductGroupingSetting"));
        //myGroup.AddPermission(PikachuPermissions.StockReport, L("Permission:StockReport"));
        myGroup1.AddPermission(PikachuPermissions.FreebieSetting, L("Permission:FreebieSetting"));
        
        var productCategoryPermissions = myGroup1.AddPermission(PikachuPermissions.ProductCategories.Default, L("Permission:ProductCategories"));
        productCategoryPermissions.AddChild(PikachuPermissions.ProductCategories.Create, L("Permission:Create"));
        productCategoryPermissions.AddChild(PikachuPermissions.ProductCategories.Edit, L("Permission:Edit"));
        productCategoryPermissions.AddChild(PikachuPermissions.ProductCategories.Delete, L("Permission:Delete"));

        myGroup.AddPermission(PikachuPermissions.POList, L("Permission:OrderList"));
        //myGroup.AddPermission(PikachuPermissions.POReturningList, L("Permission:ProductReturningList"));
        var myGroup2 = context.AddGroup(PikachuPermissions.OrderManagement);
        var orderPermissions = myGroup2.AddPermission(PikachuPermissions.Orders.Default, L("Permission:Orders"));
        orderPermissions.AddChild(PikachuPermissions.Orders.AddStoreComment, L("Permission:AddStoreComments"));
        myGroup2.AddPermission(PikachuPermissions.ReturnExchangeOrder, L("Permission:ReturnExchangeOrder"));



        var refundPermissions = myGroup2.AddPermission(PikachuPermissions.Refund.Default, L("Permission:Refunds"));
        refundPermissions.AddChild(PikachuPermissions.Refund.Create, L("Permission:Create"));
        refundPermissions.AddChild(PikachuPermissions.Refund.RefundOrderProcess, L("Permission:RefundProcess"));
        var myGroup3 = context.AddGroup(PikachuPermissions.PaymentManagement);
        myGroup3.AddPermission(PikachuPermissions.CashFlowDealerSettings, L("Permission:CashFlowDealerSettings"));
        myGroup3.AddPermission(PikachuPermissions.CashFlowReconciliationStatement, L("Permission:CashFlowReconciliationStatement"));
        myGroup3.AddPermission(PikachuPermissions.InvoiceSetting, L("Permission:E-InvoiceSetting"));
        var myGroup4 = context.AddGroup(PikachuPermissions.LogisticsManagement);
        myGroup4.AddPermission(PikachuPermissions.LogisticsSetting, L("Permission:LogisticsSetting"));

        var myGroup5 = context.AddGroup(PikachuPermissions.SystemManagement);
        myGroup.AddPermission(PikachuPermissions.RefundAuditList, L("Permission:RefundReviewingList"));

        myGroup5.AddPermission(PikachuPermissions.EmailSettings, L("Permission:EmailSettings"));
        myGroup5.AddPermission(PikachuPermissions.AutomaticEmailing, L("Permission:AutomaticEmailing"));
        myGroup5.AddPermission(PikachuPermissions.PaymentGatewaySetting, L("Permission:PaymentSetting"));
        myGroup5.AddPermission(PikachuPermissions.DeliveryTemperatureCost, L("Permission:DeliveryTemperatureCost"));


        myGroup5.AddPermission(PikachuPermissions.TenentList, L("Permission:TenentList"));
        myGroup5.AddPermission(PikachuPermissions.TenentBillReport, L("Permission:TenentBillReport"));
        myGroup5.AddPermission(PikachuPermissions.PermissionSetting, L("Permission:PermissionSetting"));

        var itemPermission = myGroup1.AddPermission(PikachuPermissions.Item.Default, L("Permission:Item"));
        itemPermission.AddChild(PikachuPermissions.Item.Create, L("Permission:Create"));
        itemPermission.AddChild(PikachuPermissions.Item.Update, L("Permission:Update"));
        itemPermission.AddChild(PikachuPermissions.Item.Delete, L("Permission:Delete"));

        var setItemPermission = myGroup1.AddPermission(PikachuPermissions.SetItem.Default, L("Permission:SetItem"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Create, L("Permission:Create"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Update, L("Permission:Update"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Delete, L("Permission:Delete"));

        var setItemDetailsPermission = myGroup1.AddPermission(PikachuPermissions.SetItemDetails.Default, L("Permission:SetItemDetails"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Create, L("Permission:Create"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Update, L("Permission:Update"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Delete, L("Permission:Delete"));

        var memberManagementGroup = context.AddGroup(PikachuPermissions.MembersManagement, L(PikachuPermissions.MembersManagement));
        var memberPermissions = memberManagementGroup.AddPermission(PikachuPermissions.Members.Default, L("Permission:Members"));
        memberPermissions.AddChild(PikachuPermissions.Members.Create, L("Permission:Create"));
        memberPermissions.AddChild(PikachuPermissions.Members.Edit, L("Permission:Edit"));
        memberPermissions.AddChild(PikachuPermissions.Members.Delete, L("Permission:Delete"));

        var userAddressPermissions = memberManagementGroup.AddPermission(PikachuPermissions.UserAddresses.Default, L("Permission:UserAdresses"));
        userAddressPermissions.AddChild(PikachuPermissions.UserAddresses.Create, L("Permission:Create"));
        userAddressPermissions.AddChild(PikachuPermissions.UserAddresses.Edit, L("Permission:Edit"));
        userAddressPermissions.AddChild(PikachuPermissions.UserAddresses.Delete, L("Permission:Delete"));
        userAddressPermissions.AddChild(PikachuPermissions.UserAddresses.SetIsDefault, L("Permission:SetIsDefault"));

        var userShoppingCreditPermissions = memberManagementGroup.AddPermission(PikachuPermissions.UserShoppingCredits.Default, L("Permission:UserShoppingCredits"));
        userShoppingCreditPermissions.AddChild(PikachuPermissions.UserShoppingCredits.Create, L("Permission:Create"));
        userShoppingCreditPermissions.AddChild(PikachuPermissions.UserShoppingCredits.Edit, L("Permission:Edit"));
        userShoppingCreditPermissions.AddChild(PikachuPermissions.UserShoppingCredits.Delete, L("Permission:Delete"));
        userShoppingCreditPermissions.AddChild(PikachuPermissions.UserShoppingCredits.SetIsActive, L("Permission:SetIsActive"));

        var promotion = context.AddGroup(PikachuPermissions.Promotions, L("Permission:Promotions"));
        var addonProducts = promotion.AddPermission(PikachuPermissions.AddOnProducts.Default, L("Permission:AddOnProducts"));
        addonProducts.AddChild(PikachuPermissions.AddOnProducts.Create, L("Permission:Create"));
        addonProducts.AddChild(PikachuPermissions.AddOnProducts.Edit, L("Permission:Edit"));
        addonProducts.AddChild(PikachuPermissions.AddOnProducts.Delete, L("Permission:Delete"));
        var discountCodes = promotion.AddPermission(PikachuPermissions.DiscountCodes.Default, L("Permission:DiscountCodes"));
        discountCodes.AddChild(PikachuPermissions.DiscountCodes.Create, L("Permission:Create"));
        discountCodes.AddChild(PikachuPermissions.DiscountCodes.Edit, L("Permission:Edit"));
        discountCodes.AddChild(PikachuPermissions.DiscountCodes.Delete, L("Permission:Delete"));
        var shoppingCredits = promotion.AddPermission(PikachuPermissions.ShoppingCredits.Default, L("Permission:ShoppingCredits"));

        _ = memberManagementGroup.AddPermission(PikachuPermissions.ShopCarts.Default, L("Permission:ShopCart"));

        var tenantSettingsPermissions = myGroup.AddPermission(PikachuPermissions.TenantSettings.Default, L("Permission:TenantSettings"), MultiTenancySides.Tenant);
        tenantSettingsPermissions.AddChild(PikachuPermissions.TenantSettings.Edit, L("Permission:Edit"), MultiTenancySides.Tenant);

        var websiteSettingsPermissions = myGroup.AddPermission(PikachuPermissions.WebsiteSettings.Default, L("Permission:WebsiteSettings"));
        websiteSettingsPermissions.AddChild(PikachuPermissions.WebsiteSettings.Create, L("Permission:Create"));
        websiteSettingsPermissions.AddChild(PikachuPermissions.WebsiteSettings.Edit, L("Permission:Edit"));
        websiteSettingsPermissions.AddChild(PikachuPermissions.WebsiteSettings.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PikachuResource>(name);
    }
}
