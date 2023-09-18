using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;


namespace Kooco.Pikachu.GroupBuys
{
    public interface IGroupBuyAppService:IApplicationService
    {
        Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input);
        Task<GroupBuyDto> GetAsync(Guid id);
        Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input);
        Task DeleteAsync(Guid id);
        Task<GroupBuyDto> UpdateAsync(Guid id,GroupBuyUpdateDto input);
        Task<List<KeyValueDto>> GetGroupBuysAsync();
    }
}
