using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Data;
using static Kooco.Pikachu.EdmManagement.EdmTemplateConsts;

namespace Kooco.Pikachu.EdmManagement;

public class EdmTemplateBuilder
{
    public static string Build(Edm edm, TenantSettingsDto tenantSettings, Campaign? campaign, string? groupBuyName)
    {
        string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{edm.GroupBuyId}";

        var mainTemplate = GetTemplate(Main);

        var template = mainTemplate
            .Replace("{{LogoUrl}}", tenantSettings?.LogoUrl)
            .Replace("{{FacebookUrl}}", tenantSettings?.FacebookLink)
            .Replace("{{InstagramUrl}}", tenantSettings?.InstagramLink)
            .Replace("{{LineUrl}}", tenantSettings?.LineLink)
            .Replace("{{CurrentYear}}", DateTime.Today.Year.ToString())
            .Replace("{{CompanyName}}", tenantSettings?.CompanyName)
            .Replace("{{ButtonLink}}", groupBuyUrl)
            .Replace(EdmTemplatePlaceholders.TemplateTypeMain, edm.Message)
            .Replace(EdmTemplatePlaceholders.GroupBuyName, groupBuyName ?? "N/A")
            .Replace(EdmTemplatePlaceholders.CampaignName, campaign?.Name)
            .Replace(EdmTemplatePlaceholders.CampaignPeriod, $"{campaign?.StartDate:dd/MM/yyyy} to {campaign?.EndDate:dd/MM/yyyy}")
            .Replace(EdmTemplatePlaceholders.DiscountCode, campaign?.Discount?.DiscountCode)
            .Replace(EdmTemplatePlaceholders.MinimumSpendAmount, campaign?.Discount?.MinimumSpendAmount?.ToString() ?? "N/A")
            .Replace(EdmTemplatePlaceholders.MaximumUsePerPerson, campaign?.Discount?.MaximumUsePerPerson.ToString() ?? "N/A")
            .Replace(EdmTemplatePlaceholders.ValidForDays, campaign?.ShoppingCredit?.ValidForDays?.ToString() ?? "N/A")
            .Replace(EdmTemplatePlaceholders.Threshold, campaign?.AddOnProduct?.Threshold?.ToString() ?? "N/A")
            .Replace(EdmTemplatePlaceholders.LimitPerOrder, campaign?.AddOnProduct?.LimitPerOrder.ToString() ?? "N/A");

        template = template.Replace(EdmTemplatePlaceholders.Discount,
            campaign?.PromotionModule switch
            {
                PromotionModule.Discount => campaign?.Discount?.DiscountType == DiscountType.FixedAmount ? $"${campaign?.Discount?.DiscountAmount}" : $"{campaign?.Discount?.DiscountPercentage}%" ?? "N/A",
                PromotionModule.ShoppingCredit => campaign?.ShoppingCredit?.CalculationMethod == CalculationMethod.UnifiedCalculation ? $"{campaign?.ShoppingCredit?.CalculationPercentage}%" : $"${campaign?.ShoppingCredit?.GetMaxPointsToReceive()}" ?? "N/A",
                PromotionModule.AddOnProduct => campaign?.AddOnProduct?.ProductAmount != null ? "$" + campaign?.AddOnProduct?.ProductAmount.ToString() : "N/A",
                _ => "N/A"
            });

        return template;
    }

    public static string BuildForMember(string template, Guid id, string name, List<ShopCart> shopCarts)
    {
        var memberTemplate = template.Replace(EdmTemplatePlaceholders.MemberName, name);

        var memberCartItems = shopCarts.Where(sc => sc.UserId == id).SelectMany(sc => sc.CartItems);

        if (memberCartItems.Any())
        {
            var shopCartItemsTemplate = GetTemplate(ShopCartItemsName);

            var memberItemsTemplate = string.Join("", memberCartItems.Select(ci =>
                shopCartItemsTemplate
                    .Replace("{{ItemName}}", ci.Item?.ItemName ?? ci.SetItem?.SetItemName)
                    .Replace("{{ItemDetails}}", "")
                    .Replace("{{GroupBuyPrice}}", ci.GroupBuyPrice.ToString("N2"))
                    .Replace("{{SellingPrice}}", ci.SellingPrice.ToString("N2"))
                    .Replace("{{ItemQuantity}}", ci.Quantity.ToString("N0"))
                    .Replace("{{ItemTotal}}", (ci.GroupBuyPrice * ci.Quantity).ToString("N2"))));

            memberTemplate = memberTemplate.Replace(EdmTemplatePlaceholders.ShopCartItems, memberItemsTemplate);
        }

        return memberTemplate;
    }
}
