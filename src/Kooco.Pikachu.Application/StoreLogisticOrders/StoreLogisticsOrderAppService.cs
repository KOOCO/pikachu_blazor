using AutoMapper;
using Azure;
using Azure.Core;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Kooco.Pikachu.StoreLogisticOrders;

[RemoteService(IsEnabled = false)]
public class StoreLogisticsOrderAppService : ApplicationService, IStoreLogisticsOrderAppService
{
    #region Inject
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
    TCatLogisticsCreateUpdateDto TCatLogistics { get; set; }
    TCatNormalCreateUpdateDto TCatNormal { get; set; }
    TCatFreezeCreateUpdateDto TCatFreeze { get; set; }
    TCatFrozenCreateUpdateDto TCatFrozen { get; set; }
    TCat711NormalCreateUpdate TCat711Normal { get; set; }
    TCat711FreezeCreateUpdateDto TCat711Freeze { get; set; }
    TCat711FrozenCreateUpdateDto TCat711Frozen { get; set; }

    private IStringLocalizer<PikachuResource> _L;

    private readonly IConfiguration _configuration;

    private readonly IGroupBuyAppService _GroupBuyAppService;
    #endregion

    #region Constructor
    public StoreLogisticsOrderAppService(
        IOrderDeliveryRepository orderDeliveryRepository,
        IOrderRepository orderRepository,
        ILogisticsProvidersAppService logisticsProvidersAppService, 
        IConfiguration configuration,
        IStringLocalizer<PikachuResource> L,
        IGroupBuyAppService GroupBuyAppService
    ) 
    {   
        _orderRepository = orderRepository;
        _deliveryRepository = orderDeliveryRepository;
        _logisticsProvidersAppService= logisticsProvidersAppService;
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
    }
    #endregion

