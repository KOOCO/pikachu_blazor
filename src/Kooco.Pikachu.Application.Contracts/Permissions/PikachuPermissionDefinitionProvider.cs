using Kooco.Pikachu.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Kooco.Pikachu.Permissions;

public class PikachuPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PikachuPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(PikachuPermissions.MyPermission1, L("Permission:MyPermission1"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyList, L("Permission:GroupBuyList"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyReport, L("Permission:GroupBuyReport"));
        myGroup.AddPermission(PikachuPermissions.InventoryReport, L("Permission:InventoryReport"));
        myGroup.AddPermission(PikachuPermissions.GroupBuyPageConfig, L("Permission:GroupBuyPageConfig"));
        myGroup.AddPermission(PikachuPermissions.ReturnExchangeOrder, L("Permission:ReturnExchangeOrder"));
        myGroup.AddPermission(PikachuPermissions.GoodsList, L("Permission:ProductList"));
        myGroup.AddPermission(PikachuPermissions.GoodsGroupingSetting, L("Permission:ProductGroupingSetting"));
        //myGroup.AddPermission(PikachuPermissions.StockReport, L("Permission:StockReport"));
        myGroup.AddPermission(PikachuPermissions.FreebieSetting, L("Permission:FreebieSetting"));
        myGroup.AddPermission(PikachuPermissions.POList, L("Permission:OrderList"));
        //myGroup.AddPermission(PikachuPermissions.POReturningList, L("Permission:ProductReturningList"));
        myGroup.AddPermission(PikachuPermissions.RefundAuditList, L("Permission:RefundReviewingList"));
        myGroup.AddPermission(PikachuPermissions.CashFlowDealerSettings, L("Permission:CashFlowDealerSettings"));
        myGroup.AddPermission(PikachuPermissions.CashFlowReconciliationStatement, L("Permission:CashFlowReconciliationStatement"));
        myGroup.AddPermission(PikachuPermissions.EmailSettings, L("Permission:EmailSettings"));
        myGroup.AddPermission(PikachuPermissions.AutomaticEmailing, L("Permission:AutomaticEmailing"));
        myGroup.AddPermission(PikachuPermissions.PaymentGatewaySetting, L("Permission:PaymentSetting"));
        myGroup.AddPermission(PikachuPermissions.InvoiceSetting, L("Permission:E-InvoiceSetting"));
        myGroup.AddPermission(PikachuPermissions.LogisticsSetting, L("Permission:LogisticsSetting"));
        myGroup.AddPermission(PikachuPermissions.TenentList, L("Permission:TenentList"));
        myGroup.AddPermission(PikachuPermissions.TenentBillReport, L("Permission:TenentBillReport"));
        myGroup.AddPermission(PikachuPermissions.PermissionSetting, L("Permission:PermissionSetting"));

        var itemPermission = myGroup.AddPermission(PikachuPermissions.Item.Default, L("Permission:Item"));
        itemPermission.AddChild(PikachuPermissions.Item.Create, L("Permission:Create"));
        itemPermission.AddChild(PikachuPermissions.Item.Update, L("Permission:Update"));
        itemPermission.AddChild(PikachuPermissions.Item.Delete, L("Permission:Delete"));

        var setItemPermission = myGroup.AddPermission(PikachuPermissions.SetItem.Default, L("Permission:SetItem"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Create, L("Permission:Create"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Update, L("Permission:Update"));
        setItemPermission.AddChild(PikachuPermissions.SetItem.Delete, L("Permission:Delete"));

        var setItemDetailsPermission = myGroup.AddPermission(PikachuPermissions.SetItemDetails.Default, L("Permission:SetItemDetails"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Create, L("Permission:Create"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Update, L("Permission:Update"));
        setItemDetailsPermission.AddChild(PikachuPermissions.SetItemDetails.Delete, L("Permission:Delete"));

        var orderPermissions = myGroup.AddPermission(PikachuPermissions.Orders.Default, L("Permission:Orders"));
        orderPermissions.AddChild(PikachuPermissions.Orders.AddStoreComment, L("Permission:AddStoreComments"));

        var refundPermissions = myGroup.AddPermission(PikachuPermissions.Refund.Default, L("Permission:Refunds"));
        refundPermissions.AddChild(PikachuPermissions.Refund.Create, L("Permission:Create"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PikachuResource>(name);
    }
}
