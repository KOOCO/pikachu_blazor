using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單項目
/// </summary>
public class OrderItem : Entity<Guid>
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    public Guid? ItemId { get; set; }

    /// <summary>
    /// 商品導覽屬性
    /// </summary>
    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    /// <summary>
    /// 組合商品識別碼
    /// </summary>
    public Guid? SetItemId { get; set; }

    /// <summary>
    /// 組合商品導覽屬性
    /// </summary>
    [ForeignKey(nameof(SetItemId))]
    public SetItem? SetItem { get; set; }

    /// <summary>
    /// 贈品識別碼
    /// </summary>
    public Guid? FreebieId { get; set; }

    /// <summary>
    /// 贈品導覽屬性
    /// </summary>
    [ForeignKey(nameof(FreebieId))]
    public Freebie? Freebie { get; set; }

    /// <summary>
    /// 項目類型
    /// </summary>
    public ItemType ItemType { get; set; }

    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 規格
    /// </summary>
    public string? Spec { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 商品單價
    /// </summary>
    public decimal ItemPrice { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 庫存單位
    /// </summary>
    public string? SKU { get; set; }

    /// <summary>
    /// 配送溫層
    /// </summary>
    public ItemStorageTemperature DeliveryTemperature { get; set; }

    /// <summary>
    /// 配送溫層費用
    /// </summary>
    public decimal DeliveryTemperatureCost { get; set; }

    /// <summary>
    /// 折扣百分比
    /// </summary>
    public int? DiscountPercentage { get; set; }

    /// <summary>
    /// 配送訂單識別碼
    /// </summary>
    public Guid? DeliveryOrderId { get; set; }

    public bool IsAddOnProduct { get; set; }

    public OrderItem() { }
    public OrderItem(
        Guid id,
        Guid? itemId,
        Guid? setItemId,
        Guid? freebieId,
        ItemType itemType,
        Guid orderId,
        string? spec,
        decimal itemPrice,
        decimal totalAmount,
        int quantity,
        string? sku,
        ItemStorageTemperature itemStorageTemperature,
        decimal temperatureCost,
        int? discountPercentage = null,
        Guid? deliveryOrderId = null,
        bool isAddOnProduct = false
        ) : base(id)
    {
        ItemId = itemId;
        SetItemId = setItemId;
        FreebieId = freebieId;
        ItemType = itemType;
        OrderId = orderId;
        Spec = spec;
        ItemPrice = itemPrice;
        TotalAmount = totalAmount;
        Quantity = quantity;
        SKU = sku;
        DiscountPercentage = discountPercentage;
        DeliveryTemperature = itemStorageTemperature;
        DeliveryTemperatureCost = temperatureCost;
        DeliveryOrderId = deliveryOrderId;
        IsAddOnProduct = isAddOnProduct;
    }
}