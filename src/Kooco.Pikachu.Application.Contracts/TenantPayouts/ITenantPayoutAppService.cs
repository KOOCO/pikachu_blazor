using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutAppService : IApplicationService
{
    Task<List<TenantPayoutSummaryDto>> GetTenantSummariesAsync();
    Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId);
    Task<List<TenantPayoutYearlySummaryDto>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType);
    Task<TenantPayoutDetailSummaryDto> GetTenantPayoutDetailSummaryAsync(Guid tenantId, PaymentFeeType feeType, int year);
    Task<PagedResultDto<TenantPayoutRecordDto>> GetListAsync(GetTenantPayoutRecordListDto input);
}
