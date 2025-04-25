using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class GetCampaignListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public bool? IsEnabled { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