    public async Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId)
    {
        Order order = await _orderRepository.GetAsync(orderId);
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
            BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFrozen);
        }
        var options = new RestClientOptions
        {
            MaxTimeout = -1,
        };

        RestClient client = new (options);

        RestRequest request = new (_configuration["EcPay:LogisticApi"], Method.Post);
        
        string marchentDate = DateTime.Now.ToString("yyyy/MM/dd");

        string receiverAddress = string.Concat(_L[order.City].Value, order.AddressDetails);

        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("MerchantID", GreenWorld.StoreCode);
        request.AddParameter("MerchantTradeDate",marchentDate);
        request.AddParameter("LogisticsType", "HOME");
        request.AddParameter("LogisticsSubType",orderDelivery.DeliveryMethod is DeliveryMethod.PostOffice ? "POST" : "TCAT");
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
        request.AddParameter("ServerReplyURL", "https://www.ecpay.com.tw/ServerReplyURL");
        //request.AddParameter("ReceiverStoreID", "123");
        request.AddParameter("CheckMacValue", GenerateCheckMac(
            greenWorld.HashKey, greenWorld.HashIV, GreenWorld.StoreCode, order.OrderNo, marchentDate, "HOME", orderDelivery.DeliveryMethod == EnumValues.DeliveryMethod.PostOffice ? "POST" : "TCAT", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)),PostOffice.Weight, GreenWorld.SenderName,GreenWorld.SenderPhoneNumber,
            GreenWorld.SenderPostalCode,GreenWorld.SenderAddress, order.RecipientName, order.RecipientPhone,order.PostalCode, receiverAddress, "https://www.ecpay.com.tw/ServerReplyURL"));
        //request.AddParameter("IsCollection", "N");
        request.AddParameter("MerchantTradeNo",  order.OrderNo);

        RestResponse response = await client.ExecuteAsync(request);

        ResponseResultDto result = ParseApiResponse(response.Content.ToString());
        
        if (result.ResponseCode is "1")
        {
            orderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;
            
            orderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

            orderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

            await _deliveryRepository.UpdateAsync(orderDelivery);

            order.ShippingStatus = ShippingStatus.ToBeShipped;

            await _orderRepository.UpdateAsync(order);
        }
        return result;
    }

    public async Task<PrintObtResponse?> GenerateDeliveryNumberForTCatDeliveryAsync(Guid orderId, Guid orderDeliveryId)
    {
        Order order = await _orderRepository.GetAsync(orderId);

        List<OrderDelivery> orderDeliveries = await _deliveryRepository.GetWithDetailsAsync(orderId);

        OrderDelivery orderDelivery = orderDeliveries.First(f => f.Id == orderDeliveryId);

        GroupBuyDto groupBuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);

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

        if (orderDelivery.Items is { Count: > 0 } &&
            orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Normal)
        {
            thermosphere = "0001";

            spec = GetSpec(TCatNormal.Size);

            isSwipe = TCatNormal.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";
        }

        else if (orderDelivery.Items is { Count: > 0 } &&
                 orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Freeze)
        {
            thermosphere = "0002";

            spec = GetSpec(TCatFreeze.Size);

            isSwipe = TCatFreeze.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";
        }

        else if (orderDelivery.Items is { Count: > 0 } &&
                 orderDelivery.Items.First().DeliveryTemperature is ItemStorageTemperature.Frozen)
        {
            thermosphere = "0003";

            spec = GetSpec(TCatFrozen.Size);

            isSwipe = TCatFrozen.TCatPaymentMethod is TCatPaymentMethod.CardAndMobilePaymentsAccepted ? "Y" : "N";
        }

        string deliveryTime = order.ReceivingTime switch 
        { 
            ReceivingTime.Before13PM => "01",
            ReceivingTime.Between14To18PM => "02",
            _ => "04"
        };

        int collectionAmount = order.PaymentMethod is PaymentMethods.CashOnDelivery ? 
                                GetCollectionAmount(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) :
                                0;

        string isDeclare = TCatLogistics.DeclaredValue &&
                           IsOrderAmountValid(orderDelivery.Items.Sum(s => s.TotalAmount), order.DeliveryCost) ? "Y" : "N";

        PrintOBTRequest request = new ()
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
                    OrderId = order.OrderNo,
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
                    ShipmentDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd"),
                    DeliveryDate = DateTime.Now.AddDays(2).ToString("yyyyMMdd"),
                    DeliveryTime = deliveryTime,
                    IsFreight = "N",
                    IsCollection = order.PaymentMethod is PaymentMethods.CashOnDelivery ? "Y" : "N",
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

        StringContent content = new (jsonContent, Encoding.UTF8, "application/json");

        using HttpClient httpClient = new();

        HttpResponseMessage response = await httpClient.PostAsync(_configuration["T-Cat:PrintOBT"], content);

        string responseContent = await response.Content.ReadAsStringAsync();

        PrintObtResponse? printObtResponse = JsonConvert.DeserializeObject<PrintObtResponse>(responseContent);

        if (printObtResponse is null || printObtResponse.Data is null) return printObtResponse;

        orderDelivery.SrvTranId = printObtResponse.SrvTranId;
        orderDelivery.FileNo = printObtResponse.Data.FileNo;
        orderDelivery.AllPayLogisticsID = printObtResponse.SrvTranId;
        orderDelivery.LastModificationTime = DateTime.ParseExact(printObtResponse.Data.PrintDateTime, 
                                                                 "yyyyMMddHHmmss", 
                                                                 System.Globalization.CultureInfo.InvariantCulture);
        orderDelivery.DeliveryNo = printObtResponse.Data.Orders.First().OBTNumber;
        orderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

        await _deliveryRepository.UpdateAsync(orderDelivery);

        order.ShippingStatus = ShippingStatus.ToBeShipped;

        await _orderRepository.UpdateAsync(order);

        return printObtResponse;
    }

    public async Task<PrintOBTB2SResponse?> GenerateDeliveryNumberForTCat711DeliveryAsync(Guid orderId, Guid orderDeliveryId)
    {
        Order order = await _orderRepository.GetAsync(orderId);

        List<OrderDelivery> orderDeliveries = await _deliveryRepository.GetWithDetailsAsync(orderId);

        OrderDelivery orderDelivery = orderDeliveries.First(f => f.Id == orderDeliveryId);

        GroupBuyDto groupBuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);

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
                    OrderId = order.OrderNo,
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

        orderDelivery.SrvTranId = printObtB2SResponse.SrvTranId;
        orderDelivery.FileNo = printObtB2SResponse.Data.FileNo;
        orderDelivery.AllPayLogisticsID = printObtB2SResponse.Data.Orders.First().OBTNumber;
        orderDelivery.LastModificationTime = DateTime.ParseExact(printObtB2SResponse.Data.PrintDateTime,
                                                                 "yyyyMMddHHmmss",
                                                                 System.Globalization.CultureInfo.InvariantCulture);
        orderDelivery.DeliveryNo = printObtB2SResponse.Data.Orders.First().DeliveryId;
        orderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

        await _deliveryRepository.UpdateAsync(orderDelivery);

        order.ShippingStatus = ShippingStatus.ToBeShipped;

        await _orderRepository.UpdateAsync(order);

        return printObtB2SResponse;
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

    public async Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId)
    {
        ResponseResultDto result = new ();

        Order order = await _orderRepository.GetWithDetailsAsync(orderId);

        List<OrderDelivery> orderDeliverys = await _deliveryRepository.GetWithDetailsAsync(orderId);

        OrderDelivery? orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();
       
        List<LogisticsProviderSettingsDto> providers = await _logisticsProvidersAppService.GetAllAsync();

        string logisticSubType = string.Empty;
        
        if (orderDelivery is not null && (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenC2C || orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMartC2C))
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

        RestClientOptions options = new () { MaxTimeout = -1 };

        RestClient client = new (options);

        RestRequest request = new (_configuration["EcPay:LogisticApi"], Method.Post);

        string marchentDate = DateTime.Now.ToString("yyyy/MM/dd");

        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("MerchantID", GreenWorld.StoreCode);
        request.AddParameter("MerchantTradeDate", marchentDate);
        request.AddParameter("LogisticsType", "CVS");

        if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToEleven1)
        {
            request.AddParameter("LogisticsSubType", "UNIMART");

            logisticSubType = "UNIMART";
        }
        else if (orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMart1)
        {
            request.AddParameter("LogisticsSubType", "FAMI");
            logisticSubType = "FAMI";
        }
        else if (orderDelivery.DeliveryMethod is DeliveryMethod.SevenToElevenC2C)
        {
            request.AddParameter("LogisticsSubType", "UNIMARTC2C");
            logisticSubType = "UNIMARTC2C";
            request.AddParameter("GoodsName", order.GroupBuy.GroupBuyName);
            request.AddParameter("SenderCellPhone", GreenWorld.SenderPhoneNumber);
        }
        else if (orderDelivery.DeliveryMethod is DeliveryMethod.FamilyMartC2C)
        {
            request.AddParameter("LogisticsSubType", "FAMIC2C");
            logisticSubType = "FAMIC2C";
            request.AddParameter("GoodsName", order.GroupBuy.GroupBuyName);
            request.AddParameter("SenderCellPhone", GreenWorld.SenderPhoneNumber);
        }
        request.AddParameter("GoodsAmount", Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)));
        request.AddParameter("SenderName", GreenWorld.SenderName);
        request.AddParameter("ReceiverName", order.RecipientName);
        request.AddParameter("ReceiverCellPhone", order.RecipientPhone);
        request.AddParameter("ServerReplyURL", "https://www.ecpay.com.tw/ServerReplyURL");
        request.AddParameter("ReceiverStoreID", order.StoreId);

        if (orderDelivery.DeliveryMethod == DeliveryMethod.SevenToElevenC2C || orderDelivery.DeliveryMethod == DeliveryMethod.FamilyMartC2C)
        {
            request.AddParameter("CheckMacValue", GenerateRequestString(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, order.OrderNo, marchentDate, "CVS", logisticSubType, Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), GreenWorld.SenderName, order.RecipientName, order.RecipientPhone,
                "https://www.ecpay.com.tw/ServerReplyURL", order.StoreId,order.GroupBuy.GroupBuyName,GreenWorld.SenderPhoneNumber));
        }
        else {
            request.AddParameter("CheckMacValue", GenerateRequestString(GreenWorld.HashKey, GreenWorld.HashIV, GreenWorld.StoreCode, order.OrderNo, marchentDate, "CVS", logisticSubType, Convert.ToInt32(orderDelivery.Items.Sum(x => x.TotalAmount)), GreenWorld.SenderName, order.RecipientName, order.RecipientPhone,
                    "https://www.ecpay.com.tw/ServerReplyURL", order.StoreId));
        }

        //request.AddParameter("IsCollection", "N");
        request.AddParameter("MerchantTradeNo", order.OrderNo);

        RestResponse response = await client.ExecuteAsync(request);

        result = ParseApiResponse(response.Content?.ToString());

        if (result.ResponseCode is "1")
        {
            orderDelivery.DeliveryNo = result.ShippingInfo.BookingNote;

            if (order.DeliveryMethod is DeliveryMethod.SevenToEleven1 || order.DeliveryMethod is DeliveryMethod.FamilyMart1)
            {
                string strResponse = await GenerateShipmentForB2C(result);

                ResponseResultDto responseResultDto = ParseApiResponse(strResponse, true);

                orderDelivery.DeliveryNo = responseResultDto.ShippingInfo.ShipmentNo;
            }

            if (order.DeliveryMethod is DeliveryMethod.FamilyMartC2C) orderDelivery.DeliveryNo = result.ShippingInfo.CVSPaymentNo;

            if (order.DeliveryMethod is DeliveryMethod.SevenToElevenC2C)
                orderDelivery.DeliveryNo = string.Concat(result.ShippingInfo.CVSPaymentNo, result.ShippingInfo.CVSValidationNo);

            orderDelivery.AllPayLogisticsID = result.ShippingInfo.AllPayLogisticsID;

            orderDelivery.DeliveryStatus = DeliveryStatus.ToBeShipped;

            await _deliveryRepository.UpdateAsync(orderDelivery);

            order.ShippingStatus = ShippingStatus.ToBeShipped;

            await _orderRepository.UpdateAsync(order);
        }

        return result;
    }

    public async Task<string> GenerateShipmentForB2C(ResponseResultDto response)
    {
        RestClient client = new(new RestClientOptions() { MaxTimeout = -1 });

        RestRequest request = new("https://logistics-stage.ecpay.com.tw/Helper/QueryLogisticsTradeInfo/V3", Method.Post);

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

    public string GenerateRequestString(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, string senderName, string receiverName, string receiverCellPhone, string serverReplyURL, string receiverStoreID,string? goodName=null,string? senderCellNumber =null)
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

        //{ "IsCollection", "Y" },

        { "SenderName", senderName },

        { "ReceiverName", receiverName },

        { "ReceiverCellPhone", receiverCellPhone },

        { "ServerReplyURL", serverReplyURL },


        { "ReceiverStoreID", receiverStoreID },

    };
        if (goodName != null)
        {
            parameters.Add("GoodsName", goodName);
        }
        if (senderCellNumber != null)
        {
            parameters.Add("SenderCellPhone", senderCellNumber);
        }
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
        EmapApiResponse result = new ();
        //var order=await _orderRepository.GetAsync(orderId);
        //var orderDeliverys= await _deliveryRepository.GetWithDetailsAsync(orderId);
        //var orderDelivery = orderDeliverys.Where(x => x.Id == orderDeliveryId).FirstOrDefault();

        RestClient client1 = new (new RestClientOptions { MaxTimeout = -1 });

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

            StringBuilder sb = new ();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string merchantTradeDate, string logisticsType, string logisticsSubType, int goodsAmount, decimal goodsWeight, string senderName, string senderPhone, string senderZipCode, string senderAddress,
                                string receiverName, string receiverCellPhone, string receiverZipCode,string receiverAddress, string serverReplyURL)
    {
        //string HashKey = "5294y06JbISpM5x9";
        //string HashIV = "v77hoKGq4kWxNNIS";
        // Create a dictionary to hold parameters
        Dictionary<string, string> parameters = new ()
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
        ResponseResultDto result = new ();

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
}
