using Kooco.Pikachu.Application.PaymentStrategies;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.PaymentGateways.LinePay;
using Kooco.Pikachu.PaymentStrategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Application.PaymentStrategies
{
    /// <summary>
    /// Factory for creating payment strategies
    /// Follows Factory Pattern to centralize strategy creation and eliminate switch statements
    /// </summary>
    public class PaymentStrategyFactory : IPaymentStrategyFactory, ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentStrategyFactory> _logger;
        private readonly IConfiguration _configuration;
        
        // Cache of supported payment methods
        private static readonly HashSet<PaymentMethods> SupportedPaymentMethods = new()
        {
            PaymentMethods.CreditCard,
            PaymentMethods.BankTransfer,
            PaymentMethods.LinePay,
            PaymentMethods.CashOnDelivery
        };
        
        public PaymentStrategyFactory(
            IServiceProvider serviceProvider,
            ILogger<PaymentStrategyFactory> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }
        
        /// <summary>
        /// Creates a payment strategy for the specified payment method
        /// </summary>
        public IPaymentStrategy? CreateStrategy(PaymentMethods paymentMethod)
        {
            try
            {
                return paymentMethod switch
                {
                    PaymentMethods.CreditCard => CreateEcPayStrategy(PaymentMethods.CreditCard),
                    PaymentMethods.BankTransfer => CreateEcPayStrategy(PaymentMethods.BankTransfer),
                    PaymentMethods.LinePay => CreateLinePayStrategy(),
                    PaymentMethods.CashOnDelivery => CreateCashOnDeliveryStrategy(),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment strategy for method {PaymentMethod}", paymentMethod);
                return null;
            }
        }
        
        /// <summary>
        /// Gets all supported payment methods
        /// </summary>
        public IEnumerable<PaymentMethods> GetSupportedPaymentMethods()
        {
            return SupportedPaymentMethods.AsEnumerable();
        }
        
        /// <summary>
        /// Checks if a payment method is supported
        /// </summary>
        public bool IsPaymentMethodSupported(PaymentMethods paymentMethod)
        {
            return SupportedPaymentMethods.Contains(paymentMethod);
        }
        
        /// <summary>
        /// Creates ECPay strategy for credit card or bank transfer
        /// </summary>
        private IPaymentStrategy CreateEcPayStrategy(PaymentMethods paymentMethod)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BasePaymentStrategy>>();
            return new EcPayPaymentStrategy(paymentMethod, _configuration, logger);
        }
        
        /// <summary>
        /// Creates LinePay strategy
        /// </summary>
        private IPaymentStrategy CreateLinePayStrategy()
        {
            var linePayAppService = _serviceProvider.GetRequiredService<ILinePayAppService>();
            var paymentGatewayAppService = _serviceProvider.GetRequiredService<IPaymentGatewayAppService>();
            var linePayConfiguration = _serviceProvider.GetRequiredService<IOptions<LinePayConfiguration>>();
            var logger = _serviceProvider.GetRequiredService<ILogger<BasePaymentStrategy>>();
            
            return new LinePayPaymentStrategy(
                linePayAppService,
                paymentGatewayAppService,
                linePayConfiguration,
                logger);
        }
        
        /// <summary>
        /// Creates Cash on Delivery strategy
        /// </summary>
        private IPaymentStrategy CreateCashOnDeliveryStrategy()
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BasePaymentStrategy>>();
            return new CashOnDeliveryPaymentStrategy(logger);
        }
    }
}