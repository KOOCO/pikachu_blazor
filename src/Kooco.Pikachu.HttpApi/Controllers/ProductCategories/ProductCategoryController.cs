using Asp.Versioning;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.ProductCategories;

[RemoteService(IsEnabled = true)]
[ControllerName("ProductCategories")]
[Area("app")]
[Route("api/app/product-categories")]
public class ProductCategoryController(IProductCategoryAppService productCategoryAppService) : PikachuController, IProductCategoryAppService
{
    [HttpPost]
    public Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input)
    {
        return productCategoryAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return productCategoryAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<ProductCategoryDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return productCategoryAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<ProductCategoryDto>> GetListAsync(GetProductCategoryListDto input)
    {
        return productCategoryAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input)
    {
        return productCategoryAppService.UpdateAsync(id, input);
    }
}
