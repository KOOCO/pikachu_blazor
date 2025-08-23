using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.CodTradeInfos;

[AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 180, 180 })]
public class EcPayCodTradeInfoJob : AsyncBackgroundJob<int>, ITransientDependency
{
    private readonly IEcPayCodTradeInfoAppService _ecpayCodTradeInfoAppService;
    private readonly IHostApplicationLifetime _hostApplicationLifeTime;

    public EcPayCodTradeInfoJob(
        IEcPayCodTradeInfoAppService ecpayCodTradeInfoAppService,
        IHostApplicationLifetime hostApplicationLifetime
        )
    {
        _ecpayCodTradeInfoAppService = ecpayCodTradeInfoAppService;
        _hostApplicationLifeTime = hostApplicationLifetime;
    }

    public override async Task ExecuteAsync(int args)
    {
        try
        {
            var _watch = new Stopwatch();
            _watch.Start();

            Logger.LogInformation("COD Trade Info: Starting...");

            var records = await _ecpayCodTradeInfoAppService.QueryTradeInfoAsync(_hostApplicationLifeTime.ApplicationStopping);

            if (records == null || records.Count == 0)
            {
                Logger.LogWarning("COD Trade Info: No COD records found to update");
            }
            else
            {
                Logger.LogInformation("COD Trade Info: Found {count} records", records.Count);
            }

            _watch.Stop();
            Logger.LogInformation("COD Trade Info: Finishing..");

            var elapsed = _watch.Elapsed;
            Logger.LogInformation("COD Trade Info: Elapsed Time: {elapsedSeconds} seconds ({elapsedFormatted})", elapsed.TotalSeconds, elapsed.ToString(@"hh\:mm\:ss"));
        }
        catch (Exception ex)
        {
            Logger.LogError("COD Trade Info: Exception thrown");
            Logger.LogException(ex);
            throw;
        }
    }
}