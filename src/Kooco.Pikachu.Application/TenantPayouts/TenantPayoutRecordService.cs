using Kooco.Pikachu.CodTradeInfos;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Reconciliations;
using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using static Kooco.Pikachu.TenantPaymentFees.TenantPaymentFeeHelper;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutRecordService : ITransientDependency
{
    private readonly IGuidGenerator _guid;
    private readonly IRepository<TenantPayoutRecord, Guid> _tenantPayoutRecordRepository;
    private readonly ITenantPaymentFeeRepository _tenantPaymentFeeRepository;
    private readonly ILogger<TenantPayoutRecordService> _logger;

    public TenantPayoutRecordService(
        IGuidGenerator guid,
        IRepository<TenantPayoutRecord, Guid> tenantPayoutRecordRepository,
        ITenantPaymentFeeRepository tenantPaymentFeeRepository,
        ILogger<TenantPayoutRecordService> logger
        )
    {
        _guid = guid;
        _tenantPayoutRecordRepository = tenantPayoutRecordRepository;
        _tenantPaymentFeeRepository = tenantPaymentFeeRepository;
        _logger = logger;
    }

    public async Task CreateTenantReconciliationPayouts(List<EcPayReconciliationRecordDto> results, List<Order> orders, PaymentFeeType feeType)
    {
        var tenantPayments = await _tenantPaymentFeeRepository.GetListAsync(tp => tp.FeeType == feeType);

        var combinations = tenantPayments
            .ToDictionary(tp => (tp.TenantId, tp.PaymentMethod, tp.IsBaseFee), tp => tp);

        var orderLookup = orders.ToDictionary(o => o.Id);

        List<TenantPayoutRecord> records = [];

        foreach (var result in results)
        {
            if (!orderLookup.TryGetValue(result.OrderId, out var order) || !order.PaymentMethod.HasValue)
            {
                _logger.LogWarning("Order not found or PaymentMethod is null for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            var paymentMethod = order.PaymentMethod.Value;

            if (!result.TenantId.HasValue)
            {
                _logger.LogWarning("TenantId missing for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            var tenantId = result.TenantId.Value;

            var minimumHandlingFees = GetMinimumHandlingFee(paymentMethod);

            var totalAmount = order.TotalAmount;

            var feeRate = decimal.TryParse(result.FeeRate?.Trim('%'), out var rate) ? rate : 0;

            var (handlingFee, processingFee) = (result.HandlingFee, result.ProcessingFee);

            combinations.TryGetValue((tenantId, paymentMethod, false), out var tenantCombination);

            if (tenantCombination != null && tenantCombination.IsEnabled)
            {
                var isPercentage = tenantCombination.FeeKind == FeeKind.Percentage;
                var amount = tenantCombination.Amount;
                bool isCreditCardMethod = IsCreditCardMethod(paymentMethod);

                if (isPercentage)
                {
                    feeRate = amount;
                    handlingFee = Math.Max(totalAmount * amount / 100m, minimumHandlingFees);
                }
                else
                {
                    feeRate = 0m;
                    handlingFee = amount;
                }

                if (isCreditCardMethod)
                {
                    combinations.TryGetValue((tenantId, PaymentMethods.CreditCard, true), out var baseFeeSetting);
                    if (baseFeeSetting != null && baseFeeSetting.IsEnabled && baseFeeSetting.IsBaseFee)
                    {
                        processingFee = baseFeeSetting.FeeKind == FeeKind.Percentage
                            ? (totalAmount * baseFeeSetting.Amount / 100m)
                            : baseFeeSetting.Amount;
                    }
                }
            }

            var payout = new TenantPayoutRecord(
                _guid.Create(),
                result.OrderId,
                result.OrderNo,
                paymentMethod,
                tenantCombination?.FeeKind,
                order.TotalAmount,
                feeRate,
                handlingFee,
                processingFee,
                tenantId
                );

            records.Add(payout);
        }

        await _tenantPayoutRecordRepository.InsertManyAsync(records);
    }

    public async Task CreateTenantCodPayouts(List<EcPayCodTradeInfoDto> results, List<Order> orders, PaymentFeeType feeType)
    {
        var tenantPayments = await _tenantPaymentFeeRepository.GetListAsync(tp => tp.FeeType == feeType);

        var combinations = tenantPayments
            .ToDictionary(tp => (tp.TenantId, tp.PaymentMethod, tp.IsBaseFee), tp => tp);

        var orderLookup = orders.ToDictionary(o => o.Id);

        List<TenantPayoutRecord> records = [];

        foreach (var result in results)
        {
            if (!orderLookup.TryGetValue(result.OrderId, out var order) || !order.PaymentMethod.HasValue)
            {
                _logger.LogWarning("Order not found or PaymentMethod is null for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            var paymentMethod = order.PaymentMethod.Value;

            if (!result.TenantId.HasValue)
            {
                _logger.LogWarning("TenantId missing for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            var tenantId = result.TenantId.Value;

            var minimumHandlingFees = GetMinimumHandlingFee(paymentMethod);

            var totalAmount = order.TotalAmount;

            var (feeRate, handlingFee, processingFee) = (result.CollectionChargeFee, result.HandlingCharge, 0m);

            combinations.TryGetValue((tenantId, paymentMethod, false), out var tenantCombination);

            if (tenantCombination != null && tenantCombination.IsEnabled)
            {
                var isPercentage = tenantCombination.FeeKind == FeeKind.Percentage;
                var amount = tenantCombination.Amount;
                bool isCreditCardMethod = IsCreditCardMethod(paymentMethod);

                if (isPercentage)
                {
                    feeRate = amount;
                    handlingFee = Math.Max(totalAmount * amount / 100m, minimumHandlingFees);
                }
                else
                {
                    feeRate = 0m;
                    handlingFee = amount;
                }

                if (isCreditCardMethod)
                {
                    combinations.TryGetValue((tenantId, PaymentMethods.CreditCard, true), out var baseFeeSetting);
                    if (baseFeeSetting != null && baseFeeSetting.IsEnabled && baseFeeSetting.IsBaseFee)
                    {
                        processingFee = baseFeeSetting.FeeKind == FeeKind.Percentage
                            ? (totalAmount * baseFeeSetting.Amount / 100m)
                            : baseFeeSetting.Amount;
                    }
                }
            }

            var payout = new TenantPayoutRecord(
                _guid.Create(),
                result.OrderId,
                result.OrderNo,
                paymentMethod,
                tenantCombination?.FeeKind,
                order.TotalAmount,
                feeRate,
                handlingFee,
                processingFee,
                tenantId
                );

            records.Add(payout);
        }

        await _tenantPayoutRecordRepository.InsertManyAsync(records);
    }
}
