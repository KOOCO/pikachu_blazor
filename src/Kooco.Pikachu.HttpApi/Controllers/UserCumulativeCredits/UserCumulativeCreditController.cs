using Asp.Versioning;
using Kooco.Pikachu.UserCumulativeCredits;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.UserCumulativeCredits;

[RemoteService(IsEnabled = true)]
[ControllerName("UserCumulativeCredits")]
[Area("app")]
[Route("api/app/user-cumulative-credits")]
public class UserCumulativeCreditController(IUserCumulativeCreditAppService userCumulativeCreditAppService) : PikachuController, IUserCumulativeCreditAppService
{
    [HttpPost]
    public Task<UserCumulativeCreditDto> CreateAsync(CreateUserCumulativeCreditDto input)
    {
        return userCumulativeCreditAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return userCumulativeCreditAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<UserCumulativeCreditDto> GetAsync(Guid id)
    {
        return userCumulativeCreditAppService.GetAsync(id);
    }

    [HttpGet("get-list")]
    public Task<PagedResultDto<UserCumulativeCreditDto>> GetListAsync(GetUserCumulativeCreditListDto input)
    {
        return userCumulativeCreditAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<UserCumulativeCreditDto> UpdateAsync(Guid id, UpdateUserCumulativeCreditDto input)
    {
        return userCumulativeCreditAppService.UpdateAsync(id, input);
    }
}
