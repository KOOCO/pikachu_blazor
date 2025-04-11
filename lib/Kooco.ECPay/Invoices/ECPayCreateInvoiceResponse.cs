namespace Kooco.Invoices;
public readonly struct ECPayCreateInvoiceResponse
{
    public int PlatformID { get; init; }
    public int MerchantID { get; init; }
    public HeaderInfo RpHeader { get; init; }
    public int TransCode { get; init; }
    public string TransMsg { get; init; }
    public string Data { get; init; }

    public readonly struct HeaderInfo
    {
        public long Timestamp { get; init; }
        public string RqID { get; init; }
        public string Revision { get; init; }
    }
}