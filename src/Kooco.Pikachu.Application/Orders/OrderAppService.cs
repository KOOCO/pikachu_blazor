using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;

        public OrderAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager
            )
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto input)
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

            if (input.OrderItems != null)
            {
                foreach (var item in input.OrderItems)
                {
                    _orderManager.AddOrderItem(
                        order,
                        item.ItemId,
                        item.Spec,
                        item.ItemPrice,
                        item.TotalAmount,
                        item.Quantity
                        );
                }
            }
            await _orderRepository.InsertAsync(order);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);
            await _orderRepository.DeleteAsync(order);
        }

        public async Task<OrderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
        }

        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountAsync(input.Filter);

            var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }
    }
}

