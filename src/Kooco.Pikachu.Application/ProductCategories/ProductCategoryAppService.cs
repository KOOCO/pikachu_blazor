using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ProductCategories;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.ProductCategories.Default)]
public class ProductCategoryAppService(ProductCategoryManager productCategoryManager, IProductCategoryRepository productCategoryRepository,
    ImageContainerManager imageContainerManager) : PikachuAppService, IProductCategoryAppService
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
            productCategoryManager.AddProductCategoryImage(productCategory, image.Url, image.BlobName, image.Name, image.SortNo);
        }

        foreach (var categoryProduct in categoryProducts)
        {
            Check.NotDefaultOrNull(categoryProduct.ItemId, nameof(categoryProduct.ItemId));
            productCategoryManager.AddCategoryProduct(productCategory, categoryProduct.ItemId.Value);
        }
    }

    public async Task<List<CreateUpdateProductCategoryImageDto>> UploadImagesAsync(List<CreateUpdateProductCategoryImageDto> input, bool deleteExisting = false)
    {
        Check.NotNull(input, nameof(input));
        List<string> oldBlobNames = [];
        foreach (var image in input)
        {
            oldBlobNames.Add(image.BlobName);
            var bytes = Convert.FromBase64String(image.Base64);
            image.BlobName = GuidGenerator.Create().ToString().Replace("-", "") + Path.GetExtension(image.Name);
            image.Url = await imageContainerManager.SaveAsync(image.BlobName, bytes);
        }

        if (deleteExisting && oldBlobNames.Count > 0)
        {
            await DeleteOldImagesAsync(oldBlobNames).ConfigureAwait(false);
        }
        return input;
    }

    private async Task DeleteOldImagesAsync(List<string> oldBlobNames)
    {
        foreach (var oldBlobName in oldBlobNames)
        {
            try
            {
                await imageContainerManager.DeleteAsync(oldBlobName);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }

    [AllowAnonymous]
    public async Task<List<KeyValueDto>> GetProductCategoryLookupAsync()
    {
        var queryable = await productCategoryRepository.GetQueryableAsync();
        return [.. queryable.Select(x => new KeyValueDto { Id = x.Id, Name = x.Name })];
    }

    [AllowAnonymous]
    public async Task<string?> GetDefaultImageUrlAsync(Guid id)
    {
        var url = await productCategoryRepository.GetDefaultImageUrlAsync(id);
        return url;
    }
}
