using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using static Volo.Abp.TenantManagement.TenantManagementPermissions;

namespace Kooco.Pikachu.MultiTenancy;

public class MyDomainTenantResolveContributor : TenantResolveContributorBase
{
    public override string Name => "Domain";
    public string HostDomain { get; set; }

    public override async Task ResolveAsync(ITenantResolveContext context)
    {
        var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        var tenantAppService = context.ServiceProvider.GetRequiredService<IMyTenantAppService>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = context.ServiceProvider.GetRequiredService<ILogger<MyDomainTenantResolveContributor>>();

        logger.LogInformation("Domain Tenant Resolver: Request starting at domain tenant resolver.");

        var hostDomain = configuration["HostDomain:Name"];

        if (httpContext != null)
        {
            var host = httpContext.Request.Host.Host;
            if (!string.IsNullOrWhiteSpace(host))
            {
                logger.LogInformation("Domain Tenant Resolver: The host value is {0}", host);

                var parts = host.Split('.');
                string tenantName = parts.Length > 2 ? parts[1] : parts[0];

                if (tenantName.Equals(hostDomain, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Domain Tenant Resolver: Resolving the tenanacy as host");
                    context.Handled = true;
                }

                if (!context.Handled && !string.IsNullOrWhiteSpace(tenantName))
                {
                    logger.LogInformation("Domain Tenant Resolver: Successfully extracted tenant name from request host as {0}", tenantName);

                    var tenant = await tenantAppService.FindByNameAsync(tenantName);
                    if (tenant != null)
                    {
                        context.TenantIdOrName = tenant.Name;
                        logger.LogInformation("Domain Tenant Resolver: Successfully matched with tenant name as {0}", tenant.Name);
                    }
                    context.Handled = true;
                }
                else
                {
                    logger.LogInformation("Domain Tenant Resolver: Unable to match with host or tenant");
                }

            }
            else
            {
                logger.LogWarning("Domain Tenant Resolver: The request host is null");
            }
        }

        logger.LogWarning("Domain Tenant Resolver: Request finishing at domain tenant resolver.");
        await Task.CompletedTask;
    }
}
