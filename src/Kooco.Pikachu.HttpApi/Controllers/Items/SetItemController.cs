using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.Items;

[RemoteService(IsEnabled = true)]
[ControllerName("SetItems")]
[Area("app")]
[Route("api/app/set-items")]
public class SetItemController(
    ISetItemAppService _setItemAppService
    ) : AbpController, ISetItemAppService
{
    [HttpPost]
    public Task<SetItemDto> CreateAsync(CreateUpdateSetItemDto input)
    {
        return _setItemAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _setItemAppService.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public Task DeleteManyItemsAsync(List<Guid> setItemIds)
    {
        return _setItemAppService.DeleteManyItemsAsync(setItemIds);
    }

    [HttpDelete("delete-single-image/{id}/{blobImageName}")]
    public Task DeleteSingleImageAsync(Guid id, string blobImageName)
    {
        return _setItemAppService.DeleteSingleImageAsync(id, blobImageName);
    }

    [HttpGet("{id}/{includeDetails}")]
    public Task<SetItemDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return _setItemAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("{id}")]
    public Task<SetItemDto> GetAsync(Guid id)
    {
        return ((IReadOnlyAppService<SetItemDto, SetItemDto, Guid, PagedAndSortedResultRequestDto>)_setItemAppService).GetAsync(id);
    }
  

    [HttpGet("get-items-lookup")]
    public Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync()
    {
        return _setItemAppService.GetItemsLookupAsync();
    }

    [HttpGet("get-list")]
    public Task<PagedResultDto<SetItemDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _setItemAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<SetItemDto> UpdateAsync(Guid id, CreateUpdateSetItemDto input)
    {
        return _setItemAppService.UpdateAsync(id, input);
    }
}
