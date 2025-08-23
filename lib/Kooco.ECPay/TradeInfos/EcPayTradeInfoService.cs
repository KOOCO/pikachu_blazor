using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using Volo.Abp.DependencyInjection;
using static Kooco.TradeInfos.EcPayTradeInfoHelper;
namespace Kooco.TradeInfos;

public class EcPayTradeInfoService : IEcPayTradeInfoService, ITransientDependency
{
    private readonly EcPayHttpOptions _options;
    private readonly ILogger<EcPayTradeInfoService> _logger;

    public EcPayTradeInfoService(
        IOptions<EcPayHttpOptions> options,
        ILogger<EcPayTradeInfoService> logger
        )
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    /// <summary>
    /// DOC: https://developers.ecpay.com.tw/?p=7418
    /// </summary>
    public async Task<List<EcPayTradeInfoResponse>> QueryTradeInfoAsync(
            List<string> merchantTradeNos,
            CancellationToken cancellationToken = default
        )
    {
        _logger.LogInformation("COD Trade Info: Starting query trade info job");

        if (merchantTradeNos == null || merchantTradeNos.Count == 0)
        {
            _logger.LogWarning("COD Trade Info: No MerchantTradeNos provided for EcPay trade info query.");
            return [];
        }

        _logger.LogInformation("COD Trade Info: Starting query trade info for {count} orders", merchantTradeNos.Count);

        var client = new RestClient(_options.CodQueryTradeInfoUrl);

        var results = new List<EcPayTradeInfoResponse>();

        foreach (var tradeNo in merchantTradeNos)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            var payload = new Dictionary<string, string>
            {
                ["MerchantID"] = _options.MerchantID,
                ["MerchantTradeNo"] = tradeNo,
                ["TimeStamp"] = timestamp
            };

            _logger.LogInformation("COD Trade Info: Starting query for merchant id: {merchantId}, trade no: {tradeNo} and timestamp: {timestamp}", _options.MerchantID, tradeNo, timestamp);

            payload["CheckMacValue"] = EcPayCheckMacValue.ForTradeInfo(payload, _options);

            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            foreach (var pair in payload)
            {
                request.AddParameter(pair.Key, pair.Value);
            }

            var response = await client.ExecuteAsync(request, cancellationToken);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content) || response.Content.Contains("Parameter Error"))
            {
                _logger.LogError("COD Trade Info: Failed to query trade info for {tradeNo}. Response: {response}", tradeNo, response.Content);
                continue;
            }

            _logger.LogInformation("COD Trade Info: Fetched success response for {tradeNo}. Response: {response}", tradeNo, response.Content);

            var parsedData = ParseResponse(response.Content);

            var checkMacValue = EcPayCheckMacValue.ForTradeInfo(parsedData, _options);
            var tradeInfo = MapToTradeInfo(parsedData);

            if (tradeInfo.CheckMacValue != checkMacValue)
            {
                _logger.LogError("COD Trade Info: Check mac value invalid for {tradeNo}. Received {received} and generated {generated}", tradeNo, tradeInfo.CheckMacValue, checkMacValue);
            }

            results.Add(tradeInfo);
        }

        return results;
    }
}
