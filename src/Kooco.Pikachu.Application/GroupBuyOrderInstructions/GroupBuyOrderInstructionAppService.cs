using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupPurchaseOverviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuyOrderInstructions;

public class GroupBuyOrderInstructionAppService :
    CrudAppService<
        GroupBuyOrderInstruction,
        GroupBuyOrderInstructionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupBuyOrderInstructionDto
    >, IGroupBuyOrderInstructionAppService
{
    #region Inject
    private readonly IGroupBuyOrderInstructionRepository _GroupBuyOrderInstructionRepository;
    #endregion

    #region Constructor
    public GroupBuyOrderInstructionAppService(
        IRepository<GroupBuyOrderInstruction, Guid> Repository,
        IGroupBuyOrderInstructionRepository GroupBuyOrderInstructionRepository
    ) 
        : base(Repository)
    {
        _GroupBuyOrderInstructionRepository = GroupBuyOrderInstructionRepository;
    }
    #endregion

    #region Methods
    public async Task<GroupBuyOrderInstructionDto> CreateGroupBuyOrderInstructionAsync(GroupBuyOrderInstructionDto groupBuyOrderInstruction)
    {
        return await CreateAsync(
            ObjectMapper.Map<GroupBuyOrderInstructionDto, CreateUpdateGroupBuyOrderInstructionDto>(groupBuyOrderInstruction)
        );
    }

    public async Task<GroupBuyOrderInstructionDto> UpdateGroupPurchaseOverviewAsync(GroupBuyOrderInstructionDto groupBuyOrderInstruction)
    {
        return await UpdateAsync(
            groupBuyOrderInstruction.Id,
            ObjectMapper.Map<GroupBuyOrderInstructionDto, CreateUpdateGroupBuyOrderInstructionDto>(groupBuyOrderInstruction)
        );
    }

    public async Task<List<GroupBuyOrderInstructionDto>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return await MapToGetListOutputDtosAsync(
            await _GroupBuyOrderInstructionRepository.GetListByGroupBuyIdAsync(groupBuyId)
        );
    }
    #endregion
}
