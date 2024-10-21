using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ProductCategories;

public class GetProductCategoryListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}