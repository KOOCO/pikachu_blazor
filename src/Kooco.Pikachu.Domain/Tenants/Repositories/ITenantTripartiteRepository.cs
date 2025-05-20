using Kooco.Pikachu.Tenants.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Tenants.Repositories;
public interface ITenantTripartiteRepository : IRepository<TenantTripartite, Guid>
{
    /// <summary>
    /// 依租戶ID查找租戶三方資料
    /// </summary>
    Task<TenantTripartite?> FindByTenantAsync(Guid? tenantId, CancellationToken ct = default);

    /// <summary>
    /// 新增或更新租戶三方資料
    /// </summary>
    Task<TenantTripartite> UpsertAsync(TenantTripartite tenantTripartite, CancellationToken ct = default);
}