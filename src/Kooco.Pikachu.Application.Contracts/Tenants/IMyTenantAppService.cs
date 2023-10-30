using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants
{
    public interface IMyTenantAppService : IApplicationService
    {

        Task<TenantDto> GetTenantAsync(string name);
        Task<bool> CheckShortCodeForCreateAsync(string shortCode);
        Task<bool> CheckShortCodeForUpdate(string shortCode,Guid Id);
    }
}
