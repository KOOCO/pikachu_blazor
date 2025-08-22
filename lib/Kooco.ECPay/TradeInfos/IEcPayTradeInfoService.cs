
namespace Kooco.TradeInfos;

public interface IEcPayTradeInfoService
{
    Task<List<EcPayTradeInfoResponse>> QueryTradeInfoAsync(EcPayTradeInfoInput input, CancellationToken cancellationToken = default);
}