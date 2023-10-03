using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Orders
{

    public class OrderManager : DomainService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderManager(
         IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Order> CreateAsync(
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
             ReceivingTime? receivingTime
             )
        {
            return new Order(
                GuidGenerator.Create(),
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
                receivingTime
                );
        }

        public void AddOrderItem(
            Order order,
            Guid itemId,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value,
            int quantity
            )
        {
            order.AddOrderItem(
                GuidGenerator.Create(),
                itemId,
                attribute1Value,
                attribute2Value,
                attribute3Value,
                quantity
                );
        }
    }
}
