using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Reconciliations;

[AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 180, 180 })]
public class EcPayReconciliationJob : AsyncBackgroundJob<int>, ITransientDependency
{
    private readonly IEcPayReconciliationAppService _ecPayReconciliationAppService;
    private readonly IHostApplicationLifetime _hostApplicationLifeTime;

    public EcPayReconciliationJob(
        IEcPayReconciliationAppService ecPayReconciliationAppService,
        IHostApplicationLifetime hostApplicationLifetime
        )
    {
        _ecPayReconciliationAppService = ecPayReconciliationAppService;
        _hostApplicationLifeTime = hostApplicationLifetime;
    }

    public override async Task ExecuteAsync(int args)
    {
        try
        {
            var _watch = new Stopwatch();
            _watch.Start();

            Logger.LogInformation("Reconciliation Job: Starting");

            var records = await _ecPayReconciliationAppService.QueryMediaFileAsync(_hostApplicationLifeTime.ApplicationStopping);

            if (records == null || records.Count == 0)
            {
                Logger.LogWarning("Reconciliation Job: No reconciliation records found for the specified date range.");
            }
            else
            {
                Logger.LogInformation("Reconciliation Job: Found {count} records", records.Count);
            }

            _watch.Stop();
            Logger.LogInformation("Reconciliation Job: Finishing...");

            var elapsed = _watch.Elapsed;
            Logger.LogInformation("Reconciliation Job: Elapsed Time: {elapsedSeconds} seconds ({elapsedFormatted})", elapsed.TotalSeconds, elapsed.ToString(@"hh\:mm\:ss"));
        }
        catch (Exception ex)
        {
            Logger.LogError("Reconciliation Job: Exception thrown");
            Logger.LogException(ex);
            throw;
        }
    }
}
