using Kooco.Pikachu.Items;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLogManager : DomainService
{
    private readonly IInventoryLogRepository _inventoryLogRepository;
    private readonly IItemDetailsRepository _itemDetailsRepository;

    public InventoryLogManager(
        IInventoryLogRepository inventoryLogRepository,
        IItemDetailsRepository itemDetailsRepository
        )
    {
        _inventoryLogRepository = inventoryLogRepository;
        _itemDetailsRepository = itemDetailsRepository;
    }

    public async Task<InventoryLog> CreateAsync(
        Guid itemId,
        Guid itemDetailId,
        string sku,
        string attributes,
        InventoryStockType stockType,
        InventoryActionType actionType,
        int amount,
        string? description,
        Guid? orderId = null,
        string? orderNumber = null
        )
    {
        Check.NotDefaultOrNull<Guid>(itemId, nameof(itemId));
        Check.NotDefaultOrNull<Guid>(itemDetailId, nameof(itemDetailId));
        Check.NotNullOrWhiteSpace(sku, nameof(sku), InventoryLogConsts.MaxSkuLength);
        Check.NotNullOrWhiteSpace(attributes, nameof(attributes), InventoryLogConsts.MaxAttributesLength);
        Check.NotNullOrWhiteSpace(description, nameof(description), InventoryLogConsts.MaxDescriptionLength);

        var inventoryLog = new InventoryLog(
            GuidGenerator.Create(),
            itemId,
            itemDetailId,
            sku,
            attributes,
            stockType,
            actionType,
            amount,
            description,
            orderId,
            orderNumber
            );

        await _inventoryLogRepository.InsertAsync(inventoryLog);

        await AdjustItemDetailStockAsync(inventoryLog);

        return inventoryLog;
    }

    public async Task AdjustItemDetailStockAsync(InventoryLog inventoryLog)
    {
        Check.NotNull(inventoryLog, nameof(InventoryLog));

        var itemDetail = await _itemDetailsRepository.GetAsync(inventoryLog.ItemDetailId);

        var signedAmount = inventoryLog.ActionType == InventoryActionType.AddStock
            ? inventoryLog.Amount
            : inventoryLog.Amount * -1;

        switch (inventoryLog.StockType)
        {
            case InventoryStockType.CurrentStock:
                itemDetail.StockOnHand += signedAmount;
                break;
            case InventoryStockType.AvailableStock:
                itemDetail.SaleableQuantity += signedAmount;
                break;
            case InventoryStockType.PreOrderQuantity:
                itemDetail.PreOrderableQuantity += signedAmount;
                break;
            case InventoryStockType.AvailablePreOrderQuantity:
                itemDetail.SaleablePreOrderQuantity += signedAmount;
                break;
            default:
                throw new InvalidOperationException($"Unsupported StockType: {inventoryLog.StockType}");
        }

        await _itemDetailsRepository.UpdateAsync(itemDetail);
    }
}
