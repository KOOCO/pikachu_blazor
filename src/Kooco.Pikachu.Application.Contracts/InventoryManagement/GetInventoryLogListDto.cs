using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.InventoryManagement;

public class GetInventoryLogListDto : PagedAndSortedResultRequestDto
{
    public Guid ItemId { get; set; }
    public Guid ItemDetailId { get; set; }
}
