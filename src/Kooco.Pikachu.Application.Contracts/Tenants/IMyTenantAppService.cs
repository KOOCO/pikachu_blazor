using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants
{
    public interface IMyTenantAppService : IApplicationService
    {
        Task<TenantDto?> FindByNameAsync(string name);
        Task<TenantDto> GetTenantAsync(string name);
        Task<bool> CheckShortCodeForCreateAsync(string shortCode);
        Task<bool> CheckShortCodeForUpdate(string shortCode, Guid Id);
        Task<string?> FindTenantDomainAsync(Guid? id);
    }
}
