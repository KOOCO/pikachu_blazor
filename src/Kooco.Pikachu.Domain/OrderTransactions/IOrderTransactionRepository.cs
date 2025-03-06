using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.OrderTransactions;

public interface IOrderTransactionRepository : IRepository<OrderTransaction, Guid>
{
    Task<long> CountAsync(string? filter);
    Task<List<OrderTransaction>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter);
}
