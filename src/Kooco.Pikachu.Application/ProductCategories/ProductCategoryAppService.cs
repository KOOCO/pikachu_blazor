using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ProductCategories;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.ProductCategories.Default)]
public class ProductCategoryAppService(ProductCategoryManager productCategoryManager, IProductCategoryRepository productCategoryRepository) : PikachuAppService, IProductCategoryAppService
{
    [Authorize(PikachuPermissions.ProductCategories.Create)]
    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input)
    {
        Check.NotNull(input, nameof(input));

        var productCategory = await productCategoryManager.CreateAsync(input.Name, input.Description);

        AddCollections(productCategory, input.ProductCategoryImages, input.CategoryProducts, clearExisting: false);

        await productCategoryRepository.UpdateAsync(productCategory);

        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(productCategory);
    }

    [Authorize(PikachuPermissions.ProductCategories.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var productCategory = await productCategoryRepository.GetWithDetailsAsync(id);
        await productCategoryRepository.DeleteAsync(productCategory);
    }

    public async Task<ProductCategoryDto> GetAsync(Guid id, bool includeDetails = false)
    {
        var productCategory = includeDetails
            ? productCategoryRepository.GetWithDetailsAsync(id, true)
            : productCategoryRepository.GetAsync(id);

        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(await productCategory);
    }

    public async Task<PagedResultDto<ProductCategoryDto>> GetListAsync(GetProductCategoryListDto input)
    {
        var totalCount = await productCategoryRepository.GetCountAsync(input.Filter);

        var items = await productCategoryRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

        return new PagedResultDto<ProductCategoryDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(items)
        };
    }

    [Authorize(PikachuPermissions.ProductCategories.Edit)]
    public async Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input)
    {
        var productCategory = await productCategoryRepository.GetWithDetailsAsync(id);

        await productCategoryManager.UpdateAsync(productCategory, input.Name, input.Description);

        AddCollections(productCategory, input.ProductCategoryImages, input.CategoryProducts, clearExisting: true);

        await productCategoryRepository.UpdateAsync(productCategory);

        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(productCategory);
    }

    private void AddCollections(ProductCategory productCategory, List<CreateUpdateProductCategoryImageDto> productCategoryImages,
        List<CreateUpdateCategoryProductDto> categoryProducts, bool clearExisting = false)
    {
        if (clearExisting)
        {
            productCategory.ProductCategoryImages.Clear();
            productCategory.CategoryProducts.Clear();
        }

        foreach (var image in productCategoryImages)
        {
            productCategoryManager.AddProductCategoryImage(productCategory, image.Url, image.BlobName, image.Name);
        }

        foreach (var categoryProduct in categoryProducts)
        {
            Check.NotDefaultOrNull(categoryProduct.ItemId, nameof(categoryProduct.ItemId));
            productCategoryManager.AddCategoryProduct(productCategory, categoryProduct.ItemId.Value);
        }
    }
}
