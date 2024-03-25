using Hangfire.Storage;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public class ElectronicInvoiceAppService : ApplicationService, IElectronicInvoiceAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<EnumValue, int> _enumvalueRepository;
        private readonly IElectronicInvoiceSettingRepository _repository;
        public ElectronicInvoiceAppService(IConfiguration configuration, IOrderRepository orderRepository,
                                            IElectronicInvoiceSettingRepository repository,
                                            IRepository<EnumValue, int> enumvalueRepository)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _repository = repository;
            _enumvalueRepository= enumvalueRepository;

        }
        public async Task CreateInvoiceAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);

            var options = new RestClientOptions
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://einvoice-stage.ecpay.com.tw/B2CInvoice/Issue", Method.Post);

            var parameters = new Parameters
            {
                MerchantID = "2000132",
                RelateNumber = order.OrderNo,
                CustomerName = order.CustomerName,
                CustomerAddr = order.AddressDetails,
                CustomerPhone = order.RecipientPhone,
                CustomerEmail = order.CustomerEmail,
                ClearanceMark = "1",
                Print = "1",
                Donation = "0",
                TaxType = "1",
                SalesAmount = order.TotalAmount,
                InvType = "07",
                vat = "1",
                Items = new List<myItem>()
            };
            foreach (var item in order.OrderItems)
            { 
            myItem orderitem=new myItem();
                orderitem.ItemSeq = 1;
                orderitem.ItemName = item.Item.ItemName;
             //orderitem.ItemCount = 1;
                //orderitem.ItemWord = "";
                orderitem.ItemPrice = item.ItemPrice;
                orderitem.ItemTaxType =((int)(await _enumvalueRepository.FirstOrDefaultAsync(x=>x.Id==item.Item.TaxTypeId)).EnumType).ToString();
                orderitem.ItemAmount = item.TotalAmount;
                orderitem.ItemRemark = "";
                parameters.Items.Add(orderitem);

            }

    
        string json = JsonConvert.SerializeObject(parameters);
            //var json = "{\"MerchantID\": \"2000132\",\"RelateNumber\": ,\"CustomerName\": \"SomiKayani\",\"CustomerAddr\": \"Abcxyz street 123\",\"CustomerPhone\": \"0912345678\",\"CustomerEmail\": \"kiani_mujahid@yahoo.com\",\"ClearanceMark\": \"1\",\"Print\": \"1\",\"Donation\": \"0\",\"TaxType\": \"1\",\"SalesAmount\": 70,\"InvType\": \"07\",\"vat\": \"1\",\"Items\": [{\"ItemSeq\": 1,\"ItemName\": \"item01\",\"ItemCount\": 1,\"ItemWord\": \"Test\",\"ItemPrice\": 50,\"ItemTaxType\": \"1\",\"ItemAmount\": 50,\"ItemRemark\": \"item01_desc\"},{\"ItemSeq\": 2,\"ItemName\": \"item02\",\"ItemCount\": 1,\"ItemWord\": \"Test2\",\"ItemPrice\": 20,\"ItemTaxType\": \"1\",\"ItemAmount\": 20,\"ItemRemark\": \"item02_desc\"}]}";
          var newJson  = Uri.EscapeDataString(json);
            var encryptedString = EncryptStringToAES(newJson, "ejCk326UnaZWKisg", "q9jcZX8Ib9LM8wYk");
            request.AddHeader("Content-Type", "application/json");
            var body = $@"{{
                         ""MerchantID"": ""2000132"",
                         ""RqHeader"": {{
                         ""Timestamp"": {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}
                           }},
                        ""Data"": ""{encryptedString}""
                            }}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            ApiResponse response1 = JsonConvert.DeserializeObject<ApiResponse>(json);
            if (response1.TransCode == 1)
            {
                var result = DecryptStringFromAES(response1.Data, "ejCk326UnaZWKisg", "q9jcZX8Ib9LM8wYk");

               var data= HttpUtility.UrlDecode(result);
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(data);

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
    }
}
public class myItem
{
    public int ItemSeq { get; set; }
    public string ItemName { get; set; }
    public int ItemCount { get; set; }
    public string ItemWord { get; set; }
    public decimal ItemPrice { get; set; }
    public string ItemTaxType { get; set; }
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
    public string Print { get; set; }
    public string Donation { get; set; }
    public string TaxType { get; set; }
    public decimal SalesAmount { get; set; }
    public string InvType { get; set; }
    public string vat { get; set; }
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