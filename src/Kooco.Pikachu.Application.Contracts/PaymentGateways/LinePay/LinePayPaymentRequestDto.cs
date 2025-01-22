using System.Collections.Generic;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayPaymentRequestDto
{
    public int Amount { get; set; }
    public string Currency { get; set; }
    public string OrderId { get; set; }
    public List<LinePayPaymentRequestPackageDto> Packages { get; set; }
    public LinePayPaymentRequestRedirectUrlDto RedirectUrls { get; set; }
}

public class LinePayPaymentRequestPackageDto
{
    public string Id { get; set; }
    public int Amount { get; set; }
    public string? Name { get; set; }
    public List<LinePayPaymentRequestProductDto> Products { get; set; }
}

public class LinePayPaymentRequestProductDto
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int OriginalPrice { get; set; }
}

public class LinePayPaymentRequestRedirectUrlDto
{
    public string ConfirmUrl { get; set; }
    public string CancelUrl { get; set; }
}
