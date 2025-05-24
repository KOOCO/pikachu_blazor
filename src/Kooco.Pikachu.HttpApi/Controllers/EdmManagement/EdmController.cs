using Asp.Versioning;
using Kooco.Pikachu.EdmManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.EdmManagement;

[RemoteService]
[ControllerName("Edms")]
[Area("app")]
[Route("api/app/edms")]
public class EdmController(IEdmAppService edmAppService) : PikachuController, IEdmAppService
{
    [HttpPost]
    public Task<EdmDto> CreateAsync(CreateEdmDto input)
    {
        return edmAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return edmAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<EdmDto> GetAsync(Guid id)
    {
        return edmAppService.GetAsync(id);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<EdmDto>> GetListAsync(GetEdmListDto input)
    {
        return edmAppService.GetListAsync(input);
    }

    [HttpPost("send/{id}")]
    public Task SendEmailAsync(Guid id)
    {
        return edmAppService.SendEmailAsync(id);
    }

    [HttpPut("{id}")]
    public Task<EdmDto> UpdateAsync(Guid id, CreateEdmDto input)
    {
        return edmAppService.UpdateAsync(id, input);
    }
}
