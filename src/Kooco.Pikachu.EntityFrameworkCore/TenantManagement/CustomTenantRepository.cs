using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;

namespace Kooco.Pikachu.TenantManagement
{
    public class CustomTenantRepository : EfCoreRepository<TenantManagementDbContext, Tenant, Guid>, ICustomTenantRepository
    {
        public CustomTenantRepository(IDbContextProvider<TenantManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

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
    }
}
