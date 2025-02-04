using Asp.Versioning;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.UserShoppingCredits;

[RemoteService(IsEnabled = true)]
[ControllerName("UserShoppingCredits")]
[Area("app")]
[Route("api/app/user-shopping-credits")]
public class UserShoppingCreditController(IUserShoppingCreditAppService userShoppingCreditAppService) : PikachuController, IUserShoppingCreditAppService
{
    [HttpPost]
    public Task<UserShoppingCreditDto> CreateAsync(CreateUserShoppingCreditDto input)
    {
        return userShoppingCreditAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return userShoppingCreditAppService.DeleteAsync(id);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public Task<int> ExpireCreditsAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public Task<UserShoppingCreditDto> GetAsync(Guid id)
    {
        return userShoppingCreditAppService.GetAsync(id);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<UserShoppingCreditDto>> GetListAsync(GetUserShoppingCreditListDto input)
    {
        return userShoppingCreditAppService.GetListAsync(input);
    }

    [HttpGet("member-current-shopping-credit/{memberId}")]
    public Task<int> GetMemberCurrentShoppingCreditAsync(Guid memberId)
    {
        return userShoppingCreditAppService.GetMemberCurrentShoppingCreditAsync(memberId);
    }

    [HttpGet("shopping-credit-stats")]
    public Task<ShoppingCreditStatDto> GetShoppingCreditStatsAsync()
    {
        return userShoppingCreditAppService.GetShoppingCreditStatsAsync();
    }

    [HttpPost("record-shopping-credit")]
    public Task<UserShoppingCreditDto> RecordShoppingCreditAsync(RecordUserShoppingCreditDto input)
    {
        return userShoppingCreditAppService.RecordShoppingCreditAsync(input);
    }

    [HttpPost("set-active/{id}/{isActive}")]
    public Task<UserShoppingCreditDto> SetIsActiveAsync(Guid id, bool isActive)
    {
        return userShoppingCreditAppService.SetIsActiveAsync(id, isActive);
    }

    [HttpPut("{id}")]
    public Task<UserShoppingCreditDto> UpdateAsync(Guid id, UpdateUserShoppingCreditDto input)
    {
        return userShoppingCreditAppService.UpdateAsync(id, input);
    }
}
