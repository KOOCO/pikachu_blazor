using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Kooco.Reconciliations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Reconciliations;

[Authorize(PikachuPermissions.EcPayReconciliations.Default)]
public class EcPayReconciliationAppService : PikachuAppService, IEcPayReconciliationAppService
{
    private readonly IEcPayReconciliationService _reconciliationService;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;
    private readonly IRepository<EcPayReconciliationRecord, Guid> _reconciliationRecordRepository;

    public EcPayReconciliationAppService(
        IEcPayReconciliationService reconciliationService,
        IOrderRepository orderRepository,
        IPaymentGatewayAppService paymentGatewayAppService,
        IRepository<EcPayReconciliationRecord, Guid> reconciliationRecordRepository
        )
    {
        _reconciliationService = reconciliationService;
        _orderRepository = orderRepository;
        _paymentGatewayAppService = paymentGatewayAppService;
        _reconciliationRecordRepository = reconciliationRecordRepository;
    }

    [AllowAnonymous]
    public async Task<List<EcPayReconciliationRecordDto>> QueryMediaFileAsync(CancellationToken cancellationToken = default)
    {
        var ecPayConfigs = await _paymentGatewayAppService.GetAllEcPayAsync(true);
        if (ecPayConfigs == null || ecPayConfigs.Count == 0)
        {
            Logger.LogWarning("Recociliation Job: EcPay Configuration not found.");
            return [];
        }

        List<EcPayReconciliationRecordDto> returnData = [];

        foreach (var ecPay in ecPayConfigs)
        {
            using (CurrentTenant.Change(ecPay.TenantId))
            {
                Logger.LogInformation("Recociliation Job: Running for tenant id: {tenantId}", ecPay.TenantId);
                Logger.LogInformation("Recociliation Job: Current tenant id: {tenantId} name: {name}", CurrentTenant.Id, CurrentTenant.Name);

                await Task.Delay(TimeSpan.FromSeconds(70), cancellationToken); //EcPay only allows to query one file per minute

                var input = new EcPayReconciliationInput
                {
                    HashKey = ecPay.HashKey!,
                    HashIV = ecPay.HashIV!,
                    MerchantID = ecPay.MerchantId!,
                    BeginDate = DateTime.Today.AddDays(-10),
                    EndDate = DateTime.Today.AddMilliseconds(-1)
                };

                var records = await _reconciliationService.QueryMediaFileAsync(input, cancellationToken);

                if (records == null || records.Count == 0)
                {
                    Logger.LogWarning("Recociliation Job: No reconciliation records found for the specified date range for {tenantName}.", CurrentTenant.Name);
                    continue;
                }

                var existingTradeNos = (await _reconciliationRecordRepository.GetQueryableAsync())
                    .Where(r => records.Select(rec => rec.MerchantTradeNo).Contains(r.MerchantTradeNo))
                    .Select(r => r.MerchantTradeNo)
                    .ToList();

                var orders = await _orderRepository.GetListAsync(o =>
                    records.Select(r => r.MerchantTradeNo).ToList().Contains(o.MerchantTradeNo)
                    && !existingTradeNos.Contains(o.MerchantTradeNo),
                    cancellationToken: cancellationToken
                    );

                var inputRecords = new List<EcPayReconciliationRecord>();

                foreach (var record in records)
                {
                    var order = orders.FirstOrDefault(o => o.MerchantTradeNo == record.MerchantTradeNo);

                    if (order == null)
                    {
                        Logger.LogWarning("Recociliation Job: No order found for MerchantTradeNo: {MerchantTradeNo}", record.MerchantTradeNo);
                        continue;
                    }

                    if (record.PaymentStatus == null || !record.PaymentStatus.Contains("已付款"))
                    {
                        Logger.LogInformation("Recociliation Job: Skipping merchant trade no: {merchantTradeNo} due to payment status: {paymentStatus}", record.MerchantTradeNo, record.PaymentStatus);
                        continue;
                    }

                    if (record.PayoutStatus == null || !record.PayoutStatus.Contains("已撥款"))
                    {
                        Logger.LogInformation("Recociliation Job: Skipping merchant trade no: {merchantTradeNo} due to payout status: {payoutStatus}", record.MerchantTradeNo, record.PayoutStatus);
                        continue;
                    }

                    var inputRecord = ObjectMapper.Map<EcPayReconciliationResponse, EcPayReconciliationRecord>(record);
                    inputRecord.OrderId = order.Id;
                    inputRecord.TenantId = order.TenantId;
                    order.EcPayNetAmount = record.NetAmount;

                    inputRecords.Add(inputRecord);
                }

                await _reconciliationRecordRepository.InsertManyAsync(inputRecords, cancellationToken: cancellationToken);
                returnData.AddRange(ObjectMapper.Map<List<EcPayReconciliationRecord>, List<EcPayReconciliationRecordDto>>(inputRecords));
            }
        }

        return returnData;
    }

    public async Task<PagedResultDto<EcPayReconciliationRecordDto>> GetListAsync(EcPayReconciliationRecordListInput input, CancellationToken cancellationToken = default)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var queryable = await _reconciliationRecordRepository.GetQueryableAsync();

            return new PagedResultDto<EcPayReconciliationRecordDto>
            {
                TotalCount = await queryable.LongCountAsync(cancellationToken: cancellationToken),
                Items = ObjectMapper.Map<List<EcPayReconciliationRecord>, List<EcPayReconciliationRecordDto>>(
                    await queryable
                        .OrderBy(!string.IsNullOrWhiteSpace(input.Sorting) ? input.Sorting : "CreationTime DESC")
                        .PageBy(input.SkipCount, input.MaxResultCount)
                        .ToListAsync(cancellationToken: cancellationToken)
                )
            };
        }
    }
}