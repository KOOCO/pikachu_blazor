using Kooco.Pikachu.DeliveryTempratureCosts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.DeliveryTemperatureCosts
{
    public class DeliveryTemperatureCostAppService : ApplicationService, IDeliveryTemperatureCostAppService
    {
        private readonly IRepository<DeliveryTemperatureCost,Guid> _repository;
        public DeliveryTemperatureCostAppService(IRepository<DeliveryTemperatureCost, Guid> repository)
        { 
        _repository = repository;
        
        }
        public async Task<List<DeliveryTemperatureCostDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
         
            return ObjectMapper.Map<List<DeliveryTemperatureCost>,List<DeliveryTemperatureCostDto>>(list);
        }

        public async Task UpdateCostAsync(List<UpdateDeliveryTemperatureCostDto> Costs)
        {
            foreach (var item in Costs)
            {
                var id=item.Id;
                var deliveryTemperature = await _repository.GetAsync(id);
                deliveryTemperature.Cost = item.Cost;
                await _repository.UpdateAsync(deliveryTemperature);

           
            }
        }
    }
}
