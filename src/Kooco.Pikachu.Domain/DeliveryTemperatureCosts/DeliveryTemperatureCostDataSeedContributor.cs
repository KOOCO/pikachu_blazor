using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.DeliveryTempratureCosts;

public class DeliveryTemperatureCostDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    #region Inject
    private readonly IRepository<DeliveryTemperatureCost, Guid> _tempCostRepository;
    private IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    #endregion

    #region Constructor
    public DeliveryTemperatureCostDataSeedContributor(
        IRepository<DeliveryTemperatureCost, Guid> tempCostRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant
    )
    {
        _tempCostRepository = tempCostRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }
    #endregion

    #region Methods
    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            if (await _tempCostRepository.GetCountAsync() > 0) return;

            DeliveryTemperatureCost temp = new ();

            temp.Temperature = ItemStorageTemperature.Normal;
            
            temp.Cost = 0;
            
            await _tempCostRepository.InsertAsync(temp);
            
            temp = new DeliveryTemperatureCost();
            
            temp.Temperature = ItemStorageTemperature.Freeze;
            
            temp.Cost = 0;
            
            await _tempCostRepository.InsertAsync(temp);
            
            temp = new DeliveryTemperatureCost();
            
            temp.Temperature = ItemStorageTemperature.Frozen;
            
            temp.Cost = 0;
            
            await _tempCostRepository.InsertAsync(temp);
        }
    }
    #endregion
}
