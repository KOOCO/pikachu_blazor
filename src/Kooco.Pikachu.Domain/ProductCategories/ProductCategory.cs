using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategory : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [RequireOneOf(nameof(ZHName))]
    public string? Name { get; set; }
    [RequireOneOf(nameof(Name))]
    public string? ZHName { get; set; }
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? MainCategoryId { get; set; }
    public List<ProductCategoryImage> ProductCategoryImages { get; set; }
    public List<CategoryProduct> CategoryProducts { get; set; }

    protected ProductCategory() { }
    public ProductCategory(
        Guid id,
        string? name,
        string? zhName,
        string? description,
        Guid? mainCategoryId=null
        ) : base(id)
    {
        Name = name;
        ZHName = zhName;
        Description = description;
        MainCategoryId = mainCategoryId;
        ProductCategoryImages = [];
        CategoryProducts = [];
    }



  

    public ProductCategoryImage AddProductCategoryImage(Guid id, string url, string blobName, string? name, int sortNo)
    {
        if (ProductCategoryImages.Count >= ProductCategoryConsts.MaxImageLimit)
        {
            throw new ProductCategoryImagesMaxLimitException();
        }
        var productCategoryImage = new ProductCategoryImage(id, Id, url, blobName, name, sortNo);
        ProductCategoryImages.Add(productCategoryImage);
        return productCategoryImage;
    }

    public CategoryProduct AddCategoryProduct(Guid itemId)
    {
        var categoryProduct = new CategoryProduct(itemId, Id);
        CategoryProducts.AddIfNotContains(categoryProduct);
        return categoryProduct;
    }
}
