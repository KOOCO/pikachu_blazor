using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    public class ShippingInfoDto
    {
        public string AllPayLogisticsID { get; set; }
        public string BookingNote { get; set; }
        public string CheckMacValue { get; set; }
        public string CVSPaymentNo { get; set; }
        public string CVSValidationNo { get; set; }
        public string GoodsAmount { get; set; }
        public string LogisticsSubType { get; set; }
        public string LogisticsType { get; set; }
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverCellPhone { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string UpdateStatusDate { get; set; }
        public string ShipmentNo { get; set; }
    }
}
