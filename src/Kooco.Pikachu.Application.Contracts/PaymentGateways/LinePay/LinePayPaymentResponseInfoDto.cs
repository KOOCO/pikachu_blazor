namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayPaymentResponseInfoDto
{
    public long TransactionId { get; set; }
    public string PaymentAccessToken { get; set; }
    public LinePayPaymentResponseUrlDto PaymentUrl { get; set; }
}

public class LinePayPaymentResponseUrlDto
{
    public string? Web { get; set; }
    public string? App { get; set; }
}