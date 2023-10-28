using Kooco.Pikachu.Freebies.Dtos;
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

    [HttpPut("ChangeGroupBuyAvailability/{id}")]
    public Task ChangeGroupBuyAvailability(Guid id)
    {
        return _groupBuyAppService.ChangeGroupBuyAvailability(id);
    }

    [HttpGet("get-carousel-images/{id}")]
    public Task<List<string>> GetCarouselImagesAsync(Guid id)
    {
        return _groupBuyAppService.GetCarouselImagesAsync(id);
    }

    [HttpGet("for-store/{id}")]
    public Task<GroupBuyDto> GetForStoreAsync(Guid id)
    {
        return _groupBuyAppService.GetForStoreAsync(id);
    }

    [HttpGet("freebie-store/{groupBuyId}")]
    public Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId)
    {
        return _groupBuyAppService.GetFreebieForStoreAsync(groupBuyId);
    }

    [HttpGet("check-shortcode-forcreate/{shortCode}")]
    public Task<bool> CheckShortCodeForCreate(string shortCode)
    {
        return _groupBuyAppService.CheckShortCodeForCreate(shortCode);
    }

    [HttpGet("check-shortcode-foredit")]
    public Task<bool> CheckShortCodeForEdit(string shortCode, Guid Id)
    {
       return _groupBuyAppService.CheckShortCodeForEdit(shortCode, Id);
    }

    [HttpGet("get-by-shortcode/{ShortCode}")]
    public Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode)
    {
        return _groupBuyAppService.GetGroupBuyByShortCode(ShortCode);
    }

    [HttpGet("for-store-with-details/{id}")]
    public Task<GroupBuyDto> GetWithDetailsForStoreAsync(Guid id)
    {
        return _groupBuyAppService.GetWithDetailsForStoreAsync(id);
    }


    [HttpGet("get-groupbuy-for-tenant")]
    public  Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode, Guid TenantId)
    {
        return _groupBuyAppService.GetGroupBuyofTenant(ShortCode, TenantId);
    }
}
