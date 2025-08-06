using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public interface IOrderMatchingService
    {
        Task<OrderMatchingResult> FindOrderAsync(string merchantTradeNo);
    }

    public class OrderMatchingResult
    {
        public bool IsFound { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? OrderId { get; set; }
        public string OrderNumber { get; set; }
    }
}
