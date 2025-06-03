using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryAppService : IApplicationService
{
    Task<PagedResultDto<InventoryDto>> GetListAsync(GetInventoryDto input);
}
