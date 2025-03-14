using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Orders;

public class GroupBuyReportModelWithCount
{
    public long TotalCount { get; set; }
    public List<GroupBuyReportOrderModel> Items { get; set; }
}

public class GroupBuyReportOrderModel
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public DateTime CreationTime { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid GroupBuyId { get; set; }
    public List<GroupBuyReportOrderItemsModel> OrderItems { get; set; } = [];
}

public class GroupBuyReportOrderItemsModel
{
    public Guid Id { get; set; }
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Spec { get; set; }
    public int Quantity { get; set; }
    public ItemType ItemType { get; set; }
}