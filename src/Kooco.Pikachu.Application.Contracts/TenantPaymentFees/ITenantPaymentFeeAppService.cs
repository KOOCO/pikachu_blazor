using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantPaymentFees;

public interface ITenantPaymentFeeAppService : IApplicationService
{
    Task UpdateEcPayPaymentAsync(Guid tenantId, List<UpdateTenantPaymentFeeDto> input);
    Task UpdateTCatPaymentAsync(Guid tenantId, List<UpdateTenantPaymentFeeDto> input);
    Task<List<TenantPaymentFeeDto>> GetEcPayPaymentAsync(Guid tenantId);
    Task<List<TenantPaymentFeeDto>> GetTCatPaymentAsync(Guid tenantId);
}
