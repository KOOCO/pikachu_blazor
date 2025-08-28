using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeHelper
{
    public static readonly IReadOnlyList<PaymentMethods> CreditCardOptions =
    [
        PaymentMethods.CreditCard,
        PaymentMethods.CreditCard3,
        PaymentMethods.CreditCard6,
        PaymentMethods.CreditCard12,
        PaymentMethods.CreditCard18,
        PaymentMethods.CreditCard24
    ];

    public static readonly IReadOnlyList<PaymentMethods> OtherMethods =
    [
        PaymentMethods.EcPayVirtualBankTransfer,
        PaymentMethods.CashOnDelivery
    ];

    public static readonly IReadOnlyList<PaymentMethods> TCatOptions =
    [
        PaymentMethods.CashOnDelivery
    ];

    public static readonly Dictionary<PaymentMethods, decimal> MinimumHandlingFees = new()
    {
        { PaymentMethods.CreditCard, 5.00m },
        { PaymentMethods.CreditCard3, 5.00m },
        { PaymentMethods.CreditCard6, 5.00m },
        { PaymentMethods.CreditCard12, 5.00m },
        { PaymentMethods.CreditCard18, 5.00m },
        { PaymentMethods.CreditCard24, 5.00m },
        { PaymentMethods.EcPayVirtualBankTransfer, 15.00m },
        { PaymentMethods.ManualBankTransfer, 15.00m },
        { PaymentMethods.CashOnDelivery, 3.00m }
    };

    public static decimal GetMinimumHandlingFee(PaymentMethods method)
    {
        return MinimumHandlingFees.TryGetValue(method, out var fee) ? fee : 0m;
    }

    public static bool IsCreditCardMethod(PaymentMethods? method)
    {
        return method.HasValue && CreditCardOptions.Contains(method.Value);
    }
}