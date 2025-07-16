using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentStrategies;
using Kooco.Pikachu.Performance;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Application.Performance
{
    /// <summary>
    /// Performance monitoring wrapper for strategy factories
    /// Tracks performance impact of SOLID refactoring
    /// </summary>
    public interface IPerformanceMonitoringService
    {
        /// <summary>
        /// Create payment strategy with performance monitoring
        /// </summary>
        IPaymentStrategy? CreatePaymentStrategy(PaymentMethods paymentMethod);

        /// <summary>
        /// Create shipping strategy with performance monitoring
        /// </summary>
        IShippingStrategy? CreateShippingStrategy(DeliveryMethod deliveryMethod);

        /// <summary>
        /// Get performance report for all monitored operations
        /// </summary>
        Task<PerformanceReport> GetPerformanceReportAsync();
    }

    /// <summary>
    /// Performance report containing metrics and analysis
    /// </summary>
    public class PerformanceReport
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan MonitoringPeriod { get; set; }
        public int TotalOperations { get; set; }
        public PaymentStrategyMetrics PaymentStrategies { get; set; } = new();
        public ShippingStrategyMetrics ShippingStrategies { get; set; } = new();
        public ServiceDelegationMetrics ServiceDelegation { get; set; } = new();
        public PerformanceAnalysis Analysis { get; set; } = new();
    }

    public class PaymentStrategyMetrics
    {
        public int TotalCreations { get; set; }
        public double AverageCreationTimeMs { get; set; }
        public string FastestStrategy { get; set; } = string.Empty;
        public string SlowestStrategy { get; set; } = string.Empty;
        public long TotalMemoryUsage { get; set; }
    }

    public class ShippingStrategyMetrics
    {
        public int TotalCreations { get; set; }
        public double AverageCreationTimeMs { get; set; }
        public string FastestStrategy { get; set; } = string.Empty;
        public string SlowestStrategy { get; set; } = string.Empty;
        public long TotalMemoryUsage { get; set; }
    }

    public class ServiceDelegationMetrics
    {
        public int TotalServiceCalls { get; set; }
        public double AverageServiceCallTimeMs { get; set; }
        public double DelegationOverheadPercentage { get; set; }
        public string MostUsedService { get; set; } = string.Empty;
    }

    public class PerformanceAnalysis
    {
        public string OverallAssessment { get; set; } = string.Empty;
        public bool IsPerformanceAcceptable { get; set; }
        public string[] Recommendations { get; set; } = Array.Empty<string>();
        public string[] PerformanceBottlenecks { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Implementation of performance monitoring service
    /// </summary>
    public class PerformanceMonitoringService : IPerformanceMonitoringService, ITransientDependency
    {
        private readonly IPaymentStrategyFactory _paymentStrategyFactory;
        private readonly IShippingStrategyFactory _shippingStrategyFactory;
        private readonly IPerformanceMetrics _performanceMetrics;
        private readonly ILogger<PerformanceMonitoringService> _logger;
        private readonly DateTime _startTime;

        public PerformanceMonitoringService(
            IPaymentStrategyFactory paymentStrategyFactory,
            IShippingStrategyFactory shippingStrategyFactory,
            IPerformanceMetrics performanceMetrics,
            ILogger<PerformanceMonitoringService> logger)
        {
            _paymentStrategyFactory = paymentStrategyFactory;
            _shippingStrategyFactory = shippingStrategyFactory;
            _performanceMetrics = performanceMetrics;
            _logger = logger;
            _startTime = DateTime.UtcNow;
        }

        public IPaymentStrategy? CreatePaymentStrategy(PaymentMethods paymentMethod)
        {
            return _performanceMetrics.MonitorStrategyCreation(
                $"Payment.{paymentMethod}",
                () => _paymentStrategyFactory.CreateStrategy(paymentMethod));
        }

        public IShippingStrategy? CreateShippingStrategy(DeliveryMethod deliveryMethod)
        {
            return _performanceMetrics.MonitorStrategyCreation(
                $"Shipping.{deliveryMethod}",
                () => _shippingStrategyFactory.CreateStrategy(deliveryMethod));
        }

        public async Task<PerformanceReport> GetPerformanceReportAsync()
        {
            var allStats = _performanceMetrics.GetAllStatistics();
            var monitoringPeriod = DateTime.UtcNow - _startTime;

            var paymentStats = FilterStatsByPrefix(allStats, "Strategy.Payment.");
            var shippingStats = FilterStatsByPrefix(allStats, "Strategy.Shipping.");
            var serviceStats = FilterStatsByPrefix(allStats, "Service.");

            var report = new PerformanceReport
            {
                GeneratedAt = DateTime.UtcNow,
                MonitoringPeriod = monitoringPeriod,
                TotalOperations = allStats.Values.Sum(s => s.ExecutionCount),
                PaymentStrategies = AnalyzePaymentStrategies(paymentStats),
                ShippingStrategies = AnalyzeShippingStrategies(shippingStats),
                ServiceDelegation = AnalyzeServiceDelegation(serviceStats),
                Analysis = GenerateAnalysis(allStats)
            };

            _logger.LogInformation("Generated performance report: {TotalOperations} operations over {Period}", 
                report.TotalOperations, monitoringPeriod);

            return report;
        }

        private Dictionary<string, PerformanceStatistics> FilterStatsByPrefix(
            Dictionary<string, PerformanceStatistics> allStats, 
            string prefix)
        {
            return allStats.Where(kvp => kvp.Key.StartsWith(prefix))
                          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private PaymentStrategyMetrics AnalyzePaymentStrategies(Dictionary<string, PerformanceStatistics> stats)
        {
            if (!stats.Any()) return new PaymentStrategyMetrics();

            var totalCreations = stats.Values.Sum(s => s.ExecutionCount);
            var avgTimeMs = stats.Values.Average(s => s.AverageExecutionTime.TotalMilliseconds);
            var fastest = stats.OrderBy(s => s.Value.AverageExecutionTime).First();
            var slowest = stats.OrderByDescending(s => s.Value.AverageExecutionTime).First();

            return new PaymentStrategyMetrics
            {
                TotalCreations = totalCreations,
                AverageCreationTimeMs = avgTimeMs,
                FastestStrategy = fastest.Key,
                SlowestStrategy = slowest.Key,
                TotalMemoryUsage = stats.Values.Sum(s => s.TotalMemoryUsage)
            };
        }

        private ShippingStrategyMetrics AnalyzeShippingStrategies(Dictionary<string, PerformanceStatistics> stats)
        {
            if (!stats.Any()) return new ShippingStrategyMetrics();

            var totalCreations = stats.Values.Sum(s => s.ExecutionCount);
            var avgTimeMs = stats.Values.Average(s => s.AverageExecutionTime.TotalMilliseconds);
            var fastest = stats.OrderBy(s => s.Value.AverageExecutionTime).First();
            var slowest = stats.OrderByDescending(s => s.Value.AverageExecutionTime).First();

            return new ShippingStrategyMetrics
            {
                TotalCreations = totalCreations,
                AverageCreationTimeMs = avgTimeMs,
                FastestStrategy = fastest.Key,
                SlowestStrategy = slowest.Key,
                TotalMemoryUsage = stats.Values.Sum(s => s.TotalMemoryUsage)
            };
        }

        private ServiceDelegationMetrics AnalyzeServiceDelegation(Dictionary<string, PerformanceStatistics> stats)
        {
            if (!stats.Any()) return new ServiceDelegationMetrics();

            var totalCalls = stats.Values.Sum(s => s.ExecutionCount);
            var avgTimeMs = stats.Values.Average(s => s.AverageExecutionTime.TotalMilliseconds);
            var mostUsed = stats.OrderByDescending(s => s.Value.ExecutionCount).First();

            // Calculate delegation overhead (compared to baseline)
            var baselineTimeMs = 0.1; // Assume 0.1ms baseline for direct method calls
            var overheadPercentage = (avgTimeMs - baselineTimeMs) / baselineTimeMs * 100;

            return new ServiceDelegationMetrics
            {
                TotalServiceCalls = totalCalls,
                AverageServiceCallTimeMs = avgTimeMs,
                DelegationOverheadPercentage = Math.Max(0, overheadPercentage),
                MostUsedService = mostUsed.Key
            };
        }

        private PerformanceAnalysis GenerateAnalysis(Dictionary<string, PerformanceStatistics> allStats)
        {
            var analysis = new PerformanceAnalysis();
            var recommendations = new List<string>();
            var bottlenecks = new List<string>();

            // Analyze overall performance
            var totalOperations = allStats.Values.Sum(s => s.ExecutionCount);
            var avgOperationTime = allStats.Values.Average(s => s.AverageExecutionTime.TotalMilliseconds);

            // Performance thresholds
            const double acceptableAvgTimeMs = 10.0; // 10ms average is acceptable
            const double warningAvgTimeMs = 50.0;   // 50ms average is concerning

            analysis.IsPerformanceAcceptable = avgOperationTime < acceptableAvgTimeMs;

            if (avgOperationTime > warningAvgTimeMs)
            {
                analysis.OverallAssessment = "Performance needs attention - average operation time is high";
                bottlenecks.Add($"High average operation time: {avgOperationTime:F2}ms");
            }
            else if (avgOperationTime > acceptableAvgTimeMs)
            {
                analysis.OverallAssessment = "Performance is acceptable but could be improved";
                recommendations.Add("Consider optimizing slower operations");
            }
            else
            {
                analysis.OverallAssessment = "Performance is excellent - SOLID refactoring has minimal impact";
            }

            // Analyze specific bottlenecks
            var slowOperations = allStats.Where(s => s.Value.AverageExecutionTime.TotalMilliseconds > acceptableAvgTimeMs);
            foreach (var slowOp in slowOperations)
            {
                bottlenecks.Add($"{slowOp.Key}: {slowOp.Value.FormattedAverageTime}");
            }

            // Generate recommendations
            if (totalOperations > 1000)
            {
                recommendations.Add("Consider caching frequently used strategies");
            }

            var highMemoryOps = allStats.Where(s => s.Value.AverageMemoryUsage > 1024 * 1024); // 1MB
            if (highMemoryOps.Any())
            {
                recommendations.Add("Review memory usage for high-consumption operations");
            }

            analysis.Recommendations = recommendations.ToArray();
            analysis.PerformanceBottlenecks = bottlenecks.ToArray();

            return analysis;
        }
    }
}