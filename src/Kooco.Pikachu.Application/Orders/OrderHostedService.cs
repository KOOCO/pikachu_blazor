using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Orders;
internal sealed class OrderHostedService : HostedServiceBase<OrderHostedService>
{
    protected override async Task ExecutionAsync(IServiceProvider provider, CancellationToken ct)
    {
        var tenantRepository = provider.GetRequiredService<ITenantRepository>();
        var orderDeliveryRepository = provider.GetRequiredService<IOrderDeliveryRepository>();
        var orderDeliverdyRepository = provider.GetRequiredService<ITenantTripartiteRepository>();

        var tenants = await tenantRepository.GetListAsync(cancellationToken: ct);
        foreach (var tenant in tenants)
        {
            var orderDeliveries = await orderDeliveryRepository.GetByTenantIdAsync(tenant.Id, ct: ct);
            foreach (var orderDelivery in orderDeliveries)
            {

            }
            //var electronicInvoiceSetting = await orderDeliverdyRepository.GetByTenantIdAsync(tenant.Id, cancellationToken: ct);
            // Perform operations with orderDelivery and electronicInvoiceSetting
            // ...
        }
    }
}