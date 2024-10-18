using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ProductCategories;

public interface IProductCategoryAppService : IApplicationService
{
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input);
    Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input);
    Task DeleteAsync(Guid id);
    Task<ProductCategoryDto> GetAsync(Guid id);
    Task<PagedResultDto<ProductCategoryDto>> GetListAsync(GetProductCategoryListDto input);
}
