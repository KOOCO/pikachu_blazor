using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.OrderItems;

public class CreateUpdateOrderItemDto
{
    public Guid Id { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public Guid? FreebieId { get; set; }
    public ItemType ItemType { get; set; }
    public Guid OrderId { get; set; }
    public string? Spec { get; set; }
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SKU { get; set; }
    public ItemStorageTemperature DeliveryTemperature { get; set; }
}
