using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.UserCumulativeOrders;

public interface IUserCumulativeOrderAppService : IApplicationService
{
    Task<UserCumulativeOrderDto> CreateAsync(CreateUserCumulativeOrderDto input);
    Task<UserCumulativeOrderDto> UpdateAsync(Guid id, UpdateUserCumulativeOrderDto input);
    Task<UserCumulativeOrderDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<UserCumulativeOrderDto>> GetListAsync(GetUserCumulativeOrderListDto input);
}
