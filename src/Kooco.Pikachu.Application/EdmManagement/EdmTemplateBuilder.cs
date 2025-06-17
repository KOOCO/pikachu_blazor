using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using static Kooco.Pikachu.EdmManagement.EdmTemplateConsts;

namespace Kooco.Pikachu.EdmManagement;

public class EdmTemplateBuilder : ITransientDependency
{
    private readonly IEdmRepository _edmRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ITenantSettingsAppService _tenantSettingsAppService;
    private readonly IItemRepository _itemAppService;

    public EdmTemplateBuilder(
        IEdmRepository edmRepository,
        ICampaignRepository campaignRepository,
        ITenantSettingsAppService tenantSettingsAppService,
        IItemRepository itemAppService)
    {
        _edmRepository = edmRepository;
        _campaignRepository = campaignRepository;
        _tenantSettingsAppService = tenantSettingsAppService;
        _itemAppService = itemAppService;
    }

    public async Task<string> BuildAsync(Edm edm)
    {
        var groupBuyName = await _edmRepository.GetGroupBuyNameAsync(edm.GroupBuyId);

        var tenantSettings = await _tenantSettingsAppService.FirstOrDefaultAsync();

        var campaign = edm.CampaignId.HasValue
            ? await _campaignRepository.GetWithDetailsAsync(edm.CampaignId.Value)
            : null;

        string NA = "N/A";
        string groupBuyUrl = tenantSettings?.Tenant.GetProperty<string>(Constant.TenantUrl)?.EnsureEndsWith('/') + $"groupBuy/{edm.GroupBuyId}";
        string campaignPeriod = $"{DateFormat(campaign?.StartDate)} to {DateFormat(campaign?.EndDate)}";
        string discountCode = !string.IsNullOrWhiteSpace(campaign?.Discount?.DiscountCode) ? campaign.Discount.DiscountCode : "無需折扣碼";
        string discount = campaign?.Discount?.DiscountType == DiscountType.FixedAmount ? $"${campaign?.Discount?.DiscountAmount}" : $"{campaign?.Discount?.DiscountPercentage}%" ?? NA;

        (string? addOnProductName, string? applicableProducts) = await GetProductNames(campaign);
        
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
            .Replace(EdmTemplatePlaceholders.ProductName, addOnProductName ?? NA)
            .Replace(EdmTemplatePlaceholders.AddOnProductAmount, campaign?.AddOnProduct?.ProductAmount.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.ApplicableProductName, applicableProducts)
            .Replace(EdmTemplatePlaceholders.CampaignName, campaign?.Name)
            .Replace(EdmTemplatePlaceholders.CampaignPeriod, campaignPeriod)
            .Replace(EdmTemplatePlaceholders.CapCashbackAmount, campaign?.ShoppingCredit?.CapAmount.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.Discount, discount)
            .Replace(EdmTemplatePlaceholders.DiscountCode, discountCode)
            .Replace(EdmTemplatePlaceholders.GroupBuyName, groupBuyName ?? NA)
            .Replace(EdmTemplatePlaceholders.LimitPerOrder, campaign?.AddOnProduct?.LimitPerOrder.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.MinimumSpendAmount, campaign?.Discount?.MinimumSpendAmount?.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.MaximumUsePerPerson, campaign?.Discount?.MaximumUsePerPerson.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.Threshold, campaign?.AddOnProduct?.Threshold?.ToString() ?? NA)
            .Replace(EdmTemplatePlaceholders.ValidForDays, campaign?.ShoppingCredit?.ValidForDays?.ToString() ?? "不限時間");

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

    static string? DateFormat(DateTime? date)
    {
        return date?.ToString(EdmTemplatePlaceholders.DateFormat);
    }

    async Task<(string? addOnProductName, string? applicableProducts)> GetProductNames(Campaign campaign)
    {
        string? addOnProductName = "";
        List<string> applicableProductNames = [];
        string? applicableProducts = "";

        var itemQueryable = await _itemAppService.GetQueryableAsync();

        if (campaign?.AddOnProduct != null)
        {
            addOnProductName = await itemQueryable
                .Where(item => item.Id == campaign.AddOnProduct.ProductId)
                .Select(item => item.ItemName)
                .FirstOrDefaultAsync();
        }

        if (campaign?.Products is { Count: > 0 })
        {
            var productIds = campaign.Products.Select(product => product.ProductId);
            applicableProductNames = await itemQueryable
                .Where(item => productIds.Contains(item.Id))
                .Select(item => item.ItemName)
                .ToListAsync();
        }

        applicableProducts = campaign?.ApplyToAllProducts == false
                ? applicableProductNames?.JoinAsString(", ")
                : "所有商品";
        
        return (addOnProductName, applicableProducts);
    }
}
