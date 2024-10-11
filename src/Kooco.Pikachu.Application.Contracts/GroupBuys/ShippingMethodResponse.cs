using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuys
{
    public class ShippingMethodResponse
    {
        public Dictionary<string, List<string>> HomeDeliveryType { get; set; }
        public Dictionary<string, List<string>> ConvenienceStoreType { get; set; }
        public Dictionary<string, List<string>> SelfPickupType { get; set; }

        public ShippingMethodResponse()
        {
            HomeDeliveryType = new Dictionary<string, List<string>>();
            ConvenienceStoreType = new Dictionary<string, List<string>>();
            SelfPickupType = new Dictionary<string, List<string>>();
        }
    }
}
