using Kooco.Pikachu.DeliveryTempratureCosts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.DeliveryTemperatureCosts;

[RemoteService(IsEnabled = false)]
public class DeliveryTemperatureCostAppService : ApplicationService, IDeliveryTemperatureCostAppService
{
    #region Inject
    private readonly IRepository<DeliveryTemperatureCost, Guid> _repository;
    #endregion

    #region Constructor
    public DeliveryTemperatureCostAppService(IRepository<DeliveryTemperatureCost, Guid> repository)
    {
        _repository = repository;
    }
    #endregion

    #region Methods
    public async Task<List<DeliveryTemperatureCostDto>> GetListAsync()
    {
        List<DeliveryTemperatureCost> list = await _repository.GetListAsync();

        return ObjectMapper.Map<List<DeliveryTemperatureCost>, List<DeliveryTemperatureCostDto>>(list);
    }

    public async Task UpdateCostAsync(List<UpdateDeliveryTemperatureCostDto> Costs)
    {
        foreach (UpdateDeliveryTemperatureCostDto item in Costs)
        {
            Guid id = item.Id;

            DeliveryTemperatureCost deliveryTemperature = await _repository.GetAsync(id);

            deliveryTemperature.LogisticProvider = item.LogisticProvider;

            deliveryTemperature.DeliveryMethod = item.DeliveryMethod;

            deliveryTemperature.Cost = item.Cost;

            await _repository.UpdateAsync(deliveryTemperature);
        }
    }
    #endregion
}
