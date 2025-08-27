using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeConsts
{
    public static IReadOnlyList<PaymentMethods> CreditCardOptions =
    [
        PaymentMethods.CreditCard,
        PaymentMethods.CreditCard3,
        PaymentMethods.CreditCard6,
        PaymentMethods.CreditCard12,
        PaymentMethods.CreditCard18,
        PaymentMethods.CreditCard24
    ];

    public static IReadOnlyList<PaymentMethods> OtherMethods =
    [
        PaymentMethods.EcPayVirtualBankTransfer,
        PaymentMethods.CashOnDelivery
    ];

    public static IReadOnlyList<PaymentMethods> TCatOptions =
    [
        PaymentMethods.CashOnDelivery
    ];
}
