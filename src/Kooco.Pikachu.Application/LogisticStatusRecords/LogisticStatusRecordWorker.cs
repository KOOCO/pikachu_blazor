using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Kooco.Pikachu.LogisticStatusRecords;

public class LogisticStatusRecordWorker : AsyncPeriodicBackgroundWorkerBase
{
    public LogisticStatusRecordWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
    {
        Timer.Period = (int)TimeSpan.FromHours(12).TotalSeconds;
    }

    protected override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        throw new NotImplementedException();
    }
}
