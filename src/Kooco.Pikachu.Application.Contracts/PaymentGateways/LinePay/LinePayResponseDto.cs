namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayResponseDto<T>
{
    public bool IsSuccessful { get; set; }
    public string ReturnCode { get; set; }
    public string ReturnMessage { get; set; }
    public T Info { get; set; }
}
