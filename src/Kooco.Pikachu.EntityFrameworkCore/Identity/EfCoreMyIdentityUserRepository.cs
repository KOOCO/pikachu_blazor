using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Kooco.Pikachu.Identity;

public class EfCoreMyIdentityUserRepository(IDbContextProvider<IIdentityDbContext> dbContextProvider) : EfCoreIdentityUserRepository(dbContextProvider), IMyIdentityUserRepository
{
    public async Task<IdentityUser?> FindByExternalIdAsync(LoginMethod loginMethod, string externalId, bool exception = false)
    {
        var queryable = await GetQueryableAsync();

        var user = queryable
            .WhereIf(loginMethod == LoginMethod.Line, x => EF.Property<string>(x, Constant.LineId) == externalId)
            .WhereIf(loginMethod == LoginMethod.Facebook, x => EF.Property<string>(x, Constant.FacebookId) == externalId)
            .WhereIf(loginMethod == LoginMethod.Google, x => EF.Property<string>(x, Constant.GoogleId) == externalId)
            .Include(x=>x.Roles)
            .FirstOrDefault();

        if (exception && user is null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        return user;
    }

    public async Task<IdentityUser?> FindByNameOrEmailAsync(string userNameOrEmail)
    {
        userNameOrEmail = userNameOrEmail.ToUpper();

        var queryable = await GetQueryableAsync();

        var user = await queryable
            .Where(user => user.NormalizedUserName == userNameOrEmail || user.NormalizedEmail == userNameOrEmail)
            .Include(user => user.Roles)
            .FirstOrDefaultAsync();

        return user;
    }
}
