using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Identity;

public interface IMyIdentityUserAppService : IIdentityUserAppService
{
    Task<PagedResultDto<IdentityUserDto>> GetCategorizedListAsync(GetCategorizedListDto input);
    Task<List<KeyValueDto>> GetCategorizedLookupAsync(UserTypes? userTypes);
}
