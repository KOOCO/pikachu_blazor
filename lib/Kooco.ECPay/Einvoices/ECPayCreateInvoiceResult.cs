namespace Kooco.Einvoices;
public readonly struct ECPayCreateInvoiceResult
{
    public int RtnCode { get; init; }
    public string RtnMsg { get; init; }
    public string InvoiceNo { get; init; }
    public string InvoiceDate { get; init; }
    public string RandomNumber { get; init; }
}