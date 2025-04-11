using Kooco.Parameters.Einvoices;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Kooco.Invoices.Interfaces;
public interface IECPayInvoiceService
{
    Task<(int transCode, CreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ECPayInvoiceService : IECPayInvoiceService
{
    public async Task<(int transCode, CreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV)
    {
        return await EinvoiceService.CreateInvoiceAsync(hashKey, hashIV, new CreateInvoiceInput());
    }

    public required IEinvoiceService EinvoiceService { get; init; }
}