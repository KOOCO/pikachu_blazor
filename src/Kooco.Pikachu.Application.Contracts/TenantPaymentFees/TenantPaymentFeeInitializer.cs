using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.TenantPaymentFees;

public static class TenantPaymentFeeInitializer
{
    public static List<UpdateTenantPaymentFeeDto> Initialize()
    {
        var init = new List<UpdateTenantPaymentFeeDto>();

        init.AddRange(
        [
            EcPayCreditCardOption(PaymentMethods.CreditCard, true),
            EcPayCreditCardOption(PaymentMethods.CreditCard),
            EcPayCreditCardOption(PaymentMethods.CreditCard3),
            EcPayCreditCardOption(PaymentMethods.CreditCard6),
            EcPayCreditCardOption(PaymentMethods.CreditCard12),
            EcPayCreditCardOption(PaymentMethods.CreditCard18),
            EcPayCreditCardOption(PaymentMethods.CreditCard24),
            EcPayOtherMethodOption(PaymentMethods.EcPayVirtualBankTransfer),
            EcPayOtherMethodOption(PaymentMethods.ManualBankTransfer),
            EcPayOtherMethodOption(PaymentMethods.CashOnDelivery),
            TCatOption(PaymentMethods.CashOnDelivery),
        ]);

        return init;
    }

    private static UpdateTenantPaymentFeeDto EcPayCreditCardOption(PaymentMethods paymentMethod, bool isBaseFee = false)
    {
        return new()
        {
            FeeType = PaymentFeeType.EcPay,
            FeeSubType = PaymentFeeSubType.CreditCardOptions,
            PaymentMethod = paymentMethod,
            FeeKind = FeeKind.Percentage,
            Amount = 0,
            IsBaseFee = isBaseFee
        };
    }

    private static UpdateTenantPaymentFeeDto EcPayOtherMethodOption(PaymentMethods paymentMethod)
    {
        return new()
        {
            FeeType = PaymentFeeType.EcPay,
            FeeSubType = PaymentFeeSubType.OtherMethods,
            PaymentMethod = paymentMethod,
            FeeKind = FeeKind.Percentage,
            Amount = 0,
            IsBaseFee = false
        };
    }

    private static UpdateTenantPaymentFeeDto TCatOption(PaymentMethods paymentMethod)
    {
        return new()
        {
            FeeType = PaymentFeeType.TCat,
            FeeSubType = PaymentFeeSubType.TCatOptions,
            PaymentMethod = paymentMethod,
            FeeKind = FeeKind.Percentage,
            Amount = 0,
            IsBaseFee = false
        };
    }
}
