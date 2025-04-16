using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.Orders;

public class OrderConsts
{
    public static readonly List<ShippingStatus> CompletedShippingStatus = [ShippingStatus.Completed, ShippingStatus.Closed];
}
