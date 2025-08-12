namespace Kooco.Pikachu.Blazor.Menus;

public class PikachuMenus
{
    private const string Prefix = "Pikachu";
    public const string Home = Prefix + ".Home";

    public const string GroupBuyManagement = Prefix + "GroupBuyManagement";
    public const string ProductManagement = Prefix + "ProductManagement";

    public const string Promotions = Prefix + "Promotions";
    public const string OrderManagement = Prefix + "OrderManagement";
    public const string PaymentManagement = Prefix + "PaymentManagement";
    //物流(後勤)
    public const string LogisticsManagement = Prefix + "LogisticsManagement";
    //多商戶管理
    public const string TenantManagement = Prefix + "TenantManagement";
    //系統管理(包含權限)
    public const string SystemManagement = Prefix + "SystemManagement";


    public const string Members = Prefix + ".Members";
    public const string MembersList = Prefix + ".MembersList";
    public const string LoginConfigurations = Prefix + ".LoginConfigurations";
    public const string TierManagement = Prefix + ".TierManagement";
    public const string MemberTags = Prefix + ".MemberTags";
    public const string ShopCarts = Prefix + ".ShopCarts";

    public const string TenantSettings = Prefix + ".TenantSettings";
    public const string TenantWallets = Prefix + ".TenantWallets";

    public const string WebsiteManagement = Prefix + ".WebsiteManagement";
    public const string WebsiteSettings = WebsiteManagement + ".WebsiteSettings";
    public const string TopbarSettings = WebsiteManagement + ".TopbarSettings";
    public const string FooterSettings = WebsiteManagement + ".FooterSettings";
    public const string WebsiteBasicSettings = WebsiteManagement + ".WebsiteBasicSettings";

    public const string EmailManagement = Prefix + ".EmailManagement";
    public const string EmailSettings = Prefix + ".EmailSettings";
    public const string EdmList = Prefix + ".EdmList";

    public const string Campaigns = Prefix + ".Campaigns";
    public const string TenantWalletManagement = Prefix + ".TenantWalletManagement";
    public const string LogisticsFeeManagement = Prefix + ".LogisticsFeeManagement";

    public const string InboxManagement = Prefix + ".Inbox";
}
