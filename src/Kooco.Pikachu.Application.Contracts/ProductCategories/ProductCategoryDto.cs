using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }
    public List<ProductCategoryImageDto> ProductCategoryImages { get; set; } = [];
    public List<CategoryProductDto> CategoryProducts { get; set; } = [];
}