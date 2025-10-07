using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Items
{
    public class ItemWithItemType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public ItemStorageTemperature? Temperature { get; set; }
    }

    public class ItemDetailWithItemType
    {
        public Guid Id { get; set; }
        public Guid? ItemDetailId { get; set; }
        public ItemType ItemType { get; set; }
        public string Sku { get; set; }
        public List<string> Attributes { get; set; }
        public SetItemPricing Pricing { get; set; } = new();
    }

    public class SetItemPricing
    {
        public float Price { get; set; }
        public float? Available { get; set; }
        public float AvailableQuantity => Available ?? 0;
    }
}
