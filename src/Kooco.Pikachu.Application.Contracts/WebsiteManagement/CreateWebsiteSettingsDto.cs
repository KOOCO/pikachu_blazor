﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement;

public class CreateWebsiteSettingsDto
{
    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageTitleLength)]
    public string PageTitle { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageDescriptionLength)]
    public string PageDescription { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageLinkLength)]
    public string PageLink { get; set; }

    public bool SetAsHomePage { get; set; }

    [Required]
    public WebsitePageType? PageType { get; set; }

    [Required]
    public GroupBuyTemplateType? TemplateType { get; set; }

    public GroupBuyModuleType? GroupBuyModuleType { get; set; }

    public Guid? ProductCategoryId { get; set; }

    public string? ArticleHtml { get; set; }

    public List<WebsiteSettingsModuleDto> Modules { get; set; } = [];

    public List<WebsiteSettingsOverviewModuleDto> OverviewModules { get; set; } = [];

    public List<WebsiteSettingsInstructionModuleDto> InstructionModules { get; set; } = [];
    
    public List<WebsiteSettingsProductRankingModuleDto> ProductRankingModules { get; set; } = [];
}