using System;
using Volo.Abp.Application.Dtos;
namespace Kooco.Pikachu.Items;

public class GetItemListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? ItemId { get; set; }
    public DateTime? MinAvailableTime { get; set; }
    public DateTime? MaxAvailableTime { get; set; }
    public bool? IsFreeShipping { get; set; }
    public bool? IsAvailable { get; set; }
}