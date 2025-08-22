using Kooco.Pikachu.PaymentGateways;
using Kooco.TradeInfos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kooco.Pikachu.CodTradeInfos;

public class EcPayCodTradeInfoAppService : PikachuAppService, IEcPayCodTradeInfoAppService
{
    private readonly IEcPayTradeInfoService _ecPayTradeInfoService;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;
    private readonly IEcPayCodTradeInfoRepository _ecPayCodTradeInfoRepository;
    public EcPayCodTradeInfoAppService(
        IPaymentGatewayAppService paymentGatewayAppService,
        IEcPayTradeInfoService ecPayTradeInfoService,
        IEcPayCodTradeInfoRepository ecPayCodTradeInfoRepository
        )
    {
        _paymentGatewayAppService = paymentGatewayAppService;
        _ecPayTradeInfoService = ecPayTradeInfoService;
        _ecPayCodTradeInfoRepository = ecPayCodTradeInfoRepository;
    }

    [AllowAnonymous]
    public async Task<List<EcPayCodTradeInfoDto>> QueryTradeInfoAsync(CancellationToken cancellationToken = default)
    {
        var ecPayConfigs = await _paymentGatewayAppService.GetAllEcPayAsync(true);
        if (ecPayConfigs == null || ecPayConfigs.Count == 0)
        {
            Logger.LogWarning("Reconciliation Job: EcPay Configuration not found.");
            return [];
        }

        List<EcPayCodTradeInfoRecord> results = [];

        foreach (var ecpay in ecPayConfigs)
        {
            var response = await _ecPayTradeInfoService.QueryTradeInfoAsync(new EcPayTradeInfoInput
            {
                HashIV = "jggHhdCBky7tPFk6",
                HashKey = "jxrMfff0dQml5Zhn",
                MerchantID = "3087335",
                MerchantTradeNos = ["5075BC128F0896887", "6C4A11ACE0A707537", "6C5D557CB53942756"]
            }, cancellationToken);

            var inputRecords = new List<EcPayCodTradeInfoRecord>();

            foreach (var item in response)
            {
                if (!item.CollectionAllocateDate.HasValue)
                {
                    Logger.LogInformation("COD Trade Info: Skipping MerchantTradeNo: {MerchantTradeNo}. CollectionAllocateDate is null", item.MerchantTradeNo);
                    continue;
                }

                var inputRecord = ObjectMapper.Map<EcPayTradeInfoResponse, EcPayCodTradeInfoRecord>(item);
                inputRecord.TenantId = ecpay.TenantId;
                //input.OrderId = order.Id;
                //input.OrderNo = order.OrderNo;
                inputRecord.CreationTime = Clock.Now;
                inputRecords.Add(inputRecord);
            }

            //INSSERT into DB
            await _ecPayCodTradeInfoRepository.InsertManyAsync(inputRecords, cancellationToken: cancellationToken);
            results.AddRange(inputRecords);
        }

        return ObjectMapper.Map<List<EcPayCodTradeInfoRecord>, List<EcPayCodTradeInfoDto>>(results);
    }
}
