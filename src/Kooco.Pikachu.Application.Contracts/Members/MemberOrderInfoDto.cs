using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.Members;

public class MemberOrderInfoDto
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public DateTime CreationTime { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public double TotalAmount { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
}
