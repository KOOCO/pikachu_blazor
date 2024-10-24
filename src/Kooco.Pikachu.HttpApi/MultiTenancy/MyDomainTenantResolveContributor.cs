using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.MultiTenancy;

public class MyDomainTenantResolveContributor : TenantResolveContributorBase
{
    public override string Name => "Domain";

    public override async Task ResolveAsync(ITenantResolveContext context)
    {
        var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
        var tenantAppService = context.ServiceProvider.GetRequiredService<IMyTenantAppService>();
        var logger = context.ServiceProvider.GetRequiredService<ILogger<MyDomainTenantResolveContributor>>();

        logger.LogInformation("Domain Tenant Resolver: Request starting.");

        if (httpContext == null)
        {
            logger.LogWarning("Domain Tenant Resolver: HTTP context is null.");
            return;
        }

        var host = httpContext.Request.Host.Host;
        if (string.IsNullOrWhiteSpace(host))
        {
            logger.LogWarning("Domain Tenant Resolver: Request host is null or empty.");
            return;
        }

        logger.LogInformation("Domain Tenant Resolver: Host value is {0}", host);

        var parts = CleanUrl(host).Split('.');
        string domain = CleanDomainName(parts.Length > 2 ? parts[1] : parts[0]);
        string subdomain = parts.Length > 2 ? CleanDomainName(parts[0]) : string.Empty;

        var tenant = await ResolveTenantAsync(tenantAppService, domain, subdomain, logger);
        if (tenant != null)
        {
            context.TenantIdOrName = tenant.Name;
            context.Handled = true;
            logger.LogInformation("Domain Tenant Resolver: Tenant resolved successfully as {0}.", tenant.Name);
        }
        else
        {
            logger.LogInformation("Domain Tenant Resolver: No matching tenant found.");
        }
        
        logger.LogInformation("Domain Tenant Resolver: Request finished.");
    }

    private static async Task<TenantDto?> ResolveTenantAsync(IMyTenantAppService tenantAppService, string domain, string subdomain, ILogger logger)
    {
        if (!string.IsNullOrWhiteSpace(domain))
        {
            logger.LogInformation("Domain Tenant Resolver: Resolving tenant by domain name {0}", domain);
            var tenant = await tenantAppService.FindByNameAsync(domain);
            if (tenant != null) return tenant;

            logger.LogInformation("Domain Tenant Resolver: Failed to resolve tenant by domain name {0}", domain);
        }

        if (!string.IsNullOrWhiteSpace(subdomain))
        {
            logger.LogInformation("Domain Tenant Resolver: Resolving tenant by subdomain name {0}", subdomain);
            var tenant = await tenantAppService.FindByNameAsync(subdomain);
            if (tenant != null) return tenant;

            logger.LogInformation("Domain Tenant Resolver: Failed to resolve tenant by subdomain name {0}", subdomain);
        }

        return null;
    }

    public static string CleanDomainName(string domain)
    {
        return domain.Replace("admin-d", "")
                     .Replace("admin", "")
                     .Replace("dev", "")
                     .Replace("-d", "")
                     .Replace("-", "");
    }

    public static string CleanUrl(string host)
    {
        return host.Replace("https://", "")
                   .Replace("http://", "")
                   .Replace("www.", "");
    }
}
