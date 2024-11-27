using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserShoppingCredits;

//[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.UserShoppingCredits.Default)]
public class UserShoppingCreditAppService(UserShoppingCreditManager userShoppingCreditManager,
    IUserShoppingCreditRepository userShoppingCreditRepository) : PikachuAppService, IUserShoppingCreditAppService
{
    [Authorize(PikachuPermissions.UserShoppingCredits.Create)]
    public async Task<UserShoppingCreditDto> CreateAsync(CreateUserShoppingCreditDto input)
    {
        Check.NotNull(input, nameof(input));

        var userShoppingCredit = await userShoppingCreditManager.CreateAsync(input.UserId, input.Amount,
            input.CurrentRemainingCredits, input.TransactionDescription, input.ExpirationDate, input.IsActive);

        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    [Authorize(PikachuPermissions.UserShoppingCredits.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var userShoppingCredit = await userShoppingCreditRepository.GetAsync(id);
        await userShoppingCreditRepository.DeleteAsync(userShoppingCredit);
    }

    public async Task<UserShoppingCreditDto> GetAsync(Guid id)
    {
        var userShoppingCredit = await userShoppingCreditRepository.GetAsync(id);
        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    public async Task<PagedResultDto<UserShoppingCreditDto>> GetListAsync(GetUserShoppingCreditListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(UserShoppingCredit.CreationTime) + " DESC";
        }

        var totalCount = await userShoppingCreditRepository.GetCountAsync(input.Filter, input.UserId, input.MinAmount, input.MaxAmount,
            input.MinCurrentRemainingCredits, input.MaxCurrentRemainingCredits, input.TransactionDescription,
            input.MinExpirationDate, input.MaxExpirationDate, input.IsActive);

        var items = await userShoppingCreditRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter,
            input.UserId, input.MinAmount, input.MaxAmount, input.MinCurrentRemainingCredits, input.MaxCurrentRemainingCredits,
            input.TransactionDescription, input.MinExpirationDate, input.MaxExpirationDate, input.IsActive);

        return new PagedResultDto<UserShoppingCreditDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<UserShoppingCredit>, List<UserShoppingCreditDto>>(items)
        };
    }
    [AllowAnonymous]
    public async Task<UserShoppingCreditDto> RecordShoppingCreditAsync(RecordUserShoppingCreditDto input)
    {
        Check.NotNull(input, nameof(input));

        var userShoppingCredit = await userShoppingCreditManager.CreateAsync(input.UserId, input.Amount,
            input.CurrentRemainingCredits, input.TransactionDescription, input.ExpirationDate, input.IsActive);

        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    [Authorize(PikachuPermissions.UserShoppingCredits.SetIsActive)]
    public async Task<UserShoppingCreditDto> SetIsActiveAsync(Guid id, bool isActive)
    {
        var userShoppingCredit = await userShoppingCreditRepository.GetAsync(id);
        await userShoppingCreditManager.SetIsActiveAsync(userShoppingCredit, isActive);
        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    [Authorize(PikachuPermissions.UserShoppingCredits.Edit)]
    public async Task<UserShoppingCreditDto> UpdateAsync(Guid id, UpdateUserShoppingCreditDto input)
    {
        Check.NotNull(input, nameof(input));

        var userShoppingCredit = await userShoppingCreditRepository.GetAsync(id);

        await userShoppingCreditManager.UpdateAsync(userShoppingCredit, input.UserId, input.Amount,
            input.CurrentRemainingCredits, input.TransactionDescription, input.ExpirationDate, input.IsActive);

        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    public async Task<int> GetMemberCurrentShoppingCreditAsync(Guid memberId)
    {
        var userCredit = (await userShoppingCreditRepository.GetQueryableAsync()).Where(x => x.UserId == memberId).OrderByDescending(x=>x.CreationTime).LastOrDefault();
        return userCredit?.CurrentRemainingCredits??0;
    
    }
}
