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

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
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

        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(
                PikachuMenus.GroupBuyManagement,
                "團購管理",
                url: "/GroupBuyManagement",
                icon: "fas fa-store",
                order: 1
            )
            .AddItem(new ApplicationMenuItem(
            name: "GroupBuyListView",
            displayName: "團購列表",
            icon: "fas fa-list",
            url: "/GroupBuyManagement/GroupBuyList")
            )
            .AddItem(new ApplicationMenuItem(
            name: "GroupBuyReports",
            icon: "fas fa-newspaper",
            displayName: "團購報表",
            url: "/GroupBuyManagement/GroupBuyReport")
            )
            .AddItem(new ApplicationMenuItem(
            name: "GroupBuyPageConfig",
            icon: "fas fa-chalkboard-teacher",
            displayName: "團購頁面設定",
            url: "/GroupBuyManagement/GroupBuyPageConfig")
            )
            
        );

        context.Menu.Items.Insert(
            2,
            new ApplicationMenuItem(
                PikachuMenus.ProductManagement,
                "商品設定",
                url: "/ProductManagement",
                icon: "fas fa-tags",
                order: 2
            )
            .AddItem(new ApplicationMenuItem(
            name: "GoodsList",
            icon: "fas fa-list",
            displayName: "商品列表",
            url: "/Items")
            )
            .AddItem(new ApplicationMenuItem(
            name: "GroupGoodsManagement",
            icon: "fas fa-object-group",
            displayName: "組合商品設定",
            url: "/SetItem")
            )
            .AddItem(new ApplicationMenuItem(
            name: "InventoryReport",
            icon: "fas fa-newspaper",
            displayName: "庫存報表",
            url: "/Items/InventoryReport")
            )
            .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-gifts",
            displayName: "贈品設定",
            url: "/Freebie/FreebieList")
            )

        );

        context.Menu.Items.Insert(
            3,
            new ApplicationMenuItem(
                PikachuMenus.OrderManagement,
                "訂單管理",
                url: "/OrderManagement",
                icon: "fas fa-cart-plus",
                order: 3
            )
            .AddItem(new ApplicationMenuItem(
            "Orders",
            icon: "fas fa-list",
            displayName: l["Menu:Orders"],
            url: "/Orders",
            requiredPermissionName: PikachuPermissions.Orders.Default
            )
            )
            .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-truck-loading",
            displayName: "退換貨列表",
            url: "/Orders/ReturnAndExchangeOrder")
            )
            .AddItem(new ApplicationMenuItem(
            name: "Refund",
            icon: "fas fa-stamp",
            displayName: l["Refund"],
            url: "/Refund",
            requiredPermissionName: PikachuPermissions.Orders.Default)
            )
        );

        context.Menu.Items.Insert(
            4,
            new ApplicationMenuItem(
                PikachuMenus.PaymentManagement,
                "金流設定",
                url: "/PaymentManagement",
                icon: "fas fa-funnel-dollar",
                order: 4
            )
                  .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-hand-holding-usd",
            displayName: "金流商設定",
            url: "/Temp/Temp2")
            )
                        .AddItem(new ApplicationMenuItem(
            name: "ElectronicInvoiceSetting",
            icon: "fas fa-receipt",
            displayName: "電子發票設定",
            url: "/CashFlowManagement/ElectronicInvoiceSetting")
            )
                              .AddItem(new ApplicationMenuItem(
            name: "CashFlowReconciliationStatement",
            icon: "fas fa-file-invoice",
            displayName: "金流對帳表",
            url: "/CashFlowManagement/CashFlowReconciliationStatement")
            )
        );

        context.Menu.Items.Insert(
            5,
            new ApplicationMenuItem(
                PikachuMenus.LogisticsManagement,
                "物流設定",
                url: "/LogisticsManagement",
                icon: "fas fa-truck",
                order: 5
            )
            .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-shipping-fast",
            displayName: "物流商設定",
            url: "/GroupBuyManagement/GroupBuyReport6")
            )
        );

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

        context.Menu.Items.Insert(
            7,
            new ApplicationMenuItem(
                PikachuMenus.SystemManagement,
                "系統管理",
                url: "/SystemManagement",
                icon: "fas fa-user",
                order: 7
            ).AddItem(new ApplicationMenuItem(
                name: "Permission",
                displayName: "權限管理",
                icon: "fas fa-user-lock",
                url: "/Identify/Users")
            //.RequirePermissions("MyProject.Crm.Orders")
            )
            .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-mail-bulk",
            displayName: "寄送信設定",
            url: "/GroupBuyManagement/GroupBuyReport7")
            )
            .AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-envelope-open-text",
            displayName: "自動發送報表",
            url: "/GroupBuyManagement/GroupBuyReport8")
            )

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

        return Task.CompletedTask;
    }
}
