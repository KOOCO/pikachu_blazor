using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Tenants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Kooco.Pikachu.EdmManagement;

public class EdmTemplateBuilder
{
    public static async Task<string> Build(Edm edm, TenantSettingsDto tenantSettings, ICampaignRepository campaignRepository)
    {
        return await GetCampaignTemplate(edm, tenantSettings, campaignRepository);
    }

    #region Campaign Template
    private static async Task<string> GetCampaignTemplate(Edm edm, TenantSettingsDto tenantSettings, ICampaignRepository campaignRepository)
    {
        string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{edm.GroupBuyId}";

        var templatePath = Path.Combine("wwwroot", "EmailTemplates", "Edms", $"edm_campaign_main.html");
        var template = File.ReadAllText(templatePath)
            .Replace("{{LogoUrl}}", tenantSettings?.LogoUrl)
            .Replace("{{GroupBuyName}}", edm.GroupBuy?.GroupBuyName ?? "N/A")
            .Replace("{{Message}}", edm.Message)
            .Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink)
            .Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink)
            .Replace("{{LineUrl}}", tenantSettings?.LineLink)
            .Replace("{{CurrentYear}}", DateTime.Today.Year.ToString())
            .Replace("{{CompanyName}}", tenantSettings?.CompanyName);

        if (edm.TemplateType == EdmTemplateType.Campaign && edm.CampaignId.HasValue)
        {
            var campaign = await campaignRepository.GetWithDetailsAsync(edm.CampaignId.Value);

            template = template
                .Replace("{{CampaignName}}", campaign.Name)
                .Replace("{{CampaignPeriod}}", $"{campaign.StartDate:dd/MM/yyyy} to {campaign.EndDate:dd/MM/yyyy}")
                .Replace("{{ButtonLink}}", groupBuyUrl);

            template = InjectCampaignProperties(campaign, template);
        }

        return template;
    }

    private static string InjectCampaignProperties(Campaign campaign, string template)
    {
        var propTemplatePath = Path.Combine("wwwroot", "EmailTemplates", "Edms", "edm_campaign_property_cell.html");
        var propTemplate = File.ReadAllText(propTemplatePath);

        var properties = campaign.PromotionModule switch
        {
            PromotionModule.Discount when campaign.Discount != null => GetDiscountProperties(campaign.Discount),
            PromotionModule.ShoppingCredit when campaign.ShoppingCredit != null => GetShoppingCreditProperties(campaign.ShoppingCredit),
            PromotionModule.AddOnProduct when campaign.AddOnProduct != null => GetAddOnProductProperties(campaign.AddOnProduct),
            _ => Enumerable.Empty<CampaignProperty>()
        };

        var propsHtml = string.Join("", properties.Select(p =>
            propTemplate.Replace("{{PropIcon}}", p.Icon)
                        .Replace("{{PropName}}", p.Name)
                        .Replace("{{PropValue}}", p.Value)));

        return template.Replace("{{edm_campaign_properties}}", propsHtml);
    }

    private static List<CampaignProperty> GetDiscountProperties(CampaignDiscount discount)
    {
        return
        [
            new("💸", "折扣", discount.DiscountType == DiscountType.FixedAmount
                ? $"${discount.DiscountAmount}"
                : $"{discount.DiscountPercentage}%"),
            new("🏷", "您的代碼", discount.DiscountCode ?? "N/A"),
            new("💵", "最低消費金額", "$" + discount.MinimumSpendAmount?.ToString() ?? "N/A"),
            new("👤", "每人最高使用次數", discount.MaximumUsePerPerson.ToString())
        ];
    }

    private static List<CampaignProperty> GetShoppingCreditProperties(CampaignShoppingCredit credit)
    {
        return
        [
            new("💸", "折扣", credit.CalculationMethod == CalculationMethod.UnifiedCalculation
                ? $"{credit.CalculationPercentage}%"
                : $"${credit.GetMaxPointsToReceive()}"),
            new("🕒", "使用期限", credit.ValidForDays?.ToString() ?? "N/A")
        ];
    }

    private static List<CampaignProperty> GetAddOnProductProperties(CampaignAddOnProduct addOn)
    {
        return
        [
            new("💸", "折扣", addOn.ProductAmount.ToString()),
            new("💵", "最低消費金額", "$" + addOn.Threshold?.ToString() ?? "N/A"),
            new("📦", "每筆訂單限用次數", addOn.LimitPerOrder.ToString())
        ];
    }

    private record CampaignProperty(string Icon, string Name, string Value);
    #endregion
}
