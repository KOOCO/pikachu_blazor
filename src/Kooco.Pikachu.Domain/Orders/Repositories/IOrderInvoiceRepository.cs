using Kooco.Pikachu.Orders.Entities;
using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;
public interface IOrderInvoiceRepository : IRepository<OrderInvoice, Guid>
{

}