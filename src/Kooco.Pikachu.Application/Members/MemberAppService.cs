using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Kooco.Pikachu.Members;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Members.Default)]
public class MemberAppService(IRepository<IdentityUser, Guid> identityUserRepository,
    IdentityUserManager identityUserManager, UserAddressManager userAddressManager,
    IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository) : PikachuAppService, IMemberAppService
{
    public async Task<MemberDto> GetAsync(Guid id)
    {
        var member = await identityUserRepository.GetAsync(id);
        return ObjectMapper.Map<IdentityUser, MemberDto>(member);
    }

    public async Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(IdentityUser.UserName);
        }

        var queryable = await identityUserRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.UserName.Contains(input.Filter)
            || q.PhoneNumber.Contains(input.Filter) || q.Email.Contains(input.Filter));

        var random = new Random();
        var amount = random.Next(100, 1000);
        return new PagedResultDto<MemberDto>
        {
            TotalCount = queryable.Count(),
            Items = [..queryable.OrderBy(input.Sorting).PageBy(input.SkipCount, input.MaxResultCount)
                    .Select(x => new MemberDto
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Orders = random.Next(0, 15),
                        Spent = random.Next(0, 15) * amount
                    })]
        };
    }

    [Authorize(PikachuPermissions.Members.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var member = await identityUserRepository.GetAsync(id);
        await identityUserRepository.DeleteAsync(member);
    }

    [Authorize(PikachuPermissions.Members.Edit)]
    public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.DefaultAddressId, nameof(input.DefaultAddressId));

        var member = await identityUserRepository.GetAsync(id);

        member.Name = input.Name;

        (await identityUserManager.SetEmailAsync(member, input.Email)).CheckErrors();
        (await identityUserManager.SetPhoneNumberAsync(member, input.PhoneNumber)).CheckErrors();

        member.ExtraProperties.Remove(Constant.Birthday);
        member.ExtraProperties.TryAdd(Constant.Birthday, input.Birthday);

        (await identityUserManager.UpdateAsync(member)).CheckErrors();

        await userAddressManager.SetIsDefaultAsync(input.DefaultAddressId.Value, true);
        return ObjectMapper.Map<IdentityUser, MemberDto>(member);
    }

    public async Task<UserAddressDto?> GetDefaultAddressAsync(Guid id)
    {
        var defaultAddress = await userAddressManager.GetDefaultAddressAsync(id);
        return ObjectMapper.Map<UserAddress?, UserAddressDto?>(defaultAddress);
    }

    public async Task<PagedResultDto<OrderDto>> GetMemberOrdersAsync(GetOrderListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Order.CreationTime) + " DESC";
        }

        var queryable = await orderRepository.GetQueryableAsync();

        queryable = queryable
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => x.OrderNo.Contains(input.Filter))
                .WhereIf(input.StartDate.HasValue, x => x.CreationTime.Date >= input.StartDate.Value.Date)
                .WhereIf(input.EndDate.HasValue, x => x.CreationTime.Date <= input.EndDate.Value)
                .WhereIf(input.GroupBuyId.HasValue, x => x.GroupBuyId == input.GroupBuyId)
                .WhereIf(input.ShippingStatus.HasValue, x => x.ShippingStatus == input.ShippingStatus)
                .WhereIf(input.DeliveryMethod.HasValue, x => x.DeliveryMethod == input.DeliveryMethod);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable.OrderBy(input.Sorting).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }

    public async Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
    {
        var queryable = await groupBuyRepository.GetQueryableAsync();
        return [.. queryable.Select(x => new KeyValueDto { Id = x.Id, Name = x.GroupBuyName })];
    }
}
