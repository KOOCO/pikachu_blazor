using Blazorise;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;

using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowSettings;

public partial class CashFlowDealerSettings
{
    #region Inject
    private UpdateLinePayDto LinePay { get; set; }
    private Validations? validations;
    private Validations? orderValidations;
    private Validations? validationsChinaTrust;
    private Validations? validationsEcPay;
    private Validations? validationsManual;
    private bool IsLinePayNotExists = false;

    private bool IsChinaTrustNotExists = false;

    private bool IsEcPayNotExists = false;

    private bool IsManualBankTransferNotExist = false;
 
    private UpdateOrderValidityDto OrderValidity { get; set; }
    private UpdateChinaTrustDto ChinaTrust { get; set; }
    private UpdateEcPayDto EcPay { get; set; }
    private UpdateManualBankTransferDto ManualBankTransfer { get; set; }
    private LoadingIndicator Loading { get; set; }
    private record FieldMeta(string Id, string Label, Func<string> Get, Action<string> Set);


    private List<FieldMeta> textFields=new();
    #endregion
    #region Constructor
    public CashFlowDealerSettings()
    {
        LinePay = new UpdateLinePayDto();
        ChinaTrust = new UpdateChinaTrustDto();
        EcPay = new UpdateEcPayDto();
        OrderValidity = new UpdateOrderValidityDto();
        ManualBankTransfer = new UpdateManualBankTransferDto();
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

            var manualBankTransfer = paymentGateways.FirstOrDefault(x => x.PaymentIntegrationType is PaymentIntegrationType.ManualBankTransfer);

            ManualBankTransfer = ObjectMapper.Map<PaymentGatewayDto, UpdateManualBankTransferDto>(manualBankTransfer) ?? new();

            IsManualBankTransferNotExist = manualBankTransfer == null;

            var orderValidity = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();
            if (orderValidity != null)
            {
                OrderValidity = ObjectMapper.Map<PaymentGatewayDto, UpdateOrderValidityDto>(orderValidity);
            }
            textFields = new()
        {
            new("mbt-account-name",   L["AccountName"],       () => ManualBankTransfer.AccountName,   v => ManualBankTransfer.AccountName = v),
            new("mbt-bank-name",      L["BankName"],          () => ManualBankTransfer.BankName,      v => ManualBankTransfer.BankName = v),
            new("mbt-branch-name",    L["BranchName"],        () => ManualBankTransfer.BranchName,    v => ManualBankTransfer.BranchName = v),
            new("mbt-bank-code",      L["BankCode"],          () => ManualBankTransfer.BankCode,      v => ManualBankTransfer.BankCode = v),
            new("mbt-account-number", L["BankAccountNumber"], () => ManualBankTransfer.BankAccountNumber, v => ManualBankTransfer.BankAccountNumber = v),
        };
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
    // ----- Submit handlers -----
    private async Task OnSubmitChinaTrustAsync()
    {
        if (validationsChinaTrust is not null && await validationsChinaTrust.ValidateAll())
            await UpdateChinaTrustAsync();
    }

    private async Task OnSubmitEcPayAsync()
    {
        if (validationsEcPay is not null && await validationsEcPay.ValidateAll())
            await UpdateEcPayAsync();
    }

    private async Task OnSubmitManualAsync()
    {
        if (validationsManual is not null && await validationsManual.ValidateAll())
            await UpdateManualBankTransferAsync();
    }


    private async Task OnSubmitAsync()
    {
        if (validations is not null && await validations.ValidateAll())
        {
            await UpdateLinePayAsync();
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
    private async Task OnOrderSubmitAsync()
    {
        if (validations is not null && await orderValidations.ValidateAll())
        {
            await UpdateOrderValidityAsync();
        }
    }

    // Period must be > 0
    public static void ValidatePositiveInt(ValidatorEventArgs e)
    {
        var value = (int)e.Value;

        e.Status = value <= 0 ? ValidationStatus.Error : ValidationStatus.Success;
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

    async Task UpdateManualBankTransferAsync()
    {
        try
        {
            await Loading.Show();
            await _paymentGatewayAppService.UpdateManualBankTransferAsync(ManualBankTransfer);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            await Loading.Hide();
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


