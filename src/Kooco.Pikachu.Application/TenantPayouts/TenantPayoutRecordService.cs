using Kooco.Pikachu.CodTradeInfos;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.OrderTradeNos;
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
using Volo.Abp.ObjectMapping;
using static Kooco.Pikachu.TenantPaymentFees.TenantPaymentFeeHelper;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutRecordService : ITransientDependency
{
    private readonly IGuidGenerator _guid;
    private readonly ITenantPayoutRepository _tenantPayoutRepository;
    private readonly ITenantPaymentFeeRepository _tenantPaymentFeeRepository;
    private readonly ILogger<TenantPayoutRecordService> _logger;
    private readonly IOrderTradeNoRepository _orderTradeNoRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IObjectMapper _mapper;

    public TenantPayoutRecordService(
        IGuidGenerator guid,
        ITenantPayoutRepository tenantPayoutRepository,
        ITenantPaymentFeeRepository tenantPaymentFeeRepository,
        ILogger<TenantPayoutRecordService> logger,
        IOrderTradeNoRepository orderTradeNoRepository,
        IOrderRepository orderRepository,
        IObjectMapper mapper
        )
    {
        _guid = guid;
        _tenantPayoutRepository = tenantPayoutRepository;
        _tenantPaymentFeeRepository = tenantPaymentFeeRepository;
        _logger = logger;
        _orderTradeNoRepository = orderTradeNoRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task CreateEcPayReconciliationPayouts(List<EcPayReconciliationRecordDto> results, List<Order> orders)
    {
        var tenantPayments = await _tenantPaymentFeeRepository.GetListAsync(tp => tp.FeeType == PaymentFeeType.EcPay);

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
                PaymentFeeType.EcPay,
                order.CreationTime,
                tenantId
                );

            records.Add(payout);
        }

        await InsertPayoutRecordsAsync(records);
    }

    public async Task CreateEcPayCodPayouts(List<EcPayCodTradeInfoDto> results, List<Order> orders)
    {
        var tenantPayments = await _tenantPaymentFeeRepository.GetListAsync(tp => tp.FeeType == PaymentFeeType.EcPay);

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
                PaymentFeeType.EcPay,
                order.CreationTime,
                tenantId
                );

            records.Add(payout);
        }

        await InsertPayoutRecordsAsync(records);
    }

    public async Task<List<TCatCodTradeInfoRecordDto>> CreateTCatCodPayouts(List<TCatCodTradeInfoRecordDto> results)
    {
        var merchantTradeNos = results.Where(r => !string.IsNullOrWhiteSpace(r.MerchantTradeNo)).Select(r => r.MerchantTradeNo).Distinct().ToList();
        var orderTradeNos = await _orderTradeNoRepository.GetListAsync(ot => merchantTradeNos.Contains(ot.MarchentTradeNo));
        var orderTradeNosLookup = orderTradeNos.ToDictionary(x => x.MarchentTradeNo);

        var orderIds = orderTradeNos
            .Select(ot => ot.OrderId)
            .Distinct()
            .ToList();

        var orders = await _orderRepository.GetListAsync(o => orderIds.Contains(o.Id));

        var tenantPayments = await _tenantPaymentFeeRepository.GetListAsync(tp => tp.FeeType == PaymentFeeType.TCat);

        var combinations = tenantPayments
            .ToDictionary(tp => (tp.TenantId, tp.PaymentMethod, tp.IsBaseFee), tp => tp);

        var orderLookup = orders.ToDictionary(o => o.Id);

        foreach (var result in results)
        {
            if (!orderTradeNosLookup.TryGetValue(result.MerchantTradeNo, out var tradeNo))
            {
                _logger.LogWarning("Order Trade No not found for Merchant Trade No: {merchantTradeNo}", result.MerchantTradeNo);
                continue;
            }

            result.OrderId = tradeNo.OrderId;

            if (!orderLookup.TryGetValue(result.OrderId, out var order) || !order.PaymentMethod.HasValue)
            {
                _logger.LogWarning("Order not found or PaymentMethod is null for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            result.OrderId = order.Id;
            result.OrderNo = order.OrderNo;
            result.OrderDate = order.CreationTime;
            result.TenantId = order.TenantId;
            var paymentMethod = order.PaymentMethod.Value;

            if (!result.TenantId.HasValue)
            {
                _logger.LogWarning("TenantId missing for Order No: {orderNo}", result.OrderNo);
                continue;
            }

            var tenantId = result.TenantId.Value;

            var minimumHandlingFees = GetMinimumHandlingFee(paymentMethod);

            var totalAmount = order.TotalAmount;

            var (feeRate, handlingFee) = (0m, 0m);

            combinations.TryGetValue((tenantId, paymentMethod, false), out var tenantCombination);

            if (tenantCombination != null && tenantCombination.IsEnabled)
            {
                var isPercentage = tenantCombination.FeeKind == FeeKind.Percentage;
                var amount = tenantCombination.Amount;

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
            }

            result.FeeRate = feeRate;
            result.HandlingFee = handlingFee;
            result.NetAmount = (result.CODAmount ?? 0) - handlingFee;

            var payout = new TenantPayoutRecord(
                _guid.Create(),
                result.OrderId,
                result.OrderNo,
                paymentMethod,
                tenantCombination?.FeeKind,
                order.TotalAmount,
                feeRate,
                handlingFee,
                0,
                PaymentFeeType.TCat,
                order.CreationTime,
                tenantId
                );

            result.PayoutRecord = _mapper.Map<TenantPayoutRecord, TenantPayoutRecordDto>(payout);
        }

        return [.. results.Where(r => r.OrderId != Guid.Empty)];
    }

    public async Task InsertPayoutRecordsAsync(List<TenantPayoutRecord> records)
    {
        await _tenantPayoutRepository.InsertManyAsync(records);
    }
}
