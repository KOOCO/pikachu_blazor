using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Volo.Abp.DependencyInjection;
using static Kooco.TradeInfos.EcPayTradeInfoHelper;
namespace Kooco.TradeInfos;

public class EcPayTradeInfoService : IEcPayTradeInfoService, ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EcPayTradeInfoService> _logger;

    public EcPayTradeInfoService(
        IConfiguration configuration,
        ILogger<EcPayTradeInfoService> logger
        )
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// DOC: https://developers.ecpay.com.tw/?p=7418
    /// </summary>
    public async Task<List<EcPayTradeInfoResponse>> QueryTradeInfoAsync(
            EcPayTradeInfoInput input,
            CancellationToken cancellationToken = default
        )
    {
        var results = new List<EcPayTradeInfoResponse>();

        if (input.MerchantTradeNos == null || input.MerchantTradeNos.Count == 0)
        {
            _logger.LogWarning("No MerchantTradeNos provided for EcPay trade info query.");
            return results;
        }

        _logger.LogInformation("Starting query trade info for {count} orders", input.MerchantTradeNos.Count);

        var client = new RestClient(_configuration["EcPay:QueryTradeInfo"]!);

        foreach (var tradeNo in input.MerchantTradeNos)
        {
            var payload = new Dictionary<string, string>
            {
                ["MerchantID"] = input.MerchantID,
                ["MerchantTradeNo"] = tradeNo,
                ["TimeStamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
            };

            payload["CheckMacValue"] = EcPayCheckMacValue.ForTradeInfo(payload, input.HashKey, input.HashIV);

            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            foreach (var pair in payload)
            {
                request.AddParameter(pair.Key, pair.Value);
            }

            var response = await client.ExecuteAsync(request, cancellationToken);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content) || response.Content.Contains("Parameter Error"))
            {
                _logger.LogError("Failed to query trade info for {tradeNo}. Response: {response}", tradeNo, response.Content);
                continue;
            }
            //Verify CheckMacValue
            var parsedData = ParseResponse(response.Content);

            var tradeInfo = MapToTradeInfo(parsedData);

            results.Add(tradeInfo);
        }

        return results;
    }
}
