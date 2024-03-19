using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.StoreComments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders
{
    public class Order : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string OrderNo { get; set; }
        public bool IsIndividual { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public PaymentMethods? PaymentMethod { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? UniformNumber { get; set; }
        public bool IsAsSameBuyer { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientEmail { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }
        public string? ShippingNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? District { get; set; }
        public string? Road { get; set; }
        public string? AddressDetails { get; set; }
        public string? Remarks { get; set; }
        public ReceivingTime? ReceivingTime { get; set; }
        public Guid GroupBuyId { get; set; }
        public Guid? SplitFromId { get; set; }

        [ForeignKey(nameof(GroupBuyId))]
        public GroupBuy GroupBuy { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CheckMacValue { get; set; }
        public DateTime? PaymentDate { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string PrepareShipmentBy { get; set; }
        public string ShippedBy { get; set; }
        public string ClosedBy { get; set; }
        public OrderReturnStatus? ReturnStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<StoreComment> StoreComments { get; set; }
        public bool IsRefunded { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public OrderType? OrderType { get; set; }
        public Order() { }

        public Order(
            [NotNull] Guid id,
            [NotNull] Guid groupBuyId,
            string orderNo,
            bool isIndividual,
            string? customerName,
            string? customerPhone,
            string? customerEmail,
            PaymentMethods? paymentMethods,
            InvoiceType? invoiceType,
            string invoiceNumber,
            string? uniformNumber,
            bool isAsSameBuyer,
            string? recipientName,
            string? recipientPhone,
            string? recipientEmail,
            DeliveryMethod? deliveryMethod,
            string? postalCode,
            string? city,
            string? district,
            string? road,
            string? addressDetails,
            string? remarks,
            ReceivingTime? receivingTime,
            int totalQuantity,
            decimal totalAmount,
            OrderReturnStatus? orderReturnStatus,
            OrderType? orderType,
            Guid? splitFromId=null
         )
        {
            Id = id;
            GroupBuyId = groupBuyId;
            OrderNo = orderNo;
            IsIndividual = isIndividual;
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            CustomerEmail = customerEmail;
            PaymentMethod = paymentMethods;
            InvoiceType = invoiceType;
            InvoiceNumber = invoiceNumber;
            UniformNumber = uniformNumber;
            IsAsSameBuyer = isAsSameBuyer;
            RecipientName = recipientName;
            RecipientEmail = recipientEmail;
            RecipientPhone = recipientPhone;
            DeliveryMethod = deliveryMethod;
            City = city;
            District = district;
            Road = road;
            AddressDetails = addressDetails;
            Remarks = remarks;
            ReceivingTime = receivingTime;
            TotalQuantity = totalQuantity;
            TotalAmount = totalAmount;
            OrderStatus = OrderStatus.Open;
            ShippingStatus = ShippingStatus.WaitingForPayment;
            OrderItems = new List<OrderItem>();
            StoreComments = new List<StoreComment>();
            IsRefunded = false;
            ReturnStatus = orderReturnStatus;
            OrderType = orderType;
            SplitFromId = splitFromId;
            PostalCode = postalCode;
        }

        public void AddOrderItem(
            Guid id,
            Guid? itemId,
            Guid? setItemId,
            Guid? freebieId,
            ItemType itemType,
            string? spec,
            decimal itemPrice,
            decimal totalAmount,
            int quantity,
            string? sku,
            ItemStorageTemperature temperature,
            decimal temperatureCost
            )
        {
            OrderItems.Add(new OrderItem(
                    id,
                    itemId,
                    setItemId,
                    freebieId,
                    itemType,
                    this.Id,
                    spec,
                    itemPrice,
                    totalAmount,
                    quantity,
                    sku,
                    temperature,
                    temperatureCost
                    ));
        }
        public void UpdateOrderItem(
          List<OrderItem> items,Guid DeliveryOrderId

         
          )
        {
           foreach ( OrderItem item in items )
            {

                item.DeliveryOrderId = DeliveryOrderId;


            }
        }
        internal void AddStoreComment(
            [NotNull] string comment
            )
        {
            StoreComments.AddIfNotContains(new StoreComment(comment));
        }
    }
}
