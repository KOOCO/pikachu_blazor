using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.OrderTransactions;

/// <summary>
/// Extensions for OrderTransaction operations
/// Refactored to use dictionary lookup instead of switch statements for better maintainability
/// </summary>
public static class OrderTransactionExtensions
{
    /// <summary>
    /// Payment method to channel mapping
    /// This replaces the hard-coded switch statement with a configurable mapping
    /// </summary>
    private static readonly Dictionary<PaymentMethods, PaymentChannel> PaymentChannelMapping = new()
    {
        { PaymentMethods.CreditCard, PaymentChannel.EcPay },
        { PaymentMethods.BankTransfer, PaymentChannel.EcPay },
        { PaymentMethods.LinePay, PaymentChannel.LinePay },
        { PaymentMethods.CashOnDelivery, PaymentChannel.CashOnDelivery }
    };
    
    /// <summary>
    /// Gets the payment channel for a given payment method
    /// Uses dictionary lookup instead of switch statement for better extensibility
    /// </summary>
    /// <param name="paymentMethod">Payment method to get channel for</param>
    /// <returns>Payment channel if found, null otherwise</returns>
    public static PaymentChannel? GetPaymentChannel(this PaymentMethods paymentMethod)
    {
        return PaymentChannelMapping.TryGetValue(paymentMethod, out var channel) ? channel : null;
    }
    
    /// <summary>
    /// Gets all supported payment methods and their channels
    /// </summary>
    /// <returns>Dictionary of payment methods and their channels</returns>
    public static Dictionary<PaymentMethods, PaymentChannel> GetAllPaymentChannels()
    {
        return new Dictionary<PaymentMethods, PaymentChannel>(PaymentChannelMapping);
    }
    
    /// <summary>
    /// Checks if a payment method is supported
    /// </summary>
    /// <param name="paymentMethod">Payment method to check</param>
    /// <returns>True if supported, false otherwise</returns>
    public static bool IsPaymentMethodSupported(this PaymentMethods paymentMethod)
    {
        return PaymentChannelMapping.ContainsKey(paymentMethod);
    }
}
