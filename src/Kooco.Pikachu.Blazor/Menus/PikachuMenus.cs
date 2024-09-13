namespace Kooco.Pikachu.Blazor.Menus;

public class PikachuMenus
{
    private const string Prefix = "Pikachu";
    public const string Home = Prefix + ".Home";

    public const string GroupBuyManagement = Prefix + "GroupBuyManagement";
    public const string ProductManagement = Prefix + "ProductManagement";
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
}
