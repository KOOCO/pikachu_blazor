using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Kooco.Pikachu.OrderTradeNos
{
    public class EfCoreOrderTradeNoRepository
    (IDbContextProvider<PikachuDbContext> pikachuDbContextProvider) 
        : EfCoreRepository<PikachuDbContext, OrderTradeNo,Guid>(pikachuDbContextProvider),IOrderTradeNoRepository
    {
      
        public async Task<OrderTradeNo?> FindByMarchentTradeNoAsync(string marchentTradeNo)
        {
           var orderTradeNo =await (await GetQueryableAsync())
                .Where(x => x.MarchentTradeNo == marchentTradeNo)
                .Include(x => x.Order)
                
                .FirstOrDefaultAsync();
            return orderTradeNo;
        }

        public async Task<OrderTradeNo?> FindByOrderIdAsync(Guid orderId)
        {
            var orderTradeNo = await(await GetQueryableAsync())
                .Where(x => x.OrderId == orderId)
                .Include(x => x.Order)

                .FirstOrDefaultAsync();
            return orderTradeNo;
        }

       
    }
}
