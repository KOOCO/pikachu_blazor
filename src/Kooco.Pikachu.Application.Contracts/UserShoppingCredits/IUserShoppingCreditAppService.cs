using Kooco.Pikachu.ShoppingCredits;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.UserShoppingCredits;

public interface IUserShoppingCreditAppService : IApplicationService
{
    Task<UserShoppingCreditDto> CreateAsync(CreateUserShoppingCreditDto input);
    Task<UserShoppingCreditDto> UpdateAsync(Guid id, UpdateUserShoppingCreditDto input);
    Task DeleteAsync(Guid id);
    Task<UserShoppingCreditDto> GetAsync(Guid id);
    Task<PagedResultDto<UserShoppingCreditDto>> GetListAsync(GetUserShoppingCreditListDto input);
    Task<UserShoppingCreditDto> SetIsActiveAsync(Guid id, bool isActive);
    Task<UserShoppingCreditDto> RecordShoppingCreditAsync(RecordUserShoppingCreditDto input);
    Task<int> GetMemberCurrentShoppingCreditAsync(Guid memberId);
    Task<ShoppingCreditStatDto> GetShoppingCreditStatsAsync();

}
