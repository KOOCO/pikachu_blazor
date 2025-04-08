using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public interface IGroupBuyItemsPriceAppService : IApplicationService
    {
        Task<GroupBuyItemsPriceDto> CreateAsync(CreateUpdateGroupBuyItemsPriceDto input);
        Task<GroupBuyItemsPriceDto> UpdateAsync(Guid id, CreateUpdateGroupBuyItemsPriceDto input);
        Task<GroupBuyItemsPriceDto> GetAsync(Guid id);
        Task<List<GroupBuyItemsPriceDto>> GetListAsync();
        Task<List<GroupBuyItemsPriceDto>> GetListByGroupBuyAsync(Guid GroupBuyId);
        Task<GroupBuyItemsPriceDto> GetByItemIdAndGroupBuyIdAsync(Guid itemDetailId, Guid GroupBuyId);
        Task<GroupBuyItemsPriceDto> GetBySetItemIdAndGroupBuyIdAsync(Guid SetItemId, Guid GroupBuyId);
        Task DeleteAsync(Guid id);
    }
}
