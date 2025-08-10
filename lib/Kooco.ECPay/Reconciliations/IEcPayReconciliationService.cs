namespace Kooco.Reconciliations;

public interface IEcPayReconciliationService
{
    Task<List<EcPayReconciliationResponse>> QueryMediaFileAsync(
        EcPayReconciliationInput input,
        CancellationToken cancellationToken = default
        );
}
