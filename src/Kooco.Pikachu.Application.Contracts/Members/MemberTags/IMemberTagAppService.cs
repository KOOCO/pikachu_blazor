using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Members.MemberTags;

public interface IMemberTagAppService : IApplicationService
{
    Task AddTagForUsersAsync(AddTagForUsersDto input);
    Task<PagedResultDto<MemberTagDto>> GetListAsync(GetMemberTagsListDto input);
    Task DeleteManyAsync(List<string> tagsList);
    Task SetIsEnabledAsync(string name, bool isEnabled);
}
