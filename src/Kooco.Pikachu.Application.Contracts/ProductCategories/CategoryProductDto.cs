using System;

namespace Kooco.Pikachu.ProductCategories;

public class CategoryProductDto
{
    public Guid ItemId { get; set; }
    public Guid ProductCategoryId { get; set; }
    public string? ProductCategoryFirstImageUrl { get; set; }
    public string? ItemName { get; set; }
    public string? ProductCategoryName {  get; set; }
}