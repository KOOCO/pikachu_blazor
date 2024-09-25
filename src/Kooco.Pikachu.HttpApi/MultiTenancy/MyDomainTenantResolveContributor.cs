using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

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

        var hostDomain = configuration["HostDomain:Name"];

        if (httpContext != null)
        {
            var host = httpContext.Request.Host.Host;
            if (!string.IsNullOrWhiteSpace(host))
            {
                var parts = host.Split('.');
                string tenantName = parts.Length > 2 ? parts[1] : parts[0];

                if (tenantName.Equals(hostDomain, StringComparison.OrdinalIgnoreCase))
                {
                    context.Handled = true;
                }

                if (!string.IsNullOrWhiteSpace(tenantName))
                {
                    var tenant = await tenantAppService.FindByNameAsync(tenantName);
                    if (tenant != null)
                    {
                        context.TenantIdOrName = tenant.Name;
                    }
                    context.Handled = true;
                }
            }
        }

        await Task.CompletedTask;
    }
}
