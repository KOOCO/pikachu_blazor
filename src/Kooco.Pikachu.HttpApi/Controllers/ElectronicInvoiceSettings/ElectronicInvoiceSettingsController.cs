using Asp.Versioning;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.ElectronicInvoiceSettings;

[RemoteService(IsEnabled = false)]
[ControllerName("ElectronicInvoiceSettings")]
[Area("app")]
[Route("api/app/electronic-invoice-settings")]
public class ElectronicInvoiceSettingsController(
    IElectronicInvoiceSettingAppService _electronicInvoiceSettingAppService
    ) : AbpController, IElectronicInvoiceSettingAppService
{
    [HttpPost]
    public Task<ElectronicInvoiceSettingDto> CreateAsyc(CreateUpdateElectronicInvoiceDto input)
    {
        return _electronicInvoiceSettingAppService.CreateAsyc(input);
    }

    [HttpGet]
    public Task<ElectronicInvoiceSettingDto> GetSettingAsync()
    {
        return _electronicInvoiceSettingAppService.GetSettingAsync();
    }

    [HttpPut]
    public Task<ElectronicInvoiceSettingDto> UpdateAsyc(Guid Id, CreateUpdateElectronicInvoiceDto input)
    {
        return _electronicInvoiceSettingAppService.UpdateAsyc(Id, input);
    }
}
