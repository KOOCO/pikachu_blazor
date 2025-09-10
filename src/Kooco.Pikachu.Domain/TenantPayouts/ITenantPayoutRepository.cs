using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutRepository : IRepository<TenantPayoutRecord, Guid>
{
    Task<List<TenantPayoutSummary>> GetTenantSummariesAsync();
}
