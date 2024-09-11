using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Response;

public class Response
{
}

public class OrderPaymentMethodRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public string? MerchantTradeNo { get; set; }
}

public class PrintOBTRequest
{
    public string CustomerId { get; set; }
    public string CustomerToken { get; set; }
    public string PrintType { get; set; }
    public string PrintOBTType { get; set; }
    public OrderOBT[] Orders { get; set; }
}

public class OrderOBT
{
    public string OBTNumber { get; set; }
    public string OrderId { get; set; }
    public string Thermosphere { get; set; }
    public string Spec { get; set; }
    public string ReceiptLocation { get; set; }
    public string ReceiptStationNo { get; set; }
    public string RecipientName { get; set; }
    public string RecipientTel { get; set; }
    public string RecipientMobile { get; set; }
    public string RecipientAddress { get; set; }
    public string SenderName { get; set; }
    public string SenderTel { get; set; }
    public string SenderMobile { get; set; }
    public string SenderZipCode { get; set; }
    public string SenderAddress { get; set; }
    public string ShipmentDate { get; set; }
    public string DeliveryDate { get; set; }
    public string DeliveryTime { get; set; }
    public string IsFreight { get; set; }
    public string IsCollection { get; set; }
    public string CollectionAmount { get; set; }
    public string IsSwipe { get; set; }
    public string IsMobilePay { get; set; }
    public string IsDeclare { get; set; }
    public decimal DeclareAmount { get; set; }
    public string ProductTypeId { get; set; }
    public string ProductName { get; set; }
    public string Memo { get; set; }
}