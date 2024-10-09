using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuyOrderInstructions;

public class EfCoreGroupBuyOrderInstructionRepository :
    EfCoreRepository<
        PikachuDbContext,
        GroupBuyOrderInstruction,
        Guid
    >, IGroupBuyOrderInstructionRepository
{
    #region Constructor
    public EfCoreGroupBuyOrderInstructionRepository(
        IDbContextProvider<PikachuDbContext> DbContextProvider
    ) 
        : base(DbContextProvider)
    {
    }
    #endregion

    #region Methods
    public async Task<List<GroupBuyOrderInstruction>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return [.. (await GetQueryableAsync()).Where(w => w.GroupBuyId == groupBuyId)];
    }
    #endregion
}
