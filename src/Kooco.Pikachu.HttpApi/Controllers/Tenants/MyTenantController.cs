using Asp.Versioning;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Controllers.Tenants;

[RemoteService(IsEnabled = true)]
[ControllerName("MyTenant")]
[Area("app")]
[Route("api/app/my-tenant")]
public class MyTenantController(
    IMyTenantAppService _myTenantAppService
    ) : AbpController, IMyTenantAppService
{
    [HttpGet("check-short-code/{shortCode}")]
    public Task<bool> CheckShortCodeForCreateAsync(string shortCode)
    {
        return _myTenantAppService.CheckShortCodeForCreateAsync(shortCode);
    }

    [HttpGet("check-short-code/{shortCode}/{id}")]
    public Task<bool> CheckShortCodeForUpdate(string shortCode, Guid Id)
    {
        return _myTenantAppService.CheckShortCodeForUpdate(shortCode, Id);
    }

    [HttpGet("find-by-name/{name}")]
    public Task<TenantDto> FindByNameAsync(string name)
    {
        return _myTenantAppService.FindByNameAsync(name);
    }

    [HttpGet("find-domain/{id}")]
    public Task<string?> FindTenantDomainAsync(Guid? id)
    {
        return _myTenantAppService.FindTenantDomainAsync(id);
    }

    [HttpGet("get-by-shortcode/{shortcode}")]
    public Task<TenantDto> GetTenantAsync(string shortcode)
    {
        return _myTenantAppService.GetTenantAsync(shortcode);
    }
}
