using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuyOrderInstructions.Interface;

public interface IGroupBuyOrderInstructionRepository : IRepository<GroupBuyOrderInstruction, Guid>
{
    Task<List<GroupBuyOrderInstruction>> GetListByGroupBuyIdAsync(Guid groupBuyId);
}
