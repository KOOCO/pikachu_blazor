using Kooco.Pikachu.Campaigns;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Volo.Abp.Reflection;

namespace Kooco.Pikachu.EdmManagement;

public class EdmConsts
{
    public const string DefaultSorting = "CreationTime DESC";
    public const int MaxSubjectLength = 255;

    public static string GetSortingOrDefault(string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting)) return DefaultSorting;
        return sorting!;
    }
}

public class EdmTemplateConsts
{
    public const string Main = "edm_main";
    public const string CustomizeName = "edm_customize";
    public const string CampaignName = "edm_campaign_main";
    public const string CampaignDiscountName = "edm_campaign_discount";
    public const string CampaignShoppingCreditName = "edm_campaign_shopping_credit";
    public const string CampaignAddOnProductName = "edm_campaign_addon_product";
    public const string ShopCartName = "edm_shopping_cart_main";
    public const string ShopCartItemsName = "edm_shopping_cart_items";

    public static string GetTemplateName(EdmTemplateType edmTemplateType)
    {
        var templateName = edmTemplateType switch
        {
            EdmTemplateType.Customize => CustomizeName,
            EdmTemplateType.Campaign => CampaignName,
            EdmTemplateType.ShoppingCart => ShopCartName,
            _ => string.Empty
        };

        return templateName;
    }

    public static string GetTemplateName(PromotionModule promotionModule)
    {
        var templateName = promotionModule switch
        {
            PromotionModule.Discount => CampaignDiscountName,
            PromotionModule.ShoppingCredit => CampaignShoppingCreditName,
            PromotionModule.AddOnProduct => CampaignAddOnProductName,
            _ => string.Empty
        };

        return templateName;
    }

    public static string GetTemplatePath(string templateName)
    {
        return Path.Combine("wwwroot", "EmailTemplates", "Edms", $"{templateName}.html");
    }

    public static string GetTemplate(PromotionModule promotionModule)
    {
        return GetTemplate(GetTemplateName(promotionModule));
    }

    public static string GetTemplate(EdmTemplateType edmTemplateType)
    {
        return GetTemplate(GetTemplateName(edmTemplateType));
    }

    public static string GetTemplate(string templateName)
    {
        return File.ReadAllText(GetTemplatePath(templateName));
    }
}

public class EdmTemplatePlaceholders
{
    public const string AddOnProductAmount = "{{AddOnProductAmount}}";
    public const string ApplicableProductName = "{{ApplicableProductName_OR_所有商品}}";
    public const string CampaignName = "{{CampaignName}}";
    public const string CampaignPeriod = "{{CampaignPeriod}}";
    public const string CapCashbackAmount = "{{CapCashbackAmount}}";
    public const string Discount = "{{Discount}}";
    public const string DiscountCode = "{{DiscountCode_OR_無需折扣碼}}";
    public const string GroupBuyName = "{{GroupBuyName}}";
    public const string LimitPerOrder = "{{LimitPerOrder}}";
    public const string MaximumUsePerPerson = "{{MaximumUsePerPerson}}";
    public const string MemberName = "{{MemberName}}";
    public const string MinimumSpendAmount = "{{MinimumSpendAmount}}";
    public const string ProductName = "{{ProductName}}";
    public const string Threshold = "{{Threshold}}";
    public const string ValidForDays = "{{獲取後ValidForDays天內使用_OR_不限時間}}";

    public const string DateFormat = "yyyy/MM/dd";
    public const string TemplateTypeMain = "{{edm_template_type_main}}";
    public const string CampaignProperties = "{{edm_campaign_properties}}";
    public const string PromotionModule = "{{edm_promotion_module_template}}";
    public const string ShopCartItems = "{{edm_shopping_cart_items}}";

    public static string[] GetTemplateTypeConstants(EdmTemplateType? edmTemplateType)
    {
        var constants = GetAllInsertable();

        return [.. GetAllInsertable()
            .Where(x => edmTemplateType != null)
            .AsQueryable()
            .WhereIf(edmTemplateType == EdmTemplateType.Customize || edmTemplateType == EdmTemplateType.ShoppingCart,
                x => new[] { MemberName, GroupBuyName }.Contains(x))
            ];
    }

    public static string[] GetAllInsertable()
    {
        var nonInsertables = new[] { TemplateTypeMain, CampaignProperties, PromotionModule, ShopCartItems };
        return [.. GetAll().Where(x => !nonInsertables.Contains(x))];
    }

    public static string[] GetAll()
    {
        return [.. ReflectionHelper.GetPublicConstantsRecursively(typeof(EdmTemplatePlaceholders)).OrderBy(x => x)];
    }
}