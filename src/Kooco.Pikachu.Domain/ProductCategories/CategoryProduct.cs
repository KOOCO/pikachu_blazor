using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ProductCategories;

public class CategoryProduct : Entity
{
    public Guid ItemId { get; set; }
    public Guid ProductCategoryId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    [ForeignKey(nameof(ProductCategoryId))]
    public ProductCategory? ProductCategory { get; set; }

    public CategoryProduct(
        Guid itemId,
        Guid productCategoryId
        )
    {
        ItemId = itemId;
        ProductCategoryId = productCategoryId;
    }

    public override object?[] GetKeys()
    {
        return [ItemId, ProductCategoryId];
    }
}
