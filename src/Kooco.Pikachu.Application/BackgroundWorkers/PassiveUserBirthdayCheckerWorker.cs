using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.BackgroundWorkers;
public class PassiveUserBirthdayCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    private readonly ICurrentTenant _currentTenant;

    public PassiveUserBirthdayCheckerWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentTenant currentTenant
    ) : base(timer, serviceScopeFactory)
    {
        _currentTenant = currentTenant;
        Timer.Period = CalculateNextRunInterval();
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var now = DateTime.Now;
        Logger.LogInformation("[BirthdayBonus] Multi-tenant run started at {Time}", now);

        var sp = workerContext.ServiceProvider;
        var dataFilter = sp.GetRequiredService<IDataFilter>();
        var tenantRepository = sp.GetRequiredService<ITenantRepository>();
        var unitOfWorkManager = sp.GetRequiredService<IUnitOfWorkManager>();

        var currentMonth = now.Month;
        int totalGrantedCount = 0;

        // Get all tenants first (outside of any tenant context)
        List<Tenant> allTenants;
        using (dataFilter.Disable<IMultiTenant>())
        {
            allTenants = await tenantRepository.GetListAsync();
        }

        Logger.LogInformation("[BirthdayBonus] Processing {TenantCount} tenants", allTenants.Count);

        foreach (var tenant in allTenants)
        {
            try
            {
                // Process each tenant with its own Unit of Work
                using var uow = unitOfWorkManager.Begin(new AbpUnitOfWorkOptions
                {
                    IsTransactional = true
                });

                using (_currentTenant.Change(tenant.Id))
                {
                    Logger.LogInformation("[BirthdayBonus] Processing tenant: {TenantName} (ID: {TenantId})",
                        tenant.Name, tenant.Id);

                    var tenantGrantedCount = await ProcessSingleTenantAsync(sp, now, currentMonth, tenant);
                    totalGrantedCount += tenantGrantedCount;

                    await uow.CompleteAsync();

                    Logger.LogInformation("[BirthdayBonus] Tenant {TenantName}: Granted {Count} bonuses",
                        tenant.Name, tenantGrantedCount);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[BirthdayBonus] Error processing tenant {TenantName} (ID: {TenantId})",
                    tenant.Name, tenant.Id);
            }
        }

        Logger.LogInformation("[BirthdayBonus] Multi-tenant run finished. Total granted: {TotalCount} bonuses for {Year}-{Month}",
            totalGrantedCount, now.Year, currentMonth);

        Timer.Period = CalculateNextRunInterval();
    }

    private async Task<int> ProcessSingleTenantAsync(IServiceProvider serviceProvider, DateTime now, int currentMonth, Tenant tenant)
    {
        // Resolve services within tenant context
        var memberRepository = serviceProvider.GetRequiredService<IMemberRepository>();
        var userShoppingCreditAppService = serviceProvider.GetRequiredService<IUserShoppingCreditAppService>();
        var shoppingCreditEarnSettingAppService = serviceProvider.GetRequiredService<IShoppingCreditEarnSettingAppService>();
        var userCumulativeCreditAppService = serviceProvider.GetRequiredService<IUserCumulativeCreditAppService>();
        var userCumulativeCreditRepository = serviceProvider.GetRequiredService<IUserCumulativeCreditRepository>();
        var userShoppingCreditRepository = serviceProvider.GetRequiredService<IUserShoppingCreditRepository>();

        // Get settings for this tenant
        var settings = await shoppingCreditEarnSettingAppService.GetFirstAsync();
        if (!(settings?.BirthdayBonusEnabled ?? false))
        {
            Logger.LogInformation("[BirthdayBonus] Birthday bonus disabled for tenant {TenantName}. Skipping.",
                tenant.Name);
            return 0;
        }

        // Get members with birthdays in current month for this tenant
        var members = await memberRepository.GetBirthdayMember();

        // Filter members based on current month
        members = members.Where(m =>
        {
            if (m.GetProperty(Constant.Birthday, null) == null) return false;
            var dob = (DateTime)m.GetProperty(Constant.Birthday, null);

            // Handle February 29 birthdays on non-leap years
            if (currentMonth == 2 && !DateTime.IsLeapYear(now.Year))
            {
                return dob.Month == 2 && (dob.Day <= 28 || dob.Day == 29);
            }

            return dob.Month == currentMonth;
        }).ToList();

        int tenantGrantedCount = 0;

        foreach (var member in members)
        {
            try
            {
                // Check if already granted this year for this member
                var alreadyGranted = await userShoppingCreditRepository.AnyAsync(x =>
                    x.UserId == member.Id &&
                    x.ShoppingCreditType == UserShoppingCreditType.Grant &&
                    x.TransactionDescription == "獲得生日禮金" &&
                    x.CreationTime.Year == now.Year
                );

                if (alreadyGranted)
                {
                    Logger.LogDebug("[BirthdayBonus] Member {MemberId} already received birthday bonus this year",
                        member.Id);
                    continue;
                }

                // Calculate expiry date
                DateTime? expiry = null;
                if (!string.Equals(settings.BirthdayUsagePeriodType, "NoExpiry", StringComparison.OrdinalIgnoreCase))
                {
                    expiry = now.Date.AddDays(settings.BirthdayValidDays);
                }

                // Record the credit
                await userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
                {
                    UserId = member.Id,
                    Amount = settings.BirthdayEarnedPoints,
                    ExpirationDate = expiry,
                    IsActive = settings.BirthdayBonusEnabled,
                    TransactionDescription = "獲得生日禮金",
                    ShoppingCreditType = UserShoppingCreditType.Grant
                });

                // Update or create cumulative totals
                var cum = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == member.Id);
                if (cum is null)
                {
                    await userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto
                    {
                        UserId = member.Id,
                        TotalAmount = settings.BirthdayEarnedPoints,
                        TotalDeductions = 0,
                        TotalRefunds = 0
                    });
                }
                else
                {
                    cum.ChangeTotalAmount((int)(cum.TotalAmount + settings.BirthdayEarnedPoints));
                    await userCumulativeCreditRepository.UpdateAsync(cum);
                }

                tenantGrantedCount++;

                Logger.LogDebug("[BirthdayBonus] Granted birthday bonus to member {MemberId} in tenant {TenantName}",
                    member.Id, tenant.Name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[BirthdayBonus] Error processing member {MemberId} in tenant {TenantName}",
                    member.Id, tenant.Name);
            }
        }

        return tenantGrantedCount;
    }


    private int CalculateNextRunInterval()
    {
        var now = DateTime.Now;

        // Two daily target times
        var todayMidnightRun = new DateTime(now.Year, now.Month, now.Day, 0, 10, 0);
        var todayEveningRun = new DateTime(now.Year, now.Month, now.Day, 23, 50, 0);

        DateTime nextRun;

        if (now < todayMidnightRun)
        {
            nextRun = todayMidnightRun; // before 00:10 → run today 00:10
        }
        else if (now < todayEveningRun)
        {
            nextRun = todayEveningRun; // before 23:50 → run today 23:50
        }
        else
        {
            // past both → schedule for tomorrow's 00:10
            nextRun = todayMidnightRun.AddDays(1);
        }

        var ms = (int)(nextRun - now).TotalMilliseconds;
        return ms > 0 ? ms : 1000 * 60; // fallback: 1 min
    }

}
//public class PassiveUserBirthdayCheckerWorker : AsyncPeriodicBackgroundWorkerBase
//{
//    private readonly ICurrentTenant _currentTenant;
//    public PassiveUserBirthdayCheckerWorker(
//    AbpAsyncTimer timer,
//    IServiceScopeFactory serviceScopeFactory,
//     ICurrentTenant currentTenant // Inject ICurrentTenant
//) : base(timer, serviceScopeFactory)
//    {
//        // Set the initial timer period
//        Timer.Period = CalculateNextRunInterval();
//        _currentTenant = currentTenant;

