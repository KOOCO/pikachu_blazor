using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutAppService : IApplicationService
{
    Task<PagedResultDto<TenantPayoutSummaryDto>> GetTenantSummariesAsync(GetTenantSummariesDto input, CancellationToken cancellationToken = default);
    Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<TenantPayoutYearlySummaryDto>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType, CancellationToken cancellationToken = default);
    Task<TenantPayoutDetailSummaryDto> GetTenantPayoutDetailSummaryAsync(GetTenantPayoutRecordListDto input, CancellationToken cancellationToken = default);
    Task<PagedResultDto<TenantPayoutRecordDto>> GetListAsync(GetTenantPayoutRecordListDto input, CancellationToken cancellationToken = default);
    Task MarkAsPaidAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<int> TransferToWalletAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<byte[]> ExportAsync(GetTenantPayoutRecordListDto input, string exportType, List<TenantPayoutRecordDto> selected = null!);
}
