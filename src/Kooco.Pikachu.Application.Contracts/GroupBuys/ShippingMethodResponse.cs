using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuys;

public class ShippingMethodResponse
{
    public Dictionary<string, List<string>> HomeDeliveryType { get; set; }
    public Dictionary<string, List<string>> ConvenienceStoreType { get; set; }
    public Dictionary<string, List<string>> SelfPickupType { get; set; }
    public Dictionary<string, DeliveredByStoreTypeResponse> DeliveredByStoreType { get; set; }

    public ShippingMethodResponse()
    {
        HomeDeliveryType = new Dictionary<string, List<string>>();
        ConvenienceStoreType = new Dictionary<string, List<string>>();
        SelfPickupType = new Dictionary<string, List<string>>();
        DeliveredByStoreType = [];
    }
}

public class DeliveredByStoreTypeResponse
{
    public string? DeliveryMethod { get; set; }
    public List<string> DeliveryTime { get; set; }
    public int DeliveryType { get; set; }
} 