using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserAddresses;

public class EfCoreUserAddressRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, UserAddress, Guid>(dbContextProvider), IUserAddressRepository
{
    public async Task<long> GetCountAsync(string? filter, Guid? userId, string? postalCode, string? city,
        string? street, string? recipientName, string? recipientPhoneNumber, bool? isDefault)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, postalCode, city, street, recipientName, recipientPhoneNumber, isDefault);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserAddress>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        Guid? userId, string? postalCode, string? city, string? street, string? recipientName, string? recipientPhoneNumber, bool? isDefault)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, postalCode, city, street, recipientName, recipientPhoneNumber, isDefault);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task RemoveDefaultUserAddressesAsync(Guid userId)
    {
        var dbContext = await GetDbContextAsync();

        var userAddresses = dbContext.UserAddresses
                            .Where(x => x.UserId == userId)
                            .ExecuteUpdate(address => address.SetProperty(a => a.IsDefault, a => false));
    }

    public async Task<IQueryable<UserAddress>> GetFilteredQueryableAsync(string? filter, Guid? userId, string? postalCode, string? city,
        string? street, string? recipientName, string? recipientPhoneNumber, bool? isDefault)
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(isDefault.HasValue, x => x.IsDefault == isDefault)
            .WhereIf(!postalCode.IsNullOrWhiteSpace(), x => x.PostalCode.Contains(postalCode))
            .WhereIf(!city.IsNullOrWhiteSpace(), x => x.City.Contains(city))
            .WhereIf(!street.IsNullOrWhiteSpace(), x => x.Street.Contains(street))
            .WhereIf(!recipientName.IsNullOrWhiteSpace(), x => x.RecipientName.Contains(recipientName))
            .WhereIf(!recipientPhoneNumber.IsNullOrWhiteSpace(), x => x.RecipientPhoneNumber.Contains(recipientPhoneNumber))
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.PostalCode.Contains(filter)
            || x.City.Contains(filter) || x.Street.Contains(filter)
            || x.RecipientName.Contains(filter) || x.RecipientPhoneNumber.Contains(filter));
    }
}
