using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.OrderDeliveries
{
    public class EfCoreOrderDeliveryRepository : EfCoreRepository<PikachuDbContext, OrderDelivery, Guid>, IOrderDeliveryRepository
    {
        public EfCoreOrderDeliveryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<OrderDelivery>> GetWithDetailsAsync(Guid id)
        {
            return await (await GetQueryableAsync())
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
        }
    }
}
