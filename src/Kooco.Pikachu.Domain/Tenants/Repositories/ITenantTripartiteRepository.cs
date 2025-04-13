using Kooco.Pikachu.Tenants.Entities;
using System.Threading.Tasks;
using System.Threading;
using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Tenants.Repositories;
public interface ITenantTripartiteRepository : IRepository<TenantTripartite, Guid>
{
    /// <summary>
    /// 依租戶ID查找租戶三方資料
    /// </summary>
    Task<TenantTripartite?> FindByTenantAsync(Guid tenantId, CancellationToken ct = default);
}