using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ProductCategories;

public class UpdateProductCategoryDto
{
  
    [MaxLength(ProductCategoryConsts.MaxNameLength)]
 
    [RequireOneOf(nameof(ZHName))]
    public string? Name { get; set; }
    [RequireOneOf(nameof(Name))]
    public string? ZHName { get; set; }
    public string? Description { get; set; }
    public Guid? MainCategoryId { get; set; }
    public List<CreateUpdateProductCategoryImageDto> ProductCategoryImages { get; set; } = [];

    public List<CreateUpdateCategoryProductDto> CategoryProducts { get; set; } = [];
}