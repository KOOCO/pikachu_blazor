using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ProductCategories;

public interface IProductCategoryAppService : IApplicationService
{
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input);
    Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input);
    Task DeleteAsync(Guid id);
    Task<ProductCategoryDto> GetAsync(Guid id, bool includeDetails = false);
    Task<PagedResultDto<ProductCategoryDto>> GetListAsync(GetProductCategoryListDto input);
    Task<List<CreateUpdateProductCategoryImageDto>> UploadImagesAsync(List<CreateUpdateProductCategoryImageDto> input, bool deleteExisting = false);
    Task<List<KeyValueDto>> GetProductCategoryLookupAsync();
    Task<string?> GetDefaultImageUrlAsync(Guid id);
    Task<List<KeyValueDto>> GetMainProductCategoryLookupAsync();
    Task<List<ProductCategoryDto>> GetSubCategoryListAsync(Guid mainCategoryId);
}
