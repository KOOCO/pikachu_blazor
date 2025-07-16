using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.PaymentGateways.LinePay;
using Kooco.Pikachu.PaymentStrategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Application.PaymentStrategies
{
    /// <summary>
    /// Payment strategy for LinePay
    /// Handles LinePay payment gateway integration
    /// </summary>
    public class LinePayPaymentStrategy : BasePaymentStrategy
    {
        private readonly ILinePayAppService _linePayAppService;
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IOptions<LinePayConfiguration> _linePayConfiguration;
        
        public LinePayPaymentStrategy(
            ILinePayAppService linePayAppService,
            IPaymentGatewayAppService paymentGatewayAppService,
            IOptions<LinePayConfiguration> linePayConfiguration,
            ILogger<BasePaymentStrategy> logger) 
            : base(logger)
        {
            _linePayAppService = linePayAppService;
            _paymentGatewayAppService = paymentGatewayAppService;
            _linePayConfiguration = linePayConfiguration;
        }
        
        public override PaymentMethods PaymentMethod => PaymentMethods.LinePay;
        
        public override PaymentChannel PaymentChannel => PaymentChannel.LinePay;
        
        protected override Task<bool> ValidatePaymentMethodAsync(Order order)
        {
            // LinePay specific validation
            return Task.FromResult(order.TotalAmount > 0);
        }
        
        public override async Task<PaymentProcessingResult> ProcessPaymentAsync(Order order, object paymentGateway, string clientBackUrl, bool isInstallments = false)
        {
            try
            {
                if (!await CanProcessPaymentAsync(order))
                {
                    return CreateFailureResult($"Cannot process LinePay payment for order {order.Id}");
                }
                
                var redirectUrlDto = new LinePayPaymentRequestRedirectUrlDto
                {
                    ConfirmUrl = $"{GetBaseUrl()}/api/app/orders/linepay/confirm",
                    CancelUrl = clientBackUrl
                };
                
                var response = await _linePayAppService.PaymentRequest(order.Id, redirectUrlDto);
                
                if (response.ReturnCode == _linePayConfiguration.Value.SuccessReturnCode)
                {
                    return new PaymentProcessingResult
                    {
                        IsSuccess = true,
                        TransactionId = response.Info?.TransactionId.ToString() ?? string.Empty,
                        ResponseData = response,
                        RedirectUrl = response.Info?.PaymentUrl?.Web
                    };
                }
                else
                {
                    return CreateFailureResult($"LinePay payment request failed: {response.ReturnMessage}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LinePay payment processing failed for order {OrderId}", order.Id);
                return CreateFailureResult($"LinePay payment processing failed: {ex.Message}");
            }
        }
        
        public override async Task<PaymentConfirmationResult> ConfirmPaymentAsync(Order order, object paymentResult)
        {
            try
            {
                if (paymentResult is not LinePayConfirmationRequest confirmationRequest)
                {
                    return CreateConfirmationFailureResult("Invalid LinePay confirmation request");
                }
                
                var response = await _linePayAppService.ConfirmPayment(confirmationRequest.TransactionId, order.OrderNo);
                
                if (response.ReturnCode == _linePayConfiguration.Value.SuccessReturnCode)
                {
                    return new PaymentConfirmationResult
                    {
                        IsSuccess = true,
                        TransactionId = response.Info?.TransactionId.ToString() ?? string.Empty,
                        ResponseData = response
                    };
                }
                else
                {
                    return CreateConfirmationFailureResult($"LinePay confirmation failed: {response.ReturnMessage}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LinePay payment confirmation failed for order {OrderId}", order.Id);
                return CreateConfirmationFailureResult($"LinePay payment confirmation failed: {ex.Message}");
            }
        }
        
        public override async Task<RefundProcessingResult> ProcessRefundAsync(Order order, decimal refundAmount, string reason)
        {
            try
            {
                // LinePay refund processing would need a refund ID
                // This would typically be called from a refund service
                return CreateRefundFailureResult("LinePay refund processing requires refund ID - use LinePay refund service directly");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LinePay refund processing failed for order {OrderId}", order.Id);
                return CreateRefundFailureResult($"LinePay refund processing failed: {ex.Message}");
            }
        }
        
        private string GetBaseUrl()
        {
            // This should be injected from configuration
            return "https://localhost"; // Placeholder
        }
    }
    
    /// <summary>
    /// LinePay confirmation request model
    /// </summary>
    public class LinePayConfirmationRequest
    {
        public string TransactionId { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
    }
}