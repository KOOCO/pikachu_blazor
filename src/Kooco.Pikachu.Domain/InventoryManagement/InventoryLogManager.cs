using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders.Entities;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLogManager : DomainService
{
    private readonly IInventoryLogRepository _inventoryLogRepository;
    private readonly IItemDetailsRepository _itemDetailsRepository;
    private readonly ISetItemRepository _setItemRepository;
    private readonly IStringLocalizer<PikachuResource> _l;

    public InventoryLogManager(
        IInventoryLogRepository inventoryLogRepository,
        IItemDetailsRepository itemDetailsRepository,
        IStringLocalizer<PikachuResource> l,
        ISetItemRepository setItemRepository
        )
    {
        _inventoryLogRepository = inventoryLogRepository;
        _itemDetailsRepository = itemDetailsRepository;
        _l = l;
        _setItemRepository = setItemRepository;
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

    public async Task<InventoryLog> ItemSoldAsync(Order order, OrderItem orderItem, ItemDetails itemDetail, int amount, bool isAddOnProduct = false)
    {
        var attributes = InventoryLogConsts.GetAttributes(itemDetail);

        var description = $"{(isAddOnProduct ? "Add-on Product Sold" : "Item Sold")}: Order #" + order.OrderNo;

        var itemSoldLog = CreateItemSoldLog(itemDetail, amount);

        var actionType = isAddOnProduct ? InventoryActionType.AddOnProductSold : InventoryActionType.ItemSold;

        var inventoryLog = new InventoryLog(
            GuidGenerator.Create(),
            itemDetail.ItemId,
            itemDetail.Id,
            itemDetail.SKU,
            attributes,
            actionType,
            itemSoldLog.StockOnHand,
            itemSoldLog.SaleableQuantity,
            itemSoldLog.PreOrderQuantity,
            itemSoldLog.SaleablePreOrderQuantity,
            description,
            order.Id,
            order.OrderNo,
            orderItem.Id
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

    public async Task ItemUnsoldAsync(Order order)
    {
        var items = order.OrderItems;

        // Collect all direct detailIds
        var detailIds = items
            .Where(i => i.ItemDetailId.HasValue)
            .Select(i => i.ItemDetailId!.Value)
            .ToList();

        // Collect set item ids
        var setItemIds = items
            .Where(i => i.SetItemId.HasValue)
            .Select(i => i.SetItemId!.Value)
            .ToList();

        // Pre-fetch all set item details (flattened)
        var setItemDetailQ = (await _setItemRepository.GetQueryableAsync())
            .Where(si => setItemIds.Contains(si.Id))
            .SelectMany(si => si.SetItemDetails);

        // Resolve all matching ItemDetails for those set item definitions
        var setItemDetailIds = (await _itemDetailsRepository.GetQueryableAsync())
            .Where(id => setItemDetailQ.Any(sid =>
                sid.ItemId == id.ItemId &&
                sid.Attribute1Value == id.Attribute1Value &&
                sid.Attribute2Value == id.Attribute2Value &&
                sid.Attribute3Value == id.Attribute3Value))
            .Select(id => id.Id)
            .ToList();


        var setItemDetails = setItemDetailQ.ToList();

        detailIds.AddRange(setItemDetailIds);

        // Load all details in one go
        var details = await _itemDetailsRepository.GetListAsync(id => detailIds.Contains(id.Id));
        var detailsMap = details.ToDictionary(d => d.Id);

        // Load logs for this order
        var logs = await _inventoryLogRepository.GetListAsync(l => l.OrderId == order.Id);
        var logsLookup = logs.ToLookup(l => (l.OrderItemId, l.ItemDetailId));

        // Process reversals
        foreach (var item in items)
        {
            // Direct item
            if (item.ItemDetailId.HasValue)
            {
                detailsMap.TryGetValue(item.ItemDetailId.Value, out var detail);

                if (detail != null)
                {
                    foreach (var originalLog in logsLookup[(item.Id, detail.Id)])
                    {
                        await ReverseLogAsync(order, item, detail, originalLog);
                    }
                }
            }

            // Set item
            if (item.SetItemId.HasValue)
            {
                var thisSetItemDetails = setItemDetails.Where(sid => sid.SetItemId == item.SetItemId);

                foreach (var sid in thisSetItemDetails)
                {
                    var detail = details.FirstOrDefault(d =>
                        d.ItemId == sid.ItemId &&
                        d.Attribute1Value == sid.Attribute1Value &&
                        d.Attribute2Value == sid.Attribute2Value &&
                        d.Attribute3Value == sid.Attribute3Value);

                    if (detail != null)
                    {
                        foreach (var originalLog in logsLookup[(item.Id, detail.Id)])
                        {
                            await ReverseLogAsync(order, item, detail, originalLog);
                        }
                    }
                }
            }
        }
    }

    private async Task ReverseLogAsync(Order order, OrderItem item, ItemDetails detail, InventoryLog originalLog)
    {
        var attributes = InventoryLogConsts.GetAttributes(detail);

        var actionType = item.IsAddOnProduct
            ? InventoryActionType.AddOnProductUnsold
            : InventoryActionType.ItemUnsold;

        var description = $"{(item.IsAddOnProduct ? "Add-on Product Unsold" : "Item Unsold")}: Order #{order.OrderNo}";

        var inventoryLog = new InventoryLog(
            GuidGenerator.Create(),
            detail.ItemId,
            detail.Id,
            detail.SKU,
            attributes,
            actionType,
            -originalLog.StockOnHand,
            -originalLog.SaleableQuantity,
            -originalLog.PreOrderQuantity,
            -originalLog.SaleablePreOrderQuantity,
            description,
            order.Id,
            order.OrderNo,
            item.Id
        );

        await _inventoryLogRepository.InsertAsync(inventoryLog);
        await AdjustItemDetailStockAsync(inventoryLog);
    }

    private record ItemSoldLog(int SaleableQuantity, int StockOnHand, int SaleablePreOrderQuantity, int PreOrderQuantity);
}
