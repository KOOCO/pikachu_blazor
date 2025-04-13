using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories.Orders;
public class OrderDeliveryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, OrderDelivery, Guid>(dbContextProvider), IOrderDeliveryRepository
{
    public async Task<List<OrderDelivery>> GetWithDetailsAsync(Guid id)
    {
        try
        {
            List<OrderDelivery> orderDeliveries = await (await GetQueryableAsync())
                .Where(o => o.OrderId == id)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Item)
                    .ThenInclude(i => i.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.SetItem)
                    .ThenInclude(i => i.SetItemDetails)
                    .ThenInclude(i => i.Item)
                    .ThenInclude(i => i.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.SetItem)
                    .ThenInclude(i => i.Images)
                .Include(o => o.Items.OrderBy(oi => oi.ItemType))
                    .ThenInclude(oi => oi.Freebie)
                    .ThenInclude(i => i.Images)
                .ToListAsync();

            orderDeliveries =
            [
                .. orderDeliveries.OrderBy(od => od.Items is { Count: > 0 }
                    ? od.Items.Min(oi => oi.DeliveryTemperature) :
                    ItemStorageTemperature.Normal)
            ];

            return orderDeliveries;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<Guid> GetOrderIdByAllPayLogisticsId(string AllPayLogisticsId)
    {
        return (await GetQueryableAsync())
                            .Where(w => w.AllPayLogisticsID == AllPayLogisticsId || w.FileNo == AllPayLogisticsId)
                            .Select(s => s.OrderId)
                            .FirstOrDefault();
    }
    public async Task<List<OrderDelivery>> GetByStatusAsync(Guid tenantId, DeliveryStatus status, CancellationToken ct)
    {
        return await (await GetQueryableAsync())
            .Where(od =>
                od.TenantId == tenantId &&
                od.DeliveryStatus == status)
            .ToListAsync(cancellationToken: ct);
    }
}