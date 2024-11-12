using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Kooco.Pikachu.Response;

public class Response
{
}

public static class RecordEcPay
{
    public static Dictionary<Guid, EcPayStoreData> UniqueEcPayData { get; set; }
    public static Dictionary<Guid, TCatStoreData> UniqueTCatStoreData { get; set; }
}

public class EcPayStoreData
{
    public string CVSStoreID { get; set; }
    public string CVSAddress { get; set; }
    public string CVSOutSide { get; set; }
    public string LogisticsSubType { get; set; }
    public string CVSStoreName { get; set; }
    public string CVSTelephone { get; set; }
    public string MerchantID { get; set; }
    public string MerchantTradeNo { get; set; }
    public string UniqueId { get; set; }
    public string ExtraData { get; set; }
}

public class TCatStoreData
{
    public string outside { get; set; }
    public string ship { get; set; }
    public string storeaddress { get; set; }
    public string storeid { get; set; }
    public string storename { get; set; }
    public string ExtraData { get; set; }
    public string UniqueId { get; set; }
}

public class AttributeNameOption
{
    public string AttributeName { get; set; }
    public List<string> AttributeOptions { get; set; }
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
    public int CollectionAmount { get; set; }
    public string IsSwipe { get; set; }
    public string IsMobilePay { get; set; }
    public string IsDeclare { get; set; }
    public int DeclareAmount { get; set; }
    public string ProductTypeId { get; set; }
    public string ProductName { get; set; }
    public string Memo { get; set; }
}

public class PrintObtResponse
{
    public string SrvTranId { get; set; }
    public string IsOK { get; set; }
    public string Message { get; set; }
    public PrintObtData? Data { get; set; }
}

public class PrintObtData
{
    public string PrintDateTime { get; set; }
    public List<PrintObtOrders> Orders { get; set; } 
    public string FileNo { get; set; }
}

public class PrintObtOrders
{
    public string OBTNumber { get; set; }
    public string OrderId { get; set; }
}

public class PrintOBTB2SRequest
{
    public string CustomerId { get; set; }
    public string CustomerToken { get; set; }
    public string PrintType { get; set; }
    public string PrintOBTType { get; set; }
    public OrderOBTB2S[] Orders { get; set; }
}

public class OrderOBTB2S
{
    public string OBTNumber { get; set; }
    public string OrderId { get; set; }
    public string Thermosphere { get; set; }
    public string Spec { get; set; }
    public string ReceiveStoreId { get; set; }
    public string RecipientName { get; set; }
    public string RecipientTel { get; set; }
    public string RecipientMobile { get; set; }
    public string SenderName { get; set; }
    public string SenderTel { get; set; }
    public string SenderMobile { get; set; }
    public string SenderZipCode { get; set; }
    public string SenderAddress { get; set; }
    public string IsCollection { get; set; }
    public int CollectionAmount { get; set; }
    public string Memo { get; set; }
}

public class PrintOBTB2SResponse
{
    public string SrvTranId { get; set; }
    public string IsOK { get; set; }
    public string Message { get; set; }
    public PrintOBTB2SData? Data { get; set; }
}

public class PrintOBTB2SData
{
    public string PrintDateTime { get; set; }
    public List<PrintOBTB2SOrders> Orders { get; set; }
    public string FileNo { get; set; }
}

public class PrintOBTB2SOrders
{
    public string OBTNumber { get; set; }
    public string OrderId { get; set; }
    public string DeliveryId { get; set; }
}