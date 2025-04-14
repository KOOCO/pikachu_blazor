using Blazorise;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Responses;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowManagement;
public partial class ElectronicInvoiceSetting
{
    protected Validations CreateValidationsRef;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var tripartiteDto = await TenantTripartiteAppService.FindAsync();

            if (tripartiteDto != null)
            {
                Id = tripartiteDto.Id;
                ResultDto = new()
                {
                    IsEnable = tripartiteDto.IsEnable,
                    StoreCode = tripartiteDto.StoreCode,
                    StatusOnInvoiceIssue = tripartiteDto.StatusOnInvoiceIssue,
                    InvoiceType = tripartiteDto.InvoiceType,
                    HashKey = tripartiteDto.HashKey,
                    HashIV = tripartiteDto.HashIV,
                    DisplayInvoiceName = tripartiteDto.DisplayInvoiceName,
                    DaysAfterShipmentGenerateInvoice = tripartiteDto.DaysAfterShipmentGenerateInvoice,
                };

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(ex.GetType().ToString());
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
            TenantTripartiteDto tripartiteDto;
            var tripartite = await TenantTripartiteAppService.FindAsync();
            if (tripartite is null)
            {
                tripartiteDto = await TenantTripartiteAppService.AddAsync(new()
                {
                    DisplayInvoiceName = ResultDto.DisplayInvoiceName,
                    HashIV = ResultDto.HashIV,
                    HashKey = ResultDto.HashKey,
                    InvoiceType = ResultDto.InvoiceType,
                    IsEnable = ResultDto.IsEnable,
                    StatusOnInvoiceIssue = ResultDto.StatusOnInvoiceIssue,
                    StoreCode = ResultDto.StoreCode,
                    DaysAfterShipmentGenerateInvoice = ResultDto.DaysAfterShipmentGenerateInvoice,
                });
            }
            else
            {
                tripartiteDto = await TenantTripartiteAppService.PutAsync(new()
                {
                    DaysAfterShipmentGenerateInvoice = ResultDto.DaysAfterShipmentGenerateInvoice,
                    DisplayInvoiceName = ResultDto.DisplayInvoiceName,
                    HashIV = ResultDto.HashIV,
                    HashKey = ResultDto.HashKey,
                    InvoiceType = ResultDto.InvoiceType,
                    IsEnable = ResultDto.IsEnable,
                    StatusOnInvoiceIssue = ResultDto.StatusOnInvoiceIssue,
                    StoreCode = ResultDto.StoreCode,
                    Id = tripartite.Id,
                });
            }

            ResultDto = new()
            {
                StoreCode = tripartiteDto.StoreCode,
                StatusOnInvoiceIssue = tripartiteDto.StatusOnInvoiceIssue,
                IsEnable = tripartiteDto.IsEnable,
                InvoiceType = tripartiteDto.InvoiceType,
                HashIV = tripartiteDto.HashIV,
                DisplayInvoiceName = tripartiteDto.DisplayInvoiceName,
                DaysAfterShipmentGenerateInvoice = tripartiteDto.DaysAfterShipmentGenerateInvoice,
                HashKey = tripartiteDto.HashKey,
            };
            await UiMessageService.Success(L["Setting Update Successfully"]);
            StateHasChanged();
        }
    }

    public Guid Id { get; set; }
    public TenantTripartiteResultDto ResultDto { get; set; } = new();
    public required IUiMessageService UiMessageService { get; init; }
    public required ITenantTripartiteAppService TenantTripartiteAppService { get; init; }
}