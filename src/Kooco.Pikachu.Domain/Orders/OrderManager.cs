using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using System;
using System.Threading.Tasks;
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
             string name,
             string phone,
             string email,
             PaymentMethods? paymentMethods,
             InvoiceType? invoiceType,
             string invoiceNumber,
             string uniformNumber,
             bool IsAsSameBuyer,
             string name2,
             string phone2,
             string email2,
             DeliveryMethod? deliveryMethod,
             string city,
             string district,
             string road,
             string addressDetails,
             string remarks,
             ReceivingTime? receivingTime,
             int totalQuantity,
             decimal totalAmount
             )
        {
            string orderNo = await GenerateOrderNoAsync(groupBuyId);

            return new Order(
                GuidGenerator.Create(),
                groupBuyId,
                orderNo,
                isIndividual,
                name,
                phone,
                email,
                paymentMethods,
                invoiceType,
                invoiceNumber,
                uniformNumber,
                IsAsSameBuyer,
                name2,
                phone2,
                email2,
                deliveryMethod,
                city,
                district,
                road,
                addressDetails,
                remarks,
                receivingTime,
                totalQuantity,
                totalAmount
                );
        }

        public void AddOrderItem(
            Order order,
            Guid itemId,
            string? spec,
            decimal itemPrice,
            decimal totalAmount,
            int quantity
            )
        {
            order.AddOrderItem(
                GuidGenerator.Create(),
                itemId,
                spec,
                itemPrice,
                totalAmount,
                quantity
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
    }
}
