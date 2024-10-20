using System;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryImageDto
{
    public string Url { get; set; }
    public string BlobName { get; set; }
    public string? Name { get; set; }
    public Guid ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public int SortNo { get; set; }
}