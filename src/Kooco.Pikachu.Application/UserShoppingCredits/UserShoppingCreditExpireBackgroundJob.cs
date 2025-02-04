using Hangfire;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.UserShoppingCredits;

[AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
public class UserShoppingCreditExpireBackgroundJob : AsyncBackgroundJob<int>, ITransientDependency
{
    private readonly IUserShoppingCreditAppService _userShoppingCreditAppService;

    public UserShoppingCreditExpireBackgroundJob(IUserShoppingCreditAppService userShoppingCreditAppService)
    {
        _userShoppingCreditAppService = userShoppingCreditAppService;
    }

    public override async Task ExecuteAsync(int args)
    {
        Logger.LogInformation("UserShoppingCreditExpireBackgroundJob starting..");
        var result = await _userShoppingCreditAppService.ExpireCreditsAsync();
        if (result != 1)
        {
            Logger.LogError("There was an error during the shopping credit expire worker.");
            return;
        }
        Logger.LogInformation("UserShoppingCreditExpireBackgroundJob finished..");
    }
}
