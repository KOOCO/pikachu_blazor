using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories.Tenants;
public class CustomTenantRepository(IDbContextProvider<TenantManagementDbContext> dbContextProvider) :
    EfCoreRepository<TenantManagementDbContext, Tenant, Guid>(dbContextProvider), ICustomTenantRepository
{
    public async Task<bool> CheckShortCodeForCreate(string shortCode, CancellationToken cancellationToken = default)
    {
        var context = await GetDbContextAsync();
        var tenant = context.Tenants.Any(u => EF.Property<string>(u, "ShortCode") == shortCode);
        return tenant;
    }
    public async Task<bool> CheckShortCodeForUpdate(string shortCode, Guid Id, CancellationToken cancellationToken = default)
    {
        var context = await GetDbContextAsync();
        var tenant = context.Tenants.Any(u => EF.Property<string>(u, "ShortCode") == shortCode && u.Id != Id);
        return tenant;
    }
    public async Task<Tenant> FindByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        var context = await GetDbContextAsync();
        var tenant = context.Tenants.Where(u => EF.Property<string>(u, "ShortCode") == shortCode);
        return await tenant.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
    public async Task<string?> FindTenantDomainAsync(Guid? id)
    {
        var context = await GetDbContextAsync();
        var tenant = await context.Tenants.Where(t => t.Id == id)
                        .FirstOrDefaultAsync();
        if (tenant is null)
        {
            return default;
        }
        return tenant.GetProperty<string?>(Constant.Domain);
    }
    public async Task<string?> FindTenantUrlAsync(Guid? id)
    {
        var context = await GetDbContextAsync();
        var tenant = await context.Tenants.Where(t => t.Id == id)
                        .FirstOrDefaultAsync();
        if (tenant is null)
        {
            return default;
        }
        return tenant.GetProperty<string?>(Constant.TenantUrl);
    }
}