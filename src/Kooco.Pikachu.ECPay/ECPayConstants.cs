namespace Kooco.Pikachu;
public readonly ref struct ECPayConstants
{
    public const string Name = "ECPay";
    public const string MediaType = "application/json";

    public readonly ref struct Einvoice
    {
        public const string FormalUrl = "https://einvoice.ecpay.com.tw";
        public const string TestUrl = "https://einvoice-stage.ecpay.com.tw";
        public const string B2CInvoicePath = "/B2CInvoice/Issue";
    }
}