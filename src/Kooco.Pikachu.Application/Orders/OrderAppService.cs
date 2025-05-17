using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Members.MemberTags;
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
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;

namespace Kooco.Pikachu.Orders;

[RemoteService(IsEnabled = false)]
public class OrderAppService : PikachuAppService, IOrderAppService
{
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

        using (DataFilter.Disable<IMultiTenant>())
        {
            if (input.UserId.HasValue)
            {
                user = await UserRepository.FindAsync(input.UserId.Value);
                if (user != null)
                {
                    var blacklisted = await MemberTagManager.IsBlacklistedAsync(user.Id);
                    if (blacklisted)
                    {
                        throw new UserFriendlyException("該用戶已被列入黑名單，無法下單 - This user is blacklisted and can not place an order");
                    }
                }
            }
            groupBuy = await GroupBuyRepository.GetAsync(input.GroupBuyId);
        }

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order = await OrderManager.CreateAsync(
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
            order.CampaignId = input.CampaignId;

            if (input.IsTest)
            {
                order.RowVersion = new byte[8];
            }

            if (input.OrderItems is { Count: > 0 })
            {
                List<string> insufficientItems = new List<string>();

                foreach (CreateUpdateOrderItemDto item in input.OrderItems)
                {
                    OrderManager.AddOrderItem(
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

                    using (DataFilter.Disable<IMultiTenant>())
                    {
                        ItemDetails? details = await ItemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == item.ItemId && x.ItemName == item.SKU);

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

                                await ItemDetailsRepository.UpdateAsync(details);
                            }
                        }

                        if (item.SetItemId.HasValue)
                        {
                            var setItem = await SetItemRepository.GetWithDetailsAsync(item.SetItemId.Value);
                            if (setItem != null)
                            {
                                if (setItem.SaleableQuantity < item.Quantity)
                                {
                                    insufficientItems.Add($"Item: {setItem.SetItemName}, Requested: {item.Quantity}, Available: {setItem.SaleableQuantity},Details:{JsonConvert.SerializeObject(setItem)}");
                                }
                                else
                                {
                                    setItem.SaleableQuantity -= item.Quantity;

                                    foreach (var setItemDetail in setItem.SetItemDetails)
                                    {
                                        var totalOrderQuantity = setItemDetail.Quantity * item.Quantity;
                                        var detail = await ItemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == setItemDetail.ItemId
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

                                                await ItemDetailsRepository.UpdateAsync(detail);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Handle Freebies if applicable
                        if (item.FreebieId != null)
                        {
                            Freebie? freebie = await FreebieRepository.FirstOrDefaultAsync(x => x.Id == item.FreebieId);

                            if (freebie != null && freebie.FreebieAmount > 0)
                            {
                                freebie.FreebieAmount -= 1;
                                await FreebieRepository.UpdateAsync(freebie);
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

            await OrderRepository.InsertAsync(order);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (order.PaymentMethod is PaymentMethods.CashOnDelivery && order.ShippingStatus is ShippingStatus.PrepareShipment)
            {
                Order newOrder = await OrderRepository.GetWithDetailsAsync(order.Id);
                await CreateOrderDeliveriesAsync(newOrder);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

            if (order.DiscountCodeId != null)
            {
                var discountCode = await DiscountCodeRepository.GetAsync(order.DiscountCodeId.Value);
                discountCode.AvailableQuantity = discountCode.AvailableQuantity - 1;
                await DiscountCodeRepository.EnsureCollectionLoadedAsync(discountCode, x => x.DiscountCodeUsages);

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

                await DiscountCodeRepository.UpdateAsync(discountCode);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            if (order.UserId != null && order.UserId != Guid.Empty)
            {
                if (order.cashback_amount > 0)
                {
                    if (order.CampaignId.HasValue)
                    {
                        var campaign = await CampaignRepository.FirstOrDefaultAsync(c => c.Id == order.CampaignId);
                        if (campaign != null && campaign.PromotionModule == PromotionModule.ShoppingCredit)
                        {
                            await CampaignRepository.EnsurePropertyLoadedAsync(campaign, c => c.ShoppingCredit);
                            campaign.ShoppingCredit?.DeductBudget((int)order.cashback_amount);
                        }
                    }

                    var newcashback = await UserShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
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

                    var userCumulativeCredits = await UserCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == order.UserId);
                    if (userCumulativeCredits is null)
                    {
                        await UserCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = (int)order.cashback_amount, TotalDeductions = 0, TotalRefunds = 0, UserId = order.UserId });
                    }
                    else
                    {
                        userCumulativeCredits.ChangeTotalAmount((int)(userCumulativeCredits.TotalAmount + order.cashback_amount));
                        await UserCumulativeCreditRepository.UpdateAsync(userCumulativeCredits);
                    }

                    await CurrentUnitOfWork!.SaveChangesAsync();
                }

            }
            if (order.UserId != null && order.UserId != Guid.Empty)
            {
                if (order.CreditDeductionAmount > 0)
                {

                    var newdeduction = await UserShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
                    {
                        Amount = (int)order.CreditDeductionAmount,
                        ExpirationDate = null,
                        IsActive = true,
                        TransactionDescription = "購物折抵：訂單 #" + order.OrderNo,
                        UserId = order.UserId.Value,
                        ShoppingCreditType = UserShoppingCreditType.Deduction
                    });
                    order.CreditDeductionAmount = newdeduction.Amount;
                    order.CreditDeductionRecordId = newdeduction.Id;

                    var userCumulativeCredits = await UserCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == order.UserId);
                    if (userCumulativeCredits is null)
                    {
                        await UserCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = 0, TotalDeductions = order.CreditDeductionAmount, TotalRefunds = 0, UserId = order.UserId });
                    }
                    else
                    {
                        userCumulativeCredits.ChangeTotalDeductions((int)(userCumulativeCredits.TotalDeductions + order.CreditDeductionAmount));
                        await UserCumulativeCreditRepository.UpdateAsync(userCumulativeCredits);
                    }
                }

            }
            // **ADD ORDER HISTORY LOG**
            Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
            string editorUserName = editorUserId != null ? (await UserRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderCreated", // Localization key instead of hardcoded text
                new object[] { order.OrderNo }, // Dynamic placeholders
                editorUserId,
                editorUserName
            );

            await OrderRepository.UpdateAsync(order);
            await SendEmailAsync(order.Id);
            var validitySettings = (await PaymentGatewayRepository.GetQueryableAsync()).Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
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
                var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, (expirationTime - order.CreationTime));
            }

            if (user != null)
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    await MemberTagManager.AddExistingAsync(user.Id);
                    var vipTier = await MemberRepository.CheckForVipTierAsync(user.Id);
                    if (vipTier != null)
                    {
                        await MemberTagManager.AddVipTierAsync(user.Id, vipTier.TierName, vipTier.Id);
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
            await OrderRepository.GetOrderAsync(groupBuyId, orderNo, extraInfo)
        );

        order.StoreCustomerServiceMessages = await OrderMessageAppService.GetOrderMessagesAsync(order.Id);

        return order;
    }
    public async Task<Guid> GetOrderIdAsync(string orderNo)
    {
        return await (await OrderRepository.GetQueryableAsync()).Where(x => x.OrderNo == orderNo).Select(x => x.Id).FirstOrDefaultAsync();

    }
    public async Task<OrderDto> UpdateOrderPaymentMethodAsync(OrderPaymentMethodRequest request)
    {
        Order order = await OrderRepository.GetAsync(request.OrderId);
        var oldPaymentMethod = order.PaymentMethod;
        order.PaymentMethod = request.PaymentMethod;
        // Determine EditorUserId (set to null if third-party)
        Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
        string editorUserName = editorUserId != null ? (await UserRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

        // Log Order History
        await OrderHistoryManager.AddOrderHistoryAsync(
                                    order.Id,
                                    "PaymentMethodUpdated", // Localization key instead of raw text
                                    new object[] { oldPaymentMethod, order.PaymentMethod }, // Dynamic placeholders
                                    editorUserId,
                                    editorUserName);

        return ObjectMapper.Map<Order, OrderDto>(
            await OrderRepository.UpdateAsync(order)
        );
    }

    public async Task<OrderDto> UpdateMerchantTradeNoAsync(OrderPaymentMethodRequest request)
    {
        Order order = await OrderRepository.GetAsync(request.OrderId);
        var oldMerchantTradeNo = order.MerchantTradeNo;
        order.MerchantTradeNo = request.MerchantTradeNo;
        // Determine EditorUserId (set to null if third-party)
        Guid? editorUserId = order.UserId != null && order.UserId != Guid.Empty ? order.UserId : null;
        string editorUserName = editorUserId != null ? (await UserRepository.GetAsync(editorUserId.Value))?.UserName ?? "System" : "System";

        // Log Order History
        await OrderHistoryManager.AddOrderHistoryAsync(
    order.Id,
    "MerchantTradeNoUpdated", // Localization key
    new object[] { oldMerchantTradeNo, order.MerchantTradeNo }, // Dynamic placeholders
    editorUserId,
    editorUserName
    );

        return ObjectMapper.Map<Order, OrderDto>(
            await OrderRepository.UpdateAsync(order)
        );
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Order, OrderDto>(await OrderRepository.GetAsync(id));
    }

    public async Task<OrderDto> MergeOrdersAsync(List<Guid> Ids)
    {
        decimal TotalAmount = 0; int TotalQuantity = 0;

        Order ord = await OrderRepository.GetWithDetailsAsync(Ids[0]);
        string OrderNo = "";
        GroupBuy groupBuy = new();
        List<decimal> DeliveriesCost = new();
        using (DataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await GroupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        List<OrderItemsCreateDto> orderItems = [];

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            foreach (Guid id in Ids)
            {
                Order order = await OrderRepository.GetWithDetailsAsync(id);
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

            Order order1 = await OrderManager.CreateAsync(
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
                OrderManager.AddOrderItem(
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
            await OrderRepository.InsertAsync(order1);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (order1.ShippingStatus is ShippingStatus.PrepareShipment)
            {
                await CreateOrderDeliveriesAsync(order1);
                await UnitOfWorkManager.Current.SaveChangesAsync();

                await OrderRepository.UpdateAsync(order1);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";
            OrderNo = OrderNo.TrimEnd(',');
            // **Log Order History for Merge**
            await OrderHistoryManager.AddOrderHistoryAsync(
    order1.Id,
    "OrderMerge", // Localization key
    new object[] { OrderNo, order1.OrderNo }, // Dynamic placeholders
    currentUserId,
    currentUserName
);

            foreach (Guid id in Ids)
            {
                Order ord1 = await OrderRepository.GetWithDetailsAsync(id);

                ord1.OrderType = OrderType.MargeToNew;

                ord1.ShippingStatus = ShippingStatus.Closed;

                ord1.TotalAmount = 0;

                foreach (OrderItem item in ord1.OrderItems)
                {
                    item.ItemPrice = 0; item.TotalAmount = 0;
                }


                await OrderRepository.UpdateAsync(ord1, autoSave: true);
                // Log old order closure in OrderHistory
                await OrderHistoryManager.AddOrderHistoryAsync(
     ord1.Id,
     "OrderMergedToNew", // Localization key
     new object[] { ord1.OrderNo, order1.OrderNo }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );

            }
            await UnitOfWorkManager.Current.SaveChangesAsync();

            await EmailAppService.SendMergeOrderEmailAsync(Ids, order1.Id);

            return ObjectMapper.Map<Order, OrderDto>(order1);
        }
    }

    public async Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId)
    {
        Order newOrder = new();

        Order ord = await OrderRepository.GetWithDetailsAsync(OrderId);

        decimal TotalAmount = ord.TotalAmount; int TotalQuantity = ord.TotalQuantity;

        GroupBuy groupBuy = new();

        using (DataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await GroupBuyRepository.GetAsync(ord.GroupBuyId);
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

                    await OrderItemRepository.DeleteAsync(item.Id);

                    Order order1 = await OrderManager.CreateAsync(
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

                    OrderManager.AddOrderItem(
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

                    await OrderRepository.InsertAsync(order1);
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    newOrder = order1;

                    OrderItem? orderItem1 = newOrder.OrderItems.FirstOrDefault();

                    if (order1.ShippingStatus is ShippingStatus.PrepareShipment)
                    {
                        OrderDelivery oD = new(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Processing, null, string.Empty, order1.Id);

                        oD = await OrderDeliveryRepository.InsertAsync(oD);

                        order1.UpdateOrderItem(
                            [.. order1.OrderItems.Where(w => w.DeliveryTemperature == orderItem1?.DeliveryTemperature)],
                            oD.Id
                        );

                        await OrderRepository.UpdateAsync(order1);
                    }
                }

            }
            if (ord.OrderItems.Count <= 0)
            {
                newOrder.TotalAmount += TotalAmount;
                await OrderRepository.UpdateAsync(newOrder);
                ord.TotalAmount = 0;
                ord.TotalQuantity = 0;
            }
            else
            {
                ord.TotalAmount = TotalAmount;
                ord.TotalQuantity = TotalQuantity;
            }

            ord.OrderType = OrderType.SplitToNew;

            await OrderRepository.UpdateAsync(ord);
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Split Order**
            await OrderHistoryManager.AddOrderHistoryAsync(
     splitOrderId,
     "OrderSplit", // Localization key
     new object[] { ord.OrderNo, newOrder.OrderNo }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );

            // **Log Update to Original Order**
            await OrderHistoryManager.AddOrderHistoryAsync(
                ord.Id,
                "OrderSplitFrom",
                new object[] { ord.OrderNo, newOrder.OrderNo },
                currentUserId,
                currentUserName
            );

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await EmailAppService.SendSplitOrderEmailAsync(OrderId, splitOrderId);

            return ObjectMapper.Map<Order, OrderDto>(ord);
        }
    }
    public async Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId)
    {
        Order newOrder = new();

        string? returnedOrderItemIds = null;

        Order ord = await OrderRepository.GetWithDetailsAsync(OrderId);

        decimal TotalAmount = ord.TotalAmount;

        int TotalQuantity = ord.TotalQuantity;

        GroupBuy groupBuy = new();

        using (DataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await GroupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        List<OrderItemsCreateDto> orderItems = [];

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order1 = await OrderManager.CreateAsync(
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
                    //await OrderItemRepository.DeleteAsync(item.Id);

                    OrderManager.AddOrderItem(
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

            await OrderRepository.InsertAsync(order1);
            await OrderRepository.UpdateAsync(ord);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            newOrder = order1;

            newOrder.TotalAmount = newOrder.OrderItems.Sum(x => x.TotalAmount);
            newOrder.TotalQuantity = newOrder.OrderItems.Sum(x => x.Quantity);
            var OrderDelivery1 = await OrderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == ord.Id);
            await OrderDeliveryRepository.DeleteAsync(OrderDelivery1.Id);
            foreach (var item in ord.OrderItems.Where(x => x.ItemPrice >= 0))
            {
                if (ord.ShippingStatus == ShippingStatus.PrepareShipment)
                {
                    await CreateOrderDeliveriesAsync(ord);
                }
            }

            ord.RefundAmount += newOrder.TotalAmount;

            ord.ReturnedOrderItemIds = returnedOrderItemIds;

            await OrderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await RefundAppService.CreateAsync(newOrder.Id);
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Order History for Refund Request**
            await OrderHistoryManager.AddOrderHistoryAsync(
     ord.Id,
     "RefundRequested", // Localization key
     new object[] { newOrder.TotalAmount.ToString("C", new CultureInfo("en-US")) }, // Format currency correctly before passing
     currentUserId,
     currentUserName
 );


            // **Log the creation of the new refunded order**
            await OrderHistoryManager.AddOrderHistoryAsync(
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
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Store the old status before updating
        var oldStatus = order.ShippingStatus;
        order.ShippingStatus = status;
        //order.ClosedBy = CurrentUser.Name;
        order.CancellationDate = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnResult = ObjectMapper.Map<Order, OrderDto>(order);

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        if (status == ShippingStatus.Delivered)
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(currentUserId);
            if (order.InvoiceNumber.IsNullOrEmpty())
            {
                if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Delivered)
                {
                    if (order.GroupBuy.IssueInvoice)
                    {
                        order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                        await UnitOfWorkManager.Current.SaveChangesAsync();
                        //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                        var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                        if (invoiceDely == 0)
                        {
                            var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                            returnResult.InvoiceMsg = result;


                        }
                        else
                        {
                            //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                            //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                            //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                        }
                    }
                }
            }
        }


        // **Log Order History for Status Change**
        await OrderHistoryManager.AddOrderHistoryAsync(
            order.Id,
            "StatusChanged",
            new object[] { L[oldStatus.ToString()]?.Value, L[status.ToString()]?.Value },
            currentUserId,
            currentUserName
        );
        return returnResult;


    }
    public async Task RefundAmountAsync(double amount, Guid OrderId)
    {
        Order ord = await OrderRepository.GetWithDetailsAsync(OrderId);

        ord.RefundAmount += (decimal)amount;

        GroupBuy groupBuy = new();

        using (DataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await GroupBuyRepository.GetAsync(ord.GroupBuyId);
        }

        using (CurrentTenant.Change(groupBuy?.TenantId))
        {
            Order order1 = await OrderManager.CreateAsync(
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

            await OrderRepository.InsertAsync(order1);

            await OrderRepository.UpdateAsync(ord);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            await RefundAppService.CreateAsync(order1.Id);
            // **Get Current User (Editor)**
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            // **Log Refund Action in Order History**
            await OrderHistoryManager.AddOrderHistoryAsync(
      ord.Id,
      "RefundProcessed", // Localization key
      new object[] { ((decimal)amount).ToString("C", new CultureInfo("en-US")), ord.OrderNo }, // Format currency correctly before passing
      currentUserId,
      currentUserName
  );

            // **Log New Refund Order Creation**
            await OrderHistoryManager.AddOrderHistoryAsync(
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
        Order order = await OrderRepository.GetWithDetailsAsync(id);

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<List<OrderHistoryDto>> GetOrderLogsAsync(Guid orderId)
    {
        var result = await OrderHistoryRepository.GetAllHistoryByOrderIdAsync(orderId);

        return ObjectMapper.Map<List<OrderHistory>, List<OrderHistoryDto>>(result);

    }
    public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        if (input.Sorting.IsNullOrEmpty()) input.Sorting = $"{nameof(Order.CreationTime)} desc";

        long totalCount = await OrderRepository.CountAsync(input.Filter,
                                                            input.GroupBuyId,
                                                            input.StartDate,
                                                            input.EndDate,
                                                            input.OrderStatus,
                                                            input.ShippingStatus,
                                                            input.DeliveryMethod);

        List<Order> items = await OrderRepository.GetListAsync(input.SkipCount,
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
                GroupBuy groupbuy = await GroupBuyRepository.GetAsync(input.GroupBuyId.Value);

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

        var data = await OrderRepository.GetReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate, input.OrderStatus);

        var dtos = ObjectMapper.Map<List<GroupBuyReportOrderModel>, List<GroupBuyReportOrderDto>>(data.Items);

        if (hideCredentials)
        {
            var groupbuy = await GroupBuyRepository.GetAsync(input.GroupBuyId.Value);
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
        using (DataFilter.Disable<IMultiTenant>())
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await OrderRepository.CountAsync(input.Filter, input.GroupBuyId);

            var items = await OrderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }
    }

    public async Task<long> GetReturnOrderNotificationCount()
    {
        var totalCount = await OrderRepository.ReturnOrderNotificationCountAsync();
        return totalCount;
    }
    public async Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Order.CreationTime)} desc";
        }

        var totalCount = await OrderRepository.ReturnOrderCountAsync(input.Filter, input.GroupBuyId);

        var items = await OrderRepository.GetReturnListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId);

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

        var totalCount = await OrderRepository.CountReconciliationAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

        var items = await OrderRepository.GetReconciliationListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

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

        var totalCount = await OrderRepository.CountVoidAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

        var items = await OrderRepository.GetVoidListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }
    [Authorize(PikachuPermissions.Orders.AddStoreComment)]
    public async Task AddStoreCommentAsync(Guid id, string comment)
    {
        var order = await OrderRepository.GetAsync(id);
        await OrderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
        OrderManager.AddStoreComment(order, comment);
    }

    [Authorize(PikachuPermissions.Orders.AddStoreComment)]
    public async Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
    {
        var order = await OrderRepository.GetAsync(id);
        await OrderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
        var storeComment = order.StoreComments.First(c => c.Id == commentId);
        if (storeComment.CreatorId != CurrentUser.Id)
        {
            throw new UnauthorizedAccessException();
        }
        storeComment.Comment = comment;
    }

    public async Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
    {
        var order = await OrderRepository.GetAsync(id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        // Capture changes in order details for logging
        List<string> changes = new();

        if (order.RecipientName != input.RecipientName)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RecipientNameChanged", // Localization key for ActionType
 new object[] { order.RecipientName, input.RecipientName }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.RecipientPhone != input.RecipientPhone)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RecipientPhoneChanged", // Localization key for ActionType
 new object[] { order.RecipientPhone, input.RecipientPhone }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.PostalCode != input.PostalCode)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "PostalCodeChanged", // Localization key for ActionType
 new object[] { order.PostalCode, input.PostalCode }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.District != input.District)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "DistrictChanged", // Localization key for ActionType
 new object[] { order.District, input.District }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.City != input.City)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "CityChanged", // Localization key for ActionType
 new object[] { order.City, input.City }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.Road != input.Road)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "RoadChanged", // Localization key for ActionType
 new object[] { order.Road, input.Road }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.AddressDetails != input.AddressDetails)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "AddressDetailsChanged", // Localization key for ActionType
 new object[] { order.AddressDetails, input.AddressDetails }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.OrderStatus != input.OrderStatus)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "OrderStatusChanged", // Localization key for ActionType
 new object[] { order.OrderStatus, input.OrderStatus }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.StoreId != input.StoreId)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
 order.Id,
 "StoreIdChanged", // Localization key for ActionType
 new object[] { order.StoreId, input.StoreId }, // Pass localized changes as an array
 currentUserId,
 currentUserName
);

        }


        if (order.CVSStoreOutSide != input.CVSStoreOutSide)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
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

        await OrderRepository.UpdateAsync(order);

        if (input.ShouldSendEmail)
        {
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await EmailAppService.SendOrderUpdateEmailAsync(order.Id);
        }




        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus, bool isRefund)
    {
        var order = await OrderRepository.GetAsync(id);
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
            await RefundAppService.CreateAsync(id);
            var refund = (await RefundRepository.GetQueryableAsync()).Where(x => x.OrderId == order.Id).FirstOrDefault();
            await RefundAppService.UpdateRefundReviewAsync(refund.Id, RefundReviewStatus.Proccessing);
            await RefundAppService.SendRefundRequestAsync(refund.Id);

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Returned, TransactionStatus.Successful, order.PaymentMethod?.GetPaymentChannel());
            await OrderTransactionManager.CreateAsync(orderTransaction);
        }
        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
        {
            order.ExchangeBy = CurrentUser.UserName;
            order.ExchangeTime = DateTime.Now;
            order.ShippingStatus = ShippingStatus.Exchange;

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Exchange, TransactionStatus.Successful, order.PaymentMethod?.GetPaymentChannel());
            await OrderTransactionManager.CreateAsync(orderTransaction);
        }
        await OrderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Return Status Change**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "ReturnStatusChanged", // Localization key
     new object[] { L[oldReturnStatus.ToString()]?.Value, L[orderReturnStatus.ToString()]?.Value }, // Dynamic placeholders
     currentUserId,
     currentUserName
 );


        // **Log Additional Details for Refund or Exchange**
        if (orderReturnStatus == OrderReturnStatus.Approve && order.OrderStatus == OrderStatus.Returned)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
                 order.Id,
                 "RefundInitiated", // Localization key
                 new object[] { order.OrderNo }, // Dynamic placeholders
                 currentUserId,
                 currentUserName
             );

        }

        if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
        {
            await OrderHistoryManager.AddOrderHistoryAsync(
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
        var order = await OrderRepository.GetAsync(id);
        // Capture old statuses before updating
        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Exchange;

        await OrderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Exchange**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderExchangeInitiated", // Localization key
     new object[]
     {
        L[oldReturnStatus.ToString()]?.Value,
        L[order.ReturnStatus.ToString()]?.Value,
        L[oldOrderStatus.ToString()]?.Value,
        L[order.OrderStatus.ToString()]?.Value
     }, // Dynamic placeholders for localized statuses
     currentUserId,
     currentUserName
 );



    }
    public async Task ReturnOrderAsync(Guid id)
    {
        var order = await OrderRepository.GetAsync(id);
        // Capture old statuses before updating
        var oldReturnStatus = order.ReturnStatus;
        var oldOrderStatus = order.OrderStatus;
        order.ReturnStatus = OrderReturnStatus.Pending;
        order.OrderStatus = OrderStatus.Returned;

        await OrderRepository.UpdateAsync(order);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Return**
        await OrderHistoryManager.AddOrderHistoryAsync(
       order.Id,
       "OrderReturnInitiated", // Localization key
       new object[]
       {
        L[oldReturnStatus.ToString()].Name,
        L[order.ReturnStatus.ToString()].Name,
        L[oldOrderStatus.ToString()].Name,
        L[order.OrderStatus.ToString()].Name
       }, // Dynamic placeholders for localized statuses
       currentUserId,
       currentUserName
   );


    }
    public async Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);


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
                await OrderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "ItemQuantityChanged", // Localization key
                  new object[] { itemName, orderItem.Quantity, item.Quantity }, // Join localized changes as a single string
                  currentUserId,
                  currentUserName
              );
            }


            if (orderItem.ItemPrice != item.ItemPrice)
            {
                await OrderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "ItemPriceChanged", // Localization key
                  new object[] { itemName, orderItem.ItemPrice.ToString("C", new CultureInfo("en-US")), item.ItemPrice.ToString("C", new CultureInfo("en-US")) }, // Join localized changes as a single string
                  currentUserId,
                  currentUserName
              );
            }


            if (orderItem.TotalAmount != item.TotalAmount)
            {
                await OrderHistoryManager.AddOrderHistoryAsync(
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
        await OrderRepository.UpdateAsync(order);



    }
    public async Task CancelOrderAsync(Guid id)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Capture the old status before updating
        var oldOrderStatus = order.OrderStatus;
        order.OrderStatus = OrderStatus.Closed;
        order.CancellationDate = DateTime.Now;
        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id, OrderStatus.Closed);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Cancellation**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderCancelled", // Localization key
     new object[] { L[oldOrderStatus.ToString()].Name }, // Localized previous status
     currentUserId,
     currentUserName
 );

    }
    public async Task VoidInvoice(Guid id, string reason)
    {
        var order = await OrderRepository.GetAsync(id);
        // Capture old invoice status before voiding
        var oldInvoiceStatus = order.InvoiceStatus;
        order.IsVoidInvoice = true;
        order.VoidReason = reason;
        order.VoidUser = CurrentUser.Name;
        order.VoidDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.InvoiceVoided;
        order.LastModificationTime = DateTime.Now;
        order.LastModifierId = CurrentUser.Id;
        await OrderRepository.UpdateAsync(order);
        await ElectronicInvoiceAppService.CreateVoidInvoiceAsync(id, reason);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Invoice Voiding**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "InvoiceVoided", // Localization key
     new object[] { L[oldInvoiceStatus.ToString()].Name, reason }, // Localized invoice status & reason
     currentUserId,
     currentUserName
 );


    }
    public async Task CreditNoteInvoice(Guid id, string reason)
    {
        Order order = await OrderRepository.GetAsync(id);
        // Capture old invoice status before issuing the credit note
        var oldInvoiceStatus = order.InvoiceStatus;
        order.IsVoidInvoice = true;
        order.CreditNoteReason = reason;
        order.CreditNoteUser = CurrentUser.Name;
        order.CreditNoteDate = DateTime.Now;
        order.InvoiceStatus = InvoiceStatus.CreditNote;

        await OrderRepository.UpdateAsync(order);

        await ElectronicInvoiceAppService.CreateCreditNoteAsync(id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Credit Note Issuance**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "CreditNoteIssued", // Localization key
     new object[] { L[oldInvoiceStatus.ToString()].Name, reason }, // Localized previous status & reason
     currentUserId,
     currentUserName
 );
    }
    public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);

        // Capture old shipping details before updating
        var oldDeliveryMethod = order.DeliveryMethod;
        var oldShippingNumber = order.ShippingNumber;
        var oldShippingStatus = order.ShippingStatus;
        order.DeliveryMethod = input.DeliveryMethod;
        order.ShippingNumber = input.ShippingNumber;
        order.ShippingStatus = ShippingStatus.Shipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
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
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "ShippingDetailsUpdated", // Localization key
     new object[]
     {
        L[oldDeliveryMethod.ToString()].Name,
        L[order.DeliveryMethod.ToString()].Name,
        oldShippingNumber,
        order.ShippingNumber,
        L[oldShippingStatus.ToString()].Name,
        L[order.ShippingStatus.ToString()].Name
     }, // Dynamic placeholders for localized statuses and tracking numbers
     currentUserId,
     currentUserName
 );

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderShipped(Guid id)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;

        order.ShippingStatus = ShippingStatus.Shipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await EmailAppService.SendLogisticsEmailAsync(order.Id);
        await SendEmailAsync(order.Id);
        var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentUser.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        string result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);

                        returnOrder.InvoiceMsg = result;
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping**
        await OrderHistoryManager.AddOrderHistoryAsync(
      order.Id,
      "OrderShipped", // Localization key
      new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name }, // Dynamic placeholders for localized statuses
      currentUserId,
      currentUserName
  );

        return returnOrder;
        // await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);

    }
    public async Task<OrderDto> OrderToBeShipped(Guid id)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
        order.ShippingStatus = ShippingStatus.ToBeShipped;
        order.ShippedBy = CurrentUser.Name;
        order.ShippingDate = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.ToBeShipped)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                        returnOrder.InvoiceMsg = result;
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        // await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Shipping Status Change**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderToBeShipped", // Localization key
     new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

        return returnOrder;

    }
    public async Task<OrderDto> OrderClosed(Guid id)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
        order.ShippingStatus = ShippingStatus.Closed;
        order.ClosedBy = CurrentUser.Name;
        order.CancellationDate = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Order Closure**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderClosed", // Localization key
     new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
    public async Task<OrderDto> OrderComplete(Guid id)
    {
        var order = await OrderRepository.GetWithDetailsAsync(id);
        // Capture old shipping status before updating
        var oldShippingStatus = order.ShippingStatus;
        order.ShippingStatus = ShippingStatus.Completed;
        order.CompletedBy = CurrentUser.Name;
        order.CompletionTime = DateTime.Now;

        await OrderRepository.UpdateAsync(order);
        await UnitOfWorkManager.Current.SaveChangesAsync();
        await SendEmailAsync(order.Id);
        var returnResult = ObjectMapper.Map<Order, OrderDto>(order);
        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
            if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Completed)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                    var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDely == 0)
                    {
                        var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                        returnResult.InvoiceMsg = result;

                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";

        // **Log Order History for Order Completion**
        await OrderHistoryManager.AddOrderHistoryAsync(
     order.Id,
     "OrderCompleted", // Localization key
     new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name }, // Localized placeholders
     currentUserId,
     currentUserName
 );

        return returnResult;

    }

    private async Task SendEmailAsync(Guid id, OrderStatus? orderStatus = null)
    {
        await EmailAppService.SendOrderStatusEmailAsync(id);
    }

    public async Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo, PaymentMethods? paymentMethod = null)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            Order order = await OrderRepository.GetAsync(id);

            order.MerchantTradeNo = merchantTradeNo;

            order.CheckMacValue = checkMacValue;

            if (paymentMethod.HasValue) order.PaymentMethod = paymentMethod;

            await OrderRepository.UpdateAsync(order);
        }
    }

    [AllowAnonymous]
    public async Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            Order order = await OrderRepository.GetAsync(id);

            order.MerchantTradeNo = merchantTradeNo;

            order.CheckMacValue = checkMacValue;

            await OrderRepository.UpdateAsync(order);
        }
    }

    [AllowAnonymous]
    public async Task AddCheckMacValueAsync(Guid id, string checkMacValue)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var order = await OrderRepository.GetAsync(id);
            order.CheckMacValue = checkMacValue;

            await OrderRepository.UpdateAsync(order);
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
        Order order = await OrderRepository.GetOrderByMerchantTradeNoAsync(merchantTradeNo);
        // Capture old logistics status and shipping status before updating
        var oldLogisticsStatus = order.EcpayLogisticsStatus;
        var oldShippingStatus = order.ShippingStatus;

        order.EcpayLogisticsStatus = rtnMsg;
        order.EcpayLogisticRtnCode = rtnCode;

        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "EcPay Logistic Update";

        // **Log Order History for Logistic Status Update**
        await OrderHistoryManager.AddOrderHistoryAsync(
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

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "ShippingStatusUpdated", // Localization key
                new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );
        }

        await OrderRepository.UpdateAsync(order);
    }

    [AllowAnonymous]
    public async Task CloseOrdersAsync()
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var ordersToClose = await OrderRepository.GetOrdersToCloseAsync();
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

            await OrderHistoryRepository.InsertManyAsync(orderHistoryList);
        }
    }

    [AllowAnonymous]
    public async Task<string> HandlePaymentAsync(PaymentResult paymentResult)
    {
        if (paymentResult.SimulatePaid is 0)
        {
            Order order = new();
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
            using (DataFilter.Disable<IMultiTenant>())
            {
                if (paymentResult.OrderId is null)
                {
                    order = await OrderRepository
                                   .FirstOrDefaultAsync(o => o.MerchantTradeNo == paymentResult.MerchantTradeNo)
                                   ?? throw new EntityNotFoundException();

                    order = await OrderRepository.GetWithDetailsAsync(order.Id);

                    if (paymentResult.CustomField1 != order.CheckMacValue) throw new Exception();

                    if (paymentResult.TradeAmt != order.TotalAmount) throw new Exception();
                }
                else
                {
                    order = await OrderRepository
                                      .FirstOrDefaultAsync(o => o.Id == paymentResult.OrderId)
                                      ?? throw new EntityNotFoundException();
                    order = await OrderRepository.GetWithDetailsAsync(order.Id);
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
                await OrderHistoryManager.AddOrderHistoryAsync(
                      order.Id,
                      "PaymentProcessed", // Localization key
                      new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name }, // Localized placeholders
                      currentUserId,
                      currentUserName
                  );
                order.PrepareShipmentBy = CurrentUser.Name ?? "System";
                _ = DateTime.TryParse(paymentResult.PaymentDate, out DateTime parsedDate);
                order.PaymentDate = paymentResult.OrderId == null ? parsedDate : DateTime.Now;

                var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                    order.TotalAmount, TransactionType.Payment, TransactionStatus.Successful, PaymentChannel.EcPay);
                await OrderTransactionManager.CreateAsync(orderTransaction);

                await CreateOrderDeliveriesAsync(order);

                await OrderRepository.UpdateAsync(order);
                await SendEmailAsync(order.Id);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                if (order.InvoiceNumber.IsNullOrEmpty())
                {
                    if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Processing)
                    {
                        if (order.GroupBuy.IssueInvoice)
                        {
                            order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                            //var invoiceSetting = await ElectronicInvoiceSettingRepository.FirstOrDefaultAsync();
                            var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                            if (invoiceDely == 0)
                            {
                                string invoiceMsg = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                                await OrderRepository.UpdateAsync(order);
                                await UnitOfWorkManager.Current.SaveChangesAsync();
                                return invoiceMsg;
                            }
                            else
                            {
                                //var delay = DateTime.Now.AddDays(invoiceDely) - DateTime.Now;
                                //GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
                                //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
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
        using (DataFilter.Disable<IMultiTenant>())
        {
            groupBuy = await GroupBuyRepository.GetAsync(id);
        }
        using (CurrentTenant.Change(groupBuy.TenantId))
        {
            PaymentGateway? paymentGateway = await PaymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);

            PaymentGatewayDto paymentGatewayDto = ObjectMapper.Map<PaymentGateway, PaymentGatewayDto>(paymentGateway);

            PropertyInfo[] properties = typeof(PaymentGatewayDto).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = (string?)property.GetValue(paymentGatewayDto);

                    if (!string.IsNullOrEmpty(value))
                    {
                        var decryptedValue = StringEncryptionService.Decrypt(value);
                        property.SetValue(paymentGatewayDto, decryptedValue);
                    }
                }
            }

            return paymentGatewayDto;
        }
    }

    public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input)
    {
        var items = await OrderRepository.GetListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
        var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

        // Create a dictionary for localized headers
        var headers = new Dictionary<string, string>
        {
            { "OrderNumber", L["OrderNo"] },
            { "OrderDate", L["OrderDate"] },
            { "CustomerName", L["CustomerName"] },
            { "Email", L["Email"] },
            { "RecipientInformation", L["RecipientInformation"] },
            { "ShippingMethod", L["ShippingMethod"] },
            { "Address", L["Address"] },
            { "Notes", L["Notes"] },
            { "MerchantNotes", L["MerchantNotes"] },
            { "OrderedItems", L["OrderedItems"] },
            { "InvoiceStatus", L["InvoiceStatus"] },
            { "ShippingStatus", L["ShippingStatus"] },
            { "PaymentMethod", L["PaymentMethod"] },
            { "CheckoutAmount", L["CheckoutAmount"] }
        };

        var excelContent = Results.Select(x => new Dictionary<string, object>
        {
            { headers["OrderNumber"], x.OrderNo },
            { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
            { headers["CustomerName"], x.CustomerName },
            { headers["Email"], x.CustomerEmail },
            { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
            { headers["ShippingMethod"], L[x.DeliveryMethod.ToString()] },
            { headers["Address"], x.AddressDetails },
            { headers["Notes"], x.Remarks },
            { headers["MerchantNotes"], x.Remarks },
            { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
            )},
            { headers["InvoiceStatus"], L[x.InvoiceStatus.ToString()] },
            { headers["ShippingStatus"], L[x.ShippingStatus.ToString()] },
            { headers["PaymentMethod"], L[x.PaymentMethod.ToString()] },
            { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelContent);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input)
    {
        var items = await OrderRepository.GetReconciliationListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
        var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

        // Create a dictionary for localized headers
        var headers = new Dictionary<string, string>
        {
            { "OrderNumber", L["OrderNo"] },
            { "OrderDate", L["OrderDate"] },
            { "CustomerName", L["CustomerName"] },
            { "Email", L["Email"] },
            { "RecipientInformation", L["RecipientInformation"] },
            { "ShippingMethod", L["ShippingMethod"] },
            { "Address", L["Address"] },
            { "Notes", L["Notes"] },
            { "MerchantNotes", L["MerchantNotes"] },
            { "OrderedItems", L["OrderedItems"] },
            { "InvoiceStatus", L["InvoiceStatus"] },
            { "ShippingStatus", L["ShippingStatus"] },
            { "PaymentMethod", L["PaymentMethod"] },
            { "CheckoutAmount", L["CheckoutAmount"] },
            { "DeliveryMethod", L["DeliveryMethod"] }
        };

        var excelContent = Results.Select(x => new Dictionary<string, object>
        {
            { headers["OrderNumber"], x.OrderNo },
            { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
            { headers["CustomerName"], x.CustomerName },
            { headers["Email"], x.CustomerEmail },
            { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
            { headers["ShippingMethod"], L[x.DeliveryMethod.ToString()] },
            { headers["Address"], x.AddressDetails },
            { headers["Notes"], x.Remarks },
            { headers["MerchantNotes"], x.Remarks },
            { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
            )},
            { headers["InvoiceStatus"], L[x.InvoiceStatus.ToString()] },
            { headers["ShippingStatus"], L[x.ShippingStatus.ToString()] },
            { headers["PaymentMethod"], L[x.PaymentMethod.ToString()] },
            { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") },
            { headers["DeliveryMethod"], L[x.DeliveryMethod.ToString()] }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelContent);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "Reconciliation Report.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId)
    {
        await OrderRepository.UpdateOrdersIfIsEnterpricePurchaseAsync(groupBuyId);
    }

    public async Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync()
    {
        return await OrderRepository.GetTotalDeliveryTemperatureCountsAsync();
    }

    public async Task<(decimal PaidAmount, decimal UnpaidAmount, decimal RefundedAmount)> GetOrderStatusAmountsAsync(Guid userId)
    {
        // Sum of Paid orders
        var paidAmount = (await OrderRepository.GetQueryableAsync())
            .Where(order => order.UserId == userId
                    && OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus))
            .Sum(order => order.TotalAmount);

        // Sum of Unpaid/Due orders
        var unpaidAmount = (await OrderRepository.GetQueryableAsync())
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
        var refundedAmount = (await OrderRepository.GetQueryableAsync())
            .Where(order => order.UserId == userId && order.IsRefunded)
            .Sum(order => order.RefundAmount);

        return (paidAmount, unpaidAmount, refundedAmount);
    }

    public async Task<(int Open, int Exchange, int Return)> GetOrderStatusCountsAsync(Guid userId)
    {
        // Sum of Paid orders
        var paidAmount = (await OrderRepository.GetQueryableAsync())
            .Where(order =>
                    OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)
                    && order.UserId == userId
                    )

            .Count();

        // Sum of Unpaid/Due orders
        var unpaidAmount = (await OrderRepository.GetQueryableAsync())
            .Where(order =>
                (order.OrderStatus == OrderStatus.Exchange)
                     && order.UserId == userId)
            .Count();

        // Sum of Refunded orders
        var refundedAmount = (await OrderRepository.GetQueryableAsync())
            .Where(order => order.OrderStatus == OrderStatus.Exchange && order.UserId == userId)
            .Count();

        return (paidAmount, unpaidAmount, refundedAmount);
    }

    public async Task ExpireOrderAsync(Guid OrderId)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var order = await OrderRepository.GetWithDetailsAsync(OrderId);
            if (order.ShippingStatus == ShippingStatus.WaitingForPayment)
            {
                // Capture previous order status before updating
                var oldOrderStatus = order.OrderStatus;
                var oldShippingStatus = order.ShippingStatus;

                order.OrderStatus = OrderStatus.Closed;
                order.ShippingStatus = ShippingStatus.Closed;

                foreach (var orderItem in order.OrderItems)
                {

                    var details = await ItemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);

                    if (details != null)
                    {
                        details.SaleableQuantity += orderItem.Quantity;
                        details.StockOnHand += orderItem.Quantity;

                        await ItemDetailsRepository.UpdateAsync(details);
                    }

                    if (orderItem.FreebieId != null)
                    {
                        var freebie = await FreebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);

                        if (freebie != null)
                        {
                            freebie.FreebieAmount += orderItem.Quantity;
                            await FreebieRepository.UpdateAsync(freebie);
                        }
                    }

                }

                await OrderRepository.UpdateAsync(order);
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Order Expiration**
                await OrderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "OrderExpired", // Localization key
                    [
                        L[oldOrderStatus.ToString()].Name,
                        L[order.OrderStatus.ToString()].Name,
                        L[oldShippingStatus.ToString()].Name,
                        L[order.ShippingStatus.ToString()].Name,
                        order.OrderItems.Count
                    ], // Dynamic placeholders
                    currentUserId,
                    currentUserName
                );
            }
        }
    }

    private async Task CreateOrderDeliveriesAsync(Order order)
    {
        var temperatures = Enum.GetValues<ItemStorageTemperature>();
        foreach (ItemStorageTemperature temperature in temperatures)
        {
            var temperatureOrderItems = order.OrderItems
                .Where(x => x.DeliveryTemperature == temperature && x.FreebieId == null)
                .ToList();
            if (temperatureOrderItems.Count > 0)
            {
                var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Processing, null, "", order.Id);
                OrderDelivery = await OrderDeliveryRepository.InsertAsync(OrderDelivery);
                order.UpdateOrderItem(temperatureOrderItems, OrderDelivery.Id);
            }
        }

        if (order.OrderItems.Any(x => x.FreebieId != null))
        {
            OrderDelivery? orderDelivery = await OrderDeliveryRepository.FirstOrDefaultAsync(x => x.OrderId == order.Id);
            if (orderDelivery is not null)
                order.UpdateOrderItem(order.OrderItems.Where(x => x.FreebieId != null).ToList(), orderDelivery.Id);
        }
    }

    [AllowAnonymous]
    public async Task CreateOrderDeliveriesAndInvoiceAsync(Guid orderId)
    {
        var order = await OrderRepository.GetWithDetailsAsync(orderId);
        await CreateOrderDeliveriesAsync(order);

        await SendEmailAsync(order.Id);

        if (order.InvoiceNumber.IsNullOrEmpty())
        {
            var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);

            if (invoiceSetting?.StatusOnInvoiceIssue == DeliveryStatus.Processing)
            {
                if (order.GroupBuy.IssueInvoice)
                {
                    order.IssueStatus = IssueInvoiceStatus.SentToBackStage;

                    var invoiceDelay = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                    if (invoiceDelay == 0)
                    {
                        await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                    }
                    else
                    {
                        //var delay = DateTime.Now.AddDays(invoiceDelay) - DateTime.Now;
                        //GenerateInvoiceBackgroundJobArgs args = new() { OrderId = order.Id };
                        //var jobid = await BackgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
                    }
                }
            }
        }
    }

    public required MemberTagManager MemberTagManager { get; init; }
    public required OrderManager OrderManager { get; init; }
    public required OrderHistoryManager OrderHistoryManager { get; init; }
    public required OrderTransactionManager OrderTransactionManager { get; init; }
    public required IRepository<OrderItem, Guid> OrderItemRepository { get; init; }
    public required IRepository<OrderDelivery, Guid> OrderDeliveryRepository { get; init; }
    public required IRepository<PaymentGateway, Guid> PaymentGatewayRepository { get; init; }
    public required IOrderRepository OrderRepository { get; init; }
    public required IGroupBuyRepository GroupBuyRepository { get; init; }
    public required IBackgroundJobManager BackgroundJobManager { get; init; }
    public required IStringEncryptionService StringEncryptionService { get; init; }
    public required IItemDetailsRepository ItemDetailsRepository { get; init; }
    public required IOrderInvoiceAppService ElectronicInvoiceAppService { get; init; }
    public required IFreebieRepository FreebieRepository { get; init; }
    public required IRefundAppService RefundAppService { get; init; }
    public required IRefundRepository RefundRepository { get; init; }
    public required IDiscountCodeRepository DiscountCodeRepository { get; init; }
    public required IUserShoppingCreditAppService UserShoppingCreditAppService { get; init; }
    public required IEmailAppService EmailAppService { get; init; }
    public required IOrderMessageAppService OrderMessageAppService { get; init; }
    public required ISetItemRepository SetItemRepository { get; init; }
    public required IUserCumulativeCreditAppService UserCumulativeCreditAppService { get; init; }
    public required IUserCumulativeCreditRepository UserCumulativeCreditRepository { get; init; }
    public required IIdentityUserRepository UserRepository { get; init; }
    public required IOrderHistoryRepository OrderHistoryRepository { get; init; }
    public required IMemberRepository MemberRepository { get; init; }
    public required ITenantTripartiteRepository TenantTripartiteRepository { get; init; }
    public required ICampaignRepository CampaignRepository { get; init; }
}