namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayPaymentResponseDto
{
    public string ReturnCode { get; set; }
    public string ReturnMessage { get; set; }
    public LinePayPaymentResponseInfoDto Info { get; set; }
}

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