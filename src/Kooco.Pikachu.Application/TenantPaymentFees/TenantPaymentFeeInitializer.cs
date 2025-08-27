using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeInitializer
{
    private static readonly HashSet<(PaymentFeeType, PaymentMethods)> BaseFeeMethods =
    [
        (PaymentFeeType.EcPay, PaymentMethods.CreditCard)
    ];

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
            if (!init.Any(x => x.FeeType == feeType &&
                               x.FeeSubType == method.FeeSubType &&
                               x.PaymentMethod == method.Method))
            {
                init.Add(CreateDto(feeType, method.FeeSubType, method.Method));
            }
        }

        return init;
    }

    private static TenantPaymentFeeDto CreateDto(
        PaymentFeeType feeType,
        PaymentFeeSubType feeSubType,
        PaymentMethods paymentMethod
        )
    {
        return new()
        {
            FeeType = feeType,
            FeeSubType = feeSubType,
            PaymentMethod = paymentMethod,
            FeeKind = FeeKind.Percentage,
            Amount = 0,
            IsBaseFee = BaseFeeMethods.Contains((feeType, paymentMethod))
        };
    }
}
