using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public interface IUserCumulativeFinancialAppService : IApplicationService
{
    Task<UserCumulativeFinancialDto> CreateAsync(CreateUserCumulativeFinancialDto input);
    Task<UserCumulativeFinancialDto> UpdateAsync(Guid id, UpdateUserCumulativeFinancialDto input);
    Task<UserCumulativeFinancialDto> GetAsync(Guid id);
    Task<PagedResultDto<UserCumulativeFinancialDto>> GetListAsync(GetUserCumulativeFinancialListDto input);
    Task DeleteAsync(Guid id);
}
