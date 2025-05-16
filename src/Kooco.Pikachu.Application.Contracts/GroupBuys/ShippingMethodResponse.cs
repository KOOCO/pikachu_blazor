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
    public List<DeliveredByStoreTypeResponse> DeliveredByStoreType { get; set; }

    public ShippingMethodResponse()
    {
        HomeDeliveryType = new Dictionary<string, List<string>>();
        ConvenienceStoreType = new Dictionary<string, List<string>>();
        SelfPickupType = new Dictionary<string, List<string>>();
        DeliveredByStoreType = new List<DeliveredByStoreTypeResponse>();
    }
}

public class DeliveredByStoreTypeResponse
{
    public string? Type { get; set; }               // Previously the key in the dictionary
    public string? DeliveryMethod { get; set; }
    public List<string> DeliveryTime { get; set; } = new();
    public int DeliveryType { get; set; }
}
