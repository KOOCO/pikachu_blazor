using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;

[RemoteService(IsEnabled = false)]
public class ElectronicInvoiceSettingAppService(ITenantTripartiteRepository repository) : ApplicationService, IElectronicInvoiceSettingAppService
{
    public async Task<ElectronicInvoiceSettingDto> GetSettingAsync()
    {
        var query = await repository.GetQueryableAsync();
        return ObjectMapper.Map<TenantTripartite, ElectronicInvoiceSettingDto>(query.FirstOrDefault());
    }

    public async Task<ElectronicInvoiceSettingDto> CreateAsyc(CreateUpdateElectronicInvoiceDto input)
    {
        var setting = new TenantTripartite(
            Guid.NewGuid(),
            input.IsEnable,
            input.StoreCode,
            input.HashKey,
            input.HashIV,
            input.DisplayInvoiceName,
            input.InvoiceType,
            input.StatusOnInvoiceIssue.Value,
            input.DaysAfterShipmentGenerateInvoice.Value);
        _ = await repository.InsertAsync(setting);
        return ObjectMapper.Map<TenantTripartite, ElectronicInvoiceSettingDto>(setting);
    }

    public async Task<ElectronicInvoiceSettingDto> UpdateAsyc(Guid Id, CreateUpdateElectronicInvoiceDto input)
    {
        var query = await repository.GetQueryableAsync();
        var setting = query.Where(x => x.Id == Id).First();
        setting.IsEnable = input.IsEnable;
        setting.StoreCode = input.StoreCode;
        setting.HashKey = input.HashKey;
        setting.HashIV = input.HashIV;
        setting.InvoiceType = input.InvoiceType;
        setting.DisplayInvoiceName = input.DisplayInvoiceName;
        setting.DaysAfterShipmentGenerateInvoice = input.DaysAfterShipmentGenerateInvoice.Value;
        setting.StatusOnInvoiceIssue = input.StatusOnInvoiceIssue.Value;
        var result = await repository.UpdateAsync(setting);
        return ObjectMapper.Map<TenantTripartite, ElectronicInvoiceSettingDto>(setting);
    }
}