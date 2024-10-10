using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Kooco.Pikachu.Identity;

[Dependency(ReplaceServices = true)]
public class MyIdentityUserAppService : IdentityUserAppService, IMyIdentityUserAppService
{
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
    public MyIdentityUserAppService(IdentityUserManager userManager, IIdentityUserRepository userRepository, IIdentityRoleRepository roleRepository,
        IOptions<IdentityOptions> identityOptions, IPermissionChecker permissionChecker, IRepository<IdentityUser, Guid> identityUserRepository) : base(userManager, userRepository, roleRepository, identityOptions, permissionChecker)
    {
        _identityUserRepository = identityUserRepository;
    }

    [Authorize(IdentityPermissions.Users.Default)]
    public async Task<PagedResultDto<IdentityUserDto>> GetCategorizedListAsync(GetCategorizedListDto input)
    {
        var queryable = await GetCategorizedQueryableAsync(input.UserTypes);

        string filter = input.Filter;
        var upperFilter = filter?.ToUpperInvariant();

        queryable = queryable
            .WhereIf(
                !input.Filter.IsNullOrWhiteSpace(),
                u =>
                    u.NormalizedUserName.Contains(upperFilter) ||
                    u.NormalizedEmail.Contains(upperFilter) ||
                    (u.Name != null && u.Name.Contains(filter)) ||
                    (u.Surname != null && u.Surname.Contains(filter)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(filter))
            );

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable.OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(IdentityUser.UserName) : input.Sorting).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<IdentityUserDto>(
            totalCount,
            ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(items)
        );
    }

    public async Task<List<KeyValueDto>> GetCategorizedLookupAsync(UserTypes? userTypes)
    {
        var queryable = await GetCategorizedQueryableAsync(userTypes);
        return [.. queryable.Select(x => new KeyValueDto { Id = x.Id, Name = x.UserName })];
    }

    private async Task<IQueryable<IdentityUser>> GetCategorizedQueryableAsync(UserTypes? userTypes)
    {
        var queryable = await _identityUserRepository.WithDetailsAsync();
        if (userTypes.HasValue && userTypes != UserTypes.All)
        {
            var role = await RoleRepository.FindByNormalizedNameAsync(MemberConsts.Role);
            queryable = queryable
                .WhereIf(userTypes == UserTypes.Frontend, x => x.Roles.Any(r => role != null && r.RoleId == role.Id))
                .WhereIf(userTypes == UserTypes.Backend, x => !x.Roles.Any(r => role == null || r.RoleId == role.Id));
        }

        return queryable;
    }
}
