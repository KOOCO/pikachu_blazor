using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Orders;
internal sealed class OrderHostedService : HostedServiceBase<OrderHostedService>
{
    protected override async Task ExecutionAsync(IServiceProvider provider, CancellationToken ct)
    {
        var tenantRepository = provider.GetRequiredService<ITenantRepository>();
        var orderDeliveryRepository = provider.GetRequiredService<IOrderDeliveryRepository>();
        var tenantTripartiteRepository = provider.GetRequiredService<ITenantTripartiteRepository>();
        var orderInvoiceRepository = provider.GetRequiredService<IOrderInvoiceRepository>();
        var orderInvoiceAppService = provider.GetRequiredService<IOrderInvoiceAppService>();

        var tenants = await tenantRepository.GetListAsync(cancellationToken: ct);
        foreach (var tenant in tenants)
        {
            using (DataFilter.Disable<IMultiTenant>())
            {
                var tripartite = await tenantTripartiteRepository.FindByTenantAsync(tenant.Id, ct: ct);
                if (tripartite is null) continue;

                var orderDeliveries = await orderDeliveryRepository.GetByStatusAsync(
                    tenant.Id, tripartite.StatusOnInvoiceIssue, ct: ct);

                foreach (var orderDelivery in orderDeliveries)
                {
                    if (!await orderInvoiceRepository.HasNonVoidedInvoiceAsync(orderDelivery.OrderId))
                    {
                        var openingTime = orderDelivery.CreationTime.AddDays(tripartite.DaysAfterShipmentGenerateInvoice);
                        if (Clock.Now >= openingTime)
                        {
                            await orderInvoiceAppService.CreateInvoiceAsync(orderDelivery.OrderId);
                        }
                    }
                }
            }
        }
    }
}