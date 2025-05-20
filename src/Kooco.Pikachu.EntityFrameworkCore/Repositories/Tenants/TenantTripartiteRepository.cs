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
    public async Task<TenantTripartite?> FindByTenantAsync(Guid? tenantId, CancellationToken ct = default)
    {
        return await (await GetQueryableAsync())
            .Where(od => od.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    /// <summary>
    /// 新增或更新租戶三方資料
    /// </summary>
    /// <param name="tenantTripartite">租戶三方資料</param>
    /// <param name="ct">取消權杖</param>
    /// <returns>已新增或更新的租戶三方資料</returns>
    public async Task<TenantTripartite> UpsertAsync(TenantTripartite tenantTripartite, CancellationToken ct = default)
    {
        if (tenantTripartite.TenantId is null)
        {
            throw new ArgumentException("租戶ID不能為空", nameof(tenantTripartite));
        }

        var existing = await FindByTenantAsync(tenantTripartite.TenantId.Value, ct);

        if (existing == null)
        {
            // 不存在，新增記錄
            return await InsertAsync(tenantTripartite, true, ct);
        }
        else
        {
            // 存在，更新記錄的屬性
            // 複製需要更新的屬性
            var dbContext = await GetDbContextAsync();

            // 這裡需要根據實際的TenantTripartite類別屬性進行複製
            // 以下僅為示例，請根據實際情況調整需要更新的欄位
            dbContext.Entry(existing).CurrentValues.SetValues(tenantTripartite);

            await dbContext.SaveChangesAsync(ct);
            return existing;
        }
    }
}
