using Volo.Abp;

namespace Kooco.Pikachu.Orders;

[RemoteService(false)]
public class OrderInvoiceAppService : PikachuAppService, IOrderInvoiceAppService
{

    public required IOrderInvoiceRepository OrderInvoiceRepository { get; init; }
}