using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.UserShoppingCredits;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.UserShoppingCredits.Default)]
public class UserShoppingCreditAppService(UserShoppingCreditManager userShoppingCreditManager,
    IUserShoppingCreditRepository userShoppingCreditRepository, IUserCumulativeCreditAppService userCumulativeCreditAppService,
    IUserCumulativeCreditRepository userCumulativeCreditRepository, IDataFilter<IMultiTenant> _multiTenantFilter,
    IOrderRepository orderRepository) : PikachuAppService, IUserShoppingCreditAppService
{
    [Authorize(PikachuPermissions.UserShoppingCredits.Create)]
    public async Task<UserShoppingCreditDto> CreateAsync(CreateUserShoppingCreditDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNull(input.ShoppingCreditType, nameof(input.ShoppingCreditType));

        var userShoppingCredit = await userShoppingCreditManager.CreateAsync(input.UserId, input.Amount,
            input.CurrentRemainingCredits, input.TransactionDescription, input.ExpirationDate, input.IsActive, input.ShoppingCreditType.Value);

        var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == input.UserId);

        if (userCumulativeCredit is null)
        {
            await userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto
            {
                TotalAmount = input.ShoppingCreditType == UserShoppingCreditType.Grant ? input.Amount : 0,
                TotalDeductions = input.ShoppingCreditType == UserShoppingCreditType.Deduction ? input.Amount : 0,
                TotalRefunds = 0,
                UserId = input.UserId
            });
        }
        else
        {
            if (input.ShoppingCreditType == UserShoppingCreditType.Grant)
            {
                userCumulativeCredit.ChangeTotalAmount(userCumulativeCredit.TotalAmount + input.Amount);
            }
            else
            {
                userCumulativeCredit.ChangeTotalDeductions(userCumulativeCredit.TotalDeductions + input.Amount);
            }
            await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
        }

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
        Check.NotNull(input.ShoppingCreditType, nameof(input.ShoppingCreditType));

        var userShoppingCredit = await userShoppingCreditManager.CreateAsync(input.UserId, input.Amount,
            input.CurrentRemainingCredits, input.TransactionDescription, input.ExpirationDate, input.IsActive, input.ShoppingCreditType.Value);
        return ObjectMapper.Map<UserShoppingCredit, UserShoppingCreditDto>(userShoppingCredit);
    }

    public async Task<ShoppingCreditStatDto> GetShoppingCreditStatsAsync()
    {
        var query = await userShoppingCreditRepository.GetQueryableAsync();
        var result = new ShoppingCreditStatDto();

        result.TodayIssueAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Grant && x.CreationTime.Date == DateTime.Now.Date).Sum(x => x.Amount);
        result.ThisWeekIssueAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Grant && (x.CreationTime.Date <= DateTime.Now.Date && x.CreationTime.Date > DateTime.Now.Date.AddDays(-7))).Sum(x => x.Amount);
        result.ThisMonthIssueAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Grant && (x.CreationTime.Date <= DateTime.Now.Date && x.CreationTime.Date > DateTime.Now.Date.AddDays(-30))).Sum(x => x.Amount);

        result.TodayRedeemedAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Deduction && x.CreationTime.Date == DateTime.Now.Date).Sum(x => x.Amount);
        result.ThisWeekRedeemedAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Deduction && (x.CreationTime.Date <= DateTime.Now.Date && x.CreationTime.Date > DateTime.Now.Date.AddDays(-7))).Sum(x => x.Amount);
        result.ThisMonthRedeemedAmount = query.Where(x => x.ShoppingCreditType == UserShoppingCreditType.Deduction && (x.CreationTime.Date <= DateTime.Now.Date && x.CreationTime.Date > DateTime.Now.Date.AddDays(-30))).Sum(x => x.Amount);
        return result;

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
        var userCredit = await (await userShoppingCreditRepository.GetQueryableAsync())
            .Where(x => x.UserId == memberId)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
        return userCredit?.CurrentRemainingCredits ?? 0;
    }

    [AllowAnonymous]
    public async Task<int> ExpireCreditsAsync()
    {
        using (_multiTenantFilter.Disable())
        {
            var expirableCredits = await userShoppingCreditRepository.GetExpirableCreditsAsync();
            var orders = await (await orderRepository.GetQueryableAsync())
                        .Where(o => o.CreditDeductionRecordId.HasValue && expirableCredits.Select(e => e.Id).Contains(o.CreditDeductionRecordId.Value))
                        .Select(o => new
                        {
                            o.Id,
                            o.CreditDeductionRecordId,
                            o.CreditDeductionAmount
                        })
                        .ToListAsync();

            expirableCredits.ForEach(async credit =>
            {
                using (CurrentTenant.Change(credit.TenantId))
                {
                    credit.SetIsActive(false);
                    var usedAmount = orders.Where(o => o.CreditDeductionRecordId == credit.Id).Sum(o => o.CreditDeductionAmount);
                    var remainingAmount = credit.Amount - usedAmount;
                    await userShoppingCreditManager.CreateAsync(credit.UserId, remainingAmount,
                        remainingAmount, "Credits Expired", null, false, UserShoppingCreditType.Deduction, true);

                    var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == credit.UserId);

                    if (userCumulativeCredit is null)
                    {
                        await userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto
                        {
                            TotalAmount = 0,
                            TotalDeductions = remainingAmount,
                            TotalRefunds = 0,
                            UserId = credit.UserId
                        });
                    }
                    else
                    {
                        userCumulativeCredit.ChangeTotalDeductions(userCumulativeCredit.TotalDeductions + remainingAmount);
                    }
                }
            });

            return 1;
        }
    }
}
