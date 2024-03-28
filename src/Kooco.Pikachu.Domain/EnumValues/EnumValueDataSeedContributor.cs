using Azure.Storage.Blobs.Models;
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

namespace Kooco.Pikachu.EnumValues
{
    public class EnumValueDataSeedContributor: IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<EnumValue, int> _enumRepository;
       
        private readonly ICurrentTenant _currentTenant;

        public EnumValueDataSeedContributor(
            IRepository<EnumValue, int> enumRepository,
            IGuidGenerator guidGenerator,
            ICurrentTenant currentTenant)
        {
            _enumRepository = enumRepository;
           
            _currentTenant = currentTenant;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            using (_currentTenant.Change(context?.TenantId))
            {
                if (await _enumRepository.GetCountAsync() > 0)
                {
                    return;
                }
                var enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "UPS";

                await _enumRepository.InsertAsync(enumValue);
                 enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "FedEX";

                await _enumRepository.InsertAsync(enumValue);
                 enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "DHL";

                await _enumRepository.InsertAsync(enumValue);
                await _enumRepository.InsertAsync(enumValue);
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "Post Office";

                await _enumRepository.InsertAsync(enumValue);
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "Store Delivery";
                await _enumRepository.InsertAsync(enumValue);

              
                 enumValue = new EnumValue();
                enumValue.EnumType = EnumType.TaxType;
                enumValue.Text = "Taxable";

                await _enumRepository.InsertAsync(enumValue);
                

                await _enumRepository.InsertAsync(enumValue);
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.TaxType;
                enumValue.Text = "Non Taxable";

                await _enumRepository.InsertAsync(enumValue);
             
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.Unit;
                enumValue.Text = "box";

                await _enumRepository.InsertAsync(enumValue);
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "cm";
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.Unit;
                enumValue.Text = "kg";

                await _enumRepository.InsertAsync(enumValue);
                enumValue = new EnumValue();
                enumValue.EnumType = EnumType.ShippingMethod;
                enumValue.Text = "pcs";

                await _enumRepository.InsertAsync(enumValue);
            }
        }
    }
}

