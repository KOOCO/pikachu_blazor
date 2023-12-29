using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.EnumValues;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.AutomaticEmails;

[RemoteService(IsEnabled = true)]
[ControllerName("AutomaticEmails")]
[Area("app")]
[Route("api/app/automatic-emails")]
public class AutomaticEmailController(
    IAutomaticEmailAppService _automaticEmailAppService
        ) : AbpController, IAutomaticEmailAppService
{
    [HttpPost]
    public Task CreateAsync(AutomaticEmailCreateUpdateDto input)
    {
        return _automaticEmailAppService.CreateAsync(input);
    }

    [HttpGet("{id}")]
    public Task<AutomaticEmailDto> GetAsync(Guid id)
    {
        return _automaticEmailAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<AutomaticEmailDto>> GetListAsync(GetAutomaticEmailListDto input)
    {
        return _automaticEmailAppService.GetListAsync(input);
    }

    [HttpGet("get-with-details/{id}")]
    public Task<AutomaticEmailDto> GetWithDetailsByIdAsync(Guid id)
    {
        return _automaticEmailAppService.GetWithDetailsByIdAsync(id);
    }

    [HttpPut("{id}")]
    public Task UpdateAsync(Guid id, AutomaticEmailCreateUpdateDto input)
    {
        return _automaticEmailAppService.UpdateAsync(id, input);
    }

    [HttpPut("update-job-status/{id}/{status}/{tenantId}")]
    public Task UpdateJobStatusAsync(Guid id, JobStatus status, Guid? tenantId)
    {
        return _automaticEmailAppService.UpdateJobStatusAsync(id, status, tenantId);
    }
}
