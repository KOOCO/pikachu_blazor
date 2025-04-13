using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories.Tenants;
public class TenantTripartiteRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, TenantTripartite, Guid>(dbContextProvider), ITenantTripartiteRepository
{
    public async Task<TenantTripartite?> FindByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await (await GetQueryableAsync())
            .Where(od => od.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}