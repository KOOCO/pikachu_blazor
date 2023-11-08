using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebies
{
    public interface IFreebieAppService : ICrudAppService<
        FreebieDto,
        Guid,
        PagedAndSortedResultRequestDto,
        FreebieCreateDto, UpdateFreebieDto>
    {
        Task<List<FreebieDto>> GetListAsync();
        Task<FreebieDto> GetAsync(Guid id, bool includeDetails = false);
        Task DeleteSingleImageAsync(Guid itemId, string blobImageName);
        Task DeleteManyItemsAsync(List<Guid> setItemIds);
        Task ChangeFreebieAvailability(Guid freebieId);
    }
}
