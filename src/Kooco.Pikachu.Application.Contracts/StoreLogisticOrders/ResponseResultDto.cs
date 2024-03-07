using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    public class ResponseResultDto
    {
        public string ResponseCode { get; set; }
        public ShippingInfoDto ShippingInfo { get; set; }
        public string ResponseMessage { get; set; }
    }
}
