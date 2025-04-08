using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowSettings;

public partial class CashFlowDealerSettings
{
    #region Inject
    private UpdateLinePayDto LinePay { get; set; }

    private bool IsLinePayNotExists = false;

    private bool IsChinaTrustNotExists = false;

    private bool IsEcPayNotExists = false;
    private UpdateOrderValidityDto OrderValidity { get; set; }
    private UpdateChinaTrustDto ChinaTrust { get; set; }
    private UpdateEcPayDto EcPay { get; set; }
    private LoadingIndicator Loading { get; set; }
    #endregion
    #region Constructor
    public CashFlowDealerSettings()
    {
        LinePay = new UpdateLinePayDto();
        ChinaTrust = new UpdateChinaTrustDto();
        EcPay = new UpdateEcPayDto();
        OrderValidity = new UpdateOrderValidityDto();
    }
    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
        try
        {
            List<PaymentGatewayDto> paymentGateways = await _paymentGatewayAppService.GetAllAsync();

            PaymentGatewayDto? linePay = paymentGateways.FirstOrDefault(x => x.PaymentIntegrationType is PaymentIntegrationType.LinePay);

            if (linePay is not null) LinePay = ObjectMapper.Map<PaymentGatewayDto, UpdateLinePayDto>(linePay);

            IsLinePayNotExists = !paymentGateways.Any(a => a.PaymentIntegrationType is PaymentIntegrationType.LinePay);

            PaymentGatewayDto? chinaTrust = paymentGateways.FirstOrDefault(x => x.PaymentIntegrationType is PaymentIntegrationType.ChinaTrust);

            if (chinaTrust is not null) ChinaTrust = ObjectMapper.Map<PaymentGatewayDto, UpdateChinaTrustDto>(chinaTrust);

            IsChinaTrustNotExists = !paymentGateways.Any(a => a.PaymentIntegrationType is PaymentIntegrationType.ChinaTrust);

            PaymentGatewayDto? ecPay = paymentGateways.FirstOrDefault(x => x.PaymentIntegrationType is PaymentIntegrationType.EcPay);

            if (ecPay is not null) EcPay = ObjectMapper.Map<PaymentGatewayDto, UpdateEcPayDto>(ecPay);


            var orderValidity = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
            if (orderValidity != null)
            {
                OrderValidity = ObjectMapper.Map<PaymentGatewayDto, UpdateOrderValidityDto>(orderValidity);
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }

    async Task UpdateEcPayAsync()
    {
        if (EcPay.MerchantId.IsNullOrWhiteSpace()
            || EcPay.HashKey.IsNullOrWhiteSpace()
            || EcPay.HashIV.IsNullOrWhiteSpace()
            || EcPay.TradeDescription.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            if (!EcPay.IsCreditCardEnabled)
            {
                EcPay.InstallmentPeriods.Clear();
            }
            await Loading.Show();
            await _paymentGatewayAppService.UpdateEcPayAsync(EcPay);
            var paymentGateways = await _paymentGatewayAppService.GetAllAsync();
            var ecPay = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).FirstOrDefault();
            if (ecPay != null)
            {
                EcPay = ObjectMapper.Map<PaymentGatewayDto, UpdateEcPayDto>(ecPay);
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
            StateHasChanged();
        }
    }

    async Task UpdateChinaTrustAsync()
    {
        if (ChinaTrust.MerchantId.IsNullOrWhiteSpace()
            || ChinaTrust.Code.IsNullOrWhiteSpace()
            || ChinaTrust.CodeValue.IsNullOrWhiteSpace()
            || ChinaTrust.TerminalCode.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            await Loading.Show();
            await _paymentGatewayAppService.UpdateChinaTrustAsync(ChinaTrust);
            var paymentGateways = await _paymentGatewayAppService.GetAllAsync();
            var chinaTrust = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust).FirstOrDefault();
            if (chinaTrust != null)
            {
                ChinaTrust = ObjectMapper.Map<PaymentGatewayDto, UpdateChinaTrustDto>(chinaTrust);
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
            StateHasChanged();
        }
    }

    async Task UpdateLinePayAsync()
    {
        if (LinePay.ChannelId.IsNullOrWhiteSpace()
            || LinePay.ChannelSecretKey.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            await Loading.Show();
            await _paymentGatewayAppService.UpdateLinePayAsync(LinePay);
            var paymentGateways = await _paymentGatewayAppService.GetAllAsync();
            var linePay = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay).FirstOrDefault();
            if (linePay != null)
            {
                LinePay = ObjectMapper.Map<PaymentGatewayDto, UpdateLinePayDto>(linePay);
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
            StateHasChanged();
        }
    }

    async Task UpdateOrderValidityAsync()
    {
        if (OrderValidity.Unit.IsNullOrWhiteSpace()
           || OrderValidity.Period == null)
        {
            return;
        }

        try
        {
            await Loading.Show();
            await _paymentGatewayAppService.UpdateOrderValidityAsync(OrderValidity);
            var paymentGateways = await _paymentGatewayAppService.GetAllAsync();
            var orderValidity = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
            if (orderValidity != null)
            {
                OrderValidity = ObjectMapper.Map<PaymentGatewayDto, UpdateOrderValidityDto>(orderValidity);
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
            StateHasChanged();
        }
    }

    void OnInstallmentPeriodChange(bool value, string period)
    {
        if (value)
        {
            EcPay.InstallmentPeriods.Add(period);
        }
        else
        {
            EcPay.InstallmentPeriods.Remove(period);
        }
    }
    #endregion
}


