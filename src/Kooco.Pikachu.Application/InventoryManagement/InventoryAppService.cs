using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

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
        var totalCount = await _inventoryRepository.CountAsync(
            input.Filter,
            input.ItemId,
            input.Warehouse,
            input.Sku,
            input.Attributes,
            input.MinCurrentStock,
            input.MaxCurrentStock,
            input.MinAvailableStock,
            input.MaxAvailableStock
            );

        var items = await _inventoryRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter,
            input.ItemId,
            input.Warehouse,
            input.Sku,
            input.Attributes,
            input.MinCurrentStock,
            input.MaxCurrentStock,
            input.MinAvailableStock,
            input.MaxAvailableStock
            );

        return new PagedResultDto<InventoryDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<InventoryModel>, List<InventoryDto>>(items)
        };
    }

    public async Task<List<string>> GetWarehouseLookupAsync()
    {
        return await _inventoryRepository.GetWarehouseLookupAsync();
    }

    public async Task<List<string>> GetSkuLookupAsync()
    {
        return await _inventoryRepository.GetSkuLookupAsync();
    }

    public async Task<List<string>> GetAttributesLookupAsync()
    {
        return await _inventoryRepository.GetAttributesLookupAsync();
    }

    public async Task<IRemoteStreamContent> GetListAsExcelAsync(List<InventoryDto>? items = null)
    {
        items ??= ObjectMapper.Map<List<InventoryModel>, List<InventoryDto>>([.. await _inventoryRepository.GetFilteredQueryableAsync()]);

        var headers = new Dictionary<string, string>
        {
            { "ItemName", L["ItemName"] },
            { "Attributes", L["Attributes"] },
            { "SKU", L["SKU"] },
            { "Warehouse", L["Warehouse"] },
            { "CurrentStock", L["CurrentStock"] },
            { "AvailableStock", L["AvailableStock"] },
            { "PreOrderQuantity", L["PreOrderQuantity"] },
            { "AvailablePreOrderQuantity", L["AvailablePreOrderQuantity"] }
        };

        var excelData = items.Select(x => new Dictionary<string, object>
        {
            { headers["ItemName"], x.ItemName },
            { headers["Attributes"], x.Attributes },
            { headers["SKU"], x.Sku },
            { headers["Warehouse"], x.Warehouse },
            { headers["CurrentStock"], x.CurrentStock },
            { headers["AvailableStock"], x.AvailableStock },
            { headers["PreOrderQuantity"], x.PreOrderQuantity },
            { headers["AvailablePreOrderQuantity"], x.AvailablePreOrderQuantity }
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

        string fileName = L["Inventory"] + $" - {DateTime.Now:yyyy-MM-dd HH:mm:ss}.xlsx";
        return new RemoteStreamContent(memoryStream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}
