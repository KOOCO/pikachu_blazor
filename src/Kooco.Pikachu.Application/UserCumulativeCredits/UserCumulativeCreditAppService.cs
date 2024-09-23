using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeCredits;

[Authorize]
[RemoteService(IsEnabled = false)]
public class UserCumulativeCreditAppService(UserCumulativeCreditManager userCumulativeCreditManager,
    IUserCumulativeCreditRepository userCumulativeCreditRepository) : PikachuAppService, IUserCumulativeCreditAppService
{
    public async Task<UserCumulativeCreditDto> CreateAsync(CreateUserCumulativeCreditDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeCredit = await userCumulativeCreditManager.CreateAsync(input.UserId!.Value,
            input.TotalAmount, input.TotalDeductions, input.TotalRefunds);
        return ObjectMapper.Map<UserCumulativeCredit, UserCumulativeCreditDto>(userCumulativeCredit);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userCumulativeCredit = await userCumulativeCreditRepository.GetAsync(id);
        await userCumulativeCreditRepository.DeleteAsync(userCumulativeCredit);
    }

    public async Task<UserCumulativeCreditDto> GetAsync(Guid id)
    {
        var userCumulativeCredit = await userCumulativeCreditRepository.GetAsync(id);
        return ObjectMapper.Map<UserCumulativeCredit, UserCumulativeCreditDto>(userCumulativeCredit);
    }

    public async Task<PagedResultDto<UserCumulativeCreditDto>> GetListAsync(GetUserCumulativeCreditListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(UserCumulativeCredit.CreationTime) + " DESC";
        }

        var totalCount = await userCumulativeCreditRepository.GetCountAsync(input.UserId, input.MinTotalAmount, input.MaxTotalAmount,
            input.MinTotalDeductions, input.MaxTotalDeductions, input.MinTotalRefunds, input.MaxTotalRefunds);

        var items = await userCumulativeCreditRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.UserId,
            input.MinTotalAmount, input.MaxTotalAmount, input.MinTotalDeductions, input.MaxTotalDeductions, input.MinTotalRefunds, input.MaxTotalRefunds);

        return new PagedResultDto<UserCumulativeCreditDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<UserCumulativeCredit>, List<UserCumulativeCreditDto>>(items)
        };
    }

    public async Task<UserCumulativeCreditDto> UpdateAsync(Guid id, UpdateUserCumulativeCreditDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userCumulativeCredit = await userCumulativeCreditRepository.GetAsync(id);
        await userCumulativeCreditManager.UpdateAsync(userCumulativeCredit, input.UserId!.Value,
            input.TotalAmount, input.TotalDeductions, input.TotalRefunds);
        return ObjectMapper.Map<UserCumulativeCredit, UserCumulativeCreditDto>(userCumulativeCredit);
    }
}
