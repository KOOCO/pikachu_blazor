using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.EnumValues;

public enum ShippingStatus
{
    WaitingForPayment,
    PrepareShipment,
    ToBeShipped,
    Shipped,
    Delivered,
    PickedUp,
    EnterpricePurchase,
    Return,
    Closed,
    Exchange,
    Completed
}
