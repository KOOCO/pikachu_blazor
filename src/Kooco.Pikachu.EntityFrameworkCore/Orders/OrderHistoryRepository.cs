using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Orders;
public class OrderHistoryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, OrderHistory, Guid>(dbContextProvider), IOrderHistoryRepository
{
    public async Task<List<OrderHistory>> GetAllHistoryByOrderIdAsync(Guid orderId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.OrderHistories
            .Where(h => h.OrderId == orderId)
            .OrderBy(h => h.CreationTime)
            .ToListAsync();
    }
}