namespace Kooco.Reconciliations;

public interface IEcPayReconciliationService
{
    Task<List<EcPayReconciliationResponse>> QueryMediaFileAsync(
        CancellationToken cancellationToken = default
        );
}
