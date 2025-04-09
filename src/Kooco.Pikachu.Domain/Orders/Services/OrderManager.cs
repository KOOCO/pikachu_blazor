using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Orders.Services;
public class OrderManager(IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository) : DomainService
{
    public async Task<Order> CreateAsync(
        Guid groupBuyId,
        bool isIndividual,
        string customerName,
        string customerPhone,
        string customerEmail,
        PaymentMethods? paymentMethods,
        InvoiceType? invoiceType,
        string invoiceNumber,
        string uniformNumber,
        string carrierId,
        string taxTitle,
        bool IsAsSameBuyer,
        string recipientName,
        string recipientPhone,
        string recipientEmail,
        DeliveryMethod? deliveryMethod,
        string postalCode,
        string city,
        string district,
        string road,
        string addressDetails,
        string remarks,
        ReceivingTime? receivingTime,
        int totalQuantity,
        decimal totalAmount,
        OrderReturnStatus? orderReturnStatus,
        OrderType? orderType,
        Guid? splitFromId = null,
        Guid? userId = null,
        int creditDeductionAmount = 0,
        Guid? creditDeductionRecordId = null,
        decimal creditRefundAmount = 0,
        Guid? creditRefundRecordId = null,
        int? discountCodeAmount = null,
        Guid? discountAmountId = null,
        string? recipientNameDbsNormal = null,
        string? recipientNameDbsFreeze = null,
        string? recipientNameDbsFrozen = null,
        string? recipientPhoneDbsNormal = null,
        string? recipientPhoneDbsFreeze = null,
        string? recipientPhoneDbsFrozen = null,
        string? postalCodeDbsNormal = null,
        string? postalCodeDbsFreeze = null,
        string? postalCodeDbsFrozen = null,
        string? cityDbsNormal = null,
        string? cityDbsFreeze = null,
        string? cityDbsFrozen = null,
        string? addressDetailsDbsNormal = null,
        string? addressDetailsDbsFreeze = null,
        string? addressDetailsDbsFrozen = null,
        string? remarksDbsNormal = null,
        string? remarksDbsFreeze = null,
        string? remarksDbsFrozen = null,
        string? storeIdNormal = null,
        string? storeIdFreeze = null,
        string? storeIdFrozen = null,
        string? cVSStoreOutSideNormal = null,
        string? cVSStoreOutSideFreeze = null,
        string? cVSStoreOutSideFrozen = null,
        ReceivingTime? receivingTimeNormal = null,
        ReceivingTime? receivingTimeFreeze = null,
        ReceivingTime? receivingTimeFrozen = null
   )
    {
        //string orderNo = await GenerateOrderNoAsync(groupBuyId);
        Guid newGuid = Guid.NewGuid();
        string orderNo = newGuid.ToString().Replace("-", "");
        orderNo = orderNo.Length >= 10 ? orderNo.Substring(0, 11) : orderNo;
        orderNo = orderNo.ToUpper();

        return new Order(
            GuidGenerator.Create(),
            groupBuyId,
            orderNo,
            isIndividual,
            customerName,
            customerPhone,
            customerEmail,
            paymentMethods,
            invoiceType,
            invoiceNumber,
            uniformNumber,
            carrierId,
            taxTitle,
            IsAsSameBuyer,
            recipientName,
            recipientPhone,
            recipientEmail,
            deliveryMethod,
            postalCode,
            city,
            district,
            road,
            addressDetails,
            remarks,
            receivingTime,
            totalQuantity,
            totalAmount,
            orderReturnStatus,
            orderType,
            splitFromId,
            userId,
            creditDeductionAmount,
            creditDeductionRecordId,
            creditRefundAmount,
            creditRefundRecordId,
            discountAmountId,
            discountCodeAmount,
            recipientNameDbsNormal,
            recipientNameDbsFreeze,
            recipientNameDbsFrozen,
            recipientPhoneDbsNormal,
            recipientPhoneDbsFreeze,
            recipientPhoneDbsFrozen,
            postalCodeDbsNormal,
            postalCodeDbsFreeze,
            postalCodeDbsFrozen,
            cityDbsNormal,
            cityDbsFreeze,
            cityDbsFrozen,
            addressDetailsDbsNormal,
            addressDetailsDbsFreeze,
            addressDetailsDbsFrozen,
            remarksDbsNormal,
            remarksDbsFreeze,
            remarksDbsFrozen,
            storeIdNormal,
            storeIdFreeze,
            storeIdFrozen,
            cVSStoreOutSideNormal,
            cVSStoreOutSideFreeze,
            cVSStoreOutSideFrozen,
            receivingTimeNormal,
            receivingTimeFreeze,
            receivingTimeFrozen
        );
    }

    public void AddOrderItem(
        Order order,
        Guid? itemId,
        Guid? setItemId,
        Guid? freebieId,
        ItemType itemType,
        Guid orderId,
        string? spec,
        decimal itemPrice,
        decimal totalAmount,
        int quantity,
        string? sku,
        ItemStorageTemperature temperature,
        decimal temperatureCost
    )
    {
        order.AddOrderItem(
            GuidGenerator.Create(),
            itemId,
            setItemId,
            freebieId,
            itemType,
            spec,
            itemPrice,
            totalAmount,
            quantity,
            sku,
            temperature,
            temperatureCost
        );
    }

    async Task<string> GenerateOrderNoAsync(Guid groupBuyId)
    {
        var order = await orderRepository.MaxByOrderNumberAsync();

        long orderNo = 1;
        if (order != null)
        {
            string lastNineDigits = order.OrderNo[^9..];
            _ = long.TryParse(lastNineDigits, out orderNo);
            orderNo++;
        }

        var groupBuy = await groupBuyRepository.GetAsync(groupBuyId);
        string tenantIdPrefix = groupBuy.TenantId?.ToString().Substring(0, 2);

        return $"{tenantIdPrefix}{DateTime.Now:yy}{orderNo:D9}";
    }

    public void AddStoreComment(
        [NotNull] Order order,
        [NotNull] string comment
    )
    {
        Check.NotNullOrWhiteSpace(comment, nameof(comment));
        order.AddStoreComment(comment);
    }
}