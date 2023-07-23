using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.Items;

namespace Kooco.Pikachu
{
    internal class PikachuDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Item, Guid> _appItemRepository;

        public PikachuDataSeedContributor(IRepository<Item, Guid> appItemRepository)
        {
            _appItemRepository = appItemRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _appItemRepository.GetCountAsync() <= 0)
            {
                await _appItemRepository.InsertAsync(
                    new Item
                    {
                        ItemName = "SunShine Umbrella",
                        ItemDescription = "This is a simple description of demo item",
                        SellingPrice = 10,
                        SKU = "APCJ-Blue-001"

                    },
                autoSave: true);
            }
        }
    }
  
}
