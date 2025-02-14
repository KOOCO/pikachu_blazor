﻿using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Orders;

[RemoteService(IsEnabled = false)]
public class OrderAppService : ApplicationService, IOrderAppService
{
    #region Inject
    private readonly IOrderRepository _orderRepository;
    private readonly OrderManager _orderManager;
    private readonly IDataFilter _dataFilter;
    private readonly IRepository<OrderDelivery, Guid> _orderDeliveryRepository;
    private readonly IGroupBuyRepository _groupBuyRepository;
    private readonly IEmailSender _emailSender;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
    private readonly IStringEncryptionService _stringEncryptionService;
    private readonly IRepository<OrderItem, Guid> _orderItemRepository;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly IItemRepository _itemRepository;
    private readonly IItemDetailsRepository _itemDetailsRepository;
    private readonly IElectronicInvoiceAppService _electronicInvoiceAppService;
    private readonly IElectronicInvoiceSettingRepository _electronicInvoiceSettingRepository;
    private readonly IFreebieRepository _freebieRepository;
    private readonly IRefundAppService _refundAppService;
    private readonly IRefundRepository _refundRepository;
    private readonly IDiscountCodeRepository _discountCodeRepository;
    private readonly ITenantSettingsAppService _tenantSettingsAppService;
    private readonly IUserShoppingCreditAppService _userShoppingCreditAppService;
    private readonly IUserShoppingCreditRepository _userShoppingCreditRepository;
    private readonly IEmailAppService _emailAppService;
    private readonly IOrderMessageAppService _OrderMessageAppService;
    private readonly ISetItemRepository _setItemRepository;
    private readonly IUserCumulativeCreditAppService _userCumulativeCreditAppService;
    private readonly IUserCumulativeCreditRepository _userCumulativeCreditRepository;
    #endregion

