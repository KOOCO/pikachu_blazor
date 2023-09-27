using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public class OrderAppService : CrudAppService<Order, OrderDto, Guid, PagedAndSortedResultRequestDto, CreateOrderDto>,
    IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;

        public OrderAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager
            ) : base(orderRepository)
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            try
            {
                var order = await _orderManager.CreateAsync(
               input.IsIndividual,
               input.Name,
               input.Phone,
               input.Email,
               input.PaymentMethod,
               input.InvoiceType,
               input.InvoiceNumber,
               input.UniformNumber,
               input.IsAsSameBuyer,
               input.Name2,
               input.Email2,
               input.Phone2,
               input.DeliveryMethod,
               input.City,
               input.District,
               input.Road,
               input.AddressDetails,
               input.Remarks,
               input.ReceivingTime
               );
                await _orderRepository.InsertAsync(order);
                return ObjectMapper.Map<Order, OrderDto>(order);
            }
            catch (Exception e)
            {
                throw;
            }

        }

    }
}

