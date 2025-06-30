using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryDto : FullAuditedEntityDto<Guid>
{
    [RequireOneOf(nameof(ZHName))]
    public string? Name { get; set; }
    [RequireOneOf(nameof(Name))]
    public string? ZHName { get; set; }
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? MainCategoryId { get; set; }
    public List<ProductCategoryImageDto> ProductCategoryImages { get; set; } = [];
    public List<CategoryProductDto> CategoryProducts { get; set; } = [];
}