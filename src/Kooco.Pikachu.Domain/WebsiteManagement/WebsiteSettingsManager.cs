using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettingsManager(IWebsiteSettingsRepository websiteSettingsRepository) : DomainService
{
    public async Task<WebsiteSettings> CreateAsync(string pageTitle, string pageDescription, string pageLink, bool setAsHomePage, WebsitePageType pageType,
        GroupBuyTemplateType? templateType, GroupBuyModuleType? groupBuyModuleType, Guid? productCategoryId, string? articleHtml)
    {
        var websiteSettings = new WebsiteSettings(GuidGenerator.Create(), pageTitle, pageDescription, pageLink, setAsHomePage, pageType,
            templateType, groupBuyModuleType, productCategoryId, articleHtml);

        await websiteSettingsRepository.InsertAsync(websiteSettings);

        return websiteSettings;
    }

    public async Task<WebsiteSettings> UpdateAsync(WebsiteSettings websiteSettings, string pageTitle, string pageDescription, string pageLink, bool setAsHomePage,
        WebsitePageType pageType, GroupBuyTemplateType? templateType, GroupBuyModuleType? groupBuyModuleType, Guid? productCategoryId, string? articleHtml)
    {
        Check.NotNull(websiteSettings, nameof(WebsiteSettings));

        websiteSettings.SetPageTitle(pageTitle);
        websiteSettings.SetPageDescription(pageDescription);
        websiteSettings.SetPageLink(pageLink);
        websiteSettings.SetHomePage(setAsHomePage);
        websiteSettings.SetPageType(pageType);
        websiteSettings.TemplateType = templateType;
        websiteSettings.GroupBuyModuleType = groupBuyModuleType;
        websiteSettings.ProductCategoryId = productCategoryId;
        websiteSettings.ArticleHtml = articleHtml;

        await websiteSettingsRepository.UpdateAsync(websiteSettings);

        return websiteSettings;
    }
}