//    }

//    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
//    {
//        var now = DateTime.Now;

//        // Ensure the worker runs only at 3 AM on the last day of the month
//        if (now.Day == DateTime.DaysInMonth(now.Year, now.Month) && now.Hour == 3)
//        {
//            Logger.LogInformation("Starting: getting users whos birthday next month...");

//            // Resolve dependencies
//            var userRepository = workerContext
//                .ServiceProvider
//                .GetRequiredService<IMemberRepository>();
//            var userShoppingCreditAppService = workerContext
//              .ServiceProvider
//              .GetRequiredService<IUserShoppingCreditAppService>();
//            var shoppingCreditEarnSettingAppService = workerContext
//              .ServiceProvider
//              .GetRequiredService<IShoppingCreditEarnSettingAppService>();
//            var userCumulativeCreditAppService = workerContext
//             .ServiceProvider
//             .GetRequiredService<IUserCumulativeCreditAppService>();
//            var userCumulativeCreditRepository = workerContext
//           .ServiceProvider
//           .GetRequiredService<IUserCumulativeCreditRepository>();

//            var shoppingCredit = await shoppingCreditEarnSettingAppService.GetFirstAsync();

//            if (shoppingCredit.BirthdayBonusEnabled)
//            {
//                // Perform the work
//                var members = await userRepository.GetBirthdayMember();
//                foreach (var member in members)
//                {
//                    using (_currentTenant.Change(member.TenantId))
//                    {
//                        await userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
//                        {
//                            UserId = member.Id,
//                            Amount = shoppingCredit.BirthdayEarnedPoints,
//                            ExpirationDate = shoppingCredit.BirthdayUsagePeriodType == "NoExpiry" ? null : DateTime.Now.AddDays(shoppingCredit.BirthdayValidDays),
//                            IsActive = shoppingCredit.BirthdayBonusEnabled,
//                            TransactionDescription = "獲得生日禮金",
//                            ShoppingCreditType = UserShoppingCreditType.Grant
//                        });
//                        var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == member.Id);
//                        if (userCumulativeCredit is null)
//                        {
//                            await userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = shoppingCredit.BirthdayEarnedPoints, TotalDeductions = 0, TotalRefunds = 0, UserId = member.Id });
//                        }
//                        else
//                        {
//                            userCumulativeCredit.ChangeTotalAmount((int)(userCumulativeCredit.TotalAmount + shoppingCredit.BirthdayEarnedPoints));
//                            await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
//                        }
//                    }

//                }
//            }
//            Logger.LogInformation("Starting: getting users whos birthday next month...");
//        }

//        // Recalculate the timer period for the next run
//        Timer.Period = CalculateNextRunInterval();
//    }

//    private int CalculateNextRunInterval()
//    {
//        var now = DateTime.Now;

//        // Determine the next 3 AM on the last day of the month
//        var currentMonthLastDay = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
//        var nextRunDate = currentMonthLastDay.AddHours(3);

//        // If it's already past 3 AM on the last day, schedule for the next month
//        if (now >= nextRunDate)
//        {
//            var nextMonth = now.Month == 12 ? 1 : now.Month + 1;
//            var nextYear = now.Month == 12 ? now.Year + 1 : now.Year;
//            nextRunDate = new DateTime(nextYear, nextMonth, DateTime.DaysInMonth(nextYear, nextMonth)).AddHours(3);
//        }

//        // Calculate the interval in milliseconds
//        var time = (nextRunDate - now).TotalMilliseconds;
//        if (time < 0)
//        {
//            return (int)time * -1;

//        }
//        else
//        {

//            return (int)time;
//        }
//    }
//}
