using Kooco.Pikachu.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

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

       
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PikachuResource>(name);
    }
}
