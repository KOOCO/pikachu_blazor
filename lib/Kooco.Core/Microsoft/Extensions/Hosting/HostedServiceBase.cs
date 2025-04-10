namespace Microsoft.Extensions.Hosting;
public abstract class HostedServiceBase<TEntity>(int seconds = 10) : BackgroundService
{
    readonly Dictionary<Type, SemaphoreSlim> _semaphores = [];
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var entityType = typeof(TEntity);
        _semaphores[entityType] = new SemaphoreSlim(1, 1);
        while (await new PeriodicTimer(TimeSpan
            .FromSeconds(seconds))
            .WaitForNextTickAsync(ct)
            .ConfigureAwait(false))
        {
            try
            {
                await _semaphores[entityType].WaitAsync(ct).ConfigureAwait(false);
                await ExecutionAsync(ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hosting service to {typeof(TEntity).Name}, {e.Message}");
            }
            finally
            {
                _semaphores[entityType].Release();
            }
        }
    }
    protected abstract Task ExecutionAsync(CancellationToken ct);
}