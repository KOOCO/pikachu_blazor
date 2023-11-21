using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.OrderDeliveries
{
    public class OrderDeliveryAppService : ApplicationService, IOrderDeliveryAppService
    {
        private readonly IOrderDeliveryRepository _orderDeliveryRepository;
        public OrderDeliveryAppService(IOrderDeliveryRepository orderDeliveryRepository) {
        
        _orderDeliveryRepository = orderDeliveryRepository;
        
        }
        public async Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id)
        {
            var result = await _orderDeliveryRepository.GetWithDetailsAsync(Id);
            
            return ObjectMapper.Map<List<OrderDelivery>, List<OrderDeliveryDto>>(result.ToList());

        }
    }
}