    #region Constructor
    public OrderAppService(
        IOrderRepository orderRepository,
        OrderManager orderManager,
        IDataFilter dataFilter,
        IGroupBuyRepository groupBuyRepository,
        IEmailSender emailSender,
        IRepository<PaymentGateway, Guid> paymentGatewayRepository,
        IStringEncryptionService stringEncryptionService,
        IStringLocalizer<PikachuResource> l,
        IRepository<OrderItem, Guid> orderItemRepository,
        IRepository<OrderDelivery, Guid> orderDeliveryRepository,
        IItemRepository itemRepository,
        IItemDetailsRepository itemDetailsRepository,
        IElectronicInvoiceAppService electronicInvoiceAppService,
        IFreebieRepository freebieRepository,
        IBackgroundJobManager backgroundJobManager,
        IElectronicInvoiceSettingRepository electronicInvoiceSettingRepository,
        IRefundAppService refundAppService,
        IRefundRepository refundRepository,
        ITenantSettingsAppService tenantSettingsAppService,
        IDiscountCodeRepository discountCodeRepository,
        IUserShoppingCreditAppService userShoppingCreditAppService,
        IUserShoppingCreditRepository userShoppingCreditRepository,
        IEmailAppService emailAppService,
        IOrderMessageAppService OrderMessageAppService,
        ISetItemRepository setItemRepository,
        IUserCumulativeCreditAppService userCumulativeCreditAppService,
        IUserCumulativeCreditRepository userCumulativeCreditRepository
    )
    {
        _orderRepository = orderRepository;
        _orderManager = orderManager;
        _dataFilter = dataFilter;
        _groupBuyRepository = groupBuyRepository;
        _emailSender = emailSender;
        _paymentGatewayRepository = paymentGatewayRepository;
        _stringEncryptionService = stringEncryptionService;
        _orderItemRepository = orderItemRepository;
        _orderDeliveryRepository = orderDeliveryRepository;
        _l = l;
        _itemRepository = itemRepository;
        _orderItemRepository = orderItemRepository;
        _itemDetailsRepository = itemDetailsRepository;
        _electronicInvoiceAppService = electronicInvoiceAppService;
        _freebieRepository = freebieRepository;
        _backgroundJobManager = backgroundJobManager;
        _electronicInvoiceSettingRepository = electronicInvoiceSettingRepository;
        _refundAppService = refundAppService;
        _refundRepository = refundRepository;
        _tenantSettingsAppService = tenantSettingsAppService;
        _discountCodeRepository = discountCodeRepository;
        _userShoppingCreditAppService = userShoppingCreditAppService;
        _userShoppingCreditRepository = userShoppingCreditRepository;
        _emailAppService = emailAppService;
        _OrderMessageAppService = OrderMessageAppService;
        _setItemRepository = setItemRepository;
        _userCumulativeCreditAppService = userCumulativeCreditAppService;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Create Order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<OrderDto> CreateAsync(CreateUpdateOrderDto input)
    {
        GroupBuy groupBuy = new();

        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(input.GroupBuyId);
        }

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order = await _orderManager.CreateAsync(
                input.GroupBuyId,
                input.IsIndividual,
                input.CustomerName,
                input.CustomerPhone,
                input.CustomerEmail,
                input.PaymentMethod,
                input.InvoiceType,
                string.Empty,
                input.UniformNumber,
                input.CarrierId,
                input.TaxTitle,
                input.IsAsSameBuyer,
                input.RecipientName,
                input.RecipientPhone,
                input.RecipientEmail,
                input.DeliveryMethod,
                input.PostalCode,
                input.City,
                string.Empty,
                string.Empty,
                input.AddressDetails,
                input.Remarks,
                input.ReceivingTime,
                input.TotalQuantity,
                input.TotalAmount,
                input.ReturnStatus,
                input.OrderType,
                userId: input.UserId,
                creditDeductionAmount: input.CreditDeductionAmount,
                creditDeductionRecordId: input.CreditDeductionRecordId,
                creditRefundAmount: input.cashback_amount,
                creditRefundRecordId: input.cashback_record_id,
                discountAmountId: input.DiscountCodeId,
                discountCodeAmount: input.DiscountCodeAmount,
                recipientNameDbsNormal: input.RecipientNameDbsNormal,
                recipientNameDbsFreeze: input.RecipientNameDbsFreeze,
                recipientNameDbsFrozen: input.RecipientNameDbsFrozen,
                recipientPhoneDbsNormal: input.RecipientPhoneDbsNormal,
                recipientPhoneDbsFreeze: input.RecipientPhoneDbsFreeze,
                recipientPhoneDbsFrozen: input.RecipientPhoneDbsFrozen,
                postalCodeDbsNormal: input.PostalCodeDbsNormal,
                postalCodeDbsFreeze: input.PostalCodeDbsFreeze,
                postalCodeDbsFrozen: input.PostalCodeDbsFrozen,
                cityDbsNormal: input.CityDbsNormal,
                cityDbsFreeze: input.CityDbsFreeze,
                cityDbsFrozen: input.CityDbsFrozen,
                addressDetailsDbsNormal: input.AddressDetailsDbsNormal,
                addressDetailsDbsFreeze: input.AddressDetailsDbsFreeze,
                addressDetailsDbsFrozen: input.AddressDetailsDbsFrozen,
                remarksDbsNormal: input.RemarksDbsNormal,
                remarksDbsFreeze: input.RemarksDbsFreeze,
                remarksDbsFrozen: input.RemarksDbsFrozen,
                storeIdNormal: input.StoreIdNormal,
                storeIdFreeze: input.StoreIdFreeze,
                storeIdFrozen: input.StoreIdFrozen,
                cVSStoreOutSideNormal: input.CVSStoreOutSideNormal,
                cVSStoreOutSideFreeze: input.CVSStoreOutSideFreeze,
                cVSStoreOutSideFrozen: input.CVSStoreOutSideFrozen,
                receivingTimeNormal: input.ReceivingTimeNormal,
                receivingTimeFreeze: input.ReceivingTimeFreeze,
                receivingTimeFrozen: input.ReceivingTimeFrozen
            );

            order.StoreId = input.StoreId;
            order.CVSStoreOutSide = input.CVSStoreOutSide;
            order.ShippingStatus = groupBuy!.IsEnterprise ? ShippingStatus.EnterpricePurchase : input.ShippingStatus;
            order.DeliveryCostForNormal = input.DeliveryCostForNormal;
            order.DeliveryCostForFreeze = input.DeliveryCostForFreeze;
            order.DeliveryCostForFrozen = input.DeliveryCostForFrozen;
            order.DeliveryCost = input.DeliveryCost;

            if (input.OrderItems is { Count: > 0 })
            {
                List<string> insufficientItems = new List<string>();

                foreach (CreateUpdateOrderItemDto item in input.OrderItems)
                {
                    _orderManager.AddOrderItem(
                        order,
                        item.ItemId,
                        item.SetItemId,
                        item.FreebieId,
                        item.ItemType,
                        item.OrderId,
                        item.Spec,
                        item.ItemPrice,
                        item.TotalAmount,
                        item.Quantity,
                        item.SKU,
                        item.DeliveryTemperature,
                        0
                    );

                    using (_dataFilter.Disable<IMultiTenant>())
                    {
                        ItemDetails? details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == item.ItemId && x.ItemName == item.SKU);

                        if (details != null)
                        {
                            // Check if the available quantity is sufficient
                            if (details.SaleableQuantity < item.Quantity)
                            {
                                // Add item to insufficientItems list
                                insufficientItems.Add($"Item: {details.ItemName}, Requested: {item.Quantity}, Available: {details.SaleablePreOrderQuantity},Details:{JsonConvert.SerializeObject(details)}");
                            }
                            else
                            {
                                // Proceed with updating the stock if sufficient
                                details.SaleableQuantity = details.SaleableQuantity- item.Quantity;
                                details.StockOnHand = details.StockOnHand- item.Quantity;

                                await _itemDetailsRepository.UpdateAsync(details);
                            }
                        }

                        if (item.SetItemId.HasValue)
                        {
                            var setItem = await _setItemRepository.GetWithDetailsAsync(item.SetItemId.Value);
                            if (setItem != null)
                            {
                                if (setItem.SaleableQuantity < item.Quantity)
                                {
                                    insufficientItems.Add($"Item: {setItem.SetItemName}, Requested: {item.Quantity}, Available: {setItem.SaleableQuantity},Details:{JsonConvert.SerializeObject(setItem)}");
                                }
                                else
                                {
                                    setItem.SaleableQuantity= setItem.SaleableQuantity- item.Quantity;

                                    foreach (var setItemDetail in setItem.SetItemDetails)
                                    {
                                        var detail = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == setItemDetail.ItemId
                                                    && x.Attribute1Value == setItemDetail.Attribute1Value && x.Attribute2Value == setItemDetail.Attribute2Value
                                                    && x.Attribute3Value == setItemDetail.Attribute3Value);
                                        if (detail != null)
                                        {
                                            // Check if the available quantity is sufficient
                                            if (detail.SaleableQuantity < item.Quantity)
                                            {
                                                // Add item to insufficientItems list
                                                insufficientItems.Add($"Item: {detail.ItemName}, Requested: {item.Quantity}, Available: {detail.SaleablePreOrderQuantity},Details:{JsonConvert.SerializeObject(detail)}");
                                            }
                                            else
                                            {
                                                // Proceed with updating the stock if sufficient
                                                detail.SaleableQuantity -= item.Quantity;
                                                detail.StockOnHand -= item.Quantity;

                                                await _itemDetailsRepository.UpdateAsync(detail);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Handle Freebies if applicable
                        if (item.FreebieId != null)
                        {
                            Freebie? freebie = await _freebieRepository.FirstOrDefaultAsync(x => x.Id == item.FreebieId);

                            if (freebie != null && freebie.FreebieAmount > 0)
                            {
                                freebie.FreebieAmount -= item.Quantity;
                                await _freebieRepository.UpdateAsync(freebie);
                            }
                            else
                            {

                                insufficientItems.Add("Insufficient Giftable Quantity");
                            }
                        }
                    }
                }

                // If there are items with insufficient stock, throw a BusinessException with the list
                if (insufficientItems.Count > 0)
                {
                    string errorMessage = string.Join("; ", insufficientItems);
                    throw new BusinessException("409", "Insufficient stock for the following items: " + errorMessage);
                }
            }

            await _orderRepository.InsertAsync(order);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment)
            {
                Order newOrder = await _orderRepository.GetWithDetailsAsync(order.Id);

                if (newOrder.OrderItems.Any(x => x.Item?.IsFreeShipping == true))
                {
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && (x.Item != null || x.Item?.IsFreeShipping == true)).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == true)).ToList(), OrderDelivery.Id);
                    }
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                }
                if (newOrder.OrderItems.Any(x => x.Item?.IsFreeShipping == false))
                {
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }

                }
                await UnitOfWorkManager.Current.SaveChangesAsync();

                OrderDelivery? orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == newOrder.Id);

                if (newOrder.OrderItems.Any(x => x.FreebieId != null))
                {
                    if (orderDelivery is not null)
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
                }

                if (newOrder.OrderItems.Any(a => a.SetItemId is not null && a.SetItemId != Guid.Empty))
                {
                    if (orderDelivery is not null)
                        newOrder.UpdateOrderItem([.. newOrder.OrderItems.Where(w => w.SetItemId is not null && w.SetItemId != Guid.Empty)], orderDelivery.Id);
                }
            }

            if (order.DiscountCodeId != null)
            {
                var discountCode = await _discountCodeRepository.GetAsync(order.DiscountCodeId.Value);
                discountCode.AvailableQuantity = discountCode.AvailableQuantity - 1;
                await _discountCodeRepository.EnsureCollectionLoadedAsync(discountCode, x => x.DiscountCodeUsages);

                if (discountCode.DiscountCodeUsages != null && discountCode.DiscountCodeUsages.Count > 0)
                {
                    var usage = discountCode.DiscountCodeUsages.FirstOrDefault();
                    usage.TotalOrders = usage.TotalOrders + 1;
                    usage.TotalDiscountAmount = usage.TotalDiscountAmount + (int)(order.DiscountAmount.HasValue ? order?.DiscountAmount.Value : 0);
                }
                else
                {
                    var newusage = new DiscountCodeUsage(GuidGenerator.Create(), 1, 0, order.DiscountAmount ?? 0);
                    discountCode.DiscountCodeUsages.Add(newusage);
                }

                await _discountCodeRepository.UpdateAsync(discountCode);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            if (order.UserId != null && order.UserId != Guid.Empty)
            {
                if (order.cashback_amount > 0)
                {

                    var newcashback = await _userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
                    {
                        Amount = (int)order.cashback_amount,
                        ExpirationDate = null,
                        IsActive = true,
                        TransactionDescription = "購物回饋：訂單 #" + order.OrderNo,
                        UserId = order.UserId.Value,
                        ShoppingCreditType = UserShoppingCreditType.Grant
                    });
                    order.cashback_amount = newcashback.Amount;
                    order.cashback_record_id = newcashback.Id;

                    var userCumulativeCredits = await _userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == order.UserId);
                    if (userCumulativeCredits is null)
                    {
                        await _userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = (int)order.cashback_amount, TotalDeductions = 0, TotalRefunds = 0, UserId = order.UserId });
                    }
                    else
                    {
                        userCumulativeCredits.ChangeTotalAmount((int)(userCumulativeCredits.TotalAmount + order.cashback_amount));
                        await _userCumulativeCreditRepository.UpdateAsync(userCumulativeCredits);
                    }
                }

            }
            if (order.UserId != null && order.UserId != Guid.Empty)
            {
                if (order.CreditDeductionAmount > 0)
                {

                    var newdeduction = await _userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
                    {
                        Amount = (int)order.CreditDeductionAmount,
                        ExpirationDate = null,
                        IsActive = true,
                        TransactionDescription = "購物折抵：訂單 #" + order.OrderNo,
                        UserId = order.UserId.Value,
                        ShoppingCreditType = UserShoppingCreditType.Grant
                    });
                    order.CreditDeductionAmount = newdeduction.Amount;
                    order.CreditDeductionRecordId = newdeduction.Id;

                    var userCumulativeCredits = await _userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == order.UserId);
                    if (userCumulativeCredits is null)
                    {
                        await _userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = 0, TotalDeductions = order.CreditDeductionAmount, TotalRefunds = 0, UserId = order.UserId });
                    }
                    else
                    {
                        userCumulativeCredits.ChangeTotalDeductions((int)(userCumulativeCredits.TotalDeductions + order.CreditDeductionAmount));
                        await _userCumulativeCreditRepository.UpdateAsync(userCumulativeCredits);
                    }
                }

            }
            await _orderRepository.UpdateAsync(order);
            await SendEmailAsync(order.Id);
            var validitySettings = (await _paymentGatewayRepository.GetQueryableAsync()).Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
            DateTime expirationTime = DateTime.Now.AddMinutes(10);

            if (validitySettings.Unit == "Days")
            {
                expirationTime = order.CreationTime.AddDays(validitySettings.Period.Value);
            }
            else if (validitySettings.Unit == "Hours")
            {
                expirationTime = order.CreationTime.AddHours(validitySettings.Period.Value);
            }
            else if (validitySettings.Unit == "Minutes")
            {
                expirationTime = order.CreationTime.AddMinutes(validitySettings.Period.Value);
            }

            ExpireOrderBackgroundJobArgs args = new ExpireOrderBackgroundJobArgs { OrderId = order.Id };
            var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, (expirationTime - order.CreationTime));
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
    }

    /// <summary>
    /// get order by Order No
    /// </summary>
    /// <param name="groupBuyId"></param>
    /// <param name="orderNo"></param>
    /// <param name="extraInfo"></param>
    /// <returns></returns>
    public async Task<OrderDto> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo)
    {
        OrderDto order = ObjectMapper.Map<Order, OrderDto>(
            await _orderRepository.GetOrderAsync(groupBuyId, orderNo, extraInfo)
        );

        order.StoreCustomerServiceMessages = await _OrderMessageAppService.GetOrderMessagesAsync(order.Id);

        return order;
    }

    public async Task<OrderDto> UpdateOrderPaymentMethodAsync(OrderPaymentMethodRequest request)
    {
        Order order = await _orderRepository.GetAsync(request.OrderId);

        order.PaymentMethod = request.PaymentMethod;

        return ObjectMapper.Map<Order, OrderDto>(
            await _orderRepository.UpdateAsync(order)
        );
    }

    public async Task<OrderDto> UpdateMerchantTradeNoAsync(OrderPaymentMethodRequest request)
    {
        Order order = await _orderRepository.GetAsync(request.OrderId);

        order.MerchantTradeNo = request.MerchantTradeNo;

        return ObjectMapper.Map<Order, OrderDto>(
            await _orderRepository.UpdateAsync(order)
        );
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
    }

    public async Task<OrderDto> MergeOrdersAsync(List<Guid> Ids)
    {
        decimal TotalAmount = 0; int TotalQuantity = 0;

        Order ord = await _orderRepository.GetWithDetailsAsync(Ids[0]);

        GroupBuy groupBuy = new();
        List<decimal> DeliveriesCost = new();
        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        List<OrderItemsCreateDto> orderItems = [];

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            foreach (Guid id in Ids)
            {
                Order order = await _orderRepository.GetWithDetailsAsync(id);

                TotalAmount += order.TotalAmount;
                TotalQuantity += order.TotalQuantity;
                if (order.DeliveryCost is not null)
                    DeliveriesCost.Add(order.DeliveryCost.Value);
                foreach (OrderItem item in order.OrderItems)
                {
                    if (orderItems.Any(x => x.ItemId == item.ItemId && x.FreebieId == null) ||
                        orderItems.Any(x => x.FreebieId == item.FreebieId && x.ItemId == null))
                    {
                        if (item.ItemId is not null)
                        {
                            OrderItemsCreateDto? orderItem = orderItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault();

                            if (orderItem.SKU is null || orderItem.SKU == item.SKU)
                            {
                                orderItem.TotalAmount += item.TotalAmount;
                                orderItem.Quantity += item.Quantity;
                            }

                            else if (item.SKU is not null && orderItems.Any(a => a.SKU == item.SKU))
                            {
                                orderItem = orderItems.First(f => f.SKU == item.SKU);

                                orderItem.TotalAmount += item.TotalAmount;
                                orderItem.Quantity += item.Quantity;
                            }

                            else
                            {
                                OrderItemsCreateDto orderItemCreate = new()
                                {
                                    ItemId = item.ItemId,
                                    SetItemId = item.SetItemId,
                                    FreebieId = item.FreebieId,
                                    ItemType = item.ItemType,
                                    Spec = item.Spec,
                                    ItemPrice = item.ItemPrice,
                                    TotalAmount = item.TotalAmount,
                                    Quantity = item.Quantity,
                                    SKU = item.SKU
                                };

                                orderItems.Add(orderItemCreate);
                            }
                        }
                        else
                        {
                            OrderItemsCreateDto? orderItem = orderItems.Where(x => x.FreebieId == item.FreebieId).FirstOrDefault();

                            orderItem.TotalAmount += item.TotalAmount;
                            orderItem.Quantity += item.Quantity;
                        }
                    }
                    else
                    {
                        OrderItemsCreateDto orderItem = new()
                        {
                            ItemId = item.ItemId,
                            SetItemId = item.SetItemId,
                            FreebieId = item.FreebieId,
                            ItemType = item.ItemType,
                            Spec = item.Spec,
                            ItemPrice = item.ItemPrice,
                            TotalAmount = item.TotalAmount,
                            Quantity = item.Quantity,
                            SKU = item.SKU
                        };

                        orderItems.Add(orderItem);
                    }
                }
            }

            orderItems = [.. orderItems.OrderBy(o => o.ItemType)];

            Order order1 = await _orderManager.CreateAsync(
                ord.GroupBuyId,
                ord.IsIndividual,
                ord.CustomerName,
                ord.CustomerPhone,
                ord.CustomerEmail,
                ord.PaymentMethod,
                ord.InvoiceType,
                ord.InvoiceNumber,
                ord.CarrierId,
                ord.UniformNumber,
                ord.TaxTitle,
                ord.IsAsSameBuyer,
                ord.RecipientName,
                ord.RecipientPhone,
                ord.RecipientEmail,
                ord.DeliveryMethod,
                ord.PostalCode,
                ord.City,
                ord.District,
                ord.Road,
                ord.AddressDetails,
                ord.Remarks,
                ord.ReceivingTime,
                TotalQuantity,
                TotalAmount,
                ord.ReturnStatus,
                OrderType.NewMarge
            );

            order1.ShippingStatus = ord.ShippingStatus;

            order1.MerchantTradeNo = ord.OrderNo;

            foreach (OrderItemsCreateDto item in orderItems)
            {
                _orderManager.AddOrderItem(
                    order1,
                    item.ItemId,
                    item.SetItemId,
                    item.FreebieId,
                    item.ItemType,
                    order1.Id,
                    item.Spec,
                    item.ItemPrice,
                    item.TotalAmount,
                    item.Quantity,
                    item.SKU,
                    item.DeliveryTemperature,
                    item.DeliveryTemperatureCost
                );
            }
            if (DeliveriesCost.Count > 0)
                order1.DeliveryCost = DeliveriesCost.Max();
            await _orderRepository.InsertAsync(order1);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (order1.ShippingStatus is ShippingStatus.PrepareShipment)
            {
                if (order1.OrderItems.Any(x => x.Item?.IsFreeShipping == true))
                {
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && (x.Item != null || x.Item?.IsFreeShipping == true)).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == true)).ToList(), OrderDelivery.Id);
                    }
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x?.Item.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                }
                if (order1.OrderItems.Any(x => x.Item?.IsFreeShipping == false))
                {
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                }
                await UnitOfWorkManager.Current.SaveChangesAsync();

                if (order1.OrderItems.Any(x => x.FreebieId != null))
                {
                    var OrderDelivery1 = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == order1.Id);
                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.FreebieId != null).ToList(), OrderDelivery1.Id);
                }

                await _orderRepository.UpdateAsync(order1);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

            foreach (Guid id in Ids)
            {
                Order ord1 = await _orderRepository.GetWithDetailsAsync(id);

                ord1.OrderType = OrderType.MargeToNew;

                ord1.ShippingStatus = ShippingStatus.Closed;

                ord1.TotalAmount = 0;

                foreach (OrderItem item in ord1.OrderItems)
                {
                    item.ItemPrice = 0; item.TotalAmount = 0;
                }

                await _orderRepository.UpdateAsync(ord1, autoSave: true);
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();

            await _emailAppService.SendMergeOrderEmailAsync(Ids, order1.Id);
            
            return ObjectMapper.Map<Order, OrderDto>(order1);
        }
    }

    public async Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId)
    {
        Order newOrder = new();

        Order ord = await _orderRepository.GetWithDetailsAsync(OrderId);

        decimal TotalAmount = ord.TotalAmount; int TotalQuantity = ord.TotalQuantity;

        GroupBuy groupBuy = new();

        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        List<OrderItemsCreateDto> orderItems = [];
        Guid splitOrderId = Guid.Empty;
        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            foreach (OrderItem item in ord.OrderItems)
            {
                if (OrderItemIds.Any(a => a == item.Id))
                {
                    OrderItemsCreateDto orderItem = new();

                    orderItem = ObjectMapper.Map<OrderItem, OrderItemsCreateDto>(item);

                    TotalAmount -= item.TotalAmount;
                    TotalQuantity -= item.Quantity;

                    await _orderItemRepository.DeleteAsync(item.Id);

                    Order order1 = await _orderManager.CreateAsync(
                        ord.GroupBuyId,
                        ord.IsIndividual,
                        ord.CustomerName,
                        ord.CustomerPhone,
                        ord.CustomerEmail,
                        ord.PaymentMethod,
                        ord.InvoiceType,
                        ord.InvoiceNumber,
                        ord.CarrierId,
                        ord.UniformNumber,
                        ord.TaxTitle,
                        ord.IsAsSameBuyer,
                        ord.RecipientName,
                        ord.RecipientPhone,
                        ord.RecipientEmail,
                        ord.DeliveryMethod,
                        ord.PostalCode,
                        ord.City,
                        ord.District,
                        ord.Road,
                        ord.AddressDetails,
                        ord.Remarks,
                        ord.ReceivingTime,
                        orderItem.Quantity,
                        orderItem.TotalAmount,
                        ord.ReturnStatus,
                        OrderType.NewSplit,
                        ord.Id
                    );
                    splitOrderId = order1.Id;
                    order1.ShippingStatus = ord.ShippingStatus;

                    _orderManager.AddOrderItem(
                        order1,
                        orderItem.ItemId,
                        orderItem.SetItemId,
                        orderItem.FreebieId,
                        orderItem.ItemType,
                        order1.Id,
                        orderItem.Spec,
                        orderItem.ItemPrice,
                        orderItem.TotalAmount,
                        orderItem.Quantity,
                        orderItem.SKU,
                        orderItem.DeliveryTemperature,
                        orderItem.DeliveryTemperatureCost
                    );

                    await _orderRepository.InsertAsync(order1);
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    newOrder = order1;

                    OrderItem? orderItem1 = newOrder.OrderItems.FirstOrDefault();

                    if (order1.ShippingStatus is ShippingStatus.PrepareShipment)
                    {
                        OrderDelivery oD = new(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, string.Empty, order1.Id);

                        oD = await _orderDeliveryRepository.InsertAsync(oD);

                        order1.UpdateOrderItem(
                            [.. order1.OrderItems.Where(w => w.DeliveryTemperature == orderItem1?.DeliveryTemperature &&
                                                             w.Item?.IsFreeShipping == orderItem1?.Item?.IsFreeShipping)],
                            oD.Id
                        );

                        await _orderRepository.UpdateAsync(order1);
                    }
                }

            }
            if (ord.OrderItems.Count <= 0)
            {
                newOrder.TotalAmount += TotalAmount;
                await _orderRepository.UpdateAsync(newOrder);
                ord.TotalAmount = 0;
                ord.TotalQuantity = 0;
            }
            else
            {
                ord.TotalAmount = TotalAmount;
                ord.TotalQuantity = TotalQuantity;
            }

            ord.OrderType = OrderType.SplitToNew;

            await _orderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await _emailAppService.SendSplitOrderEmailAsync(OrderId, splitOrderId);
            
            return ObjectMapper.Map<Order, OrderDto>(ord);
        }
    }
    public async Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId)
    {
        Order newOrder = new();

        string? returnedOrderItemIds = null;

        Order ord = await _orderRepository.GetWithDetailsAsync(OrderId);

        decimal TotalAmount = ord.TotalAmount;

        int TotalQuantity = ord.TotalQuantity;

        GroupBuy groupBuy = new();

        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        List<OrderItemsCreateDto> orderItems = [];

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order1 = await _orderManager.CreateAsync(
                ord.GroupBuyId,
                ord.IsIndividual,
                ord.CustomerName,
                ord.CustomerPhone,
                ord.CustomerEmail,
                ord.PaymentMethod,
                ord.InvoiceType,
                ord.InvoiceNumber,
                ord.CarrierId,
                ord.UniformNumber,
                ord.TaxTitle,
                ord.IsAsSameBuyer,
                ord.RecipientName,
                ord.RecipientPhone,
                ord.RecipientEmail,
                ord.DeliveryMethod,
                ord.PostalCode,
                ord.City,
                ord.District,
                ord.Road,
                ord.AddressDetails,
                ord.Remarks,
                ord.ReceivingTime,
                ord.TotalQuantity,
                ord.TotalAmount,
                ord.ReturnStatus,
                null,
                ord.Id
            );
            order1.ShippingStatus = ord.ShippingStatus;
            order1.MerchantTradeNo = ord.MerchantTradeNo;
            order1.StoreId = ord.StoreId;
            order1.CVSStoreOutSide = ord.CVSStoreOutSide;
            order1.DeliveryCostForNormal = ord.DeliveryCostForNormal;
            order1.DeliveryCostForFreeze = ord.DeliveryCostForFreeze;
            order1.DeliveryCostForFrozen = ord.DeliveryCostForFrozen;
            order1.DeliveryCost = ord.DeliveryCost;
            order1.GWSR = ord.GWSR;
            order1.TradeNo = ord.TradeNo;
            order1.OrderRefundType = OrderRefundType.PartialRefund;
            //order1.OrderStatus = OrderStatus.Returned;
            foreach (var item in ord.OrderItems)
            {
                if (OrderItemIds.Any(x => x == item.Id))
                {
                    returnedOrderItemIds = returnedOrderItemIds.IsNullOrEmpty() ?
                                            item.Id.ToString() :
                                            string.Join(',', returnedOrderItemIds, item.Id.ToString());

                    OrderItemsCreateDto orderItem = new();
                    orderItem.ItemId = item.ItemId;
                    orderItem.SetItemId = item.SetItemId;
                    orderItem.FreebieId = item.FreebieId;
                    orderItem.ItemType = item.ItemType;

                    orderItem.Spec = item.Spec;
                    orderItem.ItemPrice = item.ItemPrice;
                    orderItem.TotalAmount = item.TotalAmount;
                    orderItem.Quantity = item.Quantity;
                    orderItem.SKU = item.SKU;

                    //ord.TotalAmount = ord.TotalAmount - item.TotalAmount;
                    ord.TotalQuantity = ord.TotalQuantity - item.Quantity;
                    //await _orderItemRepository.DeleteAsync(item.Id);

                    _orderManager.AddOrderItem(
                        order1,
                        orderItem.ItemId,
                        orderItem.SetItemId,
                        orderItem.FreebieId,
                        orderItem.ItemType,
                        order1.Id,
                        orderItem.Spec,
                        orderItem.ItemPrice,
                        orderItem.TotalAmount,
                        orderItem.Quantity,
                        orderItem.SKU, orderItem.DeliveryTemperature,
                        orderItem.DeliveryTemperatureCost
                    );

                    //item.TotalAmount = item.TotalAmount - item.TotalAmount;
                    //item.ItemPrice = item.ItemPrice - item.ItemPrice;
                    item.Quantity = item.Quantity - item.Quantity;
                    await _orderRepository.InsertAsync(order1);
                    await _orderRepository.UpdateAsync(ord);
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    newOrder = order1;
                }
            }
            newOrder.TotalAmount = newOrder.OrderItems.Sum(x => x.TotalAmount);
            newOrder.TotalQuantity = newOrder.OrderItems.Sum(x => x.Quantity);
            var OrderDelivery1 = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == ord.Id);
            await _orderDeliveryRepository.DeleteAsync(OrderDelivery1.Id);
            foreach (var item in ord.OrderItems.Where(x => x.ItemPrice >= 0))
            {
                if (ord.ShippingStatus == ShippingStatus.PrepareShipment)
                {
                    var orderItemList = ord.OrderItems.Where(x => x.TotalAmount >= 0).ToList();
                    foreach (var orderItem in orderItemList)
                    {
                        if (orderItem.Item?.IsFreeShipping == true)
                        {
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Normal && orderItem.Item != null || orderItem.Item?.IsFreeShipping == true)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), ord.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", ord.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == true)).ToList(), OrderDelivery.Id);
                            }
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Freeze && orderItem.Item?.IsFreeShipping == true)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), ord.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", ord.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                            }
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Frozen && orderItem.Item?.IsFreeShipping == true)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", ord.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x?.Item.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                            }
                        }
                        if (orderItem.Item?.IsFreeShipping == false)
                        {
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Normal && orderItem.Item?.IsFreeShipping == false)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), ord.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                            }
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Freeze && orderItem.Item?.IsFreeShipping == false)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), ord.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order1.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                            }
                            if (orderItem.DeliveryTemperature == ItemStorageTemperature.Frozen && orderItem.Item?.IsFreeShipping == false)
                            {
                                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), ord.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", ord.Id);
                                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                ord.UpdateOrderItem(ord.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                            }

                        }
                        await _orderRepository.UpdateAsync(ord);
                    }
                }
            }

            ord.RefundAmount += newOrder.TotalAmount;

            ord.ReturnedOrderItemIds = returnedOrderItemIds;

            await _orderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await _refundAppService.CreateAsync(newOrder.Id);

            return ObjectMapper.Map<Order, OrderDto>(ord);
        }
    }
    public async Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        order.ShippingStatus = status;
        //order.ClosedBy = CurrentUser.Name;
        order.CancellationDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnResult = ObjectMapper.Map<Order, OrderDto>(order);
        if (status == ShippingStatus.Delivered)
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (order.InvoiceNumber.IsNullOrEmpty())
            {
                if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Delivered)
                {
                    if (order.GroupBuy.IssueInvoice)
                    {
                        order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                        await UnitOfWorkManager.Current.SaveChangesAsync();
                        //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                        var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                        if (invoiceDely == 0)
                        {
                            var result = await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                            returnResult.InvoiceMsg = result;


                        }
                        else
                        {
                            var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                            GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                            var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                        }
                    }
                }
            }
        }
        return returnResult;


    }
    public async Task RefundAmountAsync(double amount, Guid OrderId)
    {
        Order ord = await _orderRepository.GetWithDetailsAsync(OrderId);

        ord.RefundAmount += (decimal)amount;

        GroupBuy groupBuy = new();

        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order1 = await _orderManager.CreateAsync(
                ord.GroupBuyId,
                ord.IsIndividual,
                ord.CustomerName,
                ord.CustomerPhone,
                ord.CustomerEmail,
                ord.PaymentMethod,
                ord.InvoiceType,
                ord.InvoiceNumber,
                ord.CarrierId,
                ord.UniformNumber,
                ord.TaxTitle,
                ord.IsAsSameBuyer,
                ord.RecipientName,
                ord.RecipientPhone,
                ord.RecipientEmail,
                ord.DeliveryMethod,
                ord.PostalCode,
                ord.City,
                ord.District,
                ord.Road,
                ord.AddressDetails,
                ord.Remarks,
                ord.ReceivingTime,
                0,
                (decimal)amount,
                ord.ReturnStatus,
                null,
                ord.Id
            );

            order1.ShippingStatus = ord.ShippingStatus;
            order1.MerchantTradeNo = ord.MerchantTradeNo;
            order1.StoreId = ord.StoreId;
            order1.CVSStoreOutSide = ord.CVSStoreOutSide;
            order1.DeliveryCostForNormal = ord.DeliveryCostForNormal;
            order1.DeliveryCostForFreeze = ord.DeliveryCostForFreeze;
            order1.DeliveryCostForFrozen = ord.DeliveryCostForFrozen;
            order1.DeliveryCost = ord.DeliveryCost;
            order1.GWSR = ord.GWSR;
            order1.TradeNo = ord.TradeNo;
            order1.OrderRefundType = OrderRefundType.PartialRefund;

            await _orderRepository.InsertAsync(order1);

            await _orderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await _refundAppService.CreateAsync(order1.Id);
        }
    }
    public async Task<OrderDto> GetWithDetailsAsync(Guid id)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(id);

        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        if (input.Sorting.IsNullOrEmpty()) input.Sorting = $"{nameof(Order.CreationTime)} desc";

        long totalCount = await _orderRepository.CountAsync(input.Filter,
                                                            input.GroupBuyId,
                                                            input.StartDate,
                                                            input.EndDate,
                                                            input.OrderStatus,
                                                            input.ShippingStatus,
                                                            input.DeliveryMethod);

        List<Order> items = await _orderRepository.GetListAsync(input.SkipCount,
                                                                input.MaxResultCount,
                                                                input.Sorting,
                                                                input.Filter,
                                                                input.GroupBuyId,
                                                                input.OrderIds,
                                                                input.StartDate,
                                                                input.EndDate,
                                                                input.OrderStatus,
                                                                input.ShippingStatus,
                                                                input.DeliveryMethod);

        try
        {
            List<OrderDto> dtos = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

            if (hideCredentials)
            {
                GroupBuy groupbuy = await _groupBuyRepository.GetAsync(input.GroupBuyId.Value);

                if (groupbuy.ProtectPrivacyData) dtos.HideCredentials();
            }

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = dtos
            };
        }
        catch (Exception e)
        {

            throw;
        }
    }
    public async Task<PagedResultDto<OrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Order.CreationTime)} desc";
        }

        var totalCount = await _orderRepository.CountAllAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate, input.OrderStatus);

        var items = await _orderRepository.GetAllListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate, input.OrderStatus);

        try
        {
            var dtos = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

            if (hideCredentials)
            {
                var groupbuy = await _groupBuyRepository.GetAsync(input.GroupBuyId.Value);
                if (groupbuy.ProtectPrivacyData)
                {
                    dtos.HideCredentials();
                }
            }

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = dtos
            };
        }
        catch (Exception e)
        {

            throw;
        }
    }
    public async Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountAsync(input.Filter, input.GroupBuyId);

            var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }
    }

    public async Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Order.CreationTime)} desc";
        }

        var totalCount = await _orderRepository.ReturnOrderCountAsync(input.Filter, input.GroupBuyId);

        var items = await _orderRepository.GetReturnListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId);

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }

    public async Task<PagedResultDto<OrderDto>> GetReconciliationListAsync(GetOrderListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Order.LastModificationTime)} desc";
        }

        var totalCount = await _orderRepository.CountReconciliationAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

        var items = await _orderRepository.GetReconciliationListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }
    public async Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Order.CreationTime)} desc";
        }

        var totalCount = await _orderRepository.CountVoidAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

        var items = await _orderRepository.GetVoidListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }
    [Authorize(PikachuPermissions.Orders.AddStoreComment)]
    public async Task AddStoreCommentAsync(Guid id, string comment)
    {
        var order = await _orderRepository.GetAsync(id);
        await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
        _orderManager.AddStoreComment(order, comment);
    }

    [Authorize(PikachuPermissions.Orders.AddStoreComment)]
    public async Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
    {
        var order = await _orderRepository.GetAsync(id);
        await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
        var storeComment = order.StoreComments.First(c => c.Id == commentId);
        if (storeComment.CreatorId != CurrentUser.Id)
        {
            throw new UnauthorizedAccessException();
        }
        storeComment.Comment = comment;
    }

    public async Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
    {
        var order = await _orderRepository.GetAsync(id);
        order.RecipientName = input.RecipientName;
        order.RecipientPhone = input.RecipientPhone;
        order.PostalCode = input.PostalCode;
        order.District = input.District;
        order.City = input.City;
        order.Road = input.Road;
        order.AddressDetails = input.AddressDetails;
        order.OrderStatus = input.OrderStatus;
        order.StoreId = input.StoreId;
        order.CVSStoreOutSide = input.CVSStoreOutSide;

        order.RecipientNameDbsNormal = input.RecipientNameDbsNormal;
        order.RecipientNameDbsFreeze = input.RecipientNameDbsFreeze;
        order.RecipientNameDbsFrozen = input.RecipientNameDbsFrozen;
        order.RecipientPhoneDbsNormal = input.RecipientPhoneDbsNormal;
        order.RecipientPhoneDbsFreeze = input.RecipientPhoneDbsFreeze;
        order.RecipientPhoneDbsFrozen = input.RecipientPhoneDbsFrozen;
        order.PostalCodeDbsNormal = input.PostalCodeDbsNormal;
        order.PostalCodeDbsFreeze = input.PostalCodeDbsFreeze;
        order.PostalCodeDbsFrozen = input.PostalCodeDbsFrozen;
        order.CityDbsNormal = input.CityDbsNormal;
        order.CityDbsFreeze = input.CityDbsFreeze;
        order.CityDbsFrozen = input.CityDbsFrozen;
        order.AddressDetailsDbsNormal = input.AddressDetailsDbsNormal;
        order.AddressDetailsDbsFreeze = input.AddressDetailsDbsFreeze;
        order.AddressDetailsDbsFrozen = input.AddressDetailsDbsFrozen;
        order.StoreIdNormal = input.StoreIdNormal;
        order.StoreIdFreeze = input.StoreIdFreeze;
        order.StoreIdFrozen = input.StoreIdFrozen;
        order.CVSStoreOutSideNormal = input.CVSStoreOutSideNormal;
        order.CVSStoreOutSideFreeze = input.CVSStoreOutSideFreeze;
        order.CVSStoreOutSideFrozen = input.CVSStoreOutSideFrozen;

        await _orderRepository.UpdateAsync(order);

        if (input.ShouldSendEmail)
        {
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await _emailAppService.SendOrderUpdateEmailAsync(order.Id);
        }

        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus)
    {
        var order = await _orderRepository.GetAsync(id);
        order.ReturnStatus = orderReturnStatus;
        if (orderReturnStatus == OrderReturnStatus.Reject)
        {
            order.OrderStatus = OrderStatus.Open;

        }

        if (orderReturnStatus == OrderReturnStatus.Approve)
        {

            if (order.OrderStatus == OrderStatus.Returned)
            {
                await _refundAppService.CreateAsync(id);
                var refund = (await _refundRepository.GetQueryableAsync()).Where(x => x.OrderId == order.Id).FirstOrDefault();
                await _refundAppService.UpdateRefundReviewAsync(refund.Id, RefundReviewStatus.Proccessing);
                await _refundAppService.SendRefundRequestAsync(refund.Id);
            }
            order.ReturnStatus = OrderReturnStatus.Processing;
        }
        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
        {
            order.ExchangeBy = CurrentUser.UserName;
            order.ExchangeTime = DateTime.Now;
            order.ShippingStatus = ShippingStatus.Exchange;
        }
        await _orderRepository.UpdateAsync(order);
    }

    public async Task ExchangeOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Exchange;

        await _orderRepository.UpdateAsync(order);


    }
    public async Task ReturnOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Returned;

        await _orderRepository.UpdateAsync(order);


    }
    public async Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems)
    {
        var order = await _orderRepository.GetAsync(id);
        await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.OrderItems);

        foreach (var item in orderItems)
        {
            var orderItem = order.OrderItems.First(o => o.Id == item.Id);
            orderItem.Quantity = item.Quantity;
            orderItem.ItemPrice = item.ItemPrice;
            orderItem.TotalAmount = item.TotalAmount;
        }

        order.TotalQuantity = order.OrderItems.Sum(o => o.Quantity);
        order.TotalAmount = order.OrderItems.Sum(o => o.TotalAmount);
        await _orderRepository.UpdateAsync(order);
    }
    public async Task CancelOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        order.OrderStatus = OrderStatus.Closed;
        order.CancellationDate = DateTime.Now;
        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id, OrderStatus.Closed);
    }
    public async Task VoidInvoice(Guid id, string reason)
    {
        var order = await _orderRepository.GetAsync(id);
        order.IsVoidInvoice = true;
        order.VoidReason = reason;
        order.VoidUser = CurrentUser.Name;
        order.VoidDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.InvoiceVoided;
        order.LastModificationTime = DateTime.Now;
        order.LastModifierId = CurrentUser.Id;
        await _orderRepository.UpdateAsync(order);
        await _electronicInvoiceAppService.CreateVoidInvoiceAsync(id, reason);

    }
    public async Task CreditNoteInvoice(Guid id, string reason)
    {
        Order order = await _orderRepository.GetAsync(id);

        order.IsVoidInvoice = true;
        order.CreditNoteReason = reason;
        order.CreditNoteUser = CurrentUser.Name;
        order.CreditNoteDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.CreditNote;

        await _orderRepository.UpdateAsync(order);

        await _electronicInvoiceAppService.CreateCreditNoteAsync(id);
    }
    public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        order.DeliveryMethod = input.DeliveryMethod;
        order.ShippingNumber = input.ShippingNumber;
        order.ShippingStatus = ShippingStatus.Shipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        if (order.ShippingNumber != null)
        {
            await SendEmailAsync(order.Id);
        }
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderShipped(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        order.ShippingStatus = ShippingStatus.Shipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await _emailAppService.SendLogisticsEmailAsync(order.Id);
        await SendEmailAsync(order.Id);
        var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        string result = await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);

                        returnOrder.InvoiceMsg = result;
                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        return returnOrder;
        // await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);

    }
    public async Task<OrderDto> OrderToBeShipped(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        order.ShippingStatus = ShippingStatus.ToBeShipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.ToBeShipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        var result = await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                        returnOrder.InvoiceMsg = result;
                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        // await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
        return returnOrder;

    }
    public async Task<OrderDto> OrderClosed(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        order.ShippingStatus = ShippingStatus.Closed;
        order.ClosedBy = CurrentUser.Name;
        order.CancellationDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderComplete(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        order.ShippingStatus = ShippingStatus.Completed;
        order.CompletedBy = CurrentUser.Name;
        order.CompletionTime = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnResult = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Completed)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        var result = await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                        returnResult.InvoiceMsg = result;

                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        return returnResult;

    }

    private async Task SendEmailAsync(Guid id, OrderStatus? orderStatus = null)
    {
        await _emailAppService.SendOrderStatusEmailAsync(id);
    }

    public async Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo, PaymentMethods? paymentMethod = null)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            Order order = await _orderRepository.GetAsync(id);

            order.MerchantTradeNo = merchantTradeNo;

            order.CheckMacValue = checkMacValue;

            if (paymentMethod.HasValue) order.PaymentMethod = paymentMethod;

            await _orderRepository.UpdateAsync(order);
        }
    }

    [AllowAnonymous]
    public async Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            Order order = await _orderRepository.GetAsync(id);

            order.MerchantTradeNo = merchantTradeNo;

            order.CheckMacValue = checkMacValue;

            await _orderRepository.UpdateAsync(order);
        }
    }

    [AllowAnonymous]
    public async Task AddCheckMacValueAsync(Guid id, string checkMacValue)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var order = await _orderRepository.GetAsync(id);
            order.CheckMacValue = checkMacValue;

            await _orderRepository.UpdateAsync(order);
        }
    }

    public string GenerateMerchantTradeNo(string orderNo)
    {
        Random random = new();

        int maxLength = 20;

        int availableLength = maxLength - orderNo.Length;

        string randomNumeric = random.Next(0, (int)Math.Pow(10, availableLength)).ToString().PadLeft(availableLength, '0');

        return string.Concat(orderNo, randomNumeric);
    }

    public async Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg)
    {
        Order order = await _orderRepository.GetOrderByMerchantTradeNoAsync(merchantTradeNo);

        order.EcpayLogisticsStatus = rtnMsg;

        if (order.ShippingStatus < ShippingStatus.ToBeShipped) order.ShippingStatus = ShippingStatus.ToBeShipped;

        await _orderRepository.UpdateAsync(order);
    }

    [AllowAnonymous]
    public async Task<string> HandlePaymentAsync(PaymentResult paymentResult)
    {
        if (paymentResult.SimulatePaid is 0)
        {
            Order order = new();
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (paymentResult.OrderId is null)
                {
                    order = await _orderRepository
                                   .FirstOrDefaultAsync(o => o.MerchantTradeNo == paymentResult.MerchantTradeNo)
                                   ?? throw new EntityNotFoundException();

                    order = await _orderRepository.GetWithDetailsAsync(order.Id);

                    if (paymentResult.CustomField1 != order.CheckMacValue) throw new Exception();

                    if (paymentResult.TradeAmt != order.TotalAmount) throw new Exception();
                }
                else
                {
                    order = await _orderRepository
                                      .FirstOrDefaultAsync(o => o.Id == paymentResult.OrderId)
                                      ?? throw new EntityNotFoundException();
                    order = await _orderRepository.GetWithDetailsAsync(order.Id);
                }

                order.GWSR = paymentResult.GWSR;
                order.TradeNo = paymentResult.TradeNo;
                order.ShippingStatus = ShippingStatus.PrepareShipment;
                order.PrepareShipmentBy = CurrentUser.Name ?? "System";
                _ = DateTime.TryParse(paymentResult.PaymentDate, out DateTime parsedDate);
                order.PaymentDate = paymentResult.OrderId == null ? parsedDate : DateTime.Now;
                if (order.OrderItems.Any(x => x.Item?.IsFreeShipping == true))
                {
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && (x.Item != null || x.Item?.IsFreeShipping == true)).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == true)).ToList(), OrderDelivery.Id);
                    }
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                }
                if (order.OrderItems.Any(x => x.Item?.IsFreeShipping == false))
                {
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                    if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).Any())
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }

                }
                //await UnitOfWorkManager.Current.SaveChangesAsync();

                OrderDelivery? orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == order.Id);

                if (order.OrderItems.Any(x => x.FreebieId != null))
                {
                    if (orderDelivery is not null)
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
                }

                if (order.OrderItems.Any(a => a.SetItemId is not null && a.SetItemId != Guid.Empty))
                {
                    if (orderDelivery is not null)
                        order.UpdateOrderItem([.. order.OrderItems.Where(w => w.SetItemId is not null && w.SetItemId != Guid.Empty)], orderDelivery.Id);
                }

                await _orderRepository.UpdateAsync(order);
                await SendEmailAsync(order.Id);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                if (order.InvoiceNumber.IsNullOrEmpty())
                {
                    if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Processing)
                    {
                        if (order.GroupBuy.IssueInvoice)
                        {
                            order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                            //var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
                            var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                            if (invoiceDely == 0)
                            {
                                string invoiceMsg = await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                                await _orderRepository.UpdateAsync(order);
                                await UnitOfWorkManager.Current.SaveChangesAsync();
                                return invoiceMsg;
                            }
                            else
                            {
                                var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                                GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                                var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                            }
                        }
                    }
                }
                return "";
            }
        }
        return "";
    }

    [AllowAnonymous]
    public async Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync(Guid id)
    {
        GroupBuy groupBuy = new();
        using (_dataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await _groupBuyRepository.GetAsync(id);
        }
        using (CurrentTenant.Change(groupBuy.TenantId))
        {
            PaymentGateway? paymentGateway = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);

            PaymentGatewayDto paymentGatewayDto = ObjectMapper.Map<PaymentGateway, PaymentGatewayDto>(paymentGateway);

            PropertyInfo[] properties = typeof(PaymentGatewayDto).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = (string?)property.GetValue(paymentGatewayDto);

                    if (!string.IsNullOrEmpty(value))
                    {
                        var decryptedValue = _stringEncryptionService.Decrypt(value);
                        property.SetValue(paymentGatewayDto, decryptedValue);
                    }
                }
            }

            return paymentGatewayDto;
        }
    }

    public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input)
    {
        var items = await _orderRepository.GetListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
        var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

        // Create a dictionary for localized headers
        var headers = new Dictionary<string, string>
        {
            { "OrderNumber", _l["OrderNo"] },
            { "OrderDate", _l["OrderDate"] },
            { "CustomerName", _l["CustomerName"] },
            { "Email", _l["Email"] },
            { "RecipientInformation", _l["RecipientInformation"] },
            { "ShippingMethod", _l["ShippingMethod"] },
            { "Address", _l["Address"] },
            { "Notes", _l["Notes"] },
            { "MerchantNotes", _l["MerchantNotes"] },
            { "OrderedItems", _l["OrderedItems"] },
            { "InvoiceStatus", _l["InvoiceStatus"] },
            { "ShippingStatus", _l["ShippingStatus"] },
            { "PaymentMethod", _l["PaymentMethod"] },
            { "CheckoutAmount", _l["CheckoutAmount"] }
        };

        var excelContent = Results.Select(x => new Dictionary<string, object>
        {
            { headers["OrderNumber"], x.OrderNo },
            { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
            { headers["CustomerName"], x.CustomerName },
            { headers["Email"], x.CustomerEmail },
            { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
            { headers["ShippingMethod"], _l[x.DeliveryMethod.ToString()] },
            { headers["Address"], x.AddressDetails },
            { headers["Notes"], x.Remarks },
            { headers["MerchantNotes"], x.Remarks },
            { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
            )},
            { headers["InvoiceStatus"], _l[x.InvoiceStatus.ToString()] },
            { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
            { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
            { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelContent);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input)
    {
        var items = await _orderRepository.GetReconciliationListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
        var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

        // Create a dictionary for localized headers
        var headers = new Dictionary<string, string>
        {
            { "OrderNumber", _l["OrderNo"] },
            { "OrderDate", _l["OrderDate"] },
            { "CustomerName", _l["CustomerName"] },
            { "Email", _l["Email"] },
            { "RecipientInformation", _l["RecipientInformation"] },
            { "ShippingMethod", _l["ShippingMethod"] },
            { "Address", _l["Address"] },
            { "Notes", _l["Notes"] },
            { "MerchantNotes", _l["MerchantNotes"] },
            { "OrderedItems", _l["OrderedItems"] },
            { "InvoiceStatus", _l["InvoiceStatus"] },
            { "ShippingStatus", _l["ShippingStatus"] },
            { "PaymentMethod", _l["PaymentMethod"] },
            { "CheckoutAmount", _l["CheckoutAmount"] },
            { "DeliveryMethod", _l["DeliveryMethod"] }
        };

        var excelContent = Results.Select(x => new Dictionary<string, object>
        {
            { headers["OrderNumber"], x.OrderNo },
            { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
            { headers["CustomerName"], x.CustomerName },
            { headers["Email"], x.CustomerEmail },
            { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
            { headers["ShippingMethod"], _l[x.DeliveryMethod.ToString()] },
            { headers["Address"], x.AddressDetails },
            { headers["Notes"], x.Remarks },
            { headers["MerchantNotes"], x.Remarks },
            { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
            )},
            { headers["InvoiceStatus"], _l[x.InvoiceStatus.ToString()] },
            { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
            { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
            { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") },
            { headers["DeliveryMethod"], _l[x.DeliveryMethod.ToString()] }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelContent);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "Reconciliation Report.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId)
    {
        await _orderRepository.UpdateOrdersIfIsEnterpricePurchaseAsync(groupBuyId);
    }

    public async Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync()
    {
        return await _orderRepository.GetTotalDeliveryTemperatureCountsAsync();
    }

    public async Task<(decimal PaidAmount, decimal UnpaidAmount, decimal RefundedAmount)> GetOrderStatusAmountsAsync(Guid userId)
    {
        // Sum of Paid orders
        var paidAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order => order.UserId == userId && (
                (order.PaymentMethod != PaymentMethods.CashOnDelivery &&
                    (order.ShippingStatus == ShippingStatus.PrepareShipment ||
                     order.ShippingStatus == ShippingStatus.ToBeShipped ||
                     order.ShippingStatus == ShippingStatus.Shipped ||
                     order.ShippingStatus == ShippingStatus.Delivered)) ||
                (order.PaymentMethod == PaymentMethods.CashOnDelivery &&
                    order.ShippingStatus == ShippingStatus.Delivered)

                    ))

            .Sum(order => order.TotalAmount);

        // Sum of Unpaid/Due orders
        var unpaidAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order => order.UserId == userId && (
                (order.PaymentMethod == PaymentMethods.CashOnDelivery &&
                    (order.ShippingStatus == ShippingStatus.WaitingForPayment ||
                     order.ShippingStatus == ShippingStatus.PrepareShipment ||
                     order.ShippingStatus == ShippingStatus.ToBeShipped ||
                     order.ShippingStatus == ShippingStatus.Shipped)) ||
                (order.PaymentMethod != PaymentMethods.CashOnDelivery &&
                    order.ShippingStatus == ShippingStatus.WaitingForPayment)
                    ))
            .Sum(order => order.TotalAmount);

        // Sum of Refunded orders
        var refundedAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order => order.UserId == userId && order.IsRefunded)
            .Sum(order => order.RefundAmount);

        return (paidAmount, unpaidAmount, refundedAmount);
    }

    public async Task<(int Open, int Exchange, int Return)> GetOrderStatusCountsAsync(Guid userId)
    {
        // Sum of Paid orders
        var paidAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order =>
                (order.OrderStatus == OrderStatus.Open)
                    && order.UserId == userId
                    )

            .Count();

        // Sum of Unpaid/Due orders
        var unpaidAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order =>
                (order.OrderStatus == OrderStatus.Exchange)
                     && order.UserId == userId)
            .Count();

        // Sum of Refunded orders
        var refundedAmount = (await _orderRepository.GetQueryableAsync())
            .Where(order => order.OrderStatus == OrderStatus.Exchange && order.UserId == userId)
            .Count();

        return (paidAmount, unpaidAmount, refundedAmount);
    }
    public async Task ExpireOrderAsync(Guid OrderId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var order = await _orderRepository.GetWithDetailsAsync(OrderId);
            if (order.ShippingStatus == ShippingStatus.WaitingForPayment)
            {
                order.OrderStatus = OrderStatus.Closed;
                order.ShippingStatus = ShippingStatus.Closed;

                foreach (var orderItem in order.OrderItems)
                {

                    var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);

                    if (details != null)
                    {
                        details.SaleableQuantity += orderItem.Quantity;
                        details.StockOnHand += orderItem.Quantity;

                        await _itemDetailsRepository.UpdateAsync(details);
                    }

                    if (orderItem.FreebieId != null)
                    {
                        var freebie = await _freebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);

                        if (freebie != null)
                        {
                            freebie.FreebieAmount += orderItem.Quantity;
                            await _freebieRepository.UpdateAsync(freebie);
                        }
                    }

                }

                await _orderRepository.UpdateAsync(order);
            }
        }

    }
    #endregion
}

