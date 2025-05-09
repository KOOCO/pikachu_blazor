using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.Items;

public class ItemWithDetailsModel
{
    public Guid Id { get; set; }
    public ItemType ItemType { get; set; }
    public string? ItemName { get; set; }
    public string? Image { get; set; }
    public int? Stock { get; set; }
    public int UnitPrice { get; set; }
    public IEnumerable<CartItemDetailsModel> Details { get; set; }
}

public class CartItemDetailsModel
{
    public Guid Id { get; set; }
    public string? Name => string.Join("/", new[] { Attribute1Value, Attribute2Value, Attribute3Value }
                    .Where(v => !string.IsNullOrWhiteSpace(v)));
    public int? Stock { get; set; }
    public int UnitPrice { get; set; }
    public string? Attribute1Value { get; set; }
    public string? Attribute2Value { get; set; }
    public string? Attribute3Value { get; set; }
}
