using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettings : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string PageTitle { get; private set; }
    public string? PageDescription { get; private set; }
    public string PageLink { get; private set; }
    public bool SetAsHomePage { get; set; }
    public WebsitePageType PageType { get; set; }
    public GroupBuyTemplateType? TemplateType { get; set; }
    public GroupBuyModuleType? GroupBuyModuleType { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string? ArticleHtml { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<WebsiteSettingsModule> Modules { get; set; }
    public virtual ICollection<WebsiteSettingsOverviewModule> OverviewModules { get; set; }
    public virtual ICollection<WebsiteSettingsInstructionModule> InstructionModules { get; set; }
    public virtual ICollection<WebsiteSettingsProductRankingModule> ProductRankingModules { get; set; }

    public WebsiteSettings()
    {
        Modules = new List<WebsiteSettingsModule>();
        OverviewModules = new List<WebsiteSettingsOverviewModule>();
        InstructionModules = new List<WebsiteSettingsInstructionModule>();
        ProductRankingModules = new List<WebsiteSettingsProductRankingModule>();
    }

    public WebsiteSettings(
        Guid id,
        string pageTitle,
        string pageDescription,
        string pageLink,
        bool setAsHomePage,
        WebsitePageType pageType,
        GroupBuyTemplateType? templateType,
        GroupBuyModuleType? groupBuyModuleType,
        Guid? productCategoryId,
        string? articleHtml
        ) : base(id)
    {
        SetPageTitle(pageTitle);
        SetPageDescription(pageDescription);
        SetPageLink(pageLink);
        SetHomePage(setAsHomePage);
        SetPageType(pageType);
        TemplateType = templateType;
        GroupBuyModuleType = groupBuyModuleType;
        ProductCategoryId = productCategoryId;
        ArticleHtml = articleHtml;
        Modules = new List<WebsiteSettingsModule>();
        OverviewModules = new List<WebsiteSettingsOverviewModule>();
        InstructionModules = new List<WebsiteSettingsInstructionModule>();
        ProductRankingModules = new List<WebsiteSettingsProductRankingModule>();
    }

    public WebsiteSettings SetPageTitle(string pageTitle)
    {
        PageTitle = Check.NotNullOrWhiteSpace(pageTitle, nameof(PageTitle), maxLength: WebsiteSettingsConsts.MaxPageTitleLength);
        return this;
    }

    public WebsiteSettings SetPageDescription(string pageDescription)
    {
        PageDescription = Check.NotNullOrWhiteSpace(pageDescription, nameof(PageDescription), maxLength: WebsiteSettingsConsts.MaxPageDescriptionLength);
        return this;
    }

    public WebsiteSettings SetPageLink(string pageLink)
    {
        PageLink = Check.NotNullOrWhiteSpace(pageLink, nameof(PageLink), maxLength: WebsiteSettingsConsts.MaxPageLinkLength);
        return this;
    }

    public WebsiteSettings SetHomePage(bool setAsHomePage)
    {
        SetAsHomePage = setAsHomePage;
        return this;
    }

    public WebsiteSettings SetPageType(WebsitePageType pageType)
    {
        PageType = pageType;
        return this;
    }

    public WebsiteSettingsModule AddModule(Guid id, int sortOrder, GroupBuyModuleType groupBuyModuleType, string? additionalInfo,
        string productGroupModuleTitle, string? productGroupModuleImageSize, int? moduleNumber)
    {
        var module = new WebsiteSettingsModule(id, Id, sortOrder, groupBuyModuleType,
                    additionalInfo, productGroupModuleTitle, productGroupModuleImageSize, moduleNumber);
        Modules.Add(module);
        return module;
    }

    public WebsiteSettingsOverviewModule AddOverviewModule(Guid id, string title, string image, string subTitle, string? bodyText,
        bool isButtonEnable, string? buttonText, string? buttonLink)
    {
        var overviewModule = new WebsiteSettingsOverviewModule(id, Id, title, image, subTitle, bodyText, isButtonEnable,
            buttonText, buttonLink);
        OverviewModules.Add(overviewModule);
        return overviewModule;
    }

    public WebsiteSettingsInstructionModule AddInstructionModule(Guid id, string title, string image, string bodyText)
    {
        var instructionModule = new WebsiteSettingsInstructionModule(id, Id, title, image, bodyText);
        InstructionModules.Add(instructionModule);
        return instructionModule;
    }

    public WebsiteSettingsProductRankingModule AddProductRankingModule(Guid id, string title, string subTitle,
        string? content, int? moduleNumber)
    {
        var productRanking = new WebsiteSettingsProductRankingModule(id, Id, title, subTitle, content, moduleNumber);
        return productRanking;
    }
}