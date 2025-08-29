using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Kooco.Pikachu.Permissions;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    [Authorize(PikachuPermissions.Feeds.Default)]
    public class TenantDeliveryFeeAppService : PikachuAppService, ITenantDeliveryFeeAppService
    {
        private readonly ITenantDeliveryFeeRepository _repository;
        private readonly TenantDeliveryFeeManager _manager;

        public TenantDeliveryFeeAppService(
            ITenantDeliveryFeeRepository repository,
            TenantDeliveryFeeManager manager)
        {
            _repository = repository;
            _manager = manager;
        }
        [Authorize(PikachuPermissions.Feeds.View)]
        public async Task<PagedResultDto<TenantDeliveryFeeDto>> GetListAsync(TenantDeliveryFeeGetListInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var tenantId = input.TenantId ?? CurrentTenant.Id;
            if (!tenantId.HasValue)
                throw new BusinessException("TenantDeliveryFee:TenantIdMissing");

            // Default sorting matches repo’s dynamic OrderBy(string)
            var sorting = string.IsNullOrWhiteSpace(input.Sorting)
                ? nameof(TenantDeliveryFee.CreationTime) + " DESC"
                : input.Sorting;

            using (CurrentTenant.Change(tenantId))
            {
                var totalCount = await _repository.GetCountAsync(
                    tenantId: tenantId,
                    deliveryProvider: input.DeliveryProvider,
                    isEnabled: input.IsEnabled,
                    feeKind: input.FeeKind
                );

                var entities = await _repository.GetListAsync(
                    tenantId: tenantId,
                    deliveryProvider: input.DeliveryProvider,
                    isEnabled: input.IsEnabled,
                    feeKind: input.FeeKind,
                    sorting: sorting,
                    skipCount: input.SkipCount,
                    maxResultCount: input.MaxResultCount
                );

                var dtos = ObjectMapper.Map<List<TenantDeliveryFee>, List<TenantDeliveryFeeDto>>(entities);
                return new PagedResultDto<TenantDeliveryFeeDto>(totalCount, dtos);
            }
        }

        /// <summary>
        /// Paged tenants LEFT JOIN TenantDeliveryFee with rollups:
        /// - LogisticsFeeStatus = true if any provider row for tenant is enabled
        /// - LastModificationTime = latest of (LastModificationTime ?? CreationTime) across that tenant’s fee rows
        /// Tenants with no fee rows are still included (status=false, lastModification=null).
        /// </summary>
        [Authorize(PikachuPermissions.Feeds.Overview)]
        public async Task<PagedResultDto<TenantLogisticsFeeRowDto>> GetTenantLogisticsFeeOverviewAsync(
            TenantLogisticsFeeGetListInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            // Default sort by tenant name to match repo’s dynamic OrderBy
            var sorting = string.IsNullOrWhiteSpace(input.Sorting)
                ? "Name ASC"
                : input.Sorting;

            var (items, totalCount) = await _repository.GetTenantLogisticsFeeOverviewAsync(
                tenantNameFilter: input.Filter,
                sorting: sorting,
                skipCount: input.SkipCount,
                maxResultCount: input.MaxResultCount
            );

            var dtos = items.Select(x => new TenantLogisticsFeeRowDto
            {
                Id = x.TenantId,
                TenantName = x.TenantName,
                PaymentFeeStatus = x.PaymentFeeStatus,
                LogisticsFeeStatus = x.LogisticsFeeStatus,
                LastModificationTime = x.LastModificationTime
            }).ToList();

            return new PagedResultDto<TenantLogisticsFeeRowDto>(totalCount, dtos);
        }

        [Authorize(PikachuPermissions.Feeds.Manage)]
        public async Task UpsertManyAsync(UpsertTenantDeliveryFeesInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.Items == null) throw new BusinessException("TenantDeliveryFee:ItemsRequired");

            // If TenantId not provided (tenant self-serve), use CurrentTenant.Id
            var tenantId = input.TenantId ?? CurrentTenant.Id;
            if (!tenantId.HasValue)
                throw new BusinessException("TenantDeliveryFee:TenantIdMissing");

            using (CurrentTenant.Change(tenantId))
            {
                // Load existing rows for this tenant once, keyed by DeliveryProvider
                var existing = await _repository.GetListAsync(tenantId: tenantId);
                var byProvider = existing.ToDictionary(x => x.DeliveryProvider);

                // Upsert one-by-one (create or update)
                foreach (var item in input.Items)
                {
                    if (byProvider.TryGetValue(item.DeliveryProvider, out var entity))
                    {
                        // Update
                        await _manager.UpdateAsync(
                            id: entity.Id,
                            isEnabled: item.IsEnabled,
                            feeKind: item.FeeKind,
                            percentValue: item.PercentValue,
                            fixedAmount: item.FixedAmount
                        );
                    }
                    else
                    {
                        // Create
                        var created = await _manager.CreateAsync(
                            tenantId: tenantId,
                            deliveryProvider: item.DeliveryProvider,
                            isEnabled: item.IsEnabled,
                            feeKind: item.FeeKind,
                            percentValue: item.PercentValue,
                            fixedAmount: item.FixedAmount
                        );

                        byProvider[item.DeliveryProvider] = created; // keep dictionary in sync
                    }
                }

                // Optional if your manager uses Insert/Update with autoSave:false:
                // await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
