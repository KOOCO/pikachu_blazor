﻿using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Kooco.Pikachu.Members;

public class EfCoreMemberRepository(IDbContextProvider<PikachuDbContext> pikachuDbContextProvider,
    IDbContextProvider<IIdentityDbContext> dbContextProvider) : EfCoreIdentityUserRepository(dbContextProvider), IMemberRepository
{
    protected virtual Task<PikachuDbContext> GetPikachuDbContextAsync()
    {
        return pikachuDbContextProvider.GetDbContextAsync();
    }

    public async Task<long> GetCountAsync(string? filter)
    {
        var queryable = await GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(filter), q => q.UserName.Contains(filter)
            || q.PhoneNumber.Contains(filter) || q.Email.Contains(filter));
        return queryable.Count();
    }

    public async Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var query = from user in dbContext.Users
                    
                    where string.IsNullOrWhiteSpace(filter)
                          || user.UserName.Contains(filter)
                          || user.PhoneNumber.Contains(filter)
                          || user.Email.Contains(filter)
                    select new MemberModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Birthday = (DateTime?)user.GetProperty(Constant.Birthday, null),
                        TotalOrders =dbContext.Orders.Where(x=>x.UserId==user.Id).Count(),
                        TotalSpent = (int)dbContext.Orders.Where(x => x.UserId == user.Id && (x.OrderStatus!=EnumValues.OrderStatus.Exchange && x.OrderStatus != EnumValues.OrderStatus.Refund)).Sum(x=>x.TotalAmount)
                    };


        var members = await query.OrderBy(sorting)
                                 .Skip(skipCount)
                                 .Take(maxResultCount)
                                 .ToListAsync();

        return members;
    }

    public async Task<long> GetMemberCreditRecordCountAsync(string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo,
        DateTime? expirationTimeFrom, DateTime? expirationTimeTo, int? minRemainingCredits, int? maxRemainingCredits,
        int? minAmount, int? maxAmount, Guid? userId)
    {
        var queryable = await GetMemberCreditRecordQueryableAsync(filter, usageTimeFrom, usageTimeTo, expirationTimeFrom, expirationTimeTo,
            minRemainingCredits, maxRemainingCredits, minAmount, maxAmount, userId);
        return await queryable.LongCountAsync();
    }

    public async Task<List<MemberCreditRecordModel>> GetMemberCreditRecordListAsync(int skipCount, int maxResultCount,
        string sorting, string? filter, DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom,
        DateTime? expirationTimeTo, int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId)
    {
        var queryable = await GetMemberCreditRecordQueryableAsync(filter, usageTimeFrom, usageTimeTo, expirationTimeFrom, expirationTimeTo,
            minRemainingCredits, maxRemainingCredits, minAmount, maxAmount, userId);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<MemberCreditRecordModel>> GetMemberCreditRecordQueryableAsync(string? filter,
        DateTime? usageTimeFrom, DateTime? usageTimeTo, DateTime? expirationTimeFrom, DateTime? expirationTimeTo,
        int? minRemainingCredits, int? maxRemainingCredits, int? minAmount, int? maxAmount, Guid? userId)
    {
        var dbContext = await GetPikachuDbContextAsync();

        var memberCredits = (from credits in dbContext.UserShoppingCredits
                            .Where(x => userId != null && x.UserId == userId)
                            join orders in dbContext.Orders
                            on credits.Id equals orders.CreditDeductionRecordId
                            into creditsWithOrders
                            from orders in creditsWithOrders.DefaultIfEmpty()
                            select new { credits, orders })
                           //.Join(dbContext.Orders, credits => credits.Id, orders => orders.CreditDeductionRecordId, (credits, orders) => new { credits, orders })
                           .WhereIf(usageTimeFrom.HasValue, x => x.credits.CreationTime >= usageTimeFrom)
                           .WhereIf(usageTimeTo.HasValue, x => x.credits.CreationTime <= usageTimeTo)
                           .WhereIf(expirationTimeFrom.HasValue, x => x.credits.ExpirationDate >= expirationTimeFrom)
                           .WhereIf(expirationTimeTo.HasValue, x => x.credits.ExpirationDate <= expirationTimeTo)
                           .WhereIf(minRemainingCredits.HasValue, x => x.credits.CurrentRemainingCredits >= minRemainingCredits)
                           .WhereIf(maxRemainingCredits.HasValue, x => x.credits.CurrentRemainingCredits <= maxRemainingCredits)
                           .WhereIf(minAmount.HasValue, x => x.orders.CreditDeductionAmount >= minAmount)
                           .WhereIf(maxAmount.HasValue, x => x.orders.CreditDeductionAmount <= maxAmount)
                           .WhereIf(maxAmount.HasValue, x => x.orders.CreditDeductionAmount <= maxAmount)
                           .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.credits.TransactionDescription != null && x.credits.TransactionDescription.Contains(filter));

        return memberCredits
            .Select(x => new MemberCreditRecordModel
            {
                Id = x.credits.Id,
                UsageTime = x.credits.CreationTime,
                TransactionDescription = x.credits.TransactionDescription,
                Amount = x.credits.Amount,// x.orders != null ? x.orders.CreditDeductionAmount : 0,
                ExpirationDate = x.credits.ExpirationDate,
                RemainingCredits = x.credits.CurrentRemainingCredits,
                ShoppingCreditType = x.credits.ShoppingCreditType
            });
    }

    public async Task<List<IdentityUser>> GetBirthdayMember()
    {
        var dbContext = await GetPikachuDbContextAsync();

        var users = await dbContext.Users.IgnoreQueryFilters().ToListAsync(); // Fetch data into memory

        var nextMonth = DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1;

        var filteredUsers = users
            .Where(x => x.GetProperty(Constant.Birthday, null) != null &&
                        ((DateTime)x.GetProperty(Constant.Birthday, null)).Month == nextMonth)

            .ToList();

        return filteredUsers;


    }
}
