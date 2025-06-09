using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryAppService : IApplicationService
{
    Task<InventoryDto> GetAsync(Guid itemId, Guid itemDetailId);
    Task<PagedResultDto<InventoryDto>> GetListAsync(GetInventoryDto input);
    Task<List<string>> GetWarehouseLookupAsync();
    Task<List<string>> GetSkuLookupAsync();
    Task<List<string>> GetAttributesLookupAsync();
    Task<IRemoteStreamContent> GetListAsExcelAsync(List<InventoryDto>? items = null);
}
