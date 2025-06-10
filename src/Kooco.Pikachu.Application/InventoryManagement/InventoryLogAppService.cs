using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.InventoryManagement;

[RemoteService(IsEnabled = false)]
public class InventoryLogAppService : PikachuAppService, IInventoryLogAppService
{
    private readonly InventoryLogManager _inventoryLogManager;
    private readonly IInventoryLogRepository _inventoryLogRepository;

    public InventoryLogAppService(
        InventoryLogManager inventoryLogManager,
        IInventoryLogRepository inventoryLogRepository
        )
    {
        _inventoryLogManager = inventoryLogManager;
        _inventoryLogRepository = inventoryLogRepository;
    }

    public async Task<InventoryLogDto> CreateAsync(CreateInventoryLogDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNull(input.StockType, nameof(input.StockType));
        Check.NotNull(input.ActionType, nameof(input.ActionType));
        Check.NotNull(input.Amount, nameof(input.Amount));

        var inventoryLog = await _inventoryLogManager
            .CreateAsync(
                input.ItemId,
                input.ItemDetailId,
                input.Sku,
                input.Attributes,
                input.StockType.Value,
                input.ActionType.Value,
                input.Amount.Value,
                input.Description
                );

        return ObjectMapper.Map<InventoryLog, InventoryLogDto>(inventoryLog);
    }

    public async Task<List<InventoryLogDto>> GetListAsync(Guid itemId, Guid itemDetailId)
    {
        var inventoryLogs = await _inventoryLogRepository.GetListAsync(itemId, itemDetailId);
        return ObjectMapper.Map<List<InventoryLog>, List<InventoryLogDto>>(inventoryLogs);
    }
}
