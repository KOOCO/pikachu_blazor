using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants;

[RemoteService(IsEnabled = false)]
public class MyTenantAppService(ITenantRepository tenantRepository, ICustomTenantRepository customTenantRepository, IConfiguration configuration) : ApplicationService, IMyTenantAppService
{
    public async Task<bool> CheckShortCodeForCreateAsync(string shortCode)
    {
        return await customTenantRepository.CheckShortCodeForCreate(shortCode);
    }

    public async Task<bool> CheckShortCodeForUpdate(string shortCode, Guid Id)
    {
        return await customTenantRepository.CheckShortCodeForUpdate(shortCode, Id);
    }

    public async Task<TenantDto?> FindByNameAsync(string name)
    {
        var tenant = await tenantRepository.FindByNameAsync(name);
        return ObjectMapper.Map<Tenant?, TenantDto?>(tenant);
    }

    [AllowAnonymous]

    public async Task<TenantDto> GetTenantAsync(string shortCode)
    {
        var tenant = await customTenantRepository.FindByShortCodeAsync(shortCode);
        return ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }

    [AllowAnonymous]
    public async Task<string?> FindTenantDomainAsync(Guid? id)
    {
        var domain = await customTenantRepository.FindTenantDomainAsync(id);
        domain ??= configuration["EntryUrl"];
        return domain;
    }

    [AllowAnonymous]
    public async Task<string?> FindTenantUrlAsync(Guid? id)
    {
        var tenantUrl = await customTenantRepository.FindTenantUrlAsync(id);
        return tenantUrl;
    }
}
