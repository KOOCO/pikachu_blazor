using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.StoreLogisticOrders;

[RemoteService(IsEnabled = false)]
public class StoreLogisticsOrderAppService : ApplicationService, IStoreLogisticsOrderAppService
{
    #region Inject
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDeliveryRepository _deliveryRepository;
    private readonly ILogisticsProvidersAppService _logisticsProvidersAppService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }
    GreenWorldLogisticsCreateUpdateDto GreenWorldC2C { get; set; }
    HomeDeliveryCreateUpdateDto HomeDelivery { get; set; }
    PostOfficeCreateUpdateDto PostOffice { get; set; }
    SevenToElevenCreateUpdateDto SevenToEleven { get; set; }
    SevenToElevenCreateUpdateDto SevenToElevenFrozen { get; set; }
    SevenToElevenCreateUpdateDto FamilyMart { get; set; }
    BNormalCreateUpdateDto BNormal { get; set; }
    BNormalCreateUpdateDto BFreeze { get; set; }
    BNormalCreateUpdateDto BFrozen { get; set; }
    TCatLogisticsCreateUpdateDto TCatLogistics { get; set; }
    TCatNormalCreateUpdateDto TCatNormal { get; set; }
    TCatFreezeCreateUpdateDto TCatFreeze { get; set; }
    TCatFrozenCreateUpdateDto TCatFrozen { get; set; }
    TCat711NormalCreateUpdate TCat711Normal { get; set; }
    TCat711FreezeCreateUpdateDto TCat711Freeze { get; set; }
    TCat711FrozenCreateUpdateDto TCat711Frozen { get; set; }
    private readonly IOrderInvoiceAppService _electronicInvoiceAppService;
    private readonly ITenantTripartiteRepository _electronicInvoiceSettingRepository;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private IStringLocalizer<PikachuResource> _L;

    private readonly IConfiguration _configuration;

    private readonly IGroupBuyAppService _GroupBuyAppService;

    private readonly IGroupBuyRepository _GroupBuyRepository;

    private readonly ITenantSettingsAppService _tenantSettingsAppService;

    private readonly IEmailSender _emailSender;
    private readonly IEmailAppService _emailAppService;
    private readonly OrderHistoryManager _orderHistoryManager;
    #endregion

    #region Constructor
    public StoreLogisticsOrderAppService(
        IOrderDeliveryRepository orderDeliveryRepository,
        IOrderRepository orderRepository,
        ILogisticsProvidersAppService logisticsProvidersAppService,
        IConfiguration configuration,
        IStringLocalizer<PikachuResource> L,
        IGroupBuyAppService GroupBuyAppService,
        IBackgroundJobManager backgroundJobManager,
        ITenantTripartiteRepository electronicInvoiceSettingRepository,
        IOrderInvoiceAppService electronicInvoiceAppService,
        IHttpContextAccessor httpContextAccessor,
        IGroupBuyRepository GroupBuyRepository,
        IEmailSender emailSender,
        ITenantSettingsAppService tenantSettingsAppService,
        IEmailAppService emailAppService,
        OrderHistoryManager orderHistoryManager
    )
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
        _L = L;
        _GroupBuyAppService = GroupBuyAppService;
        _backgroundJobManager = backgroundJobManager;
        _electronicInvoiceSettingRepository = electronicInvoiceSettingRepository;
        _electronicInvoiceAppService = electronicInvoiceAppService;
        _httpContextAccessor = httpContextAccessor;
        _GroupBuyRepository = GroupBuyRepository;
        _emailSender = emailSender;
        _tenantSettingsAppService = tenantSettingsAppService;
        _emailAppService = emailAppService;
        _orderHistoryManager = orderHistoryManager;
    }
    #endregion

    #region Methods
    public async Task IssueInvoiceAync(Guid orderId)
    {
        try
        {
            var order = await _orderRepository.GetAsync(orderId);

            // await _orderRepository.EnsurePropertyLoadedAsync(order, o => o.GroupBuy);
            var groupbuy = await _GroupBuyRepository.GetAsync(order.GroupBuyId);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.ToBeShipped;
           
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Delivery Update**
            await _orderHistoryManager.AddOrderHistoryAsync(
      order.Id,
      "OrderToBeShipped", // Localization key
      new object[] { _L[oldShippingStatus.ToString()].Name, _L[order.ShippingStatus.ToString()].Name }, // Localized placeholders
      currentUserId,
      currentUserName
  );

            var invoiceSetting = await _electronicInvoiceSettingRepository.FindByTenantAsync(CurrentUser.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.ToBeShipped)
            {

                if (groupbuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();

                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await _electronicInvoiceAppService.CreateInvoiceAsync(orderId);
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = orderId };
                        //var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogError(e.Message);
            Logger.LogWarning("Issue on Invoice Generation");
            return;

        }


    }
    public async Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
        
            ResponseResultDto result = new ResponseResultDto();
            try
            {
                var order = await _orderRepository.GetAsync(orderId);
                //await _orderRepository.EnsurePropertyLoadedAsync(order, o => o.GroupBuy);
                var orderDeliverys = await _deliveryRepository.GetWithDetailsAsync(orderId);

                var orderDelivery = orderDeliverys.FirstOrDefault(f => f.Id == orderDeliveryId);

                List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

                var greenWorld = providers.Where(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics).FirstOrDefault();
                if (greenWorld != null)
                {
                    GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
                }

                var homeDelivery = providers.Where(p => p.LogisticProvider is LogisticProviders.HomeDelivery).FirstOrDefault();
                if (homeDelivery != null)
                {
                    HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);
                }
                var postOffice = providers.Where(p => p.LogisticProvider is LogisticProviders.PostOffice).FirstOrDefault();
                if (postOffice != null)
                {
                    PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);
                }
                var sevenToEleven = providers.Where(p => p.LogisticProvider is LogisticProviders.SevenToEleven).FirstOrDefault();
                if (sevenToEleven != null)
                {
                    SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);
                }
                var familyMart = providers.Where(p => p.LogisticProvider is LogisticProviders.FamilyMart).FirstOrDefault();
                if (familyMart != null)
                {
                    FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);
                }
                var sevenToElevenFrozen = providers.Where(p => p.LogisticProvider is LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
                if (sevenToElevenFrozen != null)
                {
                    SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);
                }
                var bNormal = providers.Where(p => p.LogisticProvider is LogisticProviders.BNormal).FirstOrDefault();
                if (bNormal != null)
                {
                    BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);
                }
                var bFreeze = providers.Where(p => p.LogisticProvider is LogisticProviders.BFreeze).FirstOrDefault();
                if (bFreeze != null)
                {
                    BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
                }
                var bFrozen = providers.Where(p => p.LogisticProvider is LogisticProviders.BFrozen).FirstOrDefault();
                if (bFrozen != null)
                {
                    BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFrozen);
                }
                var options = new RestClientOptions
                {
                    MaxTimeout = -1,
                };

                RestClient client = new(options);

                RestRequest request = new(_configuration["EcPay:LogisticApi"], Method.Post);

                string marchentDate = DateTime.Now.ToString("yyyy/MM/dd");

                string receiverAddress = string.Concat(_L[order.City].Name, order.AddressDetails);

                HttpRequest? domainRequest = _httpContextAccessor?.HttpContext?.Request;

                string? domainName = $"{domainRequest?.Scheme}://{domainRequest?.Host.Value}";

                string serverReplyURL = $"{domainName}/api/app/orders/ecpay-logisticsStatus-callback";
            var merchantTrdaeNo = AddNumericSuffix(order.OrderNo);
                request.AddHeader("Accept", "text/html");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("MerchantID", GreenWorld.StoreCode);
                request.AddParameter("MerchantTradeDate", marchentDate);
                request.AddParameter("LogisticsType", "HOME");
                request.AddParameter("LogisticsSubType", orderDelivery.DeliveryMethod is DeliveryMethod.PostOffice || deliveryMethod is DeliveryMethod.PostOffice ? "POST" : "TCAT");
                request.AddParameter("GoodsAmount", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)));
                request.AddParameter("GoodsWeight", PostOffice.Weight);
                request.AddParameter("SenderName", GreenWorld.SenderName);
                request.AddParameter("SenderPhone", GreenWorld.SenderPhoneNumber);
                request.AddParameter("SenderZipCode", GreenWorld.SenderPostalCode);
                request.AddParameter("SenderAddress", GreenWorld.SenderAddress);
                request.AddParameter("ReceiverName", order.RecipientName);
                request.AddParameter("ReceiverCellPhone", order.RecipientPhone);
                request.AddParameter("ReceiverZipCode", order.PostalCode);
                request.AddParameter("ReceiverAddress", receiverAddress);
                request.AddParameter("ServerReplyURL", serverReplyURL);
                //request.AddParameter("ReceiverStoreID", "123");
                request.AddParameter("CheckMacValue", GenerateCheckMac(
                    greenWorld.HashKey, greenWorld.HashIV, GreenWorld.StoreCode, merchantTrdaeNo, marchentDate, "HOME", orderDelivery.DeliveryMethod is DeliveryMethod.PostOffice || deliveryMethod is DeliveryMethod.PostOffice ? "POST" : "TCAT", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), PostOffice.Weight, GreenWorld.SenderName, GreenWorld.SenderPhoneNumber,
                    GreenWorld.SenderPostalCode, GreenWorld.SenderAddress, order.RecipientName, order.RecipientPhone, order.PostalCode, receiverAddress, serverReplyURL));
                //request.AddParameter("IsCollection", "N");
                request.AddParameter("MerchantTradeNo", merchantTrdaeNo);

                RestResponse response = await client.ExecuteAsync(request);

                result = ParseApiResponse(response.Content.ToString());

                try
                {
                    if (result.ResponseCode is "1")
                    {
                    using (var uow = UnitOfWorkManager.Begin(
                  requiresNew: true, isTransactional: false
              ))
                    {

                        var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                        // Capture old delivery details before updating
                       
                        var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;
                        newOrderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;

                        newOrderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

                        newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                        if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                            deliveryMethod is not null)
                            newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                        await uow.SaveChangesAsync();
                        // **Get Current User (Editor)**
                        var currentUserId = CurrentUser.Id ?? Guid.Empty;
                        var currentUserName = CurrentUser.UserName ?? "System";

                        // **Log Order History for Delivery Update**
                        await _orderHistoryManager.AddOrderHistoryAsync(
     newOrderDelivery.OrderId,
     "GenerateDeliveryNumberLog", // Localization key
     new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
     currentUserId,
     currentUserName
 );

                        //order.ShippingStatus = ShippingStatus.ToBeShipped;
                        //order = await _orderRepository.GetAsync(orderId);

                        //await _orderRepository.UpdateAsync(order);

                    }

                    await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
                    }
                }
            catch (AbpDbConcurrencyException e)
            {
                Logger.LogError(e.Message);
                Logger.LogWarning("Issue on Home Delivery Shipment");
                if (result.ResponseCode is "1")
                {
                    using (var uow = UnitOfWorkManager.Begin(
                  requiresNew: true, isTransactional: false
              ))
                    {
                        var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                        newOrderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;

                        newOrderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

                        newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                        if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                            deliveryMethod is not null)
                            newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                        await uow.SaveChangesAsync();
                        //order.ShippingStatus = ShippingStatus.ToBeShipped;
                        //order = await _orderRepository.GetAsync(orderId);

                        //await _orderRepository.UpdateAsync(order);

                    }

                    await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
                }
               
            }
            return result;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("main catch block");
                Logger.LogException(ex);

                return result;
            }
        
    }

    public async Task<string> OnPrintShippingLabel(OrderDto order, OrderDeliveryDto orderDelivery)
    {
        RestClientOptions? options = new RestClientOptions { MaxTimeout = -1 };

        RestClient client = new(options);

        List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

        if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.PostOffice ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCat1 ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCatFrozen ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCatFreeze ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToEleven1 ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenFrozen ||
            orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMart1)
        {
            LogisticsProviderSettingsDto? greenWorld = providers.Where(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics).FirstOrDefault();

            if (greenWorld != null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);

            RestRequest request = new(_configuration["EcPay:PrintTradeDocument"], Method.Post);

            request.AddParameter("MerchantID", GreenWorld.StoreCode);
            request.AddParameter("AllPayLogisticsID", orderDelivery.AllPayLogisticsID);

            Dictionary<string, string> parameters = new()
            {
                { "MerchantID", GreenWorld.StoreCode },
                { "AllPayLogisticsID", orderDelivery.AllPayLogisticsID }
            };

            request.AddParameter("CheckMacValue", GenerateCheckMacValue(greenWorld!.HashKey, greenWorld!.HashIV, parameters));

            RestResponse response = await client.ExecuteAsync(request);

            string result = response.Content.ToString();

            return result;
        }

        else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C)
        {

        }

        else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C)
        {

        }

        else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                 orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                 orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen ||
                 orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                 orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                 orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
        {

        }

        return string.Empty;
    }

    public async Task<string> OnBatchPrintingShippingLabel(List<string> allPayLogisticsId)
    {
        string html = string.Empty;

        RestClientOptions? options = new RestClientOptions { MaxTimeout = -1 };

        RestClient client = new(options);

        List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

        LogisticsProviderSettingsDto? greenWorld = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics);

        if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);

        RestRequest request = new(_configuration["EcPay:PrintTradeDocument"], Method.Post);

        request.AddParameter("MerchantID", GreenWorld.StoreCode);
        request.AddParameter("AllPayLogisticsID", string.Join(", ", allPayLogisticsId));

        Dictionary<string, string> parameters = new()
        {
            { "MerchantID", GreenWorld.StoreCode },
            { "AllPayLogisticsID", string.Join(", ", allPayLogisticsId) }
        };

        request.AddParameter("CheckMacValue", GenerateCheckMacValue(greenWorld!.HashKey, greenWorld!.HashIV, parameters));

        RestResponse response = await client.ExecuteAsync(request);

        html = response.Content.ToString();

        html = html.Replace("/Content/Logistics/Helper/PrintTradeDocument.css?v=12", "https://logistics.ecpay.com.tw/Content/Logistics/Helper/PrintTradeDocument.css?v=12");
        html = html.Replace("/Scripts/jquery-1.4.4.min.js", "https://logistics.ecpay.com.tw/Scripts/jquery-1.4.4.min.js");
        html = html.Replace("/Scripts/Logistics/Helper/PrintTradeDocument.js?v=9", "https://logistics.ecpay.com.tw/Scripts/Logistics/Helper/PrintTradeDocument.js?v=9");

        return html;
    }

    private async Task<Tuple<List<string>, List<string>>> HandleTCatRequest(KeyValuePair<string, string> allPayLogisticsId, Dictionary<string, string>? DeliveryNumbers, string? valueOfTCat711)
    {
        List<string> pdfFilePath = []; List<string> messages = [];

        string[]? logisticsIdsArray = [];

        List<string> listOfTCat711Value = valueOfTCat711.IsNullOrEmpty() ? [] : [.. valueOfTCat711.Split(',')];

        List<string> deliveryNumbers = [];

        logisticsIdsArray = allPayLogisticsId.Value.Split(',');

        if (allPayLogisticsId.Key.Contains("TCatDeliveryNormal"))
        {
            deliveryNumbers.AddRange(DeliveryNumbers?.GetValueOrDefault("TCatDeliveryNormal")?.Split(','));
        }

        else if (allPayLogisticsId.Key.Contains("TCatDeliveryFreeze"))
        {
            deliveryNumbers.AddRange(DeliveryNumbers?.GetValueOrDefault("TCatDeliveryFreeze")?.Split(','));
        }

        else if (allPayLogisticsId.Key.Contains("TCatDeliveryFrozen"))
        {
            deliveryNumbers.AddRange(DeliveryNumbers?.GetValueOrDefault("TCatDeliveryFrozen")?.Split(','));
        }

        if (logisticsIdsArray != null)
        {
            foreach (string id in logisticsIdsArray)
            {
                List<DownloadOBTOrders> orders = [];

                if (allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenNormal") ||
                    allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenFreeze") ||
                    allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenFrozen"))
                {
                    orders.Add(new DownloadOBTOrders { OBTNumber = listOfTCat711Value[logisticsIdsArray.ToList().IndexOf(id)].Trim() });
                }

                else
                {
                    orders.Add(new DownloadOBTOrders { OBTNumber = deliveryNumbers[logisticsIdsArray.ToList().IndexOf(id)].Trim() });
                }

                DownloadOBTRequest requestBody = new()
                {
                    CustomerId = TCatLogistics.CustomerId,
                    CustomerToken = TCatLogistics.CustomerToken,
                    FileNo = id,
                    Orders = orders
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);

                StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

                using HttpClient httpClient = new();

                HttpResponseMessage tCatResponse = await httpClient.PostAsync(_configuration["T-Cat:DownloadOBT"], content);

                string? contentType = tCatResponse.Content.Headers.ContentType?.MediaType;

                if (tCatResponse.IsSuccessStatusCode && contentType == "application/pdf")
                {
                    string tempFolder = Path.Combine(Path.GetTempPath(), "MergeTemp");

                    string fileName = $"TCatInvoice_{Guid.NewGuid().ToString().Split('-')[0]}.pdf";

                    string filePath = Path.Combine(tempFolder, fileName);

                    using (Stream responseStream = await tCatResponse.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }

                    pdfFilePath.Add(filePath);
                }

                else if (contentType == "application/json")
                {
                    string responseContent = await tCatResponse.Content.ReadAsStringAsync();

                    DownloadOBTResponse? downloadObtResponse = JsonConvert.DeserializeObject<DownloadOBTResponse>(responseContent);

                    Guid OrderId = await _deliveryRepository.GetOrderIdByAllPayLogisticsId(id);

                    string OrderNo = await _orderRepository.GetOrderNoByOrderId(OrderId);

                    messages.Add(string.Concat($"({OrderNo}) ", downloadObtResponse?.Message) ?? string.Empty);
                }
            }
        }

        return Tuple.Create(pdfFilePath, messages);
    }

    public async Task<Tuple<List<string>, List<string>, List<string>>> OnBatchPrintingShippingLabel(
        Dictionary<string, string> allPayLogisticsIds,
        Dictionary<string, string>? DeliveryNumbers,
        Dictionary<string, string>? allPayLogisticsForTCat711)
    {
        List<string> htmls = []; List<string> errors = []; List<string> preDefinedPdfPaths = [];

        List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

        LogisticsProviderSettingsDto? greenWorld = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics);

        if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);

        LogisticsProviderSettingsDto? greenWorldC2C = providers.Where(p => p.LogisticProvider == LogisticProviders.GreenWorldLogisticsC2C).FirstOrDefault();

        if (greenWorldC2C is not null) GreenWorldC2C = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorldC2C);

        LogisticsProviderSettingsDto? tCat = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat);

        if (tCat is not null) TCatLogistics = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatLogisticsCreateUpdateDto>(tCat);

        foreach (KeyValuePair<string, string> allPayLogisticsId in allPayLogisticsIds)
        {
            if (allPayLogisticsId.Value.IsNullOrEmpty()) continue;

            RestClientOptions? options = new() { MaxTimeout = -1 };

            RestClient client = new(options);

            RestRequest request = new();

            Dictionary<string, string> parameters = [];

            if (allPayLogisticsId.Key.Contains("TCatDeliveryNormal") ||
                allPayLogisticsId.Key.Contains("TCatDeliveryFreeze") ||
                allPayLogisticsId.Key.Contains("TCatDeliveryFrozen") ||
                allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenNormal") ||
                allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenFreeze") ||
                allPayLogisticsId.Key.Contains("TCatDeliverySevenElevenFrozen"))
            {
                Tuple<List<string>, List<string>> tCatTuple = await HandleTCatRequest(
                    allPayLogisticsId,
                    DeliveryNumbers,
                    allPayLogisticsForTCat711.GetValueOrDefault(allPayLogisticsId.Key)
                );

                if (tCatTuple.Item1 is { Count: > 0 }) preDefinedPdfPaths.AddRange(tCatTuple.Item1);

                if (tCatTuple.Item2 is { Count: > 0 }) errors.AddRange(tCatTuple.Item2);

                continue;
            }

            else if (allPayLogisticsId.Key.Contains("SevenToEleven1") ||
                     allPayLogisticsId.Key.Contains("PostOffice") ||
                     allPayLogisticsId.Key.Contains("BlackCat1") ||
                     allPayLogisticsId.Key.Contains("BlackCatFreeze") ||
                     allPayLogisticsId.Key.Contains("BlackCatFrozen") ||
                     allPayLogisticsId.Key.Contains("FamilyMart1") ||
                     allPayLogisticsId.Key.Contains("SevenToElevenFrozen"))
            {
                request = new(_configuration["EcPay:PrintTradeDocument"], Method.Post);

                request.AddParameter("MerchantID", GreenWorld.StoreCode);
                request.AddParameter("AllPayLogisticsID", allPayLogisticsId.Value);

                parameters = new()
                {
                    { "MerchantID", GreenWorld.StoreCode },
                    { "AllPayLogisticsID", allPayLogisticsId.Value }
                };

                request.AddParameter("CheckMacValue", GenerateCheckMacValue(greenWorld!.HashKey, greenWorld!.HashIV, parameters));
            }

            else if (allPayLogisticsId.Key.Contains("FamilyMartC2C"))
            {
                request = new(_configuration["EcPay:PrintFAMIC2COrderInfo"], Method.Post);

                request.AddParameter("MerchantID", GreenWorldC2C.StoreCode);
                request.AddParameter("AllPayLogisticsID", allPayLogisticsId.Value);
                request.AddParameter("CVSPaymentNo", DeliveryNumbers.GetValueOrDefault("FamilyMartC2C"));


                parameters = new()
                {
                    { "MerchantID", GreenWorldC2C.StoreCode },
                    { "AllPayLogisticsID", allPayLogisticsId.Value},
                    { "CVSPaymentNo", DeliveryNumbers.GetValueOrDefault("FamilyMartC2C") }
                };

                request.AddParameter("CheckMacValue", GenerateCheckMacValue(greenWorldC2C!.HashKey, greenWorldC2C!.HashIV, parameters));
            }

            RestResponse response = await client.ExecuteAsync(request);

            if (response.Content!.ToString().Contains("alert("))
            {
                Guid OrderId = await _deliveryRepository.GetOrderIdByAllPayLogisticsId(allPayLogisticsId.Value);

                string OrderNo = await _orderRepository.GetOrderNoByOrderId(OrderId);

                errors.Add($"({OrderNo}) Cannot Generate Pdf, Please contact site owner.\n");
            }

            else
            {
                string html = response.Content!.ToString();

                html = html.Replace("div class=\"PrintToolsBlock\"", "div class=\"PrintToolsBlock\" hidden");

                if (allPayLogisticsId.Key.Contains("SevenToEleven1") ||
                    allPayLogisticsId.Key.Contains("PostOffice") ||
                    allPayLogisticsId.Key.Contains("BlackCat1") ||
                    allPayLogisticsId.Key.Contains("BlackCatFreeze") ||
                    allPayLogisticsId.Key.Contains("BlackCatFrozen") ||
                    allPayLogisticsId.Key.Contains("FamilyMart1") ||
                    allPayLogisticsId.Key.Contains("SevenToElevenFrozen"))
                {
                    html = html.Replace("/Content/Logistics/Helper/PrintTradeDocument.css?v=12", "https://logistics.ecpay.com.tw/Content/Logistics/Helper/PrintTradeDocument.css?v=12");
                    html = html.Replace("/Scripts/jquery-1.4.4.min.js", "https://logistics.ecpay.com.tw/Scripts/jquery-1.4.4.min.js");
                    html = html.Replace("/Scripts/Logistics/Helper/PrintTradeDocument.js?v=9", "https://logistics.ecpay.com.tw/Scripts/Logistics/Helper/PrintTradeDocument.js?v=9");
                    html = html.Replace("<body class=\"PrintBody\">", "<script>\r\n    document.querySelectorAll('img[data-src]').forEach(img => {\r\n        img.src = img.getAttribute('data-src');\r\n    });\r\n</script> \n <body class=\"PrintBody\">");
                }

                else if (allPayLogisticsId.Key.Contains("FamilyMartC2C"))
                {
                    html = html.Replace("/Scripts/jquery-1.4.4.min.js", "https://logistics.ecpay.com.tw/Scripts/jquery-1.4.4.min.js");
                    html = html.Replace("/Scripts/jquery-ui.min.js", "https://logistics.ecpay.com.tw/Scripts/jquery-ui.min.js");
                    html = html.Replace("/Scripts/jquery.blockUI.js", "https://logistics.ecpay.com.tw/Scripts/jquery.blockUI.js");
                }

                htmls.Add(html);
            }
        }

        return Tuple.Create(htmls, preDefinedPdfPaths, errors);
    }

    public async Task<Dictionary<string, string>> OnSevenElevenC2CShippingLabelAsync(
        Dictionary<string, string> allPayLogisticsIds,
        Dictionary<string, string>? DeliveryNumbers)
    {
        List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();
        LogisticsProviderSettingsDto? greenWorldC2C = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.GreenWorldLogisticsC2C);

        if (greenWorldC2C is not null)
        {
            GreenWorldC2C = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorldC2C);
        }

        KeyValuePair<string, string> allPayLogisticsId = allPayLogisticsIds.FirstOrDefault(f => f.Key.Contains("SevenToElevenC2C"));

        Dictionary<string, string> parameters = [];

        string cvsPaymentNo = string.Join(",", DeliveryNumbers?.GetValueOrDefault("SevenToElevenC2C")?
                                    .Split(',')
                                    .Select(number => number.Remove(number.Length - 4)));

        string cvsValidationNo = string.Join(",", DeliveryNumbers?.GetValueOrDefault("SevenToElevenC2C")?
                                    .Split(',')
                                    .Select(number => number.Substring(number.Length - 4)));

        parameters = new Dictionary<string, string>
        {
            { "MerchantID", GreenWorldC2C.StoreCode },
            { "AllPayLogisticsID", allPayLogisticsId.Value },
            { "CVSPaymentNo", cvsPaymentNo },
            { "CVSValidationNo", cvsValidationNo }
        };

        parameters["CheckMacValue"] = GenerateCheckMacValue(greenWorldC2C?.HashKey, greenWorldC2C?.HashIV, parameters);
        parameters["ActionUrl"] = _configuration["EcPay:PrintUniMartC2COrderInfo"] ?? string.Empty;

        return parameters;
    }

    public void MapAllLogistics(List<LogisticsProviderSettingsDto> providers)
    {
        LogisticsProviderSettingsDto? greenWorld = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics);

        if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);

        LogisticsProviderSettingsDto? homeDelivery = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.HomeDelivery);

        if (homeDelivery is not null) HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);

        LogisticsProviderSettingsDto? postOffice = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.PostOffice);

        if (postOffice is not null) PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);

        LogisticsProviderSettingsDto? sevenToEleven = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.SevenToEleven);

        if (sevenToEleven is not null) SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);

        LogisticsProviderSettingsDto? familyMart = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.FamilyMart);

        if (familyMart is not null) FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);

        LogisticsProviderSettingsDto? sevenToElevenFrozen = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.SevenToElevenFrozen);

        if (sevenToElevenFrozen is not null) SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);

        LogisticsProviderSettingsDto? bNormal = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.BNormal);

        if (bNormal is not null) BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);

        LogisticsProviderSettingsDto? bFreeze = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.BFreeze);

        if (bFreeze is not null) BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);

        LogisticsProviderSettingsDto? bFrozen = providers.FirstOrDefault(p => p.LogisticProvider is LogisticProviders.BFrozen);

        if (bFrozen is not null) BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFrozen);
    }

    public async Task<PrintObtResponse?> GenerateDeliveryNumberForTCatDeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
       
            var order = await _orderRepository.GetAsync(orderId);

            List<OrderDelivery> orderDeliveries = await _deliveryRepository.GetWithDetailsAsync(orderId);

            OrderDelivery orderDelivery = orderDeliveries.First(f => f.Id == orderDeliveryId);

            var groupBuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);

            List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

            LogisticsProviderSettingsDto? tCat = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat);

            if (tCat is not null) TCatLogistics = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatLogisticsCreateUpdateDto>(tCat);

            LogisticsProviderSettingsDto? tCatNormal = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatNormal);

            if (tCatNormal is not null) TCatNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatNormalCreateUpdateDto>(tCatNormal);

            LogisticsProviderSettingsDto? tCatFreeze = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatFreeze);

            if (tCatFreeze is not null) TCatFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatFreezeCreateUpdateDto>(tCatFreeze);

            LogisticsProviderSettingsDto? tCatFrozen = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatFrozen);

            if (tCatFrozen is not null) TCatFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatFrozenCreateUpdateDto>(tCatFrozen);

            string thermosphere = string.Empty; string spec = string.Empty; string isSwipe = string.Empty;

            string isCollection = string.Empty; int collectionAmount = 0; string deliveryTime = string.Empty;

            if (orderDelivery.Items is { Count: > 0 } &&
                orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Normal)
            {
                thermosphere = "0001";

                spec = GetSpec(TCatNormal.Size);

                isCollection = TCatNormal.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ? "Y" : "N";

                collectionAmount = TCatNormal.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ?
                                    GetCollectionAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) :
                                    0;

                isSwipe = TCatNormal.Payment && TCatNormal.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";

                deliveryTime = order.ReceivingTimeNormal switch
                {
                    ReceivingTime.Before13PM => "01",
                    ReceivingTime.Between14To18PM => "02",
                    _ => "04"
                };
            }

            else if (orderDelivery.Items is { Count: > 0 } &&
                     orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Freeze)
            {
                thermosphere = "0002";

                spec = GetSpec(TCatFreeze.Size);

                isCollection = TCatFreeze.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ? "Y" : "N";

                collectionAmount = TCatFreeze.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ?
                                    GetCollectionAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) :
                                    0;

                isSwipe = TCatFreeze.Payment && TCatFreeze.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";

                deliveryTime = order.ReceivingTimeFreeze switch
                {
                    ReceivingTime.Before13PM => "01",
                    ReceivingTime.Between14To18PM => "02",
                    _ => "04"
                };
            }

            else if (orderDelivery.Items is { Count: > 0 } &&
                     orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Frozen)
            {
                thermosphere = "0003";

                spec = GetSpec(TCatFrozen.Size);

                isCollection = TCatFrozen.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ? "Y" : "N";

                collectionAmount = TCatFrozen.Payment && order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment ?
                                    GetCollectionAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) :
                                    0;

                isSwipe = TCatFrozen.Payment && TCatFrozen.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";

                deliveryTime = order.ReceivingTimeFrozen switch
                {
                    ReceivingTime.Before13PM => "01",
                    ReceivingTime.Between14To18PM => "02",
                    _ => "04"
                };
            }

            //if (order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment)
            //    isCollection = "Y";

            string isDeclare = TCatLogistics.DeclaredValue &&
                               IsOrderAmountValid(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) ? "Y" : "N";

            DayOfWeek TodaysDay = DateTime.Today.DayOfWeek;
        var newOrderNo = AddNumericSuffix(order.OrderNo);
        PrintOBTRequest request = new()
            {
                CustomerId = TCatLogistics.CustomerId,
                CustomerToken = TCatLogistics.CustomerToken,
                PrintType = "01",
                PrintOBTType = $"0{(int)TCatLogistics.TCatShippingLabelForm}",
                Orders =
                [
                    new OrderOBT
                {
                    OBTNumber = string.Empty,
                    OrderId = newOrderNo,
                    Thermosphere = thermosphere,
                    Spec = spec,
                    ReceiptLocation = "01",
                    ReceiptStationNo = string.Empty,
                    RecipientName = order.RecipientName ?? string.Empty,
                    RecipientTel = string.Empty,
                    RecipientMobile = order.RecipientPhone ?? string.Empty,
                    RecipientAddress = $"{_L[order.City]}{order.AddressDetails}",
                    SenderName = TCatLogistics.SenderName,
                    SenderTel = string.Empty,
                    SenderMobile = TCatLogistics.SenderPhoneNumber,
                    SenderZipCode = TCatLogistics.SenderPostalCode,
                    SenderAddress = TCatLogistics.SenderAddress,
                    ShipmentDate = TodaysDay is DayOfWeek.Saturday ? DateTime.Now.AddDays(2).ToString("yyyyMMdd") : DateTime.Now.AddDays(1).ToString("yyyyMMdd"),
                    DeliveryDate = TodaysDay is DayOfWeek.Friday || TodaysDay is DayOfWeek.Saturday ? DateTime.Now.AddDays(3).ToString("yyyyMMdd") : DateTime.Now.AddDays(2).ToString("yyyyMMdd"),
                    DeliveryTime = deliveryTime,
                    IsFreight = "N",
                    IsCollection = isCollection,
                    CollectionAmount = collectionAmount,
                    IsSwipe = isSwipe,
                    IsMobilePay = isSwipe,
                    IsDeclare = isDeclare,
                    DeclareAmount = isDeclare is "Y" ? GetDeclareAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) : 0,
                    ProductTypeId = GetProductType(groupBuy.ProductType),
                    ProductName = GetProductName(groupBuy.GroupBuyName),
                    Memo = string.Empty
                }
                ]
            };

            string jsonContent = JsonConvert.SerializeObject(request);

            StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

            using HttpClient httpClient = new();

            HttpResponseMessage response = await httpClient.PostAsync(_configuration["T-Cat:PrintOBT"], content);

            string responseContent = await response.Content.ReadAsStringAsync();

            PrintObtResponse? printObtResponse = JsonConvert.DeserializeObject<PrintObtResponse>(responseContent);

            if (printObtResponse is null || printObtResponse.Data is null) return printObtResponse;

            try
            {
            using (var uow = UnitOfWorkManager.Begin(
               requiresNew: true, isTransactional: false
           ))
            {
                var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                newOrderDelivery.SrvTranId = printObtResponse.SrvTranId;
                newOrderDelivery.FileNo = printObtResponse.Data.FileNo;
                newOrderDelivery.AllPayLogisticsID = printObtResponse.SrvTranId;
                newOrderDelivery.LastModificationTime = DateTime.ParseExact(printObtResponse.Data.PrintDateTime,
                                                                         "yyyyMMddHHmmss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                newOrderDelivery.DeliveryNo = printObtResponse.Data.Orders.First().OBTNumber;
                var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;
                newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                    newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                //order.ShippingStatus = ShippingStatus.ToBeShipped;
                await uow.SaveChangesAsync();
                //await _orderRepository.UpdateAsync(order);
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Delivery Update**
                await _orderHistoryManager.AddOrderHistoryAsync(
      newOrderDelivery.OrderId,
      "GenerateDeliveryNumberLog", // Localization key
      new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
      currentUserId,
      currentUserName
  );

            }
                await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
            }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogError(e.Message);
            Logger.LogWarning("Issue on Delivery Number For TCat");
            using (var uow = UnitOfWorkManager.Begin(
             requiresNew: true, isTransactional: false
         ))
            {
                var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                newOrderDelivery.SrvTranId = printObtResponse.SrvTranId;
                newOrderDelivery.FileNo = printObtResponse.Data.FileNo;
                newOrderDelivery.AllPayLogisticsID = printObtResponse.SrvTranId;
                newOrderDelivery.LastModificationTime = DateTime.ParseExact(printObtResponse.Data.PrintDateTime,
                                                                         "yyyyMMddHHmmss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                newOrderDelivery.DeliveryNo = printObtResponse.Data.Orders.First().OBTNumber;
                var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;
                newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                    newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                //order.ShippingStatus = ShippingStatus.ToBeShipped;
                await uow.SaveChangesAsync();
                //await _orderRepository.UpdateAsync(order);
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Delivery Update**
                await _orderHistoryManager.AddOrderHistoryAsync(
     newOrderDelivery.OrderId,
     "GenerateDeliveryNumberLog", // Localization key
     new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
     currentUserId,
     currentUserName
 );
            }
            await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
            return printObtResponse;

        }

        return printObtResponse;
        
    }

    public async Task<PrintOBTB2SResponse?> GenerateDeliveryNumberForTCat711DeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
       
            var order = await _orderRepository.GetAsync(orderId);

            List<OrderDelivery> orderDeliveries = await _deliveryRepository.GetWithDetailsAsync(orderId);

            OrderDelivery orderDelivery = orderDeliveries.First(f => f.Id == orderDeliveryId);

            var groupBuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);

            List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

            LogisticsProviderSettingsDto? tCat = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat);

            if (tCat is not null) TCatLogistics = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatLogisticsCreateUpdateDto>(tCat);

            LogisticsProviderSettingsDto? tCat711Normal = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Normal);

            if (tCat711Normal is not null) TCat711Normal = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711NormalCreateUpdate>(tCat711Normal);

            LogisticsProviderSettingsDto? tCat711Freeze = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Freeze);

            if (tCat711Freeze is not null) TCat711Freeze = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711FreezeCreateUpdateDto>(tCat711Freeze);

            LogisticsProviderSettingsDto? tCat711Frozen = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Frozen);

            if (tCat711Frozen is not null) TCat711Frozen = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711FrozenCreateUpdateDto>(tCat711Frozen);

            string thermosphere = string.Empty;

            if (orderDelivery.Items is { Count: > 0 } &&
                orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Normal) thermosphere = "0001";

            else if (orderDelivery.Items is { Count: > 0 } &&
                     orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Freeze) thermosphere = "0002";

            else if (orderDelivery.Items is { Count: > 0 } &&
                     orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Frozen) thermosphere = "0003";

            int collectionAmount = order.PaymentMethod is PaymentMethods.CashOnDelivery ?
                                    GetCollectionAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost, true) :
                                    0;
        var newOrderNo = AddNumericSuffix(order.OrderNo);
            PrintOBTB2SRequest request = new()
            {
                CustomerId = TCatLogistics.CustomerId,
                CustomerToken = TCatLogistics.CustomerToken,
                PrintType = "01",
                PrintOBTType = $"0{(int)TCatLogistics.TCatShippingLabelForm711}",
                Orders =
                [
                    new OrderOBTB2S
                {
                    OBTNumber = string.Empty,
                    OrderId = newOrderNo,
                    Thermosphere = thermosphere,
                    Spec = "0003",
                    ReceiveStoreId = order.StoreId ?? string.Empty,
                    RecipientName = order.RecipientName ?? string.Empty,
                    RecipientTel = string.Empty,
                    RecipientMobile = order.RecipientPhone ?? string.Empty,
                    SenderName = TCatLogistics.SenderName,
                    SenderTel = string.Empty,
                    SenderMobile = TCatLogistics.SenderPhoneNumber,
                    SenderZipCode = TCatLogistics.SenderPostalCode,
                    SenderAddress = TCatLogistics.SenderAddress,
                    IsCollection = order.PaymentMethod is PaymentMethods.CashOnDelivery ? "Y" : "N",
                    CollectionAmount = collectionAmount,
                    Memo = string.Empty
                }
                ]
            };

            string jsonContent = JsonConvert.SerializeObject(request);

            StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

            using HttpClient httpClient = new();

            HttpResponseMessage response = await httpClient.PostAsync(_configuration["T-Cat:PrintOBTByB2S"], content);

            string responseContent = await response.Content.ReadAsStringAsync();

            PrintOBTB2SResponse? printObtB2SResponse = JsonConvert.DeserializeObject<PrintOBTB2SResponse>(responseContent);

            if (printObtB2SResponse is null || printObtB2SResponse.Data is null) return printObtB2SResponse;

            try
            {
            using (var uow = UnitOfWorkManager.Begin(
               requiresNew: true, isTransactional: false
           ))
            {
                var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                newOrderDelivery.SrvTranId = printObtB2SResponse.SrvTranId;
                newOrderDelivery.FileNo = printObtB2SResponse.Data.FileNo;
                newOrderDelivery.AllPayLogisticsID = printObtB2SResponse.Data.Orders.First().OBTNumber;
                newOrderDelivery.LastModificationTime = DateTime.ParseExact(printObtB2SResponse.Data.PrintDateTime,
                                                                         "yyyyMMddHHmmss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                newOrderDelivery.DeliveryNo = printObtB2SResponse.Data.Orders.First().DeliveryId;
                newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;
                var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;

                if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                    newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                //order.ShippingStatus = ShippingStatus.ToBeShipped;
                await uow.SaveChangesAsync();
                //await _orderRepository.UpdateAsync(order);
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Delivery Update**
                await _orderHistoryManager.AddOrderHistoryAsync(
    newOrderDelivery.OrderId,
    "GenerateDeliveryNumberLog", // Localization key
    new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
    currentUserId,
    currentUserName
);
            }

                await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
            }
            catch (AbpDbConcurrencyException e)
            {
            Logger.LogError(e.Message);
            Logger.LogWarning("Issue on Delivery Number For TCat 711");
            using (var uow = UnitOfWorkManager.Begin(
            requiresNew: true, isTransactional: false
        ))
            {
                var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                newOrderDelivery.SrvTranId = printObtB2SResponse.SrvTranId;
                newOrderDelivery.FileNo = printObtB2SResponse.Data.FileNo;
                newOrderDelivery.AllPayLogisticsID = printObtB2SResponse.Data.Orders.First().OBTNumber;
                newOrderDelivery.LastModificationTime = DateTime.ParseExact(printObtB2SResponse.Data.PrintDateTime,
                                                                         "yyyyMMddHHmmss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                newOrderDelivery.DeliveryNo = printObtB2SResponse.Data.Orders.First().DeliveryId;
                newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;
                var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;

                if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                    newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                //order.ShippingStatus = ShippingStatus.ToBeShipped;
                await uow.SaveChangesAsync();
                //await _orderRepository.UpdateAsync(order);
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Delivery Update**
                await _orderHistoryManager.AddOrderHistoryAsync(
    newOrderDelivery.OrderId,
    "GenerateDeliveryNumberLog", // Localization key
    new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
    currentUserId,
    currentUserName
);
            }

            await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
            return printObtB2SResponse;

        }

            return printObtB2SResponse;
        
    }

    public async Task GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(Guid orderId, Guid orderDeliveryId)
    {

        Order order = await _orderRepository.GetWithDetailsAsync(orderId);

        List<OrderDelivery> orderDeliveries = await _deliveryRepository.GetWithDetailsAsync(orderId);

        OrderDelivery orderDelivery = orderDeliveries.First(f => f.Id == orderDeliveryId);

        orderDelivery.DeliveryNo = order.OrderNo;
    var oldDeliveryStatus = orderDelivery.DeliveryStatus;
        orderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

        await _deliveryRepository.UpdateAsync(orderDelivery);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Delivery Update**
        await _orderHistoryManager.AddOrderHistoryAsync(
     orderDelivery.OrderId,
     "GenerateDeliveryNumberLog", // Localization key
     new object[] { orderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
     currentUserId,
     currentUserName
 );

        if (orderDeliveries.All(a => a.DeliveryStatus == DeliveryStatus.ToBeShipped))
        {
            //order.ShippingStatus = ShippingStatus.ToBeShipped;

            await _orderRepository.UpdateAsync(order);
        }

        await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
    }

    public bool IsOrderAmountValid(decimal? totalAmount, decimal? deliveryCost)
    {
        decimal totalAmountValue = totalAmount is not null ? totalAmount.Value : 0.00m;

        decimal deliveryCostValue = deliveryCost is not null ? deliveryCost.Value : 0.00m;

        decimal totalValue = totalAmountValue + deliveryCostValue;

        return totalValue > 20000;
    }

    public int GetDeclareAmount(decimal? totalAmount, decimal? deliveryCost)
    {
        decimal totalAmountValue = totalAmount is not null ? totalAmount.Value : 0.00m;

        decimal deliveryCostValue = deliveryCost is not null ? deliveryCost.Value : 0.00m;

        decimal totalValue = totalAmountValue + deliveryCostValue;

        totalValue = totalValue.IsBetween(20000, 100001) ? totalValue : 0;

        return (int)totalValue;
    }

    public int GetCollectionAmount(decimal? totalAmount, decimal? deliveryCost, bool isFor711 = false)
    {
        decimal totalAmountValue = totalAmount is not null ? totalAmount.Value : 0.00m;

        decimal deliveryCostValue = deliveryCost is not null ? deliveryCost.Value : 0.00m;

        decimal totalValue = totalAmountValue + deliveryCostValue;

        totalValue = totalValue.IsBetween(0, isFor711 ? 20000 : 100001) ? totalValue : 0;

        return (int)totalValue;
    }

    public string GetProductName(string groupBuyName)
    {
        if (groupBuyName.Length <= 20) return groupBuyName;

        return groupBuyName[..20];
    }

    public string GetProductType(ProductType? type)
    {
        return type switch
        {
            ProductType.GeneralFood => "0001",
            ProductType.SpecialityProductsSweets => "0002",
            ProductType.AlcoholOilVinegarSauces => "0003",
            ProductType.GrainsVegetablesFruits => "0004",
            ProductType.SeafoodMeatProducts => "0005",
            ProductType.ThreeCConsumerElectronics => "0006",
            ProductType.HomeAppliances => "0007",
            ProductType.ClothingandAccessories => "0008",
            ProductType.HouseholdItems => "0009",
            ProductType.BeautyandCosmetics => "0010",
            ProductType.HealthSupplements => "0011",
            ProductType.MedicalSupplies => "0012",
            ProductType.PetSuppliesandFeed => "0013",
            ProductType.PrintedMaterials => "0014",
            ProductType.Others => "0015",
            _ => string.Empty
        };
    }

    public string GetSpec(SizeEnum size)
    {
        return size switch
        {
            SizeEnum.Cm0001 => "0001",
            SizeEnum.Cm0002 => "0002",
            SizeEnum.Cm0003 => "0003",
            SizeEnum.Cm0004 => "0004",
            _ => string.Empty,
        };
    }

    public async Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
      
            ResponseResultDto result = new();

            var order = await _orderRepository.GetAsync(orderId);
        var groupbuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);
        List<OrderDelivery> orderDeliverys = await _deliveryRepository.GetWithDetailsAsync(orderId);

            OrderDelivery? orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();

            List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

            string logisticSubType = string.Empty;

            string isCollection = string.Empty;

            if (orderDelivery is not null &&
                (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                 orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMartC2C ||
                 deliveryMethod is DeliveryMethod.FamilyMartC2C ||
                 deliveryMethod is DeliveryMethod.SevenToElevenC2C)
            )
            {
                LogisticsProviderSettingsDto? greenWorld = providers.Where(p => p.LogisticProvider == LogisticProviders.GreenWorldLogisticsC2C).FirstOrDefault();

                if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            else
            {
                LogisticsProviderSettingsDto? greenWorld = providers.Where(p => p.LogisticProvider == LogisticProviders.GreenWorldLogistics).FirstOrDefault();

                if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            LogisticsProviderSettingsDto? homeDelivery = providers.Where(p => p.LogisticProvider == LogisticProviders.HomeDelivery).FirstOrDefault();

            if (homeDelivery is not null) HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);

            LogisticsProviderSettingsDto? postOffice = providers.Where(p => p.LogisticProvider == LogisticProviders.PostOffice).FirstOrDefault();

            if (postOffice is not null) PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);

            LogisticsProviderSettingsDto? sevenToEleven = providers.Where(p => p.LogisticProvider == LogisticProviders.SevenToEleven).FirstOrDefault();

            if (sevenToEleven is not null) SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);

            LogisticsProviderSettingsDto? familyMart = providers.Where(p => p.LogisticProvider == LogisticProviders.FamilyMart).FirstOrDefault();

            if (familyMart is not null) FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);

            LogisticsProviderSettingsDto? sevenToElevenFrozen = providers.Where(p => p.LogisticProvider == LogisticProviders.SevenToElevenFrozen).FirstOrDefault();

            if (sevenToElevenFrozen is not null) SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);

            LogisticsProviderSettingsDto? bNormal = providers.Where(p => p.LogisticProvider == LogisticProviders.BNormal).FirstOrDefault();

            if (bNormal is not null) BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);

            LogisticsProviderSettingsDto? bFreeze = providers.Where(p => p.LogisticProvider == LogisticProviders.BFreeze).FirstOrDefault();

            if (bFreeze is not null) BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);

            LogisticsProviderSettingsDto? bFrozen = providers.Where(p => p.LogisticProvider == LogisticProviders.BFrozen).FirstOrDefault();

            if (bFrozen is not null) BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);

            RestClientOptions options = new() { MaxTimeout = -1 };

            RestClient client = new(options);

            RestRequest request = new(_configuration["EcPay:LogisticApi"], Method.Post);

            string marchentDate = DateTime.Now.ToString("yyyy/MM/dd");

            string validatePattern = @"[\^'`!@#%&*+\\\""<>|_\[\]]";

            string goodsName = groupbuy.GroupBuyName;

            goodsName = Regex.Replace(goodsName, validatePattern, "");
        var merchantTrdaeNo = AddNumericSuffix(order.OrderNo);
            HttpRequest? domainRequest = _httpContextAccessor?.HttpContext?.Request;

            string? domainName = $"{domainRequest?.Scheme}://{domainRequest?.Host.Value}";

            string serverReplyURL = $"{domainName}/api/app/orders/ecpay-logisticsStatus-callback";

            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", GreenWorld.StoreCode);
            request.AddParameter("MerchantTradeDate", marchentDate);
            request.AddParameter("LogisticsType", "CVS");

            if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToEleven1 ||
                (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore && deliveryMethod is DeliveryMethod.SevenToEleven1))
            {
                request.AddParameter("LogisticsSubType", "UNIMART");

                logisticSubType = "UNIMART";
            }
            else if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
                     (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore && deliveryMethod is DeliveryMethod.SevenToElevenFrozen))
            {
                request.AddParameter("LogisticsSubType", "UNIMARTFREEZE");

                logisticSubType = "UNIMARTFREEZE";
            }
            else if (orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMart1 ||
                    (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore && deliveryMethod is DeliveryMethod.FamilyMart1))
            {
                request.AddParameter("LogisticsSubType", "FAMI");
                logisticSubType = "FAMI";
            }
            else if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                     deliveryMethod is DeliveryMethod.SevenToElevenC2C)
            {
                request.AddParameter("LogisticsSubType", "UNIMARTC2C");
                logisticSubType = "UNIMARTC2C";

                request.AddParameter("GoodsName", goodsName);
                request.AddParameter("SenderCellPhone", GreenWorld.SenderPhoneNumber);
            }
            else if (orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMartC2C ||
                     deliveryMethod is DeliveryMethod.FamilyMartC2C)
            {
                request.AddParameter("LogisticsSubType", "FAMIC2C");
                logisticSubType = "FAMIC2C";
                request.AddParameter("GoodsName", goodsName);
                request.AddParameter("SenderCellPhone", GreenWorld.SenderPhoneNumber);
            }
            request.AddParameter("GoodsAmount", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)));
            request.AddParameter("SenderName", GreenWorld.SenderName);
            request.AddParameter("ReceiverName", order.RecipientName);
            request.AddParameter("ReceiverCellPhone", order.RecipientPhone);
            request.AddParameter("ServerReplyURL", serverReplyURL);
            request.AddParameter("ReceiverStoreID", order.StoreId);

            if (order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment)
            {
                request.AddParameter("IsCollection", "Y");
                isCollection = "Y";
            }

            if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMartC2C ||
                deliveryMethod is DeliveryMethod.FamilyMartC2C ||
                deliveryMethod is DeliveryMethod.SevenToElevenC2C)
            {
                request.AddParameter("CheckMacValue", GenerateRequestString(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, merchantTrdaeNo, marchentDate, "CVS", logisticSubType, Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), GreenWorld.SenderName, order.RecipientName, order.RecipientPhone,
                    serverReplyURL, order.StoreId, goodsName, GreenWorld.SenderPhoneNumber, isCollection: isCollection));
            }
            else
            {
                request.AddParameter("CheckMacValue", GenerateRequestString(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, merchantTrdaeNo, marchentDate, "CVS", logisticSubType, Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), GreenWorld.SenderName, order.RecipientName, order.RecipientPhone,
                        serverReplyURL, order.StoreId, isCollection: isCollection));
            }

            request.AddParameter("MerchantTradeNo", merchantTrdaeNo);

            RestResponse response = await client.ExecuteAsync(request);

            result = ParseApiResponse(response.Content?.ToString());
            try
            {
                if (result.ResponseCode is "1")
                {
                using (var uow = UnitOfWorkManager.Begin(
              requiresNew: true, isTransactional: false
          ))
                {
                    var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                    newOrderDelivery.DeliveryNo = result.ShippingInfo.BookingNote??"";
                    var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;
                    if (order.DeliveryMethod is DeliveryMethod.SevenToEleven1 ||
                        order.DeliveryMethod is DeliveryMethod.FamilyMart1 ||
                        order.DeliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
                        deliveryMethod is DeliveryMethod.FamilyMart1 ||
                        deliveryMethod is DeliveryMethod.SevenToEleven1 ||
                        deliveryMethod is DeliveryMethod.SevenToElevenFrozen)
                    {
                        string strResponse = await GenerateShipmentForB2C(result);

                        ResponseResultDto responseResultDto = ParseApiResponse(strResponse, true);

                        newOrderDelivery.DeliveryNo = responseResultDto?.ShippingInfo?.ShipmentNo;
                    }

                    if (order.DeliveryMethod is DeliveryMethod.FamilyMartC2C ||
                        deliveryMethod is DeliveryMethod.FamilyMartC2C)
                        newOrderDelivery.DeliveryNo = result.ShippingInfo.CVSPaymentNo;

                    if (order.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                        deliveryMethod is DeliveryMethod.SevenToElevenC2C)
                        newOrderDelivery.DeliveryNo = string.Concat(result.ShippingInfo.CVSPaymentNo, result.ShippingInfo.CVSValidationNo);

                    newOrderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

                    newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                    if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                        newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                    //order.ShippingStatus = ShippingStatus.ToBeShipped;
                    await uow.SaveChangesAsync();
                    // await _orderRepository.UpdateAsync(order);
                    // **Get Current User (Editor)**
                    var currentUserId = CurrentUser.Id ?? Guid.Empty;
                    var currentUserName = CurrentUser.UserName ?? "System";

                    // **Log Order History for Delivery Update**
                    await _orderHistoryManager.AddOrderHistoryAsync(
     newOrderDelivery.OrderId,
     "GenerateDeliveryNumberLog", // Localization key
     new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
     currentUserId,
     currentUserName
 );

                }


                    await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);

                }
            }
        catch (AbpDbConcurrencyException e)
        {
            Logger.LogError(e.Message);
            Logger.LogWarning("Issue on Store Logistics");
            if (result.ResponseCode is "1")
            {
                using (var uow = UnitOfWorkManager.Begin(
              requiresNew: true, isTransactional: false
          ))
                {
                    var newOrderDelivery = await _deliveryRepository.GetAsync(orderDeliveryId);
                    newOrderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;
                    var oldDeliveryStatus = newOrderDelivery.DeliveryStatus;

                    if (order.DeliveryMethod is DeliveryMethod.SevenToEleven1 ||
                        order.DeliveryMethod is DeliveryMethod.FamilyMart1 ||
                        order.DeliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
                        deliveryMethod is DeliveryMethod.FamilyMart1 ||
                        deliveryMethod is DeliveryMethod.SevenToEleven1 ||
                        deliveryMethod is DeliveryMethod.SevenToElevenFrozen)
                    {
                        string strResponse = await GenerateShipmentForB2C(result);

                        ResponseResultDto responseResultDto = ParseApiResponse(strResponse, true);

                        newOrderDelivery.DeliveryNo = responseResultDto.ShippingInfo.ShipmentNo;
                    }

                    if (order.DeliveryMethod is DeliveryMethod.FamilyMartC2C ||
                        deliveryMethod is DeliveryMethod.FamilyMartC2C)
                        newOrderDelivery.DeliveryNo = result.ShippingInfo.CVSPaymentNo;

                    if (order.DeliveryMethod is DeliveryMethod.SevenToElevenC2C ||
                        deliveryMethod is DeliveryMethod.SevenToElevenC2C)
                        newOrderDelivery.DeliveryNo = string.Concat(result.ShippingInfo.CVSPaymentNo, result.ShippingInfo.CVSValidationNo);

                    newOrderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

                    newOrderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

                    if (orderDelivery.DeliveryMethod is DeliveryMethod.DeliveredByStore &&
                        deliveryMethod is not null)
                        newOrderDelivery.ActualDeliveryMethod = deliveryMethod;



                    //order.ShippingStatus = ShippingStatus.ToBeShipped;
                    await uow.SaveChangesAsync();
                    // await _orderRepository.UpdateAsync(order);

                    // **Get Current User (Editor)**
                    var currentUserId = CurrentUser.Id ?? Guid.Empty;
                    var currentUserName = CurrentUser.UserName ?? "System";

                    // **Log Order History for Delivery Update**
                    await _orderHistoryManager.AddOrderHistoryAsync(
    newOrderDelivery.OrderId,
    "GenerateDeliveryNumberLog", // Localization key
    new object[] { newOrderDelivery.DeliveryNo }, // Dynamic placeholder for delivery number
    currentUserId,
    currentUserName
);
                }


                await SendEmailAsync(orderId, ShippingStatus.ToBeShipped);
            }
                return result;
        }

        return result;
        
    }

    public async Task<string> FindStatusAsync()
    {
        RestClientOptions options = new() { MaxTimeout = -1 };

        RestClient client = new(options);

        RestRequest request = new(_configuration["EcPay:QueryLogisticsTradeInfo"], Method.Post);

        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("MerchantID", "2000132");
        request.AddParameter("MerchantTradeNo", "87FBF77DDE6");
        request.AddParameter("TimeStamp", "1732699855");

        var parameters = new Dictionary<string, string>
        {
            { "MerchantID", "2000132" },
            { "MerchantTradeNo", "87FBF77DDE6" },
            { "TimeStamp", "1732699855" },
        };

        IOrderedEnumerable<KeyValuePair<string, string>> sortedParameters = parameters.OrderBy(p => p.Key);

        string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

        requestString = $"HashKey=5294y06JbISpM5x9&{requestString}&HashIV=v77hoKGq4kWxNNIS";

        string urlEncodedData = HttpUtility.UrlEncode(requestString);

        string lowercaseData = urlEncodedData.ToLower();

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            request.AddParameter("CheckMacValue", sb.ToString());
        }

        RestResponse response = await client.ExecuteAsync(request);

        return response.Content.ToString();
    }

    public async Task<string> GenerateShipmentForB2C(ResponseResultDto response)
    {
        RestClient client = new(new RestClientOptions() { MaxTimeout = -1 });

        RestRequest request = new(_configuration["EcPay:QueryLogisticsTradeInfo"], Method.Post);
      

        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("MerchantID", GreenWorld.StoreCode);
        request.AddParameter("AllPayLogisticsID", response.ShippingInfo.AllPayLogisticsID);
        request.AddParameter("TimeStamp", timeStamp);
        request.AddParameter("CheckMacValue", GenerateCheckMac(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, response.ShippingInfo.AllPayLogisticsID, timeStamp));

        RestResponse restResponse = await client.ExecuteAsync(request);

        string? str = Convert.ToString(restResponse.Content);

        if (!str.IsNullOrEmpty() && str.Contains(PikachuResource.ShipmentNo)) str = string.Concat("1|", str);

        return str ?? string.Empty;
    }

    public string GenerateRequestString(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, string senderName, string receiverName, string receiverCellPhone, string serverReplyURL, string receiverStoreID, string? goodName = null, string? senderCellNumber = null, string? isCollection = null)
    {
        //string HashKey = "5294y06JbISpM5x9";
        //string HashIV = "v77hoKGq4kWxNNIS";
        // Create a dictionary to hold parameters

        var parameters = new Dictionary<string, string>
        {
            { "MerchantID", merchantID },
            { "MerchantTradeNo", merchantTradeNo },
            { "MerchantTradeDate", merchantTradeDate },
            { "LogisticsType", logisticsType },
            { "LogisticsSubType", logisticsSubType },
            { "GoodsAmount", goodsAmount.ToString() },

            { "SenderName", senderName },

            { "ReceiverName", receiverName },

            { "ReceiverCellPhone", receiverCellPhone },

            { "ServerReplyURL", serverReplyURL },


            { "ReceiverStoreID", receiverStoreID },

        };
        if (!isCollection.IsNullOrEmpty()) parameters.Add("IsCollection", "Y");

        if (goodName is not null) parameters.Add("GoodsName", goodName);

        if (senderCellNumber is not null) parameters.Add("SenderCellPhone", senderCellNumber);

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

    public async Task<EmapApiResponse> GetStoreAsync(string deliveryMethod)
    {
        EmapApiResponse result = new();
        //var order=await _orderRepository.GetAsync(orderId);
        //var orderDeliverys= await _deliveryRepository.GetWithDetailsAsync(orderId);
        //var orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();

        RestClient client1 = new(new RestClientOptions { MaxTimeout = -1 });

        //var request1 = new RestRequest(_configuration["EcPay:E-MAPApi"], Method.Post);
        var request1 = new RestRequest("https://logistics-stage.ecpay.com.tw/Express/map", Method.Post);
        request1.AddHeader("Accept", "text/html");
        request1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //request1.AddHeader("Cookie", "MapInfo=MerchantID=2000132&MerchantTradeNo=ECPay&LogisticsType=CVS&LogisticsSubType=FAMI&IsCollection=N&ServerReplyURL=https%3a%2f%2fwww.ecpay.com.tw%2fServerReplyURL&CallBackFunction=&IsGet=&Device=0");
        request1.AddParameter("MerchantID", "2000132");
        request1.AddParameter("LogisticsType", "CVS");

        if (deliveryMethod == DeliveryMethod.SevenToEleven1.ToString()) request1.AddParameter("LogisticsSubType", "UNIMART");

        else if (deliveryMethod == DeliveryMethod.FamilyMart1.ToString()) request1.AddParameter("LogisticsSubType", "FAMI");

        else if (deliveryMethod == DeliveryMethod.SevenToElevenC2C.ToString()) request1.AddParameter("LogisticsSubType", "UNIMARTC2C");

        else if (deliveryMethod == DeliveryMethod.FamilyMartC2C.ToString()) request1.AddParameter("LogisticsSubType", "FAMIC2C");

        //request1.AddParameter("ServerReplyURL", "https://ba6ee5b524c5287100a78b51d8b7fc18.m.pipedream.net");
        request1.AddParameter("ServerReplyURL", "https://emap.pcsc.com.tw/ecmap/default.aspx");
        request1.AddParameter("IsCollection", "N");
        RestResponse response1 = await client1.ExecuteAsync(request1);

        Console.WriteLine(response1.Content);
        result.HtmlString = response1.Content;
        result.CookieName = response1.Cookies.Select(x => x.Name).FirstOrDefault();
        result.CookieValue = response1.Cookies.Select(x => x.Value).FirstOrDefault();

        return result;
    }

    public string GenerateCheckMacValue(string HashKey, string HashIV, Dictionary<string, string> parameters)
    {
        IOrderedEnumerable<KeyValuePair<string, string>> sortedParameters = parameters.OrderBy(p => p.Key);

        string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

        requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";

        string urlEncodedData = HttpUtility.UrlEncode(requestString);

        string lowercaseData = urlEncodedData.ToLower();

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, string allPayLogisticsID, long timeStamp)
    {
        Dictionary<string, string> keyValuePairs = [];

        keyValuePairs.Add("MerchantID", merchantID);
        keyValuePairs.Add("AllPayLogisticsID", allPayLogisticsID);
        keyValuePairs.Add("TimeStamp", timeStamp.ToString());

        IOrderedEnumerable<KeyValuePair<string, string>> sortedParameters = keyValuePairs.OrderBy(p => p.Key);

        string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

        requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";

        string urlEncodedData = HttpUtility.UrlEncode(requestString);

        string lowercaseData = urlEncodedData.ToLower();

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, decimal goodsWeight, string senderName, string senderPhone, string senderZipCode, string senderAddress,
                                string receiverName, string receiverCellPhone, string receiverZipCode, string receiverAddress, string serverReplyURL)
    {
        //string HashKey = "5294y06JbISpM5x9";
        //string HashIV = "v77hoKGq4kWxNNIS";
        // Create a dictionary to hold parameters
        Dictionary<string, string> parameters = new()
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
        //requestString = $"HashKey={HashKey}&{requestString}&HashIV={HashIV}";
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

    static ResponseResultDto ParseApiResponse(string apiResponse, bool isSevenElevenB2C = false)
    {
        ResponseResultDto result = new();

        // Split the response into response code and key-value pairs
        string[] responseParts = apiResponse.Split('|');

        result.ResponseCode = responseParts[0];

        if (result.ResponseCode is not "1")
        {
            result.ResponseMessage = responseParts[1];

            return result;
        }
        // Split the key-value pairs
        string[] keyValuePairs = responseParts[1].Split('&');

        // Parse key-value pairs into dictionary
        Dictionary<string, string> dataDict = [];

        foreach (string kvPair in keyValuePairs)
        {
            string[] parts = kvPair.Split('=');

            string key = parts[0];

            string value = parts.Length > 1 ? parts[1] : string.Empty;

            dataDict[key] = value;
        }

        if (isSevenElevenB2C) result.ShippingInfo = new() { ShipmentNo = dataDict["ShipmentNo"] };

        else
        {
            result.ShippingInfo = new ShippingInfoDto
            {
                AllPayLogisticsID = dataDict["AllPayLogisticsID"],
                BookingNote = dataDict["BookingNote"],
                CheckMacValue = dataDict["CheckMacValue"],
                CVSPaymentNo = dataDict["CVSPaymentNo"],
                CVSValidationNo = dataDict["CVSValidationNo"],
                GoodsAmount = dataDict["GoodsAmount"],
                LogisticsSubType = dataDict["LogisticsSubType"],
                LogisticsType = dataDict["LogisticsType"],
                MerchantID = dataDict["MerchantID"],
                MerchantTradeNo = dataDict["MerchantTradeNo"],
                ReceiverAddress = dataDict["ReceiverAddress"],
                ReceiverCellPhone = dataDict["ReceiverCellPhone"],
                ReceiverEmail = dataDict["ReceiverEmail"],
                ReceiverName = dataDict["ReceiverName"],
                ReceiverPhone = dataDict["ReceiverPhone"],
                RtnCode = dataDict["RtnCode"],
                RtnMsg = dataDict["RtnMsg"],
                UpdateStatusDate = dataDict["UpdateStatusDate"]
            };
        }

        return result;
    }

    #endregion

    #region Private Functions
    private async Task SendEmailAsync(Guid id, ShippingStatus? shippingStatus = null)
    {
        await _emailAppService.SendOrderStatusEmailAsync(id, shippingStatus: shippingStatus);
    }
    private  string AddNumericSuffix(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty.");

        Random random = new Random();
        int suffix = random.Next(100000, 999999); // Generates a 6-digit random number
        return input + suffix.ToString();
    }
    #endregion
}
