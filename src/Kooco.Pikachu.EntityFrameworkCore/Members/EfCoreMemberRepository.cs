﻿using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TierManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Kooco.Pikachu.Members;

public class EfCoreMemberRepository(IDbContextProvider<PikachuDbContext> pikachuDbContextProvider,
    IDbContextProvider<IIdentityDbContext> dbContextProvider) : EfCoreIdentityUserRepository(dbContextProvider), IMemberRepository
{
    private readonly List<ShippingStatus> CompletedShippingStatus = [ShippingStatus.Completed, ShippingStatus.Closed];

    protected virtual Task<PikachuDbContext> GetPikachuDbContextAsync()
    {
        return pikachuDbContextProvider.GetDbContextAsync();
    }

    public async Task<MemberModel> GetMemberAsync(Guid memberId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == memberId)
            ?? throw new EntityNotFoundException(typeof(IdentityUser), memberId);

        return new MemberModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Birthday = (DateTime?)user.GetProperty(Constant.Birthday, null),
            TotalOrders = dbContext.Orders.Where(x => x.UserId == user.Id).Count(),
            TotalSpent = (int)dbContext.Orders.Where(x => x.UserId == user.Id && (x.OrderStatus != EnumValues.OrderStatus.Exchange && x.OrderStatus != EnumValues.OrderStatus.Refund)).Sum(x => x.TotalAmount),
            MemberTags = [.. dbContext.MemberTags.AsNoTracking().Where(x => x.UserId == user.Id).Select(x => x.Name)]
        };
    }

    public async Task<long> GetCountAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, memberType, selectedMemberTags);
        return await queryable.LongCountAsync();
    }

    public async Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null)
    {
        var query = await GetFilteredQueryableAsync(filter, memberType, selectedMemberTags);
        var members = await query
                        .OrderBy(sorting)
                        .Skip(skipCount)
                        .Take(maxResultCount)
                        .ToListAsync();

        return members;
    }

    private async Task<IQueryable<MemberModel>> GetFilteredQueryableAsync(string? filter, string? memberType, IEnumerable<string>? selectedMemberTags = null)
    {
        selectedMemberTags ??= [];
        var dbContext = await GetPikachuDbContextAsync();
        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);
        var queryable = dbContext.Users
                    .Include(user => user.Roles)
                    .Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id))
                    .Select(user => new MemberModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Birthday = (DateTime?)user.GetProperty(Constant.Birthday, null),
                        TotalOrders = dbContext.Orders.Where(x => x.UserId == user.Id && CompletedShippingStatus.Contains(x.ShippingStatus)).Count(),
                        TotalSpent = (int)dbContext.Orders.Where(x => x.UserId == user.Id && CompletedShippingStatus.Contains(x.ShippingStatus)).Sum(x => x.TotalAmount),
                        LineId = EF.Property<string>(user, Constant.LineId),
                        GoogleId = EF.Property<string>(user, Constant.GoogleId),
                        FacebookId = EF.Property<string>(user, Constant.FacebookId),
                        MemberTags = dbContext.MemberTags.AsNoTracking().Where(x => x.UserId == user.Id).Select(x => x.Name).ToList(),
                    })
                    .WhereIf(!string.IsNullOrWhiteSpace(filter), member => member.MemberTags.Contains(filter) || member.UserName.Contains(filter)
                    || member.PhoneNumber.Contains(filter) || member.Email.Contains(filter))
                    .WhereIf(selectedMemberTags.Any(), member => selectedMemberTags.Any(tier => member.MemberTags.Contains(tier)))
                    .WhereIf(!string.IsNullOrWhiteSpace(memberType), member => member.MemberTags.Contains(memberType));

        return queryable;
    }

    public async Task<long> CountOrdersAsync(Guid memberId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        return await dbContext.Orders.Where(x => x.UserId == memberId).LongCountAsync();
    }

    public async Task<VipTier?> CheckForVipTierAsync(Guid userId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var vipTierSettings = await dbContext.VipTierSettings.Include(v => v.Tiers).FirstOrDefaultAsync();

        if (vipTierSettings == null) return default;

        var orders = dbContext.Orders.Where(o => o.UserId == userId && CompletedShippingStatus.Contains(o.ShippingStatus));

        var count = await orders.LongCountAsync();
        var amount = await orders.SumAsync(x => x.TotalAmount);

        var tiers = vipTierSettings.Tiers.OrderByDescending(tier => tier.Tier);

        return vipTierSettings switch
        {
            { BasedOnAmount: true, BasedOnCount: true, TierCondition: VipTierCondition.OnlyWhenReachedBoth }
                => tiers.FirstOrDefault(tier => tier.OrdersAmount <= amount && tier.OrdersCount <= count),

            { BasedOnAmount: true, BasedOnCount: true }
                => tiers.FirstOrDefault(tier => tier.OrdersAmount <= amount || tier.OrdersCount <= count),

            { BasedOnAmount: true }
                => tiers.FirstOrDefault(tier => tier.OrdersAmount <= amount),

            { BasedOnCount: true }
                => tiers.FirstOrDefault(tier => tier.OrdersCount <= count),

            _ => null
        };
    }

    public async Task UpdateMemberTierAsync()
    {
        var dbContext = await GetPikachuDbContextAsync();
        var vipTierSettings = await dbContext.VipTierSettings.Include(v => v.Tiers).FirstOrDefaultAsync();
        var tiers = vipTierSettings?.Tiers.OrderByDescending(tier => tier.Tier);

        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);

        var members = await dbContext.Users
            .Include(user => user.Roles)
            .Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id))
            .GroupJoin(dbContext.Orders,
            user => user.Id,
            order => order.UserId,
            (user, orders) => new { user, orders = orders.Where(o => CompletedShippingStatus.Contains(o.ShippingStatus)) })
            .Select(g => new
            {
                User = g.user,
                MemberTags = dbContext.MemberTags.Where(mt => mt.UserId == g.user.Id).ToList(),
                TotalOrders = g.orders.Count(),
                TotalSpent = g.orders.Sum(order => order.TotalAmount)
            })
            .ToListAsync();

        foreach (var member in members)
        {
            dbContext.MemberTags.RemoveRange(member.MemberTags.Where(mt => mt.Name != MemberConsts.MemberTags.Blacklisted));

            if (tiers.Any())
            {
                var tier = vipTierSettings switch
                {
                    { BasedOnAmount: true, BasedOnCount: true, TierCondition: VipTierCondition.OnlyWhenReachedBoth }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent && tier.OrdersCount <= member.TotalOrders),

                    { BasedOnAmount: true, BasedOnCount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent || tier.OrdersCount <= member.TotalOrders),

                    { BasedOnAmount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent),

                    { BasedOnCount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersCount <= member.TotalOrders),

                    _ => null
                };

                if (tier != null)
                {
                    dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, tier.TierName, tier.Id));
                }
            }

            if (member.TotalOrders == 0)
            {
                dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, MemberConsts.MemberTags.New));
            }
            else
            {
                dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, MemberConsts.MemberTags.Existing));
            }
        }
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
