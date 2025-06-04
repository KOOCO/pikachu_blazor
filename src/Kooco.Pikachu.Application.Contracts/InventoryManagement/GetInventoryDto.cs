using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.InventoryManagement;

public class GetInventoryDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? ItemId { get; set; }
    public int? MinCurrentStock { get; set; }
    public int? MaxCurrentStock { get; set; }
    public int? MinAvailableStock { get; set; }
    public int? MaxAvailableStock { get; set; }
}
