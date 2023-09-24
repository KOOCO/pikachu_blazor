using Kooco.Pikachu.GroupBuys;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.GroupBuys;

[RemoteService(IsEnabled = true)]
[ControllerName("GroupBuy")]
[Area("app")]
[Route("api/app/group-buy")]
public class GroupBuyController : AbpController, IGroupBuyAppService
{
    private readonly IGroupBuyAppService _groupBuyAppService;
    public GroupBuyController(
        IGroupBuyAppService groupBuyAppService
        )
    {
        _groupBuyAppService = groupBuyAppService;
    }

    [HttpPost]
    public Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
    {
        return _groupBuyAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _groupBuyAppService.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
    {
        return _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyIds);
    }

    [HttpGet("{id}")]
    public Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return _groupBuyAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("get-list-for-store")]
    public Task<List<GroupBuyDto>> GetListForStoreAsync()
    {
        return _groupBuyAppService.GetListForStoreAsync();
    }

    [HttpGet("get-list")]
    public Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
    {
        return _groupBuyAppService.GetListAsync(input);
    }

    [HttpGet("with-details/{id}")]
    public Task<GroupBuyDto> GetWithDetailsAsync(Guid id)
    {
        return _groupBuyAppService.GetWithDetailsAsync(id);
    }

    [HttpPut("{id}")]
    public Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
    {
        return _groupBuyAppService.UpdateAsync(id, input);
    }

    [HttpGet("get-carousel-images/{id}")]
    public Task<List<string>> GetCarouselImagesAsync(Guid id)
    {
        return _groupBuyAppService.GetCarouselImagesAsync(id);
    }
}
