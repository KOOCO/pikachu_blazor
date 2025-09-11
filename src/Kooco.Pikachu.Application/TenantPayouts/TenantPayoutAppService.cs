using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
}
