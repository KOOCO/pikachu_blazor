using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement;

public class GetWebsiteSettingsListDto : PagedAndSortedResultRequestDto
{
    public string? Filter {  get; set; }
    public string? PageTitle { get; set; }
    public bool? SetAsHomePage { get; set; }
    public WebsitePageType? PageType { get; set; }
    public GroupBuyTemplateType? TemplateType { get; set; }
    public Guid? ProductCategoryId { get; set; }
}