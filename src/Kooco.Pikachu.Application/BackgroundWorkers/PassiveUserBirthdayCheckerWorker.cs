using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

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
        Timer.Period = CalculateNextRunInterval(); // schedule next 3:00 AM
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var now = DateTime.Now;

        // Run exactly at 3 AM (daily)
        if (now.Hour != 3)
        {
            Timer.Period = CalculateNextRunInterval();
            return;
        }

        Logger.LogInformation("[BirthdayBonus] Run started at {Time}", now);

        // Resolve services
        var sp = workerContext.ServiceProvider;
        var memberRepository = sp.GetRequiredService<IMemberRepository>();
        var userShoppingCreditAppService = sp.GetRequiredService<IUserShoppingCreditAppService>();
        var shoppingCreditEarnSettingAppService = sp.GetRequiredService<IShoppingCreditEarnSettingAppService>();
        var userCumulativeCreditAppService = sp.GetRequiredService<IUserCumulativeCreditAppService>();
        var userCumulativeCreditRepository = sp.GetRequiredService<IUserCumulativeCreditRepository>();

        // (New) We’ll need read-access to credits to enforce once-per-year
        var userShoppingCreditRepository = sp.GetRequiredService<IUserShoppingCreditRepository>();

        var settings = await shoppingCreditEarnSettingAppService.GetFirstAsync();
        if (!(settings?.BirthdayBonusEnabled ?? false))
        {
            Logger.LogInformation("[BirthdayBonus] Disabled in settings. Skipping.");
            Timer.Period = CalculateNextRunInterval();
            return;
        }

        // Strategy change:
        // - Every day at 3AM, award birthday bonus to members whose birthday month == current month
        // - This catches users who register mid-month.
        // - Enforce "once per calendar year" per member.
        var currentMonth = now.Month;

        // You likely already have a query. Otherwise, here's a repository method you can expose:
        //   Task<List<Member>> GetMembersWithBirthdayInMonthAsync()
        // Make sure to handle NULL birthdays at the repo level.
        var members = await memberRepository.GetBirthdayMember();

        // Optional: handle Feb 29 birthdays on non-leap years (award on Feb 28)
        // If your repo handles this, remove the filter below.
        if (currentMonth == 2 && !DateTime.IsLeapYear(now.Year))
        {
            members = members.Where(m =>
            {
                if (m.GetProperty(Constant.Birthday, null) == null) return false;
                var dob = (DateTime)m.GetProperty(Constant.Birthday, null);
                return dob.Month == 2 && (dob.Day <= 28 || dob.Day == 29);
            }).ToList();
        }

        int grantedCount = 0;

        foreach (var member in members)
        {
            // guard
            if (member.GetProperty(Constant.Birthday, null) == null) continue;

            using (_currentTenant.Change(member.TenantId))
            {
                // Enforce once per year:
                // Mark “Birthday” rows by a clear discriminator. If you already store a reason/source,
                // prefer that field. Here we check by UserId + Reason/Description + Year.
                var alreadyGranted = await userShoppingCreditRepository.AnyAsync(x =>
                    x.UserId == member.Id &&
                    x.ShoppingCreditType == UserShoppingCreditType.Grant &&
                    x.TransactionDescription == "獲得生日禮金" &&
                    x.CreationTime.Year == now.Year
                );

                if (alreadyGranted)
                {
                    continue;
                }

                // Respect usage period setting
                DateTime? expiry = null;
                if (!string.Equals(settings.BirthdayUsagePeriodType, "NoExpiry", StringComparison.OrdinalIgnoreCase))
                {
                    // If your semantics are "valid N days from grant," this is fine.
                    // If semantics are different (e.g., end of birthday month), adjust here.
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

                grantedCount++;
            }
        }

        Logger.LogInformation("[BirthdayBonus] Finished. Granted {Count} bonuses for {Year}-{Month}.", grantedCount, now.Year, currentMonth);


        Timer.Period = CalculateNextRunInterval();
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
