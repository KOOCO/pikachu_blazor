using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.StoreLogisticOrders;
using Kooco.Pikachu.TenantEmailing;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TestLables
{
    public class TestLableAppService : ApplicationService, ITestLableAppService
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
        private readonly IConfiguration _configuration;
        public TestLableAppService(IOrderDeliveryRepository orderDeliveryRepository, IOrderRepository orderRepository,
            ILogisticsProvidersAppService logisticsProvidersAppService, IConfiguration configuration)
        {

            _orderRepository = orderRepository;
            _deliveryRepository = orderDeliveryRepository;
            _logisticsProvidersAppService = logisticsProvidersAppService;
            _configuration = configuration;
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

        public async Task<string> TestLableAsync(string logisticSubType)
        {
            var providers = await _logisticsProvidersAppService.GetAllAsync();
            var greenWorld = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.GreenWorldLogistics).FirstOrDefault();
            if (greenWorld != null)
            {
                GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            var options = new RestClientOptions
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(_configuration["EcPay:TestLabelApi"], Method.Post);
         
            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", GreenWorld.StoreCode);
            request.AddParameter("PlatformID", "");
            request.AddParameter("LogisticsSubType", logisticSubType);
            request.AddParameter("ClientReplyURL", "https://eou1b5tf5r0pjog.m.pipedream.net");
           

         
           
            request.AddParameter("CheckMacValue", GenerateRequestString(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, logisticSubType,
                "https://eou1b5tf5r0pjog.m.pipedream.net"));
            
           


            RestResponse response = await client.ExecuteAsync(request);
         
            return response.ToString();
        }

        public string GenerateRequestString(string HashKey, string HashIV, string merchantID, string logisticsSubType,string serverReplyURL)
        {
            //string HashKey = "5294y06JbISpM5x9";
            //string HashIV = "v77hoKGq4kWxNNIS";
            // Create a dictionary to hold parameters
            var parameters = new Dictionary<string, string>
        {
            { "MerchantID", merchantID },
            { "PlatformID", "" },
          
            { "LogisticsSubType", logisticsSubType },
                {"ClientReplyURL",serverReplyURL }

           

           

        };
         
            // Sort parameters alphabetically
            var sortedParameters = parameters.OrderBy(p => p.Key);

            // Construct the request string
            string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

            // Add HashKey and HashIV
            requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";
            string urlEncodedData = HttpUtility.UrlEncode(requestString);

            // Step 5: Convert to lowercase
            string lowercaseData = urlEncodedData.ToLower();

            // Step 6: Create MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2")); // To hexadecimal string
                }
                return sb.ToString(); // Step 7: Convert to uppercase implicitly
            }


        }
    }
}
