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

[Authorize(PikachuPermissions.Members.Default)]
public class MemberAppService(IRepository<IdentityUser, Guid> identityUserRepository,
    IdentityUserManager identityUserManager, UserAddressManager userAddressManager) : PikachuAppService, IMemberAppService
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
        (await identityUserManager.UpdateAsync(member)).CheckErrors();

        await userAddressManager.SetIsDefaultAsync(input.DefaultAddressId.Value, true);
        return ObjectMapper.Map<IdentityUser, MemberDto>(member);
    }

    public async Task<UserAddressDto?> GetDefaultAddressAsync(Guid id)
    {
        var defaultAddress = await userAddressManager.GetDefaultAddressAsync(id);
        return ObjectMapper.Map<UserAddress?, UserAddressDto?>(defaultAddress);
    }
}
