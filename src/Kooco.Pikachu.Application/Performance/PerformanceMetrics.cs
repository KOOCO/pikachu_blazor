using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Performance
{
    /// <summary>
    /// Performance metrics collection and monitoring for SOLID refactoring impact
    /// </summary>
    public interface IPerformanceMetrics
    {
        /// <summary>
        /// Record execution time for an operation
        /// </summary>
        void RecordExecutionTime(string operationName, TimeSpan duration);

        /// <summary>
        /// Record memory usage for an operation
        /// </summary>
        void RecordMemoryUsage(string operationName, long memoryBytes);

        /// <summary>
        /// Get performance statistics for an operation
        /// </summary>
        PerformanceStatistics GetStatistics(string operationName);

        /// <summary>
        /// Get all performance statistics
        /// </summary>
        Dictionary<string, PerformanceStatistics> GetAllStatistics();

        /// <summary>
        /// Execute an operation and automatically record its performance
        /// </summary>
        Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation);

        /// <summary>
        /// Execute an operation and automatically record its performance
        /// </summary>
        T Measure<T>(string operationName, Func<T> operation);
    }

    /// <summary>
    /// Performance statistics for a specific operation
    /// </summary>
    public class PerformanceStatistics
    {
        public string OperationName { get; set; } = string.Empty;
        public int ExecutionCount { get; set; }
        public TimeSpan TotalExecutionTime { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public TimeSpan MinExecutionTime { get; set; }
        public TimeSpan MaxExecutionTime { get; set; }
        public long TotalMemoryUsage { get; set; }
        public long AverageMemoryUsage { get; set; }
        public DateTime FirstRecorded { get; set; }
        public DateTime LastRecorded { get; set; }

        public double ExecutionsPerSecond => ExecutionCount / TotalExecutionTime.TotalSeconds;
        public string FormattedAverageTime => $"{AverageExecutionTime.TotalMilliseconds:F2}ms";
        public string FormattedAverageMemory => FormatBytes(AverageMemoryUsage);

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }
    }

    /// <summary>
    /// Implementation of performance metrics collection
    /// </summary>
    public class PerformanceMetrics : IPerformanceMetrics, ISingletonDependency
    {
        private readonly ILogger<PerformanceMetrics> _logger;
        private readonly ConcurrentDictionary<string, ConcurrentBag<TimeSpan>> _executionTimes;
        private readonly ConcurrentDictionary<string, ConcurrentBag<long>> _memoryUsages;
        private readonly ConcurrentDictionary<string, DateTime> _firstRecorded;
        private readonly ConcurrentDictionary<string, DateTime> _lastRecorded;

        public PerformanceMetrics(ILogger<PerformanceMetrics> logger)
        {
            _logger = logger;
            _executionTimes = new ConcurrentDictionary<string, ConcurrentBag<TimeSpan>>();
            _memoryUsages = new ConcurrentDictionary<string, ConcurrentBag<long>>();
            _firstRecorded = new ConcurrentDictionary<string, DateTime>();
            _lastRecorded = new ConcurrentDictionary<string, DateTime>();
        }

        public void RecordExecutionTime(string operationName, TimeSpan duration)
        {
            _executionTimes.AddOrUpdate(operationName, 
                new ConcurrentBag<TimeSpan> { duration }, 
                (key, bag) => { bag.Add(duration); return bag; });

            var now = DateTime.UtcNow;
            _firstRecorded.TryAdd(operationName, now);
            _lastRecorded.AddOrUpdate(operationName, now, (key, existing) => now);

            _logger.LogDebug("Recorded execution time for {Operation}: {Duration}ms", 
                operationName, duration.TotalMilliseconds);
        }

        public void RecordMemoryUsage(string operationName, long memoryBytes)
        {
            _memoryUsages.AddOrUpdate(operationName, 
                new ConcurrentBag<long> { memoryBytes }, 
                (key, bag) => { bag.Add(memoryBytes); return bag; });

            _logger.LogDebug("Recorded memory usage for {Operation}: {Memory} bytes", 
                operationName, memoryBytes);
        }

        public PerformanceStatistics GetStatistics(string operationName)
        {
            var executionTimes = _executionTimes.GetValueOrDefault(operationName) ?? new ConcurrentBag<TimeSpan>();
            var memoryUsages = _memoryUsages.GetValueOrDefault(operationName) ?? new ConcurrentBag<long>();

            var times = executionTimes.ToArray();
            var memories = memoryUsages.ToArray();

            return new PerformanceStatistics
            {
                OperationName = operationName,
                ExecutionCount = times.Length,
                TotalExecutionTime = times.Aggregate(TimeSpan.Zero, (total, time) => total + time),
                AverageExecutionTime = times.Length > 0 ? TimeSpan.FromTicks(times.Sum(t => t.Ticks) / times.Length) : TimeSpan.Zero,
                MinExecutionTime = times.Length > 0 ? times.Min() : TimeSpan.Zero,
                MaxExecutionTime = times.Length > 0 ? times.Max() : TimeSpan.Zero,
                TotalMemoryUsage = memories.Sum(),
                AverageMemoryUsage = memories.Length > 0 ? memories.Sum() / memories.Length : 0,
                FirstRecorded = _firstRecorded.GetValueOrDefault(operationName),
                LastRecorded = _lastRecorded.GetValueOrDefault(operationName)
            };
        }

        public Dictionary<string, PerformanceStatistics> GetAllStatistics()
        {
            var allOperations = _executionTimes.Keys.Union(_memoryUsages.Keys).Distinct();
            return allOperations.ToDictionary(op => op, op => GetStatistics(op));
        }

        public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var memoryBefore = GC.GetTotalMemory(false);
            
            try
            {
                var result = await operation();
                return result;
            }
            finally
            {
                stopwatch.Stop();
                var memoryAfter = GC.GetTotalMemory(false);
                var memoryUsed = memoryAfter - memoryBefore;

                RecordExecutionTime(operationName, stopwatch.Elapsed);
                if (memoryUsed > 0)
                {
                    RecordMemoryUsage(operationName, memoryUsed);
                }
            }
        }

        public T Measure<T>(string operationName, Func<T> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var memoryBefore = GC.GetTotalMemory(false);
            
            try
            {
                var result = operation();
                return result;
            }
            finally
            {
                stopwatch.Stop();
                var memoryAfter = GC.GetTotalMemory(false);
                var memoryUsed = memoryAfter - memoryBefore;

                RecordExecutionTime(operationName, stopwatch.Elapsed);
                if (memoryUsed > 0)
                {
                    RecordMemoryUsage(operationName, memoryUsed);
                }
            }
        }
    }

    /// <summary>
    /// Extension methods for easy performance monitoring
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Monitor performance of strategy factory operations
        /// </summary>
        public static T MonitorStrategyCreation<T>(this IPerformanceMetrics metrics, string strategyType, Func<T> createStrategy)
        {
            return metrics.Measure($"Strategy.{strategyType}.Creation", createStrategy);
        }

        /// <summary>
        /// Monitor performance of service delegation
        /// </summary>
        public static async Task<T> MonitorServiceDelegation<T>(this IPerformanceMetrics metrics, string serviceName, string methodName, Func<Task<T>> serviceCall)
        {
            return await metrics.MeasureAsync($"Service.{serviceName}.{methodName}", serviceCall);
        }

        /// <summary>
        /// Monitor performance of service delegation (synchronous)
        /// </summary>
        public static T MonitorServiceDelegation<T>(this IPerformanceMetrics metrics, string serviceName, string methodName, Func<T> serviceCall)
        {
            return metrics.Measure($"Service.{serviceName}.{methodName}", serviceCall);
        }
    }
}