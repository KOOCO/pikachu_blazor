using Kooco.Pikachu.Orders.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;
public interface IOrderInvoiceRepository : IRepository<OrderInvoice, Guid>
{
    /// <summary>
    /// 新增訂單發票
    /// </summary>
    Task<OrderInvoice> CreateInvoiceAsync(OrderInvoice invoice);

    /// <summary>
    /// 取得訂單的發票序號
    /// </summary>
    Task<short> GetInvoiceSerialNoAsync(Guid orderId);

    /// <summary>
    /// 檢查訂單是否有未作廢的發票
    /// </summary>
    Task<bool> HasNonVoidedInvoiceAsync(Guid orderId);
}