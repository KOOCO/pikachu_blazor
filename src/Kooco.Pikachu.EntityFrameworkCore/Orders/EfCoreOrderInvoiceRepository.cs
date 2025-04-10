using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Orders;
public class EfCoreOrderInvoiceRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
    : EfCoreRepository<PikachuDbContext, OrderInvoice, Guid>(dbContextProvider), IOrderInvoiceRepository
{

}