using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutAppService : PikachuAppService, ITenantPayoutAppService
{
    private readonly ITenantPayoutRepository _tenantPayoutRepository;

    public TenantPayoutAppService(ITenantPayoutRepository tenantPayoutRepository)
    {
        _tenantPayoutRepository = tenantPayoutRepository;
    }

    public async Task<List<TenantPayoutSummaryDto>> GetTenantSummariesAsync()
    {
        var summaries = await _tenantPayoutRepository.GetTenantSummariesAsync();
        return ObjectMapper.Map<List<TenantPayoutSummary>, List<TenantPayoutSummaryDto>>(summaries);
    }

    public Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId)
    {
        return _tenantPayoutRepository.GetActivePaymentProvidersAsync(tenantId);
    }

    public async Task<List<TenantPayoutYearlySummaryDto>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType)
    {
        var summaries = await _tenantPayoutRepository.GetTenantPayoutYearlySummariesAsync(tenantId, feeType);
        return ObjectMapper.Map<List<TenantPayoutYearlySummary>, List<TenantPayoutYearlySummaryDto>>(summaries);
    }

    public async Task<TenantPayoutDetailSummaryDto> GetTenantPayoutDetailSummaryAsync(GetTenantPayoutRecordListDto input)
    {
        var summary = await _tenantPayoutRepository.GetTenantPayoutDetailSummaryAsync(
            input.TenantId,
            input.FeeType,
            input.StartDate,
            input.EndDate,
            input.PaymentMethod,
            input.Filter
            );
        return ObjectMapper.Map<TenantPayoutDetailSummary, TenantPayoutDetailSummaryDto>(summary);
    }

    public async Task<PagedResultDto<TenantPayoutRecordDto>> GetListAsync(GetTenantPayoutRecordListDto input)
    {
        var result = await _tenantPayoutRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.TenantId,
            input.FeeType,
            input.StartDate,
            input.EndDate,
            input.PaymentMethod,
            input.Filter
            );

        return new PagedResultDto<TenantPayoutRecordDto>
        {
            TotalCount = result.TotalCount,
            Items = ObjectMapper.Map<List<TenantPayoutRecord>, List<TenantPayoutRecordDto>>(result.Items)
        };
    }
}
