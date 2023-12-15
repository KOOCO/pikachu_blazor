using System.Threading.Tasks;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.MultiTenancy;
using Kooco.Pikachu.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace Kooco.Pikachu.Blazor.Menus;

public class PikachuMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<PikachuResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                PikachuMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "IconType.Outline.Home",
                order: 0
            )
        );

        var groupMangment = new ApplicationMenuItem(
                 PikachuMenus.GroupBuyManagement,
                 "團購管理",

                 icon: "fas fa-store",
                 order: 1
             );
        context.Menu.AddItem(groupMangment);
        if (await context.IsGrantedAsync(PikachuPermissions.GroupBuyList))
        {
            groupMangment.AddItem(new ApplicationMenuItem(
            name: "GroupBuyListView",
            displayName: "團購列表",
            icon: "fas fa-list",
            url: "/GroupBuyManagement/GroupBuyList"
            ));
        };
        if (await context.IsGrantedAsync(PikachuPermissions.GroupBuyReport))
        {
            groupMangment.AddItem(new ApplicationMenuItem(
                name: "GroupBuyReports",
                icon: "fas fa-newspaper",
                displayName: "團購報表",
                url: "/GroupBuyManagement/GroupBuyReport"
                ));
        }
        if (await context.IsGrantedAsync(PikachuPermissions.GroupBuyPageConfig))
        {
            groupMangment.AddItem(new ApplicationMenuItem(
                name: "GroupBuyPageConfig",
                icon: "fas fa-chalkboard-teacher",
                displayName: "團購頁面設定",
                url: "/GroupBuyManagement/GroupBuyPageConfig"
                )

            );
        }

        var productmangment =
         new ApplicationMenuItem(
             PikachuMenus.ProductManagement,
             "商品設定",

             icon: "fas fa-tags",
             order: 2
         );
        context.Menu.AddItem(productmangment);
        if (await context.IsGrantedAsync(PikachuPermissions.GoodsList))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "GoodsList",
            icon: "fas fa-list",
            displayName: "商品列表",
            url: "/Items"
            ));
        }
        if (await context.IsGrantedAsync(PikachuPermissions.SetItem.Default))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "GroupGoodsManagement",
            icon: "fas fa-object-group",
            displayName: "組合商品設定",
            url: "/SetItem"
            ));
        }
        if (await context.IsGrantedAsync(PikachuPermissions.InventoryReport))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "InventoryReport",
            icon: "fas fa-newspaper",
            displayName: "庫存報表",
            url: "/Items/InventoryReport")
            );
        }
        if (await context.IsGrantedAsync(PikachuPermissions.FreebieSetting))
        {
            productmangment.AddItem(new ApplicationMenuItem(
        name: "FreebieManagement",
        icon: "fas fa-gifts",
        displayName: "贈品設定",
        url: "/Freebie/FreebieList")
        );
        }



        var orderMangment = new ApplicationMenuItem(
               PikachuMenus.OrderManagement,
               "訂單管理",

               icon: "fas fa-cart-plus",
               order: 3
           );
        context.Menu.AddItem(orderMangment);
        orderMangment.AddItem(new ApplicationMenuItem(
            "Orders",
            icon: "fas fa-list",
            displayName: l["Menu:Orders"],
            url: "/Orders",
            requiredPermissionName: PikachuPermissions.Orders.Default
            )
            );
        orderMangment.AddItem(new ApplicationMenuItem(
        name: "FreebieManagement",
        icon: "fas fa-truck-loading",
        displayName: "退換貨列表",
        url: "/Orders/ReturnAndExchangeOrder",
         requiredPermissionName: PikachuPermissions.ReturnExchangeOrder)
        );
        orderMangment.AddItem(new ApplicationMenuItem(
        name: "Refund",
        icon: "fas fa-stamp",
        displayName: l["Refund"],
        url: "/Refund",
        requiredPermissionName: PikachuPermissions.Orders.Default)
        );

        var paymentManagement = new ApplicationMenuItem(
                PikachuMenus.PaymentManagement,
                "金流設定",

                icon: "fas fa-funnel-dollar",
                order: 4
            );

        context.Menu.AddItem(paymentManagement);
        paymentManagement.AddItem(new ApplicationMenuItem(
            name: "CashFlowDealerSettings",
            icon: "fas fa-hand-holding-usd",
            displayName: l["CashFlowDealerSettings"],
            url: "/CashFlowDealerSettings",
            requiredPermissionName: PikachuPermissions.CashFlowDealerSettings)

            );
        paymentManagement.AddItem(new ApplicationMenuItem(
            name: "ElectronicInvoiceSetting",
            icon: "fas fa-receipt",
            displayName: "電子發票設定",
            url: "/CashFlowManagement/ElectronicInvoiceSetting",
            requiredPermissionName: PikachuPermissions.InvoiceSetting)
            );
                    paymentManagement.AddItem(new ApplicationMenuItem(
            name: "CashFlowReconciliationStatement",
            icon: "fas fa-file-invoice",
            displayName: "金流對帳表",
            url: "/CashFlowManagement/CashFlowReconciliationStatement",
            requiredPermissionName: PikachuPermissions.CashFlowReconciliationStatement)
            );

        var logisticsManagment =
     new ApplicationMenuItem(
                PikachuMenus.LogisticsManagement,
                "物流設定",

                icon: "fas fa-truck",
                order: 5
            );
        logisticsManagment.AddItem(new ApplicationMenuItem(
            name: l["LogisticsProviderSettings"],
            icon: "fas fa-shipping-fast",
            displayName: "物流商設定",
            url: "/LogisticsProviderSettings",
            requiredPermissionName: PikachuPermissions.LogisticsSetting
            )
            );

        context.Menu.AddItem(logisticsManagment);
        //context.Menu.Items.Insert(
        //    6,
        //    new ApplicationMenuItem(
        //        PikachuMenus.TenantManagement,
        //        "多商戶管理",
        //        url: "/LogisticsManagement",
        //        icon: "fas fa-users-cog",
        //        order: 6
        //    )
        //.AddItem(new ApplicationMenuItem(
        //name: "TenantManagement",
        //displayName: "商戶列表",
        //icon: "fas fa-list",
        //url: "/TenantManagement/Tenants")
        //).AddItem(new ApplicationMenuItem(
        //name: "TenantManagementBillList",
        //displayName: "商戶帳單列表",
        //icon: "fas fa-file-invoice",
        //url: "/TenantManagement/TenantBillList")
        //)
        //);

        var systemManagment =
            new ApplicationMenuItem(
                PikachuMenus.SystemManagement,
                "系統管理",

                icon: "fas fa-user",
                order: 7
            );
        context.Menu.AddItem(systemManagment);
        systemManagment.AddItem(new ApplicationMenuItem(
                name: "Permission",
                displayName: "權限管理",
                icon: "fas fa-user-lock",
                url: "/identity/users",
                requiredPermissionName: PikachuPermissions.PermissionSetting)

            );
        systemManagment.AddItem(new ApplicationMenuItem(
        name: "EmailSettings",
        icon: "fas fa-mail-bulk",
        displayName: l["EmailSettings"],
        url: "/EmailSettings",
        requiredPermissionName: PikachuPermissions.EmailSettings)
        );
        systemManagment.AddItem(new ApplicationMenuItem(
        name: "AutomaticEmailing",
        icon: "fas fa-envelope-open-text",
        displayName: l["AutomaticEmailing"],
        url: "/AutomaticEmailing",
        requiredPermissionName: PikachuPermissions.AutomaticEmailing)
        );
        systemManagment.AddItem(new ApplicationMenuItem(
      name: "多商戶管理-商戶賬單報表",
      icon: "fas fa-newspaper",
      displayName: l["多商戶管理-商戶賬單報表"],
      url: "/TenantBillingReport",
      requiredPermissionName: PikachuPermissions.TenentBillReport)

      );
        systemManagment.AddItem(new ApplicationMenuItem(
    name: "Delivery Temperature Cost",
    icon: "fas fa-temperature-low",
    displayName: l["DeliveryTemperatureCost"],
    url: "/DeliveryTemperatureCost",
    requiredPermissionName: PikachuPermissions.DeliveryTemperatureCost)

    );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);
        //remove administration item from menu
        //context.Menu.Items.Remove( administration );


    }
}
