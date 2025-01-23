using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Users;

namespace Kooco.Pikachu.BackgroundWorkers
{
    public class PassiveUserBirthdayCheckerWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly ICurrentTenant _currentTenant;
        public PassiveUserBirthdayCheckerWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
         ICurrentTenant currentTenant // Inject ICurrentTenant
    ) : base(timer, serviceScopeFactory)
        {
            // Set the initial timer period
            Timer.Period = CalculateNextRunInterval();
            _currentTenant = currentTenant;
          
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var now = DateTime.Now;

            // Ensure the worker runs only at 3 AM on the last day of the month
            if (now.Day == DateTime.DaysInMonth(now.Year, now.Month) && now.Hour == 3)
            {
                Logger.LogInformation("Starting: getting users whos birthday next month...");

                // Resolve dependencies
                var userRepository = workerContext
                    .ServiceProvider
                    .GetRequiredService<IMemberRepository>();
                var userShoppingCreditAppService = workerContext
                  .ServiceProvider
                  .GetRequiredService<IUserShoppingCreditAppService>();
                var shoppingCreditEarnSettingAppService = workerContext
                  .ServiceProvider
                  .GetRequiredService<IShoppingCreditEarnSettingAppService>();
                var userCumulativeCreditAppService = workerContext
                 .ServiceProvider
                 .GetRequiredService<IUserCumulativeCreditAppService>();
                var userCumulativeCreditRepository = workerContext
               .ServiceProvider
               .GetRequiredService<IUserCumulativeCreditRepository>();

                var shoppingCredit = await shoppingCreditEarnSettingAppService.GetFirstAsync();

                if (shoppingCredit.BirthdayBonusEnabled)
                {
                    // Perform the work
                    var members = await userRepository.GetBirthdayMember();
                    foreach (var member in members)
                    {
                        using (_currentTenant.Change(member.TenantId))
                        {
                            await userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
                            {
                                UserId = member.Id,
                                Amount = shoppingCredit.BirthdayEarnedPoints,
                                ExpirationDate = shoppingCredit.BirthdayUsagePeriodType == "NoExpiry" ? null : DateTime.Now.AddDays(shoppingCredit.BirthdayValidDays),
                                IsActive = shoppingCredit.BirthdayBonusEnabled,
                                TransactionDescription = "獲得生日禮金",





                            });
                            var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == member.Id);
                            if (userCumulativeCredit is null)
                            {
                                await userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = shoppingCredit.BirthdayEarnedPoints, TotalDeductions = 0, TotalRefunds = 0, UserId = member.Id });


                            }
                            else
                            {
                                userCumulativeCredit.ChangeTotalAmount((int)(userCumulativeCredit.TotalAmount + shoppingCredit.BirthdayEarnedPoints));
                                await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
                            }
                        }

                    }
                }
                Logger.LogInformation("Starting: getting users whos birthday next month...");
            }

            // Recalculate the timer period for the next run
            Timer.Period = CalculateNextRunInterval();
        }

        private int CalculateNextRunInterval()
        {
            var now = DateTime.Now;

            // Determine the next 3 AM on the last day of the month
            var currentMonthLastDay = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            var nextRunDate = currentMonthLastDay.AddHours(3);

            // If it's already past 3 AM on the last day, schedule for the next month
            if (now >= nextRunDate)
            {
                var nextMonth = now.Month == 12 ? 1 : now.Month + 1;
                var nextYear = now.Month == 12 ? now.Year + 1 : now.Year;
                nextRunDate = new DateTime(nextYear, nextMonth, DateTime.DaysInMonth(nextYear, nextMonth)).AddHours(3);
            }

            // Calculate the interval in milliseconds
            var time= (nextRunDate - now).TotalMilliseconds;
            if (time < 0)
            {
                return (int)time*-1;

            }
            else {

                return (int)time;
            }
        }
    }
}
