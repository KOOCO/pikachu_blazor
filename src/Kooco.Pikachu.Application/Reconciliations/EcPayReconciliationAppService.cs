using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.TenantPayouts;
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
    private readonly IRepository<EcPayReconciliationRecord, Guid> _reconciliationRecordRepository;
    private readonly TenantPayoutRecordService _tenantPayoutRecordService;

    public EcPayReconciliationAppService(
        IEcPayReconciliationService reconciliationService,
        IOrderRepository orderRepository,
        IRepository<EcPayReconciliationRecord, Guid> reconciliationRecordRepository,
        TenantPayoutRecordService tenantPayoutRecordService
        )
    {
        _reconciliationService = reconciliationService;
        _orderRepository = orderRepository;
        _reconciliationRecordRepository = reconciliationRecordRepository;
        _tenantPayoutRecordService = tenantPayoutRecordService;
    }

    [AllowAnonymous]
    public async Task<List<EcPayReconciliationRecordDto>> QueryMediaFileAsync(CancellationToken cancellationToken = default)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            var records = await _reconciliationService.QueryMediaFileAsync(cancellationToken);

            if (records == null || records.Count == 0)
            {
                Logger.LogWarning("Reconciliation Job: No reconciliation records found for the specified date range.");
                return [];
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
            var ordersToUpdate = new List<Order>();

            foreach (var record in records)
            {
                var order = orders.FirstOrDefault(o => o.MerchantTradeNo == record.MerchantTradeNo);

                if (order == null)
                {
                    Logger.LogWarning("Reconciliation Job: No order found for MerchantTradeNo: {MerchantTradeNo}", record.MerchantTradeNo);
                    continue;
                }

                if (record.PaymentStatus == null || !record.PaymentStatus.Contains("已付款"))
                {
                    Logger.LogInformation("Reconciliation Job: Skipping merchant trade no: {merchantTradeNo} due to payment status: {paymentStatus}", record.MerchantTradeNo, record.PaymentStatus);
                    continue;
                }

                if (record.PayoutStatus == null || !record.PayoutStatus.Contains("已撥款"))
                {
                    Logger.LogInformation("Reconciliation Job: Skipping merchant trade no: {merchantTradeNo} due to payout status: {payoutStatus}", record.MerchantTradeNo, record.PayoutStatus);
                    continue;
                }

                var inputRecord = ObjectMapper.Map<EcPayReconciliationResponse, EcPayReconciliationRecord>(record);
                inputRecord.TenantId = order.TenantId;
                inputRecord.OrderId = order.Id;
                inputRecord.OrderNo = order.OrderNo;
                inputRecord.CreationTime = Clock.Now;
                inputRecords.Add(inputRecord);

                order.EcPayNetAmount = record.NetAmount;
                ordersToUpdate.Add(order);
            }

            await _reconciliationRecordRepository.InsertManyAsync(inputRecords, cancellationToken: cancellationToken);
            Logger.LogInformation("Reconciliation Job: Insert {count} records into EcPayReconciliationRecord", inputRecords.Count);

            await _orderRepository.UpdateManyAsync(ordersToUpdate, cancellationToken: cancellationToken);
            Logger.LogInformation("Reconciliation Job: Updated {count} orders", ordersToUpdate.Count);

            var results = ObjectMapper.Map<List<EcPayReconciliationRecord>, List<EcPayReconciliationRecordDto>>(inputRecords);
            
            await _tenantPayoutRecordService.CreateTenantReconciliationPayouts(results, orders, PaymentFeeType.EcPay);

            return results;
        }
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