using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.OrderDeliveries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.OrderHistories
{
    public class EfCoreOrderHistoryRepository :EfCoreRepository<PikachuDbContext,OrderHistory,Guid>, IOrderHistoryRepository
    {
        
        public EfCoreOrderHistoryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        /// <summary>
        /// Gets all history records for a specific order, ordered by creation time.
        /// </summary>
        public async Task<List<OrderHistory>> GetAllHistoryByOrderIdAsync(Guid orderId)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.OrderHistories
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.CreationTime)
                .ToListAsync();
        }

    }
}
