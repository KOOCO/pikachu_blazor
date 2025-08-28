using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.TenantPayouts;
using Kooco.TradeInfos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.CodTradeInfos;

public class EcPayCodTradeInfoAppService : PikachuAppService, IEcPayCodTradeInfoAppService
{
    private readonly IEcPayTradeInfoService _ecPayTradeInfoService;
    private readonly IEcPayCodTradeInfoRepository _ecPayCodTradeInfoRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly TenantPayoutRecordService _tenantPayoutRecordService;

    public EcPayCodTradeInfoAppService(
        IEcPayTradeInfoService ecPayTradeInfoService,
        IEcPayCodTradeInfoRepository ecPayCodTradeInfoRepository,
        IOrderRepository orderRepository,
        TenantPayoutRecordService tenantPayoutRecordService
        )
    {
        _ecPayTradeInfoService = ecPayTradeInfoService;
        _ecPayCodTradeInfoRepository = ecPayCodTradeInfoRepository;
        _orderRepository = orderRepository;
        _tenantPayoutRecordService = tenantPayoutRecordService;
    }

    [AllowAnonymous]
    public async Task<List<EcPayCodTradeInfoDto>> QueryTradeInfoAsync(CancellationToken cancellationToken = default)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var cutoffDate = DateTime.Today.AddDays(-15);

            var orderInfos = await _ecPayCodTradeInfoRepository
                .GetMerchantTradeNos(cutoffDate, cancellationToken);

            var merchantTradeNos = orderInfos.Select(m => m.MerchantTradeNo).ToList();

            Logger.LogInformation("COD Trade Info: Retrieved {count} trade records after cutoff date: {date} that aren't already reconciled.", merchantTradeNos.Count, cutoffDate);

            var response = await _ecPayTradeInfoService.QueryTradeInfoAsync(merchantTradeNos, cancellationToken);

            if (response == null || response.Count == 0)
            {
                Logger.LogInformation("COD Trade Info: No records found");
                return [];
            }

            var responseTradeNos = response
                .Where(r => r.CollectionAllocateDate.HasValue)
                .Select(r => r.MerchantTradeNo);

            var orderIds = orderInfos
                .Where(m => responseTradeNos.Contains(m.MerchantTradeNo))
                .Select(m => m.OrderId)
                .Distinct()
                .ToList();

            var orders = await _orderRepository.GetListAsync(o => orderIds.Contains(o.Id), cancellationToken: cancellationToken);

            var inputRecords = new List<EcPayCodTradeInfoRecord>();
            var ordersToUpdate = new List<Order>();

            foreach (var item in response)
            {
                if (!item.CollectionAllocateDate.HasValue)
                {
                    Logger.LogInformation("COD Trade Info: Skipping MerchantTradeNo: {MerchantTradeNo}. CollectionAllocateDate is null", item.MerchantTradeNo);
                    continue;
                }

                var orderInfo = orderInfos.FirstOrDefault(m => m.MerchantTradeNo == item.MerchantTradeNo);
                if (orderInfo == default)
                {
                    Logger.LogWarning("COD Trade Info: Order Info not found for MerchantTradeNo: {MerchantTradeNo}", item.MerchantTradeNo);
                    continue;
                }
                var order = orders.FirstOrDefault(o => o.Id == orderInfo.OrderId);
                if (order == default)
                {
                    Logger.LogWarning("COD Trade Info: Order not found for MerchantTradeNo: {MerchantTradeNo}", item.MerchantTradeNo);
                    continue;
                }

                var inputRecord = ObjectMapper.Map<EcPayTradeInfoResponse, EcPayCodTradeInfoRecord>(item);
                inputRecord.TenantId = order.TenantId;
                inputRecord.OrderId = order.Id;
                inputRecord.OrderNo = order.OrderNo;
                inputRecord.CreationTime = Clock.Now;
                inputRecords.Add(inputRecord);

                order.EcPayNetAmount = item.CollectionAllocateAmount;
                ordersToUpdate.Add(order);
            }

            await _ecPayCodTradeInfoRepository.InsertManyAsync(inputRecords, cancellationToken: cancellationToken);
            Logger.LogInformation("COD Trade Info: Insert {count} records into EcPayCodTradeInfoRecords", inputRecords.Count);

            await _orderRepository.UpdateManyAsync(ordersToUpdate, cancellationToken: cancellationToken);
            Logger.LogInformation("COD Trade Info: Updated {count} orders", ordersToUpdate.Count);

            var results = ObjectMapper.Map<List<EcPayCodTradeInfoRecord>, List<EcPayCodTradeInfoDto>>(inputRecords);

            await _tenantPayoutRecordService.CreateTenantCodPayouts(results, orders, PaymentFeeType.EcPay);

            return results;
        }
    }
}
