using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowSettings
{
    public partial class CashFlowDealerSettings
    {
        private UpdateLinePayDto LinePay { get; set; }
        private UpdateChinaTrustDto ChinaTrust { get; set; }
        private UpdateEcPayDto EcPay { get; set; }
        private LoadingIndicator Loading { get; set; }

        public CashFlowDealerSettings()
        {
            LinePay = new UpdateLinePayDto();
            ChinaTrust = new UpdateChinaTrustDto();
            EcPay = new UpdateEcPayDto();
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var paymentGateways = await _paymentGatewayAppService.GetAllAsync();

                var linePay = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay).FirstOrDefault();
                if (linePay != null)
                {
                    LinePay = ObjectMapper.Map<PaymentGatewayDto, UpdateLinePayDto>(linePay);
                }

                var chinaTrust = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust).FirstOrDefault();
                if (chinaTrust != null)
                {
                    ChinaTrust = ObjectMapper.Map<PaymentGatewayDto, UpdateChinaTrustDto>(chinaTrust);
                }

                var ecPay = paymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).FirstOrDefault();
                if (ecPay != null)
                {
                    EcPay = ObjectMapper.Map<PaymentGatewayDto, UpdateEcPayDto>(ecPay);
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
    }
}
