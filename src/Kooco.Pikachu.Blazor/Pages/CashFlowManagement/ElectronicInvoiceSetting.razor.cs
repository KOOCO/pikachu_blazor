using Blazorise;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.ElectronicInvoiceSettings;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowManagement;
public partial class ElectronicInvoiceSetting(ITenantTripartiteAppService appService, IObjectMapper objectMapper, IUiMessageService uiMessageService)
{
    private CreateUpdateElectronicInvoiceDto CreateUpdateElectronicInvoiceDto = new();
    protected Validations CreateValidationsRef;

    public Guid Id { get; set; }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var setting = await appService.GetSettingAsync();

            if (setting != null)
            {
                Id = (Guid)(setting?.Id);
                CreateUpdateElectronicInvoiceDto = setting != null ?
                    objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(setting) :
                    new();

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            await uiMessageService.Error(ex.GetType().ToString());
        }
    }
    protected virtual async Task CreateEntityAsync()
    {
        var validate = true;
        if (CreateValidationsRef != null)
        {
            validate = await CreateValidationsRef.ValidateAll();
        }
        if (validate)
        {
            if (Id == Guid.Empty)
            {
                var result = await appService.CreateAsyc(CreateUpdateElectronicInvoiceDto);
                CreateUpdateElectronicInvoiceDto = objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(result);
                StateHasChanged();

                await uiMessageService.Success(L["Setting Update Successfully"]);
            }
            else
            {

                var result = await appService.UpdateAsyc(Id, CreateUpdateElectronicInvoiceDto);
                CreateUpdateElectronicInvoiceDto = objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(result);
                StateHasChanged();

                await uiMessageService.Success(L["Setting Update Successfully"]);
            }
        }
    }
}