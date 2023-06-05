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
        myGroup.AddPermission(PikachuPermissions.GoodsList, L("Permission:ProductList"));
        myGroup.AddPermission(PikachuPermissions.GoodsGroupingSetting, L("Permission:ProductGroupingSetting"));
        myGroup.AddPermission(PikachuPermissions.StockReport, L("Permission:StockReport"));
        myGroup.AddPermission(PikachuPermissions.FreebieSetting, L("Permission:FreebieSetting"));
        myGroup.AddPermission(PikachuPermissions.POList, L("Permission:OrderList"));
        myGroup.AddPermission(PikachuPermissions.POReturningList, L("Permission:ProductReturningList"));
        myGroup.AddPermission(PikachuPermissions.RefundAuditList, L("Permission:RefundReviewingList"));
        myGroup.AddPermission(PikachuPermissions.PaymentGatewaySetting, L("Permission:PaymentSetting"));
        myGroup.AddPermission(PikachuPermissions.InvoiceSetting, L("Permission:E-InvoiceSetting"));
        myGroup.AddPermission(PikachuPermissions.LogisticsSetting, L("Permission:LogisticsSetting"));
        myGroup.AddPermission(PikachuPermissions.TenentList, L("Permission:TenentList"));
        myGroup.AddPermission(PikachuPermissions.TenentBillReport, L("Permission:TenentBillReport"));
        myGroup.AddPermission(PikachuPermissions.PermissionSetting, L("Permission:PermissionSetting"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PikachuResource>(name);
    }
}
