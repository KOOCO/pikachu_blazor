using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Refunds
{
    public interface IRefundRepository : IRepository<Refund, Guid>
    {
        Task<long> GetCountAsync(string? filter);
        Task<List<Refund>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter);
        Task<Refund> GetAsync(Guid Id);
     }
}
