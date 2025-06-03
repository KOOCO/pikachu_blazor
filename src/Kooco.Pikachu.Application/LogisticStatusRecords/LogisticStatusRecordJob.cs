using Hangfire;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.LogisticStatusRecords;

public class LogisticStatusRecordJob(TCatSFTPService tcatSftpService) : ITransientDependency
{
    [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task ExecuteAsync()
    {
        await tcatSftpService.ExecuteAsync();
    }
}