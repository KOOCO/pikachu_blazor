using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryAppService : PikachuAppService, IProductCategoryAppService
{
    public Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductCategoryDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResultDto<ProductCategoryDto>> GetListAsync(GetProductCategoryListDto input)
    {
        throw new NotImplementedException();
    }

    public Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input)
    {
        throw new NotImplementedException();
    }
}
