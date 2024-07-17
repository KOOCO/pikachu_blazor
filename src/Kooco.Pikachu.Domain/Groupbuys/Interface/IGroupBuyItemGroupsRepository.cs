using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Groupbuys.Interface;

public interface IGroupBuyItemGroupsRepository : IRepository<GroupBuyItemGroup, Guid>
{
}
