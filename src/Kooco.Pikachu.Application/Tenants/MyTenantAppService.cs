﻿using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Authorization;
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
        public MyTenantAppService(ITenantRepository tenantRepository, ICustomTenantRepository customTenantRepository)
        {
            _tenantRepository = tenantRepository;
            _customTenantRepository = customTenantRepository;
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
    }
}
