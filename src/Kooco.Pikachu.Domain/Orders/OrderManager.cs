using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Orders
{

    public class OrderManager : DomainService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGroupBuyRepository _groupBuyRepository;

        public OrderManager(
            IOrderRepository orderRepository,
            IGroupBuyRepository groupBuyRepository
            )
        {
            _orderRepository = orderRepository;
            _groupBuyRepository = groupBuyRepository;
        }

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
             bool IsAsSameBuyer,
             string recipientName,
             string recipientPhone,
             string recipientEmail,
             DeliveryMethod? deliveryMethod,
             string city,
             string district,
             string road,
             string addressDetails,
             string remarks,
             ReceivingTime? receivingTime,
             int totalQuantity,
             decimal totalAmount,
             OrderReturnStatus? orderReturnStatus,
             OrderType? orderType
             )
        {
            //string orderNo = await GenerateOrderNoAsync(groupBuyId);
            var newGuid = Guid.NewGuid();
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
                IsAsSameBuyer,
                recipientName,
                recipientPhone,
                recipientEmail,
                deliveryMethod,
                city,
                district,
                road,
                addressDetails,
                remarks,
                receivingTime,
                totalQuantity,
                totalAmount,
                orderReturnStatus,
                orderType

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
            string? sku
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
                sku
                );
        }

        async Task<string> GenerateOrderNoAsync(Guid groupBuyId)
        {
            var order = await _orderRepository.MaxByOrderNumberAsync();

            long orderNo = 1;
            if (order != null)
            {
                string lastNineDigits = order.OrderNo[^9..];
                _ = long.TryParse(lastNineDigits, out orderNo);
                orderNo++;
            }

            var groupBuy = await _groupBuyRepository.GetAsync(groupBuyId);
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
}
