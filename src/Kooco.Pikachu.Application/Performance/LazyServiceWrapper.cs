using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Performance
{
    /// <summary>
    /// Lazy service wrapper for expensive service initialization
    /// Defers service creation until first access to improve performance
    /// </summary>
    public interface ILazyServiceWrapper<T>
    {
        /// <summary>
        /// Get the service instance, creating it if necessary
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Check if the service has been initialized
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Reset the service instance (for testing purposes)
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Implementation of lazy service wrapper
    /// Uses thread-safe lazy initialization
    /// </summary>
    public class LazyServiceWrapper<T> : ILazyServiceWrapper<T>, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LazyServiceWrapper<T>> _logger;
        private readonly Lazy<T> _lazyService;

        public LazyServiceWrapper(
            IServiceProvider serviceProvider,
            ILogger<LazyServiceWrapper<T>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _lazyService = new Lazy<T>(CreateService, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public T Value
        {
            get
            {
                try
                {
                    return _lazyService.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating lazy service of type {ServiceType}", typeof(T).Name);
                    throw;
                }
            }
        }

        public bool IsInitialized => _lazyService.IsValueCreated;

        public void Reset()
        {
            // Note: Lazy<T> doesn't support reset, so this would require recreating the lazy instance
            // This method is mainly for testing scenarios
            _logger.LogWarning("Reset called on LazyServiceWrapper<{ServiceType}> - this operation is not supported", typeof(T).Name);
        }

        private T CreateService()
        {
            _logger.LogDebug("Creating lazy service of type {ServiceType}", typeof(T).Name);
            return _serviceProvider.GetRequiredService<T>();
        }
    }

    /// <summary>
    /// Factory for creating lazy service wrappers
    /// </summary>
    public interface ILazyServiceFactory
    {
        /// <summary>
        /// Create a lazy wrapper for a service
        /// </summary>
        ILazyServiceWrapper<T> CreateLazy<T>();
    }

    /// <summary>
    /// Implementation of lazy service factory
    /// </summary>
    public class LazyServiceFactory : ILazyServiceFactory, ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public LazyServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILazyServiceWrapper<T> CreateLazy<T>()
        {
            return _serviceProvider.GetRequiredService<ILazyServiceWrapper<T>>();
        }
    }

    /// <summary>
    /// Extension methods for lazy service registration
    /// </summary>
    public static class LazyServiceExtensions
    {
        /// <summary>
        /// Register a service with lazy initialization
        /// </summary>
        public static IServiceCollection AddLazyService<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class, TInterface
            where TInterface : class
        {
            services.AddScoped<TInterface, TImplementation>();
            services.AddTransient<ILazyServiceWrapper<TInterface>, LazyServiceWrapper<TInterface>>();
            return services;
        }

        /// <summary>
        /// Register a singleton service with lazy initialization
        /// </summary>
        public static IServiceCollection AddLazySingleton<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class, TInterface
            where TInterface : class
        {
            services.AddSingleton<TInterface, TImplementation>();
            services.AddTransient<ILazyServiceWrapper<TInterface>, LazyServiceWrapper<TInterface>>();
            return services;
        }

        /// <summary>
        /// Register a transient service with lazy initialization
        /// </summary>
        public static IServiceCollection AddLazyTransient<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class, TInterface
            where TInterface : class
        {
            services.AddTransient<TInterface, TImplementation>();
            services.AddTransient<ILazyServiceWrapper<TInterface>, LazyServiceWrapper<TInterface>>();
            return services;
        }
    }
}