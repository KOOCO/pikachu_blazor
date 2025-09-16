using Kooco.Pikachu.Common;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutRepository : IRepository<TenantPayoutRecord, Guid>
{
    Task<PagedResultModel<TenantPayoutSummary>> GetTenantSummariesAsync(
        int skipCount,
        int maxResultCount,
        string? sorting,
        string? filter,
        CancellationToken cancellationToken = default
        );
    Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<TenantPayoutYearlySummary>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType, CancellationToken cancellationToken = default);
    Task<TenantPayoutDetailSummary> GetTenantPayoutDetailSummaryAsync(
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        bool? isPaid = null,
        CancellationToken cancellationToken = default
        );
    Task<PagedResultModel<TenantPayoutRecord>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        bool? isPaid = null,
        CancellationToken cancellationToken = default
        );
    Task<IQueryable<TenantPayoutRecord>> GetFilteredQueryableAsync(
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        bool? isPaid = null
        );
    Task MarkAsPaidAsync(List<Guid> ids, CancellationToken cancellationToken = default);
}
