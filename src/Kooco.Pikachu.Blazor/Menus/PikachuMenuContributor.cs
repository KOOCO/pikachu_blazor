using System.Threading.Tasks;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.MultiTenancy;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Refunds;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

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
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        var env = configuration["App:Environment"];
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<PikachuResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                PikachuMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        var productmangment =
         new ApplicationMenuItem(
             PikachuMenus.ProductManagement,
             displayName: l["Menu:ProductManagement"],
             //"商品設定",
             icon: "fas fa-tags",
             order: 1
         );
        context.Menu.AddItem(productmangment);
        if (await context.IsGrantedAsync(PikachuPermissions.GoodsList))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "GoodsList",
            icon: "fas fa-list",
            displayName: l["Menu:GoodsList"],
            //displayName: "商品列表",
            url: "/Items"
            ));
        }
        if (await context.IsGrantedAsync(PikachuPermissions.SetItem.Default))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "GroupGoodsManagement",
            icon: "fas fa-object-group",
            displayName: l["Menu:GroupGoodsManagement"],
            //displayName: "組合商品設定",
            url: "/SetItem"
            ));
        }
        //if (await context.IsGrantedAsync(PikachuPermissions.InventoryReport))
        //{
        //    productmangment.AddItem(new ApplicationMenuItem(
        //    name: "InventoryReport",
        //    icon: "fas fa-newspaper",
        //    displayName: l["Menu:InventoryReport"],
        //    //displayName: "庫存報表",
        //    url: "/Items/InventoryReport")
        //    );
        //}
        if (await context.IsGrantedAsync(PikachuPermissions.FreebieSetting))
        {
            productmangment.AddItem(new ApplicationMenuItem(
            name: "FreebieManagement",
            icon: "fas fa-gifts",
            displayName: l["Menu:FreebiesListView"],
            //displayName: "贈品設定",
            url: "/Freebie/FreebieList")
            );
        }

        productmangment.AddItem(new ApplicationMenuItem(
            name: "ProductCategories",
            icon: "fas fa-layer-group",
            displayName: l["Menu:ProductCategories"],
            url: "Product-Categories",
            requiredPermissionName: PikachuPermissions.ProductCategories.Default
            ));

        productmangment.AddItem(new ApplicationMenuItem(
            name: "Inventory",
            icon: "fas fa-boxes-stacked",
            displayName: l["Menu:Inventory"],
            url: "Inventory-Management/Inventory"
            ));

        var groupMangment = new ApplicationMenuItem(
                 PikachuMenus.GroupBuyManagement,
                 displayName: l["Menu:GroupBuyManagement"],
                 //"團購管理",
                 icon: "fas fa-store",
                 order: 2
             );
        context.Menu.AddItem(groupMangment);
        if (await context.IsGrantedAsync(PikachuPermissions.GroupBuyList))
        {
            groupMangment.AddItem(new ApplicationMenuItem(
            name: "GroupBuyListView",
            displayName: l["Menu:GroupBuyListView"],
            //displayName: "團購列表",
            icon: "fas fa-list",
            url: "/GroupBuyManagement/GroupBuyList"
            ));
        }
        ;
        if (await context.IsGrantedAsync(PikachuPermissions.GroupBuyReport))
        {
            groupMangment.AddItem(new ApplicationMenuItem(
                name: "GroupBuyReports",
                icon: "fas fa-newspaper",
                displayName: l["Menu:GroupBuyReports"],
                //displayName: "團購報表",
                url: "/GroupBuyManagement/GroupBuyReport"
                ));
        }
        groupMangment.AddItem(new ApplicationMenuItem(
      name: "ReportNotification",
      icon: "fas fa-envelope-open-text",
      displayName: l["ReportNotification"],
      url: "/AutomaticEmailing",
      requiredPermissionName: PikachuPermissions.ReportNotification)
      );
        var orderMangment = new ApplicationMenuItem(
               PikachuMenus.OrderManagement,
               displayName: l["Menu:OrderManagement"],
               //"訂單管理",
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

        string displayName = l["Menu:ReturnAndExchangeOrder"];
        //if (returnExchangeOrderCount > 0)
        //{
        //   displayName += $" <span class='notification-badge'>{returnExchangeOrderCount}</span>"; ; // Append the count
        //}
        var returnAndExchangeMenu = new ApplicationMenuItem(
             name: "return",
             icon: "fas fa-truck-loading",
             displayName: l["Menu:ReturnAndExchangeOrder"],
             url: "/Orders/ReturnAndExchangeOrder",
             requiredPermissionName: PikachuPermissions.ReturnExchangeOrder
         );
        using (var scope = context.ServiceProvider.CreateScope())
        {
            var orderAppService = scope.ServiceProvider.GetRequiredService<IOrderAppService>();

            // Fetch notification count
            int returnExchangeOrderCount = (int)await orderAppService.GetReturnOrderNotificationCount();

            // Store count as an HTML attribute (JavaScript will use this)
            returnAndExchangeMenu.CustomData["data-notification-count"] = returnExchangeOrderCount.ToString();
        }

        orderMangment.AddItem(returnAndExchangeMenu);
        var refundmenu = new ApplicationMenuItem(
         name: "Refund",
         icon: "fas fa-stamp",
         displayName: l["RefundsList"],
         url: "/Refund",
         requiredPermissionName: PikachuPermissions.Orders.Default);

        using (var scope = context.ServiceProvider.CreateScope())
        {
            var refundService = scope.ServiceProvider.GetRequiredService<RefundAppService>();

            // Fetch notification count
            int returnExchangeOrderCount = (int)await refundService.GetRefundPendingCount();

            // Store count as an HTML attribute (JavaScript will use this)
            refundmenu.CustomData["data-notification-count"] = returnExchangeOrderCount.ToString();
        }
        orderMangment.AddItem(refundmenu);
        orderMangment.AddItem(new ApplicationMenuItem(
         name: "EnterpricePurchase",
         icon: "fas fa-stamp",
         displayName: l["EnterpricePurchase"],
         url: "/enterprice-purchase",
         requiredPermissionName: PikachuPermissions.Orders.Default)
         );
        var membersMenu = new ApplicationMenuItem(
            PikachuMenus.Members,
            displayName: l["Menu:Members"],
            icon: "fas fa-users",
            order: 4
            );
        membersMenu.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.MembersList,
            displayName: l["Menu:MembersList"],
            icon: "fas fa-user-group",
            url: "/Members",
            requiredPermissionName: PikachuPermissions.Members.Default
            ));
        membersMenu.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.LoginConfigurations,
            displayName: l["Menu:LoginConfigurations"],
            icon: "fas fa-user-gear",
            url: "/Login-Configurations"
            ).RequireAuthenticated());
        membersMenu.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.TierManagement,
            displayName: l["Menu:TierManagement"],
            icon: "fas fa-user-tag",
            url: "/Members/Tier-Management"
            ).RequireAuthenticated());
        membersMenu.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.MemberTags,
            displayName: l["Menu:MemberTags"],
            icon: "fas fa-user-tag",
            url: "/Members/Member-Tags",
            requiredPermissionName: PikachuPermissions.Members.Default
            ).RequireAuthenticated());
        membersMenu.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.ShopCarts,
            displayName: l["Menu:ShopCarts"],
            icon: "fas fa-cart-shopping",
            url: "/Members/Shop-Carts",
            requiredPermissionName: PikachuPermissions.ShopCarts.Default
            ).RequireAuthenticated());
        context.Menu.AddItem(membersMenu);

        var promotions =
            new ApplicationMenuItem(
              PikachuMenus.ProductManagement,
              displayName: l["Menu:Promotions"],
              //"商品設定",
              icon: "fas fa-gift",
              order: 5
            );


        //promotions.AddItem(new ApplicationMenuItem(
        //       name: "AddOnProducts",
        //       displayName: l["AddOnProducts"],
        //       url: "/add-on-products",
        //       requiredPermissionName: PikachuPermissions.AddOnProducts.Default)
        //       );

        //promotions.AddItem(new ApplicationMenuItem(
        //       name: "DiscountCodes",

        //       displayName: l["DiscountCodes"],
        //       url: "/discount-code",
        //       requiredPermissionName: PikachuPermissions.DiscountCodes.Default)
        //       );
        promotions.AddItem(new ApplicationMenuItem(
            name: "ShoppingCredits",

            displayName: l["ShoppingCredits"],
            url: "/shopping-credit",
            requiredPermissionName: PikachuPermissions.ShoppingCredits.Default)
            );
        promotions.AddItem(new ApplicationMenuItem(
            name: "Campaigns",
            displayName: l["Menu:Campaigns"],
            url: "/Campaigns",
            requiredPermissionName: PikachuPermissions.Campaigns.Default)
            );
        context.Menu.AddItem(promotions);

        var paymentManagement = new ApplicationMenuItem(
                PikachuMenus.PaymentManagement,
                displayName: l["Menu:PaymentManagement"],
                //"金流設定",
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
            displayName: l["Menu:ElectronicInvoiceSetting"],
            //displayName: "電子發票設定",
            url: "/CashFlowManagement/ElectronicInvoiceSetting",
            requiredPermissionName: PikachuPermissions.InvoiceSetting)
            );
        if (env == "Development")
        {
            paymentManagement.AddItem(new ApplicationMenuItem(
            name: "CashFlowReconciliationStatement",
            icon: "fas fa-file-invoice",
            displayName: l["Menu:CashFlowReconciliationStatement"],
            //displayName: "金流對帳表",
            url: "/CashFlowManagement/CashFlowReconciliationStatement",
            requiredPermissionName: PikachuPermissions.CashFlowReconciliationStatement)
            );
        }
        paymentManagement.AddItem(new ApplicationMenuItem(
           name: "VoidInvoice",
           icon: "fas fa-file-invoice",
           displayName: l["Menu:VoidInvoice"],
           //displayName: "金流對帳表",
           url: "/VoidInvoice",
           requiredPermissionName: PikachuPermissions.CashFlowReconciliationStatement)
           );

        paymentManagement.AddItem(new ApplicationMenuItem(
           name: "SalesReport",
           icon: "fas fa-file-invoice",
           displayName: l["Menu:SalesReport"],
           url: "/sales-report",
            requiredPermissionName: PikachuPermissions.SaleReport
           ).RequireAuthenticated()
           );

        var logisticsManagment =
                new ApplicationMenuItem(
                PikachuMenus.LogisticsManagement,
                displayName: l["Menu:LogisticsManagement"],
                //"物流設定",
                icon: "fas fa-truck",
                order: 5
            )
                .AddItem(new ApplicationMenuItem(
            name: l["LogisticsProviderSettings"],
            icon: "fas fa-shipping-fast",
            displayName: l["Menu:LogisticsProviderSettings"],
            //displayName: "物流商設定",
            url: "/LogisticsProviderSettings",
            requiredPermissionName: PikachuPermissions.LogisticsSetting)
            )
                .AddItem(new ApplicationMenuItem(
            name: "Delivery By Store Settings",
            icon: "fas fa-temperature-low",
            displayName: l["Menu:DeliveryByStoreSettings"],
            url: "/DeliveryTemperatureCost",
            requiredPermissionName: PikachuPermissions.DeliveryTemperatureCost)
            );

        context.Menu.AddItem(logisticsManagment);

        var websiteManagement = new ApplicationMenuItem(
            PikachuMenus.WebsiteManagement,
            displayName: l["Menu:WebsiteManagement"],
            icon: "fas fa-computer",
            order: 6,
            requiredPermissionName: PikachuPermissions.WebsiteManagement.Default
            );
        if (env == "Development")
        {
            websiteManagement.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.WebsiteBasicSettings,
            displayName: l["Menu:WebsiteBasicSettings"],
            url: "/Website-Basic-Settings",
            icon: "fas fa-sliders",
            requiredPermissionName: PikachuPermissions.WebsiteManagement.WebsiteBasicSettings
            ));

            websiteManagement.AddItem(new ApplicationMenuItem(
                name: PikachuMenus.TopbarSettings,
                displayName: l["Menu:TopbarSettings"],
                url: "/Topbar-Settings",
                icon: "fas fa-window-maximize",
                requiredPermissionName: PikachuPermissions.WebsiteManagement.TopbarSettings
                ));
        }
        websiteManagement.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.FooterSettings,
            displayName: l["Menu:FooterSettings"],
            url: "/Footer-Settings",
            icon: "fas fa-window-maximize",
            requiredPermissionName: PikachuPermissions.WebsiteManagement.TopbarSettings
            ));
        if (env == "Development")
        {
            websiteManagement.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.WebsiteSettings,
            displayName: l["Menu:WebsiteSettings"],
            url: "/Website-Settings",
            icon: "fas fa-laptop-code",
            requiredPermissionName: PikachuPermissions.WebsiteManagement.WebsiteSettings.Default
            ));
        }
        context.Menu.AddItem(websiteManagement);

        var emailManagement = new ApplicationMenuItem(
            PikachuMenus.EmailManagement,
            displayName: l["Menu:EmailManagement"],
            icon: "fas fa-envelopes-bulk",
            order: 7,
            requiredPermissionName: PikachuPermissions.EmailSettings
            );

        emailManagement.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.EmailSettings,
            displayName: l["Menu:EmailSettings"],
            url: "/EmailSettings",
            icon: "fas fa-gears",
            requiredPermissionName: SettingManagementPermissions.Emailing
            ));

        emailManagement.AddItem(new ApplicationMenuItem(
            name: PikachuMenus.EdmList,
            displayName: l["Menu:EdmList"],
            url: "/Edm",
            icon: "fas fa-envelope",
            requiredPermissionName: PikachuPermissions.EdmManagement.Default
            ));

        context.Menu.AddItem(emailManagement);

        context.Menu.AddItem(new ApplicationMenuItem(
            PikachuMenus.Notifications,
            displayName: l["Menu:Notifications"],
            icon: "fas fa-bell",
            order: 10,
            requiredPermissionName: PikachuPermissions.Notifications.Default,
            url: "/Notifications"
            ));

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);

            ApplicationMenuItem tenantManagementMenu = administration.GetMenuItem(TenantManagementMenuNames.GroupName);

            tenantManagementMenu.Items[0].DisplayName = l["Tenants"];

            tenantManagementMenu?.AddItem(
                new ApplicationMenuItem(
                    name: "多商戶管理-商戶賬單報表",
                    icon: "fas fa-newspaper",
                    displayName: l["Menu:TenantBillingReport"],
                    url: "/TenantBillingReport",
                    requiredPermissionName: PikachuPermissions.TenentBillReport
                )
            );

            tenantManagementMenu?.AddItem(
                new ApplicationMenuItem(
                    name: PikachuMenus.TenantSettings,
                    icon: "fas fa-cogs",
                    displayName: l["Menu:TenantSettings"],
                    url: "/Tenant-Settings",
                    requiredPermissionName: PikachuPermissions.TenantSettings.Default
                )
            );

            tenantManagementMenu?.AddItem(
                new ApplicationMenuItem(
                    name: PikachuMenus.TenantWallets,
                    displayName: l["Menu:TenantWallet"],
                    url: "/TenantManagement/TenantWalletList",
                    requiredPermissionName: PikachuPermissions.TenantWallet.Default
                )
            );
            tenantManagementMenu?.AddItem(
    new ApplicationMenuItem(
        name: PikachuMenus.LogisticsFeeManagement,
        displayName: l["LogisticsFeeManagement"],
        url: "/logistics-management",
        requiredPermissionName: PikachuPermissions.LogisticsFeeManagement.Default
    )
);
            administration?.AddItem(
             new ApplicationMenuItem(
                 name: PikachuMenus.TenantWalletManagement,
                 displayName: l["Menu:TenantWalletManagement"],
                 url: "/wallet",
                 requiredPermissionName: PikachuPermissions.TenantWalletTransactions.Default
             )
         );

            tenantManagementMenu?.AddItem(
                new ApplicationMenuItem(
                    name: "EcPayReconciliation",
                    displayName: l["Menu:EcPayReconciliation"],
                    url: "/ecpay/reconciliation",
                    requiredPermissionName: PikachuPermissions.EcPayReconciliations.Default
                )
            );
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        //remove administration item from menu
        //context.Menu.Items.Remove( administration );

        //if (!await context.IsGrantedAsync(SettingManagementPermissions.Emailing))
        //{
        administration.TryRemoveMenuItem(SettingManagementMenus.GroupName);
        //}
    }
}
