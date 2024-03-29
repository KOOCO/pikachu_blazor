﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.OrderItems
{
    public class OrderItem : Entity<Guid>
    {
        public Guid? ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
        public Guid? SetItemId { get; set; }
        [ForeignKey(nameof(SetItemId))]
        public SetItem? SetItem { get; set; }
        public Guid? FreebieId { get; set; }
        [ForeignKey(nameof(FreebieId))]
        public Freebie? Freebie { get; set; }
        public ItemType ItemType { get; set; }
        public Guid OrderId { get; set; }
        public string? Spec { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SKU { get; set; }
        public ItemStorageTemperature DeliveryTemperature { get; set; }
        public decimal DeliveryTemperatureCost { get; set; }
        public int? DiscountPercentage { get; set; }
        public Guid? DeliveryOrderId { get; set; }
        public OrderItem()
        {

        }
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
            int? discountPercentage=null,
            Guid? deliveryOrderId=null
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
        }
    }
}
