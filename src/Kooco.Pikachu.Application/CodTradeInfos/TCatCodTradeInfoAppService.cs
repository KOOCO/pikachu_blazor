using Kooco.Pikachu.TenantPayouts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.CodTradeInfos;

public class TCatCodTradeInfoAppService : PikachuAppService, ITCatCodTradeInfoAppService
{
    private readonly TCatCodTradeInfoService _tcatCodTradeInfoService;
    private readonly ITCatCodTradeInfoRepository _tcatCodTradeInfoRepository;

    public TCatCodTradeInfoAppService(
        TCatCodTradeInfoService tcatCodTradeInfoService,
        ITCatCodTradeInfoRepository tCatCodTradeInfoRepository
        )
    {
        _tcatCodTradeInfoService = tcatCodTradeInfoService;
        _tcatCodTradeInfoRepository = tCatCodTradeInfoRepository;
    }

    public async Task ImportAsync(List<TCatCodTradeInfoRecordDto> input)
    {
        var records = ObjectMapper.Map<List<TCatCodTradeInfoRecordDto>, List<TCatCodTradeInfoRecord>>(input);
        await _tcatCodTradeInfoRepository.InsertManyAsync(records);

        var payoutRecords = input.Select(r => ObjectMapper.Map<TenantPayoutRecordDto, TenantPayoutRecord>(r.PayoutRecord)).ToList();
        await _tcatCodTradeInfoService.InsertPayoutRecordsAsync(payoutRecords);
    }

    public async Task<List<TCatCodTradeInfoRecordDto>> ProcessFile(string fileName, byte[] fileBytes)
    {
        var records = await _tcatCodTradeInfoService.ProcessFile(fileName, fileBytes);

        if (records.Count == 0) return [];

        var merchantTradeNos = records.Select(r => r.MerchantTradeNo).ToList();

        var existingRecords = await (await _tcatCodTradeInfoRepository.GetQueryableAsync())
            .Where(er => merchantTradeNos.Contains(er.MerchantTradeNo))
            .Select(er => er.MerchantTradeNo)
            .ToListAsync();

        return [.. records.Where(r => !existingRecords.Contains(r.MerchantTradeNo))];
    }
}
