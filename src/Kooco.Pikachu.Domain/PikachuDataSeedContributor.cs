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
                await _appItemRepository.InsertAsync(
                    new Item
                    {
                        ItemName = "SunShine Umbrella",
                        ItemDescription = "This is a simple description of demo item",
                        Returnable = false,
                        ItemDetails = new List<ItemDetails>
                        { new ItemDetails
                             {
                                 SellingPrice = 10,
                                 OpeningStockValue = 100,
                                 SKU = "APCJ-Blue-001"
                             }
                        }
                    },
                autoSave: true);
                await _appItemRepository.InsertAsync(
                   new Item
                   {
                       ItemName = "Lovely Pillow",
                       ItemDescription = "This is a simple description of demo item",
                       Returnable = false,
                       ItemDetails = new List<ItemDetails>
                        { new ItemDetails
                             {
                                 SellingPrice = 100,
                                 OpeningStockValue = 2,
                                 SKU = "APCJ-Blue-001"
                             }
                        }
                   },
               autoSave: true);
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
