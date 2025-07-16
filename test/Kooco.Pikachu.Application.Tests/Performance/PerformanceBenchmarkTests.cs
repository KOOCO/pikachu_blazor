// BenchmarkDotNet package not available
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentStrategies;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kooco.Pikachu.Performance
{
    /// <summary>
    /// Performance benchmark tests for SOLID refactoring
    /// Compares performance before and after service extraction and strategy patterns
    /// </summary>
    public class PerformanceBenchmarkTests : PikachuApplicationTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly IOrderAppService _orderAppService;
        private readonly IPaymentStrategyFactory _paymentStrategyFactory;
        private readonly IShippingStrategyFactory _shippingStrategyFactory;
        private readonly IServiceProvider _serviceProvider;

        public PerformanceBenchmarkTests(ITestOutputHelper output)
        {
            _output = output;
            _orderAppService = GetRequiredService<IOrderAppService>();
            _paymentStrategyFactory = GetRequiredService<IPaymentStrategyFactory>();
            _shippingStrategyFactory = GetRequiredService<IShippingStrategyFactory>();
            _serviceProvider = GetRequiredService<IServiceProvider>();
        }

        #region Strategy Pattern Performance Tests

        [Fact]
        public async Task Benchmark_PaymentStrategyFactory_vs_SwitchStatement()
        {
            const int iterations = 1000;
            var paymentMethods = new[] { PaymentMethods.CreditCard, PaymentMethods.BankTransfer, PaymentMethods.CashOnDelivery };

            // Benchmark Strategy Pattern
            var strategyStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var method in paymentMethods)
                {
                    var strategy = _paymentStrategyFactory.CreateStrategy(method);
                    // Simulate strategy usage
                    if (strategy != null)
                    {
                        var canProcess = strategy.PaymentMethod == method;
                    }
                }
            }
            strategyStopwatch.Stop();

            // Benchmark Traditional Switch Statement
            var switchStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var method in paymentMethods)
                {
                    // Simulate traditional switch-based approach
                    string result = method switch
                    {
                        PaymentMethods.CreditCard => "EcPay",
                        PaymentMethods.BankTransfer => "EcPay",
                        PaymentMethods.CashOnDelivery => "COD",
                        _ => "Unknown"
                    };
                }
            }
            switchStopwatch.Stop();

            var strategyTime = strategyStopwatch.ElapsedMilliseconds;
            var switchTime = switchStopwatch.ElapsedMilliseconds;
            var overhead = ((double)(strategyTime - switchTime) / switchTime) * 100;

            _output.WriteLine($"Payment Strategy Performance Comparison ({iterations * paymentMethods.Length} operations):");
            _output.WriteLine($"Strategy Pattern: {strategyTime}ms");
            _output.WriteLine($"Switch Statement: {switchTime}ms");
            _output.WriteLine($"Overhead: {overhead:F2}%");

            // Strategy pattern should be within reasonable overhead (< 200%)
            Assert.True(strategyTime < switchTime * 3, 
                $"Strategy pattern overhead too high: {overhead:F2}%. Expected < 200%");
        }

        [Fact]
        public async Task Benchmark_ShippingStrategyFactory_vs_SwitchStatement()
        {
            const int iterations = 1000;
            var deliveryMethods = new[] { DeliveryMethod.SevenToEleven1, DeliveryMethod.FamilyMart1, DeliveryMethod.HomeDelivery };

            // Benchmark Strategy Pattern
            var strategyStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var method in deliveryMethods)
                {
                    var strategy = _shippingStrategyFactory.CreateStrategy(method);
                    // Simulate strategy usage
                    if (strategy != null)
                    {
                        var deliveryMethod = strategy.DeliveryMethod;
                        var provider = strategy.LogisticProvider;
                    }
                }
            }
            strategyStopwatch.Stop();

            // Benchmark Traditional Switch Statement
            var switchStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var method in deliveryMethods)
                {
                    // Simulate traditional switch-based approach
                    var (provider, supported) = method switch
                    {
                        DeliveryMethod.SevenToEleven1 => (LogisticProviders.SevenToEleven, true),
                        DeliveryMethod.FamilyMart1 => (LogisticProviders.FamilyMart, true),
                        DeliveryMethod.HomeDelivery => (LogisticProviders.HomeDelivery, true),
                        _ => (LogisticProviders.GreenWorldLogistics, false)
                    };
                }
            }
            switchStopwatch.Stop();

            var strategyTime = strategyStopwatch.ElapsedMilliseconds;
            var switchTime = switchStopwatch.ElapsedMilliseconds;
            var overhead = ((double)(strategyTime - switchTime) / switchTime) * 100;

            _output.WriteLine($"Shipping Strategy Performance Comparison ({iterations * deliveryMethods.Length} operations):");
            _output.WriteLine($"Strategy Pattern: {strategyTime}ms");
            _output.WriteLine($"Switch Statement: {switchTime}ms");
            _output.WriteLine($"Overhead: {overhead:F2}%");

            // Strategy pattern should be within reasonable overhead (< 200%)
            Assert.True(strategyTime < switchTime * 3, 
                $"Strategy pattern overhead too high: {overhead:F2}%. Expected < 200%");
        }

        #endregion

        #region Service Delegation Performance Tests

        [Fact]
        public async Task Benchmark_ServiceDelegation_vs_DirectCalls()
        {
            const int iterations = 100;

            // Benchmark Service Delegation (SOLID approach)
            var delegationStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                // Simulate service resolution through DI
                var paymentService = _serviceProvider.GetService<IOrderPaymentService>();
                var inventoryService = _serviceProvider.GetService<IOrderInventoryService>();
                var notificationService = _serviceProvider.GetService<IOrderNotificationService>();
                
                // Simulate service calls
                if (paymentService != null && inventoryService != null && notificationService != null)
                {
                    // Simulate delegation overhead
                    var services = new object[] { paymentService, inventoryService, notificationService };
                }
            }
            delegationStopwatch.Stop();

            // Benchmark Direct Object Creation (Monolithic approach)
            var directStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                // Simulate direct object creation (what would happen in monolithic approach)
                var objects = new object[] { new object(), new object(), new object() };
            }
            directStopwatch.Stop();

            var delegationTime = delegationStopwatch.ElapsedMilliseconds;
            var directTime = directStopwatch.ElapsedMilliseconds;
            var overhead = delegationTime > 0 && directTime > 0 ? 
                ((double)(delegationTime - directTime) / Math.Max(directTime, 1)) * 100 : 0;

            _output.WriteLine($"Service Delegation Performance Comparison ({iterations} operations):");
            _output.WriteLine($"Service Delegation: {delegationTime}ms");
            _output.WriteLine($"Direct Creation: {directTime}ms");
            _output.WriteLine($"Overhead: {overhead:F2}%");

            // Service delegation should be reasonable (< 500% overhead for small operations)
            Assert.True(delegationTime < Math.Max(directTime * 6, 50), 
                $"Service delegation overhead too high: {overhead:F2}%. Expected reasonable overhead for DI resolution");
        }

        #endregion

        #region Memory Usage Tests

        [Fact]
        public void Benchmark_MemoryUsage_StrategyFactories()
        {
            const int iterations = 1000;
            
            // Measure memory before
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var memoryBefore = GC.GetTotalMemory(false);

            // Create strategies
            var strategies = new List<object>();
            for (int i = 0; i < iterations; i++)
            {
                var paymentStrategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                var shippingStrategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
                
                if (paymentStrategy != null) strategies.Add(paymentStrategy);
                if (shippingStrategy != null) strategies.Add(shippingStrategy);
            }

            // Measure memory after
            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;
            var memoryPerStrategy = strategies.Count > 0 ? memoryUsed / strategies.Count : 0;

            _output.WriteLine($"Memory Usage Analysis ({iterations} iterations):");
            _output.WriteLine($"Total Memory Used: {memoryUsed:N0} bytes");
            _output.WriteLine($"Strategies Created: {strategies.Count}");
            _output.WriteLine($"Memory Per Strategy: {memoryPerStrategy:N0} bytes");

            // Memory usage should be reasonable (< 10KB per strategy)
            Assert.True(memoryPerStrategy < 10240, 
                $"Memory usage per strategy too high: {memoryPerStrategy:N0} bytes. Expected < 10KB");

            // Cleanup
            strategies.Clear();
        }

        #endregion

        #region Comprehensive Performance Test

        [Fact]
        public async Task Comprehensive_PerformanceProfile()
        {
            var results = new Dictionary<string, long>();

            // Test 1: Strategy Factory Creation Performance
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
            }
            stopwatch.Stop();
            results["StrategyCreation_100ops"] = stopwatch.ElapsedMilliseconds;

            // Test 2: Service Resolution Performance
            stopwatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                _serviceProvider.GetService<IOrderAppService>();
                _serviceProvider.GetService<IOrderPaymentService>();
                _serviceProvider.GetService<IOrderInventoryService>();
            }
            stopwatch.Stop();
            results["ServiceResolution_300ops"] = stopwatch.ElapsedMilliseconds;

            // Test 3: Strategy Method Supported Check
            var paymentStrategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
            var shippingStrategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
            
            stopwatch.Restart();
            for (int i = 0; i < 1000; i++)
            {
                if (paymentStrategy != null)
                {
                    var isSupported = _paymentStrategyFactory.IsPaymentMethodSupported(PaymentMethods.CreditCard);
                }
                if (shippingStrategy != null)
                {
                    var deliverySupported = _shippingStrategyFactory.IsDeliveryMethodSupported(DeliveryMethod.HomeDelivery);
                }
            }
            stopwatch.Stop();
            results["SupportedChecks_2000ops"] = stopwatch.ElapsedMilliseconds;

            // Output comprehensive results
            _output.WriteLine("=== COMPREHENSIVE PERFORMANCE PROFILE ===");
            foreach (var result in results)
            {
                _output.WriteLine($"{result.Key}: {result.Value}ms");
            }

            // Performance assertions
            Assert.True(results["StrategyCreation_100ops"] < 100, "Strategy creation should be fast");
            Assert.True(results["ServiceResolution_300ops"] < 50, "Service resolution should be fast");
            Assert.True(results["SupportedChecks_2000ops"] < 10, "Support checks should be very fast");

            _output.WriteLine("✅ All performance benchmarks passed!");
        }

        #endregion
    }

    // BenchmarkDotNet configuration removed - package not available
    // For detailed performance analysis, use the SimplePerformanceTests instead
}