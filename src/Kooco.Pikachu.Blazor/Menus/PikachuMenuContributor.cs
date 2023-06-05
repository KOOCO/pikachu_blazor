using System.Threading.Tasks;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.MultiTenancy;
using Volo.Abp.Authorization.Permissions;
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
                icon: "fas fa-home",
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
        );

        context.Menu.Items.Insert(
            6,
            new ApplicationMenuItem(
                PikachuMenus.TenantManagement,
                "多商戶管理",
                url: "/LogisticsManagement",
                icon: "fas fa-users-cog",
                order: 7
            )
        );

        context.Menu.Items.Insert(
            6,
            new ApplicationMenuItem(
                PikachuMenus.SystemManagement,
                "系統管理",
                url: "/SystemManagement",
                icon: "fas fa-cogs",
                order: 6
            ).AddItem(new ApplicationMenuItem(
                name: "Permission",
                displayName: "權限管理",
                url: "/SystemManagement/Permission")
            //.RequirePermissions("MyProject.Crm.Orders")
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

        return Task.CompletedTask;
    }
}
