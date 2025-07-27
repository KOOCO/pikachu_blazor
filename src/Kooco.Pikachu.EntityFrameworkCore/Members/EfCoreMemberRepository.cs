using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Members.MemberTags;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.TierManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Kooco.Pikachu.Members;

public class EfCoreMemberRepository(IDbContextProvider<PikachuDbContext> pikachuDbContextProvider,
    IDbContextProvider<IIdentityDbContext> dbContextProvider) : EfCoreIdentityUserRepository(dbContextProvider), IMemberRepository
{

    protected virtual Task<PikachuDbContext> GetPikachuDbContextAsync()
    {
        return pikachuDbContextProvider.GetDbContextAsync();
    }

    public async Task<MemberModel> GetMemberAsync(Guid memberId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);
        var user = await dbContext.Users.Include(x => x.Roles).Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id)).FirstOrDefaultAsync(m => m.Id == memberId)
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
            MemberTags = [.. dbContext.MemberTags.AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .OrderBy(tag => !MemberConsts.MemberTags.Names.Contains(tag.Name))
                .ThenBy(tag => !tag.VipTierId.HasValue)
                .ThenBy(tag => tag.Name)]
        };
    }

    public async Task<MemberModel> FindMemberByEmailAsync(string Email)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);
        var user = await dbContext.Users.Include(x => x.Roles).Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id)).FirstOrDefaultAsync(m => m.NormalizedEmail == Email.ToUpper() || m.NormalizedUserName == Email.ToUpper())
            ?? throw new EntityNotFoundException("Member Not Found");

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
            MemberTags = [.. dbContext.MemberTags.AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .OrderBy(tag => !MemberConsts.MemberTags.Names.Contains(tag.Name))
                .ThenBy(tag => !tag.VipTierId.HasValue)
                .ThenBy(tag => tag.Name)]
        };
    }

    public async Task<long> GetCountAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null,
        DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minOrderCount = null, int? maxOrderCount = null,
        int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, memberType, selectedMemberTags, minCreationTime, maxCreationTime,
            minOrderCount, maxOrderCount, minSpent, maxSpent, isSystemAssigned);

        return await queryable.LongCountAsync();
    }

    public async Task<List<MemberModel>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter = null, string? memberType = null,
        IEnumerable<string>? selectedMemberTags = null, DateTime? minCreationTime = null, DateTime? maxCreationTime = null,
        int? minOrderCount = null, int? maxOrderCount = null, int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null)
    {
        var query = await GetFilteredQueryableAsync(filter, memberType, selectedMemberTags, minCreationTime, maxCreationTime,
            minOrderCount, maxOrderCount, minSpent, maxSpent, isSystemAssigned);

        var members = await query
                        .OrderBy(sorting)
                        .Skip(skipCount)
                        .Take(maxResultCount)
                        .ToListAsync();

        return members;
    }

    public async Task<IQueryable<MemberModel>> GetFilteredQueryableAsync(string? filter = null, string? memberType = null, IEnumerable<string>? selectedMemberTags = null,
        DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minOrderCount = null, int? maxOrderCount = null,
        int? minSpent = null, int? maxSpent = null, bool? isSystemAssigned = null, IEnumerable<string>? selectedMemberTypes = null)
    {
        selectedMemberTags ??= [];
        selectedMemberTypes ??= [];
        minCreationTime = minCreationTime?.Date;
        maxCreationTime = maxCreationTime?.Date;

        var dbContext = await GetPikachuDbContextAsync();
        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);
        var queryable = dbContext.Users
                    .AsNoTracking()
                    .Include(user => user.Roles)
                    .Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id))
                    .Select(user => new MemberModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        CreationTime = user.CreationTime,
                        Birthday = (DateTime?)user.GetProperty(Constant.Birthday, null),
                        TotalOrders = dbContext.Orders.Where(x => x.UserId == user.Id && OrderConsts.CompletedShippingStatus.Contains(x.ShippingStatus)).Count(),
                        TotalSpent = (int)dbContext.Orders.Where(x => x.UserId == user.Id && OrderConsts.CompletedShippingStatus.Contains(x.ShippingStatus)).Sum(x => x.TotalAmount),
                        LineId = EF.Property<string>(user, Constant.LineId),
                        GoogleId = EF.Property<string>(user, Constant.GoogleId),
                        FacebookId = EF.Property<string>(user, Constant.FacebookId),
                        MemberTags = dbContext.MemberTags.AsNoTracking()
                            .Where(x => x.UserId == user.Id && (!isSystemAssigned.HasValue || x.IsSystemAssigned == isSystemAssigned))
                            .OrderBy(tag => !MemberConsts.MemberTags.Names.Contains(tag.Name))
                            .ThenBy(tag => !tag.VipTierId.HasValue)
                            .ThenBy(tag => tag.Name)
                            .ToList(),
                    })
                    .WhereIf(!string.IsNullOrWhiteSpace(filter), member => member.MemberTags.Any(tag => tag.Name == filter) || member.UserName.Contains(filter)
                    || member.PhoneNumber.Contains(filter) || member.Email.Contains(filter))
                    .WhereIf(selectedMemberTags.Any(), member => selectedMemberTags.Any(tier => member.MemberTags.Any(tag => tag.Name == tier)))
                    .WhereIf(!string.IsNullOrWhiteSpace(memberType), member => member.MemberTags.Any(tag => tag.Name == memberType))
                    .WhereIf(minCreationTime.HasValue, member => member.CreationTime.Date >= minCreationTime)
                    .WhereIf(maxCreationTime.HasValue, member => member.CreationTime.Date <= maxCreationTime)
                    .WhereIf(minOrderCount.HasValue, member => member.TotalOrders >= minOrderCount)
                    .WhereIf(maxOrderCount.HasValue, member => member.TotalOrders <= maxOrderCount)
                    .WhereIf(minSpent.HasValue, member => member.TotalSpent >= minSpent)
                    .WhereIf(maxSpent.HasValue, member => member.TotalSpent <= maxSpent)
                    .WhereIf(selectedMemberTypes.Any(), member => selectedMemberTypes.Any(type => member.MemberTags.Any(tag => tag.Name == type)));

        return queryable;
    }

    public async Task<long> CountOrdersAsync(Guid memberId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        return await dbContext.Orders
            .Where(x => x.UserId == memberId && OrderConsts.CompletedShippingStatus.Contains(x.ShippingStatus))
            .LongCountAsync();
    }

    public async Task<VipTier?> CheckForVipTierAsync(Guid userId)
    {
        var dbContext = await GetPikachuDbContextAsync();
        var vipTierSettings = await dbContext.VipTierSettings.Include(v => v.Tiers).FirstOrDefaultAsync();

        if (vipTierSettings == null) return default;

        var orders = dbContext.Orders.Where(o => o.UserId == userId && OrderConsts.CompletedShippingStatus.Contains(o.ShippingStatus));

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
        Logger.LogInformation("Member Tier Job: Starting Job");
        var dbContext = await GetPikachuDbContextAsync();
        var vipTierSettings = await dbContext.VipTierSettings.Include(v => v.Tiers).FirstOrDefaultAsync();
        if (vipTierSettings == null)
        {
            Logger.LogWarning("Member Tier Job: Vip Tier Settings are not set");
            return;
        }
        
        vipTierSettings.LastResetDateUtc = DateTime.UtcNow;
        Logger.LogInformation("Member Tier Job: Last Reset Date set to {LastResetDateUtc}", vipTierSettings.LastResetDateUtc);
        
        var tiers = vipTierSettings.Tiers.OrderByDescending(tier => tier.Tier);

        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == MemberConsts.Role);

        var members = await dbContext.Users
            .Include(user => user.Roles)
            .Where(user => memberRole != null && user.Roles.Any(role => role.RoleId == memberRole.Id))
            .GroupJoin(dbContext.Orders,
            user => user.Id,
            order => order.UserId,
            (user, orders) => new
            {
                user,
                orders = orders.Where(o => OrderConsts.CompletedShippingStatus.Contains(o.ShippingStatus))
            })
            .Select(g => new
            {
                User = g.user,
                MemberTags = dbContext.MemberTags.Where(mt => mt.UserId == g.user.Id).ToList(),
                TotalOrders = g.orders.Count(),
                OrdersAfterStartDate = g.orders.Where(o => o.CreationTime.Date >= vipTierSettings.StartDate.Date).Count(),
                TotalSpent = g.orders.Sum(order => order.TotalAmount)
            })
            .ToListAsync();

        foreach (var member in members)
        {
            var tagsToDelete = member.MemberTags.Where(mt => mt.IsSystemAssigned && mt.Name != MemberConsts.MemberTags.Blacklisted);
            dbContext.MemberTags.RemoveRange(tagsToDelete);

            if (tiers.Any())
            {
                var tier = vipTierSettings switch
                {
                    { BasedOnAmount: true, BasedOnCount: true, TierCondition: VipTierCondition.OnlyWhenReachedBoth }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent && tier.OrdersCount <= member.OrdersAfterStartDate),

                    { BasedOnAmount: true, BasedOnCount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent || tier.OrdersCount <= member.OrdersAfterStartDate),

                    { BasedOnAmount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersAmount <= member.TotalSpent),

                    { BasedOnCount: true }
                        => tiers.FirstOrDefault(tier => tier.OrdersCount <= member.OrdersAfterStartDate),

                    _ => null
                };

                if (tier != null)
                {
                    dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, tier.TierName, true, true, tier.Id));
                }
            }

            if (member.TotalOrders == 0)
            {
                dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, MemberConsts.MemberTags.New, true, true));
            }
            else
            {
                dbContext.MemberTags.Add(new MemberTag(GuidGenerator.Create(), member.User.Id, MemberConsts.MemberTags.Existing, true, true));
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
                ShoppingCreditType = x.credits.ShoppingCreditType,
                OrderNo = x.credits.OrderNo
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

    public async Task<List<(Guid id, string name, string email)>> GetEdmMemberNameAndEmailAsync(bool applyToAllMembers, IEnumerable<string> memberTags)
    {
        var dbContext = await GetPikachuDbContextAsync();

        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(role => role.Name == MemberConsts.Role);

        if (memberRole == null)
        {
            return [];
        }

        var users = await dbContext.Users
            .Include(user => user.Roles)
            .Where(user => user.Roles.Any(role => role.RoleId == memberRole.Id))
            .GroupJoin(dbContext.MemberTags,
            user => user.Id,
            memberTags => memberTags.UserId,
            (user, memberTags) => new { User = user, MemberTags = memberTags })
            .WhereIf(!applyToAllMembers, user => user.MemberTags.Any(tag => memberTags.Contains(tag.Name)))
            .Select(user => new { user.User.Id, user.User.UserName, user.User.NormalizedEmail })
            .ToListAsync();

        return users.Select(user => (user.Id, user.UserName, user.NormalizedEmail)).ToList();
    }
}
