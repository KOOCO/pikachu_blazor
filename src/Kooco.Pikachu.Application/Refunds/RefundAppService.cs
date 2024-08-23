using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;

namespace Kooco.Pikachu.Refunds
{
    [RemoteService(IsEnabled = false)]
    [Authorize(PikachuPermissions.Refund.Default)]
    public class RefundAppService : ApplicationService, IRefundAppService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IEmailSender _EmailSender;
       // private readonly ILogisticsProvidersAppService _logisticsProvidersAppService;
        GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }
        //private readonly IConfiguration _configuration;

        public RefundAppService(
            IRefundRepository refundRepository,
            IOrderRepository orderRepository,
            IRepository<PaymentGateway, Guid> paymentGatewayRepository,
            IEmailSender EmailSender
        )
        {
            _refundRepository = refundRepository;
            _orderRepository = orderRepository;
            _paymentGatewayRepository = paymentGatewayRepository;
            _EmailSender = EmailSender;
            GreenWorld = new();
        }

        [Authorize(PikachuPermissions.Refund.Create)]
        public async Task CreateAsync(Guid orderId)
        {
            var existing = await _refundRepository.FindAsync(x => x.OrderId == orderId);
            if (existing != null)
            {
                throw new BusinessException(PikachuDomainErrorCodes.RefundForSameOrderAlreadyExists);
            }
            var refund = new Refund(GuidGenerator.Create(), orderId);
            await _refundRepository.InsertAsync(refund);
            var order = await _orderRepository.GetAsync(orderId);
            order.IsRefunded = true;
            await _orderRepository.UpdateAsync(order);

            /// ToDo: Send Refund Email Here, and also change status for order
        }

        public async Task<PagedResultDto<RefundDto>> GetListAsync(GetRefundListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Refund.CreationTime)} desc";
            }
            var totalCount = await _refundRepository.GetCountAsync(input.Filter);
            var items = await _refundRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            return new PagedResultDto<RefundDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Refund>, List<RefundDto>>(items)
            };
        }

        public async Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input)
        {
            var refund = await _refundRepository.GetAsync(id);
            refund.RefundReview = input;
            if (input == RefundReviewStatus.Proccessing||input==RefundReviewStatus.ReturnedApplication)
            {
                refund.ReviewCompletionTime = DateTime.Now;
                refund.Approver = CurrentUser.Name;
            }
            if (input == RefundReviewStatus.Fail || input == RefundReviewStatus.Success)
            {
               
                refund.Refunder = CurrentUser.Name;
            }
            await _refundRepository.UpdateAsync(refund);
            return ObjectMapper.Map<Refund, RefundDto>(refund);
        }

        public async Task SendEmailForRefundAsync(string to, string orderNo)
        {
            string subject = $"Refund For Order #{orderNo}";

            string body = $"Refund has been approved for order #{orderNo}";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _EmailSender.SendAsync(to, subject, body);
        }

        public async Task SendRefundRequestAsync(Guid id)
        {
            Refund refund = await _refundRepository.GetAsync(id);

            Order order = await _orderRepository.GetAsync(refund.OrderId);

            PaymentGateway? ecpay = (await _paymentGatewayRepository.GetQueryableAsync()).FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay) ?? 
                                    throw new UserFriendlyException("Please Set Ecpay Setting First");

            string logisticSubType = string.Empty;

            RestClientOptions options = new () { MaxTimeout = -1 };

            RestClient client = new (options);

            RestRequest request = new ("https://payment.ecpay.com.tw/CreditDetail/DoAction", Method.Post);
   
            string HashKey = ecpay.HashKey ?? string.Empty;
            string HashIV = ecpay.HashIV ?? string.Empty;
            string MerchantId = ecpay.MerchantId ?? string.Empty;
            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", MerchantId);
            request.AddParameter("MerchantTradeNo", (refund.Order?.MerchantTradeNo) ?? (refund.Order?.OrderNo));
            request.AddParameter("TradeNo", refund.Order?.TradeNo);
            request.AddParameter("TotalAmount", (int)(refund.Order?.TotalAmount));
            request.AddParameter("Action", "R");
            request.AddParameter("CheckMacValue", GenerateCheckMac(HashKey, HashIV, MerchantId, refund.Order?.MerchantTradeNo != null ? refund.Order?.MerchantTradeNo : refund.Order?.OrderNo,refund.Order?.TradeNo,"R",((int)refund.Order?.TotalAmount).ToString()));
            //request.AddParameter("IsCollection", "N");
           
            RestResponse response = await client.ExecuteAsync(request);
            
            NameValueCollection queryParams = HttpUtility.ParseQueryString(response.Content);

            RefundResponse result = new ()
            {
                MerchantID = queryParams["MerchantID"] ?? string.Empty,
                MerchantTradeNo = queryParams["MerchantTradeNo"] ?? string.Empty,
                TradeNo = queryParams["TradeNo"] ?? string.Empty,
                RtnMsg = queryParams["RtnMsg"] ?? string.Empty,
                RtnCode = int.Parse(queryParams["RtnCode"])
            };

            if (result is not null)
            {
                if (result.RtnCode is 1)
                {
                    refund.RefundReview = RefundReviewStatus.Success;

                    if (!order.CustomerEmail.IsNullOrEmpty())
                        await SendEmailForRefundAsync(order.CustomerEmail, order.OrderNo);
                }

                else refund.RefundReview = RefundReviewStatus.Fail;
            }
        }
        public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string tradeNo,string action,string totalamount)
        {
            // Create a dictionary to hold parameters
            var parameters = new Dictionary<string, string>
            {
                { "MerchantID", merchantID },
                { "MerchantTradeNo", merchantTradeNo },
                { "TradeNo", tradeNo },
                { "Action", action },
                { "TotalAmount", totalamount },
               
            };
            IEnumerable<string>? param = parameters.ToDictionary().Keys
                                          .OrderBy(o => o)
                                          .Select(s => s + "=" + parameters.ToDictionary()[s]);

            string collectionValue = string.Join("&", param);

            collectionValue = $"HashKey={HashKey}" + "&" + collectionValue + $"&HashIV={HashIV}";

            collectionValue = WebUtility.UrlEncode(collectionValue).ToLower();

            return ComputeSHA256Hash(collectionValue);
            //// Sort parameters alphabetically
            //var sortedParameters = parameters.OrderBy(p => p.Key);

            //// Construct the request string
            //string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

            //// Add HashKey and HashIV
            //requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";

            //// URL encode the entire string
            ////requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";
            //string urlEncodedData = HttpUtility.UrlEncode(requestString);

            //// Step 5: Convert to lowercase
            //string lowercaseData = urlEncodedData.ToLower();

            //// Step 6: Create MD5 hash
            //using (MD5 md5 = MD5.Create())
            //{
            //    byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
            //    byte[] hashBytes = md5.ComputeHash(inputBytes);

            //    // Convert byte array to hex string
            //    StringBuilder sb = new StringBuilder();
            //    for (int i = 0; i < hashBytes.Length; i++)
            //    {
            //        sb.Append(hashBytes[i].ToString("X2")); // To hexadecimal string
            //    }
            //    return sb.ToString(); // Step 7: Convert to uppercase implicitly
            //}
        }
        public string ComputeSHA256Hash(string rawData)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString().ToUpper();
        }
    }
    public class RefundResponse {
       
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string TradeNo { get; set; }
        public string RtnMsg { get; set; }
        public int RtnCode { get; set; }

    }
}
