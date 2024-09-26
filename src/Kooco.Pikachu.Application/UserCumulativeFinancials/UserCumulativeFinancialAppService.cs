using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeFinancials;

[Authorize]
[RemoteService(IsEnabled = false)]
public class UserCumulativeFinancialAppService(IUserCumulativeFinancialRepository userCumulativeFinancialRepository,
    UserCumulativeFinancialManager userCumulativeFinancialManager) : PikachuAppService, IUserCumulativeFinancialAppService
{
    public async Task<UserCumulativeFinancialDto> CreateAsync(CreateUserCumulativeFinancialDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeFinancial = await userCumulativeFinancialManager.CreateAsync(input.UserId.Value, input.TotalSpent,
            input.TotalPaid, input.TotalUnpaid, input.TotalRefunded);

        return ObjectMapper.Map<UserCumulativeFinancial, UserCumulativeFinancialDto>(userCumulativeFinancial);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userCumulativeFinancial = await userCumulativeFinancialRepository.GetAsync(id);
        await userCumulativeFinancialRepository.DeleteAsync(userCumulativeFinancial);
    }

    public async Task<UserCumulativeFinancialDto> GetAsync(Guid id)
    {
        var userCumulativeFinancial = await userCumulativeFinancialRepository.GetAsync(id);
        return ObjectMapper.Map<UserCumulativeFinancial, UserCumulativeFinancialDto>(userCumulativeFinancial);
    }

    public async Task<PagedResultDto<UserCumulativeFinancialDto>> GetListAsync(GetUserCumulativeFinancialListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(UserCumulativeFinancial.CreationTime) + " DESC";
        }

        var totalCount = await userCumulativeFinancialRepository.GetCountAsync(input.UserId, input.MinTotalSpent, input.MaxTotalSpent, input.MinTotalPaid,
            input.MaxTotalPaid, input.MinTotalUnpaid, input.MaxTotalUnpaid, input.MinTotalRefunded, input.MaxTotalRefunded);

        var items = await userCumulativeFinancialRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.UserId, input.MinTotalSpent,
            input.MaxTotalSpent, input.MinTotalPaid, input.MaxTotalPaid, input.MinTotalUnpaid, input.MaxTotalUnpaid, input.MinTotalRefunded, input.MaxTotalRefunded);

        return new PagedResultDto<UserCumulativeFinancialDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<UserCumulativeFinancial>, List<UserCumulativeFinancialDto>>(items)
        };
    }

    public async Task<UserCumulativeFinancialDto> UpdateAsync(Guid id, UpdateUserCumulativeFinancialDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeFinancial = await userCumulativeFinancialRepository.GetAsync(id);
        await userCumulativeFinancialManager.UpdateAsync(userCumulativeFinancial, input.UserId.Value, input.TotalSpent, input.TotalPaid,
            input.TotalUnpaid, input.TotalRefunded);
        return ObjectMapper.Map<UserCumulativeFinancial, UserCumulativeFinancialDto>(userCumulativeFinancial);
    }
}
