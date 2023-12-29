using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.DeliveryTemperatureCosts
{
    public interface IDeliveryTemperatureCostAppService : IApplicationService
    {

        Task<List<DeliveryTemperatureCostDto>> GetListAsync();
        Task UpdateCostAsync(List<UpdateDeliveryTemperatureCostDto> Costs);
    }
}
