using Asp.Versioning;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Controllers.Items;

[RemoteService(IsEnabled = true)]
[ControllerName("ItemDetails")]
[Area("app")]
[Route("api/app/item-details")]
public class ItemDetailsController(
    IItemDetailsAppService _itemDetailsAppService
    ) : AbpController, IItemDetailsAppService
{
    [HttpPost]
    public Task<ItemDetailsDto> CreateAsync(CreateItemDetailsDto input)
    {
        return _itemDetailsAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _itemDetailsAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<ItemDetailsDto> GetAsync(Guid id)
    {
        return _itemDetailsAppService.GetAsync(id);
    }

    [HttpGet("get-inventory-report")]
    public Task<PagedResultDto<ItemDetailsDto>> GetInventroyReport(GetInventroyInputDto input)
    {
        return _itemDetailsAppService.GetInventroyReport(input);
    }

    [HttpGet("get-list-as-excel")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(InventroyExcelDownloadDto input)
    {
        return _itemDetailsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    public Task<PagedResultDto<ItemDetailsDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _itemDetailsAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ItemDetailsDto> UpdateAsync(Guid id, CreateItemDetailsDto input)
    {
        return _itemDetailsAppService.UpdateAsync(id, input);
    }
}
