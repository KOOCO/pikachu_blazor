using Azure;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    
    public class StoreLogisticsOrderAppService : ApplicationService, IStoreLogisticsOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDeliveryRepository _deliveryRepository;
        private readonly ILogisticsProvidersAppService _logisticsProvidersAppService;
        GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }
        HomeDeliveryCreateUpdateDto HomeDelivery { get; set; }
        PostOfficeCreateUpdateDto PostOffice { get; set; }
        SevenToElevenCreateUpdateDto SevenToEleven { get; set; }
        SevenToElevenCreateUpdateDto SevenToElevenFrozen { get; set; }
        SevenToElevenCreateUpdateDto FamilyMart { get; set; }
        BNormalCreateUpdateDto BNormal { get; set; }
        BNormalCreateUpdateDto BFreeze { get; set; }
        BNormalCreateUpdateDto BFrozen { get; set; }
        public StoreLogisticsOrderAppService(IOrderDeliveryRepository orderDeliveryRepository,IOrderRepository orderRepository,
            ILogisticsProvidersAppService logisticsProvidersAppService) {
           
            _orderRepository = orderRepository;
            _deliveryRepository = orderDeliveryRepository;
            _logisticsProvidersAppService= logisticsProvidersAppService;
            GreenWorld = new();
            HomeDelivery = new();
            PostOffice = new();
            SevenToEleven = new();
            SevenToElevenFrozen = new();
            FamilyMart = new();
            BNormal = new();
            BFreeze = new();
            BFrozen = new();

        }

        public async Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            var orderDeliverys = await _deliveryRepository.GetWithDetailsAsync(orderId);
            var orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();
            var providers = await _logisticsProvidersAppService.GetAllAsync();

            var greenWorld = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.GreenWorldLogistics).FirstOrDefault();
            if (greenWorld != null)
            {
                GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            var homeDelivery = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.HomeDelivery).FirstOrDefault();
            if (homeDelivery != null)
            {
                HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);
            }
            var postOffice = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.PostOffice).FirstOrDefault();
            if (postOffice != null)
            {
                PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);
            }
            var sevenToEleven = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToEleven).FirstOrDefault();
            if (sevenToEleven != null)
            {
                SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);
            }
            var familyMart = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.FamilyMart).FirstOrDefault();
            if (familyMart != null)
            {
                FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);
            }
            var sevenToElevenFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
            if (sevenToElevenFrozen != null)
            {
                SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);
            }
            var bNormal = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BNormal).FirstOrDefault();
            if (bNormal != null)
            {
                BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);
            }
            var bFreeze = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFreeze).FirstOrDefault();
            if (bFreeze != null)
            {
                BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
            var bFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFrozen).FirstOrDefault();
            if (bFrozen != null)
            {
                BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
            var options = new RestClientOptions
            {
                MaxTimeout = -1,
            };

           



            var client = new RestClient(options);
            var request = new RestRequest("https://logistics-stage.ecpay.com.tw/Express/Create", Method.Post);
            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", GreenWorld.StoreCode);
            request.AddParameter("MerchantTradeDate", DateTime.Now.ToShortDateString());
            request.AddParameter("LogisticsType", "HOME");
            request.AddParameter("LogisticsSubType",orderDelivery.DeliveryMethod==EnumValues.DeliveryMethod.PostOffice?"POST":"TCAT");
            request.AddParameter("GoodsAmount", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)));
            request.AddParameter("GoodsWeight", PostOffice.Weight);
            request.AddParameter("SenderName", GreenWorld.SenderName);
            request.AddParameter("SenderPhone", GreenWorld.SenderPhoneNumber);
            request.AddParameter("SenderZipCode", GreenWorld.SenderPostalCode);
            request.AddParameter("SenderAddress", GreenWorld.SenderAddress);

            request.AddParameter("ReceiverName", order.RecipientName);
            request.AddParameter("ReceiverCellPhone", order.RecipientPhone);
            request.AddParameter("ReceiverZipCode", order.PostalCode);
            request.AddParameter("ReceiverAddress", order.AddressDetails);
            request.AddParameter("ServerReplyURL", "https://www.ecpay.com.tw/ServerReplyURL");
            //request.AddParameter("ReceiverStoreID", "123");
            request.AddParameter("CheckMacValue", GenerateCheckMac(GreenWorld.StoreCode, "", DateTime.Now.ToShortDateString(), "HOME", orderDelivery.DeliveryMethod == EnumValues.DeliveryMethod.PostOffice ? "POST" : "TCAT", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)),PostOffice.Weight, GreenWorld.SenderName,GreenWorld.SenderPhoneNumber,
                                                    GreenWorld.SenderPostalCode,GreenWorld.SenderAddress, order.RecipientName, order.RecipientPhone,order.PostalCode,order.AddressDetails, "https://www.ecpay.com.tw/ServerReplyURL"));
            //request.AddParameter("IsCollection", "N");
            request.AddParameter("MerchantTradeNo",  "");


            RestResponse response = await client.ExecuteAsync(request);
            ResponseResultDto result = ParseApiResponse(response.Content.ToString());
            if (result.ResponseCode == "1")
            {
                orderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;
                orderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;
                await _deliveryRepository.UpdateAsync(orderDelivery);
            }
            return result;
        }

        public async Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(CreateLogisticsOrder input)
        {
            ResponseResultDto result = new ResponseResultDto();
            var order = await _orderRepository.GetAsync(input.OrderId);
            var orderDeliverys = await _deliveryRepository.GetWithDetailsAsync(input.OrderId);
            var orderDelivery = orderDeliverys.Where(x => x.Id == input.DeliveryId).FirstOrDefault();
            var providers = await _logisticsProvidersAppService.GetAllAsync();

            var greenWorld = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.GreenWorldLogistics).FirstOrDefault();
            if (greenWorld != null)
            {
                GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            var homeDelivery = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.HomeDelivery).FirstOrDefault();
            if (homeDelivery != null)
            {
                HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);
            }
            var postOffice = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.PostOffice).FirstOrDefault();
            if (postOffice != null)
            {
                PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);
            }
            var sevenToEleven = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToEleven).FirstOrDefault();
            if (sevenToEleven != null)
            {
                SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);
            }
            var familyMart = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.FamilyMart).FirstOrDefault();
            if (familyMart != null)
            {
                FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);
            }
            var sevenToElevenFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
            if (sevenToElevenFrozen != null)
            {
                SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);
            }
            var bNormal = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BNormal).FirstOrDefault();
            if (bNormal != null)
            {
                BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);
            }
            var bFreeze = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFreeze).FirstOrDefault();
            if (bFreeze != null)
            {
                BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
            var bFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFrozen).FirstOrDefault();
            if (bFrozen != null)
            {
                BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
            var options = new RestClientOptions
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://logistics-stage.ecpay.com.tw/Express/Create", Method.Post);
            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", "2000132");
            request.AddParameter("MerchantTradeDate", "2024/03/06");
            request.AddParameter("LogisticsType", "CVS");
            request.AddParameter("LogisticsSubType", "FAMI");
            request.AddParameter("GoodsAmount", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)));
            request.AddParameter("SenderName", CurrentUser.Name);
            request.AddParameter("ReceiverName", order.RecipientName);
            request.AddParameter("ReceiverCellPhone", order.RecipientPhone);
            request.AddParameter("ServerReplyURL", "https://www.ecpay.com.tw/ServerReplyURL");
            request.AddParameter("ReceiverStoreID", "123");
            request.AddParameter("CheckMacValue", GenerateRequestString("2000132", order.OrderNo, "2024/03/06", "CVS", "FAMI", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), "N", CurrentUser.Name, order.RecipientName, order.RecipientPhone, "https://www.ecpay.com.tw/ServerReplyURL", "123"));
            request.AddParameter("IsCollection", "N");
            request.AddParameter("MerchantTradeNo", order.OrderNo);


            RestResponse response = await client.ExecuteAsync(request);
             result = ParseApiResponse(response.Content.ToString());
            if (result.ResponseCode == "1")
            {
                orderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;
                orderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;
                await _deliveryRepository.UpdateAsync(orderDelivery);
            }
            return result;
        }
        public string GenerateRequestString(string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, string isCollection, string senderName, string receiverName, string receiverCellPhone, string serverReplyURL, string receiverStoreID)
        {
            string HashKey = "5294y06JbISpM5x9";
            string HashIV = "v77hoKGq4kWxNNIS";
            // Create a dictionary to hold parameters
            var parameters = new Dictionary<string, string>
        {
            { "MerchantID", merchantID },
            { "MerchantTradeNo", merchantTradeNo },
            { "MerchantTradeDate", merchantTradeDate },
            { "LogisticsType", logisticsType },
            { "LogisticsSubType", logisticsSubType },
            { "GoodsAmount", goodsAmount.ToString() },

            { "IsCollection", isCollection },

            { "SenderName", senderName },

            { "ReceiverName", receiverName },

            { "ReceiverCellPhone", receiverCellPhone },

            { "ServerReplyURL", serverReplyURL },


            { "ReceiverStoreID", receiverStoreID },

        };

            // Sort parameters alphabetically
            var sortedParameters = parameters.OrderBy(p => p.Key);

            // Construct the request string
            string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

            // Add HashKey and HashIV
            requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";

            // URL encode the entire string
            string urlEncodedString = HttpUtility.UrlEncode(requestString);

            // Lowercase the string
            string lowercaseString = urlEncodedString.ToLower();

            // Replace characters according to the urlencode conversion table
            // (Note: In C#, URL encoding is done by default in UrlEncode method, so this step is optional)
            //lowercaseString = lowercaseString.Replace("%20", "+").Replace("%21", "!").Replace("%27", "'").Replace("%28", "(").Replace("%29", ")").Replace("%2a", "*").Replace("%2d", "-").Replace("%2e", ".").Replace("%5f", "_").Replace("%7e", "~");

            // Use MD5 encryption to generate the hash value
            string md5Checksum;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                md5Checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            // Uppercase the MD5 checksum
            string finalChecksum = md5Checksum.ToUpper();

            // Append CheckMacValue to the request string
            requestString += $"&CheckMacValue={finalChecksum}";

            return finalChecksum;


        }
        public async Task<EmapApiResponse> GetStoreAsync(Guid orderId, Guid orderDeliveryId)
        {
            EmapApiResponse result=new EmapApiResponse();
            var order=await _orderRepository.GetAsync(orderId);
            var orderDeliverys= await _deliveryRepository.GetWithDetailsAsync(orderId);
            var orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();
            var options = new RestClientOptions
            {
                MaxTimeout = -1,
            };

            var client1 = new RestClient(options);
            var request1 = new RestRequest("https://logistics.ecpay.com.tw/Home/Family", Method.Post);
            request1.AddHeader("Accept", "text/html");
            request1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request1.AddHeader("Cookie", "MapInfo=MerchantID=2000132&MerchantTradeNo=ECPay&LogisticsType=CVS&LogisticsSubType=FAMI&IsCollection=N&ServerReplyURL=https%3a%2f%2fwww.ecpay.com.tw%2fServerReplyURL&CallBackFunction=&IsGet=&Device=0");
            request1.AddParameter("MerchantID", "2000132");
            request1.AddParameter("LogisticsType", "CVS");
            request1.AddParameter("LogisticsSubType", "UNIMART");
            request1.AddParameter("ServerReplyURL", "https://ba6ee5b524c5287100a78b51d8b7fc18.m.pipedream.net");
            request1.AddParameter("IsCollection", "N");
            RestResponse response1 = await client1.ExecuteAsync(request1);
            
            Console.WriteLine(response1.Content);
            result.HtmlString = response1.Content;
            result.CookieName = response1.Cookies.Select(x => x.Name).FirstOrDefault();
            result.CookieValue = response1.Cookies.Select(x => x.Value).FirstOrDefault();

           

           
            return result;
          
            


           
        }
        public string GenerateCheckMac(string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, decimal goodsWeight, string senderName, string senderPhone, string senderZipCode, string senderAddress,
                                    string receiverName, string receiverCellPhone, string receiverZipCode,string receiverAddress, string serverReplyURL)
        {
            string HashKey = "5294y06JbISpM5x9";
            string HashIV = "v77hoKGq4kWxNNIS";
            // Create a dictionary to hold parameters
            var parameters = new Dictionary<string, string>
        {
            { "MerchantID", merchantID },
            { "MerchantTradeNo", merchantTradeNo },
            { "MerchantTradeDate", merchantTradeDate },
            { "LogisticsType", logisticsType },
            { "LogisticsSubType", logisticsSubType },
            { "GoodsAmount", goodsAmount.ToString() },
            { "GoodsWeight", goodsWeight.ToString() },

            

            { "SenderName", senderName },
            { "SenderPhone", senderPhone},
            { "SenderZipCode", senderZipCode},
            { "SenderAddress", senderAddress},

            { "ReceiverName", receiverName },

            { "ReceiverCellPhone", receiverCellPhone },
            { "ReceiverZipCode", receiverZipCode },
            { "ReceiverAddress", receiverAddress },

            { "ServerReplyURL", serverReplyURL },


            

        };

            // Sort parameters alphabetically
            var sortedParameters = parameters.OrderBy(p => p.Key);

            // Construct the request string
            string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

            // Add HashKey and HashIV
            requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";

            // URL encode the entire string
            string urlEncodedString = HttpUtility.UrlEncode(requestString);

            // Lowercase the string
            string lowercaseString = urlEncodedString.ToLower();

            // Replace characters according to the urlencode conversion table
            // (Note: In C#, URL encoding is done by default in UrlEncode method, so this step is optional)
            //lowercaseString = lowercaseString.Replace("%20", "+").Replace("%21", "!").Replace("%27", "'").Replace("%28", "(").Replace("%29", ")").Replace("%2a", "*").Replace("%2d", "-").Replace("%2e", ".").Replace("%5f", "_").Replace("%7e", "~");

            // Use MD5 encryption to generate the hash value
            string md5Checksum;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                md5Checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            // Uppercase the MD5 checksum
            string finalChecksum = md5Checksum.ToUpper();

            // Append CheckMacValue to the request string
            requestString += $"&CheckMacValue={finalChecksum}";

            return finalChecksum;
        }

        static ResponseResultDto ParseApiResponse(string apiResponse)
        {
            var result = new ResponseResultDto();

            // Split the response into response code and key-value pairs
            string[] responseParts = apiResponse.Split('|');
            result.ResponseCode = responseParts[0];
            if (result.ResponseCode != "1")
            {
                result.ResponseMessage = responseParts[1];
            return result;
            }
            // Split the key-value pairs
            string[] keyValuePairs = responseParts[1].Split('&');

            // Parse key-value pairs into dictionary
            var dataDict = new Dictionary<string, string>();
            foreach (string kvPair in keyValuePairs)
            {
                string[] parts = kvPair.Split('=');
                string key = parts[0];
                string value = parts.Length > 1 ? parts[1] : string.Empty;

                dataDict[key] = value;
            }

            // Create an instance of the ShippingInfo model
            result.ShippingInfo = new ShippingInfoDto
            {
                AllPayLogisticsID = dataDict["AllPayLogisticsID"],
                BookingNote = dataDict["BookingNote"],
                CheckMacValue = dataDict["CheckMacValue"],
                // Add more properties as needed
            };

            return result;
        }
    }
}
