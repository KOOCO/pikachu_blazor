using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryLogAppService : IApplicationService
{
    Task<InventoryLogDto> CreateAsync(CreateInventoryLogDto input);
    Task<List<InventoryLogDto>> GetListAsync(Guid itemId, Guid itemDetailId);
    Task<IRemoteStreamContent> GetListAsExcelAsync(List<InventoryLogDto> input);
}
