using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders.Entities;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLogManager : DomainService
{
    private readonly IInventoryLogRepository _inventoryLogRepository;
    private readonly IItemDetailsRepository _itemDetailsRepository;
    private readonly IStringLocalizer<PikachuResource> _l;

    public InventoryLogManager(
        IInventoryLogRepository inventoryLogRepository,
        IItemDetailsRepository itemDetailsRepository,
        IStringLocalizer<PikachuResource> l
        )
    {
        _inventoryLogRepository = inventoryLogRepository;
        _itemDetailsRepository = itemDetailsRepository;
        _l = l;
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

        if (actionType != InventoryActionType.AddStock)
        {
            amount *= -1;
        }

        int stockOnHand = 0;
        int saleableQuantity = 0;
        int preOrderQuantity = 0;
        int saleablePreOrderQuantity = 0;

        switch (stockType)
        {
            case InventoryStockType.CurrentStock:
                stockOnHand = amount;
                break;
            case InventoryStockType.AvailableStock:
                saleableQuantity = amount;
                break;
            case InventoryStockType.PreOrderQuantity:
                preOrderQuantity = amount;
                break;
            case InventoryStockType.AvailablePreOrderQuantity:
                saleablePreOrderQuantity = amount;
                break;
            default:
                throw new InvalidOperationException($"Unsupported StockType: {stockType}");
        }

        var inventoryLog = new InventoryLog(
            GuidGenerator.Create(),
            itemId,
            itemDetailId,
            sku,
            attributes,
            actionType,
            stockOnHand,
            saleableQuantity,
            preOrderQuantity,
            saleablePreOrderQuantity,
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

        itemDetail.StockOnHand += inventoryLog.StockOnHand;
        itemDetail.SaleableQuantity += inventoryLog.SaleableQuantity;
        itemDetail.PreOrderableQuantity += inventoryLog.PreOrderQuantity;
        itemDetail.SaleablePreOrderQuantity += inventoryLog.SaleablePreOrderQuantity;

        // Available Stock can not be greater than Current Stock
        if (itemDetail.SaleableQuantity > itemDetail.StockOnHand)
        {
            throw new InvalidInventoryStockException(_l["AvailableStock"], _l["CurrentStock"]);
        }

        // Available Pre Order Quantity can not be greater than Pre Order Quantity
        if (itemDetail.SaleablePreOrderQuantity > itemDetail.PreOrderableQuantity)
        {
            throw new InvalidInventoryStockException(_l["AvailablePreOrderQuantity"], _l["PreOrderQuantity"]);
        }

        await _itemDetailsRepository.UpdateAsync(itemDetail);
    }

    public async Task<InventoryLog> ItemSoldAsync(Order order, ItemDetails itemDetail, int amount)
    {
        var attributes = InventoryLogConsts.GetAttributes(
            itemDetail.Attribute1Value,
            itemDetail.Attribute2Value,
            itemDetail.Attribute3Value
            );

        var description = "Item Sold: Order #" + order.OrderNo;

        var itemSoldLog = CreateItemSoldLog(itemDetail, amount);

        var inventoryLog = new InventoryLog(
            GuidGenerator.Create(),
            itemDetail.ItemId,
            itemDetail.Id,
            itemDetail.SKU,
            attributes,
            InventoryActionType.ItemSold,
            itemSoldLog.StockOnHand,
            itemSoldLog.SaleableQuantity,
            itemSoldLog.PreOrderQuantity,
            itemSoldLog.SaleablePreOrderQuantity,
            description,
            order.Id,
            order.OrderNo
            );

        await _inventoryLogRepository.InsertAsync(inventoryLog);

        await AdjustItemDetailStockAsync(inventoryLog);

        return inventoryLog;
    }

    private static ItemSoldLog CreateItemSoldLog(ItemDetails itemDetail, int amount)
    {
        int saleableQuantity = (int)(itemDetail.SaleableQuantity ?? 0);
        int stockOnHand = itemDetail.StockOnHand ?? 0;
        int saleablePreOrderQuantity = (int)(itemDetail.SaleablePreOrderQuantity ?? 0);
        int preOrderableQuantity = (int)(itemDetail.PreOrderableQuantity ?? 0);

        bool enoughAvailable = saleableQuantity >= 0
            ? (saleableQuantity + saleablePreOrderQuantity) >= amount
            : saleablePreOrderQuantity >= amount;

        bool enoughStock = stockOnHand >= 0
            ? (stockOnHand + preOrderableQuantity) >= amount
            : preOrderableQuantity >= amount;

        if (!enoughAvailable || !enoughStock)
        {
            throw new UserFriendlyException("Insufficient stock for " + itemDetail.SKU);
        }

        int saleableQuantityLog = -amount;
        int stockOnHandLog = -amount;
        int saleablePreOrderQuantityLog = 0;
        int preOrderQuantityLog = 0;

        if (amount > saleableQuantity)
        {
            saleablePreOrderQuantityLog = saleableQuantity > 0 ? saleableQuantity - amount : -amount;
        }
        if (amount > stockOnHand)
        {
            preOrderQuantityLog = stockOnHand > 0 ? stockOnHand - amount : -amount;
        }

        return new ItemSoldLog(saleableQuantityLog, stockOnHandLog, saleablePreOrderQuantityLog, preOrderQuantityLog);
    }

    private record ItemSoldLog(int SaleableQuantity, int StockOnHand, int SaleablePreOrderQuantity, int PreOrderQuantity);
}
