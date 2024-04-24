using Asp.Versioning;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.Items;

[RemoteService(IsEnabled = true)]
[ControllerName("SetItemDetails")]
[Area("app")]
[Route("api/app/set-item-details")]
public class SetItemDetailsController (
    ISetItemDetailsAppService _setItemDetailsAppService
    ) : AbpController, ISetItemDetailsAppService
{
    [HttpPost]
    public Task<SetItemDetailsDto> CreateAsync(CreateUpdateSetItemDetailsDto input)
    {
        return _setItemDetailsAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _setItemDetailsAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<SetItemDetailsDto> GetAsync(Guid id)
    {
        return _setItemDetailsAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<SetItemDetailsDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _setItemDetailsAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<SetItemDetailsDto> UpdateAsync(Guid id, CreateUpdateSetItemDetailsDto input)
    {
        return _setItemDetailsAppService.UpdateAsync(id, input);
    }
}
