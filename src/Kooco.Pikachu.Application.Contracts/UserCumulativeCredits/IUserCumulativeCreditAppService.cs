using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.UserCumulativeCredits;

public interface IUserCumulativeCreditAppService : IApplicationService
{
    Task<UserCumulativeCreditDto> CreateAsync(CreateUserCumulativeCreditDto input);
    Task<UserCumulativeCreditDto> CreateMemberRegisterAsync(CreateUserCumulativeCreditDto input);
    Task<UserCumulativeCreditDto> UpdateAsync(Guid id, UpdateUserCumulativeCreditDto input);
    
    Task<UserCumulativeCreditDto> GetAsync(Guid id);
    Task<PagedResultDto<UserCumulativeCreditDto>> GetListAsync(GetUserCumulativeCreditListDto input);
    Task DeleteAsync(Guid id);
}
