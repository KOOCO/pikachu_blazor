using Asp.Versioning;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.UserAddresses;

[RemoteService(IsEnabled = true)]
[ControllerName("UserAddresses")]
[Area("app")]
[Route("api/app/user-addresses")]
public class UserAddressController(IUserAddressAppService userAddressAppService) : PikachuController, IUserAddressAppService
{
    [HttpPost]
    public Task<UserAddressDto> CreateAsync(CreateUserAddressDto input)
    {
        return userAddressAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return userAddressAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<UserAddressDto> GetAsync(Guid id)
    {
        return userAddressAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<UserAddressDto>> GetListAsync(GetUserAddressListDto input)
    {
        return userAddressAppService.GetListAsync(input);
    }

    [HttpPost("set-is-default/{id}/{isDefault}")]
    public Task<UserAddressDto> SetIsDefaultAsync(Guid id, bool isDefault)
    {
        return userAddressAppService.SetIsDefaultAsync(id, isDefault);
    }

    [HttpPut("{id}")]
    public Task<UserAddressDto> UpdateAsync(Guid id, UpdateUserAddressDto input)
    {
        return userAddressAppService.UpdateAsync(id, input);
    }
}
