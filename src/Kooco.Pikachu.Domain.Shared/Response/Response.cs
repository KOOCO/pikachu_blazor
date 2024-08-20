using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Response;

public class Response
{
}

public class OrderPaymentMethodRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public string? MerchantTradeNo { get; set; }
}