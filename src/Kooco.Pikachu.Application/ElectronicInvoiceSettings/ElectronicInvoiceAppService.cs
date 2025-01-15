using Azure.Core;
using Hangfire.Storage;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;

public class ElectronicInvoiceAppService : ApplicationService, IElectronicInvoiceAppService
{
    #region Inject
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<EnumValue, int> _enumvalueRepository;
    private readonly IGroupBuyRepository _groupBuyRepository;
    private readonly IElectronicInvoiceSettingRepository _repository;
    private readonly IDataFilter _dataFilter;
    #endregion

    #region Constructor
    public ElectronicInvoiceAppService(IConfiguration configuration, IOrderRepository orderRepository,
                                        IElectronicInvoiceSettingRepository repository,
                                        IRepository<EnumValue, int> enumvalueRepository,
                                        IGroupBuyRepository groupBuyRepository,
                                        IDataFilter dataFilter)
    {
        _configuration = configuration;
        _orderRepository = orderRepository;
        _repository = repository;
        _enumvalueRepository= enumvalueRepository;
        _groupBuyRepository= groupBuyRepository;
        _dataFilter = dataFilter;
    }
    #endregion

    #region Methods
    public async Task<string> CreateInvoiceAsync(Guid orderId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            ElectronicInvoiceSetting? setting = await _repository.FirstOrDefaultAsync();
            //Order order = new Order();

            var order = await _orderRepository.GetWithDetailsAsync(orderId);

            if (!order.InvoiceNumber.IsNullOrEmpty()) return"";

            GroupBuy groupBuy = await _groupBuyRepository.GetAsync(order.GroupBuyId);

            string print = setting.IsEnable ? "0" : "1";
            string? CarrierType = setting.IsEnable ? "1" : null;
            CarrierType = (order.InvoiceType == InvoiceType.CellphoneInvoice) ? "3" : (order.InvoiceType == InvoiceType.BusinessInvoice ? "1" : null); 
            string? CarrierNumber = CarrierType == "3" ? order.UniformNumber : null;
            string CustomerAddress = order.AddressDetails.IsNullOrEmpty() ? order.CustomerEmail : order.AddressDetails;
            RestClientOptions options = new() { MaxTimeout = -1 };

            RestClient client = new(options);

            RestRequest request = new(_configuration["EcPay:InvoiceApi"], Method.Post);

            Parameters parameters = new()
            {
                MerchantID = setting?.StoreCode ?? string.Empty,
                RelateNumber = order.OrderNo,
                CustomerName = order.InvoiceType == InvoiceType.BusinessInvoice?order.TaxTitle: order.CustomerName,
                CustomerAddr = order.AddressDetails,
                CustomerPhone = order.RecipientPhone,
                CustomerEmail = order.CustomerEmail,
                ClearanceMark = "1",
                InvoiceRemark = setting?.DisplayInvoiceName ?? string.Empty,
                
                
                CarrierNum = CarrierNumber,
                CarrierType = CarrierType,

                Print = print,
                Donation = "0",
                TaxType = "1", //groupBuy.TaxType==TaxType.Taxable?"1":groupBuy.TaxType==TaxType.NonTaxable?"3":"9",
                SalesAmount = order.OrderItems.Sum(x => x.TotalAmount),
                InvType = "07",
                vat = "1",
                Items = new List<myItem>()
            };
            if (order.InvoiceType == InvoiceType.BusinessInvoice)
            {
                parameters.CustomerIdentifier = order.CarrierId;
            
            }    
            foreach (var item in order?.OrderItems)
            {
                myItem orderitem = new();
                orderitem.ItemSeq = 1;
                orderitem.ItemName = item.Item?.ItemName ?? item.Freebie.ItemName;
                orderitem.ItemCount = 1;
                orderitem.ItemWord = "1";
                orderitem.ItemPrice = item.ItemPrice;
                orderitem.ItemTaxType = 1;//(await _enumvalueRepository.FirstOrDefaultAsync(x=>x.Id==item.Item.TaxTypeId)).Text=="Taxable"?1:3;
                orderitem.ItemAmount = item.TotalAmount;
                orderitem.ItemRemark = "";
                parameters.Items.Add(orderitem);

            }


            string json = JsonConvert.SerializeObject(parameters);
            //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
            var newJson = Uri.EscapeDataString(json);
            var encryptedString = EncryptStringToAES(newJson, setting.HashKey, setting.HashIV);
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
            ApiResponse? response1 = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            if (response1?.TransCode == 1)
            {
                var result = DecryptStringFromAES(response1.Data, setting.HashKey, setting.HashIV);

                var data = HttpUtility.UrlDecode(result);
                ResponseModel jsonObj = JsonConvert.DeserializeObject<ResponseModel>(data);
                if (jsonObj.InvoiceNo.IsNullOrWhiteSpace())
                {
				
						order.VoidUser = CurrentUser.Name;
						order.IssueStatus = IssueInvoiceStatus.Failed;
						await _orderRepository.UpdateAsync(order);
                      
					

					return jsonObj.RtnMsg.ToString();
                   
                }
                order.InvoiceNumber = jsonObj.InvoiceNo;
                order.IssueStatus = IssueInvoiceStatus.Succeeded;
                order.InvoiceStatus = InvoiceStatus.Issued;
                order.InvoiceDate = jsonObj.InvoiceDate;
                order.VoidUser = CurrentUser.Name;
              
                await _orderRepository.UpdateAsync(order);
            }
            return "";
        }
    }
    public async Task CreateCreditNoteAsync(Guid orderId)
    {
        ElectronicInvoiceSetting? setting = await _repository.FirstOrDefaultAsync();

        Order order = await _orderRepository.GetWithDetailsAsync(orderId);
       
        GroupBuy groupBuy = await _groupBuyRepository.GetAsync(order.GroupBuyId);

        string print = order.UniformNumber.IsNullOrWhiteSpace() ? "0" : "1";

        RestClientOptions options = new () { MaxTimeout = -1 };

        RestClient client = new (options);

        RestRequest request = new (_configuration["EcPay:CreditNoteApi"], Method.Post);

        CreditNoteParameters parameters = new ()
        {
            MerchantID = setting?.StoreCode ?? string.Empty,
            InvoiceNo = order.InvoiceNumber,
            CustomerName = order.CustomerName,
            InvoiceDate = order.InvoiceDate==null?DateTime.Now: order.InvoiceDate.Value,
            NotifyMail=order.RecipientEmail,
            NotifyPhone=order.RecipientPhone,
            AllowanceNotify="E",
            AllowanceAmount =(int)order.OrderItems.Sum(x => x.TotalAmount),
            ReturnURL="https:\\localhost:4000",
            Items = new List<myItem>()
        };

        foreach (var item in order?.OrderItems)
        {
            myItem orderitem = new myItem();
            orderitem.ItemSeq = 1;
            orderitem.ItemName = (item.Item?.ItemName ?? item.Freebie?.ItemName)??"";
            orderitem.ItemCount = 1;
            orderitem.ItemWord = "1";
            orderitem.ItemPrice = item.ItemPrice;
            orderitem.ItemTaxType = 1;//(await _enumvalueRepository.FirstOrDefaultAsync(x=>x.Id==item.Item.TaxTypeId)).Text=="Taxable"?1:3;
            orderitem.ItemAmount = item.TotalAmount;
            orderitem.ItemRemark = "";
            parameters.Items.Add(orderitem);
        }

        string json = JsonConvert.SerializeObject(parameters);
        //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
        var newJson = Uri.EscapeDataString(json);
        var encryptedString = EncryptStringToAES(newJson, setting.HashKey, setting.HashIV);
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
            string result = DecryptStringFromAES(response1.Data, setting.HashKey, setting.HashIV);

            string? data = HttpUtility.UrlDecode(result);
          
            ResponseModel? jsonObj = JsonConvert.DeserializeObject<ResponseModel>(data);
        }
    }
    public async Task CreateVoidInvoiceAsync(Guid orderId, string reason)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(orderId);

        ElectronicInvoiceSetting? setting = await _repository.FirstOrDefaultAsync();

        RestClientOptions options = new () { MaxTimeout = -1 };

        RestClient client = new (options);

        RestRequest request = new (_configuration["EcPay:VoidInvoiceApi"], Method.Post);

        VoidInvoiceParameters parameters = new ()
        {
            InvoiceDate = order.InvoiceDate.Value,
            InvoiceNo = order.InvoiceNumber??"",
            MerchantID = setting?.StoreCode ?? string.Empty,
            Reason = reason
        };
        
        string json = JsonConvert.SerializeObject(parameters);
        //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
        string newJson = Uri.EscapeDataString(json);

        string encryptedString = EncryptStringToAES(newJson, setting.HashKey, setting.HashIV);
        
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
            string result = DecryptStringFromAES(response1.Data, setting.HashKey, setting.HashIV);

            string? data = HttpUtility.UrlDecode(result);

            VoidInvoiceParameters? jsonObj = JsonConvert.DeserializeObject<VoidInvoiceParameters>(data);

            order.IsVoidInvoice = true;
            
            order.InvoiceStatus = InvoiceStatus.InvoiceVoided;
       
            await _orderRepository.UpdateAsync(order);
        }
    }

    static string EncryptStringToAES(string plainText, string key, string iv)
    {
        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.KeySize = 128; // Set key size to 128 bits
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            aesAlg.Mode = CipherMode.CBC; // Set cipher mode to CBC
            aesAlg.Padding = PaddingMode.PKCS7; // Set padding mode to PKCS7

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }
    static string DecryptStringFromAES(string cipherText, string key, string iv)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.KeySize = 128; // Set key size to 128 bits
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            aesAlg.Mode = CipherMode.CBC; // Set cipher mode to CBC
            aesAlg.Padding = PaddingMode.PKCS7; // Set padding mode to PKCS7

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    #endregion
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

public class Parameters
{
public string MerchantID { get; set; }
public string RelateNumber { get; set; }
public string CustomerName { get; set; }
public string CustomerAddr { get; set; }
public string CustomerPhone { get; set; }
public string CustomerEmail { get; set; }
public string ClearanceMark { get; set; }
public string InvoiceRemark { get; set; }
public string CustomerIdentifier { get; set; }
public string Print { get; set; }
public string? CarrierNum { get; set; }
public string? CarrierType { get; set; }
public string Donation { get; set; }
public string TaxType { get; set; }
public decimal SalesAmount { get; set; }
public string InvType { get; set; }
public string vat { get; set; }
public List<myItem> Items { get; set; }
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
public class VoidInvoiceParameters {
public string MerchantID { get; set; }
public string InvoiceNo { get; set; }
public DateTime InvoiceDate { get; set; }
public string Reason { get; set; }

}