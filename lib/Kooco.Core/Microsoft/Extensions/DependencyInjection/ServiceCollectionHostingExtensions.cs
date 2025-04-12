using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionHostingExtensions
{
    public static IServiceCollection AddAllHostedServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length is 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        var baseType = typeof(HostedServiceBase<>);
        foreach (var assembly in assemblies)
        {
            var hostedServiceTypes = assembly.GetTypes().Where(type =>
                !type.IsAbstract &&
                type.BaseType != null &&
                type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == baseType);

            foreach (var serviceType in hostedServiceTypes)
            {
                var hostedServiceInterface = typeof(IHostedService);
                if (hostedServiceInterface.IsAssignableFrom(serviceType))
                {
                    services.AddSingleton(hostedServiceInterface, serviceType);
                }
            }
        }

        return services;
    }
}