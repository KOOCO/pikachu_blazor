﻿using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategory : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Name { get; private set; }
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }

    public List<ProductCategoryImage> ProductCategoryImages { get; set; }
    public List<CategoryProduct> CategoryProducts { get; set; }

    public ProductCategory(
        Guid id,
        string name,
        string? description
        ) : base(id)
    {
        SetName(name);
        Description = description;

        ProductCategoryImages = [];
        CategoryProducts = [];
    }

    public ProductCategory ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(Name), maxLength: ProductCategoryConsts.MaxNameLength);
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
