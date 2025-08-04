using Hangfire;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace Kooco.Pikachu.TierManagement;

[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
public class UpdateMemberTierJob(
    IVipTierSettingAppService vipTierSettingAppService,
    ICancellationTokenProvider cancellationTokenProvider
    ) : AsyncBackgroundJob<UpdateMemberTierArgs>, ITransientDependency
{
    private readonly IVipTierSettingAppService _vipTierSettingAppService = vipTierSettingAppService;
    private readonly ICancellationTokenProvider _cancellationTokenProvider = cancellationTokenProvider;

    public override async Task ExecuteAsync(UpdateMemberTierArgs args)
    {
        var cancellationToken = _cancellationTokenProvider.Token;
        await _vipTierSettingAppService.UpdateMemberTierAsync(args.TenantId, args.ShouldConfigureRecurringJob, cancellationToken);
    }
}
