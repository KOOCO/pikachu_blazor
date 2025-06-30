using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryManager(IProductCategoryRepository productCategoryRepository) : DomainService
{
    public async Task<ProductCategory> CreateAsync(string? name,string? zhName, string? description,Guid? mianCategory)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: ProductCategoryConsts.MaxNameLength);

        var existing = await productCategoryRepository.FindByNameAsync(name);
        if (existing != null)
        {
            throw new ProductCategoryAlreadyExistsException(name);
        }

        var productCategory = new ProductCategory(GuidGenerator.Create(), name,zhName, description, mianCategory);
        await productCategoryRepository.InsertAsync(productCategory);
        return productCategory;
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory productCategory, string? name,string?zhName, string? description,Guid? mainCategory)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(zhName))
        {
            throw new BusinessException("ProductCategory.NameOrZhNameRequired")
                .WithData("Name", name)
                .WithData("ZHName", zhName);
        }
        if (!string.IsNullOrWhiteSpace(name))
        {
            var existing = await productCategoryRepository.FindByNameAsync(name);
            if (existing != null && existing.Id != productCategory.Id)
            {
                throw new ProductCategoryAlreadyExistsException(name);
            }
        }
        if (!string.IsNullOrWhiteSpace(zhName))
        {
            var existing = await productCategoryRepository.FindByZhNameAsync(zhName);
            if (existing != null && existing.Id != productCategory.Id)
            {
                throw new ProductCategoryAlreadyExistsException(zhName);
            }
        }
        productCategory.Name = name;
        productCategory.Description = description;
        productCategory.MainCategoryId = mainCategory;

        await productCategoryRepository.UpdateAsync(productCategory);
        return productCategory;
    }

    public ProductCategory AddProductCategoryImage(ProductCategory productCategory, string url, string blobName, string? name, int sortNo)
    {
        Check.NotNull(productCategory, nameof(ProductCategory));
        Check.NotNullOrWhiteSpace(url, nameof(url));
        Check.NotNullOrWhiteSpace(blobName, nameof(blobName));

        if (productCategory.ProductCategoryImages.Count >= ProductCategoryConsts.MaxImageLimit)
        {
            throw new ProductCategoryImagesMaxLimitException();
        }

        productCategory.AddProductCategoryImage(GuidGenerator.Create(), url, blobName, name, sortNo);
        return productCategory;
    }

    public ProductCategory AddCategoryProduct(ProductCategory productCategory, Guid itemId)
    {
        Check.NotNull(productCategory, nameof(ProductCategory));
        Check.NotDefaultOrNull<Guid>(itemId, nameof(itemId));

        productCategory.AddCategoryProduct(itemId);
        return productCategory;
    }
}
