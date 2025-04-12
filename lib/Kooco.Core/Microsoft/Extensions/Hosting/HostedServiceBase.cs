using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Data;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Microsoft.Extensions.Hosting;
public abstract class HostedServiceBase<TDerived>(TimeSpan period = default, bool isTransactional = true) :
    BackgroundService where TDerived : class
{
    readonly string _name = typeof(TDerived).Name;
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        ct.Register(() => Logger.LogInformation("{ServiceName} stopping.", _name));
        Logger.LogInformation("{ServiceName} starting.", _name);
        using PeriodicTimer timer = new(period == default ? TimeSpan.FromSeconds(10) : period);
        try
        {
            while (await timer.WaitForNextTickAsync(ct).ConfigureAwait(false))
            {
                await using var scope = ServiceScopeFactory.CreateAsyncScope();
                var serviceProvider = scope.ServiceProvider;

                var uowManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
                using var uow = uowManager.Begin(new AbpUnitOfWorkOptions
                {
                    IsTransactional = isTransactional,
                    IsolationLevel = IsolationLevel.ReadCommitted
                }, requiresNew: true);

                try
                {
                    await ExecutionAsync(serviceProvider, ct).ConfigureAwait(false);
                    await uow.CompleteAsync(ct).ConfigureAwait(false);
                    Logger.LogDebug("{ServiceName} iteration completed successfully.", _name);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    Logger.LogWarning("{ServiceName} execution cancelled.", _name);
                    await uow.RollbackAsync(ct);
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Unhandled exception during {ServiceName} execution.", _name);
                    await uow.RollbackAsync(ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("{ServiceName} caught cancellation during timer wait.", _name);
        }
        catch (Exception e)
        {
            Logger.LogCritical(e, "{ServiceName} encountered a critical error in the main loop.", _name);
        }
        Logger.LogInformation("{ServiceName} has stopped.", _name);
    }
    protected abstract Task ExecutionAsync(IServiceProvider provider, CancellationToken ct);
    public required IClock Clock { get; init; }
    public required ILogger<TDerived> Logger { get; init; }
    public required IObjectMapper ObjectMapper { get; init; }
    public required IGuidGenerator GuidGenerator { get; init; }
    public required ISettingProvider SettingProvider { get; init; }
    public required IServiceScopeFactory ServiceScopeFactory { get; init; }
    public required IStringLocalizerFactory StringLocalizerFactory { get; init; }
}