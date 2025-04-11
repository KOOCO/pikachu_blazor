using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Orders;
public class OrderInvoiceRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, OrderInvoice, Guid>(dbContextProvider), IOrderInvoiceRepository
{
    public async Task<OrderInvoice> CreateInvoiceAsync(OrderInvoice invoice)
    {
        if (invoice is null)
        {
            throw new ArgumentNullException(nameof(invoice), "發票實體不能為 null");
        }

        if (invoice.OrderId == Guid.Empty)
        {
            throw new ArgumentException("訂單 ID 不能為空", nameof(invoice));
        }

        return await InsertAsync(invoice);
    }
    public async Task<short> GetInvoiceSerialNoAsync(Guid orderId)
    {
        var queryable = await GetQueryableAsync();
        var maxSerialNo = await queryable
            .Where(x => x.OrderId == orderId)
            .MaxAsync(x => (short?)x.SerialNo) ?? default;

        return (short)(maxSerialNo + 1);
    }
    public async Task<bool> HasNonVoidedInvoiceAsync(Guid orderId)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.AnyAsync(x => x.OrderId == orderId && !x.IsVoided);
    }
}