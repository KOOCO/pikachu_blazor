using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants
{
    public class MyTenantAppService : ApplicationService, IMyTenantAppService
    {
        private readonly ITenantRepository _tenantRepository;
        public MyTenantAppService(ITenantRepository tenantRepository) {
        
        _tenantRepository = tenantRepository;
        }
        [AllowAnonymous]
        
        public async Task<TenantDto> GetTenantAsync(string tenantName)
        {
           var tenant= await _tenantRepository.FindByNameAsync(tenantName);
            return ObjectMapper.Map<Tenant, TenantDto>(tenant);
        }
    }
}
