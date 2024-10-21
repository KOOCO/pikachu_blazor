using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ProductCategories;

public class UpdateProductCategoryDto
{
    [Required]
    [MaxLength(ProductCategoryConsts.MaxNameLength)]
    public string Name { get; set; }

    public string? Description { get; set; }

    public List<CreateUpdateProductCategoryImageDto> ProductCategoryImages { get; set; } = [];

    public List<CreateUpdateCategoryProductDto> CategoryProducts { get; set; } = [];
}