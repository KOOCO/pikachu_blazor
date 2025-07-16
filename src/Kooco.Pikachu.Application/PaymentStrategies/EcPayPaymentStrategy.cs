using ECPay.Payment.Integration;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.PaymentStrategies;
using Kooco.Pikachu.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Application.PaymentStrategies
{
    /// <summary>
    /// Payment strategy for ECPay (Credit Card and Bank Transfer)
    /// Handles ECPay payment gateway integration
    /// </summary>
    public class EcPayPaymentStrategy : BasePaymentStrategy
    {
        private readonly PaymentMethods _paymentMethod;
        private readonly IConfiguration _configuration;
        
        public EcPayPaymentStrategy(PaymentMethods paymentMethod, IConfiguration configuration, ILogger<BasePaymentStrategy> logger) 
            : base(logger)
        {
            _paymentMethod = paymentMethod;
            _configuration = configuration;
        }
        
        public override PaymentMethods PaymentMethod => _paymentMethod;
        
        public override PaymentChannel PaymentChannel => PaymentChannel.EcPay;
        
        protected override Task<bool> ValidatePaymentMethodAsync(Order order)
        {
            // ECPay supports both CreditCard and BankTransfer
            return Task.FromResult(_paymentMethod == PaymentMethods.CreditCard || 
                                 _paymentMethod == PaymentMethods.BankTransfer);
        }
        
        public override async Task<PaymentProcessingResult> ProcessPaymentAsync(Order order, object paymentGateway, string clientBackUrl, bool isInstallments = false)
        {
            try
            {
                if (!await CanProcessPaymentAsync(order))
                {
                    return CreateFailureResult($"Cannot process {_paymentMethod} payment for order {order.Id}");
                }

                // Cast to PaymentGatewayDto - in real implementation this would be properly typed
                var gateway = (PaymentGatewayDto)paymentGateway;
                
                var ecPayPaymentMethod = _paymentMethod switch
                {
                    PaymentMethods.CreditCard => ECPay.Payment.Integration.PaymentMethod.Credit,
                    PaymentMethods.BankTransfer => ECPay.Payment.Integration.PaymentMethod.ATM,
                    _ => ECPay.Payment.Integration.PaymentMethod.Credit // Default to credit card
                };
                
                using var oPayment = new AllInOne();
                
                oPayment.ServiceMethod = HttpMethod.HttpPOST;
                oPayment.ServiceURL = _configuration["EcPay:PaymentApiUrl"];
                oPayment.HashKey = gateway.HashKey;
                oPayment.HashIV = gateway.HashIV;
                oPayment.MerchantID = gateway.MerchantId;
                oPayment.Send.ReturnURL = $"{GetBaseUrl()}/api/app/orders/callback";
                oPayment.Send.ClientBackURL = clientBackUrl;
                oPayment.Send.MerchantTradeNo = order.MerchantTradeNo;
                oPayment.Send.MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                oPayment.Send.TotalAmount = Convert.ToInt32(order.TotalAmount);
                oPayment.Send.TradeDesc = gateway.TradeDescription;
                oPayment.Send.ChoosePayment = ecPayPaymentMethod;
                oPayment.Send.Remark = string.Empty;
                oPayment.Send.ChooseSubPayment = PaymentMethodItem.None;
                oPayment.Send.NeedExtraPaidInfo = ExtraPaymentInfo.Yes;
                oPayment.Send.DeviceSource = DeviceType.PC;
                oPayment.Send.IgnorePayment = string.Empty;
                oPayment.Send.PlatformID = string.Empty;
                oPayment.Send.HoldTradeAMT = HoldTradeType.Yes;
                oPayment.Send.CustomField1 = Guid.NewGuid().ToString();
                oPayment.Send.CustomField2 = order.GroupBuyId.ToString();
                oPayment.Send.CustomField3 = string.Empty;
                oPayment.Send.CustomField4 = string.Empty;
                oPayment.Send.EncryptType = 1;
                
                if (isInstallments && _paymentMethod == PaymentMethods.CreditCard)
                {
                    oPayment.SendExtend.CreditInstallment = string.Join(",", gateway.InstallmentPeriods);
                }
                
                // Add order items
                foreach (var item in order.OrderItems)
                {
                    if (item.Item == null && item.SetItem == null) continue;
                    
                    oPayment.Send.Items.Add(new Item()
                    {
                        Name = (item.Item?.ItemName ?? item.SetItem?.SetItemName) + " NT$ ",
                        Price = decimal.TryParse(item.ItemPrice.ToString("G29"), out decimal price) ? price : 0.00M,
                        Currency = string.Empty,
                        Quantity = item.Quantity,
                        URL = string.Empty,
                    });
                }
                
                var result = oPayment.CheckOut();
                
                if (result.ErrorList.Any())
                {
                    return CreateFailureResult($"ECPay checkout failed: {string.Join(", ", result.ErrorList)}");
                }
                
                var htmlForm = new StringBuilder();
                htmlForm.Append($"<form id='ecpay_form' method='post' action='{_configuration["EcPay:PaymentApiUrl"]}'>");
                
                foreach (DictionaryEntry parameter in result.htParameters)
                {
                    htmlForm.Append($"<input type='hidden' name='{parameter.Key}' value='{parameter.Value}'>");
                }
                
                htmlForm.Append("</form>");
                htmlForm.Append("<script>document.getElementById('ecpay_form').submit();</script>");
                
                return new PaymentProcessingResult
                {
                    IsSuccess = true,
                    TransactionId = result.htParameters["CustomField1"]?.ToString() ?? string.Empty,
                    ResponseData = result,
                    RedirectForm = htmlForm.ToString()
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ECPay payment processing failed for order {OrderId}", order.Id);
                return CreateFailureResult($"ECPay payment processing failed: {ex.Message}");
            }
        }
        
        public override async Task<PaymentConfirmationResult> ConfirmPaymentAsync(Order order, object paymentResult)
        {
            try
            {
                // ECPay confirmation is handled via callback
                // This method would be used for additional validation if needed
                return new PaymentConfirmationResult
                {
                    IsSuccess = true,
                    TransactionId = order.TradeNo ?? string.Empty,
                    ResponseData = paymentResult
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ECPay payment confirmation failed for order {OrderId}", order.Id);
                return CreateConfirmationFailureResult($"ECPay payment confirmation failed: {ex.Message}");
            }
        }
        
        public override async Task<RefundProcessingResult> ProcessRefundAsync(Order order, decimal refundAmount, string reason)
        {
            try
            {
                // ECPay refund processing would be implemented here
                // For now, returning not implemented
                return CreateRefundFailureResult("ECPay refund processing not yet implemented");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ECPay refund processing failed for order {OrderId}", order.Id);
                return CreateRefundFailureResult($"ECPay refund processing failed: {ex.Message}");
            }
        }
        
        private string GetBaseUrl()
        {
            // This should be injected from configuration
            return "https://localhost"; // Placeholder
        }
    }
}