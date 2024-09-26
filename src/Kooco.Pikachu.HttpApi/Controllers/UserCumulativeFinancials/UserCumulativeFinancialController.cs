using Asp.Versioning;
using Kooco.Pikachu.UserCumulativeFinancials;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.UserCumulativeFinancials;

[RemoteService(IsEnabled = true)]
[ControllerName("UserCumulativeFinancials")]
[Area("app")]
[Route("api/app/user-cumulative-financials")]
public class UserCumulativeFinancialController(IUserCumulativeFinancialAppService userCumulativeFinancialAppService) : PikachuController, IUserCumulativeFinancialAppService
{
    [HttpPost]
    public Task<UserCumulativeFinancialDto> CreateAsync(CreateUserCumulativeFinancialDto input)
    {
        return userCumulativeFinancialAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return userCumulativeFinancialAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<UserCumulativeFinancialDto> GetAsync(Guid id)
    {
        return userCumulativeFinancialAppService.GetAsync(id);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<UserCumulativeFinancialDto>> GetListAsync(GetUserCumulativeFinancialListDto input)
    {
        return userCumulativeFinancialAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<UserCumulativeFinancialDto> UpdateAsync(Guid id, UpdateUserCumulativeFinancialDto input)
    {
        return userCumulativeFinancialAppService.UpdateAsync(id, input);
    }
}
