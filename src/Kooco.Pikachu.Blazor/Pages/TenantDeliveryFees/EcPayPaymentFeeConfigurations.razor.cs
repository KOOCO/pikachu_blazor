using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees;

public partial class EcPayPaymentFeeConfigurations
{
    [Parameter] public Guid TenantId { get; set; }
    private bool Loading { get; set; } = true;
    private bool Saving { get; set; }
    private List<UpdateTenantPaymentFeeDto> Fees { get; set; } = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) await Get();
    }

    async Task Get()
    {
        try
        {
            Loading = true;
            var fees = await TenantPaymentFeeAppService.GetEcPayPaymentAsync(TenantId);
            Fees = ObjectMapper.Map<List<TenantPaymentFeeDto>, List<UpdateTenantPaymentFeeDto>>(fees) ?? [];
            Loading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Loading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task Update()
    {
        try
        {
            Saving = true;
            await TenantPaymentFeeAppService.UpdateEcPayPaymentAsync(TenantId, Fees);
            Saving = false;
            await Get();
        }
        catch (Exception ex)
        {
            Saving = false;
            await HandleErrorAsync(ex);
        }
    }
}