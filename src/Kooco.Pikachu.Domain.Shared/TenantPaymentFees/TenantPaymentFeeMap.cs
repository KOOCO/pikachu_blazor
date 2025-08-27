using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeMap
{
    public static readonly IReadOnlyDictionary<(PaymentFeeType FeeType, PaymentFeeSubType FeeSubType), IReadOnlyList<PaymentMethods>> AllowedCombinations
        = new Dictionary<(PaymentFeeType, PaymentFeeSubType), IReadOnlyList<PaymentMethods>>
        {
            [(PaymentFeeType.EcPay, PaymentFeeSubType.CreditCardOptions)] =
            [
                PaymentMethods.CreditCard,
                PaymentMethods.CreditCard3,
                PaymentMethods.CreditCard6,
                PaymentMethods.CreditCard12,
                PaymentMethods.CreditCard18,
                PaymentMethods.CreditCard24
            ],

            [(PaymentFeeType.EcPay, PaymentFeeSubType.OtherMethods)] =
            [
                PaymentMethods.EcPayVirtualBankTransfer,
                PaymentMethods.CashOnDelivery
            ],

            [(PaymentFeeType.TCat, PaymentFeeSubType.TCatOptions)] =
            [
                PaymentMethods.CashOnDelivery
            ],
        };

    public static IReadOnlyList<PaymentMethods> GetMethods(PaymentFeeType feeType, PaymentFeeSubType feeSubType)
    {
        return AllowedCombinations.TryGetValue((feeType, feeSubType), out var methods) ? methods : [];
    }

    public static bool IsCombinationAllowed(PaymentFeeType feeType, PaymentFeeSubType feeSubType, PaymentMethods method)
    {
        return AllowedCombinations.TryGetValue((feeType, feeSubType), out var methods) && methods.Contains(method);
    }
}
