using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants
{
    [RemoteService(IsEnabled = false)]
    public class MyTenantAppService : ApplicationService, IMyTenantAppService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICustomTenantRepository _customTenantRepository;
        private readonly IConfiguration _configuration;

        public MyTenantAppService(ITenantRepository tenantRepository, ICustomTenantRepository customTenantRepository, IConfiguration configuration)
        {
            _tenantRepository = tenantRepository;
            _customTenantRepository = customTenantRepository;
            _configuration = configuration;
        }

        public async Task<bool> CheckShortCodeForCreateAsync(string shortCode)
        {
            return await _customTenantRepository.CheckShortCodeForCreate(shortCode);
        }

        public async Task<bool> CheckShortCodeForUpdate(string shortCode, Guid Id)
        {
            return await _customTenantRepository.CheckShortCodeForUpdate(shortCode, Id);
        }

        public async Task<TenantDto?> FindByNameAsync(string name)
        {
            var tenant = await _tenantRepository.FindByNameAsync(name);
            return ObjectMapper.Map<Tenant?, TenantDto?>(tenant);
        }

        [AllowAnonymous]

        public async Task<TenantDto> GetTenantAsync(string shortCode)
        {
            var tenant = await _customTenantRepository.FindByShortCodeAsync(shortCode);
            return ObjectMapper.Map<Tenant, TenantDto>(tenant);
        }

        [AllowAnonymous]
        public async Task<string?> FindTenantDomainAsync(Guid? id)
        {
            var domain = await _customTenantRepository.FindTenantDomainAsync(id);
            domain ??= _configuration["EntryUrl"];
            return domain;
        }
    }
}
