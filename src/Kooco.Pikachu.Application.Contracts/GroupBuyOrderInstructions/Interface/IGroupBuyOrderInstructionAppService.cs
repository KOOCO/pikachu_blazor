using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuyOrderInstructions.Interface;

public interface IGroupBuyOrderInstructionAppService :
    ICrudAppService<
        GroupBuyOrderInstructionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupBuyOrderInstructionDto
    >
{
    Task<GroupBuyOrderInstructionDto> CreateGroupBuyOrderInstructionAsync(GroupBuyOrderInstructionDto groupBuyOrderInstruction);

    Task<GroupBuyOrderInstructionDto> UpdateGroupPurchaseOverviewAsync(GroupBuyOrderInstructionDto groupBuyOrderInstruction);

    Task<List<GroupBuyOrderInstructionDto>> GetListByGroupBuyIdAsync(Guid groupBuyId);
}
