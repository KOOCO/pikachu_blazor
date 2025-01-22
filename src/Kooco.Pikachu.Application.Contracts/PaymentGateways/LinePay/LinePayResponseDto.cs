namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayResponseDto<TInfo>
{
    public string ReturnCode { get; set; }
    public string ReturnMessage { get; set; }
    public TInfo Info { get; set; }
}
