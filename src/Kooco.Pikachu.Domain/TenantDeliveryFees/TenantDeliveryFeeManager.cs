using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantDeliveryFeeManager : DomainService
    {
        private readonly ITenantDeliveryFeeRepository _repository;

        public TenantDeliveryFeeManager(ITenantDeliveryFeeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new row. Enforces uniqueness on (TenantId, DeliveryProvider).
        /// </summary>
        public virtual async Task<TenantDeliveryFee> CreateAsync(
            Guid? tenantId,
            DeliveryProvider deliveryProvider,
            bool isEnabled,
            FeeKind feeKind,
            decimal? percentValue = 0,
            decimal? fixedAmount = 0)
        {
            var exists = await _repository.AnyAsync(x =>
                x.TenantId == tenantId &&
                x.DeliveryProvider == deliveryProvider
            );

            if (exists)
            {
                throw new BusinessException(PikachuDomainErrorCodes.DuplicateCombination)
                    .WithData(nameof(tenantId), tenantId)
                    .WithData(nameof(deliveryProvider), deliveryProvider);
            }

            var entity = new TenantDeliveryFee(
                id: GuidGenerator.Create(),
                tenantId: tenantId,
                deliveryProvider: deliveryProvider,
                isEnabled: isEnabled,
                feeKind: feeKind,
                percentValue: percentValue??0,
                fixedAmount: fixedAmount??0
            );

            return await _repository.InsertAsync(entity, autoSave: true);
        }

        /// <summary>
        /// Updates the toggles and fee values (does not change TenantId/DeliveryProvider).
        /// </summary>
        public virtual async Task<TenantDeliveryFee> UpdateAsync(
            Guid id,
            bool isEnabled,
            FeeKind feeKind,
            decimal? percentValue =0,
            decimal? fixedAmount =0)
        {
            var entity = await _repository.GetAsync(id);

            entity.Update(
                isEnabled: isEnabled,
                feeKind: feeKind,
                percentValue: percentValue??0,
                fixedAmount: fixedAmount??0
            );

            return await _repository.UpdateAsync(entity, autoSave: true);
        }
    }
}
