using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentStrategies;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kooco.Pikachu.Performance
{
    /// <summary>
    /// Simple performance tests for SOLID refactoring impact
    /// </summary>
    public class SimplePerformanceTests : PikachuApplicationTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly IPaymentStrategyFactory _paymentStrategyFactory;
        private readonly IShippingStrategyFactory _shippingStrategyFactory;
        private readonly IOrderAppService _orderAppService;
        private readonly IServiceProvider _serviceProvider;

        public SimplePerformanceTests(ITestOutputHelper output)
        {
            _output = output;
            _paymentStrategyFactory = GetRequiredService<IPaymentStrategyFactory>();
            _shippingStrategyFactory = GetRequiredService<IShippingStrategyFactory>();
            _orderAppService = GetRequiredService<IOrderAppService>();
            _serviceProvider = GetRequiredService<IServiceProvider>();
        }

        [Fact]
        public void Test_PaymentStrategyFactory_Performance()
        {
            // Test strategy creation performance
            const int iterations = 100;
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var creditStrategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                var bankStrategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.BankTransfer);
                var codStrategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CashOnDelivery);
            }

            stopwatch.Stop();
            var avgTimeMs = stopwatch.ElapsedMilliseconds / (double)(iterations * 3);

            _output.WriteLine($"PaymentStrategyFactory Performance:");
            _output.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Average time per strategy: {avgTimeMs:F2}ms");
            _output.WriteLine($"Strategies per second: {1000 / avgTimeMs:F0}");

            // Performance should be reasonable (< 10ms per strategy)
            Assert.True(avgTimeMs < 10, $"Strategy creation too slow: {avgTimeMs:F2}ms per strategy");
        }

        [Fact]
        public void Test_ShippingStrategyFactory_Performance()
        {
            // Test strategy creation performance
            const int iterations = 100;
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var sevenElevenStrategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.SevenToEleven1);
                var homeDeliveryStrategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
                var familyMartStrategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.FamilyMart1);
            }

            stopwatch.Stop();
            var avgTimeMs = stopwatch.ElapsedMilliseconds / (double)(iterations * 3);

            _output.WriteLine($"ShippingStrategyFactory Performance:");
            _output.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Average time per strategy: {avgTimeMs:F2}ms");
            _output.WriteLine($"Strategies per second: {1000 / avgTimeMs:F0}");

            // Performance should be reasonable (< 10ms per strategy)
            Assert.True(avgTimeMs < 10, $"Strategy creation too slow: {avgTimeMs:F2}ms per strategy");
        }

        [Fact]
        public void Test_ServiceResolution_Performance()
        {
            // Test service resolution performance
            const int iterations = 100;
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var orderService = _serviceProvider.GetService<IOrderAppService>();
                var paymentService = _serviceProvider.GetService<IOrderPaymentService>();
                var inventoryService = _serviceProvider.GetService<IOrderInventoryService>();
                var notificationService = _serviceProvider.GetService<IOrderNotificationService>();
            }

            stopwatch.Stop();
            var avgTimeMs = stopwatch.ElapsedMilliseconds / (double)(iterations * 4);

            _output.WriteLine($"Service Resolution Performance:");
            _output.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Average time per service: {avgTimeMs:F2}ms");
            _output.WriteLine($"Services per second: {1000 / avgTimeMs:F0}");

            // Service resolution should be reasonable (< 5ms per service after optimizations)
            Assert.True(avgTimeMs < 5, $"Service resolution too slow: {avgTimeMs:F2}ms per service");
        }

        [Fact]
        public void Test_StrategyPattern_vs_DirectEnum_Performance()
        {
            const int iterations = 1000;

            // Test Strategy Pattern Performance
            var strategyStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var strategy = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                if (strategy != null)
                {
                    var paymentMethod = strategy.PaymentMethod;
                    var canProcess = paymentMethod == PaymentMethods.CreditCard;
                }
            }
            strategyStopwatch.Stop();

            // Test Direct Enum Performance
            var enumStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var paymentMethod = PaymentMethods.CreditCard;
                var result = paymentMethod switch
                {
                    PaymentMethods.CreditCard => "EcPay",
                    PaymentMethods.BankTransfer => "EcPay",
                    PaymentMethods.CashOnDelivery => "COD",
                    _ => "Unknown"
                };
            }
            enumStopwatch.Stop();

            var strategyTimeMs = strategyStopwatch.ElapsedMilliseconds;
            var enumTimeMs = enumStopwatch.ElapsedMilliseconds;
            var overhead = enumTimeMs > 0 ? ((double)(strategyTimeMs - enumTimeMs) / enumTimeMs) * 100 : 0;

            _output.WriteLine($"Strategy Pattern vs Direct Enum Performance ({iterations} iterations):");
            _output.WriteLine($"Strategy Pattern: {strategyTimeMs}ms");
            _output.WriteLine($"Direct Enum: {enumTimeMs}ms");
            _output.WriteLine($"Overhead: {overhead:F1}%");

            // Strategy pattern should not be more than 5x slower than direct enum (after optimizations)
            Assert.True(strategyTimeMs < Math.Max(enumTimeMs * 5, 5), 
                $"Strategy pattern too slow compared to direct enum: {overhead:F1}% overhead");
        }

        [Fact]
        public void Test_Memory_Usage_StrategyCreation()
        {
            // Force garbage collection to get accurate baseline
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            var memoryBefore = GC.GetTotalMemory(false);

            // Create multiple strategies
            const int iterations = 100;
            var strategies = new object[iterations * 2];
            
            for (int i = 0; i < iterations; i++)
            {
                strategies[i * 2] = _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                strategies[i * 2 + 1] = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
            }

            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;
            var memoryPerStrategy = memoryUsed / (iterations * 2);

            _output.WriteLine($"Memory Usage Analysis:");
            _output.WriteLine($"Total memory used: {memoryUsed:N0} bytes");
            _output.WriteLine($"Strategies created: {iterations * 2}");
            _output.WriteLine($"Memory per strategy: {memoryPerStrategy:N0} bytes");
            _output.WriteLine($"Memory per strategy: {memoryPerStrategy / 1024.0:F1} KB");

            // Memory usage should be reasonable (< 50KB per strategy)
            Assert.True(memoryPerStrategy < 50 * 1024, 
                $"Memory usage too high: {memoryPerStrategy:N0} bytes per strategy");

            // Keep references to prevent optimization
            Assert.NotNull(strategies[0]);
        }

        [Fact]
        public async Task Test_Overall_Performance_Profile()
        {
            var results = new System.Collections.Generic.Dictionary<string, (long TimeMs, string Description)>();

            // Test 1: Strategy Factory Performance
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 50; i++)
            {
                _paymentStrategyFactory.CreateStrategy(PaymentMethods.CreditCard);
                _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);
            }
            sw.Stop();
            results.Add("StrategyCreation", (sw.ElapsedMilliseconds, "100 strategy creations"));

            // Test 2: Service Resolution Performance
            sw.Restart();
            for (int i = 0; i < 50; i++)
            {
                _serviceProvider.GetService<IOrderAppService>();
                _serviceProvider.GetService<IOrderPaymentService>();
            }
            sw.Stop();
            results.Add("ServiceResolution", (sw.ElapsedMilliseconds, "100 service resolutions"));

            // Test 3: Strategy Support Checks
            sw.Restart();
            for (int i = 0; i < 500; i++)
            {
                _paymentStrategyFactory.IsPaymentMethodSupported(PaymentMethods.CreditCard);
                _shippingStrategyFactory.IsDeliveryMethodSupported(DeliveryMethod.HomeDelivery);
            }
            sw.Stop();
            results.Add("SupportChecks", (sw.ElapsedMilliseconds, "1000 support checks"));

            // Display results
            _output.WriteLine("=== PERFORMANCE PROFILE SUMMARY ===");
            var totalTime = 0L;
            foreach (var result in results)
            {
                _output.WriteLine($"{result.Key}: {result.Value.TimeMs}ms ({result.Value.Description})");
                totalTime += result.Value.TimeMs;
            }
            _output.WriteLine($"Total Time: {totalTime}ms");

            // Overall performance should be acceptable (adjusted thresholds after optimization)
            Assert.True(totalTime < 2000, $"Overall performance too slow: {totalTime}ms");
            Assert.True(results["StrategyCreation"].TimeMs < 1000, "Strategy creation too slow");
            Assert.True(results["ServiceResolution"].TimeMs < 500, "Service resolution too slow");
            Assert.True(results["SupportChecks"].TimeMs < 100, "Support checks too slow");

            _output.WriteLine("✅ All performance tests passed!");
        }
    }
}