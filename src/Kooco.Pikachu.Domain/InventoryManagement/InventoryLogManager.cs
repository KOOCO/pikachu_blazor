using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders.Entities;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
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

        //Available Stock can not be greater than Current Stock
        if (itemDetail.SaleableQuantity > itemDetail.StockOnHand)
        {
            throw new InvalidInventoryStockException(_l["AvailableStock"], _l["CurrentStock"]);
        }

        //Available Pre Order Quantity can not be greater than Pre Order Quantity
        if (itemDetail.SaleablePreOrderQuantity > itemDetail.PreOrderableQuantity)
        {
            throw new InvalidInventoryStockException(_l["AvailablePreOrderQuantity"], _l["PreOrderQuantity"]);
        }

        await _itemDetailsRepository.UpdateAsync(itemDetail);
    }

    public async Task ItemSoldAsync(Order order, ItemDetails itemDetail, int amount)
    {
        var attributes = GetAttributes(itemDetail);
        var description = "Item Sold: Order #" + order.OrderNo;

        await CreateAsync(
            itemDetail.ItemId,
            itemDetail.Id,
            itemDetail.SKU,
            attributes,
            InventoryStockType.CurrentStock,
            InventoryActionType.ItemSold,
            amount,
            description,
            order.Id,
            order.OrderNo
            );

        await CreateAsync(
            itemDetail.ItemId,
            itemDetail.Id,
            itemDetail.SKU,
            attributes,
            InventoryStockType.AvailableStock,
            InventoryActionType.ItemSold,
            amount,
            description,
            order.Id,
            order.OrderNo
            );
    }

    static string GetAttributes(ItemDetails itemDetail)
    {
        return
            string.Join(" / ", new[]
            {
                itemDetail.Attribute1Value,
                itemDetail.Attribute2Value,
                itemDetail.Attribute3Value
            }.Where(attr => !string.IsNullOrEmpty(attr)))
            ?? "";
    }
}
