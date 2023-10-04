using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu
{
    internal class PikachuDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Item, Guid> _appItemRepository;
        private readonly IRepository<EnumValue, int> _appEnumValueRepository;

        public PikachuDataSeedContributor(IRepository<Item, Guid> appItemRepository,
                                          IRepository<EnumValue, int> appEnumValueRepository)
        {
            _appItemRepository = appItemRepository;
            _appEnumValueRepository = appEnumValueRepository;
        }

        public async Task SeedAsync(DataSeedContext context)

        {
            if (await _appItemRepository.GetCountAsync() <= 0)
            {
                var item = new Item()
                {
                    ItemName = "SunShine Umbrella",
                    ItemDescription = "This is a simple description of demo item",
                    Returnable = false,
                    ItemTags = "",
                    ItemDetails = new List<ItemDetails>()
                };
                item.AddItemDetail(
                    Guid.NewGuid(),
                    item.ItemName,
                    "APCJ-Blue-001",
                    0,
                    10,
                    0,
                    0,
                    0,
                    0,
                    "",
                    "",
                    "",
                    ""
                    );

                await _appItemRepository.InsertAsync(item);

                item = new Item()
                {
                    ItemName = "SunShine Computer",
                    ItemDescription = "This is a simple description of demo item",
                    Returnable = false,
                    ItemTags = "",
                    ItemDetails = new List<ItemDetails>()
                };
                item.AddItemDetail(
                    Guid.NewGuid(),
                    item.ItemName,
                    "APCJ-Blue-002",
                    0,
                    10,
                    0,
                    0,
                    0,
                    0,
                    "",
                    "",
                    "",
                    ""
                    );
                await _appItemRepository.InsertAsync(item);

                item = new Item()
                {
                    ItemName = "Lovely Pillow",
                    ItemDescription = "This is a simple description of demo item",
                    Returnable = false,
                    ItemTags = "",
                    ItemDetails = new List<ItemDetails>()
                };
                item.AddItemDetail(
                    Guid.NewGuid(),
                    item.ItemName,
                    "APCJ-Blue-003",
                    0,
                    100,
                    0,
                    0,
                    0,
                    0,
                    "",
                    "",
                    "",
                    ""
                    );
                await _appItemRepository.InsertAsync(item);
            }


            if (await _appEnumValueRepository.GetCountAsync() <= 0)
            {
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.ShippingMethod,
                        Text = "UPS",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.ShippingMethod,
                        Text = "FedEX",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.ShippingMethod,
                        Text = "DHL",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.TaxType,
                        Text = "Taxable",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.TaxType,
                        Text = "Special Taxable",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.TaxType,
                        Text = "Non Taxable",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.TaxType,
                        Text = "Non Taxable",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.Unit,
                        Text = "box",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.Unit,
                        Text = "cm",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.Unit,
                        Text = "kg",
                    },
                autoSave: true);
                await _appEnumValueRepository.InsertAsync(
                    new EnumValue
                    {
                        EnumType = EnumType.Unit,
                        Text = "pcs",
                    },
                autoSave: true);
            }
        }
    }

}
