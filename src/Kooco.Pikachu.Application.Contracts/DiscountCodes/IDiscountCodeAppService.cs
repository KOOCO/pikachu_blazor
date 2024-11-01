using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.DiscountCodes
{
    public interface IDiscountCodeAppService : IApplicationService
    {
        Task<PagedResultDto<DiscountCodeDto>> GetListAsync(GetDiscountCodeListDto input);
        Task<DiscountCodeDto> CreateAsync(CreateUpdateDiscountCodeDto input);
        Task<DiscountCodeDto> GetAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task<DiscountCodeDto> UpdateAsync(Guid id, CreateUpdateDiscountCodeDto input);
        Task<List<ItemDto>> GetProductsAsync(Guid id);
        Task<List<GroupBuyDto>> GetGroupBuysAsync(Guid id);
        Task UpdateStatusAsync(Guid id);
        Task<DiscountCheckOutputDto> CheckDiscountCodeAsync(DiscountCheckInputDto input);
    }
}
