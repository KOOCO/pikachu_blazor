using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.Freebies;

[RemoteService(IsEnabled = true)]
[ControllerName("Freebies")]
[Area("app")]
[Route("api/app/freebies")]
public class FreebieController(
    IFreebieAppService _freebieAppService
    ) : AbpController, IFreebieAppService
{
    [HttpPut("change-availability/{freebieId}")]
    public Task ChangeFreebieAvailability(Guid freebieId)
    {
        return _freebieAppService.ChangeFreebieAvailability(freebieId);
    }

    [HttpPost]
    public Task<FreebieDto> CreateAsync(FreebieCreateDto input)
    {
        return _freebieAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _freebieAppService.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public Task DeleteManyItemsAsync(List<Guid> setItemIds)
    {
        return _freebieAppService.DeleteManyItemsAsync(setItemIds);
    }

    [HttpDelete("delete-single-image/{itemId}/{blobImageName}")]
    public Task DeleteSingleImageAsync(Guid itemId, string blobImageName)
    {
        return _freebieAppService.DeleteSingleImageAsync(itemId, blobImageName);
    }

    [HttpGet("{id}/{includeDetails}")]
    public Task<FreebieDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return _freebieAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("{id}")]
    public Task<FreebieDto> GetAsync(Guid id)
    {
        return ((IReadOnlyAppService<FreebieDto, FreebieDto, Guid, PagedAndSortedResultRequestDto>)_freebieAppService).GetAsync(id);
    }

    [HttpGet("get-list")]
    public Task<List<FreebieDto>> GetListAsync()
    {
        return _freebieAppService.GetListAsync();
    }

    [HttpGet]
    public Task<PagedResultDto<FreebieDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _freebieAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<FreebieDto> UpdateAsync(Guid id, UpdateFreebieDto input)
    {
        return _freebieAppService.UpdateAsync(id, input);
    }
}
