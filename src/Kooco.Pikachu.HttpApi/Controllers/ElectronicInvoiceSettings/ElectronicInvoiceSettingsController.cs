using Asp.Versioning;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Requests;
using Kooco.Pikachu.Tenants.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.ElectronicInvoiceSettings;

[RemoteService(IsEnabled = false)]
[ControllerName("ElectronicInvoiceSettings")]
[Area("app")]
[Route("api/app/electronic-invoice-settings")]
public class ElectronicInvoiceSettingsController : AbpController
{
    [HttpPost]
    public Task<TenantTripartiteDto> CreateAsyc(CreateTenantTripartiteDto input)
    {
        return TenantTripartiteAppService.AddAsync(input);
    }

    [HttpGet]
    public Task<TenantTripartiteDto> GetSettingAsync()
    {
        return TenantTripartiteAppService.FindAsync();
    }

    [HttpPut]
    public Task<TenantTripartiteDto> UpdateAsyc(UpdateTenantTripartiteDto input)
    {
        return TenantTripartiteAppService.PutAsync(input);
    }

    public required ITenantTripartiteAppService TenantTripartiteAppService { get; init; }
}