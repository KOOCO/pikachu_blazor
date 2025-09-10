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
}
