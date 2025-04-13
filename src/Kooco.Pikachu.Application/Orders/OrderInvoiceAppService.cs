using Kooco.Invoices;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders;
public class OrderInvoiceAppService : PikachuAppService, IOrderInvoiceAppService
{
    public async Task<string> CreateInvoiceAsync(Guid orderId)
    {
        if (await OrderInvoiceRepository.HasNonVoidedInvoiceAsync(orderId))
        {
            throw new InvalidOperationException("訂單已有未作廢發票，不能重複建立");
        }

        var setting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);

        var order = (await OrderRepository.GetQueryableAsync())
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Item)
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Freebie)
            .First(x => x.Id == orderId);

        //foreach (var orderItem in order.OrderItems)
        //{
        //    var taxTypeId = orderItem.Item.TaxTypeId;

        //    invoiceItems.Add(new()
        //    {
        //        ItemName = orderItem.Item.ItemName,
        //        ItemCount = orderItem.Quantity,
        //        ItemPrice = orderItem.ItemPrice,
        //        //ItemTaxType = orderItem.Item.TaxTypeId == TaxType.Taxable ? 1 : 3,
        //        //ItemAmount = (decimal)orderItem.Item.Price * orderItem.Quantity,
        //    });
        //}

        if (!order.InvoiceNumber.IsNullOrEmpty()) return "";
        var groupBuy = await GroupBuyRepository.GetAsync(order.GroupBuyId);

        var print = setting.IsEnable ? "0" : "1";
        var carrierType = setting.IsEnable ? "1" : null;
        carrierType = order.InvoiceType == InvoiceType.CellphoneInvoice ? "3" : order.InvoiceType == InvoiceType.BusinessInvoice ? "1" : null;

        var carrierNumber = carrierType == "3" ? order.CarrierId : null;
        var customerAddress = order.AddressDetails.IsNullOrEmpty() ? order.CustomerEmail : order.AddressDetails;

        var salesAmount = Convert.ToInt32(order.TotalAmount + order.DeliveryCost + order.DiscountAmount);

        List<ECPayCreateInvoiceInput.Item> invoiceItems = [
            new ECPayCreateInvoiceInput.Item
            {
                ItemSeq = 1,
                ItemName = L["AmountString"],
                ItemCount = 1,
                ItemWord = "1",
                ItemPrice = order.TotalAmount,
                ItemTaxType = 1,
                ItemAmount = order.TotalAmount,
                ItemRemark = ""
            },
            new ECPayCreateInvoiceInput.Item
            {
                ItemSeq = 2,
                ItemName = L["DeliveryFee"],
                ItemCount = 1,
                ItemWord = "1",
                ItemPrice = (decimal)order.DeliveryCost,
                ItemTaxType = 1,
                ItemAmount = (decimal)order.DeliveryCost,
            },
            new ECPayCreateInvoiceInput.Item
            {
                ItemSeq = 3,
                ItemName = L["Discount"],
                ItemCount = 1,
                ItemWord = "1",
                ItemPrice = (decimal)order.DiscountAmount,
                ItemTaxType = 1,
                ItemAmount = (decimal)order.DiscountAmount,
            },
        ];

        var serialNo = await OrderInvoiceRepository.GetInvoiceSerialNoAsync(orderId);
        var relateNo = $"{order.OrderNo}-{serialNo:D3}";

        ECPayCreateInvoiceInput input = new()
        {
            MerchantId = setting?.StoreCode ?? string.Empty,
            RelateNo = relateNo,
            CustomerName = order.InvoiceType == InvoiceType.BusinessInvoice ? order.TaxTitle : order.CustomerName,
            CustomerAddr = order.AddressDetails,
            CustomerPhone = order.RecipientPhone,
            CustomerEmail = order.CustomerEmail,
            ClearanceMark = "1",
            InvoiceRemark = setting?.DisplayInvoiceName ?? string.Empty,
            CarrierNum = carrierNumber,
            CarrierType = carrierType,
            Print = print,
            Donation = "0",
            TaxType = "1", //groupBuy.TaxType==TaxType.Taxable?"1":groupBuy.TaxType==TaxType.NonTaxable?"3":"9",
            SalesAmount = salesAmount,
            CustomerIdentifier = order.UniformNumber,
            InvType = "07",
            Vat = "1", // 含稅4:1 => 未稅6:0
            Items = invoiceItems
        };

        var (transCode, result) = await ECPayInvoiceService.CreateInvoiceAsync(setting.HashKey, setting.HashIV, input);

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        if (transCode == 1)
        {
            if (result.InvoiceNo.IsNullOrWhiteSpace())
            {

                order.VoidUser = CurrentUser.Name;
                order.IssueStatus = IssueInvoiceStatus.Failed;
                await OrderRepository.UpdateAsync(order);

                await OrderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "InvoiceIssueFailed", // Localization key
                    [result.RtnMsg], // Dynamic placeholder for the failure reason
                    currentUserId,
                    currentUserName
                );

                return result.RtnMsg.ToString();
            }
            order.InvoiceNumber = result.InvoiceNo;
            order.IssueStatus = IssueInvoiceStatus.Succeeded;
            order.InvoiceStatus = InvoiceStatus.Issued;
            order.InvoiceDate = DateTime.Parse(result.InvoiceDate);
            order.VoidUser = CurrentUser.Name;

            await OrderRepository.UpdateAsync(order);

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "InvoiceIssued", // Localization key
                [order.InvoiceNumber], // Dynamic placeholders for invoice details
                currentUserId,
                currentUserName
            );

            await OrderInvoiceRepository.CreateInvoiceAsync(new()
            {
                OrderId = order.Id,
                InvoiceNo = result.InvoiceNo,
                RelateNo = relateNo,
                IsVoided = false,
                IssuanceMethod = default,
                InvoiceType = default,
                ShippingCost = (decimal)order.DeliveryCost,
                SubtotalAmount = order.TotalAmount,
                TaxAmount = default,
                TaxType = default,
                TotalAmount = order.TotalAmount,
                SerialNo = serialNo,
            });
        }

        return "";
    }
    public async Task CreateCreditNoteAsync(Guid orderId)
    {
        TenantTripartite? setting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);

        Order order = await OrderRepository.GetWithDetailsAsync(orderId);

        GroupBuy groupBuy = await GroupBuyRepository.GetAsync(order.GroupBuyId);

        string print = order.UniformNumber.IsNullOrWhiteSpace() ? "0" : "1";

        RestClientOptions options = new() { MaxTimeout = -1 };

        RestClient client = new(options);

        RestRequest request = new(Configuration["EcPay:CreditNoteApi"], Method.Post);

        var totalAmount = Convert.ToInt32(order.TotalAmount);

        CreditNoteParameters parameters = new()
        {
            MerchantID = setting?.StoreCode ?? string.Empty,
            InvoiceNo = order.InvoiceNumber,
            CustomerName = order.CustomerName,
            InvoiceDate = order.InvoiceDate == null ? DateTime.Now : order.InvoiceDate.Value,
            NotifyMail = order.RecipientEmail,
            NotifyPhone = order.RecipientPhone,
            AllowanceNotify = "E",
            AllowanceAmount = totalAmount,
            ReturnURL = "https:\\localhost:4000",
            Items =
            [
                new myItem
                {
                    ItemSeq = 1,
                    ItemName = L["Total"],
                    ItemCount = 1,
                    ItemWord = "1",
                    ItemPrice = totalAmount,
                    ItemTaxType = 1,
                    ItemAmount = totalAmount,
                    ItemRemark = ""
                }
            ]
        };

        string json = JsonConvert.SerializeObject(parameters);
        //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
        var newJson = Uri.EscapeDataString(json);
        var encryptedString = await newJson.AesEncryptAsync(setting.HashKey, setting.HashIV);
        request.AddHeader("Content-Type", "application/json");
        var body = $@"{{
                         ""MerchantID"": ""{setting?.StoreCode ?? string.Empty}"",
                         ""RqHeader"": {{
                         ""Timestamp"": {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}
                           }},
                        ""Data"": ""{encryptedString}""
                      }}";
        request.AddStringBody(body, DataFormat.Json);
        RestResponse response = await client.ExecuteAsync(request);
        ApiResponse response1 = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
        if (response1.TransCode == 1)
        {
            string result = await response1.Data.AesDecryptAsync(setting.HashKey, setting.HashIV);

            string? data = HttpUtility.UrlDecode(result);

            ResponseModel? jsonObj = JsonConvert.DeserializeObject<ResponseModel>(data);
        }
    }
    public async Task CreateVoidInvoiceAsync(Guid orderId, string reason)
    {
        Order order = await OrderRepository.GetWithDetailsAsync(orderId);

        TenantTripartite? setting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);

        RestClientOptions options = new() { MaxTimeout = -1 };

        RestClient client = new(options);

        RestRequest request = new(Configuration["EcPay:VoidInvoiceApi"], Method.Post);

        VoidInvoiceParameters parameters = new()
        {
            InvoiceDate = order.InvoiceDate.Value,
            InvoiceNo = order.InvoiceNumber ?? "",
            MerchantID = setting?.StoreCode ?? string.Empty,
            Reason = reason
        };

        string json = JsonConvert.SerializeObject(parameters);
        //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
        string newJson = Uri.EscapeDataString(json);

        string encryptedString = await newJson.AesEncryptAsync(setting.HashKey, setting.HashIV);

        request.AddHeader("Content-Type", "application/json");

        string body = $@"{{
                            ""MerchantID"": ""{setting?.StoreCode ?? string.Empty}"",
                            ""RqHeader"": {{
                            ""Timestamp"": {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}
                              }},
                            ""Data"": ""{encryptedString}""
                         }}";

        request.AddStringBody(body, DataFormat.Json);

        RestResponse response = await client.ExecuteAsync(request);

        ApiResponse? response1 = JsonConvert.DeserializeObject<ApiResponse>(response.Content);

        if (response1?.TransCode is 1)
        {
            string result = await response1.Data.AesDecryptAsync(setting.HashKey, setting.HashIV);

            string? data = HttpUtility.UrlDecode(result);

            VoidInvoiceParameters? jsonObj = JsonConvert.DeserializeObject<VoidInvoiceParameters>(data);

            order.IsVoidInvoice = true;

            order.InvoiceStatus = InvoiceStatus.InvoiceVoided;

            await OrderRepository.UpdateAsync(order);
        }
    }

    public required IConfiguration Configuration { get; init; }
    public required IOrderRepository OrderRepository { get; init; }
    public required IOrderInvoiceRepository OrderInvoiceRepository { get; init; }
    public required IGroupBuyRepository GroupBuyRepository { get; init; }
    public required IECPayInvoiceService ECPayInvoiceService { get; init; }
    public required ITenantTripartiteRepository TenantTripartiteRepository { get; init; }
    public required OrderHistoryManager OrderHistoryManager { get; init; }
}
public class myItem
{
    public int ItemSeq { get; set; }
    public string ItemName { get; set; }
    public int ItemCount { get; set; }
    public string ItemWord { get; set; }
    public decimal ItemPrice { get; set; }
    public int ItemTaxType { get; set; }
    public decimal ItemAmount { get; set; }
    public string ItemRemark { get; set; }
}
public class CreditNoteParameters
{
    public string MerchantID { get; set; }
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    public string NotifyMail { get; set; }
    public string AllowanceNotify { get; set; }
    public string NotifyPhone { get; set; }
    public string ReturnURL { get; set; }
    public int AllowanceAmount { get; set; }
    public List<myItem> Items { get; set; }
}
public class ApiResponse
{
    public int PlatformID { get; set; }
    public int MerchantID { get; set; }
    public int TransCode { get; set; }
    public string TransMsg { get; set; }
    public string Data { get; set; }
}
public class ResponseModel
{
    public int RtnCode { get; set; }
    public string RtnMsg { get; set; }
    public string InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string RandomNumber { get; set; }
}
public class VoidInvoiceParameters
{
    public string MerchantID { get; set; }
    public string InvoiceNo { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Reason { get; set; }
}