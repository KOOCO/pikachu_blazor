using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement
{
    public interface ICustomTenantRepository : IBasicRepository<Tenant, Guid>
    {
        Task<bool> CheckShortCodeForCreate(string shortCode, CancellationToken cancellationToken = default);
        Task<bool> CheckShortCodeForUpdate(string shortCode,Guid Id, CancellationToken cancellationToken = default);
        Task<Tenant> FindByShortCodeAsync(string shortCode,CancellationToken cancellationToken = default);
    }
}
