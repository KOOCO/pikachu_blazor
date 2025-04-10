﻿using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;

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
    private readonly IOrderInvoiceAppService _electronicInvoiceAppService;
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
    private readonly IIdentityUserRepository _userRepository;
    private readonly OrderHistoryManager _orderHistoryManager;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly OrderTransactionManager _orderTransactionManager;
    private readonly MemberTagManager _memberTagManager;
    private readonly IMemberRepository _memberRepository;

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
        IOrderInvoiceAppService electronicInvoiceAppService,
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
        IUserCumulativeCreditRepository userCumulativeCreditRepository,
        IIdentityUserRepository userRepository,
        OrderHistoryManager orderHistoryManager,
        IOrderHistoryRepository orderHistoryRepository,
        OrderTransactionManager orderTransactionManager,
        MemberTagManager memberTagManager,
        IMemberRepository memberRepository
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
        _userRepository = userRepository;
        _orderHistoryManager = orderHistoryManager;
        _orderHistoryRepository = orderHistoryRepository;
        _orderTransactionManager = orderTransactionManager;
        _memberTagManager = memberTagManager;
        _memberRepository = memberRepository;
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
        IdentityUser? user = null;

        using (_dataFilter.Disable<IMultiTenant>())
        {
            if (input.UserId.HasValue)
            {
                user = await _userRepository.FindAsync(input.UserId.Value);
                if (user != null)
                {
                    var blacklisted = await _memberTagManager.IsBlacklistedAsync(user.Id);
                    if (blacklisted)
                    {
                        throw new UserFriendlyException("該用戶已被列入黑名單，無法下單 - This user is blacklisted and can not place an order");
                    }
                }
            }
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
            order.StoreAddress = input.StoreAddress;
            order.CVSStoreOutSide = input.CVSStoreOutSide;
            order.ShippingStatus = groupBuy!.IsEnterprise ? ShippingStatus.EnterpricePurchase : input.ShippingStatus;
            order.DeliveryCostForNormal = input.DeliveryCostForNormal;
            order.DeliveryCostForFreeze = input.DeliveryCostForFreeze;
            order.DeliveryCostForFrozen = input.DeliveryCostForFrozen;
            order.DeliveryCost = input.DeliveryCost;
            order.DiscountAmount = input.DiscountCodeAmount;

            if (input.IsTest)
            {
                order.RowVersion = new byte[8];
            }

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
                                details.SaleableQuantity = details.SaleableQuantity - item.Quantity;
                                details.StockOnHand = details.StockOnHand - item.Quantity;

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
                                    setItem.SaleableQuantity = setItem.SaleableQuantity - item.Quantity;

                                    foreach (var setItemDetail in setItem.SetItemDetails)
                                    {
                                        var totalOrderQuantity = setItemDetail.Quantity * item.Quantity;
                                        var detail = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == setItemDetail.ItemId
                                                    && x.Attribute1Value == setItemDetail.Attribute1Value && x.Attribute2Value == setItemDetail.Attribute2Value
                                                    && x.Attribute3Value == setItemDetail.Attribute3Value);
                                        if (detail != null)
                                        {
                                            // Check if the available quantity is sufficient
                                            if (detail.SaleableQuantity < totalOrderQuantity)
                                            {
                                                // Add item to insufficientItems list
                                                insufficientItems.Add($"Item: {detail.ItemName}, Requested: {item.Quantity}, Available: {detail.SaleablePreOrderQuantity},Details:{JsonConvert.SerializeObject(detail)}");
                                            }
                                            else
                                            {
                                                // Proceed with updating the stock if sufficient
                                                detail.SaleableQuantity -= totalOrderQuantity;
                                                detail.StockOnHand -= totalOrderQuantity;

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
                                freebie.FreebieAmount -= 1;
                                await _freebieRepository.UpdateAsync(freebie);
                            }
                            else
                            {

                                insufficientItems.Add("贈品庫存不足");
                            }
                        }
                    }
                }

                // If there are items with insufficient stock, throw a BusinessException with the list
                if (insufficientItems.Count > 0)
                {
                    string errorMessage = string.Join("; ", insufficientItems);
                    throw new UserFriendlyException("409", "以下商品庫存不足,請刷新後再試: " + errorMessage);
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

                var temperatures = Enum.GetValues<ItemStorageTemperature>();
                foreach (ItemStorageTemperature temperature in temperatures)
                {
                    var temperatureOrderItems = newOrder.OrderItems
                        .Where(x => x.DeliveryTemperature == temperature)
                        .ToList();
                    if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == true))
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                    if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == false))
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), newOrder.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", newOrder.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        newOrder.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                }

                OrderDelivery? orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == newOrder.Id);

                if (newOrder.OrderItems.Any(x => x.FreebieId != null))
                {
                    if (orderDelivery is not null)
                        newOrder.UpdateOrderItem(newOrder.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
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
            // **ADD ORDER HISTORY LOG**
            Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
            string editorUserName = editorUserId != null ? (await _userRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

            await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderCreated", // Localization key instead of hardcoded text
     new object[] { order.OrderNo }, // Dynamic placeholders
     editorUserId,
     editorUserName
 );
            await _orderRepository.UpdateAsync(order);
            await SendEmailAsync(order.Id);
            var validitySettings = (await _paymentGatewayRepository.GetQueryableAsync()).Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
            DateTime expirationTime = DateTime.Now.AddMinutes(10);
            if (validitySettings is not null)
            {
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
            }

            if (user != null)
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    await _memberTagManager.AddExistingAsync(user.Id);
                    var vipTier = await _memberRepository.CheckForVipTierAsync(user.Id);
                    if (vipTier != null)
                    {
                        await _memberTagManager.AddVipTierAsync(user.Id, vipTier.TierName, vipTier.Id);
                    }
                }
            }

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
    public async Task<Guid> GetOrderIdAsync(string orderNo)
    {
        return await (await _orderRepository.GetQueryableAsync()).Where(x => x.OrderNo == orderNo).Select(x => x.Id).FirstOrDefaultAsync();

    }
    public async Task<OrderDto> UpdateOrderPaymentMethodAsync(OrderPaymentMethodRequest request)
    {
        Order order = await _orderRepository.GetAsync(request.OrderId);
        var oldPaymentMethod = order.PaymentMethod;
        order.PaymentMethod = request.PaymentMethod;
        // Determine EditorUserId (set to null if third-party)
        Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
        string editorUserName = editorUserId != null ? (await _userRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

        // Log Order History
        await _orderHistoryManager.AddOrderHistoryAsync(
                                    order.Id,
                                    "PaymentMethodUpdated", // Localization key instead of raw text
                                    new object[] { oldPaymentMethod, order.PaymentMethod }, // Dynamic placeholders
                                    editorUserId,
                                    editorUserName);

        return ObjectMapper.Map<Order, OrderDto>(
            await _orderRepository.UpdateAsync(order)
        );
    }

    public async Task<OrderDto> UpdateMerchantTradeNoAsync(OrderPaymentMethodRequest request)
    {
        Order order = await _orderRepository.GetAsync(request.OrderId);
        var oldMerchantTradeNo = order.MerchantTradeNo;
        order.MerchantTradeNo = request.MerchantTradeNo;
        // Determine EditorUserId (set to null if third-party)
        Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
        string editorUserName = editorUserId != null ? (await _userRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

        // Log Order History
        await _orderHistoryManager.AddOrderHistoryAsync(
    order.Id,
    "MerchantTradeNoUpdated", // Localization key
    new object[] { oldMerchantTradeNo, order.MerchantTradeNo }, // Dynamic placeholders
    editorUserId,
    editorUserName
    );

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
        string OrderNo = "";
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
                OrderNo += order.OrderNo + ",";
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
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";
            OrderNo = OrderNo.TrimEnd(',');
            // **Log Order History for Merge**
            await _orderHistoryManager.AddOrderHistoryAsync(
    order1.Id,
    "OrderMerge", // Localization key
    new object[] { OrderNo, order1.OrderNo }, // Dynamic placeholders
    currentUserId,
    currentUserName
);

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
                // Log old order closure in OrderHistory
                await _orderHistoryManager.AddOrderHistoryAsync(
     ord1.Id,
     "OrderMergedToNew", // Localization key
     new object[] { ord1.OrderNo, order1.OrderNo }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );

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
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Split Order**
            await _orderHistoryManager.AddOrderHistoryAsync(
     splitOrderId,
     "OrderSplit", // Localization key
     new object[] { ord.OrderNo, newOrder.OrderNo }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );

            // **Log Update to Original Order**
            await _orderHistoryManager.AddOrderHistoryAsync(
                ord.Id,
                "OrderSplitFrom",
                new object[] { ord.OrderNo, newOrder.OrderNo },
                currentUserId,
                currentUserName
            );

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
            order1.StoreAddress = ord.StoreAddress;
            order1.CVSStoreOutSide = ord.CVSStoreOutSide;
            order1.DeliveryCostForNormal = ord.DeliveryCostForNormal;
            order1.DeliveryCostForFreeze = ord.DeliveryCostForFreeze;
            order1.DeliveryCostForFrozen = ord.DeliveryCostForFrozen;
            order1.DeliveryCost = ord.DeliveryCost;
            order1.GWSR = ord.GWSR;
            order1.TradeNo = ord.TradeNo;
            order1.OrderRefundType = OrderRefundType.PartialRefund;

            foreach (var extraProp in ord.ExtraProperties ?? [])
            {
                order1.SetProperty(extraProp.Key, extraProp.Value);
            }

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
                }
            }

            await _orderRepository.InsertAsync(order1);
            await _orderRepository.UpdateAsync(ord);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            newOrder = order1;

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
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Refund Request**
            await _orderHistoryManager.AddOrderHistoryAsync(
     ord.Id,
     "RefundRequested", // Localization key
     new object[] { newOrder.TotalAmount.ToString("C", new CultureInfo("en-US")) }, // Format currency correctly before passing
     currentUserId,
     currentUserName
 );


            // **Log the creation of the new refunded order**
            await _orderHistoryManager.AddOrderHistoryAsync(
       newOrder.Id,
       "RefundOrderCreated", // Localization key
       new object[] { newOrder.OrderNo, ord.OrderNo }, // Dynamic placeholders
       currentUserId,
       currentUserName
   );
            return ObjectMapper.Map<Order, OrderDto>(ord);
        }
    }
    public async Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Store the old status before updating
        var oldStatus = order.ShippingStatus;
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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Status Change**
        await _orderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "StatusChanged",
            new object[] { _l[oldStatus.ToString()]?.Value, _l[status.ToString()]?.Value },
            currentUserId,
            currentUserName
        );
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
            order1.StoreAddress = ord.StoreAddress;
            order1.CVSStoreOutSide = ord.CVSStoreOutSide;
            order1.DeliveryCostForNormal = ord.DeliveryCostForNormal;
            order1.DeliveryCostForFreeze = ord.DeliveryCostForFreeze;
            order1.DeliveryCostForFrozen = ord.DeliveryCostForFrozen;
            order1.DeliveryCost = ord.DeliveryCost;
            order1.GWSR = ord.GWSR;
            order1.TradeNo = ord.TradeNo;
            order1.OrderRefundType = OrderRefundType.PartialRefund;

            foreach (var extraProp in ord.ExtraProperties ?? [])
            {
                order1.SetProperty(extraProp.Key, extraProp.Value);
            }

            await _orderRepository.InsertAsync(order1);

            await _orderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await _refundAppService.CreateAsync(order1.Id);
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Refund Action in Order History**
            await _orderHistoryManager.AddOrderHistoryAsync(
      ord.Id,
      "RefundProcessed", // Localization key
      new object[] { ((decimal)amount).ToString("C", new CultureInfo("en-US")), ord.OrderNo }, // Format currency correctly before passing
      currentUserId,
      currentUserName
  );

            // **Log New Refund Order Creation**
            await _orderHistoryManager.AddOrderHistoryAsync(
       order1.Id,
       "RefundAmountCreated", // Localization key
       new object[] { order1.OrderNo, ((decimal)amount).ToString("C", new CultureInfo("en-US")), ord.OrderNo }, // Format currency correctly
       currentUserId,
       currentUserName
   );

        }
    }
    public async Task<OrderDto> GetWithDetailsAsync(Guid id)
    {
        Order order = await _orderRepository.GetWithDetailsAsync(id);

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<List<OrderHistoryDto>> GetOrderLogsAsync(Guid orderId)
    {
        var result = await _orderHistoryRepository.GetAllHistoryByOrderIdAsync(orderId);

        return ObjectMapper.Map<List<OrderHistory>, List<OrderHistoryDto>>(result);

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
    public async Task<PagedResultDto<GroupBuyReportOrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(GroupBuyReportOrderModel.CreationTime)} desc";
        }

        var data = await _orderRepository.GetReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate, input.OrderStatus);

        var dtos = ObjectMapper.Map<List<GroupBuyReportOrderModel>, List<GroupBuyReportOrderDto>>(data.Items);

        if (hideCredentials)
        {
            var groupbuy = await _groupBuyRepository.GetAsync(input.GroupBuyId.Value);
            if (groupbuy.ProtectPrivacyData)
            {
                dtos.HideCredentials();
            }
        }

        return new PagedResultDto<GroupBuyReportOrderDto>
        {
            TotalCount = data.TotalCount,
            Items = dtos
        };
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

    public async Task<long> GetReturnOrderNotificationCount()
    {
        var totalCount = await _orderRepository.ReturnOrderNotificationCountAsync();
        return totalCount;
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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        // Capture changes in order details for logging
        List<string> changes = new();

        if (order.RecipientName != input.RecipientName)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RecipientNameChanged", // Localization key for ActionType
 new object[] { order.RecipientName, input.RecipientName }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.RecipientPhone != input.RecipientPhone)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RecipientPhoneChanged", // Localization key for ActionType
 new object[] { order.RecipientPhone, input.RecipientPhone }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.PostalCode != input.PostalCode)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "PostalCodeChanged", // Localization key for ActionType
 new object[] { order.PostalCode, input.PostalCode }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.District != input.District)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "DistrictChanged", // Localization key for ActionType
 new object[] { order.District, input.District }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.City != input.City)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "CityChanged", // Localization key for ActionType
 new object[] { order.City, input.City }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.Road != input.Road)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RoadChanged", // Localization key for ActionType
 new object[] { order.Road, input.Road }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.AddressDetails != input.AddressDetails)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "AddressDetailsChanged", // Localization key for ActionType
 new object[] { order.AddressDetails, input.AddressDetails }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.OrderStatus != input.OrderStatus)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "OrderStatusChanged", // Localization key for ActionType
 new object[] { order.OrderStatus, input.OrderStatus }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.StoreId != input.StoreId)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "StoreIdChanged", // Localization key for ActionType
 new object[] { order.StoreId, input.StoreId }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.CVSStoreOutSide != input.CVSStoreOutSide)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "CVSStoreChanged", // Localization key for ActionType
 new object[] { order.CVSStoreOutSide, input.CVSStoreOutSide }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        order.RecipientName = input.RecipientName;
        order.RecipientPhone = input.RecipientPhone;
        order.PostalCode = input.PostalCode;
        order.District = input.District;
        order.City = input.City;
        order.Road = input.Road;
        order.AddressDetails = input.AddressDetails;
        order.OrderStatus = input.OrderStatus;
        order.StoreId = input.StoreId;
        order.StoreAddress = input.StoreAddress;
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

    public async Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus, bool isRefund)
    {
        var order = await _orderRepository.GetAsync(id);
        // Capture old status before updating
        var oldReturnStatus = order.ReturnStatus;
        order.ReturnStatus = orderReturnStatus;
        if (orderReturnStatus == OrderReturnStatus.Reject)
        {
            order.OrderStatus = OrderStatus.Open;

        }

        if (orderReturnStatus == OrderReturnStatus.Approve)
        {


            order.ReturnStatus = OrderReturnStatus.Processing;
        }
        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Returned && isRefund)
        {
            await _refundAppService.CreateAsync(id);
            var refund = (await _refundRepository.GetQueryableAsync()).Where(x => x.OrderId == order.Id).FirstOrDefault();
            await _refundAppService.UpdateRefundReviewAsync(refund.Id, RefundReviewStatus.Proccessing);
            await _refundAppService.SendRefundRequestAsync(refund.Id);

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Returned, TransactionStatus.Successful, order.PaymentMethod?.GetPaymentChannel());
            await _orderTransactionManager.CreateAsync(orderTransaction);
        }
        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
        {
            order.ExchangeBy = CurrentUser.UserName;
            order.ExchangeTime = DateTime.Now;
            order.ShippingStatus = ShippingStatus.Exchange;

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Exchange, TransactionStatus.Successful, order.PaymentMethod?.GetPaymentChannel());
            await _orderTransactionManager.CreateAsync(orderTransaction);
        }
        await _orderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Return Status Change**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "ReturnStatusChanged", // Localization key
     new object[] { _l[oldReturnStatus.ToString()]?.Value, _l[orderReturnStatus.ToString()]?.Value }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );


        // **Log Additional Details for Refund or Exchange**
        if (orderReturnStatus == OrderReturnStatus.Approve && order.OrderStatus == OrderStatus.Returned)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
                 order.Id,
                 "RefundInitiated", // Localization key
                 new object[] { order.OrderNo }, // Dynamic placeholders
                 currentUserId,
                 currentUserName
             );

        }

        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
        {
            await _orderHistoryManager.AddOrderHistoryAsync(
                 order.Id,
                 "OrderExchanged", // Localization key
                 new object[] { order.OrderNo }, // Format date properly
                 currentUserId,
                 currentUserName
             );
        }
    }

    public async Task ExchangeOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        // Capture old statuses before updating
        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Exchange;

        await _orderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Exchange**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderExchangeInitiated", // Localization key
     new object[]
     {
        _l[oldReturnStatus.ToString()]?.Value,
        _l[order.ReturnStatus.ToString()]?.Value,
        _l[oldOrderStatus.ToString()]?.Value,
        _l[order.OrderStatus.ToString()]?.Value
     }, // Dynamic placeholders for localized statuses
     currentUserId,
     currentUserName
 );



    }
    public async Task ReturnOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        // Capture old statuses before updating
        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Returned;

        await _orderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Return**
        await _orderHistoryManager.AddOrderHistoryAsync(
       order.Id,
       "OrderReturnInitiated", // Localization key
       new object[]
       {
        _l[oldReturnStatus.ToString()].Name,
        _l[order.ReturnStatus.ToString()].Name,
        _l[oldOrderStatus.ToString()].Name,
        _l[order.OrderStatus.ToString()].Name
       }, // Dynamic placeholders for localized statuses
       currentUserId,
       currentUserName
   );


    }
    public async Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);


        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        List<string> itemChanges = new();
        foreach (var item in orderItems)
        {

            var orderItem = order.OrderItems.First(o => o.Id == item.Id);
            string itemName = "";
            if (orderItem.ItemType is ItemType.Item)
            {
                itemName = orderItem.Item?.ItemName;
            }
            else if (orderItem.ItemType is ItemType.SetItem)
            {
                itemName = orderItem.SetItem?.SetItemName;


            }
            if (orderItem.ItemType is ItemType.Freebie)
            {
                itemName = orderItem.Freebie?.ItemName;


            }
            // Capture changes for logging
            if (orderItem.Quantity != item.Quantity)
            {
                await _orderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "ItemQuantityChanged", // Localization key
                  new object[] { itemName, orderItem.Quantity, item.Quantity }, // Join localized changes as a single string
                  currentUserId,
                  currentUserName
              );
            }


            if (orderItem.ItemPrice != item.ItemPrice)
            {
                await _orderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "ItemPriceChanged", // Localization key
                  new object[] { itemName, orderItem.ItemPrice.ToString("C", new CultureInfo("en-US")), item.ItemPrice.ToString("C", new CultureInfo("en-US")) }, // Join localized changes as a single string
                  currentUserId,
                  currentUserName
              );
            }


            if (orderItem.TotalAmount != item.TotalAmount)
            {
                await _orderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "ItemTotalAmountChanged", // Localization key
                  new object[] { itemName, orderItem.TotalAmount.ToString("C", new CultureInfo("en-US")), item.TotalAmount.ToString("C", new CultureInfo("en-US")) }, // Join localized changes as a single string
                  currentUserId,
                  currentUserName
              );
            }



            // Apply updates
            orderItem.Quantity = item.Quantity;
            orderItem.ItemPrice = item.ItemPrice;
            orderItem.TotalAmount = item.TotalAmount;
        }

        order.TotalQuantity = order.OrderItems.Sum(o => o.Quantity);
        order.TotalAmount = (order.OrderItems.Sum(o => o.TotalAmount) + (decimal)order.DeliveryCost);
        await _orderRepository.UpdateAsync(order);



    }
    public async Task CancelOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Capture the old status before updating
        var oldOrderStatus = order.OrderStatus;
        order.OrderStatus = OrderStatus.Closed;
        order.CancellationDate = DateTime.Now;
        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id, OrderStatus.Closed);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Cancellation**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderCancelled", // Localization key
     new object[] { _l[oldOrderStatus.ToString()].Name }, // Localized previous status
     currentUserId,
     currentUserName
 );

    }
    public async Task VoidInvoice(Guid id, string reason)
    {
        var order = await _orderRepository.GetAsync(id);
        // Capture old invoice status before voiding
        var oldInvoiceStatus = order.InvoiceStatus;
        order.IsVoidInvoice = true;
        order.VoidReason = reason;
        order.VoidUser = CurrentUser.Name;
        order.VoidDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.InvoiceVoided;
        order.LastModificationTime = DateTime.Now;
        order.LastModifierId = CurrentUser.Id;
        await _orderRepository.UpdateAsync(order);
        await _electronicInvoiceAppService.CreateVoidInvoiceAsync(id, reason);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Invoice Voiding**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "InvoiceVoided", // Localization key
     new object[] { _l[oldInvoiceStatus.ToString()].Name, reason }, // Localized invoice status & reason
     currentUserId,
     currentUserName
 );


    }
    public async Task CreditNoteInvoice(Guid id, string reason)
    {
        Order order = await _orderRepository.GetAsync(id);
        // Capture old invoice status before issuing the credit note
        var oldInvoiceStatus = order.InvoiceStatus;
        order.IsVoidInvoice = true;
        order.CreditNoteReason = reason;
        order.CreditNoteUser = CurrentUser.Name;
        order.CreditNoteDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.CreditNote;

        await _orderRepository.UpdateAsync(order);

        await _electronicInvoiceAppService.CreateCreditNoteAsync(id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Credit Note Issuance**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "CreditNoteIssued", // Localization key
     new object[] { _l[oldInvoiceStatus.ToString()].Name, reason }, // Localized previous status & reason
     currentUserId,
     currentUserName
 );
    }
    public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);

        // Capture old shipping details before updating
        var oldDeliveryMethod = order.DeliveryMethod;
        var oldShippingNumber = order.ShippingNumber;
        var oldShippingStatus = order.ShippingStatus;
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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping Update**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "ShippingDetailsUpdated", // Localization key
     new object[]
     {
        _l[oldDeliveryMethod.ToString()].Name,
        _l[order.DeliveryMethod.ToString()].Name,
        oldShippingNumber,
        order.ShippingNumber,
        _l[oldShippingStatus.ToString()].Name,
        _l[order.ShippingStatus.ToString()].Name
     }, // Dynamic placeholders for localized statuses and tracking numbers
     currentUserId,
     currentUserName
 );

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderShipped(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;

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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping**
        await _orderHistoryManager.AddOrderHistoryAsync(
      order.Id,
      "OrderShipped", // Localization key
      new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Dynamic placeholders for localized statuses
      currentUserId,
      currentUserName
  );

        return returnOrder;
        // await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);

    }
    public async Task<OrderDto> OrderToBeShipped(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping Status Change**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderToBeShipped", // Localization key
     new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

        return returnOrder;

    }
    public async Task<OrderDto> OrderClosed(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
        order.ShippingStatus = ShippingStatus.Closed;
        order.ClosedBy = CurrentUser.Name;
        order.CancellationDate = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Order Closure**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderClosed", // Localization key
     new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderComplete(Guid id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
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
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Order Completion**
        await _orderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderCompleted", // Localization key
     new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

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

    public async Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg, int rtnCode = 0)
    {
        Order order = await _orderRepository.GetOrderByMerchantTradeNoAsync(merchantTradeNo);
        // Capture old logistics status and shipping status before updating
        var oldLogisticsStatus = order.EcpayLogisticsStatus;
        var oldShippingStatus = order.ShippingStatus;

        order.EcpayLogisticsStatus = rtnMsg;
        order.EcpayLogisticRtnCode = rtnCode;

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "EcPay Logistic Update";

        // **Log Order History for Logistic Status Update**
        await _orderHistoryManager.AddOrderHistoryAsync(
             order.Id,
             "LogisticStatusUpdated",
             new object[] { oldLogisticsStatus, order.EcpayLogisticsStatus },
             currentUserId,
             currentUserName
         );

        if (rtnCode != 0 && order.DeliveryMethod.HasValue)
        {
            var toBeShippedCodes = new Dictionary<DeliveryMethod, int[]>
            {
                { DeliveryMethod.PostOffice, new[] { 320 } }
            };

            var shippedCodes = new Dictionary<DeliveryMethod, int[]>
            {
                { DeliveryMethod.SevenToEleven1, new[] { 2063 } },
                { DeliveryMethod.SevenToElevenC2C, new[] { 2063 } },
                { DeliveryMethod.SevenToElevenFrozen, new[] { 2063 } },
                { DeliveryMethod.FamilyMart1, new[] { 3018 } },
                { DeliveryMethod.FamilyMartC2C, new[] { 3018 } },
                { DeliveryMethod.PostOffice, new[] { 3301 } },
                { DeliveryMethod.BlackCat1, new[] { 3006 } },
                { DeliveryMethod.BlackCatFreeze, new[] { 3006 } },
                { DeliveryMethod.BlackCatFrozen, new[] { 3006 } }
            };

            var deliveredCodes = new Dictionary<DeliveryMethod, int[]>
            {
                { DeliveryMethod.SevenToEleven1, new[] { 2001, 2024 } },
                { DeliveryMethod.SevenToElevenC2C, new[] { 2001, 2024 } },
                { DeliveryMethod.SevenToElevenFrozen, new[] { 2001, 2024 } },
                { DeliveryMethod.FamilyMart1, new[] { 3029 } },
                { DeliveryMethod.FamilyMartC2C, new[] { 3029 } },
                { DeliveryMethod.PostOffice, new[] { 3308 } }
            };

            var completedCodes = new Dictionary<DeliveryMethod, int[]>
            {
                { DeliveryMethod.SevenToEleven1, new[] { 2067 } },
                { DeliveryMethod.SevenToElevenC2C, new[] { 2067 } },
                { DeliveryMethod.SevenToElevenFrozen, new[] { 2067 } },
                { DeliveryMethod.FamilyMart1, new[] { 3022 } },
                { DeliveryMethod.FamilyMartC2C, new[] { 3022 } },
                { DeliveryMethod.PostOffice, new[] { 3309 } },
                { DeliveryMethod.BlackCat1, new[] { 311 } },
                { DeliveryMethod.BlackCatFreeze, new[] { 311 } },
                { DeliveryMethod.BlackCatFrozen, new[] { 311 } }
            };

            var returnedCodes = new Dictionary<DeliveryMethod, int[]>
            {
                { DeliveryMethod.SevenToEleven1, new[] { 2065, 2074 } },
                { DeliveryMethod.SevenToElevenC2C, new[] { 2065, 2074 } },
                { DeliveryMethod.SevenToElevenFrozen, new[] { 2065, 2074 } },
                { DeliveryMethod.FamilyMart1, new[] { 3020 } },
                { DeliveryMethod.FamilyMartC2C, new[] { 3020 } },
                { DeliveryMethod.PostOffice, new[] { 3310 } },
                { DeliveryMethod.BlackCat1, new[] { 325 } },
                { DeliveryMethod.BlackCatFreeze, new[] { 325 } },
                { DeliveryMethod.BlackCatFrozen, new[] { 325 } }
            };

            var deliveryMethod = order.DeliveryMethod.Value;

            if ((deliveryMethod != DeliveryMethod.PostOffice && rtnCode == 300) ||
                (toBeShippedCodes.TryGetValue(deliveryMethod, out var toBeShippedRtnCodes) && toBeShippedRtnCodes.Contains(rtnCode)))
            {
                order.ShippingStatus = ShippingStatus.ToBeShipped;
            }
            else if (shippedCodes.TryGetValue(deliveryMethod, out var shippedRtnCodes) && shippedRtnCodes.Contains(rtnCode))
            {
                order.ShippingStatus = ShippingStatus.Shipped;
            }
            else if (deliveredCodes.TryGetValue(deliveryMethod, out var deliveredRtnCodes) && deliveredRtnCodes.Contains(rtnCode))
            {
                order.ShippingStatus = ShippingStatus.Delivered;
            }
            else if (completedCodes.TryGetValue(deliveryMethod, out var completedRtnCodes) && completedRtnCodes.Contains(rtnCode))
            {
                order.ShippingStatus = ShippingStatus.Completed;
            }
            else if (returnedCodes.TryGetValue(deliveryMethod, out var returnedRtnCodes) && returnedRtnCodes.Contains(rtnCode))
            {
                order.ShippingStatus = ShippingStatus.Return;
            }

            await _orderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "ShippingStatusUpdated", // Localization key
                new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );
        }

        await _orderRepository.UpdateAsync(order);
    }

    [AllowAnonymous]
    public async Task CloseOrdersAsync()
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var ordersToClose = await _orderRepository.GetOrdersToCloseAsync();
            Logger.LogInformation("{BackgroundJob}: Found {orderCount} orders that need to be closed.", nameof(CloseOrderBackgroundJob), ordersToClose.Count);

            List<OrderHistory> orderHistoryList = [];

            foreach (var order in ordersToClose)
            {
                Logger.LogInformation("{BackgroundJob}: Closing Order #: {orderNo}, Id: {orderId}, Order Status: {orderStatus}, Shipping Status: {shippingStatus}",
                    nameof(CloseOrderBackgroundJob), order.OrderNo, order.Id, order.OrderStatus, order.ShippingStatus);


                if (order.ShippingStatus != ShippingStatus.Closed)
                {
                    var oldShippingStatus = order.ShippingStatus;
                    order.ShippingStatus = ShippingStatus.Closed;
                    var shippingStatusHistory = new OrderHistory(Guid.NewGuid(), order.Id, "OrderClosedDueToInActivity", JsonConvert.SerializeObject(new object[] { "Shipping", oldShippingStatus.ToString(), order.ShippingStatus.ToString() }), null, "CloseOrderJob");
                    orderHistoryList.Add(shippingStatusHistory);
                }

                if (order.OrderStatus != OrderStatus.Closed)
                {
                    var oldOrderStatus = order.OrderStatus;
                    order.OrderStatus = OrderStatus.Closed;
                    var orderStatusHistory = new OrderHistory(Guid.NewGuid(), order.Id, "OrderClosedDueToInActivity", JsonConvert.SerializeObject(new object[] { "Order", oldOrderStatus.ToString(), order.OrderStatus.ToString() }), null, "CloseOrderJob");
                    orderHistoryList.Add(orderStatusHistory);
                }
            }

            await _orderHistoryRepository.InsertManyAsync(orderHistoryList);
        }
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
            }
            var oldShippingStatus = order.ShippingStatus;
            using (CurrentTenant.Change(order.TenantId))
            {
                order.GWSR = paymentResult.GWSR;
                order.TradeNo = paymentResult.TradeNo;
                order.ShippingStatus = ShippingStatus.PrepareShipment;
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Payment Processing**
                await _orderHistoryManager.AddOrderHistoryAsync(
                      order.Id,
                      "PaymentProcessed", // Localization key
                      new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
                      currentUserId,
                      currentUserName
                  );
                order.PrepareShipmentBy = CurrentUser.Name ?? "System";
                _ = DateTime.TryParse(paymentResult.PaymentDate, out DateTime parsedDate);
                order.PaymentDate = paymentResult.OrderId == null ? parsedDate : DateTime.Now;

                var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                    order.TotalAmount, TransactionType.Payment, TransactionStatus.Successful, PaymentChannel.EcPay);
                await _orderTransactionManager.CreateAsync(orderTransaction);

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

                var temperatures = Enum.GetValues<ItemStorageTemperature>();
                foreach (ItemStorageTemperature temperature in temperatures)
                {
                    var temperatureOrderItems = order.OrderItems
                        .Where(x => x.DeliveryTemperature == temperature)
                        .ToList();
                    if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == true))
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                    }
                    if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == false))
                    {
                        var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                        OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                        order.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                    }
                }

                OrderDelivery? orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == order.Id);

                if (order.OrderItems.Any(x => x.FreebieId != null))
                {
                    if (orderDelivery is not null)
                        order.UpdateOrderItem(order.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
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
                // Capture previous order status before updating
                var oldOrderStatus = order.OrderStatus;
                var oldShippingStatus = order.ShippingStatus;

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
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Order Expiration**
                await _orderHistoryManager.AddOrderHistoryAsync(
    order.Id,
    "OrderExpired", // Localization key
    new object[]
    {
        _l[oldOrderStatus.ToString()].Name,
        _l[order.OrderStatus.ToString()].Name,
        _l[oldShippingStatus.ToString()].Name,
        _l[order.ShippingStatus.ToString()].Name,
        order.OrderItems.Count
    }, // Dynamic placeholders
    currentUserId,
    currentUserName
);

            }
        }

    }

    [AllowAnonymous]
    public async Task CreateOrderDeliveriesAndInvoiceAsync(Guid orderId)
    {
        var order = await _orderRepository.GetWithDetailsAsync(orderId);

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

        var temperatures = Enum.GetValues<ItemStorageTemperature>();
        foreach (ItemStorageTemperature temperature in temperatures)
        {
            var temperatureOrderItems = order.OrderItems
                .Where(x => x.DeliveryTemperature == temperature)
                .ToList();
            if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == true))
            {
                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                order.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
            }
            if (temperatureOrderItems.Any(x => x.SetItem?.IsFreeShipping == false))
            {
                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                order.UpdateOrderItem(temperatureOrderItems.Where(x => x.SetItem?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
            }
        }

        OrderDelivery? orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == order.Id);

        if (order.OrderItems.Any(x => x.FreebieId != null))
        {
            if (orderDelivery is not null)
                order.UpdateOrderItem(order.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
        }

        await SendEmailAsync(order.Id);

        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();

            if (invoiceSetting?.StatusOnInvoiceIssue == DeliveryStatus.Processing)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;

                    var invoiceDelay = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDelay == 0)
                    {
                        await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        var delay = DateTime.Now.AddDays(invoiceDelay) - DateTime.Now;
                        GenerateInvoiceBackgroundJobArgs args = new() { OrderId = order.Id };
                        var jobid = await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
    }
}
#endregion
