using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeInitializer
{
    public static List<TenantPaymentFeeDto> Init()
    {
        var init = new List<TenantPaymentFeeDto>();
        InitByType(init, PaymentFeeType.EcPay);
        InitByType(init, PaymentFeeType.TCat);
        return init;
    }

    public static List<TenantPaymentFeeDto> InitByType(List<TenantPaymentFeeDto> init, PaymentFeeType feeType)
    {
        foreach (var method in TenantPaymentFeeMap.AllowedCombinations
                     .Where(x => x.Key.FeeType == feeType)
                     .SelectMany(x => x.Value.Select(method => new { x.Key.FeeSubType, Method = method })))
        {
            if (method.Method == PaymentMethods.CreditCard)
            {
                if (!init.Any(x => x.FeeType == feeType &&
                                   x.FeeSubType == method.FeeSubType &&
                                   x.PaymentMethod == method.Method &&
                                   x.IsBaseFee == true))
                {
                    init.Add(CreateDto(feeType, method.FeeSubType, method.Method, true));
                }

                if (!init.Any(x => x.FeeType == feeType &&
                                   x.FeeSubType == method.FeeSubType &&
                                   x.PaymentMethod == method.Method &&
                                   x.IsBaseFee == false))
                {
                    init.Add(CreateDto(feeType, method.FeeSubType, method.Method, isBaseFee: false));
                }
            }
            else
            {
                if (!init.Any(x => x.FeeType == feeType &&
                                   x.FeeSubType == method.FeeSubType &&
                                   x.PaymentMethod == method.Method))
                {
                    init.Add(CreateDto(feeType, method.FeeSubType, method.Method));
                }
            }
        }

        return SortByPredefinedOrder(init, feeType);
    }

    private static List<TenantPaymentFeeDto> SortByPredefinedOrder(List<TenantPaymentFeeDto> fees, PaymentFeeType feeType)
    {
        var orderedResult = new List<TenantPaymentFeeDto>();

        foreach (var combination in TenantPaymentFeeMap.AllowedCombinations
                     .Where(x => x.Key.FeeType == feeType))
        {
            var feeSubType = combination.Key.FeeSubType;

            foreach (var paymentMethod in combination.Value)
            {
                var matchingFees = fees.Where(f => f.FeeType == feeType &&
                                                  f.FeeSubType == feeSubType &&
                                                  f.PaymentMethod == paymentMethod)
                                       .OrderBy(f => f.IsBaseFee ? 0 : 1)
                                       .ToList();

                orderedResult.AddRange(matchingFees);
            }
        }

        return orderedResult;
    }

    private static TenantPaymentFeeDto CreateDto(
        PaymentFeeType feeType,
        PaymentFeeSubType feeSubType,
        PaymentMethods paymentMethod,
        bool isBaseFee = false
        )
    {
        return new()
        {
            FeeType = feeType,
            FeeSubType = feeSubType,
            PaymentMethod = paymentMethod,
            FeeKind = FeeKind.Percentage,
            Amount = 0,
            IsBaseFee = isBaseFee
        };
    }
}
