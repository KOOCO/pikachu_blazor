using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.UserAddresses;

public interface IUserAddressAppService : IApplicationService
{
    Task<UserAddressDto> CreateAsync(CreateUserAddressDto input);
    Task<UserAddressDto> UpdateAsync(Guid id, UpdateUserAddressDto input);
    Task<UserAddressDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<UserAddressDto>> GetListAsync(GetUserAddressListDto input);
    Task<UserAddressDto> SetIsDefaultAsync(Guid id, bool isDefault);
}
