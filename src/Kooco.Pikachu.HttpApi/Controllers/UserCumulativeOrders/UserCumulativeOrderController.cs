using Asp.Versioning;
using Kooco.Pikachu.UserCumulativeOrders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.UserCumulativeOrders;

[RemoteService(IsEnabled = true)]
[ControllerName("UserCumulativeOrders")]
[Area("app")]
[Route("api/app/user-cumulative-orders")]
public class UserCumulativeOrderController(IUserCumulativeOrderAppService userCumulativeOrderAppService) : PikachuController, IUserCumulativeOrderAppService
{
    [HttpPost]
    public Task<UserCumulativeOrderDto> CreateAsync(CreateUserCumulativeOrderDto input)
    {
        return userCumulativeOrderAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return userCumulativeOrderAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<UserCumulativeOrderDto> GetAsync(Guid id)
    {
        return userCumulativeOrderAppService.GetAsync(id);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<UserCumulativeOrderDto>> GetListAsync(GetUserCumulativeOrderListDto input)
    {
        return userCumulativeOrderAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<UserCumulativeOrderDto> UpdateAsync(Guid id, UpdateUserCumulativeOrderDto input)
    {
        return userCumulativeOrderAppService.UpdateAsync(id, input);
    }
}
