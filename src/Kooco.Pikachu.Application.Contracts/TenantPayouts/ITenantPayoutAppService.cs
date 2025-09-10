using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutAppService : IApplicationService
{
    Task<List<TenantPayoutSummaryDto>> GetTenantSummariesAsync();
}
