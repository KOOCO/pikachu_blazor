using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    public class CreateLogisticsOrder
    {

        public string Address { get; set; }
        public string StoreAddress { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public Guid DeliveryId { get; set; }
        public Guid OrderId { get; set; }

    }
}
