using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Content;

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

    public async Task<IRemoteStreamContent> GetListAsExcelAsync(List<InventoryLogDto> input)
    {
        var headers = new Dictionary<string, string>
        {
            { "SKU", L["SKU"] },
            { "Timestamp", L["Timestamp"] },
            { "Action", L["Action"] },
            { "CurrentStock", L["CurrentStock"] },
            { "AvailableStock", L["AvailableStock"] },
            { "PreOrderQuantity", L["PreOrderQuantity"] },
            { "AvailablePreOrderQuantity", L["AvailablePreOrderQuantity"] }
        };

        var excelData = input.Select(log => new Dictionary<string, object>
        {
            { headers["SKU"], log.Sku },
            { headers["Timestamp"], log.CreationTime.ToString("MM/dd/yyyy HH:mm:ss") },
            { headers["Action"], Action(log) },
            { headers["CurrentStock"], Amount(log.StockOnHand) },
            { headers["AvailableStock"], Amount(log.SaleableQuantity) },
            { headers["PreOrderQuantity"], Amount(log.PreOrderQuantity) },
            { headers["AvailablePreOrderQuantity"], Amount(log.SaleablePreOrderQuantity) }
        });

        if (!excelData.Any())
        {
            excelData =
            [
                headers.ToDictionary(k => k.Value, k => (object)string.Empty)
            ];
        }

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelData);
        memoryStream.Seek(0, SeekOrigin.Begin);

        string fileName = L["InventoryLogs"] + $" - {DateTime.Now:yyyy-MM-dd HH:mm:ss}.xlsx";
        return new RemoteStreamContent(memoryStream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    static string Amount(int value)
    {
        return InventoryLogConsts.GetAmountString(value);
    }

    private string Action(InventoryLogDto value)
    {
        return value.ActionType switch
        {
            InventoryActionType.AddStock => L["StockAdded"],
            InventoryActionType.DeductStock => L["StockDeducted"],
            InventoryActionType.ItemSold => L["ItemSold", value.OrderNumber],
            _ => string.Empty,
        };
    }
}
