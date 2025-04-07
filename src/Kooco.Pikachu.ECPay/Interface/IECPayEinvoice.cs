using Kooco.Pikachu.Constants;
using Kooco.Pikachu.Parameters;
using Refit;

namespace Kooco.Pikachu.Interface;
public interface IECPayEinvoice
{
    /// <summary>
    /// 發票開立
    /// </summary>
    [Post(ECPayConstants.Einvoice.B2CInvoicePath)]
    Task<ApiResponse<EinvoiceResponse>> CreateB2CInvoiceAsync([Body] EinvoiceRequest import);
}