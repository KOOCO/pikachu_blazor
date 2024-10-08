﻿using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.UserAddresses;

public class EfCoreUserAddressRepository(IDbContextProvider<PikachuDbContext> dbContextProvider
    ) : EfCoreRepository<PikachuDbContext, UserAddress, Guid>(dbContextProvider), IUserAddressRepository
{
    public async Task<long> GetCountAsync(string? filter, Guid? userId, string? postalCode, string? city,
        string? address, string? recipientName, string? recipientPhoneNumber, bool? isDefault)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, postalCode, city, address, recipientName, recipientPhoneNumber, isDefault);
        return await queryable.LongCountAsync();
    }

    public async Task<List<UserAddress>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        Guid? userId, string? postalCode, string? city, string? address, string? recipientName, string? recipientPhoneNumber, bool? isDefault)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, postalCode, city, address, recipientName, recipientPhoneNumber, isDefault);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task RemoveDefaultUserAddressesAsync(Guid userId)
    {
        var dbContext = await GetDbContextAsync();

        var userAddresses = await dbContext.UserAddresses
                            .Where(x => x.UserId == userId)
                            .ToListAsync();
        foreach (var address in userAddresses)
        {
            address.SetIsDefault(false);
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task<IQueryable<UserAddress>> GetFilteredQueryableAsync(
            string? filter = null,
            Guid? userId = null,
            string? postalCode = null,
            string? city = null,
            string? address = null,
            string? recipientName = null,
            string? recipientPhoneNumber = null,
            bool? isDefault = null
            )
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(isDefault.HasValue, x => x.IsDefault == isDefault)
            .WhereIf(!postalCode.IsNullOrWhiteSpace(), x => x.PostalCode.Contains(postalCode))
            .WhereIf(!city.IsNullOrWhiteSpace(), x => x.City.Contains(city))
            .WhereIf(!address.IsNullOrWhiteSpace(), x => x.Address.Contains(address))
            .WhereIf(!recipientName.IsNullOrWhiteSpace(), x => x.RecipientName.Contains(recipientName))
            .WhereIf(!recipientPhoneNumber.IsNullOrWhiteSpace(), x => x.RecipientPhoneNumber.Contains(recipientPhoneNumber))
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.PostalCode.Contains(filter)
            || x.City.Contains(filter) || x.Address.Contains(filter)
            || x.RecipientName.Contains(filter) || x.RecipientPhoneNumber.Contains(filter));
    }

    public async Task<UserAddress?> GetDefaultAddressAsync(Guid userId)
    {
        var queryable = await GetQueryableAsync();

        var defaultAddress = await queryable
            .Where(q => q.UserId == userId && q.IsDefault)
            .FirstOrDefaultAsync();

        return defaultAddress;
    }
}
