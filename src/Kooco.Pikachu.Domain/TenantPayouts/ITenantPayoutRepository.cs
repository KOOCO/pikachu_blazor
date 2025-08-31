using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantPayouts;

public interface ITenantPayoutRepository : IRepository<TenantPayoutRecord, Guid>
{
}
