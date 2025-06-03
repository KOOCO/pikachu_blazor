using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.InventoryManagement;

[RemoteService(IsEnabled = false)]
public class InventoryAppService : PikachuAppService, IInventoryAppService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryAppService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<PagedResultDto<InventoryDto>> GetListAsync(GetInventoryDto input)
    {
        var totalCount = await _inventoryRepository.CountAsync(input.Filter, input.ItemId,
            input.MinCurrentStock, input.MaxCurrentStock, input.MinAvailableStock, input.MaxAvailableStock);

        var items = await _inventoryRepository.GetListAsync(input.SkipCount, input.MaxResultCount,
            input.Sorting, input.Filter, input.ItemId, input.MinCurrentStock, input.MaxCurrentStock,
            input.MinAvailableStock, input.MaxAvailableStock);

        return new PagedResultDto<InventoryDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<InventoryModel>, List<InventoryDto>>(items)
        };
    }
}
