using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebies
{
    public interface IFreebieAppService : IApplicationService
    {
        Task<FreebieDto> CreateAsync(FreebieCreateDto input);
        Task<List<KeyValueDto>> GetGroupBuyLookupAsync();
        Task<List<FreebieDto>> GetListAsync();
        Task<FreebieDto> GetAsync(Guid id, bool includeDetails = false);
        Task<FreebieDto> UpdateAsync(Guid id, UpdateFreebieDto input);
        Task DeleteSingleImageAsync(Guid itemId, string blobImageName);
        Task DeleteManyItemsAsync(List<Guid> setItemIds);


    }
}
