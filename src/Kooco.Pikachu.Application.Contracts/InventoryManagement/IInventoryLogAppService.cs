using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryLogAppService : IApplicationService
{
    Task<InventoryLogDto> CreateAsync(CreateInventoryLogDto input);
    Task<PagedResultDto<InventoryLogDto>> GetListAsync(GetInventoryLogListDto input);
    Task<IRemoteStreamContent> GetListAsExcelAsync(Guid itemId, Guid itemDetailId);
}
