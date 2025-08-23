
namespace Kooco.TradeInfos;

public interface IEcPayTradeInfoService
{
    Task<List<EcPayTradeInfoResponse>> QueryTradeInfoAsync(
             List<string> merchantTradeNos,
             CancellationToken cancellationToken = default
         );
}