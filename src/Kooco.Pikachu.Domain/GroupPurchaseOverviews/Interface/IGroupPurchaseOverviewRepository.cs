using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupPurchaseOverviews.Interface;

public interface IGroupPurchaseOverviewRepository : IRepository<GroupPurchaseOverview, Guid>
{
    Task<List<GroupPurchaseOverview>> GetListByGroupBuyIdAsync(Guid groupBuyId);
}
