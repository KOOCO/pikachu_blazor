using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;


namespace Kooco.Pikachu.OrderTradeNos
{
    public interface IOrderTradeNoRepository:IRepository<OrderTradeNo, Guid>
    {
        /// <summary>
        /// 根據商戶交易號碼查找訂單交易號
        /// </summary>
        /// <param name="marchentTradeNo">商戶交易號碼</param>
        /// <returns>訂單交易號</returns>
        Task<OrderTradeNo?> FindByMarchentTradeNoAsync(string marchentTradeNo);

        /// <summary>
        /// 根據訂單ID查找訂單交易號
        /// </summary>
        /// <param name="orderId">訂單ID</param>
        /// <returns>訂單交易號</returns>
        Task<OrderTradeNo?> FindByOrderIdAsync(Guid orderId);
    }
}
