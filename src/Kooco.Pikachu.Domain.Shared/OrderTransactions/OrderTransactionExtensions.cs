using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.OrderTransactions;

public static class OrderTransactionExtensions
{
    public static PaymentChannel? GetPaymentChannel(this PaymentMethods paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethods.CreditCard => PaymentChannel.EcPay,
            PaymentMethods.EcPayVirtualBankTransfer => PaymentChannel.EcPay,
            PaymentMethods.LinePay => PaymentChannel.LinePay,
            PaymentMethods.CashOnDelivery => PaymentChannel.CashOnDelivery,
            _ => null
        };
    }
}
