using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Kooco.Pikachu.ProductCategories;

public class CreateProductCategoryDto
{
    [RequireOneOf(nameof(ZHName))]
    [MaxLength(ProductCategoryConsts.MaxNameLength)]
    public string? Name { get; set; }
    [RequireOneOf(nameof(Name))]
    public string? ZHName { get; set; }
    public string? Description { get; set; }
    public Guid? MainCategoryId { get; set; }

    public List<CreateUpdateProductCategoryImageDto> ProductCategoryImages { get; set; } = [];
    
    public List<CreateUpdateCategoryProductDto> CategoryProducts { get; set; } = [];
}